using System;

namespace NumberFormatManager.Services
{
    public class NumberFormatService
    {
        public int A { get; set; }
        public int B { get; set; }
        public decimal D { get; set; }
        public int L { get; set; }
        
        public int Precision { get; set; }

        public NumberFormatService(int a, int b, decimal d)
        {
            A = a;
            B = b;
            D = d;
            Precision = GetPrecision(d);
            

            L = CalculateL();
        }
        
        public int RealToInt(decimal real)
        {
            var dec = ((decimal) 1 / (B - A)) * ((real - A) * (decimal) (Math.Pow(2, L) - 1));
            return (int) decimal.Round(dec);
        }

        public string IntToBin(int integer)
        {
            var binValue = Convert.ToString(integer, 2);
            return binValue.PadLeft(L, '0');
        }

        public int BinToInt(string bin)
        {
            return Convert.ToInt32(bin, 2);
        }

        public decimal IntToReal(int integer)
        {
            var real = (decimal) (A + ((B - A) * integer) / (Math.Pow(2, L) - 1));
            return decimal.Round(real, Precision);
        }

        public decimal BinToReal(string bin)
        {
            var integer = BinToInt(bin);
            return IntToReal(integer);
        }

        public string RealToBin(decimal real)
        {
            var integer = RealToInt(real);
            return IntToBin(integer);
        }
        
        public decimal CalculateFx(decimal x)
        {
            return (x % 1) * (decimal)(Math.Cos(Convert.ToDouble(20m * Convert.ToDecimal(Math.PI) * x)) - Math.Sin(Convert.ToDouble(x)));
        }

        public decimal RandomDecimal()
        {
            var value = Convert.ToDecimal(new Random().NextDouble());
            value *= (B - A);
            value += A;
            return decimal.Round(value, Precision);
        }
        
        private int GetPrecision(decimal d)
        {
            return BitConverter.GetBytes(decimal.GetBits(d)[3])[2];
        }
        private int CalculateL()
        {
            return Convert.ToInt32(Math.Ceiling(Math.Log2((B - A) * (double)(1 / D) + 1)));
        }
    }
}