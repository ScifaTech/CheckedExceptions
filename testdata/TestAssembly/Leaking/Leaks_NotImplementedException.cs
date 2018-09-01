using System;
using Scifa.CheckedExceptions.Attributes;

namespace TestAssembly
{
	public partial class Class1
	{
		public void Leaks_NotImplementedException()
		{
			throw new NotImplementedException();
		}
	}
}
