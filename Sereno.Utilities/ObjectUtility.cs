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

        public static PropertyInfo? GetPropertyInfo(object? obj, string? propertyName)
        {
            PropertyInfo? propertyInfo = null; 
            if (obj != null && !string.IsNullOrWhiteSpace(propertyName))
            {
                propertyInfo = obj?.GetType().GetProperty(propertyName);
            }
            return propertyInfo;
        }

        public static object? GetPropertyValue(object? obj, string? propertyName)
        {
            object? result = null;
            if (obj != null && !string.IsNullOrWhiteSpace(propertyName))
            {
                PropertyInfo? propertyInfo = obj?.GetType().GetProperty(propertyName);
                if (propertyInfo != null)
                {
                    result = propertyInfo.GetValue(obj);
                }
            }
            return result;
        }
    }
}
