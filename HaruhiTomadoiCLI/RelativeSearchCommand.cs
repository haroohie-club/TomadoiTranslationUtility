using Mono.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HaruhiTomadoiCLI
{
    public class RelativeSearchCommand : Command
    {
        private string _inputDirectory;
        private int[] _search;
        private int _numBytes = 1;

        public RelativeSearchCommand() : base("relative-search", "Performs a relative search in a directory")
        {
            Options = new()
            {
                { "i|input=", "Directory to search", i => _inputDirectory = i },
                { "s|search=", "Integer array to search for, space-delimited", s => _search = s.Split(' ').Select(i => int.Parse(i)).ToArray() },
                { "b|bits=", "Num bits to consider (8, 16, 32)", b => _numBytes = int.Parse(b) / 8 },
            };
        }

        public override int Invoke(IEnumerable<string> arguments)
        {
            Options.Parse(arguments);

            _search = _search.Select(i => i - _search.Min()).ToArray(); // Relative value

            string[] files = Directory.GetFiles(_inputDirectory, "*.*", SearchOption.AllDirectories);

            foreach (string file in files)
            {
                byte[] data = File.ReadAllBytes(file);
                for (int i = 0; i < data.Length - _search.Length * _numBytes; i++)
                {
                    IEnumerable<byte> segmentBytes = data.Skip(i).Take(_search.Length * _numBytes);
                    List<int> segment = new();
                    for (int j = 0; j < segmentBytes.Count(); j += _numBytes)
                    {
                        List<byte> bytes = segmentBytes.Skip(j).Take(_numBytes).ToList();
                        for (int k = bytes.Count; k < 4; k++)
                        {
                            bytes.Add(0);
                        }
                        segment.Add(BitConverter.ToInt32(bytes.ToArray()));
                    }

                    if (_search.SequenceEqual(segment.Select(s => s - segment.Min())))
                    {
                        CommandSet.Out.WriteLine($"Found matching sequence at 0x{i:X8} in file {file}");
                    }
                }
            }

            return 0;
        }
    }
}
