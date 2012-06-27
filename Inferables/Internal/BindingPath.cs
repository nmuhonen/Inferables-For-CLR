using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Inferables.Internal
{
    internal class BindingPath : Hashable<BindingPath>
    {
        public bool IsRelative { get; internal set; }
        public int RootDepth { get; internal set; }
        public bool HasNamespace { get { return Namespace != String.Empty; } }
        public string Namespace { get; internal set; }

        internal BindingPath()
        {
            IsRelative = false;
            RootDepth = 0;
            Namespace = String.Empty;
        }

        public void SetHashCode()
        {
            this.HashCode = unchecked(this.IsRelative.GetHashCode() + this.RootDepth.GetHashCode() + this.Namespace.GetHashCode());
        }

        protected override bool EqualsOverride(BindingPath compare)
        {
            return this.IsRelative == compare.IsRelative
                && this.RootDepth == compare.RootDepth
                && this.Namespace == compare.Namespace;
        }

    }
}
