<Query Kind="Program">
  <NuGetReference>Heroes.Icons</NuGetReference>
  <NuGetReference>Heroes.Models</NuGetReference>
  <NuGetReference>HeroesDataParser</NuGetReference>
  <Namespace>System.Drawing</Namespace>
</Query>

void Main()
{
	using(var data = Heroes.Icons.DataDocument.HeroDataDocument.Parse(@"C:\Users\patri\Downloads\heroes-data-2.53.2.84249_all.tar\heroes-data-2.53.2.84249_all\2.53.2.84249\data\herodata_84249_localized.json"))
	{
		
		Random random = new Random();
		
		var result = data.GetAttributeIds
			.Select(attributeId => data.GetHeroByAttributeId(attributeId, false, false, false, true))
			.SelectMany(hero => hero.HeroUnits.Select(hu => hu.Id).Concat(new[] { hero.Id }))
			.Distinct()
			.Select(tag =>
			{
				return new Tag
				{
					color = "",
					name = tag				
				};			
			}).ToList();
	
		var colors = new Queue<string>(Enumerable.Range(1, result.Count()).Select(x => 
		{
			var color = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
	
			return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
		}));
	
		result.ForEach(item => 
		{
			item.color = colors.Dequeue();
		});

	
		result.Count.Dump();

		System.Text.Json.JsonSerializer.Serialize(result, new System.Text.Json.JsonSerializerOptions() {  WriteIndented = true }).Dump();
			
	}
}

class Tag
{
	public string name { get; set;}
	public string color { get; set;}
}
