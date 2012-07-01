namespace SDRSharp.PanView
{
    public sealed class PeakDetector
    {
        private const byte Threshold = 20;

        public static void GetPeaks(byte[] buffer, bool[] peaks, int windowSize)
        {
            if (windowSize < 2)
            {
                windowSize = 2;
            }
            for (var i = 0; i < buffer.Length; i++)
            {
                var isPeak = true;
                var min = byte.MaxValue;
                var max = byte.MinValue;
                for (var j = 0; j < windowSize; j++)
                {
                    var k = i + j - windowSize / 2;
                    if (k != i && k >= 0 && k < buffer.Length)
                    {
                        if (buffer[k] > buffer[i])
                        {
                            isPeak = false;
                            break;
                        }
                        if (buffer[k] == buffer[i] && i < k)
                        {
                            isPeak = false;
                            break;
                        }
                        if (buffer[k] > max)
                        {
                            max = buffer[k];
                        }
                        else if (buffer[k] < min)
                        {
                            min = buffer[k];
                        }
                    }
                }
                peaks[i] = isPeak && max - min >= Threshold;
            }
        }
    }
}
