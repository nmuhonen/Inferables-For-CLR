using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Inferables.Internal
{
    internal class MatchPath : Hashable<MatchPath>
    {
        public bool IsWildcard { get; internal set; }
        public bool HasNamespace { get { return Namespace != String.Empty; } }
        public string Namespace { get; internal set; }

        internal MatchPath() { IsWildcard = false; Namespace = String.Empty; }

        internal void SetHashCode()
        {
            this.HashCode = this.IsWildcard.GetHashCode() + this.Namespace.GetHashCode();
        }

        protected override bool EqualsOverride(MatchPath compare)
        {
            return this.IsWildcard == compare.IsWildcard && this.Namespace == compare.Namespace;
        }

    }
}
