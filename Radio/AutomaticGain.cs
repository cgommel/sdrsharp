// Original C++ version is Copyright 2010 Moe Wheatley. All rights reserved.

using System;

namespace SDRSharp.Radio
{
    public class AutomaticGainControl
    {
        #region Constants

        //signal delay line time delay in seconds.
        //adjust to cover the impulse response time of filter
        private const double DelayTimeconst = .015;

        //Peak Detector window time delay in seconds.
        private const double WindowTimeconst = .018;

        //attack time constant in seconds
        //just small enough to let attackave charge up within the DELAY_TIMECONST time
        private const double AttackRiseTimeconst = .002;
        private const double AttackFallTimeconst = .005;

        private const double DecayRisefallRatio  = .3; //ratio between rise and fall times of Decay time constants
								//adjust for best action with SSB

        // hang timer release decay time constant in seconds
        private const double ReleaseTimeconst = .05;

        //limit output to about 3db of max
        private const double AGCOutscale = 0.7;

        #endregion

        #region Private fields

        private double[] _sigDelayBuf;
        private double[] _magBuf;
	    private double _decayAve;
        private double _attackAve;

        private double _attackRiseAlpha;
        private double _attackFallAlpha;
        private double _decayRiseAlpha;
        private double _decayFallAlpha;

        private double _fixedGain;
        private double _knee;
        private double _gainSlope;
        private double _peak;

        private int _sigDelayPtr;
        private int _magBufPos;
        private int _delaySamples;
        private int _windowSamples;
        private int _hangTime;
        private int _hangTimer;


        private double _threshold;
        private double _slopeFactor;
        private double _decay;
        private int _sampleRate;
        private bool _useHang;

        #endregion

        #region Properties

        public int SampleRate
        {
            get { return _sampleRate; }
            set
            {
                if (_sampleRate != value)
                {
                    _sampleRate = value;
                    Configure(true);
                }
            }
        }

        public bool UseHang
        {
            get { return _useHang; }
            set
            {
                _useHang = value;
                Configure(false);
            }
        }

        public double Threshold
        {
            get { return _threshold; }
            set
            {
                _threshold = value;
                Configure(false);
            }
        }

        public double Slope
        {
            get { return _slopeFactor; }
            set
            {
                _slopeFactor = value;
                Configure(false);
            }
        }

        public double Decay
        {
            get { return _decay; }
            set
            {
                _decay = value;
                Configure(false);
            }
        }

        #endregion

        private void Configure(bool resetBuffers)
        {
            if (resetBuffers)
	        {
		        _sigDelayPtr = 0;
		        _hangTimer = 0;
		        _peak = -16.0;
		        _decayAve = -5.0;
		        _attackAve = -5.0;
		        _magBufPos = 0;

                if (_sampleRate > 0)
                {
                    _delaySamples = (int) (_sampleRate * DelayTimeconst);
                    _windowSamples = (int) (_sampleRate * WindowTimeconst);

                    _sigDelayBuf = new double[_delaySamples];
                    _magBuf = new double[_windowSamples];

                    for (int i = 0; i < _windowSamples; i++)
		            {
			            _magBuf[i] = -16.0;
		            }

                    //clamp Delay samples within buffer limit
                    if (_delaySamples >= _sigDelayBuf.Length - 1)
                        _delaySamples = _sigDelayBuf.Length - 1;
                }
	        }

	        //calculate parameters for AGC gain as a function of input magnitude
	        _knee = _threshold / 20.0;
            _gainSlope = _slopeFactor / 100.0;
	        _fixedGain = AGCOutscale * Math.Pow(10.0, _knee * (_gainSlope - 1.0));	//fixed gain value used below knee threshold

	        //calculate fast and slow filter values.
	        _attackRiseAlpha = (1.0 - Math.Exp(-1.0 / (_sampleRate*AttackRiseTimeconst)));
	        _attackFallAlpha = (1.0 - Math.Exp(-1.0 / (_sampleRate*AttackFallTimeconst)));

	        _decayRiseAlpha = (1.0 - Math.Exp(-1.0 / (_sampleRate * Decay*.001*DecayRisefallRatio)));	//make rise time DECAY_RISEFALL_RATIO of fall
	        _hangTime = (int)(_sampleRate * Decay * .001);

	        if (_useHang)
                _decayFallAlpha = (1.0 - Math.Exp(-1.0 / (_sampleRate * ReleaseTimeconst)));
	        else
                _decayFallAlpha = (1.0 - Math.Exp(-1.0 / (_sampleRate * Decay * .001)));
        }

