using HaruhiTomadoiLib.Archive;
using Mono.Options;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HaruhiTomadoiCLI
{
    public class UnpackBinCommand : Command
    {
        private string _binFile, _outputDirectory;
        private bool _force;
        public UnpackBinCommand() : base("unpack-bin", "Unpack a bin archive")
        {
            Options = new()
            {
                { "b|bin=", "Input bin archive", b => _binFile = b },
                { "o|output=", "Output directory", o => _outputDirectory = o },
                { "f|force", "Overwrite existing directory", f => _force = true },
            };
        }

        public override int Invoke(IEnumerable<string> arguments)
        {
            Options.Parse(arguments);
            
            if (Directory.Exists(_outputDirectory) && _force)
            {
                Directory.Delete(_outputDirectory, true);
            }
            else if (Directory.Exists(_outputDirectory))
            {
                CommandSet.Out.WriteLine("ERROR: Directory is not empty!");
                return 1;
            }
            Directory.CreateDirectory(_outputDirectory);

            Bin bin = new(File.ReadAllBytes(_binFile));
            
            for (int i = 0; i < bin.FileEntries.Count; i++)
            {
                byte[] data = bin.FileEntries[i].GetDecompressedData();

                string ext = "bin";
                if (Encoding.ASCII.GetString(data.Take(4).ToArray()) == "TIM2")
                {
                    ext = "tm2";
                }
                File.WriteAllBytes(Path.Combine(_outputDirectory, $"{i:D8}.{ext}"), data);
            }

            return 0;
        }
    }
}
