using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TypeIDAttribute : Attribute
    {
        public string Id
        {
            get;
            private set;
        }


        public TypeIDAttribute(string id)
        {
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }
            else if (id == string.Empty)
            {
                throw new ArgumentException("Id can't be empty.");
            }

            Id = id.ToLower();
        }
    }
}
