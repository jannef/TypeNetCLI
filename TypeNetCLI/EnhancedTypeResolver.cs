using System;
using System.Collections.Generic;
using System.Linq;

namespace TypeCLI
{
    public class EnhancedTypeResolver
    {
        public HashSet<EnhancedType> EnhancedTypes;

        public EnhancedType ResolveType(Type type)
        {
            if (EnhancedTypes == null)
            {
                throw new InvalidOperationException("Types not registered!");
            }

            return EnhancedTypes.FirstOrDefault(x => x.Type == type)
                ?? throw new UnknownTypeException(type);
        }

        public void RegisterTypes(HashSet<EnhancedType> types)
        {
            EnhancedTypes = types;
        }
    }
}