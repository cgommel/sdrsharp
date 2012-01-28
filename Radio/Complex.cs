using System;

namespace SDRSharp.Radio
{
    public struct Complex
    {
        public float Real;
        public float Imag;

        public Complex(float real, float imaginary)
        {
            Real = real;
            Imag = imaginary;
        }

        public Complex(Complex c)
        {
            Real = c.Real;
            Imag = c.Imag;
        }

        public static Complex ComplexAdd(Complex xy, Complex uv)
        {
            return new Complex(xy.Real + uv.Real, xy.Imag + uv.Imag);
        }

        public static Complex ComplexSubtract(Complex xy, Complex uv)
        {
            return new Complex(xy.Real - uv.Real, xy.Imag - uv.Imag);
        }

        public float Modulus()
        {
            return (float) Math.Sqrt(Real * Real + Imag * Imag);
        }

        public float Argument()
        {
            return (float) Math.Atan(Imag / Real);
        }

        public Complex Conjugate()
        {
            return new Complex(Real, -Imag);
        }

        public override string ToString()
        {
            return string.Format("real {0}, imag {1}", Real, Imag);
        }

        public static bool operator ==(Complex leftHandSide, Complex rightHandSide)
        {            
            if (leftHandSide.Real != rightHandSide.Real)
            {
                return false;
            }
            return (leftHandSide.Imag == rightHandSide.Imag);
        }

        public static bool operator !=(Complex leftHandSide, Complex rightHandSide)
        {
            if (leftHandSide.Real != rightHandSide.Real)
            {
                return true;
            }
            return (leftHandSide.Imag != rightHandSide.Imag);
        }

        public static Complex operator *(Complex a, Complex b)
        {
            return new Complex(a.Real * b.Real - a.Imag * b.Imag,
                               a.Imag * b.Real + a.Real * b.Imag);
        }

        public static Complex operator *(Complex a, float b)
        {
            return new Complex(a.Real * b, a.Imag * b);
        }

        public static Complex operator /(Complex a, Complex b)
        {
            var dn = b.Real * b.Real + b.Imag * b.Imag;
            var re = (a.Real * b.Real + a.Imag * b.Imag) / dn;
            var im = (a.Imag * b.Real - a.Real * b.Imag) / dn;
            return new Complex(re, im);
        }

        public static Complex operator /(Complex a, float b)
        {
            return new Complex(a.Real / b, a.Imag / b);
        }

        public static Complex operator ~(Complex a)
        {
            return a.Conjugate();
        }

        public static implicit operator Complex(float d)
        {
            return new Complex(d, 0);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Real.GetHashCode() * 397) ^ Imag.GetHashCode();
            }
        }

        public bool Equals(Complex obj)
        {
            return obj.Real == Real && obj.Imag == Imag;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof (Complex)) return false;
            return Equals((Complex) obj);
        }
    }
}