using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
	public static class StdAlgorithms
	{
		public static bool AnyFrom<T>(this IIndexable<T> src, int start, Predicate<T> pred)
		{
			for (int i = start; i < src.Count; ++i)
			{
				if (pred(src[i]))
				{
					return true;
				}
			}

			return false;
		}

		public static IIndexable<T> Clone<T>(this IIndexable<T> src)
		{
			T[] dst = new T[src.Count];
			src.Count.Repeat(i => dst[i] = src[i]);
			return dst.ToIndexable();
		}

		public static IIndexable<T2> Clone<T1, T2>(this IIndexable<T1> src, Func<T1, T2> converter)
		{
			T2[] dst = new T2[src.Count];
			src.Count.Repeat(i => dst[i] = converter(src[i]));
			return dst.ToIndexable();
		}

        public static T First<T>(this IList<T> list)
        {
            return list[0];
        }

		/// <summary>
		/// Perform a specified action on every element of an collection.
		/// <param name="source">An IEnumerable&lt;T&gt; collection in which the iteration is performed.</param>
		/// <param name="func">The action to be performed.</param>
		/// </summary>
		public static void ForEach<T>(this IEnumerable<T> source, Action<T> func)
		{
			foreach (T elem in source)
			{
				func(elem);
			}
		}

        public static int FindIndex<T>(this IEnumerable<T> source, Predicate<T> predicate)
        {
            int index = 0;
            foreach (T elem in source)
            {
                if (predicate(elem))
                {
                    return index;
                }
                ++index;
            }

            return -1;
        }

		public static int IndexOf<T>(this IIndexable<T> src, T elem)
		{
			for (int i = 0; i < src.Count; ++i)
			{
				if (src[i].Equals(elem))
				{
					return i;
				}
			}
			return -1;
		}

        public static T Last<T>(this IList<T> list)
        {
            return list[list.Count - 1];
        }

        public static void Repeat(this int times, Action func)
        {
            for (int i = 0; i < times; ++i)
            {
                func();
            }
        }

		public static void Repeat(this int times, Action<int> func)
		{
			for (int i = 0; i < times; ++i)
			{
				func(i);
			}
		}

		public static bool Unique<T>(this IIndexable<T> src)
		{
			// naive implementation with O(n2) time complexity.
			for (int i = src.Count - 1; i > 0; --i)
			{
				int j = src.IndexOf(src[i]);
				if (j != -1 && j != i)
				{
					return false;
				}
			}

			return true;
		}
	}
}
