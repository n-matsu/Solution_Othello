using System.Linq;

namespace OthelloApp
{
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
		private readonly eCellState[,] m_cells = new eCellState[SIZE, SIZE];

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

			//変更セルの周囲8方向を単位ベクトル組み合わせで列挙、方向別に反転の判定・実行
			var vectors = new[] { -1, 0, 1 };
			vectors
				.Select(vector => new { vX = vector, vYs = vectors })
				.SelectMany(vector => vector.vYs, (vector, vY) => new { X = vector.vX, Y = vY })
				.AsParallel()
				.Where(vector => SetCellState_HasToUpdate(x, y, vector.X, vector.Y))
				.Effect(vector => SetCellState_Update(x, y, vector.X, vector.Y))
				.ToList();
		}

		private bool SetCellState_HasToUpdate(int changedX, int changedY, int vectorX, int vectorY)
		{
			//単位ベクトル方向に、変更セルから近いセル順に列挙・判定
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

		private void SetCellState_Update(int changedX, int changedY, int vectorX, int vectorY)
		{
			//単位ベクトル方向に、変更セルから近いセル順に列挙・反転処理
			eCellState newState = GetCellState(changedX, changedY);
			Enumerable.Range(1, SIZE - 1)
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
				.Effect(cell => SetCellState(cell.X, cell.Y, newState))
				.ToList();
		}

		public eCellState GetCellState(int x, int y)
		{
			return m_cells[x, y];
		}


	}
}