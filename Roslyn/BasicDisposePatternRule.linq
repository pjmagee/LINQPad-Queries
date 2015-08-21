<Query Kind="Program">
  <NuGetReference>MSharp.Framework</NuGetReference>
  <NuGetReference>Microsoft.CodeAnalysis</NuGetReference>
  <NuGetReference>Microsoft.CodeAnalysis.CSharp</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis</Namespace>
  <Namespace>Microsoft.CodeAnalysis.Classification</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CodeActions</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CodeFixes</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CodeRefactorings</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Formatting</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Syntax</Namespace>
  <Namespace>Microsoft.CodeAnalysis.Diagnostics</Namespace>
  <Namespace>Microsoft.CodeAnalysis.Differencing</Namespace>
  <Namespace>Microsoft.CodeAnalysis.Editing</Namespace>
  <Namespace>Microsoft.CodeAnalysis.Emit</Namespace>
  <Namespace>Microsoft.CodeAnalysis.FindSymbols</Namespace>
  <Namespace>Microsoft.CodeAnalysis.Formatting</Namespace>
  <Namespace>Microsoft.CodeAnalysis.Host</Namespace>
  <Namespace>Microsoft.CodeAnalysis.Host.Mef</Namespace>
  <Namespace>Microsoft.CodeAnalysis.MSBuild</Namespace>
  <Namespace>Microsoft.CodeAnalysis.Options</Namespace>
  <Namespace>Microsoft.CodeAnalysis.Recommendations</Namespace>
  <Namespace>Microsoft.CodeAnalysis.Rename</Namespace>
  <Namespace>Microsoft.CodeAnalysis.Simplification</Namespace>
  <Namespace>Microsoft.CodeAnalysis.Text</Namespace>
  <Namespace>Microsoft.CSharp</Namespace>
  <Namespace>System</Namespace>
  <Namespace>System.CodeDom</Namespace>
  <Namespace>System.Collections.Immutable</Namespace>
  <Namespace>System.Composition</Namespace>
  <Namespace>System.Composition.Convention</Namespace>
  <Namespace>System.Composition.Hosting</Namespace>
  <Namespace>System.Composition.Hosting.Core</Namespace>
  <Namespace>System.Linq</Namespace>
  <Namespace>System.Reflection</Namespace>
  <Namespace>System.Reflection.Metadata</Namespace>
  <Namespace>System.Reflection.Metadata.Ecma335</Namespace>
  <Namespace>System.Reflection.PortableExecutable</Namespace>
</Query>

void Main()
{
	var msCoreLib = PortableExecutableReference.CreateFromFile(typeof(object).Assembly.Location);
	var tree = CSharpSyntaxTree.ParseText(GetCode());
	var compilation = CSharpCompilation.Create("BasicDisposeRule", syntaxTrees: new[] { tree }, references: new[] { msCoreLib });		
	var semanticModel = compilation.GetSemanticModel(tree, true);					
	
	var fieldDeclarationSyntaxes = tree.GetRoot().DescendantNodes().OfType<FieldDeclarationSyntax>();
	
	foreach(var fieldDeclarationSyntax in fieldDeclarationSyntaxes)
	{
		var classDeclarationSyntax = fieldDeclarationSyntax.FirstAncestorOrSelf<ClassDeclarationSyntax>();
		
		if(classDeclarationSyntax.BaseList == null || classDeclarationSyntax.BaseList.Types.Any(n => n.ToString() != "IDisposable"))
		{
			"The class '{0}' should implement IDisposable because '{1}' implements IDisposable".FormatWith(classDeclarationSyntax.Identifier.Text, fieldDeclarationSyntax.Declaration.Variables.ToString()).Dump();
		}
	}
}

public string GetCode()
{
	return @"namespace Test
			{
				using System;
				using System.IO;				
			
				public class ShouldImplementIDisposable
				{
					private TextReader textReader;
					
					private TextReader one, two, three;
			
					public ShouldImplementIDisposable(TextReader textReader)
					{
						this.textReader = textReader;
					}
				}
			}";
}