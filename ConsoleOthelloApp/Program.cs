using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OthelloModel;
using System.Reactive;

namespace ConsoleOthelloApp
{
	class Program
	{
		static void Main(string[] args)
		{
			Game game = new Game();
			game.evtStageChanged.Subscribe(
				onNext: e =>
				{
					switch (e.Stage)
					{
						case eStage.Started:
							Console.WriteLine("started.");
							break;
						case eStage.BlackTern:
							ReadInputAndPutPiece(game, eCellState.Black, "【黒】");
							break;
						case eStage.WhiteTern:
							ReadInputAndPutPiece(game, eCellState.Black, "【白】");
							break;
						case eStage.Finished:
							Console.WriteLine("finished.");
							PrintGame(game);
							break;
					}
				});
			game.Start();
		}

		private static void ReadInputAndPutPiece(Game game, eCellState cellStage, string label)
		{
			int x = 0;
			int y = 0;
			bool valid = false;
			while (!valid)
			{
				PrintGame(game);
				Console.WriteLine($"{label}の順番です。");
				ReadNumber("行(1-8)：", out x);
				ReadNumber("列(1-8)：", out y);
				//内部はインデクスで指定
				x -= 1;
				y -= 1;
				valid = game.CanPutPeace(x, y);
				if (!valid)
					Console.WriteLine("その場所には打てません。");
			}
			game.PutPieace(x, y);
		}

		private static void ReadNumber(string label, out int num)
		{
			bool valid = false;
			do
			{
				Console.Write(label);
				valid = int.TryParse(Console.ReadLine(), out num);
				valid &= (1 <= num && num <= Boad.SIZE);
			}
			while (!valid);
		}

		private static void PrintGame(Game game)
		{
			Console.Write(" ");
			foreach (var col in Enumerable.Range(1, Boad.SIZE))
				Console.Write($" {col} ");
			Console.WriteLine();
			foreach (var item in game.Boad.Cells)
			{
				if (item.Y == 0) Console.Write($"{item.X + 1}");
				Console.Write($"[{item.Elem.GetDispText()}]");
				if (item.Y == Boad.SIZE - 1) Console.WriteLine();
			}
		}
	}
}
