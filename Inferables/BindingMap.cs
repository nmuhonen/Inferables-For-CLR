using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Inferables.Internal;

namespace Inferables
{
    [DebuggerDisplay("{Value}")]
    public class BindingMap : Hashable<BindingMap>
    {
        internal MatchPath MatchPath { get; private set; }
        internal BindingPath BindingPath { get; private set; }

        internal BindingMap() { MatchPath = new MatchPath(); BindingPath = new BindingPath(); }

        internal void SetHashCode()
        {
            HashCode = unchecked(MatchPath.GetHashCode() + BindingPath.GetHashCode());
        }

        public string Value
        {
            get
            {
                var sb = new StringBuilder();
                WriteValue((str, color) => sb.Append(str));
                return sb.ToString();
            }
        }

        public void WriteToConsole()
        {
            WriteValue((str, color) =>
            {
                var previous = Console.ForegroundColor;
                Console.ForegroundColor = color ?? Console.ForegroundColor;
                Console.Write(str);
                Console.ForegroundColor = previous;
            });
        }

        private void WriteValue(Action<string, ConsoleColor?> writer)
        {
            string result = String.Empty;
            if (MatchPath.IsWildcard)
            {
                writer("*", ConsoleColor.Green);
                if (MatchPath.HasNamespace)
                    writer(".", ConsoleColor.White);
            }
            if (MatchPath.HasNamespace)
                writer(MatchPath.Namespace, ConsoleColor.White);
            writer(" => ", ConsoleColor.Yellow);
            if (BindingPath.IsRelative)
            {
                if (BindingPath.RootDepth == 0)
                {
                    writer("~", ConsoleColor.Green);
                }
                else
                {
                    writer("-", ConsoleColor.Green);
                    var depth = 1;
                    while (depth < BindingPath.RootDepth)
                    {
                        writer(".", ConsoleColor.White);
                        writer("-", ConsoleColor.Green);
                        depth++;
                    }
                }
                if (BindingPath.HasNamespace)
                    writer(".", ConsoleColor.White);
            }
            if (BindingPath.HasNamespace)
                writer(BindingPath.Namespace, ConsoleColor.White);
        }





        protected override bool EqualsOverride(BindingMap compareMap)
        {
            return this.MatchPath.Equals(compareMap.MatchPath) && this.BindingPath.Equals(compareMap.BindingPath);
        }

        internal string GetBindingNamespace(Type castType)
        {
            var rootNamespace = castType.Namespace;

            if ((!this.MatchPath.IsWildcard && rootNamespace != MatchPath.Namespace)
                    || (MatchPath.IsWildcard && MatchPath.HasNamespace && !rootNamespace.EndsWith(MatchPath.Namespace))
                    )
                return null;

            if (!this.BindingPath.IsRelative)
                return this.BindingPath.Namespace;

            int length = rootNamespace.Length;
            int remainingRoots = this.BindingPath.RootDepth;

            if (remainingRoots > 0)
            {
                while (remainingRoots > 0)
                {
                    length = rootNamespace.LastIndexOf('.', length - 1);
                    if (length == -1)
                        return null;
                    remainingRoots--;
                }
                rootNamespace = rootNamespace.Substring(0, length);
            }

            var resultNamespace = BindingPath.HasNamespace ? rootNamespace + "." + BindingPath.Namespace : rootNamespace;
            return resultNamespace;
        }


    }
}
