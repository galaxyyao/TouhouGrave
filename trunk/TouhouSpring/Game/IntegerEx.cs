using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
	/// <summary>
	/// An integer class representing an integer modified by a sequence of expressions
	/// </summary>
	public class IntegerEx
	{
		public static Func<int, int>[] Decrease = new Func<int, int>[]
		{
			x => x - 0,
			x => x - 1,
			x => x - 2,
			x => x - 3,
			x => x - 4,
			x => x - 5,
			x => x - 6,
			x => x - 7,
			x => x - 8,
			x => x - 9,
		} ;

		/// <summary>
		/// A collection for all alive IntegerEx objects
		/// </summary>
		private static HashSet<WeakReference> s_allObjects = new HashSet<WeakReference>();

		private int? m_lockedValue;
		private List<Func<int, int>> m_modifiers = new List<Func<int, int>>();

		/// <summary>
		/// The initial value
		/// </summary>
		public int BaseValue
		{
			get;
			private set;
		}

		/// <summary>
		/// The calculated value
		/// </summary>
		public int FinalValue
		{
			get { return m_lockedValue ?? CalculateFinalValue(); }
		}

		/// <summary>
		/// Implicit conversion to int using the FinalValue
		/// </summary>
		/// <param name="integerEx">The IntegerEx object</param>
		/// <returns>The FinalValue property</returns>
		public static implicit operator int(IntegerEx integerEx)
		{
			return integerEx.FinalValue;
		}

		/// <summary>
		/// The sequence of expressions modifying the integer
		/// </summary>
		public IIndexable<Func<int, int>> Modifiers
		{
			get { return m_modifiers.ToIndexable(); }
		}

		public IntegerEx(int baseValue)
		{
			BaseValue = baseValue;

			s_allObjects.Add(new WeakReference(this));
		}

		/// <summary>
		/// Add a new expression i.e. "modifier" to the tail of the sequence.
		/// </summary>
		/// <param name="modifier">The expression to be added</param>
		public void AddModifierToTail(Func<int, int> modifier)
		{
			if (modifier == null)
			{
				throw new ArgumentNullException("modifier");
			}

			m_modifiers.Add(modifier);
		}

		/// <summary>
		/// Remove a specified expression of the modifying sequence.
		/// </summary>
		/// <param name="modifier">The expression to be removed</param>
		public void RemoveModifier(Func<int, int> modifier)
		{
			if (modifier == null)
			{
				throw new ArgumentNullException("modifier");
			}

			m_modifiers.Remove(modifier);
		}

		public void RemoveLastModifier()
		{
			if (m_modifiers.Count == 0)
				return;
			m_modifiers.Remove(m_modifiers.Last());
		}

		private int CalculateFinalValue()
		{
			return Modifiers.Aggregate(BaseValue, (value, modifier) => modifier(value));
		}

		private void LockValue()
		{
			if (m_lockedValue != null)
			{
				throw new InvalidOperationException("Already locked.");
			}

			m_lockedValue = CalculateFinalValue();
		}

		private void UnlockValue()
		{
			if (m_lockedValue == null)
			{
				throw new InvalidOperationException("Already unlocked.");
			}

			m_lockedValue = null;
		}

		private static void LockAll()
		{
			s_allObjects.Where(wr => wr.IsAlive).ForEach(wr => ((IntegerEx)wr.Target).LockValue());
			s_allObjects.RemoveWhere(wr => !wr.IsAlive);
		}

		private static void UnlockAll()
		{
			s_allObjects.Where(wr => wr.IsAlive).ForEach(wr => ((IntegerEx)wr.Target).UnlockValue());
		}

		/// <summary>
		/// This class is used for locking/unlocking all alive IntegerEx objects so that their final values
		/// remain the same values when the locking happens.
		/// </summary>
		public class LockValues : IDisposable
		{
			private bool m_disposed = false;

			public LockValues()
			{
				LockAll();
			}

			~LockValues()
			{
				Dispose(false);
			}

			public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}

			private void Dispose(bool disposing)
			{
				if (!m_disposed)
				{
					UnlockAll();
					m_disposed = true;
				}
			}
		}
	}
}
