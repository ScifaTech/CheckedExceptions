using System;
using System.Linq.Expressions;

namespace Scifa.CheckedExceptions.Test.TestFramework
{
	static class BlockActions
	{
		public static IBlockOrExpressionContext WhichThrows(this IBlockContext block, Type exceptionType)
		{
			ExpressionContext expressionContext = new ExpressionContext(Expression.Throw(Expression.New(exceptionType)));
			block.Expressions.Add(expressionContext);

			return new BlockOrExpressionContext(block, expressionContext);
		}

		public static TContext AtLocation<TContext>(this TContext expr, out CodeLocation location)
			where TContext:IExpressionContext
		{
			location = expr.LocationToken;
			return expr;
		}
	}
}
