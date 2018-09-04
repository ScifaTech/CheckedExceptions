using System;
using Scifa.CheckedExceptions.Attributes;

namespace TestAssembly
{
	public partial class ThrowsDirect
	{
		public void Catches_ByParentType()
		{
			try
			{
				throw new NotImplementedException();
			}
			catch (Exception)
			{
				// No action
			}
		}
	}
}
