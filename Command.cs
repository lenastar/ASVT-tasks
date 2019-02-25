using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FileManager
{
    static class Command
    {
        private const int HistoryTableStart = 17 * 512 + 256;

        public static Result Create(Stream source, IEnumerable<string> args)
        {
            var filename = args.First();
            var history = new byte[19];

            source.Seek(HistoryTableStart + 1, SeekOrigin.Begin);
            source.Read(history, 0, 19);

            var destinition = history.ToList().FindIndex(x => x == 0) + 1;
            source.Seek(HistoryTableStart + destinition, SeekOrigin.Begin);
            source.WriteByte(1);
            
            source.Seek(9 * 512 + 256 * destinition, SeekOrigin.Begin);
            var bytes = Encoding.ASCII.GetBytes(filename);
            source.Write(bytes, 0, bytes.Length);

            source.Seek(destinition * 9 * 1024, SeekOrigin.Begin);

            try
            {
                using (var fs = File.Open(filename, FileMode.Open, FileAccess.Read))
                {
                    if (fs.Length > 9 * 1024)
                        throw new ArgumentOutOfRangeException(filename, $"File {filename} is very big.");

                    fs.CopyTo(source);
                }
            }
            catch (FileNotFoundException e)
            {
                throw new FileNotFoundException($"File Not Found {filename}");
            }

            return new Result($"File {filename} created at {destinition} position");
        }

        public static Result Delete(Stream source, IEnumerable<string> args)
        {
            var pos = int.Parse(args.First());

            source.Seek(HistoryTableStart + pos, SeekOrigin.Begin);
            source.WriteByte(0);

            return new Result($"Position {pos} cleaned" );
        }

        public static Result All(Stream source, IEnumerable<string> args)
        {
            var fileNames = new byte[19 * 256];
            var result = new StringBuilder();
            var history = new byte[19];

            source.Seek(9 * 512, SeekOrigin.Begin);
            source.Read(fileNames, 0, 19 * 256);

            source.Seek(HistoryTableStart, SeekOrigin.Begin);
            source.Read(history, 0, 19);

            for(var i = 0; i < history.Length; i++)
            {
                if (history[i] == 0) continue;

                var filename =
                    string.Join("",
                        Encoding.ASCII.GetString(
                            fileNames
                                .Skip(256 * i)
                                .Take(256)
                                .ToArray()));
                result.AppendLine($"{i} " + filename);
            }

            return new Result(result.ToString());
        }
    }
}