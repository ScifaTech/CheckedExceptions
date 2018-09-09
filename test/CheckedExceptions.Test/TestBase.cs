using System.Collections.Generic;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Scifa.CheckedExceptions.Attributes;
using TestHelper;

namespace Scifa.CheckedExceptions.Test
{
    public abstract class TestBase : CodeFixVerifier
    {
        protected static readonly DiagnosticResult[] NoDiagnosticResults = new DiagnosticResult[0];

        protected override IEnumerable<MetadataReference> GetAdditionalReferences()
        {// CONTENT from: https://github.com/dotnet/corefx/issues/11601#issuecomment-336829147

            // first, collect all assemblies
            var assemblies = new HashSet<Assembly>();

            IncludeAll(Assembly.Load(new AssemblyName("netstandard")));

            //// add extra assemblies which are not part of netstandard.dll, for example:
            IncludeAll(typeof(CheckExceptionsAttribute).Assembly);

            // second, build metadata references for these assemblies
            var result = new List<MetadataReference>(assemblies.Count);
            foreach (var assembly in assemblies)
            {
                result.Add(MetadataReference.CreateFromFile(assembly.Location));
            }

            return result;

            // helper local function - add assembly and its referenced assemblies
            void IncludeAll(Assembly assembly)
            {
                if (!assemblies.Add(assembly))
                {
                    // already added
                    return;
                }

                var referencedAssemblyNames = assembly.GetReferencedAssemblies();

                foreach (var assemblyName in referencedAssemblyNames)
                {
                    var loadedAssembly = Assembly.Load(assemblyName);
                    assemblies.Add(loadedAssembly);
                }
            }
        }
        private protected static SourceCode BuildClass(params string[] methods)
        {
            return SourceCode.Empty
                .WithUsing("System")
                .WithUsing("Scifa.CheckedExceptions.Attributes")
                .WithAssemblyAttribute("CheckExceptions")
                .WithNamespace("Scifa.CheckedExceptions.TestSnippets")
                .WithClassName("Class1")
                .WithMethods(methods);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new CheckedExceptionsAnalyzer();
        }

    }
}