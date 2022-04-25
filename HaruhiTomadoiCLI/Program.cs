using Mono.Options;

namespace HaruhiTomadoiCLI
{
    public class Program
    {
        public static int Main(string[] args)
        {
            CommandSet commands = new("HaruhiTomadoiCLI")
            {
                "Usage: HaruhiTomadoiCLI COMMAND [OPTIONS]",
                "",
                "Available commands:",
                new ExtractDfiCommand(),
                new DecompressPackCommand(),
                new UnpackBinCommand(),
                new RelativeSearchCommand(),
                new ExtractStringsCommand(),
            };

            return commands.Run(args);
        }
    }
}