        public void Process(double[] buffer)
        {
            for (var i = 0; i<buffer.Length; i++)
		    {
			    var sample = buffer[i];	//get latest input sample

                if (sample == 0.0)
                {
                    continue;
                }

		        sample *= 1000.0;

			    //Get delayed sample of input signal
			    var delayedin = _sigDelayBuf[_sigDelayPtr];
			    //put new input sample into signal delay buffer
			    _sigDelayBuf[_sigDelayPtr++] = sample;
			    if (_sigDelayPtr >= _delaySamples)	//deal with delay buffer wrap around
				    _sigDelayPtr = 0;

			    //convert |mag| to log |mag|
			    var mag = Math.Log10(Math.Abs(sample));
                if (double.IsNaN(mag) || double.IsInfinity(mag))
                {
                    mag = -8.0;
                }

			    //create a sliding window of '_WindowSamples' magnitudes and output the peak value within the sliding window
			    var tmp = _magBuf[_magBufPos];	//get oldest mag sample from buffer into tmp
			    _magBuf[_magBufPos++] = mag;			//put latest mag sample in buffer;
			    if (_magBufPos >= _windowSamples)		//deal with magnitude buffer wrap around
				    _magBufPos = 0;
			    if (mag > _peak)
			    {
				    _peak = mag;	//if new sample is larger than current peak then use it, no need to look at buffer values
			    }
			    else
			    {
				    if (tmp == _peak)		//tmp is oldest sample pulled out of buffer
				    {	//if oldest sample pulled out was last peak then need to find next highest peak in buffer
                        _peak = -8.0;		//set to lowest value to find next max peak
					    //search all buffer for maximum value and set as new peak
					    for (var j = 0; j < _windowSamples; j++)
					    {
						    tmp = _magBuf[j];
						    if (tmp > _peak)
							    _peak = tmp;
					    }
				    }
			    }

			    if (UseHang)
			    {	//using hang timer mode
				    if (_peak>_attackAve)	//if magnitude is rising (use _AttackRiseAlpha time constant)
					    _attackAve = (1.0 - _attackRiseAlpha) * _attackAve + _attackRiseAlpha * _peak;
				    else					//else magnitude is falling (use  _AttackFallAlpha time constant)
					    _attackAve = (1.0 - _attackFallAlpha) * _attackAve + _attackFallAlpha * _peak;

				    if (_peak>_decayAve)	//if magnitude is rising (use _DecayRiseAlpha time constant)
				    {
					    _decayAve = (1.0 - _decayRiseAlpha) * _decayAve + _decayRiseAlpha * _peak;
					    _hangTimer = 0;	//reset hang timer
				    }
				    else
				    {	//here if decreasing signal
					    if (_hangTimer < _hangTime)
						    _hangTimer++;	//just inc and hold current _DecayAve
					    else	//else decay with _DecayFallAlpha which is RELEASE_TIMECONST
						    _decayAve = (1.0 - _decayFallAlpha) * _decayAve + _decayFallAlpha * _peak;
				    }
			    }
			    else
			    {	//using exponential decay mode
				    // perform average of magnitude using 2 averagers each with separate rise and fall time constants
				    if(_peak>_attackAve)	//if magnitude is rising (use _AttackRiseAlpha time constant)
					    _attackAve = (1.0 - _attackRiseAlpha) * _attackAve + _attackRiseAlpha * _peak;
				    else					//else magnitude is falling (use  _AttackFallAlpha time constant)
					    _attackAve = (1.0 - _attackFallAlpha) * _attackAve + _attackFallAlpha * _peak;

				    if(_peak>_decayAve)	//if magnitude is rising (use _DecayRiseAlpha time constant)
					    _decayAve = (1.0 - _decayRiseAlpha) * _decayAve + _decayRiseAlpha * _peak;
				    else					//else magnitude is falling (use _DecayFallAlpha time constant)
					    _decayAve = (1.0 - _decayFallAlpha) * _decayAve + _decayFallAlpha * _peak;
			    }

			    //use greater magnitude of attack or Decay Averager
			    mag = _attackAve > _decayAve ? _attackAve : _decayAve;

			    //calc gain depending on which side of knee the magnitude is on
		        double gain;
		        if (mag <= _knee)		//use fixed gain if below knee
				    gain = _fixedGain;
			    else				//use variable gain if above knee
				    gain = AGCOutscale * Math.Pow(10.0, mag * (_gainSlope - 1.0));

			    buffer[i] = delayedin * gain * 0.00001;
		    }
        }
    }
}
