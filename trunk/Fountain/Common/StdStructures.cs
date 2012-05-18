using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
	public interface IIndexable<T> : IEnumerable<T>, IEnumerable
	{
		T this[int index] { get; }
		int Count { get; }
	}

	public static class StdStructures
	{
		/// <summary>
		/// Private class for transforming an array into IIndexable.
		/// </summary>
		private class ArrayIndexable<T> : IIndexable<T>
		{
			private T[] m_array;

			public ArrayIndexable(T[] array)
			{
				if (array == null)
				{
					throw new ArgumentNullException("array");
				}

				m_array = array;
			}

			public T this[int index]
			{
				get { return m_array[index]; }
			}

			public int Count
			{
				get { return m_array.Length; }
			}

			public IEnumerator<T> GetEnumerator()
			{
				return (m_array as IEnumerable<T>).GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return m_array.GetEnumerator();
			}
		}

		public static IIndexable<T> ToIndexable<T>(this T[] array)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}

			return new ArrayIndexable<T>(array);
		}

		/// <summary>
		/// Private class for transforming a list into IIndexable.
		/// </summary>
		private class ListIndexable<T> : IIndexable<T>
		{
			private List<T> m_list;

			public ListIndexable(List<T> list)
			{
				if (list == null)
				{
					throw new ArgumentNullException("list");
				}

				m_list = list;
			}

			public T this[int index]
			{
				get { return m_list[index]; }
			}

			public int Count
			{
				get { return m_list.Count; }
			}

			public IEnumerator<T> GetEnumerator()
			{
				return m_list.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return m_list.GetEnumerator();
			}
		}

		public static IIndexable<T> ToIndexable<T>(this List<T> list)
		{
			return new ListIndexable<T>(list);
		}
	}

	public static class Indexable
	{
		private class EmptyIndexable<T> : IIndexable<T>
		{
			public T this[int index]
			{
				get { throw new ArgumentOutOfRangeException(); }
			}

			public int Count
			{
				get { return 0; }
			}

			public IEnumerator<T> GetEnumerator()
			{
				return Enumerable.Empty<T>().GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}

		public static IIndexable<T> Empty<T>()
		{
			return new EmptyIndexable<T>();
		}
	}
}
