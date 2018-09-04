using System;
using Scifa.CheckedExceptions.Attributes;

namespace TestAssembly
{
	public partial class ThrowsDirect
	{
		public void Leaks_NotImplementedException()
		{
			throw new NotImplementedException();
		}
	}
}
