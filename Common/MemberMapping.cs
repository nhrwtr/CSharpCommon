using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Common
{
    public class MemberMapping
    {
        private readonly Type _instanceType;
        private readonly string _name;
        private readonly List<TypeInfo> _propertyTypes = new List<TypeInfo>();
        private readonly Dictionary<string, int> _pairs = new Dictionary<string, int>();

        public string Name => _name;

        public IEnumerable<string> PropertyNames => _propertyTypes.Select(e => e.Name);

        public MemberMapping(Type type)
        {
            _instanceType = type;
            _name = _instanceType.Name;

            if (_instanceType.IsClass || _instanceType.IsInterface)
            {
                Mapping();
            }
        }

        private void Mapping()
        {
            foreach (PropertyInfo property in _instanceType.GetProperties())
            {
                Type type = property.PropertyType;
                // keyword types and DateTime type
                if (type.IsPrimitive ||
                    type == typeof(DateTime) || type == typeof(string) || type == typeof(decimal))
                {
                    AddPropertyInfo(property.Name, type);
                }

                // nullable types
                if (type.IsValueType &&
                    Nullable.GetUnderlyingType(type) != null)
                {
                    // keyword types and DateTime type
                    var underlyingType = Nullable.GetUnderlyingType(type);
                    if (underlyingType.IsPrimitive ||
                        underlyingType == typeof(DateTime) || underlyingType == typeof(decimal))
                    {
                        AddPropertyInfo(property.Name, underlyingType, true);
                    }
                }
            }
        }

        private void AddPropertyInfo(string name, Type type, bool isNullable = false)
        {
            _propertyTypes.Add(new TypeInfo(name, type, isNullable));
            _pairs[name] = _propertyTypes.Count - 1;
        }

        private readonly struct TypeInfo
        {
            public TypeInfo(string name, Type type, bool isNullable = false)
            {
                Name = name;
                Type = type;
                IsNullable = isNullable;
            }

            public readonly string Name;
            public readonly Type Type;
            public readonly bool IsNullable;
        }
    }
}
