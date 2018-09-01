using System;
using Scifa.CheckedExceptions.Attributes;

namespace TestAssembly
{
	public partial class Class1
	{
		public void Catches_ByParentTypeAndMultipleTypeFilter()
		{
			try
			{ 
				throw new NotImplementedException();
			}
			catch (Exception ex) when (ex is Exception && (ex is NotSupportedException || ex is NotImplementedException))
			{
				// No action
			}
		}
	}
}
