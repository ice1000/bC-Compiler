﻿using System;
using Cmc;
using Cmc.Core;
using Cmc.Decl;
using Cmc.Expr;
using Cmc.Stmt;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Environment = Cmc.Core.Environment;

namespace CmcTest
{
	[TestClass]
	public class StatementTests
	{
		[TestInitialize]
		public void Init() => Errors.ErrList.Clear();

		private const string Var1 = "variableOne";

		public static Statement StmtAst3() => new StatementList(MetaData.Empty,
			new VariableDeclaration(MetaData.Empty, Var1,
				new BoolLiteralExpression(MetaData.Empty, true)),
			new ExpressionStatement(MetaData.Empty, new WhileExpression(MetaData.Empty,
				new VariableExpression(MetaData.Empty, Var1),
				new StatementList(MetaData.Empty,
					new ExpressionStatement(MetaData.Empty, new AssignmentExpression(MetaData.Empty,
						new VariableExpression(MetaData.Empty, Var1),
						new BoolLiteralExpression(MetaData.Empty, false)))))));

		public static Statement StmtAst4() => new StatementList(MetaData.Empty,
			new VariableDeclaration(MetaData.Empty, Var1,
				new BoolLiteralExpression(MetaData.Empty, true), true),
			new ExpressionStatement(MetaData.Empty, new WhileExpression(MetaData.Empty,
				new VariableExpression(MetaData.Empty, Var1),
				new StatementList(MetaData.Empty,
					new ExpressionStatement(MetaData.Empty, new AssignmentExpression(MetaData.Empty,
						new VariableExpression(MetaData.Empty, Var1),
						new BoolLiteralExpression(MetaData.Empty, false)))))));

		public static Statement StmtAst6() => new StatementList(MetaData.Empty,
			new VariableDeclaration(MetaData.Empty, Var1,
				new BoolLiteralExpression(MetaData.Empty, true), true),
			new ExpressionStatement(MetaData.Empty, new WhileExpression(MetaData.Empty,
				new VariableExpression(MetaData.Empty, Var1),
				new StatementList(MetaData.Empty,
					new ExpressionStatement(MetaData.Empty, new AssignmentExpression(MetaData.Empty,
						new VariableExpression(MetaData.Empty, Var1),
						new IntLiteralExpression(MetaData.Empty, "123", true)))))));

		public static Statement StmtAst5() => new StatementList(MetaData.Empty,
			new VariableDeclaration(MetaData.Empty, Var1,
				new BoolLiteralExpression(MetaData.Empty, true), true),
			new ExpressionStatement(MetaData.Empty, new WhileExpression(MetaData.Empty,
				new VariableExpression(MetaData.Empty, Var1),
				new StatementList(MetaData.Empty,
					new ExpressionStatement(MetaData.Empty, new AssignmentExpression(MetaData.Empty,
						new VariableExpression(MetaData.Empty, Var1),
						new NullExpression(MetaData.Empty)))))));

		public static Statement StmtAst2() => new ExpressionStatement(MetaData.Empty,
			new IfExpression(
				MetaData.Empty,
				new NullExpression(MetaData.Empty),
				new StatementList(MetaData.Empty)
			));

		public static Statement StmtAst1() => new ExpressionStatement(MetaData.Empty,
			new IfExpression(
				MetaData.Empty,
				new BoolLiteralExpression(MetaData.Empty, false),
				new StatementList(MetaData.Empty)
			));

		/// <summary>
		///     simplest test
		/// </summary>
		[TestMethod]
		public void StatementTest1()
		{
			foreach (var stmt in
				new StatementList(MetaData.Empty,
					new Statement(MetaData.Empty)).Statements)
				stmt.PrintDumpInfo();
		}

		/// <summary>
		///     check for condition type
		/// </summary>
		[TestMethod]
		public void StatementTest2()
		{
			var stmt = StmtAst1();
			stmt.SurroundWith(Environment.SolarSystem);
			stmt.PrintDumpInfo();
			Assert.IsTrue(0 == Errors.ErrList.Count);
			var stmt2 = StmtAst2();
			stmt2.SurroundWith(Environment.SolarSystem);
			Console.WriteLine("");
			Console.WriteLine("");
			Assert.IsTrue(0 != Errors.ErrList.Count);
			foreach (var s in Errors.ErrList)
				Console.WriteLine(s);
			stmt2.PrintDumpInfo();
		}

		/// <summary>
		///     check for mutability
		/// </summary>
		[TestMethod]
		public void StatementTest3()
		{
			var stmt = StmtAst3();
			stmt.SurroundWith(Environment.SolarSystem);
			stmt.PrintDumpInfo();
			Assert.IsTrue(0 != Errors.ErrList.Count);
			Errors.PrintErrorInfo();
		}

		/// <summary>
		///     type check (when it's correct)
		/// </summary>
		[TestMethod]
		public void StatementTest4()
		{
			var stmt = StmtAst4();
			stmt.SurroundWith(Environment.SolarSystem);
			stmt.PrintDumpInfo();
			Assert.IsTrue(0 == Errors.ErrList.Count);
		}

		/// <summary>
		///     type check (when it's nulltype)
		/// </summary>
		[TestMethod]
		public void StatementTest5()
		{
			var stmt = StmtAst5();
			stmt.SurroundWith(Environment.SolarSystem);
			stmt.PrintDumpInfo();
			Assert.IsTrue(0 == Errors.ErrList.Count);
		}

		/// <summary>
		///     type check (when it's incorrect)
		/// </summary>
		[TestMethod]
		public void StatementTest6()
		{
			var stmt = StmtAst6();
			stmt.SurroundWith(Environment.SolarSystem);
			stmt.PrintDumpInfo();
			Assert.IsTrue(0 != Errors.ErrList.Count);
			Errors.PrintErrorInfo();
		}
	}
}