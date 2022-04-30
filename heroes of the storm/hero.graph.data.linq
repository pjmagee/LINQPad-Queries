<Query Kind="Program">
  <NuGetReference>AutomaticGraphLayout.WpfGraphControl</NuGetReference>
  <NuGetReference>Heroes.Icons</NuGetReference>
  <NuGetReference>HeroesDataParser</NuGetReference>
  <NuGetReference>TextCopy</NuGetReference>
  <Namespace>Heroes.Icons</Namespace>
  <Namespace>Heroes.Icons.DataDocument</Namespace>
  <Namespace>Heroes.Icons.HeroesData</Namespace>
  <Namespace>Heroes.Icons.ModelExtensions</Namespace>
  <Namespace>Heroes.Models</Namespace>
  <Namespace>Heroes.Models.AbilityTalents</Namespace>
  <Namespace>Heroes.Models.AbilityTalents.Tooltip</Namespace>
  <Namespace>Heroes.Models.Extensions</Namespace>
  <Namespace>Heroes.Models.Veterancy</Namespace>
  <Namespace>Microsoft.Msagl.Core</Namespace>
  <Namespace>Microsoft.Msagl.Core.Geometry</Namespace>
  <Namespace>Microsoft.Msagl.Core.Geometry.Curves</Namespace>
  <Namespace>Microsoft.Msagl.Layout.Layered</Namespace>
  <Namespace>Microsoft.Msagl.Miscellaneous</Namespace>
  <Namespace>Microsoft.Msagl.WpfGraphControl</Namespace>
  <Namespace>System.Text.Json.Serialization</Namespace>
  <Namespace>System.Text.Json</Namespace>
</Query>

using dwg = System.Drawing;
using Microsoft.Msagl.Drawing;

public class Node
{
	[JsonPropertyName("name")]
	public string Name { get; set; }
	
	[JsonPropertyName("value")]	
	public string? Value { get; set;}

	[JsonPropertyName("children")]
	public List<Node> Children { get; set; } = new ();
}

public class D3Category 
{
	[JsonPropertyName("name")]
	public string Name { get; set; }
	
	[JsonPropertyName("children")]
	public List<D3Hero> Children { get; set; }
 }
 
 public class D3Hero
{
	[JsonPropertyName("name")]
	public string Name { get; set; }

	[JsonPropertyName("imports")]
	public List<string> Imports { get; set; }
}

