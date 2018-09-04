using System;
using Scifa.CheckedExceptions.Attributes;

namespace TestAssembly
{
	public partial class ThrowsIndirect
	{
		public void Catches_ByParentTypeAndMultipleTypeFilter()
		{
			try
			{ 
				ThrowNotImplementedException();
			}
			catch (Exception ex) when (ex is Exception && (ex is NotSupportedException || ex is NotImplementedException))
			{
				// No action
			}
		}
	}
}
