namespace SDRSharp.Radio
{
    public class IQFirFilter
    {
        private readonly FirFilter _iFilter;
        private readonly FirFilter _qFilter;

        public IQFirFilter(double[] coefficients)
        {
            _iFilter = new FirFilter(coefficients);
            _qFilter = new FirFilter(coefficients);
        }

        public void Process(Complex[] iq)
        {
            for (var i = 0; i < iq.Length; i++)
            {
                iq[i].Real = _iFilter.Process(iq[i].Real);
                iq[i].Imag = _qFilter.Process(iq[i].Imag);
            }
        }
    }
}
