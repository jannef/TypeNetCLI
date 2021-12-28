using System;

namespace TypeCLI.Policy
{
    public interface INamingPolicy
    {
        string OutputLocation(EnhancedType type);
    }
}