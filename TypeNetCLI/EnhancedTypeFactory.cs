using System;
using TypeCLI.Policy;

namespace TypeCLI
{
    public class EnhancedTypeFactory
    {
        public readonly INamingPolicy NamingPolicy;
        public readonly string[][] RootNamespaces;
        public readonly Options Options;
        public readonly EnhancedTypeResolver TypeResolver;

        public EnhancedTypeFactory(
            Options options,
            INamingPolicy namingPolicy,
            string[][] rootNamespaces,
            EnhancedTypeResolver typeResolver)
        {
            NamingPolicy = namingPolicy;
            RootNamespaces = rootNamespaces;
            Options = options;
            TypeResolver = typeResolver;
        }

        public EnhancedType CreateEnhancedType(Type type)
        {

            return new(type, RootNamespaces, NamingPolicy, Options, TypeResolver);
        }
    }
}