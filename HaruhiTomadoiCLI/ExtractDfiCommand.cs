using HaruhiTomadoiLib.Archive;
using Mono.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HaruhiTomadoiCLI
{
    public class ExtractDfiCommand : Command
    {
        private string _dfiFile, _outputDirectory;
        private bool _force;

        public ExtractDfiCommand() : base("extract-dfi", "Extracts a DFI archive to a directory")
        {
            Options = new()
            {
                { "d|dfi=", "DFI to extract", d => _dfiFile = d },
                { "o|output=", "Directory to extract to", o => _outputDirectory = o },
                { "f|force", "Overwrite an existing directory when extracting", f => _force = true },
            };
        }

        public override int Invoke(IEnumerable<string> arguments)
        {
            Options.Parse(arguments);

            string imgFile = "";
            if (_dfiFile.EndsWith(".IMG", StringComparison.OrdinalIgnoreCase))
            {
                imgFile = _dfiFile;
                _dfiFile = _dfiFile.Replace(".IMG", ".IDX", StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                imgFile = _dfiFile.Replace(".IDX", ".IMG", StringComparison.OrdinalIgnoreCase);
            }

            Dfi dfi = new(File.ReadAllBytes(_dfiFile));

            byte[] img = File.ReadAllBytes(imgFile);

            if (Directory.Exists(_outputDirectory) && _force)
            {
                Directory.Delete(_outputDirectory, true);
            }
            else
            {
                Console.WriteLine("ERROR: Directory is not empty!");
                return 1;
            }
            Directory.CreateDirectory(_outputDirectory);

            for (int i = 1; i < dfi.Entries.Count;)
            {
                if (dfi.Entries[i].Type == FileEntry.EntryType.DIRECTORY)
                {
                    i += ExtractDfiDirectory(dfi, img, _outputDirectory, i);
                }
                else
                {
                    ExtractDfiFile(dfi.Entries[i], img, _outputDirectory);
                    i++;
                }
            }

            return 0;
        }

        private static int ExtractDfiDirectory(Dfi dfi, byte[] img, string directory, int dfiEntryIndex)
        {
            string directoryPath = Path.Combine(directory, dfi.Entries[dfiEntryIndex].Name);
            Directory.CreateDirectory(directoryPath);

            int currentDfiIndex = dfiEntryIndex + 1;
            for (int i = 1; i <= dfi.Entries[dfiEntryIndex].NumFiles; i++)
            {
                if (dfi.Entries[currentDfiIndex].Type == FileEntry.EntryType.DIRECTORY)
                {
                    currentDfiIndex += ExtractDfiDirectory(dfi, img, directoryPath, dfiEntryIndex + i);
                }
                else
                {
                    ExtractDfiFile(dfi.Entries[currentDfiIndex], img, directoryPath);
                    currentDfiIndex++;
                }
            }

            return currentDfiIndex - dfiEntryIndex;
        }

        private static void ExtractDfiFile(FileEntry dfiFileEntry, byte[] img, string directory)
        {
            string filePath = Path.Combine(directory, dfiFileEntry.Name);
            File.WriteAllBytes(filePath, img.Skip(dfiFileEntry.FileOffset).Take(dfiFileEntry.FileSize).ToArray());
        }
    }
}
