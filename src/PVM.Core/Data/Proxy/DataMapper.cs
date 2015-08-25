﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using PVM.Core.Data.Attributes;

namespace PVM.Core.Data.Proxy
{
    public class DataMapper
    {
        public static object CreateProxyFor(Type type, IDictionary<string, object> data)
        {
            var generator = new ProxyGenerator();
            return generator.CreateInterfaceProxyWithoutTarget(type, new DataInterceptor(data));
        }

        public static IDictionary<string, object> ExtractData<T>(object data) where  T : class
        {
            IDictionary<string, object> result = new Dictionary<string, object>();
            foreach (
                PropertyInfo property in
                    typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(
                        p => p.GetCustomAttributes<OutAttribute>(true).Any()))
            {
                string name = property.GetOutMappingName();

                if (property.GetGetMethod() == null)
                {
                    throw new DataMappingNotSatisfiedException(
                        string.Format("Property '{0}' of '{1}' does not have a public getter", name,
                            data.GetType().FullName));
                }
                result.Add(name, property.GetValue(data));
            }
            return result;
        }
    }
}