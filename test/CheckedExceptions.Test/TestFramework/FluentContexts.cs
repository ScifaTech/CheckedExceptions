using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Scifa.CheckedExceptions.Test.TestFramework
{
	interface INamespaceContext
	{
		IDictionary<string, IClassContext> Classes { get; }
		IList<INamespaceContext> Namespaces { get; }
	}

	interface ISourceContext : INamespaceContext
	{
		IList<IAttributeContext> AssemblyAttributes { get; }
		IList<string> Usings { get; }
	}

	interface IAttributeContext { }

	interface IClassContext
	{
		IList<IMethodContext> Methods { get; }
	}

	interface IBlockContext
	{
		IList<IExpressionContext> Expressions { get; }
	}
	interface IExpressionContext
	{
		Expression Expression { get; }
		CodeLocation LocationToken { get; }
	}

	interface IBlockOrExpressionContext : IBlockContext, IExpressionContext
	{
	}
	interface IMethodContext : IBlockContext { }
	interface IClassOrMethodContext : IClassContext, IBlockContext { }
	interface IClassOrMethodOrExpressionContext : IClassOrMethodContext, IExpressionContext { }
}
