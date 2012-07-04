using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Inferables.GenBindings;

namespace Inferables.Internal
{
    internal class Module: IInjectableModule, IAllowedBinding
    {

        public ModuleRegistry Registry { get; private set; }

        public Module(BindingRegistry domainRegistry)
        {
            this.Registry = new ModuleRegistry(domainRegistry);
        }

        public T Get<T>()
        {
            return (T)Get(typeof(T));
        }

        public T Get<T>(string name)
        {
            return (T)Get(typeof(T), name);
        }

        public T GetFactory<T>()
        {
            return (T)GetFactory(typeof(T));
        }

        public IModule Register<T>()
        {
            return Register(typeof(T));
        }

        public IModule Register<T>(string name)
        {
            return Register(typeof(T), name);
        }

        public IModule RegisterFactory<T>()
        {
            return RegisterFactory(typeof(T));
        }

        public Singleton GetSingleton<T>(string name)
        {
            return Registry.GetTypeFactory(typeof(T), false, name,this).GetSingleton();
        }

        public object Get(Type type)
        {
            return Get(type, null);
        }

        public object Get(Type type, string name)
        {
            var factory = Registry.GetTypeFactory(type, false, name, this);
            var result = factory.Get();
            return result;
        }

        public object GetFactory(Type type)
        {
            var factory = Registry.GetCustomFactory(type, this);
            return factory;
        }

        public IModule Register(Type type)
        {
            return Register(type, null);
        }

        public IModule Register(Type type, string name)
        {
            Registry.GetTypeFactory(type, false, name, this);
            return this;
        }


        public IModule RegisterFactory(Type type)
        {
            Registry.GetCustomFactory(type, this);
            return this;
        }


        public T GetExplicit<T>()
        {
            return (T)GetExplicit(typeof(T));
        }

        public object GetExplicit(Type t)
        {
            var factory = Registry.GetTypeFactory(t, true, null, this);
            return factory.Get();
        }


        public IModule RegisterExplicit<T>()
        {
            return RegisterExplicit(typeof(T));
        }

        public IModule RegisterExplicit(Type type)
        {
            Registry.GetTypeFactory(type, true, null, this);
            return this;
        }

        public Binding Binding
        {
            get { return Registry.BindingRegistry.Binding; }
        }

        public IEnumerable<BindingMap> Maps
        {
            get { return Registry.BindingRegistry.Binding.Maps; }
        }
    }
}
