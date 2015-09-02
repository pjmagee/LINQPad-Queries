<Query Kind="Program">
  <NuGetReference>MSharp.Framework</NuGetReference>
  <NuGetReference>Microsoft.CodeAnalysis</NuGetReference>
  <NuGetReference>Microsoft.CodeAnalysis.CSharp</NuGetReference>
  <NuGetReference>Microsoft.CodeAnalysis.CSharp.Extensions</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Extensions</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Syntax</Namespace>
  <IncludePredicateBuilder>true</IncludePredicateBuilder>
</Query>

void Main()
{
	var msCoreLib = PortableExecutableReference.CreateFromFile(typeof(object).Assembly.Location);
	var tree = CSharpSyntaxTree.ParseText(GetCode());
	var compilation = CSharpCompilation.Create("BasicDisposeRule", syntaxTrees: new[] { tree }, references: new[] { msCoreLib });
	var semanticModel = compilation.GetSemanticModel(tree, true);	
	var castExpressions = tree.GetRoot().DescendantNodes().OfType<CastExpressionSyntax>();
		
	foreach(var castExpression in castExpressions)
	{		
		castExpression.IsUnnecessaryCast(semanticModel, new CancellationToken(false)).Dump();
		
//		if(typeInfo.Type.Name == typeInfo.ConvertedType.Name)
//		{
//			"Cast is redundant, Line: {0}, Col: {1}".FormatWith(castExpression.GetLocation().GetLineSpan().StartLinePosition.Line + 1, castExpression.GetLocation().GetLineSpan().StartLinePosition.Character + 1).Dump();
//		}
	}
}

public string GetCode()
{
	return File.ReadAllText(@"C:\Projects\GCop\GCop.Test.Code\CastingExpressionAnalyzerTests\ShouldRemoveRedundantCasts.cs");
}