using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Inferables.GenBindings;

namespace Inferables.Internal
{
    internal class TypeFactoryDefinition: Hashable<TypeFactoryDefinition>
    {
        public Type MappedType { get; private set; }
        public TypeFactoryMap[] ParameterTypeMaps { get; private set; }
        public ConstructorInfo ConstructorInfo { get; private set; }
        public MethodInfo FactoryMethodInfo { get; private set; }
        public bool IsFactoryMethod { get; private set; }
        public bool IsSingleton { get; private set; }
        public bool IsShared { get; private set; }

        private static int factoryMethodSkip = "Create".Length;

        public TypeFactoryDefinition(TypeFactoryMap map, Stack<TypeFactoryMap> currentStack)
        {
            string name = map.Name;
            Type baseType = map.BaseType;
            BindingRegistry registry = map.Registry;
            bool isExplicitType = map.IsExplicitType;

            MapType(name, baseType, isExplicitType, registry);

            var parameters = IsFactoryMethod ? FactoryMethodInfo.GetParameters() : ConstructorInfo.GetParameters();

            ParameterTypeMaps = parameters
                .Select(item => registry.GetMapForType(item.ParameterType, item.Name, false, currentStack))
                .ToArray();

            if (IsFactoryMethod)
            {
                var matchPattern = FactoryMethodInfo.Name.Substring(factoryMethodSkip).ToLower();
                IsSingleton = matchPattern.StartsWith("singleton");
                IsShared = matchPattern.StartsWith("shared");
            }
            else
            {
                IsSingleton = MappedType.Name.ToLower().StartsWith("singleton");
                IsShared = MappedType.Name.ToLower().StartsWith("shared");
            }

            this.HashCode = IsFactoryMethod.GetHashCode() + (IsFactoryMethod ? FactoryMethodInfo.GetHashCode() : ConstructorInfo.GetHashCode()) + IsSingleton.GetHashCode() + IsShared.GetHashCode();
        }

        private void MapType(string name, Type baseType, bool isExplicitType, BindingRegistry registry)
        {
            Type alternativeType = null;
            ConstructorInfo alternativeConstructorInfo = null;

            if (isExplicitType)
            {
                CheckForFactoryMethod(name, baseType, baseType);
                if (IsFactoryMethod)
                    return;

                var constructor = GetConstructorInfoIfValid(baseType);
                if (constructor != null)
                {
                    SetInstanceTypeMap(baseType, constructor);
                    return;
                }
            }
            else
            {
                foreach (BindingMap map in registry.Binding.Maps)
                {
                    string typeNamespace = map.GetBindingNamespace(baseType);
                    if (typeNamespace == null)
                        continue;

                    foreach (var type in NamespaceManager.GetTypesForNamespace(typeNamespace))
                    {
                        CheckForFactoryMethod(name, baseType, type);
                        if (IsFactoryMethod)
                            return;

                        if (!baseType.IsAssignableFrom(type) || type.IsAbstract)
                            continue;

                        var constructor = GetConstructorInfoIfValid(type);
                        if (constructor == null)
                            continue;

                        if (type.Name.ToLower().EndsWith(name.ToLower()))
                        {
                            if (this.MappedType == null)
                            {
                                SetInstanceTypeMap(type, constructor);
                            }
                            continue;
                        }

                        if (alternativeType == null)
                        {
                            alternativeType = type;
                            alternativeConstructorInfo = constructor;
                        }
                    }

                    if (this.MappedType != null)
                        return;
                }

                if (alternativeType != null)
                {
                    SetInstanceTypeMap(alternativeType, alternativeConstructorInfo);
                    return;
                }
            }

            throw new ArgumentException("No type can be mapped for base type " + baseType.FullName + ".");
        }

        private void SetInstanceTypeMap(Type mappedType, System.Reflection.ConstructorInfo constructor)
        {
            this.MappedType = mappedType;
            this.ConstructorInfo = constructor;
            this.IsFactoryMethod = false;
        }


        private ConstructorInfo GetConstructorInfoIfValid(Type type)
        {

            var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                .OrderByDescending(item => item.GetParameters().Length).ToArray();

            if (constructors.Length == 0)
                return null;

            if (constructors.Length > 2)
                return null;

            if (constructors.Length == 2 && constructors[1].GetParameters().Length != 0)
                return null;

            return constructors[0];
        }


        private void CheckForFactoryMethod(string name, Type baseType, Type type)
        {
            var factoryMethod = type.GetMethods(BindingFlags.Static | BindingFlags.Public).Where(
                item => item.Name.ToLower().StartsWith("create") && item.Name.ToLower().EndsWith(name.ToLower()) && item.ReturnType == baseType)
                .OrderBy(item => item.Name.Length)
                .FirstOrDefault();

            if (factoryMethod != null)
            {
                SetFactoryTypeMap(type, factoryMethod);
                return;
            }
            return;
        }

        private void SetFactoryTypeMap(Type mappedType, MethodInfo factoryMethod)
        {
            this.FactoryMethodInfo = factoryMethod;
            this.MappedType = mappedType;
            this.IsFactoryMethod = true;
        }



        protected override bool EqualsOverride(TypeFactoryDefinition compare)
        {
            return
               this.IsFactoryMethod == compare.IsFactoryMethod
               && this.IsSingleton == compare.IsSingleton
               && this.IsShared == compare.IsShared
               && this.ConstructorInfo == compare.ConstructorInfo
               && this.FactoryMethodInfo == compare.FactoryMethodInfo;               
        }

        private TypeFactoryBinding binding = null;
        private object bindingLock = new object();
        public TypeFactoryBinding GetBinding()
        {
            var retVal = binding;
            if (retVal == null)
            {
                lock (bindingLock)
                {
                    retVal = binding;
                    if (retVal == null)
                    {
                        var typeBuilder = new TypeFactoryGenerator(this);
                        retVal = typeBuilder.CreateBinding();
                        binding = retVal;
                    }
                }
            }
            return retVal;
        }

    }

}
