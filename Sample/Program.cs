using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Inferables;

namespace Sample
{
    class Program
    {
        // To see the dependency inference test in action, run a console with areally big buffer :)
        static void Main(string[] args)
        {
            // create a module with default bindings and do a dependency inference sample.
            var module = ModuleManager.CreateModule();
            DemoInferables(module);
            
            // same as before, now with custom bindings- you should notice that 
            // Sample.Mocks.MockFoo5 has replaced the previous implementation.
            var module2 = ModuleManager.CreateModule("~.Mocks,~");
            DemoInferables(module2);

            //and once more, now with all types generated and cached
            DemoInferables(module2);
        }
       
        private static void DemoInferables(IModule module)
        {
            ShowConsoleHeader(module);

            // Here we are getting an inferred type that matches interface IFoo based on the namespace bindings for this module.
            var foo = module.Get<IFoo>();
            foo.Dump("Value for foo");

            // This method resolves the inferred on finding the match patten foo2 for Interface type IFoo.
            var foo2 = module.Get<IFoo>("foo2");
            foo.Dump("Value for foo2");

            // Here we are getting foo again- performance very good this time around since all type generation il code has been cached. 
            var fooAgain = module.Get<IFoo>();
            fooAgain.Dump("Value for fooAgain");

            // Here we are mapping explicitly to a Foo type, choosing the appropriate constructor and infering dependencies.
            var explicitFoo = module.GetExplicit<Foo>();
            explicitFoo.Dump("Value of explicit foo");


            ShowFactoryTestsHeader();

            // Here we are binding to a custom factory interface, which will automatically generate a type for this factory, cache the it, and instantiate it.
            var factory = module.GetFactory<IFactory>();
            factory.GetFoo().Dump("Value for GetFoo()");
            factory.GetFoo1().Dump("Value for GetFoo1()");
            factory.GetFoo2().Dump("Value for GetFoo2()");
            factory.GetFoo3().Dump("Value for GetFoo3()");
            factory.GetFoo4().Dump("Value for GetFoo4()");
            factory.GetFoo5().Dump("Value for GetFoo5()");
            factory.GetFoo6().Dump("Value for GetFoo6()");



        }

        private static void ShowFactoryTestsHeader()
        {
            Console.WriteLine();
            Console.WriteLine("Custom Factory Demo:");
            Console.WriteLine("-------------------");
        }

        private static void ShowConsoleHeader(IModule module)
        {
            Console.WriteLine("Current Map:");
            Console.WriteLine("------------");
            foreach (var map in module.Binding.Maps)
            {
                map.WriteToConsole();
                Console.WriteLine();
            }

            Console.WriteLine();
            Console.WriteLine("Instance Demos:");
            Console.WriteLine("---------------");
        }
    }
}
