using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using Inferables.GenBindings;
using System.Reflection;

namespace Inferables.Internal
{
    internal class TypeFactoryMap: Hashable<TypeFactoryMap>
    {
        public IFactoryLoader Loader { get; private set; }
        public BindingRegistry Registry { get; private set; }
        
        public Type BaseType { get; private set; }
        public string Name { get; private set; }
        public TypeFactoryDefinition Definition { get; private set; }
        public bool IsExplicitType { get; private set; }

        public TypeFactoryMap(string name,Type baseType, bool isExplicitType, BindingRegistry registry)
        {
            this.Name = name;
            this.BaseType = baseType;
            this.Registry = registry;
            this.IsExplicitType = isExplicitType;
            HashCode = unchecked(name.ToLower().GetHashCode() + baseType.GetHashCode() + isExplicitType.GetHashCode());
        }

        public void Init(Stack<TypeFactoryMap> current)
        {
            this.Definition = Registry.GetTypeFactoryDefinition(this, current);
        }

        protected override bool EqualsOverride(TypeFactoryMap compare)
        {
            return this.Name.ToLower() == compare.Name.ToLower() && this.BaseType == compare.BaseType && this.IsExplicitType == compare.IsExplicitType;
        }
    }
}
