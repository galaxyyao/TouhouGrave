using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace TouhouSpring.Particle
{
	public class ModifierList : IList<Modifier>
	{
		public class ContentReaders : ContentTypeReader<ModifierList>
		{
			public override bool CanDeserializeIntoExistingObject
			{
				get { return true; }
			}

			protected override ModifierList Read(ContentReader input, ModifierList existingInstance)
			{
				if (existingInstance == null)
				{
					throw new InvalidOperationException("ModifierList can only be deserialized from an existing instance.");
				}

				int count = input.ReadInt32();
				for (int i = 0; i < count; ++i)
				{
					existingInstance.Add(input.ReadObject<Modifier>((Modifier)null));
				}

				return existingInstance;
			}
		}

		private List<Modifier> m_modifiers;

		public Effect Effect
		{
			get; private set;
		}

		public ModifierList(Effect effect)
		{
			if (effect == null)
			{
				throw new ArgumentNullException("effect");
			}

			m_modifiers = new List<Modifier>();
			Effect = effect;
		}

		#region IList<T> interface

		public Modifier this[int index]
		{
			get { return m_modifiers[index]; }
			set
			{
				if (index < 0 || index > m_modifiers.Count)
				{
					throw new ArgumentOutOfRangeException("Index is out of range.");
				}
				else if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				else if (m_modifiers.Contains(value))
				{
					throw new ArgumentException("Item is already in the list.");
				}
				else if (value.Effect != null)
				{
					throw new InvalidOperationException("Item is contained in another ModifierList.");
				}

				var old = m_modifiers[index];
				if (old != value)
				{
					m_modifiers[index] = value;
					old.Effect = null;
					value.Effect = Effect;
				}
			}
		}

		public int IndexOf(Modifier item)
		{
			return m_modifiers.IndexOf(item);
		}

		public void Insert(int index, Modifier item)
		{
			if (index < 0 || index > m_modifiers.Count)
			{
				throw new ArgumentOutOfRangeException("Index is out of range.");
			}
			else if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			else if (m_modifiers.Contains(item))
			{
				throw new ArgumentException("Item is already in the list.");
			}
			else if (item.Effect != null)
			{
				throw new InvalidOperationException("Item is contained in another ModifierList.");
			}

			m_modifiers.Insert(index, item);
			item.Effect = Effect;
		}

		public void RemoveAt(int index)
		{
			if (index < 0 || index > m_modifiers.Count)
			{
				throw new ArgumentOutOfRangeException("Index is out of range.");
			}

			var item = m_modifiers[index];
			m_modifiers.RemoveAt(index);
			item.Effect = null;
		}

		#endregion

		#region ICollection<T> interface

		public int Count
		{
			get { return m_modifiers.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public void Add(Modifier item)
		{
			Insert(Count, item);
		}

		public void Clear()
		{
			m_modifiers.ForEach(m => m.Effect = null);
			m_modifiers.Clear();
		}

		public bool Contains(Modifier item)
		{
			return m_modifiers.Contains(item);
		}

		public void CopyTo(Modifier[] array, int arrayIndex)
		{
			m_modifiers.CopyTo(array, arrayIndex);
		}

		public bool Remove(Modifier item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}

			if (m_modifiers.Remove(item))
			{
				item.Effect = null;
				return true;
			}

			return false;
		}

		#endregion

		#region IEnumerable<T> interface

		public IEnumerator<Modifier> GetEnumerator()
		{
			return m_modifiers.GetEnumerator();
		}

		#endregion

		#region IEnumerable interface

		IEnumerator IEnumerable.GetEnumerator()
		{
			return m_modifiers.GetEnumerator();
		}

		#endregion
	}
}
