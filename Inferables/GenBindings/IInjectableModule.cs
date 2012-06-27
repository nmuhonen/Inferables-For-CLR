using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Inferables.GenBindings
{
    public interface IInjectableModule: IModule, IBindingContainer
    {
        Singleton GetSingleton<T>(string name);
    }
}
