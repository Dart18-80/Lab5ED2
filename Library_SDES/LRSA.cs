using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Library_SDES
{
    public class LRSA
    {
        int[] ListaCoprimos = new int[100000];

        public bool ComprobacionLlavesPrimos(int P , int Q) 
        {
            if (ComprobacionLlavesPrimos(P) && ComprobacionLlavesPrimos(Q))
                return true;
            else
                return false;

        }
        public bool ComprobacionLlavesPrimos(int Key)
        {
            int Aux = 0;
            int Min = 2;
            if (Key >= 2)
            {
                while (Min <= (Key / 2))
                {
                    if (Key % Min == 0) { Aux++; Min++; }
                    else
                        Min++;
                }
                if (Aux == 0)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public void GenerarLlaves(int p, int q) 
        {
            int n = p * q;
            int PhiN = (p-1)*(q-1);
            int res = 0, E = 2,Cont = 0 ;
            while (E < PhiN)
            {
                res = MinimoComun(2, PhiN);
                if (res == 1 && Cont < 100000)
                {
                    ListaCoprimos[Cont] = E;
                    E++;
                    Cont++;
                }
                else
                    E++;
            }
            Random Num = new Random();
            int Indice = Num.Next(1,Cont+1);
            E = ListaCoprimos[Indice];

            int D, K = 1;
            int Aux = (1 + K * PhiN) % E;
            while (Aux != 0) 
            {
                K++;
                Aux = (1+K*PhiN)%E;
            }
            D = (1 + K * PhiN) / E;

            string PrivateKeyPath = @"C:\\Users\\Compresion\\Private.key";
            string PublicKeyPath = @"C:\\Users\\Compresion\\Public.key";
            string PathZip = @"C:\\Users\\Compresion.zip";

            System.IO.Directory.CreateDirectory(PrivateKeyPath);
            System.IO.Directory.CreateDirectory(PublicKeyPath);


            using (BinaryWriter writer = new BinaryWriter(File.Open(PrivateKeyPath, FileMode.Create)))
            {
                writer.Write(n);
                writer.Write(D);
            }

            using (BinaryWriter writer = new BinaryWriter(File.Open(PublicKeyPath, FileMode.Create)))
            {
                writer.Write(n);
                writer.Write(E);
            }

            string startPath = @"C:\\Users\\Compresion";
            ZipFile.CreateFromDirectory(startPath, PathZip, CompressionLevel.Fastest, true);
        }

        public int MinimoComun(int E, int PhiN) 
        {
            int res = PhiN % E;
            while (res != 0) 
            {
                PhiN = E;
                E = res;
                res = PhiN % E;
            }
            return E;
        }

        public void CifrarRSA(string ArchivoNuevo, string ArchivoCodificado, int P, int N) 
        {
            long Caracteres;
            byte[] Arreglo = new byte[12000000];
            byte[] NumP = BitConverter.GetBytes(P);
            byte[] NumN = BitConverter.GetBytes(N);
            using (Stream Text = new FileStream(ArchivoNuevo, FileMode.OpenOrCreate, FileAccess.Read))
            {
                Caracteres = Text.Length;
            }
            using (BinaryReader reader = new BinaryReader(File.Open(ArchivoNuevo, FileMode.Open)))
            {
                int contador = 0;
                foreach (byte nuevo in reader.ReadBytes((int)Caracteres))
                {

                    Arreglo[contador] = Convert.ToByte(Modular((int)Math.Pow(nuevo, P), N));
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

        public void Descifrar(string ArchivoNuevo, string ArchivoCodificado, int P, int N)
        {
            long Caracteres;
            byte[] Arreglo = new byte[12000000];
            byte[] NumP = BitConverter.GetBytes(P);
            byte[] NumN = BitConverter.GetBytes(N);
            using (Stream Text = new FileStream(ArchivoNuevo, FileMode.OpenOrCreate, FileAccess.Read))
            {
                Caracteres = Text.Length;
            }
            using (BinaryReader reader = new BinaryReader(File.Open(ArchivoNuevo, FileMode.Open)))
            {
                int contador = 0;
                foreach (byte nuevo in reader.ReadBytes((int)Caracteres))
                {

                    Arreglo[contador] = Convert.ToByte(Modular((int)Math.Pow(nuevo, P),N));
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
    }
}
