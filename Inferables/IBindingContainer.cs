using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Inferables
{
    /// <summary>
    ///  This is a binding container
    /// </summary>
    public interface IBindingContainer
    {
        Binding Binding { get; }
    }
}
