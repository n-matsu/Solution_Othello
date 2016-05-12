using System;
using System.Collections.Generic;

namespace OthelloApp
{
	public class IndexedElem<TElem>
	{
		public TElem Elem { get; private set; }
		public int X { get; private set; }
		public int Y { get; private set; }
		public IndexedElem(TElem elem, int x, int y)
		{
			this.Elem = elem;
			this.X = x;
			this.Y = y;
		}
	}

	public static class ArrayExtensions
	{
		public static IEnumerable<IndexedElem<TElem>> EnumerateWithIndex<TElem>(this TElem[,] values)
		{
			for (int x = 0; x < values.GetLength(0); x++)
				for (int y = 0; y < values.GetLength(0); y++)
					yield return new IndexedElem<TElem>(values[x, y], x, y);
		}
	}

	public static class LinqExtensions
	{
		/// <summary>
		/// クエリ途中での副作用
		/// </summary>
		public static IEnumerable<T> Effect<T>(this IEnumerable<T> source, Action<T> action)
		{
			foreach (T item in source)
			{
				action(item);
				yield return item;
			}
		}
	}
}
