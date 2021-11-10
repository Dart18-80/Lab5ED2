﻿using System;
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

            Console.WriteLine("Ingresar la direccion de Memoria");
            string Memoria = Console.ReadLine();

            Cifradorsa.GenerarLlaves(p,q, Memoria);

            Console.WriteLine("Ingresar direccion del archivo");
            string Archivo = Console.ReadLine();
            Console.WriteLine("Ingresar direccion donde lo quiero guardar");
            string ArchivoSalida = Console.ReadLine();
            Console.WriteLine("Que llave desea Utilizar?");
            string Key = Console.ReadLine();


            Cifradorsa.CifrarRSA(Archivo, ArchivoSalida, Key);

            Console.WriteLine("Ingresar direccion del archivo a Desifrar");
            Archivo = Console.ReadLine();
            Console.WriteLine("Ingresar direccion donde lo quiero guardar");
            ArchivoSalida = Console.ReadLine();
            Console.WriteLine("Que llave desea Utilizar?");
            Key = Console.ReadLine();

            Cifradorsa.CifrarRSA(Archivo, ArchivoSalida, Key);



        }
    }
}
