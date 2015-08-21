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

	var nodes = tree
				    .GetRoot()
					.DescendantNodes().OfType<ClassDeclarationSyntax>();
	
	foreach(var classNode in nodes)
	{
		var filePath = classNode.SyntaxTree.FilePath;				
		var fileName = Path.GetFileName(filePath);
		
		if(classNode.Identifier.Text != fileName.Replace(".cs", string.Empty))
		{
			"should be in another file".Dump();
		}
	}
	
}