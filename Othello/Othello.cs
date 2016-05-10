using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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

	public class Othello
	{
		public enum eCellState
		{
			Empty = 0,
			Black = -1,
			White = 1,
		}

		class Action
		{
			public enum eAction
			{
				Put,
			}
		}

		private const int SIZE = 8;
		private readonly eCellState[,] m_cells = new eCellState[SIZE + 1, SIZE + 1];

		public Othello()
		{
		}

		public void Init()
		{
			m_cells.EnumerateWithIndex()
				.Select(cell => { m_cells[cell.X, cell.Y] = eCellState.Empty; return true; })
				.ToList();
			m_cells[3, 3] = eCellState.White;
			m_cells[3, 4] = eCellState.Black;
			m_cells[4, 3] = eCellState.Black;
			m_cells[4, 4] = eCellState.White;
		}

		public void SetCellState(int x, int y, eCellState newState)
		{
			var oldState = m_cells[x, y];
			if (oldState == newState) return;
			m_cells[x, y] = newState;

			var vectors = new[] { -1, 0, 1 };
			vectors
			.Select(vector => new { vX = vector, vYs = vectors })
			.SelectMany(vector => vector.vYs, (vector, vY) => new { X = vector.vX, Y = vY })
			.AsParallel()
			.Where(vector => SetCellState_HasToUpdate(x, y, vector.X, vector.Y))
			.Select(vector => { SetCellState_Update(x, y, vector.X, vector.Y); return true; })
			.ToList();
		}

		private bool SetCellState_HasToUpdate(int changedX, int changedY, int vectorX, int vectorY)
		{
			eCellState newState = GetCellState(changedX, changedY);
			var last = Enumerable.Range(1, SIZE)
				.Select(shift => new { X = vectorX * shift, Y = vectorY * shift })
				.TakeWhile(cell => newState != GetCellState(cell.X, cell.Y))
				.LastOrDefault();
			return last.X != 0 && last.Y != 0;
		}

		private void SetCellState_Update(int changedX, int changedY, int vectorX, int vectorY)
		{
			eCellState newState = GetCellState(changedX, changedY);
			Enumerable.Range(1, SIZE)
				.Select(shift => new { X = vectorX * shift, Y = vectorY * shift })
				.TakeWhile(cell => newState != GetCellState(cell.X, cell.Y))
				.Select(cell => { SetCellState(cell.X, cell.Y, newState); return true; })
				.ToList();
		}

		public eCellState GetCellState(int x, int y)
		{
			return m_cells[x, y];
		}
	}
}