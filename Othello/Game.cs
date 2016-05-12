using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace OthelloApp
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
		public eCellState NewSate { get; }
		public StageChangedEventArgs(eStage newState)
		{
			this.NewSate = NewSate;
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

		public void Start()
		{
			this.Boad.Init();
			this.ChangeStage(eStage.Started);
		}

		private void Reset()
		{
			this.Result.Reset();
			this.Boad.Clear();
			this.ChangeStage(eStage.None);
		}

		private void ChangeStage(eStage stage)
		{
			var e = new StageChangedEventArgs(stage);
			m_evtStageChanged.OnNext(e);
		}
	}
}
