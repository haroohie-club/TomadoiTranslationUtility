using Ionic.Zlib;

namespace HaruhiTomadoiLib.Archive;

public class Pack
{
    public static byte[] Decompress(IEnumerable<byte> compressedData)
    {
        byte[] postSignatureData = compressedData.Skip(0x10).ToArray();

        ZlibStream zlibStream = new(new MemoryStream(postSignatureData), CompressionMode.Decompress);
        byte[] buffer = new byte[10240];
        List<byte> decompressedData = [];

        int count = -1;
        while (count != 0)
        {
            count = zlibStream.Read(buffer, 0, buffer.Length);
            decompressedData.AddRange(buffer);
        }

        return decompressedData.ToArray();
    }
}