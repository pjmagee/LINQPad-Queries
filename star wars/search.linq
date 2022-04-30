<Query Kind="Program">
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Text.Json</Namespace>
</Query>

async Task Main()
{
	async Task<string> Query() => await Util.ReadLineAsync("Your search term");
	
	using (var client = new HttpClient())
	{
		client.BaseAddress = new Uri("https://www.starwars.com/search/more.json");

		string term;
		
		while((term = await Query()) != null)
		{
			var done = false;
			var uri = $"?f%5Bsearch_section%5D=Databank&p=1&q={term}&r=en";
			var results = new List<Dictionary<string, string>>();
			
			while (!done)
			{
				using (var document = await JsonDocument.ParseAsync(await client.GetStreamAsync(uri.Dump())))
				{
					var items = document.RootElement.GetProperty("results").EnumerateArray().Select(x => x.EnumerateObject().ToDictionary(x => x.Name, x => x.Value.ToString())).ToList();
					results.AddRange(items);
					
					done = document.RootElement.TryGetProperty("more_url", out var moreUrl);
					uri = moreUrl.GetString();
				}
			}
			
			results.Dump(term);
		}
	}
}