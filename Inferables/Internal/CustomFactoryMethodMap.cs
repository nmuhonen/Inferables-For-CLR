using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Inferables.Internal
{
    class CustomFactoryMethodMap
    {
        public MethodInfo MethodInfo { get; private set; }
        public Type ReturnType { get; private set; }
        public TypeFactoryMap Map { get; private set; }
        private static string prefix = "get";
        private static int prefixLength = prefix.Length;


        public CustomFactoryMethodMap(MethodInfo methodInfo, BindingRegistry registry)
        {
            string methodName = methodInfo.Name;
            if (!methodName.ToLower().StartsWith(prefix) ||
                methodInfo.GetParameters().Length > 0)
                throw new ArgumentException("Methods on custom types must start with get and must not have any parameters.");

            // TODO: Complete member initialization

            string name = methodName.Length <= prefixLength ? 
                (methodInfo.ReturnType.IsInterface && methodInfo.ReturnType.Name.StartsWith("I") 
                ? methodInfo.ReturnType.Name.Substring(1) : methodInfo.ReturnType.Name)
                : methodInfo.Name.Substring(prefixLength);


            this.Map = registry.GetMapForType(methodInfo.ReturnType, name, false, null);

            this.MethodInfo = methodInfo;
            this.ReturnType = this.MethodInfo.ReturnType;
        }
    }
}
