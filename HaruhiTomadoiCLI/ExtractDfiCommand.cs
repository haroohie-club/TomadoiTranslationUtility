using HaruhiTomadoiLib.Archive;
using Mono.Options;
using System;
using System.Collections.Generic;
using System.IO;

namespace HaruhiTomadoiCLI;

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

        FileStream img = File.OpenRead(imgFile);

        if (Directory.Exists(_outputDirectory) && _force)
        {
            Directory.Delete(_outputDirectory, true);
        }
        else if (Directory.Exists(_outputDirectory))
        {
            Console.WriteLine("ERROR: Directory is not empty!");
            return 1;
        }
        Directory.CreateDirectory(_outputDirectory);

        for (int i = 1; i < dfi.Entries.Count;)
        {
            if (dfi.Entries[i].Type == DfiFileEntry.EntryType.Directory)
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

    private static int ExtractDfiDirectory(Dfi dfi, FileStream img, string directory, int dfiEntryIndex)
    {
        Console.WriteLine($"Extracting directory {dfi.Entries[dfiEntryIndex].Name}...");
        string directoryPath = Path.Combine(directory, dfi.Entries[dfiEntryIndex].Name);
        Directory.CreateDirectory(directoryPath);

        int i;
        for (i = 1; i < dfi.Entries[dfiEntryIndex].NumFiles;)
        {
            if (dfi.Entries[dfiEntryIndex + i].Type == DfiFileEntry.EntryType.Directory)
            {
                i += ExtractDfiDirectory(dfi, img, directoryPath, dfiEntryIndex + i);
            }
            else
            {
                ExtractDfiFile(dfi.Entries[dfiEntryIndex + i], img, directoryPath);
                i++;
            }
        }

        return i;
    }

    private static void ExtractDfiFile(DfiFileEntry dfiFileEntry, FileStream img, string directory)
    {
        Console.WriteLine($"Extracting file {dfiFileEntry.Name}...");
        string filePath = Path.Combine(directory, dfiFileEntry.Name);
        byte[] buffer = new byte[dfiFileEntry.FileSize];
        img.Seek(dfiFileEntry.FileOffset, SeekOrigin.Begin);
        img.ReadExactly(buffer, 0, dfiFileEntry.FileSize);
        File.WriteAllBytes(filePath, buffer);
    }
}