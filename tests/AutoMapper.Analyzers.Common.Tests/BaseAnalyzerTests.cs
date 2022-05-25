namespace AutoMapper.Analyzers.Common.Tests;

public class BaseAnalyzerTests
{
    protected string EmptyProfileSourceCode(string replacer)
    {
        var sourceCode = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"{nameof(EmptyProfileSourceCode)}.cs"));
        return sourceCode.Replace("//%#Replace#%", replacer);
    }
}