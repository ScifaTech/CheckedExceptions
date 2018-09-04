using System;
using Scifa.CheckedExceptions.Attributes;

namespace TestAssembly
{
    public partial class ThrowsIndirect
    {
        public void Leaks_NotImplementedException()
        {
            ThrowNotImplementedException();
        }
    }
}
