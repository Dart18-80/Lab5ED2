using System;
using Library_SDES;        
namespace ConsolaRSA
{
    class Program
    {
        static void Main(string[] args)
        {
            LRSA Cifradorsa = new LRSA();
            int p = 0, q = 0; 
            do
            {
                Console.WriteLine("Cifrado RSA!");
                Console.WriteLine("Ingrese un numero primo p");
                p = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Ingrese un numero primo q");
                q = Convert.ToInt32(Console.ReadLine());
            } while (Cifradorsa.ComprobacionLlavesPrimos(p,q) != true);

            Cifradorsa.GenerarLlaves(p,q);


        }
    }
}
