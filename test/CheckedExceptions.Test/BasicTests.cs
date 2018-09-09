using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace Scifa.CheckedExceptions.Test
{
	[TestClass]
	public class DirectThrow : TestBase
	{

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
					throw new ArgumentException();
				}
			";

			var source = BuildClass(method);
			VerifyCSharpDiagnostic(source.FullText,
				new DiagnosticResult
				{
					Id = CheckedExceptionsAnalyzer.DiagnosticId,
					Locations = new[] { source.GetDiagnosticLocation(method: 0, line: 2, @char: 0) },
					Message = $"The method 'MyMethod' does not allow throwing 'ArgumentException'. This must be declared or caught.",
					Severity = DiagnosticSeverity.Error
				});
		}

		[TestMethod]
		public void Undeclared_throw_causes_no_error_when_not_checking()
		{
			string method = @"
				public void MyMethod(){
					throw new ArgumentException();
				}
			";

			var source = BuildClass(method).WithoutAssemblyAttribute("CheckExceptions");
			VerifyCSharpDiagnostic(source.FullText, NoDiagnosticResults);
		}

		[TestMethod]
		public void Declared_throw_causes_no_error()
		{
			string method = @"
			[Throws(typeof(ArgumentException))]
			public void Declares_ArgumentException()
			{ 
				throw new ArgumentException();
			}
			";

			var source = BuildClass(method);
			VerifyCSharpDiagnostic(source.FullText, NoDiagnosticResults);
		}
	}
}
