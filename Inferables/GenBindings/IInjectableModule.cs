using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Inferables.GenBindings
{
    public interface IInjectableModule: IModule
    {
        Singleton GetSingleton<T>(string name);
    }
}
