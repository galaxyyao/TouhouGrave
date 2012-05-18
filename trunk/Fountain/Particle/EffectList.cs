using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace TouhouSpring.Particle
{
	public class EffectList : IList<Effect>
	{
		public class ContentReaders : ContentTypeReader<EffectList>
		{
			public override bool CanDeserializeIntoExistingObject
			{
				get { return true; }
			}

			protected override EffectList Read(ContentReader input, EffectList existingInstance)
			{
				if (existingInstance == null)
				{
					throw new InvalidOperationException("EffectList can only be deserialized from an existing instance.");
				}

				int count = input.ReadInt32();
				for (int i = 0; i < count; ++i)
				{
					existingInstance.Add(input.ReadObject<Effect>((Effect)null));
				}

				return existingInstance;
			}
		}

		private ParticleSystem m_system;
		private List<Effect> m_effects;

		public EffectList(ParticleSystem system)
		{
			if (system == null)
			{
				throw new ArgumentNullException("system");
			}

			m_system = system;
			m_effects = new List<Effect>();
		}

		#region IList<T> interface

		public Effect this[int index]
		{
			get { return m_effects[index]; }
			set
			{
				if (index < 0 || index > m_effects.Count)
				{
					throw new ArgumentOutOfRangeException("Index is out of range.");
				}
				else if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				else if (m_effects.Contains(value))
				{
					throw new ArgumentException("Item is already in the list.");
				}
				else if (value.System != null)
				{
					throw new InvalidOperationException("Item is contained in another EffectList.");
				}

				var old = m_effects[index];
				if (old != value)
				{
					m_effects[index] = value;
					old.System = null;
					value.System = m_system;
				}
			}
		}

		public int IndexOf(Effect item)
		{
			return m_effects.IndexOf(item);
		}

		public void Insert(int index, Effect item)
		{
			if (index < 0 || index > m_effects.Count)
			{
				throw new ArgumentOutOfRangeException("Index is out of range.");
			}
			else if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			else if (m_effects.Contains(item))
			{
				throw new ArgumentException("Item is already in the list.");
			}
			else if (item.System != null)
			{
				throw new InvalidOperationException("Item is contained in another EffectList.");
			}

			m_effects.Insert(index, item);
			item.System = m_system;
		}

		public void RemoveAt(int index)
		{
			if (index < 0 || index > m_effects.Count)
			{
				throw new ArgumentOutOfRangeException("Index is out of range.");
			}

			var item = m_effects[index];
			m_effects.RemoveAt(index);
			item.System = null;
		}

		#endregion

		#region ICollection<T> interface

		public int Count
		{
			get { return m_effects.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public void Add(Effect item)
		{
			Insert(Count, item);
		}

		public void Clear()
		{
			m_effects.ForEach(m => m.System = null);
			m_effects.Clear();
		}

		public bool Contains(Effect item)
		{
			return m_effects.Contains(item);
		}

		public void CopyTo(Effect[] array, int arrayIndex)
		{
			m_effects.CopyTo(array, arrayIndex);
		}

		public bool Remove(Effect item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}

			if (m_effects.Remove(item))
			{
				item.System = null;
				return true;
			}

			return false;
		}

		#endregion

		#region IEnumerable<T> interface

		public IEnumerator<Effect> GetEnumerator()
		{
			return m_effects.GetEnumerator();
		}

		#endregion

		#region IEnumerable interface

		IEnumerator IEnumerable.GetEnumerator()
		{
			return m_effects.GetEnumerator();
		}

		#endregion
	}
}
