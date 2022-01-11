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
        public static readonly Dictionary<string, string> WellKnownTranslations = new ()
        {
            {nameof(String), "string"},
            {nameof(Guid), "string"},
            {nameof(DateTime), "string"},
            {nameof(Int16), "number"},
            {nameof(Int32), "number"},
            {nameof(Int64), "number"},
            {nameof(Decimal), "number"},
            {nameof(Double), "number" },
            {nameof(Boolean), "boolean"},
        };

        public readonly Type Type;
        public readonly INamingPolicy NamingPolicy;
        public readonly string[] Namespaces;
        public readonly string[] TrimmedNamespaces;
        public readonly Options Options;
        public readonly EnhancedTypeResolver TypeResolver;

        public EnhancedType(
            Type type,
            IEnumerable<string[]> rootNamespaces,
            INamingPolicy namingPolicy,
            Options options,
            EnhancedTypeResolver typeResolver)
        {
            Type = type;
            Namespaces = NamespaceHelper.GetNameSpaces(type.Namespace);
            NamingPolicy = namingPolicy;
            Options = options;
            TypeResolver = typeResolver;

            var toTrim = rootNamespaces
                .OrderByDescending(x => x.Length)
                .FirstOrDefault(x => NamespaceHelper.IsInNamespace(type.Namespace, x))?
                .Length ?? 0;

            TrimmedNamespaces = Namespaces.Skip(toTrim).ToArray();
        }

        private void WriteClass()
        {
            var propertyStringBuilder = new StringBuilder();
            var toImport = new HashSet<(EnhancedType typeToImport, string typescriptName)>();

            foreach (var propertyInfo in Type.GetProperties())
            {
                var propertyType = GetPropertyType(propertyInfo.PropertyType);
                propertyStringBuilder.AppendLine($"{Options.TabStyle}{propertyInfo.Name}: {propertyType.TypescriptName}");
                
                if (propertyType?.Type != null)
                {
                    toImport.Add((propertyType.Type, propertyType.ImportName));
                }
            }

            if (propertyStringBuilder.Length > 0)
            {
                var classStringBuilder = new StringBuilder();

                foreach (var import in toImport)
                {                    
                    classStringBuilder.AppendLine(
                        $"import {{ {import.typescriptName} }} from '{NamingPolicy.ImportPath(import.typeToImport)}'");
                }

                classStringBuilder.AppendLine($"export class {Type.Name} ");
                classStringBuilder.AppendLine("{");
                classStringBuilder.AppendLine($"{propertyStringBuilder}}}");

                var path = NamingPolicy.OutputLocation(this);
                File.WriteAllText(path, classStringBuilder.ToString());
            }
        }

        private string ReplaceWithWellKnownType(string typeName, out bool replaced)
        {
            replaced = WellKnownTranslations.ContainsKey(typeName);
            return replaced
                ? WellKnownTranslations[typeName]
                : typeName;
        }

        private ImportReference GetPropertyType(Type propertyType)
        {
            try
            {
                // Nullable
                var nullablePropertyType = Nullable.GetUnderlyingType(propertyType);
                if (nullablePropertyType != null)
                {
                    var nullableTypeName = ReplaceWithWellKnownType(nullablePropertyType.Name, out var wellKnownType);
                    
                    return new()
                    {
                        TypescriptName = $"{nullablePropertyType} | null",
                        Type = wellKnownType ? null : TypeResolver.ResolveType(nullablePropertyType),
                        ImportName = nullableTypeName
                    };
                }

                // Arrays
                if (propertyType.IsArray)
                {
                    var arrayType = propertyType.GetElementType() ?? throw new TypeAccessException();
                    var arrayTypeName = ReplaceWithWellKnownType(arrayType.Name, out var wellKnownType);
                    
                    return new()
                    {
                        TypescriptName = $"{arrayTypeName}[]",
                        Type = wellKnownType ? null : TypeResolver.ResolveType(arrayType),
                        ImportName = arrayTypeName
                    };
                }

                // Lists
                if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    var listType = propertyType.GetGenericArguments().First();
                    var listTypeName = ReplaceWithWellKnownType(listType.Name, out var wellKnownType);
                    
                    return new()
                    {
                        TypescriptName = $"{listTypeName}[]",
                        Type = wellKnownType ? null : TypeResolver.ResolveType(listType),
                        ImportName = listTypeName
                    };
                }

                // Dictionaries -- does not support importable key yet :(
                if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                {
                    var keyType = propertyType.GetGenericArguments().First();
                    var valueType = propertyType.GetGenericArguments().Last();

                    var key = ReplaceWithWellKnownType(keyType.Name, out var wellKnownKeyType);
                    var value = ReplaceWithWellKnownType(valueType.Name, out var wellKnownValueType);

                    return new()
                    {
                        TypescriptName = $"{{ [key:{key}]: {value} }}",
                        Type = wellKnownValueType ? null : TypeResolver.ResolveType(valueType),
                        ImportName = value
                    };
                }

                // Everything else
                var typeName = ReplaceWithWellKnownType(propertyType.Name, out var wellKnownTyped);

                return new()
                {
                    TypescriptName = typeName,
                    Type = wellKnownTyped ? null : TypeResolver.ResolveType(propertyType),
                    ImportName = typeName
                };
            }
            // type it any, everything craps out
            catch (UnknownTypeException e)
            {
                Console.WriteLine($"{e.Message} Falling back to 'any'.");
                return new ()
                {
                    TypescriptName = ReplaceWithWellKnownType(propertyType.Name, out var _)
                };
            }
        }

        private void WriteEnum()
        {
            var underlyingType = Enum.GetUnderlyingType(Type);
            var enumValues = Enum.GetValues(Type);
            var sb = new StringBuilder();

            foreach (var enumValue in enumValues)
            {
                var underlyingValue = Convert.ChangeType(enumValue, underlyingType);
                sb.AppendLine($"{Options.TabStyle}{enumValue} = {underlyingValue},");
            }

            if (sb.Length > 0)
            {
                var path = NamingPolicy.OutputLocation(this);
                File.WriteAllText(path, $"export enum {Type.Name} " +
                                        $"{{\n" +
                                        $"{sb}}}");
            }
        }

        public void WriteType()
        {
            if (Type.IsClass || Type.IsInterface)
            {
                WriteClass();
            } else if (Type.IsEnum)
            {
                WriteEnum();
            }
        }
    }
}