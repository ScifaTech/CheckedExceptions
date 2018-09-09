using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scifa.CheckedExceptions.Attributes;
using TestAssembly;
using TestHelper;

namespace Scifa.CheckedExceptions.Test
{
	[TestClass]
	public class DirectThrow : CodeFixVerifier
	{
		private static readonly DiagnosticResult[] NoDiagnosticResults = new DiagnosticResult[0];

		[TestMethod]
		public void Empty_class_causes_no_diagnostics()
		{
			var source = BuildClass();
			VerifyCSharpDiagnostic(source.FullText, NoDiagnosticResults);
		}

		[TestMethod]
		public void Undeclared_throw_causes_error()
		{
			string method = @"
				public void MyMethod(){
					throw new NotImplementedException();
				}
			";

			var source = BuildClass(method);
			VerifyCSharpDiagnostic(source.FullText,
				new DiagnosticResult
				{
					Id = CheckedExceptionsAnalyzer.DiagnosticId,
					Locations = new[] { source.GetDiagnosticLocation(method: 0, line: 3, @char: 0) },
					Message = $"The method 'MyMethod' does not allow throwing 'NotImplementedException'. This must be declared or caught.",
					Severity = DiagnosticSeverity.Error
				});
		}

		[TestMethod]
		public void Undeclared_throw_causes_no_error_when_not_checking()
		{
			string method = @"
				public void MyMethod(){
					throw new NotImplementedException();
				}
			";

			var source = BuildClass(method).WithoutAssemblyAttribute("CheckExceptions");
			VerifyCSharpDiagnostic(source.FullText, NoDiagnosticResults);
		}

		[TestMethod]
		public void Declared_throw_causes_no_error()
		{
			string method = @"
			[Throws(typeof(NotImplementedException))]
			public void Declares_NotImplementedException()
			{ 
				throw new NotImplementedException();
			}
			";

			var source = BuildClass(method);
			VerifyCSharpDiagnostic(source.FullText, NoDiagnosticResults);
		}

		private SourceCode BuildClass(params string[] methods)
		{
			return SourceCode.Empty
				.WithUsing("System")
				.WithUsing("Scifa.CheckedExceptions.Attributes")
				.WithAssemblyAttribute("CheckExceptions")
				.WithNamespace("Scifa.CheckedExceptions.TestSnippets")
				.WithClassName("Class1")
				.WithMethods(methods);
		}

		protected override IEnumerable<MetadataReference> GetAdditionalReferences()
		{// CONTENT from: https://github.com/dotnet/corefx/issues/11601#issuecomment-336829147

			// first, collect all assemblies
			var assemblies = new HashSet<Assembly>();

			IncludeAll(Assembly.Load(new AssemblyName("netstandard")));

			//// add extra assemblies which are not part of netstandard.dll, for example:
			IncludeAll(typeof(CheckExceptionsAttribute).Assembly);

			// second, build metadata references for these assemblies
			var result = new List<MetadataReference>(assemblies.Count);
			foreach (var assembly in assemblies)
			{
				result.Add(MetadataReference.CreateFromFile(assembly.Location));
			}

			return result;

			// helper local function - add assembly and its referenced assemblies
			void IncludeAll(Assembly assembly)
			{
				if (!assemblies.Add(assembly))
				{
					// already added
					return;
				}

				var referencedAssemblyNames = assembly.GetReferencedAssemblies();

				foreach (var assemblyName in referencedAssemblyNames)
				{
					var loadedAssembly = Assembly.Load(assemblyName);
					assemblies.Add(loadedAssembly);
				}
			}
		}
		protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
		{
			return new CheckedExceptionsAnalyzer();
		}

	}
}
