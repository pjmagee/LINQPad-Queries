<Query Kind="Program">
  <NuGetReference>Microsoft.Extensions.Configuration</NuGetReference>
  <NuGetReference>Microsoft.Extensions.Configuration.Json</NuGetReference>
  <Namespace>Microsoft.Extensions.Configuration</Namespace>
  <Namespace>Microsoft.Extensions.Configuration.Json</Namespace>
  <Namespace>Microsoft.Extensions.Configuration.Memory</Namespace>
  <Namespace>Microsoft.Extensions.FileProviders</Namespace>
  <Namespace>Microsoft.Extensions.FileProviders.Internal</Namespace>
  <Namespace>Microsoft.Extensions.FileProviders.Physical</Namespace>
  <Namespace>Microsoft.Extensions.FileSystemGlobbing</Namespace>
  <Namespace>Microsoft.Extensions.FileSystemGlobbing.Abstractions</Namespace>
  <Namespace>Microsoft.Extensions.FileSystemGlobbing.Internal</Namespace>
  <Namespace>Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments</Namespace>
  <Namespace>Microsoft.Extensions.FileSystemGlobbing.Internal.PatternContexts</Namespace>
  <Namespace>Microsoft.Extensions.FileSystemGlobbing.Internal.Patterns</Namespace>
  <Namespace>Microsoft.Extensions.Primitives</Namespace>
  <Namespace>System.Net</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

async Task Main()
{
	using(var client = new WebClient())
	{
		var json = await client.DownloadStringTaskAsync(new Uri("https://raw.githubusercontent.com/HeroesToolChest/heroes-data/master/heroesdata/2.49.2.77981/gamestrings/gamestrings_77981_enus.json"));
		
		await File.WriteAllTextAsync(@"C:\temp\heroes.json", json);

		IConfiguration configuration = new ConfigurationBuilder().AddJsonFile(@"C:\temp\heroes.json").Build();

		configuration.GetSection("gamestrings:unit:name").Dump();
	}
	
	
}

// Define other methods, classes and namespaces here
