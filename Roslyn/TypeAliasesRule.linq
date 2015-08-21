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

	var aliases = LoadTypeAliases().Dump();

	var nodes = CSharpSyntaxTree.ParseText(File.ReadAllText(@"C:\Temp\sourcefile.cs"))
				    .GetRoot()
					.DescendantNodes()
					.Where(node => node.IsKind(SyntaxKind.IdentifierName))					
					.OfType<IdentifierNameSyntax>();
					//.Where(n => n.Parent.IsKind(SyntaxKind.ClassDeclaration) || n.Parent.IsKind(SyntaxKind.Parameter) || n.Parent.IsKind(SyntaxKind.MethodDeclaration) || n.Parent.IsKind(SyntaxKind.VariableDeclaration))
					//.Select(n => new { Name = n.ToString(), Location =  n.GetLocation().GetLineSpan().StartLinePosition, Parent = n.Parent.ToFullString()  }).Dump();				
		
	foreach(var node in nodes)
	{
		foreach(var alias in aliases)
		{
			if(alias.Value.Equals(node.ToString()))
			{
				// node.ToString().Dump("use {0}".FormatWith(alias.Key));
			}
		}	
	}
}

private Dictionary<string, string> LoadTypeAliases()
{
	var mscorlib = Assembly.GetAssembly(typeof(int));
	var aliases = new Dictionary<string, string>();
	
  	using (var provider = new CSharpCodeProvider())
	{
		foreach (var type in mscorlib.DefinedTypes)
		{
			if (string.Equals(type.Namespace, "System"))
			{
				var typeRef = new CodeTypeReference(type);
				var csTypeName = provider.GetTypeOutput(typeRef);
				
				if (csTypeName.IndexOf('.') == -1)
				{
					aliases.Add(csTypeName, type.Name);
				}
          }
      }
  }
  
  return aliases;
}

