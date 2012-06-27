using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Inferables.GenBindings
{
    public class Singleton
    {
        private object syncLock;
        private Func<object> factory;
        public Singleton(Func<object> factory)
        {
            this.factory = factory;
            this.syncLock = new object();
            this.value = null;
        }

        private object value;
        public object GetValue()
        {
            var retVal = value;
            if (retVal == null)
            {
                lock (syncLock)
                {
                    retVal = value;
                    if (retVal == null)
                    {
                        retVal = factory();
                        value = retVal;
                    }
                }
            }
            return retVal;
        }

        public T GetValueAs<T>()
        {
            return (T)GetValue();
        }
    }
}
