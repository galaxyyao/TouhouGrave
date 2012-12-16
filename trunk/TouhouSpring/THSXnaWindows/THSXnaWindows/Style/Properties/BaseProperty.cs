using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Style.Properties
{
	class BaseProperty : IStyleElement
	{
		public IStyleContainer Parent
		{
			get; private set;
		}

		public virtual IEnumerable<IBindingProvider> BindingProviders
		{
			get { return Enumerable.Empty<IBindingProvider>(); }
		}

		public virtual void Initialize() { }
		public virtual void Apply() { }

		protected BaseProperty(IStyleContainer parent)
		{
			if (parent == null)
			{
				throw new ArgumentNullException("parent");
			}

			Parent = parent;
		}

		protected string BindValuesFor(string rawExpr)
		{
			StringBuilder strBuilder = new StringBuilder();
			for (int i = 0; ; )
			{
				int firstToken = rawExpr.IndexOf('#', i);
				if (firstToken == -1)
				{
					strBuilder.Append(rawExpr.Substring(i));
					break;
				}

				int secondToken = rawExpr.IndexOf('#', firstToken + 1);
				if (secondToken == -1)
				{
					throw new FormatException(String.Format("Mismatched '#' sign in expression '{0}'.", rawExpr));
				}

				string id = rawExpr.Substring(firstToken + 1, secondToken - firstToken - 1);
				string replacement = null;
				foreach (var bindingProvider in GetConcatedProviders())
				{
					if (bindingProvider.TryGetValue(id, out replacement))
					{
						break;
					}
				}
				if (replacement == null)
				{
					throw new FormatException(String.Format("Can't find a binding for '{0}'.", id));
				}
				strBuilder.Append(rawExpr.Substring(i, firstToken - i));
				strBuilder.Append(BindValuesFor(replacement));
				i = secondToken + 1;
			}
			return strBuilder.ToString();
		}

		private IEnumerable<IBindingProvider> GetConcatedProviders()
		{
			for (IStyleElement i = this; i != null; i = i.Parent)
			{
				foreach (var bp in i.BindingProviders)
				{
					yield return bp;
				}
			}
		}
	}
}
