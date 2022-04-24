using HaruhiTomadoiLib.Archive;
using Mono.Options;
using System.Collections.Generic;
using System.IO;

namespace HaruhiTomadoiCLI
{
    public class DecompressPackCommand : Command
    {
        private string _inputPack, _outputFile;
        public DecompressPackCommand() : base("decompress-pack", "Decompressed a packed file")
        {
            Options = new()
            {
                { "p|pack=", "Input pack file", p => _inputPack = p },
                { "o|output=", "Output decompressed file", o => _outputFile = o },
            };
        }

        public override int Invoke(IEnumerable<string> arguments)
        {
            Options.Parse(arguments);

            File.WriteAllBytes(_outputFile, Pack.Decompress(File.ReadAllBytes(_inputPack)));

            return 0;
        }
    }
}
