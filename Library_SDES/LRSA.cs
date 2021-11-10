using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Numerics;
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

        public void GenerarLlaves(int p, int q, string CarpetaPath) 
        {
            int n = p * q;
            int PhiN = (p-1)*(q-1);
            int res = 0, E = 2,Cont = 0 ;
            while (E < PhiN)
            {
                res = MinimoComun(E, PhiN);
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
            E = ListaCoprimos[Indice-1];

            int D, K = 1;
            int Aux = (1 + K * PhiN) % E;
            while (Aux != 0) 
            {
                K++;
                Aux = (1+K*PhiN)%E;
            }
            D = (1 + K * PhiN) / E;

            string PrivateKeyPath = "\\Private.key";
            string PublicKeyPath = "\\Public.key";

            string Resultado = CarpetaPath + ".Zip";

            PrivateKeyPath = CarpetaPath + PrivateKeyPath;
            PublicKeyPath = CarpetaPath + PublicKeyPath;

            using (BinaryWriter writer = new BinaryWriter(File.Open(PrivateKeyPath, FileMode.Create)))
            {
                writer.Write(n);
                writer.Write(D);
            }

            using (BinaryWriter Escribir = new BinaryWriter(File.Open(PublicKeyPath, FileMode.Create)))
            {
                Escribir.Write(n);
                Escribir.Write(E);
            }

            if (!Directory.Exists(Resultado)) 
            {
                File.Delete(Resultado);
                ZipFile.CreateFromDirectory(CarpetaPath, Resultado, CompressionLevel.Fastest, true);
            }
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

        
        public void CifrarRSA(string ArchivoNuevo, string ArchivoCodificado, string key) 
        {

            long Caracteres;
            byte[] KeyBytes = new byte[2];
            int[] Texto = new int[12000000];
            int contador = 0, Reduccion = 1;
            using (Stream Text = new FileStream(key, FileMode.OpenOrCreate, FileAccess.Read))
            {
                Caracteres = Text.Length;
            }
            using (BinaryReader reader = new BinaryReader(File.Open(key, FileMode.Open)))
            {
                contador = 0;
                foreach (byte nuevo in reader.ReadBytes((int)Caracteres))
                {

                    KeyBytes[contador] = nuevo;
                    contador++;
                }
            }

            Caracteres = 0;
            byte[] Arreglo = new byte[12000000];
            using (Stream Text = new FileStream(ArchivoNuevo, FileMode.OpenOrCreate, FileAccess.Read))
            {
                Caracteres = Text.Length;
            }
            using (BinaryReader reader = new BinaryReader(File.Open(ArchivoNuevo, FileMode.Open)))
            {
                contador = 0;
                foreach (byte nuevo in reader.ReadBytes((int)Caracteres))
                {
                    Arreglo[contador] = (byte)nuevo; 
                    contador++;
                }
            }
            int indice = 0;
            BigInteger Lectura = new BigInteger(Arreglo);
            for (int i = 0; i<= contador-1;i++) 
            {
                while (Lectura > 0) 
                {
                    Texto[indice] = (int)((int)Lectura % (int)KeyBytes[0]);
                    Lectura = Lectura / KeyBytes[0];
                }
                Reduccion = 1;
                for (int y = 0; y < KeyBytes[1]; y++)
                {
                    Reduccion = Reduccion * Texto[i];
                    Reduccion = Reduccion % KeyBytes[0];
                }
                Texto[indice] = Reduccion;
                indice++;
            }

            using (BinaryWriter writer = new BinaryWriter(File.Open(ArchivoCodificado, FileMode.Create)))
            {
                for (int i = 0; i < Caracteres; i++)
                {
                    writer.Write((byte)Texto[i]);
                }
            }
        }

        public void Descifrar(string ArchivoNuevo, string ArchivoCodificado, string key)
        {
            long Caracteres;
            byte[] KeyBytes = new byte[2];
            using (Stream Text = new FileStream(key, FileMode.OpenOrCreate, FileAccess.Read))
            {
                Caracteres = Text.Length;
            }
            using (BinaryReader reader = new BinaryReader(File.Open(ArchivoNuevo, FileMode.Open)))
            {
                int contador = 0;
                foreach (byte nuevo in reader.ReadBytes((int)Caracteres))
                {

                    KeyBytes[contador] = nuevo;
                    contador++;
                }
            }

            Caracteres = 0;
            byte[] Arreglo = new byte[12000000];
            using (Stream Text = new FileStream(ArchivoNuevo, FileMode.OpenOrCreate, FileAccess.Read))
            {
                Caracteres = Text.Length;
            }
            using (BinaryReader reader = new BinaryReader(File.Open(ArchivoNuevo, FileMode.Open)))
            {
                int contador = 0;
                foreach (byte nuevo in reader.ReadBytes((int)Caracteres))
                {

                    Arreglo[contador] = Convert.ToByte(((int)Math.Pow(nuevo, (int)KeyBytes[1])) % (int)KeyBytes[0]);
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
