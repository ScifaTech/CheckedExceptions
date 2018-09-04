using TestHelper;

namespace Scifa.CheckedExceptions.Test
{
	internal interface ICodeLocator
	{
		DiagnosticResultLocation GetDiagnosticLocation(int method, int line, int @char);
	}
}