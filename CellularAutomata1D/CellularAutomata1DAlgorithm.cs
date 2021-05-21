using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;

namespace CellularAutomata1D
{
    public class CellularAutomata1DAlgorithm
    {
        public string Key { get; set; }
        public string KeyPrintableBytes { get; set; }
        public string Cypher { get; set; }
        public string CypherPrintableBytes { get; set; }
        public string Message { get; set; }
        
        public void GenerateKey(byte akRule, bool[] bits, int timeSteps, bool butifyAscii)
        {
            var rule = new AKRule(akRule);
            
            var key = new List<bool>();
            var fixedIndex = new Random().Next() % bits.Length;

            for (int step = 0; step < timeSteps; step++)
            {
                key.Add(bits[fixedIndex]);
                var newBits = new bool[bits.Length];

                for (int i = 0; i < bits.Length; i++)
                {
                    var leftIndex = i - 1 < 0 ? bits.Length - 1 : i - 1;
                    var rightIndex = i + 1 > bits.Length - 1 ? 0 : i + 1;

                    var neighbour = new[] {bits[leftIndex], bits[i], bits[rightIndex]};

                    newBits[i] = rule.GetTransformedValue(neighbour);
                }

                bits = newBits;
            }

            var keyArray = key.ToArray();
            
            var keyByte = new List<byte>();
            for (int i = 0; i < keyArray.Length; i += 8)
            {
                var end = i + 8 > keyArray.Length - 1 ? keyArray.Length - 1 : i + 8;
                var sign = new ArraySegment<bool>(keyArray, i, end - i).ToArray();
                var ascii = ByteHelper.ConvertBoolArrayToByte(sign);
                if (butifyAscii)
                {
                    keyByte.Add(BeautyfulAscii(ascii));
                }
                else
                {
                    keyByte.Add(ascii);
                }
                
            }
            Key = Encoding.ASCII.GetString(keyByte.ToArray());
        }

        private byte BeautyfulAscii(byte ascii)
        {
            var intCode = 0;
            if (ascii < 33)
            {
                intCode =  (byte) (ascii + 37);
                return (byte) (intCode +33);
            }

            if (ascii < 126)
            {
                intCode =  (byte) (ascii - 33);
                return (byte) (intCode +33);
            }
            if (ascii < 219)
            {
                intCode =  (byte) (ascii - 126);
                return (byte) (intCode +33);
            }
            intCode =  (byte) (ascii - 219);
                return (byte) (intCode +33);
        }

        public void Crypt(string key, string plaintext)
        {
            var keyBytes = Encoding.ASCII.GetBytes(key);
            var plaintextBytes = Encoding.ASCII.GetBytes(plaintext);
            var cypher = new byte[keyBytes.Length];

            if (keyBytes.Length < plaintext.Length)
            {
                return;
            }

            for (int i = 0; i < plaintextBytes.Length; i++)
            {
                var bitKey = ByteHelper.ConvertByteToBoolArray(keyBytes[i]);
                var bitPlaintext = ByteHelper.ConvertByteToBoolArray(plaintextBytes[i]);
                var cypherbits = new bool[8];
                for (int j = 0; j < 8; j++)
                {
                    cypherbits[j] = XOR(bitKey[j], bitPlaintext[j]);
                }
                cypher[i] = ByteHelper.ConvertBoolArrayToByte(cypherbits);
            }
            Cypher = Encoding.ASCII.GetString(cypher);
        }
        
        public void Decrypt(string key,string crypt)
        {
            var cryptBytes = Encoding.ASCII.GetBytes(crypt);
            var keyBytes = Encoding.ASCII.GetBytes(key);
            var plaintextBytes = new byte[keyBytes.Length];

            if (keyBytes.Length < cryptBytes.Length)
            {
                return;
            }

            for (int i = 0; i < cryptBytes.Length; i++)
            {
                var bitKey = ByteHelper.ConvertByteToBoolArray(keyBytes[i]);
                var bitPlaintext = ByteHelper.ConvertByteToBoolArray(plaintextBytes[i]);
                var cypherbits = new bool[8];
                for (int j = 0; j < 8; j++)
                {
                    cypherbits[j] = XOR(bitKey[j], bitPlaintext[j]);
                }
                plaintextBytes[i] = ByteHelper.ConvertBoolArrayToByte(cypherbits);
            }
            Message = Encoding.ASCII.GetString(plaintextBytes);
        }

        private bool XOR(bool a, bool b)
        {
            return a != b;
        }
    }

    public class AKRule
    {
        private readonly bool[] _value;

        public AKRule(byte value)
        {
            _value = ByteHelper.ConvertByteToBoolArray(value);
        }

        public bool GetTransformedValue(bool[] neighbour)
        {
            if (neighbour.Length > 3)
            {
                throw new Exception("Somsiedztcwto wieksze niż 3");
            }

            var neighbourByte = ByteHelper.ConvertBoolArrayToByte(neighbour);

            return _value[neighbourByte];
        }
    }
}