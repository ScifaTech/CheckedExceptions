using System;

namespace Scifa.CheckedExceptions.Attributes
{
	[System.AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = true)]
	public sealed class CheckExceptionsAttribute : Attribute
	{
		public string NamespaceMask { get; set; } = "*";
		public CheckExceptionsAttribute() { }
	}
}
