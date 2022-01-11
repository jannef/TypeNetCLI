using System;

namespace TypeCLI
{
    public class UnknownTypeException : ApplicationException
    {
        public UnknownTypeException(Type type) :
            base($"Referenced type {type.FullName} not converted.") { }
    }
}