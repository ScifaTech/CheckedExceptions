using System;
using System.Collections.Generic;
using System.Text;

namespace Scifa.CheckedExceptions.Test.TestFramework
{
	internal class NamespaceContext
	{
		public IDictionary<string, IClassContext> Classes { get; } = new Dictionary<string, IClassContext>();
		public IList<INamespaceContext> Namespaces { get; } = new List<INamespaceContext>();
	}

	internal class SourceContext : NamespaceContext, ISourceContext
	{
		public IList<IAttributeContext> AssemblyAttributes { get; } = new List<IAttributeContext>();
	}
}
