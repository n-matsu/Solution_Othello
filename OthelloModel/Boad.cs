using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace OthelloModel
{
	public class Boad
	{
		internal class Point
		{
			public int X { get; }
			public int Y { get; }
			public Point(int x, int y)
			{
				this.X = x;
				this.Y = y;
			}
		}

		private const int SIZE = 8;
		private readonly eCellState[,] m_cells = new eCellState[SIZE, SIZE];

		private readonly Subject<CellStateChangedEventArgs> m_evtCellStateChanged =
			new Subject<CellStateChangedEventArgs>();
		public IObservable<CellStateChangedEventArgs> evtCellStateChanged => m_evtCellStateChanged;

		public Boad()
		{
		}

		public void Init()
		{
			this.Clear();
			this.SetCellState(3, 3, eCellState.White);
			this.SetCellState(3, 4, eCellState.Black);
			this.SetCellState(4, 3, eCellState.Black);
			this.SetCellState(4, 4, eCellState.White);
			this.PrintDebug();
		}

		public void Clear()
		{
			m_cells.EnumerateWithIndex()
				.Effect(cell => m_cells[cell.X, cell.Y] = eCellState.Empty);
		}

		public eCellState GetCellState(int x, int y)
		{
			return m_cells[x, y];
		}

		/// <returns>変更された場合true</returns>
		private bool SetCellState(int x, int y, eCellState newState, bool notify = false)
		{
			var oldState = this.GetCellState(x, y);
			if (oldState == newState) return false;
			m_cells[x, y] = newState;
			if (notify)
			{
				var e = new CellStateChangedEventArgs(x, y, oldState, newState);
				(evtCellStateChanged as ISubject<CellStateChangedEventArgs>).OnNext(e);
			}
			return true;
		}

		private IEnumerable<Point> GetAllVectors()
		{
			var vectors = new[] { -1, 0, 1 };
			return
				vectors
					.Select(vector => new { vX = vector, vYs = vectors })
					.SelectMany(vector => vector.vYs, (vector, vY) => new Point(vector.vX, vY));
		}

		public bool CanPutPeace(int x, int y, eCellState newState)
		{
			return
				(eCellState.Empty == this.GetCellState(x, y)) &&
				this.GetAllVectors()
					.AsParallel()
					.Any(vector => DoesNeedToTurnOverCells(x, y, vector.X, vector.Y));
		}

		public void PutPieace(int x, int y, eCellState newState)
		{
			if (!this.SetCellState(x, y, newState)) return;

			//変更セルの周囲8方向を単位ベクトル組み合わせで列挙、方向別に反転の判定・実行
			this.GetAllVectors()
				.AsParallel()
				.Where(vector => DoesNeedToTurnOverCells(x, y, vector.X, vector.Y))
				.Effect(vector => TurnOverCells(x, y, vector.X, vector.Y))
				.ToList();
			this.PrintDebug();
		}

		private bool DoesNeedToTurnOverCells(int changedX, int changedY, int vectorX, int vectorY)
		{
			//単位ベクトル方向に、変更セルから近いセル順に列挙・要反転判定
			int nToUpdate = 0;
			eCellState newState = GetCellState(changedX, changedY);
			var last = Enumerable.Range(1, SIZE - 1)
				.Select(shift => new
				{
					X = changedX + vectorX * shift,
					Y = changedY + vectorY * shift
				})
				.TakeWhile(cell =>
					0 <= cell.X && cell.X < SIZE &&
					0 <= cell.Y && cell.Y < SIZE &&
					eCellState.Empty != GetCellState(cell.X, cell.Y))
				.Effect(cell => nToUpdate++)
				.SkipWhile(cell => newState != GetCellState(cell.X, cell.Y))
				.Take(1)
				.LastOrDefault();
			return last != null && 1 < nToUpdate;
		}

		private void TurnOverCells(int changedX, int changedY, int vectorX, int vectorY)
		{
			//単位ベクトル方向に、変更セルから近いセル順に列挙・反転処理
			eCellState newState = GetCellState(changedX, changedY);
			Enumerable.Range(1, SIZE - 1)
				.ToObservable()
				.Select(shift => new
				{
					X = changedX + vectorX * shift,
					Y = changedY + vectorY * shift
				})
				.TakeWhile(cell =>
					0 <= cell.X && cell.X < SIZE &&
					0 <= cell.Y && cell.Y < SIZE &&
					eCellState.Empty != GetCellState(cell.X, cell.Y) &&
					newState != GetCellState(cell.X, cell.Y))
				.Subscribe(cell => SetCellState(cell.X, cell.Y, newState, true));
		}

		private void PrintDebug()
		{
			for (int x = 0; x < m_cells.GetLength(0); x++)
			{
				for (int y = 0; y < m_cells.GetLength(1); y++)
				{
					Console.Write($"[{m_cells[x, y].GetDispText()}]");
				}
				Console.WriteLine();
			}
		}
	}
}