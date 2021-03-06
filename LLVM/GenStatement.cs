﻿using Cmc.Decl;
using Cmc.Stmt;
using JetBrains.Annotations;
using LLVMSharp;

namespace LLVM
{
	public static class GenStatement
	{
		public static void GenAstStatement(
			LLVMModuleRef module,
			LLVMBuilderRef builder,
			[NotNull] Statement element)
		{
			switch (element)
			{
				case ReturnStatement returnStatement:
					break;
				case ExpressionStatement expression:
					break;
				case StatementList statements:
					break;
				case Declaration declaration:
					break;
			}
		}
	}
}