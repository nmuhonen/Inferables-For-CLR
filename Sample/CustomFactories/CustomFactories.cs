using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sample
{
    // This factory interface can be used in lieu of the module, to set allow only creation of specific types.
    public interface IFactory
    {
        // Creates type which maps to "Foo" in the containing module.
        IFoo GetFoo();

        // Creates type which maps to "Foo1" in the containing module.
        IFoo GetFoo1();

        // Creates type which maps to "Foo2" in the containing module.
        IFoo GetFoo2();

        // Creates type which maps to "Foo3" in the containing module.
        IFoo GetFoo3();

        // Creates type which maps to "Foo4" in the containing module.
        IFoo GetFoo4();

        // Creates type which maps to "Foo5" in the containing module.
        IFoo GetFoo5();

        // Creates type which maps to "Foo6" in the containing module.
        IFoo GetFoo6();
    }
}
