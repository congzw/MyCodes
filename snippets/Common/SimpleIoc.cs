using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace CommonTrace.Common
{
    public class SimpleIoc
    {
        public SimpleIoc()
        {
            Services = new ConcurrentDictionary<Type, Func<object>>();
        }

        public T Resolve<T>()
        {
            var theType = typeof(T);
            if (!Services.ContainsKey(theType))
            {
                return default(T);
            }
            return (T)Services[theType]();
        }

        public void Register<T>(Func<T> factory)
        {
            var theType = typeof(T);
            if (Services.ContainsKey(theType))
            {
                throw new InvalidOperationException("not allowed register more then once");
            }
            Services[theType] = () => factory();
        }

        protected IDictionary<Type, Func<object>> Services { get; set; }

        public static SimpleIoc Instance = new SimpleIoc();
    }
}
