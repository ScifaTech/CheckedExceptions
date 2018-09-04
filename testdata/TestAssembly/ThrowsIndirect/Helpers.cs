using System;
using Scifa.CheckedExceptions.Attributes;

namespace TestAssembly
{
    public partial class ThrowsIndirect
    {
        [Throws(typeof(NotImplementedException))]
        private void ThrowNotImplementedException()
        {
            throw new NotImplementedException();
        }
    }
}