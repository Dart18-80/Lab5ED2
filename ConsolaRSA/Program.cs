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
            string Prueba1 = "C:\\Users\\randr\\OneDrive\\Escritorio\\hard-test.txt";
            string Resultado= "C:\\Users\\randr\\OneDrive\\Escritorio\\Resultado.txt";
            string NuevoResultado = "C:\\Users\\randr\\OneDrive\\Escritorio\\NuevoResultado.txt";
            Cifradorsa.CifrarRSA(Prueba1, Resultado, 41, 77);
            Cifradorsa.CifrarRSA(Resultado, NuevoResultado, 101, 77);


        }
    }
}
