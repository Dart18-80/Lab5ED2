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

        protected BitArray[,] SBox0 = new BitArray[4, 4];
        protected BitArray[,] SBox1 = new BitArray[4, 4];

        protected int[] P10 = new int[10];
        protected int[] P8 = new int[8];
        protected int[] P4 = new int[4];
        protected int[] EP = new int[8];
        protected int[] IP = new int[8];
        protected int[] IP1 = new int[8];






        protected BitArray SBOX1F1C1 = new BitArray(8);
        protected BitArray key = new BitArray(10);

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
            string numBinario = Convert.ToString(numero, 2);
            string[] cadenas = numBinario.Split();
            string ceros = "";
            int falta = 10-cadenas[0].Length;

            for (int i = 0; i < falta; i++)
            {
                ceros += "0";
            }
            numBinario = ceros + numBinario;
            key = new BitArray(numero);

            byte[] Arreglo = new byte[120000];

            string Configuracion = System.IO.File.ReadAllText(PermutacionPath);

            string[] Permutaciones = Regex.Split(Configuracion, "[\r\n]+");
            string[] P10string = Regex.Split(Permutaciones[0].ToString(), ",");
            string[] P8String = Regex.Split(Permutaciones[1].ToString(), ",");
            string[] P4String = Regex.Split(Permutaciones[2].ToString(), ",");
            string[] EPString = Regex.Split(Permutaciones[3].ToString(), ",");
            string[] IPString = Regex.Split(Permutaciones[4].ToString(), ",");
            string[] IP1String = Regex.Split(Permutaciones[5].ToString(), ",");

            for (int i = 0; i < 10; i++)
            {
                if (i < 4)
                {

                    P10[i] = Convert.ToInt32(P10string[i].ToString());
                    P8[i] = Convert.ToInt32(P8String[i].ToString());
                    P4[i] = Convert.ToInt32(P4String[i].ToString());
                    EP[i] = Convert.ToInt32(EPString[i].ToString());
                    IP[i] = Convert.ToInt32(IPString[i].ToString());
                    IP1[i] = Convert.ToInt32(IP1String[i].ToString());
                }
                else if (i < 8)
                {
                    P10[i] = Convert.ToInt32(P10string[i].ToString());
                    P8[i] = Convert.ToInt32(P8String[i].ToString());
                    EP[i] = Convert.ToInt32(EPString[i].ToString());
                    IP[i] = Convert.ToInt32(IPString[i].ToString());
                    IP1[i] = Convert.ToInt32(IP1String[i].ToString());
                }
                else 
                {
                    P10[i] = Convert.ToInt32(P10string[i].ToString());
                }            
            }

            CreacionLlave(key);
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
        public void CreacionLlave(BitArray numero) 
        {
            BitArray P10op = PermutacionP10(numero);
        }

        BitArray PermutacionP10(BitArray Cifrar)
        {
            BitArray Nuevo = new BitArray(10);
            for (int i = 0; i < 10; i++) 
            {
                Nuevo[i] = Cifrar[P10[i]];
            }
            return Nuevo;
        }

        BitArray PermutacionP4(BitArray Cifrar)
        {
            BitArray Nuevo = new BitArray(4);
            for (int i = 0; i < 4; i++)
            {
                Nuevo[i] = Cifrar[P4[i]];
            }
            return Nuevo;
        }

        BitArray PermutacionIP(BitArray Cifrar)
        {
            BitArray Nuevo = new BitArray(8);
            for (int i = 0; i < 8; i++)
            {
                Nuevo[i] = Cifrar[IP[i]];
            }
            return Nuevo;
        }
        BitArray PermutacionP8(BitArray Cifrar)
        {
            BitArray Nuevo = new BitArray(8);
            for (int i = 0; i < 8; i++)
            {
                Nuevo[i] = Cifrar[P8[i]];
            }
            return Nuevo;
        }

        BitArray PermutacionEP(BitArray Cifrar)
        {
            BitArray Nuevo = new BitArray(8);
            for (int i = 0; i < 8; i++)
            {
                Nuevo[i] = Cifrar[EP[i]];
            }
            return Nuevo;
        }

        BitArray PermutacionIP1(BitArray Cifrar)
        {
            BitArray Nuevo = new BitArray(8);
            for (int i = 0; i < 8; i++)
            {
                Nuevo[i] = Cifrar[IP1[i]];
            }
            return Nuevo;
        }

    }
}
