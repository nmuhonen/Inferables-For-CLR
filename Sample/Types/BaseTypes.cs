using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sample
{
    // Heres the interface used in the demo for construction of specific types
    public interface IFoo
    {
        IFoo Child1 { get; }
        IFoo Child2 { get; }
        void Dump(string prefix = null);
        void Dump(int indent);
        string ID { get; }
    }

    // This is the base class used by all types inferred in the demo. It contains some
    // utility code to analysizethe instance scope of injetced objects
    public abstract class FooBase : IFoo
    {
        public IFoo Child1 { get; private set; }
        public IFoo Child2 { get; private set; }
        public string ID { get; private set; }
        public void Dump(string prefix = null)
        {
            if (!string.IsNullOrWhiteSpace(prefix))
            {
                var initialColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(prefix + ":");
                Console.WriteLine();
                Console.ForegroundColor = initialColor;
            }
            Console.WriteLine();
            Console.Write(new string(' ', 2));
            Dump(1);
            Console.WriteLine();
            Console.WriteLine();
        }

        public void Dump(int indent)
        {
            var indentStr = new String(' ', indent * 2);
            Console.WriteLine(ID);

            if (Child1 == null && Child2 == null)
                return;

            Console.WriteLine(indentStr + "{");
            var innerIndent = indent + 1;
            var innerIndentStr = new String(' ', innerIndent * 2);

            if (Child1 != null)
            {
                Console.Write(innerIndentStr + "Child1: ");
                Child1.Dump(innerIndent);
            }

            if (Child2 != null)
            {
                Console.Write(innerIndentStr + "Child2: ");
                Child2.Dump(innerIndent);
            }

            Console.WriteLine(indentStr + "}");
        }

        private static int id = 0;

        public FooBase(IFoo child1, IFoo child2)
        {
            Child1 = child1;
            Child2 = child2;
            ID = "Type of " + this.GetType().Name + ", Object ID " + id++;
        }
    }
}
