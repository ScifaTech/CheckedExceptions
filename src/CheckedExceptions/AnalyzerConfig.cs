using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;

namespace Scifa.CheckedExceptions
{
	internal class AnalyzerConfig
	{
		private List<Regex> namespaceRegexes = new List<Regex>();

		internal bool ShouldAnalyze(INamedTypeSymbol containingType)
		{
			if (namespaceRegexes.Count == 0)
				return true;

			foreach (var mask in namespaceRegexes)
			{
				if (mask.IsMatch(containingType.ContainingNamespace.Name))
					return true;
			}

			return false;
		}

		internal void SetNamespaceMasks(List<string> namespaceMasks)
		{
			foreach (var mask in namespaceMasks)
			{
				string rx = "^" + Regex.Escape(mask).Replace(@"\*\*", @"\.\*").Replace(@"\*", @"[^\.]*") + "$";
				namespaceRegexes.Add(new Regex(rx));
			}
		}
	}
}