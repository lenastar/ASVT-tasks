using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileManager
{
    class Program
    {
        static void Main(string[] args)
        {
            var img = args[0];
            var fs = File.Open(img, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            var command = args[1];
            var arguments = args.Length > 2 ? args.Skip(2) : null;

            var result = Resolver.Commands[command](fs, arguments);
            fs.Close();
            
            Console.WriteLine(result.Message);
        }
    }
}
