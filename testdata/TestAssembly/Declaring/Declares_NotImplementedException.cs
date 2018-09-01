using System;
using Scifa.CheckedExceptions.Attributes;

namespace TestAssembly
{
	public partial class Class1
	{
		[Throws(typeof(NotImplementedException))]
		public void Declares_NotImplementedException()
		{ 
			throw new NotImplementedException();
		}
	}
}
