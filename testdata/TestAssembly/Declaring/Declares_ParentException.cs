using System;
using Scifa.CheckedExceptions.Attributes;

namespace TestAssembly
{
	public partial class Class1
	{
		[Throws(typeof(Exception))]
		public void Declares_ParentException()
		{ 
			throw new NotImplementedException();
		}
	}
}