void Main()
{
	HeroesDataDirectory data = new HeroesDataDirectory(@"C:\Users\patri\OneDrive\Documents\Projects\heroes-data\heroesdata");
	HeroDataDocument heroData = data.HeroData(data.NewestVersion, true, Localization.ENUS);
	List<Hero> heroes = heroData.GetIds.Select(x => heroData.GetHeroById(x, true, true, true, false)).Distinct().ToList();
	IEnumerable<IGrouping<string, Hero>> francises = heroes.ToLookup(x => x.Franchise.GetFriendlyName(), x => x).Distinct();
	HashSet<string> tags = new HashSet<string>();
	Dictionary<Hero, List<string>> heroTagDictionary = new Dictionary<Heroes.Models.Hero, System.Collections.Generic.List<string>>();
		
	List<D3Hero> d3Heroes = new List<UserQuery.D3Hero>();

	var root = new
	{
		name = "heroes",
		children = tags
	}

	foreach (var hero in heroes)
	{
		// hero.SearchText.Dump();
		List<string> heroTags = new List<string>();

		/* Role Tags */
		heroTags.AddRange(hero.Roles.Select(role => "tag.Role." + role));
		heroTags.Add("tag.Role." + hero.ExpandedRole);

		/* Type */
		heroTags.Add("tag.Type." + hero.Type.PlainText);
		heroTags.Add("tag.Difficulty." + hero.Difficulty);

		/* Energy */
		heroTags.Add($"tag.Energy.{hero.Energy.EnergyType ?? "None"}");
		
		/* Macro */
		if (hero.SearchText.Contains("Double Soak")) heroTags.Add("tag.Macro.DoubleSoak");
		if (hero.SearchText.Contains("Offlaner")) heroTags.Add("tag.Macro.Offlaner");
		if (hero.SearchText.Contains("Push")) heroTags.Add("tag.Macro.Push");
		if (hero.SearchText.Contains("Camps")) heroTags.Add("tag.Macro.Camps");
		if (hero.SearchText.Contains("Clear")) heroTags.Add("tag.Macro.Waveclear");
		if (hero.SearchText.Contains("Mobile") || hero.SearchText.Contains("Mobility")) heroTags.Add("tag.Macro.Mobility");
		
		/* Style */
		if (hero.SearchText.Contains("Mage")) heroTags.Add("tag.Style.Mage");
		if (hero.SearchText.Contains("Marksman")) heroTags.Add("tag.Style.Marksman");
		if (hero.SearchText.Contains("Sustain")) heroTags.Add("tag.Style.Sustain");
		if (hero.SearchText.Contains("Artillery")) heroTags.Add("tag.Style.Artillery");
		if (hero.SearchText.Contains("Summoner")) heroTags.Add("tag.Style.Summoner");		

		/* Control */
		if (hero.SearchText.Contains("Displace")) heroTags.Add("tag.CC.Displace");
		if (hero.SearchText.Contains("Stun")) heroTags.Add("tag.CC.Stun");
		if (hero.SearchText.Contains("Root")) heroTags.Add("tag.CC.Root");
		if (hero.SearchText.Contains("Slow")) heroTags.Add("tag.CC.Slow");
		if (hero.SearchText.Contains("Sleep")) heroTags.Add("tag.CC.Sleep");
		if (hero.SearchText.Contains("Silence")) heroTags.Add("tag.CC.Silence");
		if (hero.SearchText.Contains("Blind")) heroTags.Add("tag.CC.Blind");
		if (hero.SearchText.Contains("Polymorph")) heroTags.Add("tag.CC.Polymorph");
		if (hero.SearchText.Contains("Disable")) heroTags.Add("tag.CC.Disable");
		if (hero.SearchText.Contains("Time Stop")) heroTags.Add("tag.CC.TimeStop");
		
		/* Micro */		
		if (hero.SearchText.Contains("Roam")) heroTags.Add("tag.Micro.Roam");
		if (hero.SearchText.Contains("Gank")) heroTags.Add("tag.Micro.Gank");
		if (hero.SearchText.Contains("Dive")) heroTags.Add("tag.Micro.Dive");
		if (hero.SearchText.Contains("Burst")) heroTags.Add("tag.Micro.Burst");
		if (hero.SearchText.Contains("Stealth")) heroTags.Add("tag.Micro.Stealth");
		if (hero.SearchText.Contains("Escape")) heroTags.Add("tag.Micro.Escape");		
		if (hero.SearchText.Contains("Initiation")) heroTags.Add("tag.Micro.Initiation");
		
		heroTags.Add(hero.Franchise.GetFriendlyName());
		
		heroTags.ForEach(t => tags.Add(t));
		heroTagDictionary.Add(hero, heroTags);

		d3Heroes.Add(new D3Hero { Name = hero.Name, Imports = heroTags.Select(x => $"tag.{x}").ToList() } );
	}
	
	List<D3Category> categories = new List<UserQuery.D3Category>();

	foreach (var tag in tags.OrderBy(x => x))
	{
		var node = new Node(){ Name = tag };
				
		var viewer = new AutomaticGraphLayoutControl();
		var graph = new Microsoft.Msagl.Drawing.Graph("graph");
		viewer.Graph = graph;
		graph.AddNode(tag);

		D3Category category = new D3Category() { Name = tag, Children = d3Heroes.Where(h => h.Imports.Contains($"tag.{tag}")).ToList() };		
		categories.Add(category);
		
		foreach (var hero in heroTagDictionary)
		{
			if (hero.Value.Contains(tag))
			{
				var edge = graph.AddEdge(tag, hero.Key.Name);

				node.Children.Add(new Node { Name = hero.Key.Name });

				if (hero.Key.Roles.Contains("Healer"))
					edge.TargetNode.Attr.Color = Color.Green;
				else if (hero.Key.Roles.Contains("Support"))
					edge.TargetNode.Attr.Color = Color.Green;
				else if (hero.Key.Roles.Contains("Warrior"))
					edge.TargetNode.Attr.Color = Color.Yellow;
				else if (hero.Key.Roles.Contains("Assassin"))
					edge.TargetNode.Attr.Color = Color.Red;
				else if (hero.Key.Roles.Contains("Specialist"))
					edge.TargetNode.Attr.Color = Color.Purple;
			}
		}
		
		root.Children.Add(node);
		
		new Hyperlinq(() => PanelManager.DisplayWpfElement(viewer, tag), tag).Dump();
	}

	foreach (var franchise in francises)
	{
		var node = new Node(){ Name = franchise.Key };
		
		var viewer = new Microsoft.Msagl.WpfGraphControl.AutomaticGraphLayoutControl();
		var graph = new Microsoft.Msagl.Drawing.Graph("graph");
		var franchiseNode = graph.AddNode(franchise.Key);

		D3Category category = new D3Category() { Name = franchise.Key, Children = d3Heroes.Where(h => h.Imports.Contains(franchise.Key)).ToList() };
		categories.Add(category);

		foreach (var hero in franchise)
		{			
			var heroNode = graph.AddNode(hero.Title);
			heroNode.LabelText = hero.Name;
			var edge = graph.AddEdge(franchise.Key, hero.Title);

			node.Children.Add(new Node { Name = hero.Name });

			if (hero.Roles.Contains("Healer"))
				edge.TargetNode.Attr.Color = Color.Green;
			else if (hero.Roles.Contains("Support"))
				edge.TargetNode.Attr.Color = Color.Green;
			else if (hero.Roles.Contains("Warrior"))
				edge.TargetNode.Attr.Color = Color.Yellow;
			else if (hero.Roles.Contains("Assassin"))
				edge.TargetNode.Attr.Color = Color.Red;
			else if (hero.Roles.Contains("Specialist"))
				edge.TargetNode.Attr.Color = Color.Purple;
		}
		
		viewer.MinHeight = 500;
		viewer.Graph = graph;
		
		root.Children.Add(node);
		new Hyperlinq(() => PanelManager.DisplayWpfElement(viewer, franchise.Key), franchise.Key).Dump();
	}
	
	var json = System.Text.Json.JsonSerializer.Serialize(root, new JsonSerializerOptions { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping, IgnoreNullValues = true });
	File.WriteAllText("C:\\temp\\data.json", json );
	
	json.Dump();
}