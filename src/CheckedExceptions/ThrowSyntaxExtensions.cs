using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Scifa.CheckedExceptions
{
	static class ThrowSyntaxExtensions
	{
		public static bool IsDeclared(this ThrowStatementSyntax @throw, SemanticModel semModel)
		{
			var throwType = semModel.GetTypeInfo(@throw.Expression).Type;

			var method = @throw.Ancestors().OfType<MethodDeclarationSyntax>().First();

			var throwAttributes = from m in method.AttributeLists
								  from attrSyntax in m.Attributes
								  where attrSyntax.IsThrowsAttribute(semModel)
								  select attrSyntax;

			var declaredTypes = from a in throwAttributes
								from t in a.ArgumentList.Arguments
								let expr = t.Expression
								where expr is TypeOfExpressionSyntax
								select semModel.GetTypeInfo(t.Expression);

			return declaredTypes.Any(t => t.Type == throwType);
		}

		public static bool IsThrowsAttribute(this AttributeSyntax attrSyntax, SemanticModel semModel)
			=> KnownTypes.CheckExceptionsAttributesFullName == semModel.GetTypeInfo(attrSyntax).Type.GetFullName();

	}
}
