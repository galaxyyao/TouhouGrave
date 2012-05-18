using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Messaging
{
	public class LetterBoxIsFullException : ApplicationException
	{
		public LetterBoxIsFullException()
			: base("Letter box is full.")
		{ }
	}

	public class UnexpectedMessageException : ApplicationException
	{
		public UnexpectedMessageException(string expectedText, string actualText)
			: base(String.Format("Unexpected message received. Expected: \"{0}\", Actual: \"{1}\".", expectedText, actualText))
		{
			if (expectedText == null)
			{
				throw new ArgumentNullException("expectedText");
			}
			else if (actualText == null)
			{
				throw new ArgumentNullException("actualText");
			}
		}
	}
}
