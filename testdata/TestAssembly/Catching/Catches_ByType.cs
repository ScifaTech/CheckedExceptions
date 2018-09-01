﻿using System;
using Scifa.CheckedExceptions.Attributes;

namespace TestAssembly
{
	public partial class Class1
	{
		public void Catches_NotImplementedException()
		{
			try
			{ 
				throw new NotImplementedException();
			}
			catch (NotImplementedException)
			{
				// No action
			}
		}
	}
}
