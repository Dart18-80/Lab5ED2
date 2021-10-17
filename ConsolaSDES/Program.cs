using System;
using Library_SDES;

namespace ConsolaSDES
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            SDES ds = new SDES();
            string Prueba1 = "C:\\Users\\randr\\OneDrive\\Escritorio\\prueba.txt";
            string respuesa = "C:\\Users\\randr\\OneDrive\\Escritorio\\Respuesta.txt";
            string permutation = "C:\\Users\\randr\\OneDrive\\Escritorio\\Permutations.txt";
            ds.Read_File(Prueba1, respuesa, permutation, 364);
            Console.ReadKey();
        }
    }
}
