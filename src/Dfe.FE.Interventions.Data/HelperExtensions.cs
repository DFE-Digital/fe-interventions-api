using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dfe.FE.Interventions.Data
{
    internal static class HelperExtensions
    {
        internal static readonly ConcurrentDictionary<Type, PropertyInfo[]> PropertyCache = new ConcurrentDictionary<Type, PropertyInfo[]>();

        internal static void UpdateFrom<T>(this T destination, T source, string[] propertiesToIgnore = null) where T : class
        {
            var properties = destination.GetProperties();
            
            foreach (var property in properties)
            {
                if (propertiesToIgnore != null && propertiesToIgnore.Any(x => x.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase)))
                {
                    continue;
                }

                var value = property.GetValue(source);
                property.SetValue(destination, value);
            }
        }

        internal static PropertyInfo[] GetProperties(this object instance)
        {
            var type = instance.GetType();

            var properties = PropertyCache.GetOrAdd(type, (loadType) => loadType.GetProperties());

            return properties;
        }
    }
}