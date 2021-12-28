using System;
using System.IO;
using System.Linq;
using System.Reflection;
using CommandLine;
using TypeCLI.Policy;

namespace TypeCLI
{
    public class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(o =>
                {
                    var assembly = Assembly.LoadFrom(o.Dll);
                    Console.WriteLine($"Loaded: {assembly} from {o.Dll}");

                    var outputDirectory = Directory.GetCurrentDirectory();

                    if (!string.IsNullOrEmpty(o.OutputDirectory))
                    {
                        var createdDirectory = Directory.CreateDirectory(o.OutputDirectory);
                        outputDirectory = createdDirectory.FullName;
                        Console.WriteLine($"Output directory: {outputDirectory}");
                    }

                    var namespaces = o.Namespaces
                        .Select(NamespaceHelper.GetNameSpaces)
                        .ToArray();

                    var namingPolicy = new DefaultNamingPolicy(outputDirectory);
                    var typeFactory = new EnhancedTypeFactory(namingPolicy, namespaces);

                    var filteredTypes = assembly.GetTypes()
                        .Where(x =>
                        {
                            Console.WriteLine($"{x.FullName}");
                            return namespaces.Any(y => NamespaceHelper.IsInNamespace(x.Namespace, y));
                        })
                        .Select(x => typeFactory.CreateEnhancedType(x))
                        .ToHashSet();

                    var numberOfTypes = filteredTypes.Count;
                    Console.WriteLine($"{numberOfTypes} types matched");

                    foreach (var filteredType in filteredTypes)
                    {
                        filteredType.WriteType();
                    }
                });
        }
    }
}
