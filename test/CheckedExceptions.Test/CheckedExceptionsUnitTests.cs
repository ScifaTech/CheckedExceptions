using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestHelper;
using Scifa.CheckedExceptions;
using System.Linq;
using System.Collections.Generic;
using Scifa.CheckedExceptions.Attributes;
using System.Reflection;

namespace Scifa.CheckedExceptions.Test
{
	[TestClass]
	public class UnitTest : CodeFixVerifier
	{
		//No diagnostics expected to show up
		[TestMethod]
		public void EmptySource_NoDiagnostics()
		{
			var test = @"";

			VerifyCSharpDiagnostic(test);
		}

		//Diagnostic and CodeFix both triggered and checked for
		[TestMethod]
		public void Simple_throw_Needs_Attribute()
		{
			var test = @"
using System;
    
[assembly: Scifa.CheckedExceptions.Attributes.CheckExceptions]

namespace ConsoleApplication1
{
    class TypeName
    {   
        public void DoWork(){
            throw new InvalidOperationException();
        }
    }
}";
			var expected = new DiagnosticResult
			{
				Id = "CheckedExceptions",
				Message = String.Format(Resources.AnalyzerMessageFormat, "DoWork", "InvalidOperationException", 15),
				Severity = DiagnosticSeverity.Error,
				Locations =
					new[] {
							new DiagnosticResultLocation("Test0.cs", 11, 19)
						}
			};

			VerifyCSharpDiagnostic(test, expected);

			var fixtest = @"
using System;
    
[assembly: Scifa.CheckedExceptions.Attributes.CheckExceptions]

namespace ConsoleApplication1
{
    class TypeName
    {
        [Scifa.CheckedExceptions.Attributes.Throws(typeof(InvalidOperationException))]
        public void DoWork(){
            throw new InvalidOperationException();
        }
    }
}";
			// newCompilerDiagnostics is broken - it reports a new error for identical code
			VerifyCSharpFix(test, fixtest, allowNewCompilerDiagnostics: true);
		}

		protected override IEnumerable<MetadataReference> GetAdditionalReferences()
		{
			return new[] {
				AssemblyReferenceFromType(typeof(CheckExceptionsAttribute)),
				MetadataReference.CreateFromFile(Assembly.LoadWithPartialName("System.Runtime").Location),
			};
		}

		protected override CodeFixProvider GetCSharpCodeFixProvider()
		{
			return new DeclareThrowCodeFixProvider();
		}

		protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
		{
			return new CheckedExceptionsAnalyzer();
		}
	}
}
