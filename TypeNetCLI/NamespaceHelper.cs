using System;
using System.Linq;

namespace TypeCLI
{
    public class NamespaceHelper
    {
        public static string[] GetNameSpaces(string toSplit)
        {
            var split = toSplit.Split('.');

            if (split.Any(x => string.IsNullOrWhiteSpace(x))) {
                throw new ArgumentException("Given namespace seems malformed. There was ");
            }

            return split;
        }

        public static bool IsInNamespace(
            string typeNamespace,
            string[] namespaces)
        {
            typeNamespace ??= string.Empty;

            var splitTypeNamespace = GetNameSpaces(typeNamespace);

            if (namespaces.Length > splitTypeNamespace.Length)
            {
                return false;
            }

            for (var i = 0; i < namespaces.Length; i++)
            {
                if (namespaces[i] != splitTypeNamespace[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}