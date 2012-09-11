using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Runtime.InteropServices;
using System;

namespace SDRSharp.Radio
{
    public unsafe static class Utils
    {
        public static float FastSin(float x)
        {
            float sin;

            if (x < 0)
            {
                sin = 1.27323954f * x + .405284735f * x * x;

                if (sin < 0)
                    sin = .225f * (sin * -sin - sin) + sin;
                else
                    sin = .225f * (sin * sin - sin) + sin;
            }
            else
            {
                sin = 1.27323954f * x - 0.405284735f * x * x;

                if (sin < 0)
                    sin = .225f * (sin * -sin - sin) + sin;
                else
                    sin = .225f * (sin * sin - sin) + sin;
            }

            return sin;
        }

        public static float FastCos(float x)
        {
            float cos;
            x += 1.57079632f;
            if (x > 3.14159265f)
                x -= 6.28318531f;

            if (x < 0)
            {
                cos = 1.27323954f * x + 0.405284735f * x * x;

                if (cos < 0)
                    cos = .225f * (cos * -cos - cos) + cos;
                else
                    cos = .225f * (cos * cos - cos) + cos;
            }
            else
            {
                cos = 1.27323954f * x - 0.405284735f * x * x;

                if (cos < 0)
                    cos = .225f * (cos * -cos - cos) + cos;
                else
                    cos = .225f * (cos * cos - cos) + cos;
            }

            return cos;
        }

        public static float FastAtan2(float y, float x)
        {
            const float PI = (float) Math.PI;
            const float PI2 = (float) (Math.PI / 2.0);

            float angle;
            if (x == 0.0)
            {
                if (y > 0.0)
                    return PI2;
                if (y == 0.0)
                    return 0.0f;
                return -PI2;
            }
            float z = y / x;
            if (Math.Abs(z) < 1.0)
            {
                angle = z / (1.0f + 0.2854f * z * z);
                if (x < 0.0)
                {
                    if (y < 0.0)
                        return angle - PI;
                    return angle + PI;
                }
            }
            else
            {
                angle = PI2 - z / (z * z + 0.2854f);
                if (y < 0.0)
                    return angle - PI;
            }
            return angle;
        }

        public static void ManagedMemcpy(void* dest, void* src, int len)
        {
            var d = (byte*) dest;
            var s = (byte*) src;
            if (len >= 16) 
            {
                do
                {
                    ((int*)d)[0] = ((int*)s)[0];
                    ((int*)d)[1] = ((int*)s)[1]; 
                    ((int*)d)[2] = ((int*)s)[2];
                    ((int*)d)[3] = ((int*)s)[3];
                    d += 16; 
                    s += 16;
                } while ((len -= 16) >= 16); 
            }
            if(len > 0)
            { 
                if ((len & 8) != 0)
                {
                    ((int*)d)[0] = ((int*)s)[0]; 
                    ((int*)d)[1] = ((int*)s)[1]; 
                    d += 8; 
                    s += 8;
               }
               if ((len & 4) != 0)
               { 
                    ((int*)d)[0] = ((int*)s)[0];
                    d += 4; 
                    s += 4; 
               }
               if ((len & 2) != 0) 
               {
                    ((short*)d)[0] = ((short*)s)[0];
                    d += 2;
                    s += 2; 
               }
               if ((len & 1) != 0) 
                    *d = *s; 
            }
        }

#if LINUX
        private const string Libc = "libc.so";
#else
        private const string Libc = "msvcrt.dll";

        [DllImport(Libc, EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl)]
        public static extern void* Memcpy(void* dest, void* src, int len);

