﻿using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

#pragma warning disable 659

namespace bCC
{
	public abstract class Expression : Ast
	{
		[NotNull]
		public abstract Type GetExpressionType();

		protected Expression(MetaData metaData) : base(metaData)
		{
		}
	}

	public abstract class AtomicExpression : Expression
	{
		protected AtomicExpression(MetaData metaData) : base(metaData)
		{
		}
	}

	public sealed class NullExpression : Expression
	{
		public const string NullType = "nulltype";

		public NullExpression(MetaData metaData) : base(metaData)
		{
		}

		public override Type GetExpressionType() => new PrimaryType(MetaData, NullType);

		public override IEnumerable<string> Dump() => new[] {"null expression\n"};
	}

	public class LiteralExpression : AtomicExpression
	{
		[NotNull] public readonly Type Type;
		protected LiteralExpression(MetaData metaData, [NotNull] Type type) : base(metaData) => Type = type;
		public override Type GetExpressionType() => Type;
	}

	public class IntLiteralExpression : LiteralExpression
	{
		[NotNull] public readonly string Value;

		public IntLiteralExpression(MetaData metaData, [NotNull] string value, bool isSigned, int length = 32)
			: base(metaData, new PrimaryType(metaData, (isSigned ? "i" : "u") + length)) => Value = value;

		public override IEnumerable<string> Dump() =>
			new[] {"literal expression [" + Value + "]:\n"}
				.Concat(Type.Dump().Select(MapFunc));
	}

	public class BoolLiteralExpression : LiteralExpression
	{
		public readonly bool Value;

		public BoolLiteralExpression(MetaData metaData, bool value) : base(metaData, new PrimaryType(metaData, "bool")) =>
			Value = value;

		public override IEnumerable<string> Dump() => new[] {"bool literal expression [" + Value + "]:\n"};
	}

	/// <summary>
	/// A function is a variable with the type of lambda
	/// This is the class for anonymous lambda
	/// </summary>
	public class Lambda : AtomicExpression
	{
		[NotNull] public readonly StatementList Body;
		private Type _type;

		public override void SurroundWith(Environment environment)
		{
			base.SurroundWith(environment);
			Body.SurroundWith(Env);
			// FEATURE #12
			_type = Body.Statements.Last() is ReturnStatement ret
				? ret.Expression.GetExpressionType()
				// FEATURE #19
				: new PrimaryType(MetaData, "nulltype");
		}

		public override Type GetExpressionType() => _type;

		public Lambda(MetaData metaData, [NotNull] StatementList body) : base(metaData) => Body = body;
	}

	public class VariableExpression : AtomicExpression
	{
		public override void SurroundWith(Environment environment)
		{
			base.SurroundWith(environment);
			var declaration = Env.FindDeclarationByName(Name);
			if (declaration is VariableDeclaration variableDeclaration) _type = variableDeclaration.Type;
			else Errors.Add(MetaData.GetErrorHeader() + "Wtf");
		}

		[NotNull] public readonly string Name;
		private Type _type;

		public override Type GetExpressionType() => _type ?? throw new CompilerException();

		public VariableExpression(MetaData metaData, [NotNull] string name) : base(metaData) => Name = name;

		public override IEnumerable<string> Dump() => new[]
			{
				"variable expression [" + Name + "]:\n",
				"  type:\n"
			}
			.Concat(_type.Dump().Select(MapFunc2));
	}

	public class FunctionCallExpression : AtomicExpression
	{
		public override void SurroundWith(Environment environment)
		{
			base.SurroundWith(environment);
			Receiver.SurroundWith(Env);
			foreach (var expression in ParameterList) expression.SurroundWith(Env);
			// TODO check parameter type
			var hisType = Receiver.GetExpressionType();
			if (hisType is LambdaType lambdaType) _type = lambdaType.RetType;
			else
				Errors.Add(MetaData.GetErrorHeader() + "the function call receiver shoule be a function, not " + hisType + ".");
		}

		[NotNull] public readonly Expression Receiver;
		[NotNull] public readonly IList<Expression> ParameterList;
		private Type _type;

		public FunctionCallExpression(MetaData metaData, [NotNull] Expression receiver,
			[NotNull] IList<Expression> parameterList) :
			base(metaData)
		{
			ParameterList = parameterList;
			Receiver = receiver;
		}

		public override Type GetExpressionType() => _type ?? throw new CompilerException();

		public override IEnumerable<string> Dump() => new[]
			{
				"function call expression:\n",
				"  receiver:\n"
			}
			.Concat(Receiver.Dump().Select(MapFunc2))
			.Concat(new[] {"  parameters:\n"})
			.Concat(ParameterList.SelectMany(i => i.Dump().Select(MapFunc2)))
			.Concat(new[] {"  type:\n"})
			.Concat(_type.Dump().Select(MapFunc));
	}
}