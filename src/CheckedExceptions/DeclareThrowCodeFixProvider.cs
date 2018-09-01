using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;
using System.Diagnostics.Contracts;

namespace Scifa.CheckedExceptions
{
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DeclareThrowCodeFixProvider)), Shared]
	public class DeclareThrowCodeFixProvider : CodeFixProvider
	{
		private const string title = "Declare throw";
		private const string ThrowsAttributeName = nameof(Attributes.ThrowsAttribute);

		private static readonly string ThrowsAttributeFullName = typeof(Attributes.ThrowsAttribute).FullName;
		private static readonly string ThrowsAttributeNamespace = ThrowsAttributeFullName.Substring(0, ThrowsAttributeFullName.Length - nameof(Attributes.ThrowsAttribute).Length);
		private static readonly string ThrowsAttributeShortName = ThrowsAttributeName.Substring(0, ThrowsAttributeName.Length - "Attribute".Length);

		public sealed override ImmutableArray<string> FixableDiagnosticIds
		{
			get { return ImmutableArray.Create(CheckedExceptionsAnalyzer.DiagnosticId); }
		}

		public sealed override FixAllProvider GetFixAllProvider()
		{
			// See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
			return WellKnownFixAllProviders.BatchFixer;
		}

		public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

			// TODO: Replace the following code with your own analysis, generating a CodeAction for each fix to suggest
			var diagnostic = context.Diagnostics.First();
			var diagnosticSpan = diagnostic.Location.SourceSpan;

			SyntaxToken syntax = root.FindToken(diagnosticSpan.Start);
			var methodDeclaration = syntax.Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();

			// Register a code action that will invoke the fix.
			context.RegisterCodeFix(
				CodeAction.Create(
					title: title,
					createChangedSolution: c => AddThrowsAttributeAsync(context.Document, methodDeclaration, syntax, c),
					equivalenceKey: title),
				diagnostic);
		}

		[Pure]
		private async Task<Solution> AddThrowsAttributeAsync(Document document, MethodDeclarationSyntax methodDecl, SyntaxToken diagnosticSpan, CancellationToken cancellationToken)
		{
			var semModel = await document.GetSemanticModelAsync();
			var syntaxRoot = await document.GetSyntaxRootAsync();
			var exceptionType = diagnosticSpan
				.Parent
				.AncestorsAndSelf()
				.Select(x => semModel.GetTypeInfo(x))
				.FirstOrDefault(x => x.Type != null);

			ITypeSymbol exceptionSymbol = exceptionType.Type;

			bool hasUsing = syntaxRoot.DescendantNodes().OfType<UsingDirectiveSyntax>()
				.Any(u => u.Name.ToString() == ThrowsAttributeNamespace);

			var newMethodDecl = methodDecl.AddAttributeLists(
				SyntaxFactory.AttributeList(
				   SyntaxFactory.SingletonSeparatedList(
					   CreateThrowsAttribute(await document.GetSemanticModelAsync(), methodDecl, exceptionSymbol, !hasUsing)
				   )
			   )
			).WithoutLeadingTrivia();

			return document.WithSyntaxRoot(
				syntaxRoot.ReplaceNode(methodDecl, newMethodDecl)
			).Project.Solution;
		}

		private static AttributeSyntax CreateThrowsAttribute(SemanticModel semanticModel, MethodDeclarationSyntax methodDecl, ITypeSymbol exceptionSymbol, bool useFullName)
		{
			return SyntaxFactory.Attribute(
				SyntaxFactory.IdentifierName(
					(useFullName ? ThrowsAttributeNamespace : string.Empty)
					+ ThrowsAttributeShortName
				)
			).WithArgumentList(
				SyntaxFactory.AttributeArgumentList(
					SyntaxFactory.SingletonSeparatedList(
						SyntaxFactory.AttributeArgument(
							SyntaxFactory.TypeOfExpression(
								SyntaxFactory.ParseTypeName(
									exceptionSymbol.ToMinimalDisplayString(semanticModel, methodDecl.GetLocation().SourceSpan.Start)
								)
							)
						)
					)
				).WithoutLeadingTrivia()
			);
		}
	}
}
