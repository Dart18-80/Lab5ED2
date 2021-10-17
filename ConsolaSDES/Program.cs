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
            string total = "C:\\Users\\randr\\OneDrive\\Escritorio\\Nuevoresultado.txt";

            ds.Read_File(Prueba1, respuesa, permutation, 364, 0); //0=Cifrar    1=Decifrar
            ds.Read_File(respuesa, total, permutation, 364, 1); //0=Cifrar    1=Decifrar

            Console.ReadKey();
        }
    }
}