        [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod", SetLastError = true)]
        public static extern uint TimeBeginPeriod(uint uMilliseconds);

        [DllImport("winmm.dll", EntryPoint = "timeEndPeriod", SetLastError = true)]
        public static extern uint TimeEndPeriod(uint uMilliseconds);
#endif

        public static double GetDoubleSetting(string name, double defaultValue)
        {
            var strValue = ConfigurationManager.AppSettings[name];
            double result;
            if (double.TryParse(strValue, NumberStyles.Number, CultureInfo.InvariantCulture, out result))
            {
                return result;
            }
            return defaultValue;
        }

        public static bool GetBooleanSetting(string name)
        {
            string resultString;
            try
            {
                resultString = ConfigurationManager.AppSettings[name] ?? string.Empty;
            }
            catch
            {
                return false;
            }
            resultString += " ";
            return "YyTt".IndexOf(resultString[0]) >= 0;
        }

        public static Color GetColorSetting(string name, Color defaultColor)
        {
            Color result;
            try
            {
                var colorPattern = ConfigurationManager.AppSettings[name];

                var r = int.Parse(colorPattern.Substring(0, 2), NumberStyles.HexNumber);
                var g = int.Parse(colorPattern.Substring(2, 2), NumberStyles.HexNumber);
                var b = int.Parse(colorPattern.Substring(4, 2), NumberStyles.HexNumber);
                result = Color.FromArgb(r, g, b);
            }
            catch
            {
                return defaultColor;
            }
            return result;
        }

        public static ColorBlend GetGradientBlend(int alpha, string settingName)
        {
            var colorBlend = new ColorBlend();

            string colorString;
            try
            {
                colorString = ConfigurationManager.AppSettings[settingName] ?? string.Empty;
            }
            catch
            {
                colorString = string.Empty;
            }
            var colorPatterns = colorString.Split(',');
            if (colorPatterns.Length < 2)
            {
                //colorBlend.Colors = new[] { Color.White, Color.Yellow, Color.Red, Color.FromArgb(56, 3, 2), Color.Black };
                colorBlend.Colors = new[] { Color.White, Color.LightBlue, Color.DodgerBlue, Color.FromArgb(0, 0, 80), Color.Black, Color.Black };
                //colorBlend.Colors = new[] { Color.Red, Color.Orange, Color.Yellow, Color.Lime, Color.DodgerBlue, Color.DarkBlue, Color.Black, Color.Black };
                for (var i = 0; i < colorBlend.Colors.Length; i++)
                {
                    colorBlend.Colors[i] = Color.FromArgb(alpha, colorBlend.Colors[i]);
                }
            }
            else
            {
                colorBlend.Colors = new Color[colorPatterns.Length];
                for (var i = 0; i < colorPatterns.Length; i++)
                {
                    var colorPattern = colorPatterns[i];
                    var r = int.Parse(colorPattern.Substring(0, 2), NumberStyles.HexNumber);
                    var g = int.Parse(colorPattern.Substring(2, 2), NumberStyles.HexNumber);
                    var b = int.Parse(colorPattern.Substring(4, 2), NumberStyles.HexNumber);
                    colorBlend.Colors[i] = Color.FromArgb(r, g, b);
                }
            }

            var positions = new float[colorBlend.Colors.Length];
            var distance = 1f / (positions.Length - 1);
            for (var i = 0; i < positions.Length; i++)
            {
                var r = colorBlend.Colors[i].R;
                var g = colorBlend.Colors[i].G;
                var b = colorBlend.Colors[i].B;

                colorBlend.Colors[i] = Color.FromArgb(alpha, r, g, b);
                positions[i] = i * distance;
            }
            colorBlend.Positions = positions;
            return colorBlend;
        }

        public static ColorBlend GetGradientBlend(int alpha)
        {
            return GetGradientBlend(alpha, "gradient");
        }

        public static int GetIntSetting(string name, int defaultValue)
        {
            var strValue = ConfigurationManager.AppSettings[name];
            int result;
            if (int.TryParse(strValue, out result))
            {
                return result;
            }
            return defaultValue;
        }

        public static void SaveSetting(string key, string value)
        {
            var configurationFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            configurationFile.AppSettings.Settings.Remove(key);
            configurationFile.AppSettings.Settings.Add(key, value);
            configurationFile.Save(ConfigurationSaveMode.Full);
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
