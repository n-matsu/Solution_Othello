using Microsoft.VisualStudio.TestTools.UnitTesting;
using OthelloModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OthelloApp.Tests
{
	[TestClass()]
	public class OthelloTests
	{
		Game m_game;

		[TestInitialize]
		public void TestInitialize()
		{
			m_game = new Game();
		}

		[TestCleanup]
		public void TestCleanup()
		{
			m_game = null;
		}

		[TestMethod()]
		public void OthelloTest()
		{
			Assert.IsTrue(m_game != null);
		}

		[TestMethod()]
		public void InitTest()
		{
			m_game.Reset();
			m_game.Start();
			Assert.AreEqual(eCellState.Empty, m_game.Boad.GetCellState(1, 2));
			Assert.AreEqual(eCellState.White, m_game.Boad.GetCellState(3, 3));
			Assert.AreEqual(eCellState.Black, m_game.Boad.GetCellState(3, 4));
		}

		[TestMethod()]
		public void GameTest()
		{
			m_game.Reset();
			m_game.Start();
			Assert.IsTrue(m_game.CanPutPeace(3, 2));
			m_game.PutPieace(3, 2);
			Assert.AreEqual(eCellState.Black, m_game.Boad.GetCellState(3, 2));
			Assert.AreEqual(eCellState.Black, m_game.Boad.GetCellState(3, 3));
			Assert.AreEqual(eCellState.Empty, m_game.Boad.GetCellState(2, 2));
			m_game.PutPieace(2, 2);
			Assert.AreEqual(eCellState.White, m_game.Boad.GetCellState(3, 3));
			Assert.AreEqual(eCellState.Black, m_game.Boad.GetCellState(3, 4));
		}
	}
}