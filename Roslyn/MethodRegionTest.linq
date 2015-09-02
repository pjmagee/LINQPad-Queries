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
	var compilation = CSharpCompilation.Create("RegionTriviaTest", syntaxTrees: new[] { tree }, references: new[] { msCoreLib });
	var semanticModel = compilation.GetSemanticModel(tree, true);
	var method = root.DescendantNodes().OfType<MethodDeclarationSyntax>().Last();
	
	var trivia = method.DescendantTrivia().Where(t => t.IsKind(SyntaxKind.EndRegionDirectiveTrivia));

	foreach (var t in trivia)
	{
		method = method.ReplaceTrivia(t, trivia.ToSyntaxTriviaList().Except(st => st.IsKind(SyntaxKind.EndRegionDirectiveTrivia)));
	}
	
	method.ToFullString().Dump();
	
	method.ToString().Dump();

}

public string GetCode() => File.ReadAllText(@"C:\ExampleCode.cs");