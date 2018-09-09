using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using TestHelper;

namespace Scifa.CheckedExceptions.Test
{
	internal sealed class SourceCode
	{
		private static readonly ImmutableArray<string> EmptyStringArray = ImmutableArray.Create<string>();

		public static readonly SourceCode Empty = new SourceCode();

		private readonly int[] methodStarts;
		private readonly ImmutableArray<string> usings;
		private readonly ImmutableArray<string> assemblyAttributes;
		private readonly string @namespace;
		private readonly string className;
		private readonly ImmutableArray<string> methods;
		private string fullText;

		private SourceCode(ImmutableArray<string> usings, ImmutableArray<string> assemblyAttributes, string @namespace, string className, ImmutableArray<string> methods)
		{
			this.usings = usings;
			this.assemblyAttributes = assemblyAttributes;
			this.@namespace = @namespace;
			this.className = className;
			this.methods = methods;
			methodStarts = new int[methods.IsDefaultOrEmpty ? 0 : methods.Length];
		}

		public SourceCode() : this(EmptyStringArray, EmptyStringArray, default, default, EmptyStringArray) { }

		private string CreateFullText()
		{
			var sb = new StringBuilder();
			int line = 0, tabs = 0;
			foreach (var u in usings)
			{
				sb.Append("using ").Append(u).Append(";").AppendLine();
				line++;
			}

			sb.AppendLine();
			line++;

			foreach (var a in assemblyAttributes)
			{
				sb.Append("[assembly: ").Append(a).Append("]").AppendLine();
				line++;
			}

			if (@namespace != null)
			{
				sb.Append("namespace ").AppendLine(@namespace).AppendLine(" {");
				line += 2;
				tabs++;
			}

			sb.Append('\t', tabs).Append("public class ").AppendLine(className)
				.Append('\t', tabs).AppendLine("{");
			line += 2;
			tabs++;

			for (int i = 0; i < methods.Length; i++)
			{
				methodStarts[i] = line+1;
				sb.AppendLine(methods[i]).AppendLine();
				line += methods[i].LineCount() + 2;
			}

			tabs--;
			sb.Append('\t', tabs).AppendLine("}");
			line++;

			if (@namespace != null)
			{
				tabs--;
				sb.AppendLine("}");
				line++;
			}

			return sb.ToString();
		}

		public string FullText => fullText ?? (fullText = CreateFullText());

		public DiagnosticResultLocation GetDiagnosticLocation(int method, int line, int @char = 0)
		{
			return new DiagnosticResultLocation(
				"Test0.cs",
				line + methodStarts[method],
				1 + @char + methods[method].Split('\n')[line].TakeWhile(char.IsWhiteSpace).Count()
			);
		}

		public SourceCode WithUsing(string ns) => CopyWith(usings: usings.Add(ns));
		public SourceCode WithoutUsing(string ns) => CopyWith(usings: usings.Remove(ns));

		public SourceCode WithAssemblyAttribute(string attr) => CopyWith(assemblyAttributes: assemblyAttributes.Add(attr));
		public SourceCode WithoutAssemblyAttribute(string attr) => CopyWith(assemblyAttributes: assemblyAttributes.Remove(attr));

		public SourceCode WithNamespace(string ns) => CopyWith(@namespace: ns);

		public SourceCode WithClassName(string name) => CopyWith(className: name);

		public SourceCode WithMethod(string methodText) => CopyWith(methods: methods.Add(methodText));
		public SourceCode WithoutMethod(string methodText) => CopyWith(methods: methods.Remove(methodText));

		public SourceCode WithMethods(params string[] methodText) => CopyWith(methods: methods.AddRange(methodText));
		public SourceCode WithoutMethods(params string[] methodText) => CopyWith(methods: methods.RemoveRange(methodText));

		private SourceCode CopyWith(
			ImmutableArray<string>? usings = null,
			ImmutableArray<string>? assemblyAttributes = null,
			string @namespace = null,
			string className = null,
			ImmutableArray<string>? methods = null)
		{
			return new SourceCode(
				usings ?? this.usings,
				assemblyAttributes ?? this.assemblyAttributes,
				@namespace ?? this.@namespace,
				className ?? this.className,
				methods ?? this.methods
			);
		}
	}
}
