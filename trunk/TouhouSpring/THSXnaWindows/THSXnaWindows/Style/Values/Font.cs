using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace TouhouSpring.Style.Values
{
    enum FontStyle
    {
        Regular,
        Bold,
        Italic,
    }

    struct Font
    {
        public string Family;
        public float? Size;
        public FontStyle? Style;
        public float? OutlineThickness;

        public static Font Parse(string str)
        {
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }

            string[] tokens = str.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            Font ret = new Font { Family = null, Size = null, Style = null, OutlineThickness = null };
            if (tokens.Length > 0)
            {
                ret.Family = tokens[0];
            }
            if (tokens.Length > 1)
            {
                ret.Size = float.Parse(tokens[1]);
            }
            if (tokens.Length > 2)
            {
                ret.Style = ParseStyle(tokens[2].Trim());
            }
            if (tokens.Length > 3)
            {
                ret.OutlineThickness = float.Parse(tokens[3]);
            }
            else if (tokens.Length > 4)
            {
                throw new FormatException(string.Format("'{0}' is not a valid value for Font.", str));
            }

            return ret;
        }

        public static FontStyle ParseStyle(string str)
        {
            switch (str.ToLower())
            {
                case "regular":
                    return FontStyle.Regular;
                case "bold":
                    return FontStyle.Bold;
                case "italic":
                    return FontStyle.Italic;
                default:
                    throw new FormatException(string.Format("'{0}' is not a valid value for FontStyle.", str));
            }
        }
    }

    static class FontParser
    {
        public static Font ParseFont(this XElement element, XName attributeName, Font defaultValue)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            else if (attributeName == null)
            {
                throw new ArgumentNullException("attributeName");
            }

            var attrib = element.Attribute(attributeName);
            return attrib != null ? Font.Parse(attrib.Value) : defaultValue;
        }
    }
}
