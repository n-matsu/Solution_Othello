using Microsoft.VisualStudio.TestTools.UnitTesting;
using OthelloApp;
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
		Othello m_othello;

		[TestInitialize]
		public void TestInitialize()
		{
			m_othello = new Othello();
		}

		[TestCleanup]
		public void TestCleanup()
		{
			m_othello = null;
		}

		[TestMethod()]
		public void OthelloTest()
		{
			Assert.IsTrue(m_othello != null);
		}

		[TestMethod()]
		public void InitTest()
		{
			m_othello.Init();
			Assert.AreEqual(Othello.eCellState.Empty, m_othello.GetCellState(1, 2));
			Assert.AreEqual(Othello.eCellState.White, m_othello.GetCellState(3, 3));
			Assert.AreEqual(Othello.eCellState.Black, m_othello.GetCellState(3, 4));
		}

		[TestMethod()]
		public void SetCellStateTest()
		{
			m_othello.Init();
			m_othello.SetCellState(3, 2, Othello.eCellState.Black);
			Assert.AreEqual(Othello.eCellState.Black, m_othello.GetCellState(3, 2));
			Assert.AreEqual(Othello.eCellState.Black, m_othello.GetCellState(3, 3));
			Assert.AreEqual(Othello.eCellState.Empty, m_othello.GetCellState(2, 2));
			m_othello.SetCellState(2, 2, Othello.eCellState.White);
			Assert.AreEqual(Othello.eCellState.White, m_othello.GetCellState(3, 3));
			Assert.AreEqual(Othello.eCellState.Black, m_othello.GetCellState(3, 4));
		}

		[TestMethod()]
		public void GetCellStateTest()
		{
			m_othello.Init();
			Assert.AreEqual(Othello.eCellState.Empty, m_othello.GetCellState(1, 2));
		}
	}
}