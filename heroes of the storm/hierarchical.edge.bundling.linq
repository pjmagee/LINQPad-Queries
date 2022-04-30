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

/* Data generated for https://observablehq.com/@pjmagee/heroes-of-the-storm-hero-relationships */

public class Node
{
	[JsonPropertyName("name")]
	public string Name { get; set; }

	[JsonPropertyName("imports")]
	public List<string> Imports { get; set; } = new ();
}

IEnumerable<string> GetImports(Hero hero)
{
	foreach (var role in hero.Roles)
		yield return $"hero.Roles.{role}";

	yield return $"hero.Roles.{hero.ExpandedRole}";
	yield return $"hero.Types.{hero.Type.PlainText}";
	yield return $"hero.Difficulties.{hero.Difficulty}";
	yield return $"hero.Energies.{hero.Energy.EnergyType?.Replace(" ", "") ?? "None"}";

	yield return hero.Gender.HasValue ? $"hero.Gender.{hero.Gender.Value}" : $"hero.Gender.None";
	yield return $"hero.Radius.Size {hero.Radius.ToString().Replace(".", "_")}";
	yield return $"hero.Rarity.{hero.Rarity}";
	
	if(hero.Talents.Any(t => t.Tooltip.FullTooltip?.PlainText.Contains("Sleep", StringComparison.OrdinalIgnoreCase) == true))
		yield return "hero.Talents.Sleep*";

	if (hero.Talents.Any(t => t.Tooltip.FullTooltip?.PlainText.Contains("Attack Speed", StringComparison.OrdinalIgnoreCase) == true && t.Tooltip.FullTooltip?.PlainText.Contains("Gain", StringComparison.OrdinalIgnoreCase) == true))
		yield return "hero.Talents.Attack speed*";

	if (hero.Talents.Any(t => t.Tooltip.FullTooltip?.PlainText.Contains("Stores", StringComparison.OrdinalIgnoreCase) == true && t.Tooltip.FullTooltip?.PlainText.Contains("Charges", StringComparison.OrdinalIgnoreCase) == true))
		yield return "hero.Talents.Stored charges*";

	if (hero.Talents.Any(t => t.Tooltip.FullTooltip?.PlainText.Contains("Shield", StringComparison.OrdinalIgnoreCase) == true))
		yield return "hero.Talents.Shield*";

	if (hero.Talents.Any(t => t.Tooltip.FullTooltip?.PlainText.Contains("Gambit", StringComparison.OrdinalIgnoreCase) == true))
		yield return "hero.Talents.Gambit*";

	if (hero.Talents.Any(t => t.Tooltip.FullTooltip?.PlainText.Contains("Taunt", StringComparison.OrdinalIgnoreCase) == true))
		yield return "hero.Talents.Taunt*";

	if (hero.Talents.Any(t => t.Tooltip.FullTooltip?.PlainText.Contains("Unstoppable", StringComparison.OrdinalIgnoreCase) == true))
		yield return "hero.Talents.Unstoppable*";	

	if (hero.Talents.Any(t => t.Tooltip.FullTooltip?.PlainText.Contains("Teleport", StringComparison.OrdinalIgnoreCase) == true))
		yield return "hero.Talents.Teleport*";

	if (hero.Talents.Any(t => t.Tooltip.FullTooltip?.PlainText.Contains("Stasis", StringComparison.OrdinalIgnoreCase) == true))
		yield return "hero.Talents.Stasis*";

	if (hero.Talents.Any(t => t.Tooltip.FullTooltip?.PlainText.Contains("Activate to reset", StringComparison.OrdinalIgnoreCase) == true))
		yield return "hero.Talents.Activate to reset*";

	if (hero.Talents.Any(t => t.Tooltip.FullTooltip?.PlainText.Contains("Revive", StringComparison.OrdinalIgnoreCase) == true || t.Tooltip.FullTooltip?.PlainText.Contains("Resurrect", StringComparison.OrdinalIgnoreCase) == true))
		yield return "hero.Talents.Revive*";

	if (hero.Talents.Any(t => t.Tooltip.FullTooltip?.PlainText.Contains("Takedown", StringComparison.OrdinalIgnoreCase) == true && t.Tooltip.FullTooltip?.PlainText.Contains("Reset", StringComparison.OrdinalIgnoreCase) == true))
		yield return "hero.Talents.Reset on Takedown*";

	if (hero.Talents.Any(t => t.Tooltip.FullTooltip?.PlainText.Contains("Bribe", StringComparison.OrdinalIgnoreCase) == true))
		yield return "hero.Talents.CampBribe*";
		
	if(hero.Talents.Any(t => t.Tooltip.FullTooltip?.PlainText.Contains("Protected", StringComparison.OrdinalIgnoreCase) == true))
		yield return "hero.Talents.Protected*";

	if(hero.Talents.Any(t => t.IsActive))
		yield return "hero.Talents.ActiveTalent*";

	if (hero.Talents.Any(t => t.IsQuest))
		yield return "hero.Talents.QuestTalent*";
		
	yield return hero.UsesMount ? "hero.Mounting.Mount" : "hero.Mounting.No Mount";
	
	hero.HeroDescriptors.Dump();
	
	yield return $"hero.Ratings.Complexity {hero.Ratings.Complexity.ToString("0#")}";
	yield return $"hero.Ratings.Damage {hero.Ratings.Damage.ToString("0#")}";
	yield return $"hero.Ratings.Survivability {hero.Ratings.Survivability.ToString("0#")}";
	yield return $"hero.Ratings.Utility {hero.Ratings.Utility.ToString("0#")}";	

	/* Lane and Map Related */
	
	if (hero.SearchText.Contains("Double Soak")) yield return "hero.Lanes.Double Soak";
	if (hero.SearchText.Contains("Offlaner") || hero.HeroDescriptors.Contains("SoloLaner")) yield return "hero.Lanes.Offlaner";
	if (hero.SearchText.Contains("Push") || hero.HeroDescriptors.Contains("TowerPusher")) yield return "hero.Lanes.Push";
	if (hero.SearchText.Contains("Camps")) yield return "hero.Lanes.Camps";
	if (hero.SearchText.Contains("Clear") || hero.HeroDescriptors.Contains("WaveClearer")) yield return "hero.Lanes.Wave clear";
	
	/* Hero Styles */
	if (hero.SearchText.Contains("Mage") || hero.HeroDescriptors.Contains("RoleCaster")) yield return "hero.Styles.Mage";
	if (hero.SearchText.Contains("Marksman") || hero.HeroDescriptors.Contains("RoleAutoAttacker")) yield return "hero.Styles.Marksman";
	if (hero.SearchText.Contains("Artillery")) yield return "hero.Styles.Artillery";
	if (hero.SearchText.Contains("Summoner")) yield return "hero.Styles.Summoner";	
	if (hero.SearchText.Contains("Sustain")) yield return "hero.Styles.Sustain";
	if (hero.SearchText.Contains("Mobile") || hero.SearchText.Contains("Mobility")) yield return "hero.Styles.Mobility";
	if (hero.SearchText.Contains("Roam")) yield return "hero.Styles.Roam";
	if (hero.SearchText.Contains("Gank") || hero.HeroDescriptors.Contains("Ganker")) yield return "hero.Styles.Gank";
	if (hero.SearchText.Contains("Dive")) yield return "hero.Styles.Dive";
	if (hero.SearchText.Contains("Burst")) yield return "hero.Styles.Burst";
	if (hero.SearchText.Contains("Stealth")) yield return "hero.Styles.Stealth";
	if (hero.SearchText.Contains("Escape") || hero.HeroDescriptors.Contains("Escaper")) yield return "hero.Styles.Escape";
	if (hero.SearchText.Contains("Initiation")) yield return "hero.Styles.Initiation";

	/* Control */
	if (hero.SearchText.Contains("Displace")) yield return "hero.CrowdControl.Displace";
	if (hero.SearchText.Contains("Stun")) yield return "hero.CrowdControl.Stun";
	if (hero.SearchText.Contains("Root")) yield return "hero.CrowdControl.Root";
	if (hero.SearchText.Contains("Slow")) yield return "hero.CrowdControl.Slow";
	if (hero.SearchText.Contains("Sleep")) yield return "hero.CrowdControl.Sleep";
	if (hero.SearchText.Contains("Silence")) yield return "hero.CrowdControl.Silence";
	if (hero.SearchText.Contains("Blind")) yield return "hero.CrowdControl.Blind";
	if (hero.SearchText.Contains("Polymorph")) yield return "hero.CrowdControl.Polymorph";
	if (hero.SearchText.Contains("Disable")) yield return "hero.CrowdControl.Disable";
	if (hero.SearchText.Contains("Time Stop")) yield return "hero.CrowdControl.Time Stop";

	yield return $"hero.Franchises." + hero.Franchise.GetFriendlyName();
}

void Main()
{
	HeroesDataDirectory data = new HeroesDataDirectory(@"C:\Users\patri\OneDrive\Documents\Projects\heroes-data\heroesdata");
	HeroDataDocument heroData = data.HeroData(data.NewestVersion, true, Localization.ENUS);
	List<Hero> heroes = heroData.GetIds.Select(x => heroData.GetHeroById(x, true, true, true, true)).ToList();
	
	var d3Heroes = heroes.Select(hero =>
	{
		return new Node
		{
			Name = $"hero.name.{hero.Name.Replace(" ", "").Replace(".", "")}",
			Imports = GetImports(hero).ToList()
		};
	})
	.ToList();

	var d3Tags = d3Heroes.SelectMany(x => x.Imports).Distinct().Select(import => 
	{
		return new Node
		{
			Name = import,
			Imports = d3Heroes.Where(x => x.Imports.Contains(import)).Select(x => x.Name).ToList()
		};
	}).ToList();
	
	
	var items = d3Heroes.Concat(d3Tags).ToArray().Dump();

	var json = JsonSerializer.Serialize(items, new JsonSerializerOptions { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping, IgnoreNullValues = true, WriteIndented = true });
	
	
}