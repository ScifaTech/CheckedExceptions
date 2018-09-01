using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Scifa.CheckedExceptions
{
	internal class FilterExpressionMatchesTypeVisitor : CSharpSyntaxVisitor<bool>
	{
		private readonly SemanticModel model;
		private readonly string variableName;
		private readonly ITypeSymbol throwType;

		public FilterExpressionMatchesTypeVisitor(SemanticModel model, string variableName, ITypeSymbol throwType)
		{
			this.model = model;
			this.variableName = variableName;
			this.throwType = throwType;
		}

		public override bool VisitBinaryExpression(BinaryExpressionSyntax node)
		{
			bool left, right;
			bool result = false;
			switch (node.Kind())
			{
				case SyntaxKind.LogicalOrExpression:
					left = node.Left.Accept(this);
					right = node.Right.Accept(this);
					result = left || right;
					break;
				case SyntaxKind.LogicalAndExpression:
					left = node.Left.Accept(this);
					right = node.Right.Accept(this);
					result = left && right;
					break;
				case SyntaxKind.IsExpression when IsTypeTestExpression(node):
					result = throwType.InheritsFromOrEquals(model.GetTypeInfo(node.Right).Type);
					break;
			}
			return result;
		}

		private bool IsTypeTestExpression(BinaryExpressionSyntax node)
			=> node.Left is IdentifierNameSyntax ins && ins.Identifier.ToString() == variableName;

		public override bool VisitParenthesizedExpression(ParenthesizedExpressionSyntax node)
		{
			return node.Expression.Accept(this);
		}

		public override bool DefaultVisit(SyntaxNode node)
		{
			return base.DefaultVisit(node);
		}
	}
}