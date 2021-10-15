using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Library_SDES
{
    public class SDES
    {
        public void Read_File(string Path) 
        {
            using (Stream Memory = new MemoryStream())
            {
                long Caracteres = 0;
                using (Stream Text = new FileStream(Path, FileMode.Open, FileAccess.Read))
                {
                    Caracteres = Text.Length;
                    Text.CopyTo(Memory);
                }
                for (long Count = 0; Count < Caracteres; Count++)
                {
                    Memory.Seek(Count, SeekOrigin.Begin);
                    int Vector = Memory.ReadByte();
                    int[] Binario = Convert_Binario(Vector);

                }
            }
        }

        public int[] Convert_Binario(int Num) 
        {
            string Binario = "";
            string Aux = "";
            while (Num > 0)
            {
                Binario = Num % 2 + Binario;
                Num = Num / 2;
            }
            long Quant = Binario.Length;
            Quant = 8 - Quant;
            if (Quant > 0)
            {
                while (Quant >= 0)
                {
                    Aux += "0";
                    Quant--;
                }
                Aux += Binario;
            }
            int[] Txt = Array.ConvertAll(Aux.Split(""), Int32.Parse);
            return Txt;
        }

        public string DecimalBinario(int numero) 
        {
            string binario = "";
            if (numero > 0)
            {
                while (numero > 0)
                {
                    if (numero % 2 == 0)
                    {
                        binario = "0" + binario;
                    }
                    else
                    {
                        binario = "1" + binario;
                    }
                    numero = (int)(numero / 2);
                }
                return binario;
            }
            else
            {
                if (numero == 0)
                {
                    return "0";
                }
                else
                {
                    return "";
                }
            }
        }

        public int BinarioDecinal(char[] numero) 
        {
            int sum = 0;
            Array.Reverse(numero);
            for (int i = 0; i < numero.Length; i++)
            {
                if (numero[i].ToString() == "1")
                {
                    sum += (int)Math.Pow(2, i);

                }
            }
            return sum;
        }
    }
}
