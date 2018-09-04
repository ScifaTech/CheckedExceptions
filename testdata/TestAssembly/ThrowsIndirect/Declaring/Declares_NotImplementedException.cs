using System;
using Scifa.CheckedExceptions.Attributes;

namespace TestAssembly
{
	public partial class ThrowsIndirect
	{
		[Throws(typeof(NotImplementedException))]
		public void Declares_NotImplementedException()
		{ 
				ThrowNotImplementedException();
		}
	}
}
