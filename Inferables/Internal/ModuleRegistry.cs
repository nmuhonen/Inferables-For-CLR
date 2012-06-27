using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Inferables.GenBindings;

namespace Inferables.Internal
{
    internal class ModuleRegistry
    {
        private Dictionary<IFactoryLoader, ITypeFactory> cachedTypeFactories;
        private Dictionary<Type,Dictionary<string,ITypeFactory>> typeFactoryMap;
        private Dictionary<Type, object> cachedCustomFactories;
        public BindingRegistry BindingRegistry { get; private set; }
        private object syncLock = new object();

        public ModuleRegistry(BindingRegistry registry)
        {
            this.cachedCustomFactories = new Dictionary<Type, object>();
            this.typeFactoryMap = new Dictionary<Type,Dictionary<string,ITypeFactory>>();
            this.cachedTypeFactories = new Dictionary<IFactoryLoader, ITypeFactory>();
            this.BindingRegistry = registry;
        }

        public ITypeFactory GetTypeFactory(Type type, bool isExplicitType, string findType,  IInjectableModule module)
        {
            findType = String.IsNullOrWhiteSpace(findType) ? GetDefaultNameFromType(type) : findType;
            string key = findType.ToLower();

            Dictionary<string, ITypeFactory> cachedTypeFactory = null;
            if (! typeFactoryMap.TryGetValue(type, out cachedTypeFactory))
            {
                lock (syncLock)
                {
                    if (!typeFactoryMap.TryGetValue(type, out cachedTypeFactory))
                    {
                        cachedTypeFactory = new Dictionary<string, ITypeFactory>();
                        typeFactoryMap.Add(type,cachedTypeFactory);
                    }
                }
            }
            ITypeFactory retValue = null;
            if (! cachedTypeFactory.TryGetValue(key, out retValue))
            {
                lock (syncLock)
                {
                    if (!cachedTypeFactory.TryGetValue(key, out retValue))
                    {
                        var loader = BindingRegistry.GetLoaderForType(type, isExplicitType, findType);
                        retValue = GetFactory(module, loader);
                        cachedTypeFactory.Add(key,retValue);
                    }
                }
            }
            return retValue;
        }

        private ITypeFactory GetFactory(IInjectableModule module, IFactoryLoader loader)
        {
            ITypeFactory retVal = null;

            if (! this.cachedTypeFactories.TryGetValue(loader, out retVal))
            {
                lock(syncLock)
                {
                    if (! this.cachedTypeFactories.TryGetValue(loader, out retVal))
                    {
                        retVal = (ITypeFactory)loader.CreateFactory(module);
                        cachedTypeFactories.Add(loader, retVal);
                    }
                }
            }

            return retVal;
        }





        public object GetCustomFactory(Type type, IInjectableModule module)
        {
            object retValue = null;
            if (! cachedCustomFactories.TryGetValue(type, out retValue))
            {
                lock (syncLock)
                {
                    if (!cachedCustomFactories.TryGetValue(type, out retValue))
                    {
                        var loader = BindingRegistry.GetLoaderForFactory(type);
                        retValue = loader.CreateFactory(module);
                        cachedCustomFactories.Add(type, retValue);
                    }
                }
            }
            return retValue;

        }





        private string GetDefaultNameFromType(Type baseType)
        {
            if (baseType.IsInterface && baseType.Name.StartsWith("I"))
                return baseType.Name.Substring(1);

            return baseType.Name;
        }
    }

}
