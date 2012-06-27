using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Inferables.GenBindings;

namespace Inferables.Internal
{
    internal class BindingRegistry
    {
        public Binding Binding {get; private set;}
        private Dictionary<Type,Dictionary<string,TypeFactoryMap>> typeFactoryLoaderMap;
        private Dictionary<TypeFactoryDefinition, TypeFactoryBinding> typeFactoryLoaderCache;
        private object syncLock = new object();
        private Dictionary<TypeFactoryDefinition, TypeFactoryDefinition> typeFactoryDefinitions;
        private Dictionary<Type, IFactoryLoader> customFactoryLoaderCache;

        public BindingRegistry(Binding binding)
        {
            this.Binding = binding;
            typeFactoryLoaderMap = new Dictionary<Type,Dictionary<string,TypeFactoryMap>>();
            typeFactoryLoaderCache = new Dictionary<TypeFactoryDefinition, TypeFactoryBinding>();
            typeFactoryDefinitions =  new Dictionary<TypeFactoryDefinition, TypeFactoryDefinition>();
            customFactoryLoaderCache = new Dictionary<Type, IFactoryLoader>();
        }

        public IFactoryLoader GetLoaderForType(Type baseType, bool isExplicitType, string name)
        {
            var map = GetMapForType(baseType, name, isExplicitType, null);
            return GetBindingFromMap(map).Loader;
        }

        public TypeFactoryBinding GetBindingFromMap(TypeFactoryMap map)
        {
            TypeFactoryBinding retVal = null;

            if (!typeFactoryLoaderCache.TryGetValue(map.Definition, out retVal))
            {
                lock (syncLock)
                {
                    if (!typeFactoryLoaderCache.TryGetValue(map.Definition, out retVal))
                    {
                        retVal = map.Definition.GetBinding();
                        typeFactoryLoaderCache.Add(map.Definition, retVal);
                    }
                }
            }
            return retVal;
        }

        public TypeFactoryMap GetMapForType(Type baseType, string name, bool isExplicitType, Stack<TypeFactoryMap> currentStack)
        {

            string key = name.ToLower();
            Dictionary<string,TypeFactoryMap> typeLoaderMap = null;
            if (!typeFactoryLoaderMap.TryGetValue(baseType, out typeLoaderMap))
            {
                lock(syncLock)
                {
                    if (!typeFactoryLoaderMap.TryGetValue(baseType, out typeLoaderMap))
                    {
                        typeLoaderMap = new Dictionary<string,TypeFactoryMap>();
                        typeFactoryLoaderMap.Add(baseType,typeLoaderMap);
                    }
                }
            }

            TypeFactoryMap returnMap = null;
            if (!typeLoaderMap.TryGetValue(key, out returnMap))
            {
                lock(syncLock)
                {
                    if (!typeLoaderMap.TryGetValue(key, out returnMap))
                    {
                        if (currentStack == null)
                            currentStack = new Stack<TypeFactoryMap>();
                        if (currentStack.Any(item => item.Name.ToLowerInvariant() == key && item.BaseType == baseType))
                            throw new ArgumentException("Mapping for type '" + baseType.FullName + "' on name '" + name + "' has a circular dependency.");
                        returnMap = new TypeFactoryMap(name, baseType, isExplicitType, this);
                        currentStack.Push(returnMap);
                        returnMap.Init(currentStack);
                        currentStack.Pop();
                        typeLoaderMap.Add(key,returnMap);
                        if (returnMap.Definition.IsSingleton)
                            GetBindingFromMap(returnMap);
                    }
                }
            }

            return returnMap;
        }


        public TypeFactoryDefinition GetTypeFactoryDefinition(TypeFactoryMap map, Stack<TypeFactoryMap> current)
        {
            var definition = new TypeFactoryDefinition(map, current);
            TypeFactoryDefinition result = null;

            if (!typeFactoryDefinitions.TryGetValue(definition, out result))
            {
                lock (syncLock)
                {
                    if (!typeFactoryDefinitions.TryGetValue(definition, out result))
                    {
                        typeFactoryDefinitions.Add(definition, definition);
                        result = definition;
                    }
                }
            }

            return result;
        }


        public IFactoryLoader GetLoaderForFactory(Type creationType)
        {
            IFactoryLoader loader = null;
            if (!customFactoryLoaderCache.TryGetValue(creationType, out loader))
            {
                lock(syncLock)
                {
                    if (!customFactoryLoaderCache.TryGetValue(creationType, out loader))
                    {
                        var definition = new CustomFactoryDefinition(creationType, this);
                        var builder = new CustomFactoryGenerator(definition);
                        loader = builder.CreateLoader();
                        customFactoryLoaderCache.Add(creationType, loader);
                    }
                }
            }

            return loader;
        }

        internal TypeFactoryBinding GetTypeFactoryBinding(Type type, bool isExplicitType, string name)
        {
            var map = GetMapForType(type, name, isExplicitType, null);
            var binding = GetBindingFromMap(map);
            return binding;
        }
    }
   
}