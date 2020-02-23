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
	var binaryExpressions = root.DescendantNodes().OfType<BinaryExpressionSyntax>();

	foreach (var be in binaryExpressions)
	{
		if (be.OperatorToken.IsKind(SyntaxKind.GreaterThanToken))
		{
			var memberAccessExpressionSyntax = be.Left.DescendantNodesAndSelf().OfType<MemberAccessExpressionSyntax>().FirstOrDefault();
			if (memberAccessExpressionSyntax == null) continue;

			var methodNameSyntax = memberAccessExpressionSyntax?.DescendantNodesAndSelf().OfType<IdentifierNameSyntax>().Last();
			if (methodNameSyntax.ToString() != "Count" && methodNameSyntax.ToString() != "Length") continue;

			var typeNameSyntax = memberAccessExpressionSyntax?.DescendantNodesAndSelf().OfType<IdentifierNameSyntax>().First();

			var objectCreationSyntax = memberAccessExpressionSyntax?.DescendantNodesAndSelf().OfType<ObjectCreationExpressionSyntax>().FirstOrDefault();

			if (objectCreationSyntax != null)
			{
				var variable = objectCreationSyntax;
				$"Use '{variable}.HasMany()' instead of '{be}'".Dump("RULE");
			}
			else
			{
				var invocationExpressionSyntax = memberAccessExpressionSyntax.Parent as InvocationExpressionSyntax;

				if (invocationExpressionSyntax != null)
				{
					if (invocationExpressionSyntax.ArgumentList.Arguments.Count > 0) continue;
				}

				var rightExpressionSyntax = be.Right as LiteralExpressionSyntax;
				if ((int)rightExpressionSyntax?.Token.Value != 1) continue;
				var variable = memberAccessExpressionSyntax.DescendantNodes().First();

				$"Use '{variable}.HasMany()' instead of '{be}'".Dump("RULE");
			}
		}
		else if (be.OperatorToken.IsKind(SyntaxKind.LessThanToken))
		{

		}
	}
}

public string GetCode() => File.ReadAllText(@"ShouldUseHasManyNotCountGreaterThanOne.cs");