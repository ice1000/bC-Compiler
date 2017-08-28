﻿using System.Diagnostics;
using System.IO;
using bCC;
using bCC.Core;

namespace LLVM
{
	public class Gen
	{
		public void Generate(
			string outputFile,
			params Declaration[] declarations)
		{
			var core = new Core();
			var analyzedDeclarations = core.Analyze(declarations);
			// TODO run code gen
			File.WriteAllText(outputFile, "");
		}
	}
}