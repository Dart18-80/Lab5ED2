using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Library_SDES
{
    public class LRSA
    {
        protected int[,] MatrizRSA = new int[2,2];
        public void CreacionLlaves(int p, int q) 
        {
            Random Rand = new Random();

            int n = p * q;
            int phiN = (p - 1)*(q - 1);
            //Se crea una lista sin los coprimos de p y q;
            List<int> ListaN = CrearLista(phiN, p, q);
            //la lista se reduce a primos
            ListaN = ListaPrimos(ListaN);
            int index = Rand.Next(0, ListaN.Count);
            int e = ListaN[index];

            MatrizRSA[0, 0] =phiN;
            MatrizRSA[0, 1] = phiN;
            MatrizRSA[1, 0] = e;
            MatrizRSA[1, 1] = 1;

            LlavePrivadaPublica(p, q);

            string a = "";
        }

        public void LlavePrivadaPublica(int p, int q) 
        {
            int Nuevo0=MatrizRSA[0, 0] - (MatrizRSA[0, 0] / MatrizRSA[1, 0]) * MatrizRSA[1, 0];
            int Nuevo1=MatrizRSA[0, 1] - (MatrizRSA[0, 0] / MatrizRSA[1, 0]) * MatrizRSA[1, 1];
            int phi = (p-1)*(q-1);

            if (Nuevo0<0)
            {
                Nuevo0 = Modular(Nuevo0, phi);
            }
            else if(Nuevo1<0)
            {
                Nuevo1 = Modular(Nuevo1,phi);
            }

            MatrizRSA[0, 0] = MatrizRSA[1, 0];
            MatrizRSA[0, 1] = MatrizRSA[1, 1];
            MatrizRSA[1, 0] = Nuevo0;
            MatrizRSA[1, 1] = Nuevo1;

            if (MatrizRSA[1, 0]!=1)
            {
                LlavePrivadaPublica(p, q);
            }
        }
        public List<int> ListaPrimos(List<int> listaV) 
        {
            List<int> NuevaLista = new List<int>();
            int total = 1;
            int a = 0;
            for (int i = 0; i < listaV.Count; i++)
            {
                for (int j = 1; j < listaV[i]+1; j++)
                {
                    if (listaV[i]%j==0)
                    {
                        a++;
                    }
                }
                if (a!=2)
                {

                }
                else
                {
                    NuevaLista.Add(listaV[i]);
                }
                a = 0;
            }
            return NuevaLista;
        }
        public int Modular(int num, int phi) 
        {
            int mod = 0;
            if (num<0)
            {
                int de=(num*-1) / phi;
                 mod = num+phi*(de+1);
            }
            return mod;
        }
        public List<int> CrearLista(int phi, int p, int q)
        {
            List<int> ListaNumeros = new List<int>();
            for (int i = 0; i < phi; i++)
            {
                if (i > 1)
                {
                    if (i % p != 0 && i % q != 0)
                    {
                        ListaNumeros.Add(i);
                    }
                }
            }
            return ListaNumeros;
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

                    Arreglo[contador] = Convert.ToByte((Math.Pow(nuevo, P))%N) ;
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
                    double Elevado = Math.Pow(((double)nuevo), P);
                    double res = Elevado / N;
                    int resMult = (int)res * N;

                    resMult = (int)Elevado - resMult;

                    Arreglo[contador] = (byte)resMult;
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
