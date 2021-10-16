using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Library_SDES
{
    public class SDES 
    {
        public readonly IHostingEnvironment fistenviroment;
        protected string[] Permutaciones = new string[6];

        protected byte[,] SBox0 = new byte[4, 4]{{ 01, 00, 11, 10 },
                                                { 11, 10, 01, 00 },
                                                { 00, 10, 01, 11 },
                                                { 11, 01, 11, 10 }};

        protected byte[,] SBox1 = new byte[4, 4] {{ 00, 01, 10, 11 },
                                                { 10, 00, 01, 11 },
                                                { 11, 00, 01, 00 },
                                                { 10, 01, 00, 11 }}; 
        protected SDES(IHostingEnvironment enviroment)
        {
            this.fistenviroment = enviroment;
        }

        public SDES()
        {
        }

        public void Read_File(string ArchivoNuevo, string ArchivoCodificado, char[] Numero)
        {
            string UploadFolder = "";
            byte[] Arreglo = new byte[120000];

            UploadFolder = Path.Combine(fistenviroment.ContentRootPath, "Helpers");
            string filepath = Path.Combine(UploadFolder, "Permutations.txt");

            string Configuracion = System.IO.File.ReadAllText(filepath);

            Permutaciones = Regex.Split(Configuracion, "[\r\n]+");

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
        public string LlenarEspacio(byte Numeros, int forma)//forma para saber si es de 8 o 10 bits
        {
            char[] Base = Convert.ToString(Numeros).ToCharArray();
            if (Base.Length != 0)
            {
                int Flatante = forma - Base.Length;
                string falt = "";

                for (int i = 0; i < Flatante; i++)
                {
                    falt += "0";
                }
                return (falt+Convert.ToString(Numeros));
            }
            else
            {
                return "";
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
