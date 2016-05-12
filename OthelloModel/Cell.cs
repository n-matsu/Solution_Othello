using System;

namespace OthelloModel
{
	public enum eCellState
	{
		Empty = 0,
		Black = -1,
		White = 1,
	}

	public abstract class CellEventArgs : EventArgs
	{
		public int X { get; }
		public int Y { get; }
		public CellEventArgs(int x, int y)
		{
			this.X = x;
			this.Y = y;
		}
	}

	public class CellStateChangedEventArgs : CellEventArgs
	{
		public eCellState OldSate { get; }
		public eCellState NewSate { get; }
		public CellStateChangedEventArgs(
			int x, int y,
			eCellState oldState, eCellState newState)
			: base(x, y)
		{
			this.OldSate = oldState;
			this.NewSate = NewSate;
		}
	}

	public static class CellExtensions
	{
		public static string GetDispText(this eCellState state)
		{
			switch (state)
			{
				case eCellState.Black: return "b";
				case eCellState.White: return "w";
				default: return " ";
			}
		}
	}
}
