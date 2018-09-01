using System.Collections.Generic;

namespace Scifa.CheckedExceptions.Test.TestFramework
{
	internal class ClassOrMethodContext : IClassOrMethodContext, IClassContext, IMethodContext
	{
		private readonly IClassContext @class;
		private readonly IMethodContext method;

		public ClassOrMethodContext(IClassContext @class, IMethodContext method)
		{
			this.@class = @class;
			this.method = method;
		}

		public IList<IMethodContext> Methods => @class.Methods;
	}
}