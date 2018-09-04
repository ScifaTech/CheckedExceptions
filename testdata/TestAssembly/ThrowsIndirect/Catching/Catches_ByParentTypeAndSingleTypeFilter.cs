using System;
using Scifa.CheckedExceptions.Attributes;

namespace TestAssembly
{
	public partial class ThrowsIndirect
	{
		public void Catches_ByParentTypeAndSingleTypeFilter()
		{
			try
			{
				ThrowNotImplementedException();
			}
			catch (Exception ex) when (ex is NotImplementedException)
			{
				// No action
			}
		}
	}
}
