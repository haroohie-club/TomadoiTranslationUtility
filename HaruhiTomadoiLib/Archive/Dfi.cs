using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaruhiTomadoiLib.Archive;

// This structure described here: https://github.com/pleonex/Boku-no-Natsuyasumi/wiki/Pack-file-cdimg
public class Dfi
{
    public int Unknown1 { get; set; }
    public List<DfiFileEntry> Entries { get; set; } = [];

    public Dfi(byte[] data)
    {
        if (Encoding.ASCII.GetString(data.Take(3).ToArray()) != "DFI")
        {
            throw new ArgumentException($"File is not a DFI file");
        }

        Unknown1 = BitConverter.ToInt32(data.Skip(4).Take(4).ToArray());
            
        int nameTableOffset = BitConverter.ToInt32(data.Skip(0x14).Take(4).ToArray()) + 0x10; // first name table entry

        for (int i = 0x10; i < nameTableOffset; i += 0x10)
        {
            Entries.Add(new(i, data));
        }
    }
}

public class DfiFileEntry
{
    public enum EntryType
    {
        File = 0,
        Directory = 1,
    }
    public EntryType @Type { get; set; }
    public short NumFiles { get; set; }
    public int RelativeNameOffset { get; set; }
    public string Name { get; set; }
    public uint FileOffset { get; set; }
    public int FileSize { get; set; }

    public DfiFileEntry(int offset, byte[] data)
    {
        Type = (EntryType)BitConverter.ToInt16(data.Skip(offset).Take(2).ToArray());
        NumFiles = BitConverter.ToInt16(data.Skip(offset + 2).Take(2).ToArray());
        RelativeNameOffset = BitConverter.ToInt32(data.Skip(offset + 4).Take(4).ToArray());
        Name = Encoding.ASCII.GetString(data.Skip(offset + RelativeNameOffset).TakeWhile(b => b != 0x00).ToArray());
        FileOffset = BitConverter.ToUInt32(data.Skip(offset + 8).Take(4).ToArray()) * 0x800;
        FileSize = BitConverter.ToInt32(data.Skip(offset + 12).Take(4).ToArray());
    }

    public override string ToString()
    {
        return $"{Type}: {Name}{(Type == EntryType.File ? $"({FileSize} bytes" : "")})";
    }
}