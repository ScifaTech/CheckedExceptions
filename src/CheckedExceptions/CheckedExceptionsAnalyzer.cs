using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Scifa.CheckedExceptions.Attributes;

namespace Scifa.CheckedExceptions
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class CheckedExceptionsAnalyzer : DiagnosticAnalyzer
	{
		public const string DiagnosticId = "CheckedExceptions";
		public const string DiagnosticExceptionTypeString = "ExceptionTypeString";


		// You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
		// See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
		private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
		private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
		private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
		private const string Category = "Exceptions";

		private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description);

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

		public override void Initialize(AnalysisContext context)
		{
			context.RegisterCompilationStartAction(CompilationStart);
		}

		private void CompilationStart(CompilationStartAnalysisContext context)
		{
			List<string> NamespaceMasks = new List<string>();

			var checkExceptionAttributes = from attr in context.Compilation.Assembly.GetAttributes()
										   let c = attr.AttributeClass
										   where KnownTypes.CheckExceptionsAttributesFullName == c.GetFullName()
										   select attr;

			foreach (var attr in checkExceptionAttributes)
			{
				if (!attr.NamedArguments.Any())
				{
					NamespaceMasks.Clear();
					break;
				}

				NamespaceMasks.AddRange(
					from arg in attr.NamedArguments
					where nameof(CheckExceptionsAttribute.NamespaceMask) == arg.Key
					select (string)arg.Value.Value
				);
			}
			var analyzerConfig = new AnalyzerConfig();
			analyzerConfig.SetNamespaceMasks(NamespaceMasks);

			context.RegisterCodeBlockAction(blockContext => AnalyzeCodeBlock(blockContext, analyzerConfig));
		}

		private void AnalyzeCodeBlock(CodeBlockAnalysisContext blockContext, AnalyzerConfig analyzerConfig)
		{
			if (blockContext.OwningSymbol is IMethodSymbol methodSymbol
				&& analyzerConfig.ShouldAnalyze(methodSymbol.ContainingType))
			{
				AnalyzeMethodBlock(blockContext, methodSymbol, analyzerConfig);
			}
		}

		private void AnalyzeMethodBlock(CodeBlockAnalysisContext blockContext, IMethodSymbol methodSymbol, AnalyzerConfig analyzerConfig)
		{
			var declaredThrowTypes = GetDeclaredTypes(methodSymbol);
			var throwStatements = blockContext.CodeBlock.DescendantNodes().OfType<ThrowStatementSyntax>();

			var offendingThrows = from thrw in throwStatements
								  let exType = blockContext.SemanticModel.GetTypeInfo(thrw.Expression).Type
								  where !exType.InheritsFromOrEqualsAny(declaredThrowTypes)
								  select (expression: thrw, type: exType);

			var uncaughtThrows = from thrw in offendingThrows
								 where !blockContext.SemanticModel.IsCaught(thrw.type, thrw.expression)
								 select thrw;

			foreach (var thrw in uncaughtThrows)
			{
				var exceptionType = thrw.type;

				var diagnostic = Diagnostic.Create(
					Rule,
					thrw.expression.GetLocation(),
					methodSymbol.Name,
					exceptionType.Name
				);

				blockContext.ReportDiagnostic(diagnostic);
			}
		}

		private IImmutableSet<INamedTypeSymbol> GetDeclaredTypes(IMethodSymbol methodSymbol)
		{
			var types = from attr in methodSymbol.GetAttributes()
						where attr.AttributeClass.GetFullName() == KnownTypes.ThrowsAttributeFullName
						let declType = attr.ConstructorArguments[0].Value as INamedTypeSymbol
						where declType != null
						select declType;

			return ImmutableHashSet<INamedTypeSymbol>.Empty.Union(types);
		}
	}
}
