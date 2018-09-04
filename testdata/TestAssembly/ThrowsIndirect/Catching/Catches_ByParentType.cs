using System;
using Scifa.CheckedExceptions.Attributes;

namespace TestAssembly
{
	public partial class ThrowsIndirect
	{
		public void Catches_ByParentType()
		{
			try
			{
				ThrowNotImplementedException();
			}
			catch (Exception)
			{
				// No action
			}
		}
	}
}
