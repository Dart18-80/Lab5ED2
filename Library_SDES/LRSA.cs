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
            int Indice = 1, D=0;
            E = ListaCoprimos[Indice-1];
            bool Entrar = true;
            for (int i = 0; i < PhiN; i++) 
            {
                int Res = (int)(Indice % E);
                if (Res == 0) 
                {
                    if (Entrar) 
                    {
                        D = Indice / E;
                        Entrar = false;
                    }
                }
                Indice = Indice + PhiN;
            }

            string PrivateKeyPath = "\\Private.key";
            string PublicKeyPath = "\\Public.key";

            string Resultado = CarpetaPath + ".Zip";

            PrivateKeyPath = CarpetaPath + PrivateKeyPath;
            PublicKeyPath = CarpetaPath + PublicKeyPath;

            using (StreamWriter writer = new StreamWriter(PrivateKeyPath))
            {
                writer.Write(n);
                writer.Write(",");
                writer.Write(D);
            }

            using (StreamWriter Escribir = new StreamWriter(PublicKeyPath))
            {
                Escribir.Write(n);
                Escribir.Write(",");
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
            int[] KeyBytes = new int[2];
            int contador = 0, Reduccion = 1;
            string ArchivoKeys = "";
            string[] Lineas = File.ReadAllLines(key);

            string[] Llaves = Lineas[0].Split(",");
            
            KeyBytes[0] = Convert.ToInt32(Llaves[0]);
            KeyBytes[1] = Convert.ToInt32(Llaves[1]);


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
            byte[] NuevoArreglo = new byte[contador];

            for (int j = 0; j < contador; j++)
            {
                NuevoArreglo[j] = Arreglo[j];
            }
            int indice = 0;
            BigInteger[] VectorBig = new BigInteger[contador];
            BigInteger[] Apoyo = new BigInteger[contador];
            BigInteger Lectura = new BigInteger(NuevoArreglo);
            int i = 0;

            while (Lectura > 0)
            {
                VectorBig[i] = (int)(Lectura % KeyBytes[0]);
                Lectura = Lectura / KeyBytes[0];
                i++;
            }
            indice = 0;
            while (indice<contador)
            {
                Reduccion = 1;
                BigInteger Numero = VectorBig[indice];
                for (int y = 0; y < KeyBytes[1]; y++)
                {
                    Reduccion = (int)(Reduccion * Numero);
                    Reduccion = Reduccion % KeyBytes[0];
                }
                VectorBig[indice] = Reduccion;
                indice++;
            }

            using (BinaryWriter writer = new BinaryWriter(File.Open(ArchivoCodificado, FileMode.Create)))
            {
                for (int j = 0; j < Caracteres; j++)
                {
                    byte[] Nuevo = VectorBig[j].ToByteArray();
                    for (int k = 0; k < Nuevo.Length; k++)
                    {
                        writer.Write(Nuevo[k]) ;
                    }
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
