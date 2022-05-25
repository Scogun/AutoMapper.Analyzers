using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AutoMapper.Analyzers.Common;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ConstructorAnalyzer : BaseAnalyzer
{
    public static string DiagnosticId = "AMA0001";
    
    protected override string InternalDiagnosticId => DiagnosticId;
    
    public override DiagnosticCategory Category => DiagnosticCategory.ProfileCategory;

    protected override Diagnostic AnalyzeConstructor(ConstructorDeclarationSyntax constructorDeclarationSyntax)
    {
        var classDeclaration = constructorDeclarationSyntax.Parent as ClassDeclarationSyntax;
        if (classDeclaration?.BaseList?.Types[0].Type.ToString() == nameof(Profile))
        {
            if (constructorDeclarationSyntax.Body?.DescendantNodes().OfType<ExpressionStatementSyntax>().Any(s =>
                    s.DescendantNodes().OfType<GenericNameSyntax>()
                        .Any(n => n.Identifier.ValueText.StartsWith(nameof(Profile.CreateMap), StringComparison.InvariantCulture))) == false)
            {
                return Diagnostic.Create(Rule, constructorDeclarationSyntax.GetLocation(), classDeclaration.Identifier);
            }
        }

        return default;
    }
    
    protected override Diagnostic AnalyzeClassDeclaration(ClassDeclarationSyntax classDeclaration)
    {
        if (classDeclaration.BaseList?.Types[0].Type.ToString() == nameof(Profile))
        {
            if (!classDeclaration.Members.Any(m => m.IsKind(SyntaxKind.ConstructorDeclaration)))
            {
                return Diagnostic.Create(Rule, classDeclaration.GetLocation(), classDeclaration.Identifier);
            }
        }

        return default;
    }
}