using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using System.Reflection;
using Inferables.Properties;
using System.Configuration.Assemblies;
using System.IO;

namespace Inferables.Internal
{
    internal static class DynamicAssemblyManager
    {
        static private AssemblyBuilder assemblyBuilder;
        static private ModuleBuilder moduleBuilder;
        static private string assemblyName;
        static private string assemblyFileName;

        static DynamicAssemblyManager()
        {
            assemblyName = typeof(DynamicAssemblyManager).Assembly.GetName().Name + ".Dynamic";
            assemblyFileName = assemblyName + ".dll";

            var assemblyNameObj = new AssemblyName(assemblyName)
            {
                HashAlgorithm = AssemblyHashAlgorithm.SHA1,
                KeyPair = new StrongNameKeyPair(Resources.InferablesDynamic)
            };

            var builderAccess = AssemblyBuilderAccess.Run;
#if DEBUG
            builderAccess = AssemblyBuilderAccess.RunAndSave;
#endif

            assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
                assemblyNameObj, builderAccess);

            moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyFileName);
            typeId = 0;
        }



        private static int typeId;
        private static object typeIdLock = new object();
        private static int GetNextTypeId()
        {
            lock (typeIdLock)
            {
                return typeId++;
            }
        }

        internal static TypeBuilder DefineType(string nameSuffix = "",TypeAttributes attrs = TypeAttributes.Class | TypeAttributes.NotPublic)
        {

            return moduleBuilder.DefineType(assemblyName + ".Type" + GetNextTypeId() + nameSuffix, attrs);
        }

#if DEBUG
        public static void DumpToFile(string path)
        {
            assemblyBuilder.Save(assemblyFileName);
            if (path != null)
            {
                var currentPath = Path.GetFullPath(".\\" + assemblyFileName);
                var destination = Path.GetFullPath(path + "\\" + assemblyFileName);
                File.Copy(currentPath, destination);
            }

        }
#endif


    }
}
