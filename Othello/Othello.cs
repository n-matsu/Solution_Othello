using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApplication1
{
	class Othello
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
		}

		public void SetCellState(int x, int y, eCellState newState)
		{
			var oldState = m_cells[x, y];
			if(oldState == newState) return;
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
			var last = Enumerable.Range(1, SIZE).Select(shift => new { X = vectorX * shift, Y = vectorY * shift }).TakeWhile(cell => newState != GetCellState(cell.X, cell.Y)).LastOrDefault();
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

		public eCellState GetCellState(int row, int col)
		{
			return m_cells[row, col];
		}
	}
}