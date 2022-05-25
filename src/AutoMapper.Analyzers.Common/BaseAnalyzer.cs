﻿using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AutoMapper.Analyzers.Common;

public abstract class BaseAnalyzer : DiagnosticAnalyzer
{
    protected readonly LocalizableString Title;

    protected readonly LocalizableString Description;

    protected readonly LocalizableString Message;

    protected readonly DiagnosticDescriptor Rule;
    
    protected BaseAnalyzer()
    {
        Title = new LocalizableResourceString($"{InternalDiagnosticId}{nameof(Title)}", RulesResources.ResourceManager, typeof(RulesResources));
        Description = new LocalizableResourceString($"{InternalDiagnosticId}{nameof(Description)}", RulesResources.ResourceManager, typeof(RulesResources));
        Message = new LocalizableResourceString($"{InternalDiagnosticId}{nameof(Message)}", RulesResources.ResourceManager, typeof(RulesResources));
        Rule = new DiagnosticDescriptor(InternalDiagnosticId, Title, Message, Category.ToString(), Severity, true, Description);
    }
    
    protected abstract string InternalDiagnosticId { get; }
    
    public abstract DiagnosticCategory Category { get; }
    
    public virtual DiagnosticSeverity Severity => DiagnosticSeverity.Warning;
    
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);
    
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCodeBlockAction(AnalyzeCodeBlock);
        context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, SyntaxKind.ClassDeclaration);
    }
    
    protected virtual void AnalyzeCodeBlock(CodeBlockAnalysisContext context)
    {
        if (context.CodeBlock is ConstructorDeclarationSyntax constructorDeclarationSyntax)
        {
            var diagnostic = AnalyzeConstructor(constructorDeclarationSyntax);
            if (diagnostic != default)
            {
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
    
    protected virtual Diagnostic AnalyzeConstructor(ConstructorDeclarationSyntax constructorDeclarationSyntax)
    {
        return default;
    }
    
    protected virtual void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is ClassDeclarationSyntax classDeclaration)
        {
            var diagnostic = AnalyzeClassDeclaration(classDeclaration);
            if (diagnostic != default)
            {
                context.ReportDiagnostic(diagnostic);
            }
        }
    }

    protected abstract Diagnostic AnalyzeClassDeclaration(ClassDeclarationSyntax classDeclaration);
}