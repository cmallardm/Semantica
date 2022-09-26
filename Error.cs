//  Carlos Mallard

using System;
using System.IO;

namespace Semantica
{
    public class Error : Exception
    {
        public Error(string mensaje, StreamWriter log) : base(mensaje)
        {
            //Console.WriteLine(mensaje); tamal
            log.WriteLine(mensaje);
        }
    }
}