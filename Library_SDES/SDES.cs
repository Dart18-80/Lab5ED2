using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Library_SDES
{
    public class SDES
    {
        public int[] Permutaciones() 
        {
            string ArchivoPermutacion = "";
            int Numeros = 0;
            using (Stream Permu = new FileStream(ArchivoPermutacion, FileMode.Open, FileAccess.Read)) 
            {
                            }
        }

        public void Read_File(string ArchivoNuevo, string ArchivoCodificado, char[] Numero)
        {
            byte[] Arreglo = new byte[120000];

            using (Stream Memory = new MemoryStream())
            {
                long Caracteres = 0;

                using (Stream Text = new FileStream(ArchivoNuevo, FileMode.OpenOrCreate, FileAccess.Read))
                {
                    Caracteres = Text.Length;
                }
                using (BinaryReader reader = new BinaryReader(File.Open(ArchivoNuevo, FileMode.Open)))
                {
                    int contador = 0;
                    foreach (byte nuevo in reader.ReadBytes((int)Caracteres))
                    {
                        Arreglo[contador] = nuevo;
                        int[] Binario = Convert_Binario(nuevo); 

                        // Enviar Binario a tu funcion de compresion


                        contador++;
                    }
                }

                using (BinaryWriter writer = new BinaryWriter(File.Open(ArchivoCodificado, FileMode.Create)))
                {
                    for (int i = 0; i <= Caracteres; i++)
                    {
                        writer.Write(Arreglo[i]);
                    }
                }
            }
        }

        public int[] Convert_Binario(int Num) 
        {
            int[] binario = new int[8];
            int i = 0;
            while (Num > 0)
            {
                binario[i] = Num % 2;
                Num = Num / 2;
                i++;
            }
            return binario;
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
