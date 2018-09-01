using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Scifa.CheckedExceptions
{
	public static class RoslynHelpers
	{
		[Pure]
		public static string GetFullName(this INamespaceOrTypeSymbol symbol)
		{
			Stack<string> parts = new Stack<string>();
			do
			{
				parts.Push(symbol.Name);
				symbol = symbol.ContainingType ?? (INamespaceOrTypeSymbol)symbol.ContainingNamespace;
			} while (symbol != null);

			StringBuilder sb = new StringBuilder();
			while (parts.Count > 0)
			{
				string part = parts.Pop();
				if (!string.IsNullOrEmpty(part))
					sb.Append(part).Append(".");
			}
			if (sb.Length > 0)
				sb.Length -= 1;
			return sb.ToString();
		}

		// Determine if "type" inherits from "baseType", ignoring constructed types and interfaces, dealing
		// only with original types.
		public static bool InheritsFromOrEquals(this ITypeSymbol type, ITypeSymbol baseType)
		{
			return type.GetBaseTypesAndThis().Any(t => t.GetFullName() == baseType.GetFullName());
		}

		// Determine if "type" inherits from "baseType", ignoring constructed types and interfaces, dealing
		// only with original types.
		public static bool InheritsFromOrEqualsAny(this ITypeSymbol type, IEnumerable<ITypeSymbol> baseType)
		{
			return type.GetBaseTypesAndThis().Select(t => t.GetFullName())
			   .Intersect(baseType.Select(t => t.GetFullName())).Any();
		}

		[Pure]
		public static IEnumerable<ITypeSymbol> GetBaseTypesAndThis(this ITypeSymbol type)
		{
			var current = type;
			while (current != null)
			{
				yield return current;
				current = current.BaseType;
			}
		}

		public static bool IsCaught(this SemanticModel model, ITypeSymbol throwType, SyntaxNode thrownAt)
		{
			var potentialCatches = from t in thrownAt.Ancestors().OfType<TryStatementSyntax>()
								   from c in t.Catches
								   where Catches(model, c, throwType)
								   select c;

			return potentialCatches.Any();

		}

		public static bool Catches(SemanticModel model, CatchClauseSyntax @catch, ITypeSymbol throwType)
		{
			var catchType = model.GetTypeInfo(@catch.Declaration.Type).Type;

			if (!throwType.InheritsFromOrEquals(catchType))
				return false;

			if (@catch.Filter != null)
			{
				string varName = @catch.Declaration.Identifier.ToString();
				if (string.IsNullOrWhiteSpace(varName))
					// They don't capture the exception type. As such, there is no way
					// the filter can be a simple type filter and thus it will likely
					// be too complex to analyze. Assume it's possible to leak the exception.
					return false;

				var result= @catch.Filter.FilterExpression.Accept(new FilterExpressionMatchesTypeVisitor(model, varName, throwType));
				return result;
			}

			return true;
		}

	}
}
