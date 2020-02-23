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
	var compilation = CSharpCompilation.Create("LocalTime.XXX", syntaxTrees: new[] { tree }, references: new[] { msCoreLib });
	var semanticModel = compilation.GetSemanticModel(tree, true);
	var invocations = root.DescendantNodes().OfType<InvocationExpressionSyntax>();

	
	var expressions = root.DescendantNodes().OfType<MemberAccessExpressionSyntax>();

	foreach (var expression in expressions)
	{
		if (expression.ToString() == "DateTime.Now")
		{
			"Use LocalTime.{0} instead of  ".Dump();
		}

		if (expression.ToString() == "DateTime.Today")
		{
			"Use LocalTime.Today".Dump();
		}
	}
	
}

public string GetCode() => File.ReadAllText(@"ShouldUseLocalTimeNotDateTime.cs");