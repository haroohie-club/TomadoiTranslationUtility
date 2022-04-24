using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaruhiTomadoiLib.Archive
{
    public class Bin
    {
        public int NumFiles { get; set; }
        public List<BinFileEntry> FileEntries { get; set; } = new();

        public Bin(IEnumerable<byte> data)
        {
            if (Encoding.ASCII.GetString(data.Take(6).ToArray()) == "Packed")
            {
                data = Pack.Decompress(data);
            }
            NumFiles = BitConverter.ToInt32(data.Take(4).ToArray());
            for (int i = 4; i < NumFiles * 8 + 4; i += 8)
            {
                FileEntries.Add(new(data, i));
            }
        }
    }

    public class BinFileEntry
    {
        public int Offset { get; set; }
        public int Length { get; set; }
        public List<byte> Data { get; set; } = new();
        public bool Compressed { get; set; }

        public BinFileEntry(IEnumerable<byte> data, int offset)
        {
            Offset = BitConverter.ToInt32(data.Skip(offset).Take(4).ToArray());
            Length = BitConverter.ToInt32(data.Skip(offset + 4).Take(4).ToArray());
            Data = data.Skip(Offset).Take(Length).ToList();
            Compressed = Encoding.ASCII.GetString(Data.Take(6).ToArray()) == "Packed";
        }

        public byte[] GetDecompressedData()
        {
            if (Compressed)
            {
                return Pack.Decompress(Data);
            }
            else
            {
                return Data.ToArray();
            }
        }
    }
}
