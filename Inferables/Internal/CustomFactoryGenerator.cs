using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Inferables.GenBindings;
using System.Reflection.Emit;
using System.Reflection;

namespace Inferables.Internal
{
    internal class CustomFactoryGenerator: FactoryGenerator
    {
        private CustomFactoryDefinition definition;
        private Type factoryType;
        private Type loaderType;

        public CustomFactoryGenerator(CustomFactoryDefinition definition)
        {
            this.loaderBuilder = DynamicAssemblyManager.DefineType("CustomLoaderFor" + definition.CreationType.Name);
            this.factoryBuilder = DynamicAssemblyManager.DefineType("CustomFactoryFor" + definition.CreationType.Name);
            this.definition = definition;

            GenerateTypes();
        }

        private void GenerateTypes()
        {
            ConstructorBuilder factoryConstructorBuilder = DefineFactoryType();
            DefineLoaderType(factoryConstructorBuilder);
            this.factoryType = this.factoryBuilder.CreateType();
            this.loaderType = this.loaderBuilder.CreateType();

        }


        private ConstructorBuilder DefineFactoryType()
        {
            factoryBuilder.AddInterfaceImplementation(definition.CreationType);
            var constructorBuilder = factoryBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, factoryContructorParameters);
            var singletonFields = GetSingletonFields(this.definition.MethodMaps.Select(item => item.Map));
            var constrIl = this.GetInitConstructorIL(constructorBuilder, singletonFields);
            constrIl.Emit(OpCodes.Ret);

            var methodIndex = 0;
            foreach (var methodMap in definition.MethodMaps)
            {
                methodIndex++;
                var methodBuilder = factoryBuilder.DefineMethod(
                    "InterfaceMethod" + methodIndex,
                    MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Final,
                    methodMap.ReturnType,
                    Type.EmptyTypes);
                factoryBuilder.DefineMethodOverride(methodBuilder, methodMap.MethodInfo);
                var il = methodBuilder.GetILGenerator();
                var methodTypeDef = methodMap.Map.Definition;

                if (methodTypeDef.IsSingleton)
                {
                    var field = singletonFields[methodMap.Map];
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldfld, field);
                    il.Emit(OpCodes.Callvirt, field.FieldType.GetMethod("GetValueAs").MakeGenericMethod(methodMap.Map.BaseType));
                }
                else
                {
                    var sharedVars = new Dictionary<TypeFactoryDefinition, LocalBuilder>();
                    this.InitSharedVars(sharedVars, methodMap.Map.Definition, il);
                    foreach (var pair in sharedVars)
                    {
                        var depDefinition = pair.Key;
                        GenerateInitInstanceIL(sharedVars, singletonFields, depDefinition, il);
                        il.Emit(OpCodes.Stloc, pair.Value);
                    }
                    this.GenerateInitInstanceIL(sharedVars, singletonFields, methodMap.Map.Definition, il);
                }
                il.Emit(OpCodes.Ret);
            }
            
            return constructorBuilder;
        }


        internal IFactoryLoader CreateLoader()
        {
            return (IFactoryLoader)Activator.CreateInstance(loaderType);
        }
    }
}
