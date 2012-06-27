using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Inferables
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Hashable<T>
        where T : class
    {
        virtual protected int HashCode { get; set; }

        public override int GetHashCode()
        {
            return HashCode;
        }

        public sealed override bool Equals(object obj)
        {
            T compare = obj as T;
            if (compare == null)
                return false;
            return EqualsOverride(compare);
        }

        protected abstract bool EqualsOverride(T compare);
    }
}
