namespace AutoMapper.Analyzers.Common.Tests;

public class BaseAnalyzerTests
{
    private string replacingPattern = "//%#Replace#%";

    protected string EmptyProfileSourceCode(string replacer)
    {
        return GetSourceCode(nameof(EmptyProfileSourceCode)).Replace(replacingPattern, replacer);
    }
    
    protected string ClassSourceCode(string replacer)
    {
        return GetSourceCode(nameof(ClassSourceCode)).Replace(replacingPattern, replacer);
    }

    private string GetSourceCode(string methodName)
    {
        return File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $"{methodName}.cs"));
    }
}