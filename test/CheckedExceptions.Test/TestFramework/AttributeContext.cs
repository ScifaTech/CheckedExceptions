using System;

namespace Scifa.CheckedExceptions.Test.TestFramework
{
	internal class AttributeContext: IAttributeContext
	{
		private Type attributeType;
		private string[] args;

		public AttributeContext(Type attributeType, string[] args)
		{
			this.attributeType = attributeType;
			this.args = args;
		}
	}
}