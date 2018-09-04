using System;
using Scifa.CheckedExceptions.Attributes;

namespace TestAssembly
{
	public partial class ThrowsIndirect
	{
		public void Catches_NotImplementedException()
		{
			try
			{ 
				ThrowNotImplementedException();
			}
			catch (NotImplementedException)
			{
				// No action
			}
		}
	}
}
