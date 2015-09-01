using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using V22048Game.GameRules;

namespace V22048Game.Elements
{
    public struct GameValue
    {
        public static GameValue i = new GameValue(0, 1);
        public static GameValue zero = new GameValue(0, 0);
        public static GameValue unit = new GameValue(1, 0);

        double real, imaginary;
        public double Real
        {
            get { return real; }
            set { real = value; }
        }
        public double Imaginary
        {
            get { return imaginary; }
            set { imaginary = value; }
        }

        public GameValue(double real, double imaginary)
        {
            this.real = real;
            this.imaginary = imaginary;
        }

        public double Mod
        {
            get { return Math.Sqrt(Math.Pow(Real, 2) + Math.Pow(Imaginary, 2)); }
        }

        public static GameValue operator *(GameValue w, GameValue z)
        {
            return new GameValue()
            {
                real = (w.Real * z.Real) - (w.Imaginary * z.Imaginary),
                imaginary = (w.Imaginary * z.Real) + (z.Imaginary * w.Real)
            };
        }

        public static GameValue operator +(GameValue w, GameValue z)
        {
            return new GameValue()
            {
                real = w.Real + z.Real,
                imaginary = w.Imaginary + z.Imaginary
            };
        }

        public static bool operator ==(GameValue w, GameValue z)
        {
            return w.Equals(z);
        }
        public static bool operator !=(GameValue w, GameValue z)
        {
            return !w.Equals(z);
        }

        public static GameValue operator -(GameValue w)
        {
            return new GameValue()
            {
                real = -w.Real,
                imaginary = -w.Imaginary
            };
        }

        public static GameValue operator -(GameValue w, GameValue z)
        {
            return w + (-z);
        }

        public static GameValue operator /(GameValue w, double a)
        {
            if (a != 0)
            {
                return new GameValue()
                {
                    real = w.real / a,
                    imaginary = w.imaginary / a
                };
            }
            else
            {
                throw new DivideByZeroException("Numpty!");
            }
        }

        public static GameValue operator *(GameValue w, double a)
        {
            return new GameValue()
            {
                real = w.real * a,
                imaginary = w.imaginary * a
            };
        }

        public GameValue Square()
        {
            return Power(2);
        }

        public GameValue Power(int power)
        {
            //Recursive
            if (power > 0)
            {
                return this * Power(power - 1);
            }
            else
            {
                return unit;
            }
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override string ToString()
        {
            return Real.ToString();
            /*
            string realPart = "";
            double realPartToShow = Math.Round(Real, 2);

            if (Math.Log10(Math.Abs(realPartToShow)) < 2)
            {
                if (realPartToShow != 0)
                {
                    realPart += Convert.ToString(realPartToShow);
                }
            }
            else
            {
                realPart = GetExponentialNotation(realPartToShow);
            }

            string imaginaryPart = "";
            double imaginaryPartToShow = Math.Round(Imaginary, 2);
            if (imaginaryPartToShow != 0)
            {
                if (imaginaryPartToShow > 0 && !(realPart.Equals("") && imaginaryPartToShow == 1))
                {
                    imaginaryPart = "+";
                }
                if (Math.Log10(Math.Abs(imaginaryPartToShow)) < 2)
                {
                    if (imaginaryPartToShow != 1 && imaginaryPartToShow != 0)
                    {
                        imaginaryPart += Convert.ToString(imaginaryPartToShow);
                    }
                }
                else
                {
                    imaginaryPart += GetExponentialNotation(imaginaryPartToShow);
                }
                imaginaryPart += "i";
            }

            return realPart + imaginaryPart;
             */
        }

        private string GetExponentialNotation(double value)
        {
            int exponent = (int)Math.Log10(Math.Abs(value));
            double mantissa = value / Math.Pow(10, exponent);

            string representation = Convert.ToString(Math.Round(mantissa, 1));
            representation += "e";
            representation += Convert.ToString(exponent);

            return representation;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool Equals(GameValue other)
        {
            return (this.Real == other.Real) && (this.Imaginary == other.Imaginary);
        }

        public int CompareTo(GameValue other)
        {
            return (int)((Mod - other.Mod) * 1000);
        }
    }
}
