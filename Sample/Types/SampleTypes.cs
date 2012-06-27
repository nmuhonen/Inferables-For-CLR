using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sample
{
    // when calling .Get<IFoo>() method on an the demo modules, this is the default resolution-
    // it is named Foo- ie the name "IFoo" from the interface minus the "I"
    public class Foo : FooBase
    {
        public Foo(IFoo foo1, IFoo foo2) : base(foo1, foo2) { }
    }

    // Resolves to a match of Foo1
    public class Foo1 : FooBase
    {
        public Foo1(IFoo foo2, IFoo foo3) : base(foo2, foo3) { }
    }

    // Resolves to a match of Foo2
    public class Foo2 : FooBase
    {
        public Foo2(IFoo foo3, IFoo foo4) : base(foo3, foo4) { }
    }

    // Resolves to a match of Foo3. Has shared instance scope- meaning only one instance of this matched type will be injected
    // per object creation using the module or a custom factory.
    public class SharedFoo3 : FooBase
    {
        public SharedFoo3(IFoo foo4, IFoo foo5) : base(foo4, foo5) { }
    }

    // Resolves a match of Foo4. Has singletong scope - meaning all creation of instances from a module will share one instance of this
    // type.
    public class SingletonFoo4 : FooBase
    {
        public SingletonFoo4() : base(null, null) { }
    }

    
    public static class OtherFooFactories
    {
        //Resolves a match of Foo5.
        public static IFoo CreateFoo5(IFoo foo6)
        {
            return new Foo(foo6, null);
        }

        //Resolves a match of Foo6. Has singleton scope.
        public static IFoo CreateSingletonFoo6()
        {
            return new Foo(null, null);
        }
    }
}
