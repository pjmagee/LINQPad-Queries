<Query Kind="Program">
  <NuGetReference>Microsoft.CodeAnalysis.CSharp</NuGetReference>
  <NuGetReference>MSharp.Framework</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
  <Namespace>Microsoft.CodeAnalysis</Namespace>
  <Namespace>Microsoft.CSharp</Namespace>
  <Namespace>System.CodeDom</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Syntax</Namespace>
</Query>

void Main()
{
	var tree = CSharpSyntaxTree.ParseText(File.ReadAllText(@"SourceCode.cs"), null, @"SourceCode.cs");
	var nullableNodes = tree.GetRoot()
					.DescendantNodes()
					.OfType<GenericNameSyntax>()
					.Where(n => n.Identifier.Text == "Nullable");
					
	foreach(var nullableNode in nullableNodes)
	{
		var typeArgument = (TypeArgumentListSyntax) nullableNode.ChildNodes().Single(t => t.IsKind(SyntaxKind.TypeArgumentList)); // should be 1
		
		var argument = typeArgument.Arguments.Single().ToString();
			
		// var types = nullableNode.DescendantNodes().Where(t => t.IsKind(SyntaxKind.PredefinedType) || t.IsKind(SyntaxKind.IdentifierName));
		
		"Use {0}? instead of Nullable<{1}>".FormatWith(argument.ToString(), argument.ToString()).Dump();
	}
}