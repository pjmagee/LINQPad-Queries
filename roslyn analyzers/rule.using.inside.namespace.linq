<Query Kind="Statements">
  <NuGetReference>Microsoft.CodeAnalysis.CSharp</NuGetReference>
  <NuGetReference>MSharp.Framework</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
  <Namespace>Microsoft.CodeAnalysis</Namespace>
  <Namespace>Microsoft.CSharp</Namespace>
  <Namespace>System.CodeDom</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Syntax</Namespace>
</Query>

var usingDirectives = CSharpSyntaxTree.ParseText(File.ReadAllText(@"C:\Temp\sourcefile.cs"))
				    .GetRoot()
					.DescendantNodes()
					.Where(node => node.IsKind(SyntaxKind.UsingDirective))
					.OfType<UsingDirectiveSyntax>();
					
foreach(var usingDirective in usingDirectives)
{
	var namespaces = usingDirective.Parent.ChildNodes().Where(n => n.IsKind(SyntaxKind.NamespaceDeclaration));

	foreach(var ns in namespaces.Cast<NamespaceDeclarationSyntax>())
	{		
		Console.WriteLine("'{0}' should be inside '{1}'", usingDirective.ToString(), ns.Name);
	}

}
					