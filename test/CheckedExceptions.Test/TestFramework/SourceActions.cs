using System;
using System.Collections.Generic;
using System.Text;

namespace Scifa.CheckedExceptions.Test.TestFramework
{
	static class SourceActions
	{
		public static ISourceContext WithAssemblyAttribute(this ISourceContext source, Type attributeType, params string[] args)
		{
			source.AssemblyAttributes.Add(new AttributeContext(attributeType, args));

			return source;
		}

		public static IClassContext WithClass(this ISourceContext source, string className)
		{
			var @class = new ClassContext();
			source.Classes.Add(className, @class);

			return @class;
		}
		public static IClassContext WithClass(this ISourceContext source, string @namespace, string className)
		{
			var @class = new ClassContext();
			source.Classes.Add(className, @class);

			return @class;
		}
		public static IClassOrMethodContext WithMethod(this IClassContext @class, string methodName)
		{
			var method = new MethodContext();
			@class.Methods.Add(method);

			return new ClassOrMethodContext(@class, method);
		}
			}
}
