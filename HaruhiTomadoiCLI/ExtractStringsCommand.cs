using HaruhiTomadoiLib.Font;
using Mono.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HaruhiTomadoiCLI
{
    public class ExtractStringsCommand : Command
    {
        string _inputFile, _outputFile, _charsetFile;
        public ExtractStringsCommand() : base("extract-strings", "Extract strings from a file")
        {
            Options = new()
            {
                { "i|input=", "Input file", i => _inputFile = i },
                { "o|output=", "Output file", o => _outputFile = o },
                { "c|charset=", "Charset file", c => _charsetFile = c },
            };
        }

        public override int Invoke(IEnumerable<string> arguments)
        {
            Options.Parse(arguments);

            CustomEncoding encoding = new(_charsetFile);

            string encodedString = encoding.GetString(File.ReadAllBytes(_inputFile));
            List<string> lines = new();
            for (int i = 0; i < encodedString.Length; i += 8)
            {
                lines.Add(encodedString[i..(Math.Min(i + 8, encodedString.Length))]);
            }

            File.WriteAllLines(_outputFile, lines);

            return 0;
        }
    }
}
