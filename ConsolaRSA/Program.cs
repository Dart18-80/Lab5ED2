using System;
using Library_SDES;        
namespace ConsolaRSA
{
    class Program
    {
        static void Main(string[] args)
        {
            LRSA Cifradorsa = new LRSA();
            Console.WriteLine("Cifrado RSA!");
            Console.WriteLine("Ingrese un numero primo p");
            int p = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Ingrese un numero primo q");
            int q = Convert.ToInt32(Console.ReadLine());

            Cifradorsa.CreacionLlaves(p, q);
        }
    }
}
