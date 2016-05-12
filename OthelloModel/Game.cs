using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace OthelloModel
{
	public enum eStage
	{
		None = 0,
		Started,
		BlackTern,
		WhiteTern,
		Finished,
	}

	public class StageChangedEventArgs : EventArgs
	{
		public eStage Stage { get; }
		public StageChangedEventArgs(eStage stage)
		{
			this.Stage = stage;
		}
	}

	public enum eResult
	{
		None = 0,
		BlackWon,
		WhiteWon,
	}

	public class GameResult
	{
		public int BlackCount { get; internal set; }
		public int WhitekCount { get; internal set; }
		public eResult Result { get; internal set; }

		internal GameResult()
		{
		}

		internal void Reset()
		{
			this.BlackCount = 0;
			this.WhitekCount = 0;
			this.Result = eResult.None;
		}
	}

	public class Game
	{
		public eStage Stage { get; private set; } = eStage.None;
		public GameResult Result { get; private set; } = new GameResult();
		public Boad Boad { get; } = new Boad();

		private readonly Subject<StageChangedEventArgs> m_evtStageChanged =
			new Subject<StageChangedEventArgs>();
		public IObservable<StageChangedEventArgs> evtStageChanged => m_evtStageChanged;

		public Game()
		{
			this.Reset();
			this.Boad.evtCellStateChanged.Subscribe(
				onNext: e =>
				{
					switch (e.OldSate)
					{
						case eCellState.Black: this.Result.BlackCount--; break;
						case eCellState.White: this.Result.WhitekCount--; break;
					}
					switch (e.NewSate)
					{
						case eCellState.Black: this.Result.BlackCount++; break;
						case eCellState.White: this.Result.WhitekCount++; break;
					}
				});
		}

		public void Reset()
		{
			this.Result.Reset();
			this.Boad.Clear();
			this.ChangeStage(eStage.None);
		}

		public void Start()
		{
			this.Boad.Init();
			this.ChangeStage(eStage.Started);
			this.ChangeStage(eStage.BlackTern);
		}

		public bool CanPutPeace(int x, int y)
		{
			switch (this.Stage)
			{
				case eStage.BlackTern:
					return this.Boad.CanPutPeace(x,y,eCellState.Black);
				case eStage.WhiteTern:
					return this.Boad.CanPutPeace(x,y,eCellState.White);
				default:
					return false;
			}
		}

		public void PutPieace(int x, int y)
		{
			switch (this.Stage)
			{
				case eStage.BlackTern:
					this.Boad.PutPieace(x, y, eCellState.Black);
					this.ChangeStage(eStage.WhiteTern);
					break;
				case eStage.WhiteTern:
					this.Boad.PutPieace(x, y, eCellState.White);
					this.ChangeStage(eStage.BlackTern);
					break;
			}
		}

		public void Pass()
		{
			switch (this.Stage)
			{
				case eStage.BlackTern:
					this.ChangeStage(eStage.WhiteTern);
					break;
				case eStage.WhiteTern:
					this.ChangeStage(eStage.BlackTern);
					break;
			}
		}

		private void ChangeStage(eStage stage)
		{
			this.Stage = stage;
			var e = new StageChangedEventArgs(stage);
			m_evtStageChanged.OnNext(e);
		}
	}
}
