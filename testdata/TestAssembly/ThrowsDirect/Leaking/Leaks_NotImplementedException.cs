using System;
using Scifa.CheckedExceptions.Attributes;

namespace TestAssembly
{
	public partial class ThrowsDirect
	{
		[Throws(typeof(NotImplementedException))]
		public void Leaks_NotImplementedException()
		{
			throw new NotImplementedException();
		}
	}
}
