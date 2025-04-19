namespace HaruhiTomadoiLib.Font;

public class CustomEncoding
{
    public Dictionary<short, string> Charset { get; set; } = new();

    public CustomEncoding(string charsetCsv)
    {
        foreach (string line in File.ReadAllLines(charsetCsv))
        {
            if (string.IsNullOrEmpty(line))
            {
                continue;
            }

            string[] split = line.Split(',');

            if (split.Length > 2)
            {
                Charset.Add(short.Parse(split[2]), ",");
            }
            else
            {
                Charset.Add(short.Parse(split[1]), split[0]);
            }
        }
    }

    public string GetString(byte[] bytes)
    {
        string result = "";
        for (int i = 0; i < bytes.Length - 1; i+= 2)
        {
            result += Charset.GetValueOrDefault(BitConverter.ToInt16(bytes.Skip(i).Take(2).ToArray()), "@");
        }

        return result;
    }
}