using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections;
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

        protected BitArray[,] SBox0 = new BitArray[4, 4];
        protected BitArray[,] SBox1 = new BitArray[4, 4];

        protected int[] P10 = new int[10];
        protected int[] P8 = new int[8];
        protected int[] P4 = new int[4];
        protected int[] EP = new int[8];
        protected int[] IP = new int[8];
        protected int[] IP1 = new int[8];






        public BitArray SBOX1F1C1 = new BitArray(8);

        protected SDES(IHostingEnvironment enviroment)
        {
            this.fistenviroment = enviroment;
        }

        public SDES()
        {
            BitArray FirstCombine = new BitArray(2);//00
            FirstCombine[0] = false;
            FirstCombine[1] = false;

            BitArray SecondCombine = new BitArray(2);//10
            SecondCombine[0] = true;
            SecondCombine[1] = false;

            BitArray ThirdCombine = new BitArray(2);//11
            ThirdCombine[0] = true;
            ThirdCombine[1] = true;

            BitArray FourCombine = new BitArray(2);//01
            FourCombine[0] = false;
            FourCombine[1] = true;

            SBox0 = new BitArray[4, 4] { { FourCombine, FirstCombine, ThirdCombine, SecondCombine },
                                         { ThirdCombine, SecondCombine, FourCombine, FirstCombine },
                                         { FirstCombine, SecondCombine, FourCombine, ThirdCombine },
                                         { ThirdCombine, FourCombine, ThirdCombine, SecondCombine }};

            SBox1 = new BitArray[4, 4] { { FirstCombine, FourCombine, SecondCombine, ThirdCombine},
                                         { SecondCombine, FirstCombine, FourCombine, ThirdCombine},
                                         { ThirdCombine, FirstCombine, FourCombine, FirstCombine},
                                         { SecondCombine, FourCombine, FirstCombine, ThirdCombine}};
        }

        public void Read_File(string ArchivoNuevo, string ArchivoCodificado, string PermutacionPath, int numero)
        {
            BitArray NuevoByte = new BitArray(8);
           
            byte[] Arreglo = new byte[120000];

            string Configuracion = System.IO.File.ReadAllText(PermutacionPath);

            Permutaciones = Regex.Split(Configuracion, "[\r\n]+");
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
                    NuevoByte = new BitArray(nuevo);


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
        public void CifradoSDES(int[] Binario)
        {

        }
        public void CreacionLlave(byte numero) 
        {
            
            
        }


    }
}
