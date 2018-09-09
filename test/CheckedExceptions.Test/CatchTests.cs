using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace Scifa.CheckedExceptions.Test
{
    [TestClass]
    public class CatchTests : TestBase
    {
        [TestMethod]
        public void Catch_exact_exception_causes_no_error()
        {
            string method = @"
				public void MyMethod(){
                    try{
					    throw new ArgumentException();
                    }catch(ArgumentException){
                        // ...
                    }
				}
			";

            var source = BuildClass(method);
            VerifyCSharpDiagnostic(source.FullText, NoDiagnosticResults);
        }

        [TestMethod]
        public void Catch_unrelated_exception_does_not_prevent_error()
        {
            string method = @"
				public void MyMethod(){
                    try {
					    throw new ArgumentException();
                    } catch(System.IO.IOException) {
                        // ... 
                    }
				}
			";

            var source = BuildClass(method);
            VerifyCSharpDiagnostic(source.FullText, new DiagnosticResult
            {
                Id = CheckedExceptionsAnalyzer.DiagnosticId,
                Locations = new[] { source.GetDiagnosticLocation(method: 0, line: 3) },
                Message = $"The method 'MyMethod' does not allow throwing 'ArgumentException'. This must be declared or caught.",
                Severity = DiagnosticSeverity.Error
            });
        }
        [TestMethod]
        public void Catch_unrelated_exception_does_not_cause_error()
        {
            string method = @"
				public void MyMethod(){
                    try {
                        // No-op
                    } catch(System.IO.IOException) {
                        // ... 
                    }
				}
			";

            var source = BuildClass(method);
            VerifyCSharpDiagnostic(source.FullText, NoDiagnosticResults);
        }


        [TestMethod]
        public void Irrelevant_type_filter_does_not_suppress_error()
        {
            string method = @"
				public void MyMethod(){
                    try {
					    throw new ArgumentException();
                    } catch(Exception) when (""a"" is string) {
                        // ... 
                    }
				}
			";

            var source = BuildClass(method);
            VerifyCSharpDiagnostic(source.FullText, new DiagnosticResult
            {
                Id = CheckedExceptionsAnalyzer.DiagnosticId,
                Locations = new[] { source.GetDiagnosticLocation(method: 0, line: 3) },
                Message = $"The method 'MyMethod' does not allow throwing 'ArgumentException'. This must be declared or caught.",
                Severity = DiagnosticSeverity.Error
            });
        }

        [TestMethod]
        public void Irrelevant_var_type_filter_does_not_suppress_error()
        {
            string method = @"
				public void MyMethod(bool a){
                    try {
					    throw new ArgumentException();
                    } catch(Exception) when (a is string) {
                        // ... 
                    }
				}
			";

            var source = BuildClass(method);
            VerifyCSharpDiagnostic(source.FullText, new DiagnosticResult
            {
                Id = CheckedExceptionsAnalyzer.DiagnosticId,
                Locations = new[] { source.GetDiagnosticLocation(method: 0, line: 3) },
                Message = $"The method 'MyMethod' does not allow throwing 'ArgumentException'. This must be declared or caught.",
                Severity = DiagnosticSeverity.Error
            });
        }

        [TestMethod]
        public void Irrelevant_filter_does_not_suppress_error()
        {
            string method = @"
				public void MyMethod(){
                    try {
					    throw new ArgumentException();
                    } catch(Exception) when (true && true) {
                        // ... 
                    }
				}
			";

            var source = BuildClass(method);
            VerifyCSharpDiagnostic(source.FullText, new DiagnosticResult
            {
                Id = CheckedExceptionsAnalyzer.DiagnosticId,
                Locations = new[] { source.GetDiagnosticLocation(method: 0, line: 3) },
                Message = $"The method 'MyMethod' does not allow throwing 'ArgumentException'. This must be declared or caught.",
                Severity = DiagnosticSeverity.Error
            });
        }


        [TestMethod]
        public void Matching_type_filter_suppresses_error()
        {
            string method = @"
				public void MyMethod(){
                    try {
					    throw new ArgumentException();
                    } catch(Exception ex) when (ex is ArgumentException) {
                        // ... 
                    }
				}
			";

            var source = BuildClass(method);
            VerifyCSharpDiagnostic(source.FullText, NoDiagnosticResults);
        }

        [TestMethod]
        public void Matching_OR_type_filter_suppresses_error()
        {
            string method = @"
				public void MyMethod(bool a){
                    try {
                        if(a){
					        throw new ArgumentException();
                        }else{
					        throw new InvalidOperationException();
                        }
                    } catch(Exception ex) when (ex is ArgumentException || ex is InvalidOperationException) {
                        // ... 
                    }
				}
			";

            var source = BuildClass(method);
            VerifyCSharpDiagnostic(source.FullText, NoDiagnosticResults);
        }

        [TestMethod]
        public void Matching_AND_type_filter_suppresses_error()
        {
            string method = @"
				public void MyMethod(bool a){
                    try {
					    throw new ArgumentException();
                    } catch(Exception ex) when (ex is ArgumentException && ex is Exception) {
                        // ... 
                    }
				}
			";

            var source = BuildClass(method);
            VerifyCSharpDiagnostic(source.FullText, NoDiagnosticResults);
        }

        [TestMethod]
        public void Matching_complex_type_filter_suppresses_error()
        {
            string method = @"
				public void MyMethod(bool a){
                    try {
                        if(a){
					        throw new ArgumentException();
                        }else{
					        throw new InvalidOperationException();
                        }
                    } catch(Exception ex) when (ex is Exception && (ex is ArgumentException || ex is InvalidOperationException) {
                        // ... 
                    }
				}
			";

            var source = BuildClass(method);
            VerifyCSharpDiagnostic(source.FullText, NoDiagnosticResults);
        }
    }
}