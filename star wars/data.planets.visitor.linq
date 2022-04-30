<Query Kind="Program">
  <Reference Relative="..\StarWarsPlanets.linq">&lt;MyDocuments&gt;\LINQPad Queries\StarWarsPlanets.linq</Reference>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>Microsoft.EntityFrameworkCore</Namespace>
</Query>

#load "data.planets"

async Task Main()
{
	using (var context = new Context())
	{
		await context.Database.EnsureDeletedAsync();
		await context.Database.EnsureCreatedAsync();

		var regions = await context.Nodes.Where(x => x.NodeType == NodeType.Region).Include(x => x.Nodes).ThenInclude(x => x.Nodes).ToListAsync();

		var character = new Character();

		var region = context.Nodes.First(x => x.Name == "Outer Rim" && x.NodeType == NodeType.Region);

		character.VisitAll(region);
		character.Progress(region).Dump();

		foreach (var node in character.Visited)
		{
			$"{node.Key.Name}".PadLeft(node.Value * 10, '-').Dump();
		}
	}
}

class Character
{
	public Dictionary<Node, int> Visited = new Dictionary<Node, int>();

	public (int Current, int Total) Progress(Node node, int current = 0, int total = 0)
	{
		current += Visited.ContainsKey(node) ? 1 : 0;
		total += 1;

		foreach (var n in node.Nodes)
		{
			var progress = Progress(n, current, total);
			current = progress.Current;
			total = progress.Total;
		}

		return (current, total);
	}

	public int Depth(Node node)
	{
		int depth = 0;

		while (node.Parent != null)
		{
			node = node.Parent;
			depth++;
		}

		return depth;
	}

	public void Visit(Node node) => Visited.Add(node, Depth(node));

	public void VisitAll(Node node, int depth = -1)
	{
		depth = depth == -1 ? Depth(node) : depth;

		Visited.Add(node, depth);

		foreach (var n in node.Nodes)
		{
			VisitAll(n, depth + 1);
		}
	}
}