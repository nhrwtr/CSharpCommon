using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace Common
{
    public class DataContractMapping
    {
        private Type _instanceType;

        public DataContractMapping(Type type)
        {
            _instanceType = type;
        }

        private void Mapping()
        {
            Attribute[] attributes = Attribute.GetCustomAttributes(_instanceType, typeof(DataContractAttribute));
            foreach (Attribute attr in attributes)
            {
                if (attr is DataContractAttribute)
                {
                    continue;
                }

                var dataContract = attr as DataContractAttribute;
            }
        }
    }
}
