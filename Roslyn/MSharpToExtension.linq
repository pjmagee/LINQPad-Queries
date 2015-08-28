<Query Kind="Program">
  <NuGetReference>Microsoft.CodeAnalysis</NuGetReference>
  <NuGetReference>Microsoft.CodeAnalysis.CSharp</NuGetReference>
  <NuGetReference>Microsoft.CodeAnalysis.CSharp.Extensions</NuGetReference>
  <NuGetReference>MSharp.Framework</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Extensions</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Formatting</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Syntax</Namespace>
  <Namespace>Microsoft.CodeAnalysis.Diagnostics</Namespace>
  <Namespace>Microsoft.CodeAnalysis.Emit</Namespace>
  <Namespace>Microsoft.CodeAnalysis.Text</Namespace>
  <IncludePredicateBuilder>true</IncludePredicateBuilder>
</Query>

void Main()
{
	var msCoreLib = PortableExecutableReference.CreateFromFile(typeof(object).Assembly.Location);
	var tree = CSharpSyntaxTree.ParseText(GetCode());
	var root = tree.GetRoot();
	var compilation = CSharpCompilation.Create("BasicDisposeRule", syntaxTrees: new[] { tree }, references: new[] { msCoreLib });
	var semanticModel = compilation.GetSemanticModel(tree, true);
	var invocations = root.DescendantNodes().OfType<InvocationExpressionSyntax>();

	foreach (var invocationSyntax in invocations)
	{
		var memberAccessExpressionSyntax = invocationSyntax.DescendantNodes().OfType<MemberAccessExpressionSyntax>().Single();

		var parseToken = memberAccessExpressionSyntax.DescendantTokens()
													 .Where(t => t.IsKind(SyntaxKind.IdentifierToken))
													 .FirstOrDefault(t => t.ValueText == "TryParse" || t.ValueText == "Parse");

		if (parseToken == null) continue;

		var identifierNameSyntax = (IdentifierNameSyntax)parseToken.Parent;

		var argumentListSyntax = invocationSyntax.ChildNodes().OfType<ArgumentListSyntax>().Single();
		
		if (identifierNameSyntax != null)
		{
			var methodUsed = identifierNameSyntax.ToString();
			
			var predefinedTypeSyntax = memberAccessExpressionSyntax.DescendantNodes().OfType<PredefinedTypeSyntax>().SingleOrDefault();
			var type = (predefinedTypeSyntax?.Keyword.ValueText) ?? memberAccessExpressionSyntax.DescendantNodes().First().ToString();

			if(argumentListSyntax.Arguments.Count > 2) return; 

			if (parseToken.ValueText == "TryParse")
			{
				$"use {argumentListSyntax.Arguments[0]}.TryParseAs<{type}>() instead of {type}.{methodUsed}{argumentListSyntax.ToString()}".Dump();
			}

			if (parseToken.ValueText == "Parse")
			{
				$"use {argumentListSyntax.Arguments[0]}.To<{type}>() instead of {type}.{methodUsed}{argumentListSyntax.ToString()}".Dump();
			}
		}
	}
}

public string GetCode() => File.ReadAllText(@"C:\Projects\GCop\GCop.Test.Code\ToParseAnalyzer\ShouldUseToInsteadOfParse.cs");