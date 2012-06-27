using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Inferables.GenBindings;

namespace Inferables.Internal
{
    internal class TypeFactoryBinding
    {
        public Type FactoryType { get; private set; }
        public ConstructorInfo ConstructorInfo { get; private set; }
        public IFactoryLoader Loader { get; private set; }

        public TypeFactoryBinding(Type factoryType, ConstructorInfo constructorInfo, IFactoryLoader loader)
        {
            FactoryType = factoryType;
            ConstructorInfo = constructorInfo;
            Loader = loader;
        }
    }
}
