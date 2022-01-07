using System;

namespace TypeCLI
{
    public class NamespaceHelper
    {
        public static string[] GetNameSpaces(string toSplit)
        {
            return toSplit.Split('.');
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