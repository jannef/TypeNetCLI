using System.Collections.Generic;
using CommandLine;

namespace TypeCLI
{
    public class Options
    {
        [Option('d', "dll", HelpText = "The Dll to read th option from", Required = true)]
        public string Dll { get; set; }

        [Option('n', "namespaces", HelpText = "Root namespaces to convert. These are merged into one root namespace in generated ts.")]
        public IEnumerable<string> Namespaces { get; set; }

        [Option('o', "output", HelpText = "Output directory. Will not write the files if omitted.")]
        public string OutputDirectory { get; set; }

        [Option(
            "tab",
            HelpText = "Tab style. Default is \"  \" (two spaces)",
            Default = "  ")]
        public string TabStyle { get; set; }
    }
}