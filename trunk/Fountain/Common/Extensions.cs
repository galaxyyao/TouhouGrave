using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace TouhouSpring.ExtensionMethods
{
    public static class  XDocumentExtension
    {
        public static string GetAttribute(this XElement element, XName name)
        {
            XAttribute attr = element.Attribute(name);
            if (attr == null) return null;
            return attr.Value;
        }
    }
}
