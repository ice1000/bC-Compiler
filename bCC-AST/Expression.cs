﻿using System.Collections.Generic;
using System.Linq;

#pragma warning disable 659

namespace bCC_AST
{
	public abstract class Expression : IAst
	{
		public abstract IList<Declaration> GetDependencies();
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

	/// <summary>
	/// A function is a variable with the type of lambda
	/// This is the class for anonymous lambda
	/// </summary>
	public class LambdaDeclaration : AtomicExpression
	{
		public readonly StatementList Body;

		public override IList<Declaration> GetDependencies() =>
			Body.Statements.SelectMany(i => i.GetDependencies()).ToList();

		public override Type GetExpressionType()
		{
			throw new System.NotImplementedException();
		}

		public LambdaDeclaration(MetaData metaData, string name, StatementList body) : base(metaData, name)
		{
			Body = body;
		}

		/// TODO: add type check
		public override bool Equals(object obj) => obj is Declaration declaration && declaration.Name == Name;
	}

	public class VariableExpression : AtomicExpression
	{
		public readonly string Name;

		public override IList<Declaration> GetDependencies() => new List<Declaration> {new Declaration(MetaData, Name)};

		public override Type GetExpressionType()
		{
			throw new System.NotImplementedException();
		}

		public VariableExpression(MetaData metaData, string name) : base(metaData) => Name = name;
	}

	public class FunctionCallExpression : AtomicExpression
	{
		public readonly string Name;
		public readonly IList<Expression> ParameterList;

		public FunctionCallExpression(MetaData metaData, string name, IList<Expression> parameterList) : base(metaData)
		{
			ParameterList = parameterList;
			Name = name;
		}

		public override IList<Declaration> GetDependencies() =>
			ParameterList.SelectMany(param => param.GetDependencies()).ToList();

		public override Type GetExpressionType()
		{
			throw new System.NotImplementedException();
		}
	}
}