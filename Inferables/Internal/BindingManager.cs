using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Inferables.Internal
{
    internal static class BindingManager
    {
        private static Dictionary<Binding, BindingRegistry> registryMap = new Dictionary<Binding, BindingRegistry>();


        public static Binding CombineBindings(object[] bindings)
        {
            Binding binding = null;
            if (bindings.Length == 1 && bindings[0] is Binding)
                binding = bindings[0] as Binding;

            if (binding == null)
                binding = new Binding(bindings);
            return binding;
        }

        public static Binding CreateBindingFrom(string bindingTypeName)
        {
            Binding binding = null;
            try
            {
                var bindingType = Type.GetType(bindingTypeName, true);
                binding = (Binding)Activator.CreateInstance(bindingType);
            }
            catch
            {
                throw new ArgumentException("Unable to create a Binding type containing a default constructor from specified typeName '" + bindingTypeName + "'.");
            }
            return binding;
        }

        public static IModule MapBindingToModule(Binding binding)
        {
            var registry = GetRegistry(binding);
            var module = new Module(registry);
            return module;
        }

        private static BindingRegistry GetRegistry(Binding binding)
        {
            BindingRegistry retVal = null;
            if (!registryMap.TryGetValue(binding, out retVal))
            {
                lock (registryMap)
                {
                    if (!registryMap.TryGetValue(binding, out retVal))
                    {
                        retVal = new BindingRegistry(binding);
                        registryMap.Add(binding, retVal);
                    }
                }
            }
            return retVal;
        }

    }
}
