using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace AutoMapper.Analyzers.Common.Tests;

public class BaseAnalyzerTests
{
    private string replacingPattern = "//%#Replace#%";

    protected string EmptyProfileSourceCode(string replacer)
    {
        return GetSourceCode(nameof(EmptyProfileSourceCode)).Replace(replacingPattern, replacer);
    }

    protected string ClassSourceCode(string replacer, bool normalize = false)
    {
        var sourceCode = GetSourceCode(nameof(ClassSourceCode)).Replace(replacingPattern, replacer);
        if (normalize)
        {
            var syntaxis = CSharpSyntaxTree.ParseText(sourceCode).GetRoot().NormalizeWhitespace();
            return syntaxis.ToFullString();
        }

        return sourceCode;
    }

    private string GetSourceCode(string methodName)
    {
        return File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"{methodName}.cs"));
    }
}