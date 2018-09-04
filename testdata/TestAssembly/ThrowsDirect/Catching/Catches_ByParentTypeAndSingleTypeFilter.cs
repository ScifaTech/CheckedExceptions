using System;
using Scifa.CheckedExceptions.Attributes;

namespace TestAssembly
{
	public partial class ThrowsDirect
	{
		public void Catches_ByParentTypeAndSingleTypeFilter()
		{
			try
			{
				throw new NotImplementedException();
			}
			catch (Exception ex) when (ex is NotImplementedException)
			{
				// No action
			}
		}
	}
}
