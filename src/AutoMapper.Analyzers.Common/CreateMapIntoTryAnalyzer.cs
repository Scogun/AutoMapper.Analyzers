using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AutoMapper.Analyzers.Common;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class CreateMapIntoTryAnalyzer : CreateMapAnalyzer
{
    public const string DiagnosticId = "AMA0007";

    protected override string InternalDiagnosticId => DiagnosticId;

    protected override Diagnostic AnalyzeCreateMap(InvocationExpressionSyntax createMapSyntax)
    {
        var constructorSyntax = createMapSyntax.Ancestors().OfType<ConstructorDeclarationSyntax>().FirstOrDefault();
        if (constructorSyntax is {Parent: ClassDeclarationSyntax classDeclaration} && classDeclaration.BaseList?.Types.Any(s => s.Type.ToString() == nameof(Profile)) == true)
        {
            var tryStatementSyntax = createMapSyntax.Ancestors().OfType<TryStatementSyntax>().FirstOrDefault();
            if (tryStatementSyntax != null)
            {
                var location = createMapSyntax.Ancestors().OfType<ExpressionStatementSyntax>().First().GetLocation();
                return Diagnostic.Create(Rule, location, ProfileName, createMapSyntax.Expression, GetTryCatchFinallyString(tryStatementSyntax));
            }
        }
        return base.AnalyzeCreateMap(createMapSyntax);
    }

    private string GetTryCatchFinallyString(TryStatementSyntax tryStatementSyntax)
    {
        StringBuilder builder = new StringBuilder("try-");
        var hasCatches = tryStatementSyntax.Catches.Count > 0;
        if (hasCatches)
        {
            builder.Append("catch");
        }

        if (tryStatementSyntax.Finally != null)
        {
            if (hasCatches)
            {
                builder.Append("/");
            }

            builder.Append("finally");
        }

        return builder.ToString();
    }
}