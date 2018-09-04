using System;
using Scifa.CheckedExceptions.Attributes;

namespace TestAssembly
{
    public partial class ThrowsIndirect
    {
        [Throws(typeof(Exception))]
        public void Declares_ParentException()
        {
            ThrowNotImplementedException();
        }
    }
}
