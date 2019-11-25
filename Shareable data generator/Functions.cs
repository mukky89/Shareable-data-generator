using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Shareable_data_generator
{
    public static partial class Functions
    {

        public static object GetPropertyValue(this object obj, string name)
        {
            if (obj == null || string.IsNullOrEmpty(name))
                return null;

            var methods = name.Split('.');

            object current = obj;
            object result = null;
            foreach (var method in methods)
            {
                var prop = current?.GetType().GetProperty(method, BindingFlags.Public
                                                                | BindingFlags.NonPublic
                                                                | BindingFlags.Instance
                                                                | BindingFlags.GetProperty);
                result = prop != null ? prop.GetValue(current, null) : null;
                current = result;
            }
            return result;
        }

    }
}
