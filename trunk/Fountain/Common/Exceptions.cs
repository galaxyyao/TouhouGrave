using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
	/// <summary>
	/// A general purpose exception class representing any exceptions caused by invalid data format, structure, value, etc.
	/// </summary>
	public class InvalidDataException : ApplicationException
	{
		public InvalidDataException() { }
		public InvalidDataException(string message) : base(message) { }
		public InvalidDataException(string message, Exception innerException) : base(message, innerException) { }
	}

	/// <summary>
	/// A general purpose exception class representing any exceptions caused by lack of compulsory attributes, interfaces or
	/// base classes in classes' definitions.
	/// </summary>
	public class IncompleteTypeDefinitionException : ApplicationException
	{
		public IncompleteTypeDefinitionException() { }
		public IncompleteTypeDefinitionException(string message) : base(message) { }
		public IncompleteTypeDefinitionException(string message, Exception innerException) : base(message, innerException) { }

		public IncompleteTypeDefinitionException(Type lackingComponent)
			: base(GenerateMessageFromLackingComponent(lackingComponent))
		{ }

		private static string GenerateMessageFromLackingComponent(Type lackingComponent)
		{
			if (lackingComponent.IsInterface)
			{
				return string.Format("Class doesn't implement {0}.", lackingComponent.Name);
			}
			else if (lackingComponent.IsSubclassOf<Attribute>())
			{
				return string.Format("Class doesn't have {0}.", lackingComponent.Name);
			}
			else
			{
				return string.Format("Class doesn't inherit {0}.", lackingComponent.Name);
			}
		}
	}
}
