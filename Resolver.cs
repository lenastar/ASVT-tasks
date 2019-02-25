using System.Collections.Generic;
using System.IO;

namespace FileManager
{
    static class Resolver
    {
        public delegate Result ConsoleCommand(Stream source, IEnumerable<string> args);
        public static readonly Dictionary<string, ConsoleCommand> Commands = new Dictionary<string, ConsoleCommand>
        {
            {"Create", Command.Create},
            {"Delete", Command.Delete},
            {"All", Command.All}
        };
    }
}