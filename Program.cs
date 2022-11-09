//  Carlos Mallard

using System;
using System.IO;

namespace Semantica
{
    public class Program
    {
        static void Main(string[] args)
        {
            static void instanciaObjeto()
            {
                Lenguaje a = new Lenguaje();
                a.Programa();
            }
            
            try
            {
                instanciaObjeto();
                GC.Collect();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}