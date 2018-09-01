using System;
using System.Collections.Generic;
using System.Text;

namespace Scifa.CheckedExceptions.Test.TestFramework
{
	static class CodeGenerator
	{
		public static string AsSource(this ISourceContext source)
		{
			StringBuilder result = new StringBuilder();
			foreach (var @namespace in source.Usings)
			{
				result
					.Append("using ")
					.Append(@namespace)
					.AppendLine(";");
			}

			foreach (var attribute in source.AssemblyAttributes)
			{
				result
					.Append("[assembly: ")
					.Append(attribute.AsSource())
					.AppendLine("]");
			}

			foreach (var @namespace in source.Namespaces)
			{
				result.AppendLine(@namespace.AsSource());
			}
			return result.ToString();
		}
		public static string AsSource(this INamespaceContext source, StringBuilder result)
		{
			result.Append("namespace ")
				.Append(source.Name);

		}
	}
}
