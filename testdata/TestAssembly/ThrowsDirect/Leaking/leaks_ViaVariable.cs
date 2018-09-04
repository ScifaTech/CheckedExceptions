using System;

namespace TestAssembly
{
	public partial class ThrowsDirect
	{
		public void Leaks_ViaVariable()
		{
			var ex = new NotImplementedException();
			throw ex;
		}
	}
}
