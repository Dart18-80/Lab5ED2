using System;
using System.Collections.Generic;
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
            List<int> ListaN = CrearLista(phiN, p, q);

            int index = Rand.Next(0, ListaN.Count);
            int e = ListaN[index];

            MatrizRSA[0, 0] =phiN;
            MatrizRSA[0, 1] = phiN;
            MatrizRSA[1, 0] = e;
            MatrizRSA[1, 1] = 1;

            LlavePrivadaPublica();

            string a = "";
        }

        public void LlavePrivadaPublica() 
        {
            int Nuevo0=MatrizRSA[0, 0] - (MatrizRSA[0, 0] / MatrizRSA[1, 0]) * MatrizRSA[1, 0];
            int Nuevo1=MatrizRSA[0, 1] - (MatrizRSA[0, 0] / MatrizRSA[1, 0]) * MatrizRSA[1, 1];

            MatrizRSA[0, 0] = MatrizRSA[1, 0];
            MatrizRSA[0, 1] = MatrizRSA[1, 1];
            MatrizRSA[1, 0] = Nuevo0;
            MatrizRSA[1, 1] = Nuevo1;

            if (MatrizRSA[1, 0]!=1)
            {
                LlavePrivadaPublica();
            }
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
    }
}
