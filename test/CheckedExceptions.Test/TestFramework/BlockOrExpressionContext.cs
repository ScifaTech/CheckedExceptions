using System.Collections.Generic;
using System.Linq.Expressions;

namespace Scifa.CheckedExceptions.Test.TestFramework
{
	internal class BlockOrExpressionContext : IBlockOrExpressionContext, IBlockContext, IExpressionContext
	{
		private readonly IBlockContext block;
		private readonly IExpressionContext expressionContext;

		public IList<IExpressionContext> Expressions => block.Expressions;

		public Expression Expression => expressionContext.Expression;

		public BlockOrExpressionContext(IBlockContext block, IExpressionContext expressionContext)
		{
			this.block = block;
			this.expressionContext = expressionContext;
		}
	}
}