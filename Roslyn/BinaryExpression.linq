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
	var compilation = CSharpCompilation.Create("HasMany", syntaxTrees: new[] { tree }, references: new[] { msCoreLib });
	var semanticModel = compilation.GetSemanticModel(tree, true);
	var binaryExpressions = root.DescendantNodes().OfType<BinaryExpressionSyntax>().ToList();
	
	binaryExpressions.Count.Dump("expressions");

	foreach (var expression in binaryExpressions)
	{
		var memberAccessExpressionSyntax = expression.Left as MemberAccessExpressionSyntax;
		var invocationExpressionSyntax = expression.Left as InvocationExpressionSyntax;
		var lambdaExpressionSyntax = expression.Left as LambdaExpressionSyntax;
			
		if (memberAccessExpressionSyntax != null)
		{			
			var memberAccessExpressionName = memberAccessExpressionSyntax.Name as IdentifierNameSyntax;
			var identifierNameSyntax = memberAccessExpressionSyntax.Expression as IdentifierNameSyntax;
		
			// Check Name
			if (memberAccessExpressionName != null)
			{
				memberAccessExpressionSyntax.Name.ToString().Dump("memberAccessExpressionName");
				semanticModel.GetTypeInfo(memberAccessExpressionSyntax.Name).Type.Interfaces.Any(x => x.Name == "IEnumerable").Dump("IEnumerable?");
			}
				
			// Check Expression
			if (identifierNameSyntax != null)
			{
				identifierNameSyntax.ToString().Dump("identifierNameSyntax");
				semanticModel.GetTypeInfo(identifierNameSyntax).Type.Interfaces.Any(x => x.Name == "IEnumerable").Dump("IEnumerable?");
			}
		}
		else if (invocationExpressionSyntax != null)
		{
			memberAccessExpressionSyntax = invocationExpressionSyntax.DescendantNodes().OfType<MemberAccessExpressionSyntax>().FirstOrDefault();

			if (memberAccessExpressionSyntax != null)
			{
				var typeInfo = semanticModel.GetTypeInfo(memberAccessExpressionSyntax.Expression);
				memberAccessExpressionSyntax.Expression.ToString().Dump("InvocationExpressionSyntax");
				typeInfo.Type.Interfaces.Any(x => x.Name == "IEnumerable").Dump("IEnumerable?");
			}			
			
		}
		else if (lambdaExpressionSyntax != null)
		{
			var typeInfo = semanticModel.GetTypeInfo(lambdaExpressionSyntax.Body);
			typeInfo.Type.Name.ToString().Dump();
		}
	}
}

public string GetCode() => File.ReadAllText(@"C:\Projects\GCop\GCop.Test.Code\HasManyAnalyzer\ShouldUseHasManyNotCountGreaterThanOne.cs");

// Define other methods and classes here