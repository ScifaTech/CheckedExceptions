using System;

namespace Scifa.CheckedExceptions.Attributes
{
	[System.AttributeUsage(AttributeTargets.Method|AttributeTargets.Constructor|AttributeTargets.Delegate, Inherited = false, AllowMultiple = true)]
	public sealed class ThrowsAttribute : Attribute
	{
		public ThrowsAttribute(Type exceptionType) { }
		public ThrowsAttribute(Type exceptionType, string condition) { }
	}
}
