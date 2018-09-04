using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scifa.CheckedExceptions.Attributes;
using TestAssembly;
using TestHelper;

namespace Scifa.CheckedExceptions.Test
{
	[TestClass]
	public class UnitTest : CodeFixVerifier
	{
		[TestMethod]
		public void Empty_class_causes_no_diagnostics()
		{
			var (fullSource, _) = BuildClass();
			VerifyCSharpDiagnostic(fullSource /* no diagnostics */);
		}

		[TestMethod]
		public void Undeclared_throw_causes_error()
		{
			string method = @"
				public void MyMethod(){
					throw new NotImplementedException();
				}
			";

			var (fullSource, codeLocationProvider) = BuildClass(method);
			VerifyCSharpDiagnostic(fullSource,
				new DiagnosticResult
				{
					Id = CheckedExceptionsAnalyzer.DiagnosticId,
					Locations = new[] { codeLocationProvider.GetDiagnosticLocation(method: 0, line: 3, @char: 0) },
					Message = $"The method 'MyMethod' does not allow throwing 'NotImplementedException'. This must be declared or caught.",
					Severity = DiagnosticSeverity.Error
				}
			);
		}

		private (string, ICodeLocator) BuildClass(params string[] methods)
		{
			string header = @"
using System;
using Scifa.CheckedExceptions.Attributes;

namespace MyNamespace {
	public class MyClass {
";
			string footer = @"
	}
}
";
			int nextLine = header.LineCount();
			var sb = new StringBuilder(header);
			int[] methodStarts = new int[methods.Length];

			for (int i = 0; i < methods.Length; i++)
			{
				methodStarts[i] = nextLine;
				sb.Append(methods[i]).AppendLine();
				nextLine += methods[i].LineCount() + 1;
			}

			return (sb.ToString(), new CodeLocator(methods, methodStarts));
		}

		protected override IEnumerable<MetadataReference> GetAdditionalReferences()
		{
			yield return MetadataReference.CreateFromFile(typeof(CheckExceptionsAttribute).Assembly.Location);
		}

		protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
		{
			return new CheckedExceptionsAnalyzer();
		}

	}

	internal class CodeLocator : ICodeLocator
	{
		private readonly string[] methods;
		private readonly int[] methodStarts;

		public CodeLocator(string[] methods, int[] methodStarts)
		{
			this.methods = methods;
			this.methodStarts = methodStarts;
		}

		public DiagnosticResultLocation GetDiagnosticLocation(int method, int line, int @char)
		{
			return new DiagnosticResultLocation(
				"Test0.cs",
				line + methodStarts[method],
				2 + @char + methods[method].Split('\n')[line].TakeWhile(char.IsWhiteSpace).Count()
			);
		}
	}

	public static class StringHelpers
	{
		public static int LineCount(this string s)
		{
			return CharCount(s, '\n');
		}

		private static int CharCount(this string s, char v)
		{
			int result = 0;
			for (int i = 0; i < s.Length; i++)
				if (s[i] == v) result++;

			return result;
		}
	}
}
