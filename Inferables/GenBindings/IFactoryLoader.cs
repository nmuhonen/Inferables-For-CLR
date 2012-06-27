using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Inferables.GenBindings
{
    public interface IFactoryLoader
    {
        object CreateFactory(IInjectableModule module);
    }
}
