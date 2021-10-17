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

        protected BitArray K1 = new BitArray(8);
        protected BitArray K2 = new BitArray(8);

        protected int[] P10 = new int[10];
        protected int[] P8 = new int[8];
        protected int[] P4 = new int[4];
        protected int[] EP = new int[8];
        protected int[] IP = new int[8];
        protected int[] IP1 = new int[8];

        protected BitArray SBOX1F1C1 = new BitArray(8);
        protected BitArray key = new BitArray(10);

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

        public void Read_File(string ArchivoNuevo, string ArchivoCodificado, string PermutacionPath, int numero, int FormaSDES)
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
            string[] arraybinario = Regex.Split(numBinario, "");

            for (int i = 0; i < 10; i++)
            {
                if (arraybinario[i+1]=="1")
                {
                    key[i] = true;
                }
                else 
                {
                    key[i] = false;
                }
            }

            byte[] Arreglo = new byte[120000];
            byte[] ArregloPrueba = new byte[120000];


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

            CreacionLlave(key); // Se manda a crear k1 y k2 con la llave secreta

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
                    byte[] KeyByteNuevo = {nuevo};

                    BitArray KeyNuevo = new BitArray(KeyByteNuevo);
                    BitArray Aux = new BitArray(8);

                    for (int i = 0; i < 8; i++)
                    {
                        Aux[i] = KeyNuevo[7 - i];
                    }

                    BitArray CifratoDecifrado = new BitArray(8);
                    if (FormaSDES==0)
                    {
                        CifratoDecifrado = CifradoDecifradoSDES(Aux, 0);
                    }
                    else
                    {
                        CifratoDecifrado = CifradoDecifradoSDES(Aux, 1);
                    }
                    byte[] ArregloDC = new byte[1];
                    BitArray Aux1 = new BitArray(8);

                    for (int i = 0; i < 8; i++)
                    {
                        Aux1[i] = CifratoDecifrado[7-i];
                    }
                    Aux1.CopyTo(ArregloDC, 0);
                    Arreglo[contador] = ArregloDC[0];
                    ArregloPrueba[contador] = nuevo;
                    contador++;
                }
            }

            using (BinaryWriter writer = new BinaryWriter(File.Open(ArchivoCodificado, FileMode.Create)))
            {
                for (int i = 0; i < Caracteres; i++)
                {
                    writer.Write(Arreglo[i]);
                }
            }
        }
        BitArray CifradoDecifradoSDES(BitArray NumCifrar, int FormaSDES)
        {
            BitArray IPP = PermutacionIP(NumCifrar);//Funcion IP
            BitArray IPM1 = new BitArray(4);
            BitArray IPM2 = new BitArray(4);

            for (int i = 0; i < 4; i++)
            {
                IPM1[i] = IPP[i];
                IPM2[i] = IPP[4+i];
            }

            BitArray EPP = PermutacionEP(IPM2);//Funcion EP

            BitArray XorEP = new BitArray(8);
            if (FormaSDES==0)
            {
                 XorEP = EPP.Xor(K1);
            }
            else
            {
                 XorEP = EPP.Xor(K2);
            }

            BitArray S0S1 = FilasColum(XorEP);

            BitArray PermuP4 = PermutacionP4(S0S1);

            BitArray XORP4 = PermuP4.Xor(IPM1);

            BitArray SW0 = new BitArray(4);
            BitArray SW1 = new BitArray(4);
            SW0[0] = IPM2[0]; SW0[1] = IPM2[1]; SW0[2] = IPM2[2]; SW0[3] = IPM2[3];
            SW1[0] = XORP4[0]; SW1[1] = XORP4[1]; SW1[2] = XORP4[2]; SW1[3] = XORP4[3];

            BitArray EPP1 = PermutacionEP(SW1);

            BitArray XorEP2 = new BitArray(8);
            if (FormaSDES == 0)
            {
                XorEP2 = EPP1.Xor(K2);
            }
            else
            {
                XorEP2 = EPP1.Xor(K1);
            }

            BitArray S1S1 = FilasColum(XorEP2);

            BitArray Permu1P4 = PermutacionP4(S1S1);

            BitArray XOR1P4 = Permu1P4.Xor(SW0);

            BitArray IPN1 = new BitArray(8);
            IPN1[0] = XOR1P4[0];   IPN1[1] = XOR1P4[1];   IPN1[2] = XOR1P4[2];   IPN1[3] = XOR1P4[3];
            IPN1[4] = SW1[0];  IPN1[5] = SW1[1];  IPN1[6] = SW1[2];   IPN1[7] = SW1[3];

            BitArray IPInversa = PermutacionIP1(IPN1);

            return IPInversa;
        }
        public void CreacionLlave(BitArray numero) 
        {
            BitArray P10op = PermutacionP10(numero);
            BitArray LS10 = new BitArray(5);
            BitArray LS11 = new BitArray(5);

            BitArray LS20 = new BitArray(5);
            BitArray LS21 = new BitArray(5);

            BitArray ULS1 = new BitArray(10);
            BitArray ULS2 = new BitArray(10);

            for (int i = 0; i < 5; i++)
            {
                if (i == 4)
                {
                    LS10[i] = P10op[i + 1];
                    LS11[i] = P10op[0];
                }
                else
                {
                    LS10[i] = P10op[i + 1];
                    LS11[i] = P10op[i + 6];
                }
            }

            ULS1 = CombineLS(LS10, LS11);

            K1 = PermutacionP8(ULS1);

            for (int i = 0; i < 5; i++)
            {
                if (i<3)
                {
                    LS20[i] = LS10[i + 2];
                    LS21[i] = LS11[i + 2];
                }
                else
                {
                    LS20[i] = LS10[i-3];
                    LS21[i] = LS11[i - 3];
                }
            }

            ULS2 = CombineLS(LS20, LS21);

            K2 = PermutacionP8(ULS2);
        }

        BitArray FilasColum(BitArray XorEP) 
        {
            BitArray EPK1 = new BitArray(4);
            BitArray EPK2 = new BitArray(4);

            for (int i = 0; i < 4; i++)
            {
                EPK1[i] = XorEP[i];
                EPK2[i] = XorEP[4 + i];
            }

            BitArray F01 = new BitArray(2);
            BitArray C01 = new BitArray(2);
            BitArray F11 = new BitArray(2);
            BitArray C11 = new BitArray(2);

            F01[0] = EPK1[0]; C01[0] = EPK1[1];
            F01[1] = EPK1[3]; C01[1] = EPK1[2];

            F11[0] = EPK2[0]; C11[0] = EPK2[1];
            F11[1] = EPK2[3]; C11[1] = EPK2[2];

            int S00 = IndiceMatriz(F01[0], F01[1]);
            int S01 = IndiceMatriz(C01[0], C01[1]);
            int S10 = IndiceMatriz(F11[0], F11[1]);
            int S11 = IndiceMatriz(C11[0], C11[1]);

            BitArray S0 = SBox0[S00, S01];
            BitArray S1 = SBox1[S10, S11];

            BitArray S0S1 = new BitArray(4);
            S0S1[0] = S0[0]; S0S1[1] = S0[1];
            S0S1[2] = S1[0]; S0S1[2] = S1[1];

            return S0S1;
        }

        int IndiceMatriz(bool X , bool Y) 
        {
            if (X == true && Y == true)
                return 3;
            else if (X == false && Y == false)
                return 0;
            else if (X == true && Y == false)
                return 2;
            else
                return 1;
        }


        BitArray CombineLS(BitArray LS10, BitArray LS11) 
        {
            BitArray ULS = new BitArray(10);
            for (int i = 0; i < 10; i++)
            {
                if (i < 5)
                {
                    ULS[i] = LS10[i];
                }
                else
                {
                    ULS[i] = LS11[i - 5];
                }
            }
            return ULS;
        }
        BitArray PermutacionP10(BitArray Cifrar)
        {
            BitArray Nuevo = new BitArray(10);
            for (int i = 0; i < 10; i++) 
            {
                Nuevo[i] = Cifrar[P10[i]-1];
            }
            return Nuevo;
        }

        BitArray PermutacionP4(BitArray Cifrar)
        {
            BitArray Nuevo = new BitArray(4);
            for (int i = 0; i < 4; i++)
            {
                Nuevo[i] = Cifrar[P4[i]-1];
            }
            return Nuevo;
        }

        BitArray PermutacionIP(BitArray Cifrar)
        {
            BitArray Nuevo = new BitArray(8);
            for (int i = 0; i < 8; i++)
            {
                Nuevo[i] = Cifrar[IP[i]-1];
            }
            return Nuevo;
        }
        BitArray PermutacionP8(BitArray Cifrar)
        {
            BitArray Nuevo = new BitArray(8);
            for (int i = 0; i < 8; i++)
            {
                Nuevo[i] = Cifrar[P8[i]-1];
            }
            return Nuevo;
        }

        BitArray PermutacionEP(BitArray Cifrar)
        {
            BitArray Nuevo = new BitArray(8);
            for (int i = 0; i < 8; i++)
            {
                Nuevo[i] = Cifrar[EP[i]-1];
            }
            return Nuevo;
        }

        BitArray PermutacionIP1(BitArray Cifrar)
        {
            BitArray Nuevo = new BitArray(8);
            for (int i = 0; i < 8; i++)
            {
                Nuevo[i] = Cifrar[IP1[i]-1];
            }
            return Nuevo;
        }

    }
}
