//using Microsoft.CodeAnalysis;
//using Microsoft.CodeAnalysis.CSharp;
//using Scifa.CheckedExceptions.Attributes;

//namespace Scifa.CheckedExceptions.Test
//{
//	public class AnalyzerTestUtils
//	{

//		private object GetDiagnostics(string fullSource)
//		{
//			CSharpParseOptions parseOptions = new CSharpParseOptions()
//			  .WithKind(SourceCodeKind.Regular)
//			  .WithLanguageVersion(LanguageVersion.Latest);

//			CSharpCompilationOptions compileOptions =
//			  new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
//			  .WithOptimizationLevel(OptimizationLevel.Release);

//			CSharpCompilation compilation =
//			  CSharpCompilation.Create("TestInMemoryAssembly") // ..with some fake dll name
//			  .WithOptions(compileOptions)
//			  .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
//			  .AddReferences(MetadataReference.CreateFromFile(typeof(CheckExceptionsAttribute).Assembly.Location);

//			compilation.Add

//		}
//	}
//}