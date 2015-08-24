﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PVM.Core.Data.Attributes;

namespace PVM.Core.Data
{
    public interface IDataMapper
    {
        void MapData(object destination, IDictionary<string, object> data);
        IDictionary<string, object> ExtractData(object source);
    }

    public class DefaultDataMapper : IDataMapper
    {
        public void MapData(object destination, IDictionary<string, object> data)
        {
            foreach (
                PropertyInfo property in
                    destination.GetType()
                        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(p => p.GetCustomAttributes<InAttribute>(true).Any()))
            {
                string name = property.GetCustomAttribute<InAttribute>(true).Name ?? property.Name;

                if (!data.ContainsKey(name.ToLower()))
                {
                    throw new DataMappingNotSatisfiedException(
                        string.Format("Key '{0}' demanded by '{1}' not present in data dictionary", name.ToLower(),
                            destination.GetType().FullName));
                }


                if (property.GetSetMethod() == null)
                {
                    throw new DataMappingNotSatisfiedException(
                        string.Format("Property '{0}' of '{1}' does not have a public setter", name.ToLower(),
                            destination.GetType().FullName));
                }

                property.SetValue(destination, data[name.ToLower()]);
            }
        }

        public IDictionary<string, object> ExtractData(object source)
        {
            IDictionary<string, object> result = new Dictionary<string, object>();
            foreach (
                PropertyInfo property in
                    source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(
                        p => p.GetCustomAttributes<OutAttribute>().Any()))
            {
                string name = property.GetCustomAttribute<OutAttribute>(true).Name ?? property.Name;

                if (property.GetGetMethod() == null)
                {
                    throw new DataMappingNotSatisfiedException(
                        string.Format("Property '{0}' of '{1}' does not have a public getter", name.ToLower(),
                            source.GetType().FullName));
                }

                result.Add(name.ToLower(), property.GetValue(source));
            }

            return result;
        }
    }
}