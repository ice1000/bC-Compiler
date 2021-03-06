﻿using System.Collections.Generic;
using System.Linq;
using Cmc.Core;
using Cmc.Decl;
using Cmc.Expr;
using JetBrains.Annotations;

namespace Cmc.Stmt
{
	/// <summary>
	///  when codegen, use `converted statement`
	/// </summary>
	public class ReturnStatement : ExpressionStatement
	{
		public ReturnLabelDeclaration ReturnLabel;
		[CanBeNull] private readonly string _labelName;
		public VariableDeclaration ConvertedVariableDeclaration;
		public ReturnStatement ConvertedReturnStatement;

		public ReturnStatement(
			MetaData metaData,
			[CanBeNull] Expression expression = null,
			[CanBeNull] string labelName = null) :
			base(metaData, expression ?? new NullExpression(metaData))
		{
			_labelName = labelName;
		}

		public override void SurroundWith(Environment environment)
		{
			// base.SurroundWith(environment);
			Env = environment;
			Expression.SurroundWith(Env);
			if (null != ReturnLabel) return;
			var returnLabel = Env.FindReturnLabelByName(_labelName ?? "");
			if (null == returnLabel)
				Errors.AddAndThrow($"{MetaData.GetErrorHeader()}cannot return outside a lambda");
			else
			{
				ReturnLabel = returnLabel;
				ReturnLabel.StatementsUsingThis.Add(this);
			}
			if (Expression is AtomicExpression) return;
			var variableName = $"genRet{(ulong) GetHashCode()}";
			ConvertedVariableDeclaration =
				new VariableDeclaration(MetaData, variableName, Expression, type: Expression.GetExpressionType());
			ConvertedReturnStatement = new ReturnStatement(MetaData,
				new VariableExpression(MetaData, variableName), _labelName)
			{
				ReturnLabel = ReturnLabel
			};
			ConvertedStatementList = new StatementList(MetaData,
				ConvertedVariableDeclaration,
				ConvertedReturnStatement);
		}

		/// <summary>
		///   FEATURE #45
		///   make this an inlined return statement
		/// </summary>
		/// <param name="returnValueStorer">the variable used to store the return value</param>
		public void Unify([NotNull] VariableExpression returnValueStorer)
		{
			if (null != ConvertedStatementList)
				ConvertedStatementList = new StatementList(MetaData,
					ConvertedVariableDeclaration,
					new ExpressionStatement(MetaData,
						new AssignmentExpression(MetaData, returnValueStorer,
							ConvertedReturnStatement.Expression)),
					new GotoStatement(MetaData, ReturnLabel.GetLabel().Name));
			else
				ConvertedStatementList = new StatementList(MetaData,
					new ExpressionStatement(MetaData,
						new AssignmentExpression(MetaData, returnValueStorer, Expression)),
					new GotoStatement(MetaData, ReturnLabel.GetLabel().Name));
		}

		public override IEnumerable<string> Dump() => new[]
				{$"return statement [{ReturnLabel}]:\n"}
			.Concat(Expression.Dump().Select(MapFunc));

		public override IEnumerable<string> DumpCode() =>
			new[] {$"return:{ReturnLabel} {string.Join("", Expression.DumpCode())};\n"};
	}

	public class ExitStatement : Statement
	{
		[NotNull] public readonly AtomicExpression Expression;

		public ExitStatement(
			MetaData metaData,
			[NotNull] AtomicExpression expression) : base(metaData) => Expression = expression;

		public override IEnumerable<string> Dump() => new[]
				{"return statement:\n"}
			.Concat(Expression.Dump().Select(MapFunc));

		public override IEnumerable<string> DumpCode() =>
			new[] {$"return {string.Join("", Expression.DumpCode())};\n"};
	}
}