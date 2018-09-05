using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;

namespace Scifa.CheckedExceptions
{
	internal class AnalyzerConfig
	{
		private readonly bool checkAssembly;

		public AnalyzerConfig(IEnumerable<AttributeData> checkExceptionAttributes)
		{
			checkAssembly = checkExceptionAttributes.Any();
		}

		public bool ShouldAnalyze(INamedTypeSymbol containingType)
		{
			return checkAssembly;
		}
	}
}