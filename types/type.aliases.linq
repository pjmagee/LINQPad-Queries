<Query Kind="Statements">
  <Namespace>System.CodeDom</Namespace>
  <Namespace>Microsoft.CSharp</Namespace>
</Query>

using (var provider = new CSharpCodeProvider())
{
	var mscorlib = Assembly.GetAssembly(typeof(int));
	var aliases = new Dictionary<string, string>();

	mscorlib
	.DefinedTypes
	.Where(t => t.Namespace == "System")
	.Select(type => new { Type = type, Out = provider.GetTypeOutput(new CodeTypeReference(type)) })
	.Where(x => x.Out.IndexOf('.') == -1)
	.ToDictionary(x => x.Out, x => x.Type)
	.Dump();
}