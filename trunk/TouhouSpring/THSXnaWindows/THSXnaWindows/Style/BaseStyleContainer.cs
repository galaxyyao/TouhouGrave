using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace TouhouSpring.Style
{
	class BaseStyleContainer : IStyleContainer
	{
		public IStyleContainer Parent
		{
			get; private set;
		}

		public virtual IEnumerable<IBindingProvider> BindingProviders
		{
			get { return Enumerable.Empty<IBindingProvider>(); }
		}

		public string Id
		{
			get; private set;
		}

        public bool IsIdRoot
        {
            get; private set;
        }

		public Dictionary<string, IStyleContainer> ChildIds
		{
			get; private set;
		}

		public virtual Rectangle Bounds
		{
			get; protected set;
		}

		public XElement Definition
		{
			get; protected set;
		}

		public UI.EventDispatcher Target
		{
			get; private set;
		}

		public virtual void Initialize()
		{
		}

		public virtual void Apply()
		{
			m_children.ForEach(children => children.Apply());
		}

		private List<IStyleElement> m_children = new List<IStyleElement>();

		protected BaseStyleContainer(IStyleContainer parent, XElement definition)
		{
			Parent = parent;
			Definition = definition;
		}

		protected void PreInitialize(Func<UI.EventDispatcher> creator)
		{
			if (Definition != null)
			{
				var idAttr = Definition.Attribute("Id");
				if (idAttr != null)
				{
                    IsIdRoot = idAttr.Value == "/";
					Id = IsIdRoot ? null : idAttr.Value;
					ChildIds = new Dictionary<string, IStyleContainer>();

                    if (!IsIdRoot)
                    {
                        for (IStyleContainer i = Parent; i != null; i = i.Parent)
                        {
                            if (i.ChildIds != null)
                            {
                                i.ChildIds.Add(Id, this);
                            }
                            if (i.IsIdRoot)
                            {
                                break;
                            }
                        }
                    }
				}
			}

			var target = creator != null ? creator() : null;
			if (target == null && Id != null)
			{
				throw new InvalidOperationException("Target must be created if an Id is specified.");
			}
			Target = target;

			if (Target != null)
			{
				for (IStyleContainer i = Parent; i != null; i = i.Parent)
				{
					if (i.Target != null)
					{
						i.Target.Listeners.Add(Target);
						break;
					}
				}
			}

			m_children.Clear();
		}

		protected void AddChild(IStyleElement child)
		{
			if (child == null)
			{
				throw new ArgumentNullException("child");
			}
			else if (m_children.Contains(child))
			{
				throw new ArgumentException("Child has already been added.");
			}

			m_children.Add(child);
		}

		protected void AddChildAndInitialize(IStyleElement child)
		{
			AddChild(child);
			child.Initialize();
		}

		protected void UpdateBounds(Rectangle value)
		{
			if (Target is UI.ITransformNode)
			{
				(Target as UI.ITransformNode).Transform = MatrixHelper.Translate(value.Left, value.Top);
				Bounds = new Rectangle(0, 0, value.Width, value.Height);
			}
			else
			{
				Bounds = value;
			}
		}
	}
}
