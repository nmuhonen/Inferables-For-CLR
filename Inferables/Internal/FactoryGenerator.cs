using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Inferables.GenBindings;
using System.Reflection.Emit;
using System.Reflection;

namespace Inferables.Internal
{
    internal class FactoryGenerator
    {
        static protected Type[] factoryContructorParameters = new Type[] { typeof(IInjectableModule) };

        private int fieldInstanceId = 0;
        protected TypeBuilder factoryBuilder;
        protected TypeBuilder loaderBuilder;


        protected void GenerateInitInstanceIL(Dictionary<TypeFactoryDefinition, LocalBuilder> sharedVars,
            Dictionary<TypeFactoryMap, FieldBuilder> singletonVars,
            TypeFactoryDefinition definition,
            ILGenerator il)
        {
            foreach (var constMap in definition.ParameterTypeMaps)
            {
                var constDef = constMap.Definition;
                if (constDef.IsShared)
                {
                    var local = sharedVars[constDef];
                    il.Emit(OpCodes.Ldloc, local);
                    continue;
                }
                if (constDef.IsSingleton)
                {
                    var field = singletonVars[constMap];
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldfld, field);
                    il.Emit(OpCodes.Callvirt, field.FieldType.GetMethod("GetValueAs").MakeGenericMethod(constMap.BaseType));
                    continue;
                }

                GenerateInitInstanceIL(sharedVars, singletonVars, constDef, il);
            }

            if (definition.IsFactoryMethod)
                il.Emit(OpCodes.Call, definition.FactoryMethodInfo);
            else
                il.Emit(OpCodes.Newobj, definition.ConstructorInfo);
        }


        protected ILGenerator GetInitConstructorIL(ConstructorBuilder constructorBuilder, Dictionary<TypeFactoryMap, FieldBuilder> singletonFields)
        {
            var constrIl = constructorBuilder.GetILGenerator();
            constrIl.Emit(OpCodes.Ldarg_0);
            constrIl.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes));
            foreach (var fieldPair in singletonFields)
            {
                var injectableGetSingletonMethod = typeof(IInjectableModule).GetMethod("GetSingleton").MakeGenericMethod(fieldPair.Key.BaseType);
                constrIl.Emit(OpCodes.Ldarg_0);
                constrIl.Emit(OpCodes.Ldarg_1);
                constrIl.Emit(OpCodes.Ldstr, fieldPair.Key.Name);
                constrIl.Emit(OpCodes.Callvirt, injectableGetSingletonMethod);
                constrIl.Emit(OpCodes.Stfld, fieldPair.Value);
            }
            return constrIl;
        }


        protected Dictionary<TypeFactoryMap, FieldBuilder> GetSingletonFields(IEnumerable<TypeFactoryMap> maps)
        {
            var singletonFields = new Dictionary<TypeFactoryMap, FieldBuilder>();
            GetSingletonFieldsRecursive(singletonFields, maps);
            return singletonFields;

        }

        protected void GetSingletonFieldsRecursive(Dictionary<TypeFactoryMap, FieldBuilder> fields, IEnumerable<TypeFactoryMap> maps)
        {
            foreach (var constrMap in maps)
            {
                if (fields.ContainsKey(constrMap))
                    continue;

                var constrDef = constrMap.Definition;

                if (!constrDef.IsSingleton)
                {
                    GetSingletonFieldsRecursive(fields, constrDef.ParameterTypeMaps);
                    continue;
                }

                fields.Add(constrMap, factoryBuilder.DefineField("SingletonField" + fieldInstanceId, typeof(Singleton), FieldAttributes.Private));
                fieldInstanceId++;

            }
        }


        protected void InitSharedVarsOnParameters(Dictionary<TypeFactoryDefinition, LocalBuilder> sharedVars, TypeFactoryDefinition def, ILGenerator il)
        {
            foreach (var parameterMap in def.ParameterTypeMaps)
            {
                InitSharedVars(sharedVars, parameterMap.Definition, il);
            }
        }

        protected void InitSharedVars(Dictionary<TypeFactoryDefinition, LocalBuilder> sharedVars, TypeFactoryDefinition def, ILGenerator il)
        {
            if (sharedVars.ContainsKey(def))
                return;

            if (!def.IsShared)
            {
                InitSharedVarsOnParameters(sharedVars, def, il);
                return;
            }

            var local = il.DeclareLocal(def.MappedType);
            sharedVars.Add(def, local);
        }


        protected void DefineLoaderType(ConstructorBuilder factoryConstructorBuilder)
        {
            loaderBuilder.AddInterfaceImplementation(typeof(IFactoryLoader));
            loaderBuilder.DefineDefaultConstructor(MethodAttributes.Public);
            var methodIl = loaderBuilder.DefineMethod("CreateFactory", MethodAttributes.Public | MethodAttributes.Virtual,
                typeof(object), factoryContructorParameters).GetILGenerator();
            methodIl.Emit(OpCodes.Ldarg_1);
            methodIl.Emit(OpCodes.Newobj, factoryConstructorBuilder);
            methodIl.Emit(OpCodes.Ret);
        }
    }
}
