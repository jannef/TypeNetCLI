using System.IO;

namespace TypeCLI.Policy
{
    public class DefaultNamingPolicy : INamingPolicy
    {
        public readonly string RootOutputDirectory;

        public DefaultNamingPolicy(string rootOutputDirectory)
        {
            RootOutputDirectory = rootOutputDirectory;
        }

        public string OutputLocation(EnhancedType type)
        {
            var outputDirectory = Path.Join(
                RootOutputDirectory,
                Path.Join(type.TrimmedNamespaces));
            Directory.CreateDirectory(outputDirectory);
            return Path.Join(outputDirectory, $"{type.Type.Name}.ts");
        }

        private static string RemoveInvalidChars(string filename)
        {
            return string.Concat(filename.Split(Path.GetInvalidFileNameChars()));
        }
    }
}