using System;
using System.Linq;

namespace PokyPoker.ConsoleApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            var arr = Guid.NewGuid().ToByteArray().Concat(Guid.NewGuid().ToByteArray()).ToArray();

            Console.Write(Convert.ToBase64String(arr));
            Console.ReadLine();
        }
    }
}
