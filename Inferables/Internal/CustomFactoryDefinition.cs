using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Inferables.Internal
{
    class CustomFactoryDefinition
    {
        public Type CreationType{get; private set;}
        public BindingRegistry Registry{get; private set;}
        public CustomFactoryMethodMap[] MethodMaps { get; private set; }

        public CustomFactoryDefinition(Type creationType, BindingRegistry bindingRegistry)
        {
            this.Registry = bindingRegistry;

            if (!creationType.IsInterface || !creationType.IsPublic)
                throw new ArgumentException("Custom factories can only bind on public interfaces");

            this.CreationType = creationType;

            this.MethodMaps = new []{creationType}
                .Concat(creationType.GetInterfaces())
                .SelectMany(item => item.GetMethods())
                .Select(
                    item => new CustomFactoryMethodMap(item, bindingRegistry))
                .ToArray();
        }

    }
}
