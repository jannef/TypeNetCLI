using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TypeCLI.Policy;

namespace TypeCLI
{
    public class EnhancedType
    {
        public readonly Type Type;
        public readonly INamingPolicy NamingPolicy;
        public readonly string[] Namespaces;
        public readonly string[] TrimmedNamespaces;

        public EnhancedType(
            Type type,
            IEnumerable<string[]> rootNamespaces,
            INamingPolicy namingPolicy)
        {
            Type = type;
            Namespaces = NamespaceHelper.GetNameSpaces(type.Namespace);
            NamingPolicy = namingPolicy;

            var toTrim = rootNamespaces
                .OrderByDescending(x => x.Length)
                .FirstOrDefault(x => NamespaceHelper.IsInNamespace(type.Namespace, x))?
                .Length ?? 0;

            TrimmedNamespaces = Namespaces.Skip(toTrim).ToArray();
        }

        public void WriteType()
        {
            var sb = new StringBuilder();

            foreach (var propertyInfo in Type.GetProperties())
            {
                sb.AppendLine($"  {propertyInfo.Name}: {propertyInfo.PropertyType.Name},");
            }

            if (sb.Length > 0)
            {
                var path = NamingPolicy.OutputLocation(this);
                Console.WriteLine($"Writing {path}");
                File.WriteAllText(path, $"export class {Type.Name}" +
                                        $"{{\n" +
                                        $"{sb}}}");
            }
        }

        public EnhancedType[] GetInternalDependencies()
        {
            throw new NotImplementedException();
        }
    }
}