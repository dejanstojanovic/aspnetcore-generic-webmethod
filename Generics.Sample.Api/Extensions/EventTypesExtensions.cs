using Generics.Sample.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Generics.Sample.Api.Extensions
{
    public static class EventTypesExtensions
    {
        public static IEnumerable<Type> GetEventTypes(this Assembly assembly)
        {
            IEnumerable<Type> _eventTypes = typeof(IEventModel).Assembly.GetTypes()
                .Where(t => !t.IsAbstract && t.IsPublic && typeof(IEventModel).IsAssignableFrom(t))
                .OrderBy(t => t.Name);

            return _eventTypes;
        }

        public static IEnumerable<Type> GetEventTypesFromParentAssembly(this Type type)
        {
            return type.Assembly.GetEventTypes();
        }

        public static IEnumerable<Type> GetEventTypesFromParentAssembly(this Object @object)
        {
            return @object.GetType().GetEventTypesFromParentAssembly();
        }
    }
}
