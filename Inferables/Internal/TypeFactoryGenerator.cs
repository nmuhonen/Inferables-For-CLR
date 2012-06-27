using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Inferables.GenBindings;
using System.Reflection.Emit;
using System.Reflection;

namespace Inferables.Internal
{
    internal class TypeFactoryGenerator: FactoryGenerator
    {
        private TypeFactoryDefinition definition;
        private Type mappedType;
        private TypeFactoryMap[] constructorTypeMaps;

        private Type factoryType;
        private Type loaderType;
        private ConstructorInfo constructorInfo;

 
        public TypeFactoryGenerator(TypeFactoryDefinition definition)
        {
            var typeSuffix = (definition.IsFactoryMethod ? "FactoryMethod_" + definition.MappedType.Name + "_" + definition.FactoryMethodInfo.Name : definition.MappedType.Name);
            this.loaderBuilder = DynamicAssemblyManager.DefineType("LoaderFor" + typeSuffix);
            this.factoryBuilder = DynamicAssemblyManager.DefineType("FactoryFor" + typeSuffix);
            this.definition = definition;

            GenerateTypes();
        }


        public TypeFactoryBinding CreateBinding()
        {
            var loader = (IFactoryLoader)Activator.CreateInstance(loaderType);
            return new TypeFactoryBinding(factoryType, constructorInfo, loader);
        }

        private void GenerateTypes()
        {
            ConstructorBuilder factoryConstructorBuilder = DefineFactoryType();
            DefineLoaderType(factoryConstructorBuilder);

            this.factoryType = this.factoryBuilder.CreateType();
            this.constructorInfo = factoryType.GetConstructors()[0];
            this.loaderType = this.loaderBuilder.CreateType();
        }


        private ConstructorBuilder DefineFactoryType()
        {
            factoryBuilder.AddInterfaceImplementation(typeof(ITypeFactory));
            var constructorBuilder = factoryBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, factoryContructorParameters);
            var getMethodBuilder = factoryBuilder.DefineMethod("Get", MethodAttributes.Public | MethodAttributes.Virtual,
                typeof(object), Type.EmptyTypes);
            MethodBuilder getSingletonFactory = null;
            FieldBuilder mainSingletonField = null;
            var getSingletonMethodBuilder = factoryBuilder.DefineMethod("GetSingleton", MethodAttributes.Public | MethodAttributes.Virtual,
                typeof(Singleton), Type.EmptyTypes);

            var singletonFields = GetSingletonFields(this.definition.ParameterTypeMaps);
            
            //build factory constructor
            var constrIl = GetInitConstructorIL(constructorBuilder,singletonFields);
            if (definition.IsSingleton)
            {
                var singletonType = typeof(Singleton);
                var delegateConstr = typeof(Func<>).MakeGenericType(typeof(object)).GetConstructors()[0];
                var singletonConstr = singletonType.GetConstructors()[0];
                mainSingletonField = factoryBuilder.DefineField("mainSingleton", typeof(Singleton), FieldAttributes.Private);
                getSingletonFactory = factoryBuilder.DefineMethod("SingletonFactory", MethodAttributes.Private, typeof(object), Type.EmptyTypes);
                constrIl.Emit(OpCodes.Ldarg_0);
                constrIl.Emit(OpCodes.Ldarg_0);
                constrIl.Emit(OpCodes.Ldftn, getSingletonFactory);
                constrIl.Emit(OpCodes.Newobj, delegateConstr);
                constrIl.Emit(OpCodes.Newobj, singletonConstr);
                constrIl.Emit(OpCodes.Stfld, mainSingletonField);
            }
            constrIl.Emit(OpCodes.Ret);

            //define create routine;
            
            var getIl = getMethodBuilder.GetILGenerator();
            var creationIl = definition.IsSingleton ? getSingletonFactory.GetILGenerator() : getIl;
            var sharedVars = new Dictionary<TypeFactoryDefinition, LocalBuilder>();
            InitSharedVarsOnParameters(sharedVars, definition, creationIl);
            foreach (var pair in sharedVars)
            {
                var depDefinition = pair.Key;
                GenerateInitInstanceIL(sharedVars, singletonFields, depDefinition, creationIl);
                creationIl.Emit(OpCodes.Stloc, pair.Value);
            }

            GenerateInitInstanceIL(sharedVars, singletonFields, definition, creationIl);

            //finish factory routine for singletons

            if (definition.IsSingleton)
            {
                creationIl.Emit(OpCodes.Ret);
            }


            //define get method

            if (definition.IsSingleton)
            {
                getIl.Emit(OpCodes.Ldarg_0);
                getIl.Emit(OpCodes.Ldfld, mainSingletonField);
                getIl.Emit(OpCodes.Call, mainSingletonField.FieldType.GetMethod("GetValue"));
            }

            getIl.Emit(OpCodes.Ret);

            //define get singleton method

            var getSIl = getSingletonMethodBuilder.GetILGenerator();
            if (definition.IsSingleton)
            {
                getSIl.Emit(OpCodes.Ldarg_0);
                getSIl.Emit(OpCodes.Ldfld, mainSingletonField);
            }
            else
            {
                getSIl.Emit(OpCodes.Ldnull);
            }
            getSIl.Emit(OpCodes.Ret);


            return constructorBuilder;
        }




    }
}
