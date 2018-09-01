using System.Linq.Expressions;

namespace Scifa.CheckedExceptions.Test.TestFramework
{
	internal class ExpressionContext : IExpressionContext
	{
		private CodeLocation locationToken;
		public Expression Expression { get; }

		public CodeLocation LocationToken => locationToken ?? (locationToken = new CodeLocation());

		public ExpressionContext(Expression expression)
		{
			Expression = expression;
		}
	}
}