using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sereno.Utilities
{
    public class ObjectUtility
    {
        public static PropertyInfo? GetPropertyInfo(object? obj, string propertyName)
        {
            if (obj == null || string.IsNullOrWhiteSpace(propertyName))
            {
                return null;
            }
            
            return obj.GetType().GetProperty(propertyName);
        }

        public static object? GetPropertyValue(object? obj, string propertyName)
        {
            if (obj == null || string.IsNullOrWhiteSpace(propertyName))
            {
                return null;
            }

            var propertyInfo = obj.GetType().GetProperty(propertyName);
            return propertyInfo?.GetValue(obj);
        }
    }
}
