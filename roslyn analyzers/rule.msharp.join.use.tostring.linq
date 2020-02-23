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
	var compilation = CSharpCompilation.Create("ShouldUseMSharpJoinExtension", syntaxTrees: new[] { tree }, references: new[] { msCoreLib });
	var semanticModel = compilation.GetSemanticModel(tree, true);

	var invocations = tree.GetRoot().DescendantNodes().OfType<InvocationExpressionSyntax>();

	foreach (InvocationExpressionSyntax invocation in invocations)
	{
		if (invocation.Expression.ToString().IsAnyOf("String.Join", "string.Join"))
		{
			var argumentListSyntax = invocation.ChildNodes().OfType<ArgumentListSyntax>().Single();
			var seperator = argumentListSyntax.Arguments.First();
			var seperatorName = seperator.ChildNodes().Where(x => x.IsKind(SyntaxKind.StringLiteralExpression) || x.IsKind(SyntaxKind.IdentifierName)).Single().ToString();
			var collection = argumentListSyntax.Arguments.Last();
			var collectionName = collection.ChildNodes().OfType<IdentifierNameSyntax>().Single();
			$"Use {collectionName}.ToString({seperatorName}) instead of {invocation.Expression.ToString()}({seperatorName}, {collectionName})".Dump();			
		}
	}

	var stringKeywords = tree.GetRoot().DescendantTokens().Where(t => t.IsKind(SyntaxKind.StringKeyword));

	foreach (var keyword in stringKeywords)
	{
		// is this a .Join method on string?		
		var memberAccessExpressionSyntax = keyword.Parent.FirstAncestorOrSelf<MemberAccessExpressionSyntax>();
		var methodName = memberAccessExpressionSyntax.DescendantTokens().FirstOrDefault(x => x.IsKind(SyntaxKind.IdentifierToken));

		// This is a string.Join(A, B)
		var isJoinMethod = methodName.ValueText == "Join";

		// now we suggest to use B.ToString(A)
		if (isJoinMethod)
		{
			var invocationExpressionSyntax = memberAccessExpressionSyntax.Parent as InvocationExpressionSyntax;
			var argumentListSyntax = invocationExpressionSyntax.ChildNodes().OfType<ArgumentListSyntax>().Single();

			var seperator = argumentListSyntax.Arguments.First();
			var seperatorName = seperator.ChildNodes().Where(x => x.IsKind(SyntaxKind.StringLiteralExpression) || x.IsKind(SyntaxKind.IdentifierName)).Single().ToString();
			var collection = argumentListSyntax.Arguments.Last();
			var collectionName = collection.ChildNodes().OfType<IdentifierNameSyntax>().Single();

			// $"Use {collectionName}.ToString({seperatorName}) instead of string.Join({seperatorName}, {collectionName})".Dump();			
		}
	}

	stringKeywords.Select(x => x.GetLocation().GetLineSpan().ToString()).Dump();
}

public string GetCode() => File.ReadAllText(@"ShouldUseMSharpJoinExtension.cs");