using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Style
{
	class StyleException : ApplicationException
	{
		public StyleException() { }
		public StyleException(string msg) : base(msg) { }
	}

	class CantBeInPercentageException : StyleException
	{
		public CantBeInPercentageException(string attributeName)
			: base(String.Format("Attribute '{0}' can't be in percentage because the current style element doesn't have a parent.",
								 attributeName))
		{ }
	}

	class DuplicateAttributeException : StyleException
	{
		public DuplicateAttributeException(string attributeName)
			: base(String.Format("Duplicate attribute '{0}'.", attributeName))
		{ }
	}

	class MissingAttributeException : StyleException
	{
		public MissingAttributeException(string attributeName)
			: base(String.Format("Missing attribute '{0}'.", attributeName))
		{ }

		public MissingAttributeException(string attribName1, string attribName2)
			: base(String.Format("Missing attribute '{0}' or '{1}'.", attribName1, attribName2))
		{ }
	}

	class MutalExclusiveAttributeException : StyleException
	{
		public MutalExclusiveAttributeException(string attribName1, string attribName2)
			: base(String.Format("'{0}' and '{1}' can't be both present.", attribName1, attribName2))
		{ }

		public MutalExclusiveAttributeException(string attribName1, string attribName2, string attribName3)
			: base(String.Format("'{0}', '{1}' and '{2}' can't be all present.", attribName1, attribName2, attribName3))
		{ }
	}
}
