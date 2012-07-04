using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Inferables.GenBindings;

namespace Inferables
{
    public interface IModule
    {
        Binding Binding { get; }

        T Get<T>();
        object Get(Type type);
        T Get<T>(string name);
        object Get(Type type, string name);
        T GetFactory<T>();
        object GetFactory(Type type);
        T GetExplicit<T>();
        object GetExplicit(Type t);

        IModule Register<T>();
        IModule Register(Type type);
        IModule Register<T>(string name);
        IModule Register(Type type, string name);
        IModule RegisterFactory<T>();
        IModule RegisterFactory(Type type);
        IModule RegisterExplicit<T>();
        IModule RegisterExplicit(Type type);
    }
}
