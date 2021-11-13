using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Numerics;
using System.Text;

namespace Library_SDES
{
    public static class Extensions 
    {
        public static T[] Slice<T>(this T[] source, int start, int end)
        {
            if (end < 0)
            {
                end = source.Length + end;
            }
            int len = end - start;

            T[] res = new T[len];
            for (int i = 0; i < len; i++)
            {
                res[i] = source[i + start];
            }
            return res;
        }
    }
    public class LRSA : InterfazRSA
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
            long contador = 0;
            string[] Lineas = File.ReadAllLines(key);
            string[] Llaves = Lineas[0].Split(",");
            
            KeyBytes[0] = Convert.ToInt32(Llaves[0]);
            KeyBytes[1] = Convert.ToInt32(Llaves[1]);


            Caracteres = 0;
            byte[] Arreglo =null;

            using (FileStream fs = File.OpenRead(ArchivoNuevo))
            {
                contador = fs.Length;
                using (BinaryReader binaryReader = new BinaryReader(fs))
                {
                    Arreglo = binaryReader.ReadBytes((int)fs.Length);
                }
            }
            List<byte[]> ArregloByte = new List<byte[]>();
            bool Verificar = true;
            byte[] Nuevo = null;
            int x = 0, acarreo = 8000, total = 0;
            while (Verificar) 
            {
                int operacion = (int)contador - acarreo;
                if (operacion > 0)
                {
                    byte[] Ver = Arreglo.Slice(x, acarreo);
                    ArregloByte.Add(CifradoBloques(Ver, acarreo,KeyBytes));
                    x = acarreo ;
                    acarreo += 8000;
                }
                else 
                {
                    byte[] Ver = Arreglo.Slice(x, (int)contador);
                    ArregloByte.Add(CifradoBloques(Ver, (int)contador, KeyBytes));
                    Verificar = false;
                }
            }

            using (BinaryWriter writer = new BinaryWriter(File.Open(ArchivoCodificado, FileMode.Create)))
            {
                foreach (var Escri in ArregloByte) 
                {
                    for (int y = 0; y < Escri.Length; y++)
                    {
                        writer.Write(Escri[y]);
                    }
                }
            }
        }

        public byte[] CifradoBloques(byte[] Arreglo, int contador,int[] KeyBytes) 
        {
            int indice = 0, Reduccion = 1;
            BigInteger Apoyo = 0;
            BigInteger[] VectorBig = new BigInteger[contador];
            BigInteger Lectura = new BigInteger(Arreglo);
            int i = 0;
            bool verificarsigno = false;

            if (Lectura.CompareTo(0) < 0)
            {

                while (Lectura < 0)
                {
                    VectorBig[i] = Lectura % KeyBytes[0];
                    Lectura = Lectura / KeyBytes[0];
                    i++;
                }
                verificarsigno = true;
            }
            else
            {
                while (Lectura > 0)
                {
                    VectorBig[i] = (ModularBig(Lectura, KeyBytes[0]));
                    Lectura = Lectura / KeyBytes[0];
                    i++;
                }
            }

            indice = 0;

            if (verificarsigno)
            {
                while (i>indice)
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
            }
            else
            {
                while (i>indice)
                {
                    Reduccion = 1;
                    BigInteger Numero = VectorBig[indice];
                    for (int y = 0; y < KeyBytes[1]; y++)
                    {
                        Reduccion = (int)(Reduccion * Numero);
                        Reduccion = ModularInt(Reduccion, KeyBytes[0]);
                    }
                    VectorBig[indice] = Reduccion;
                    indice++;
                }
            }


            for (int A = VectorBig.Length-1; A >= 0; A--)
            {
                Apoyo = Apoyo * KeyBytes[0];
                Apoyo = Apoyo + VectorBig[A];
            }

            byte[] EscribirByte = Apoyo.ToByteArray();
            return EscribirByte;
        }
        public BigInteger ModularBig(BigInteger Big, int num)
        {
            BigInteger Primer = Big / num;
            BigInteger Secundo = Big - (Primer * num);
            return Secundo;
        }

    
        public int ModularInt(int Big, int num)
        {
            int Primer = Big / num;
            int Secundo = Big - (Primer * num);
            return Secundo;
        }


    }
}
