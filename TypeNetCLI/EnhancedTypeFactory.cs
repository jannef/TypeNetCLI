using System;
using TypeCLI.Policy;

namespace TypeCLI
{
    public class EnhancedTypeFactory
    {
        public readonly INamingPolicy NamingPolicy;
        public readonly string[][] RootNamespaces;

        public EnhancedTypeFactory(
            INamingPolicy namingPolicy,
            string[][] rootNamespaces)
        {
            NamingPolicy = namingPolicy;
            RootNamespaces = rootNamespaces;
        }

        public EnhancedType CreateEnhancedType(Type type) =>
            new(type, RootNamespaces, NamingPolicy);
    }
}