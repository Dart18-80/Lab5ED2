﻿using Microsoft.AspNetCore.Hosting;
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

        protected byte[,] SBox1 = new byte[4, 4] {{ 00, 01, 10, 11 },
                                                { 10, 00, 01, 11 },
                                                { 11, 00, 01, 00 },
                                                { 10, 01, 00, 11 }};


        public BitArray SBOX1F1C1 = new BitArray(8);

        protected SDES(IHostingEnvironment enviroment)
        {
            this.fistenviroment = enviroment;
        }

        public SDES()
        {
            BitArray FirstCombine = new BitArray(2);
            FirstCombine[0] = false;
            FirstCombine[1] = false;

            BitArray SecondCombine = new BitArray(2);
            SecondCombine[0] = true;
            SecondCombine[1] = false;

            BitArray ThirdCombine = new BitArray(2);
            ThirdCombine[0] = true;
            ThirdCombine[1] = true;

            BitArray FourCombine = new BitArray(2);
            FourCombine[0] = false;
            FourCombine[1] = true;


            SBox0[0, 0] = FourCombine;
            SBox0[0, 1] = FirstCombine;

        }

        public void Read_File(string ArchivoNuevo, string ArchivoCodificado, string PermutacionPath, int numero)
        {
            BitArray NuevoByte = new BitArray(8);
           
            byte[] Arreglo = new byte[120000];

            string Configuracion = System.IO.File.ReadAllText(PermutacionPath);

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
        }
        public void CifradoSDES(int[] Binario)
        {

        }
        public void CreacionLlave(byte numero) 
        {
            
            
        }
    }
}
