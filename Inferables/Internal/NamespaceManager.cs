using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Inferables.Internal
{
    internal static class NamespaceManager
    {
        private static Dictionary<string, Type[]> typeMap = new Dictionary<string, Type[]>();

        static NamespaceManager()
        {
            try
            {
                var typeMapArray =
                    AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(item => item.SafeGetTypes())
                        .Where(item => item.IsClass && item.IsPublic)
                        .GroupBy(item => GetKey(item.Namespace), (key,coll) =>
                            new {
                                Key = key, 
                                Value = coll
                                    .OrderBy(grItem => grItem.Name)
                                    .OrderByDescending(grItem => grItem.Name.Length)
                            })
                        .ToArray();

                typeMap = typeMapArray
                        .ToDictionary(item => item.Key, item => item.Value.ToArray());
            }
            catch
            {
                throw;
            }
        }

        private static string GetKey(string key)
        {
            return String.IsNullOrWhiteSpace(key) ? String.Empty : key;
        }

        private static Type[] SafeGetTypes(this Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch{}
            return Type.EmptyTypes;
        }

        public static Type[] GetTypesForNamespace(string matchNamespace)
        {
            matchNamespace = String.IsNullOrWhiteSpace(matchNamespace) ? String.Empty : matchNamespace;
            Type[] returnTypes = null;

            if (!typeMap.TryGetValue(matchNamespace, out returnTypes))
                return Type.EmptyTypes;

            return returnTypes;
        }
    }
}
