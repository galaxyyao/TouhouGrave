using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.ComponentModel;

namespace TouhouSpring
{
    public static class ExtXML
    {
        public static T GetFirstDescendantsValue<T, T2>(T2 xLinqTarget, string descendantsName)
        {
            Type type = typeof(T);
            Type type2 = typeof(T2);
            object value = null;
            if (type2.Name == "XDocument")
            {
                XDocument doc = (XDocument)Convert.ChangeType(xLinqTarget, type2);
                value = TypeDescriptor.GetConverter(type).ConvertFromInvariantString(doc.Descendants(descendantsName).FirstOrDefault().Value);
            }
            else if (type2.Name == "XElement")
            {
                XElement ele = (XElement)Convert.ChangeType(xLinqTarget, type2);
                value = TypeDescriptor.GetConverter(type).ConvertFromInvariantString(ele.Descendants(descendantsName).FirstOrDefault().Value);
            }
            else
            {
                throw new Exception("Invalid xLinq target type");
            }
            T result;
            try
            {
                result = (T)Convert.ChangeType(value, type);
            }
            catch (InvalidCastException ex)
            {
                throw new InvalidCastException(ex.Message);
            }
            return result;
        }

        public static List<T> GetDescendantsValues<T, T2>(T2 xLinqTarget, string descendantsName)
        {
            Type type = typeof(T);
            Type type2 = typeof(T2);
            List<T> results = new List<T>();
            IEnumerable<XElement> elements = null;

            if (type2.Name == "XDocument")
            {
                XDocument doc = (XDocument)Convert.ChangeType(xLinqTarget, type2);

                elements = doc.Descendants(descendantsName);
            }
            else if (type2.Name == "XElement")
            {
                XElement ele = (XElement)Convert.ChangeType(xLinqTarget, type2);
                elements = ele.Descendants(descendantsName);
            }
            else
            {
                throw new Exception("Invalid xLinq target type");
            }

            foreach (var element in elements)
            {
                object value = TypeDescriptor.GetConverter(type).ConvertFromInvariantString(element.Value);
                T result;
                try
                {
                    result = (T)Convert.ChangeType(value, type);
                    results.Add(result);
                }
                catch (InvalidCastException ex)
                {
                    throw new InvalidCastException(ex.Message);
                }
            }
            return results;
        }
    }
}
