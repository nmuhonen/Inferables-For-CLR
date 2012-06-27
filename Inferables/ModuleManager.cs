using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Inferables.Internal;

namespace Inferables
{
    public static class ModuleManager
    {
        public static IModule CreateModule(params object[] bindings)
        {
            Binding binding = BindingManager.CombineBindings(bindings);

            var module = BindingManager.MapBindingToModule(binding);
            return module;
        }

        public static IModule CreateModuleFrom(string typeName)
        {
            Binding binding = BindingManager.CreateBindingFrom(typeName);
            return CreateModule(binding);
        }

        public static T CreateFactory<T>(params object[] bindings)
        {
            var module = CreateModule(bindings);
            return module.GetFactory<T>();
        }

        public static T CreateFactoryFrom<T>(string typeName)
        {
            var module = CreateModuleFrom(typeName);
            return module.GetFactory<T>();
        }

#if DEBUG
        public static void DumpDynamicAssembly(string path = null)
        {
            DynamicAssemblyManager.DumpToFile(path);
        }
#endif

    }
}
