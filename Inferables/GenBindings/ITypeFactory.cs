using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Inferables.GenBindings
{
    public interface ITypeFactory
    {
        object Get();
        Singleton GetSingleton();
    }
}
