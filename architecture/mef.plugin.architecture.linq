<Query Kind="Statements" />

public interface IFileExtractPlugin
{
	bool Supports(string path);
	PluginMetadata Metadata { get; }
	IEnumerable<Extract> GetExtracts(string contents);
}

public class CSharpProjectFilePlugin : IFileExtractPlugin
{
	public bool Supports(string path) => path.EndsWith(".csproj");
	
	public PluginMetadata Metadata => new PluginMetadata { Name = "MSBuild C# Project Files", Author = "Patrick Magee", Version=  "1.0", Id = Guid.Parse("") };
	
	public IEnumerable<Extract> GetExtracts(string contents)
	{
		// extract dependencies
		// extract framework monikers		
		yield break;	
	}
}

public class NodePackagesFilePlugin : IFileExtractPlugin
{
	public bool Supports(string path) => path.EndsWith("packages.json");

	public PluginMetadata Metadata => new PluginMetadata { Name = "Node Package Manager Support", Author = "Patrick Magee", Version = "1.0", Id = Guid.Parse("") };

	public IEnumerable<Extract> GetExtracts(string contents)
	{
		// extract dependencies		
		// extract engines		
		yield break;
	}
}

public interface IRegistryPlugin
{
	PluginMetadata Metadata { get; }
	Metadata GetLatestMetadata(Registry registry, string id);
}

public class DockerRegistryPlugin : IRegistryPlugin
{
	public PluginMetadata Metadata => new PluginMetadata { Name = "Docker Registry V2", Author = "Patrick Magee", Version = "1.0", Id = Guid.Parse("") };
	
	public Metadata GetLatestMetadata(Registry registry, string id)
	{
		return new Metadata { Author = "", Description = "", IsLatest = true, Name = "", ProjectUrl = "", Published = DateTime.Now, Version = "" };
	}
}

public class Engine 
{
	IEnumerable<IRegistryPlugin> registryPlugins = new List<IRegistryPlugin>();
	IEnumerable<IFileExtractPlugin> extractPlugins = new List<IFileExtractPlugin>();
	IEnumerable<IVCSPlugin> vcsPlugins = new List<IVCSPlugin>();
	
	void Compute()
	{
		Registry registry = new Registry { ApiKey = "API-KEY", Endpoint = "http://DOMAIN:8080", Username = "", Password = "" };
		VersionControl versionControl = new VersionControl  { Endpoint = "http://DOMAIN", ApiKey = "API-KEY",  };
	}
}

public class Registry
{
	public string Endpoint { get; set; }
	public string ApiKey { get; set; }
	public string Username { get; set; }
	public string Password { get; set; }
}

public class PluginMetadata
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public string Author { get; set; }
	public string Version { get; set; }
}

public interface IVCSPlugin
{
	PluginMetadata Metadata { get; }
	IEnumerable<Repository> GetRepositories(VersionControl versionControl);
}

public class BitBucketV2 : IVCSPlugin
{
   	public PluginMetadata Metadata  => new PluginMetadata { Name = "Bitbucket V2 API", Author = "Patrick Magee", Version = "1.0" };

	public IEnumerable<Repository> GetRepositories(VersionControl versionControl)
	{
		yield break;		
	}

	public IEnumerable<Asset> GetFiles(VersionControl versionControl, Repository repository)
	{
		yield break;
	}
}

public class Asset 
{
	public string Path { get; set; }
	public string Contents { get; set; }
}

public class VersionControl 
{
	public string Endpoint { get; set; }
	public string ApiKey { get; set; }
	public string Username { get; set; }
	public string Password { get; set; }	
	
	public Guid PluginId { get; set; }
}

public enum ExtractKind
{
	Dependency,
	Runtime,
	Framework
}

public class Repository 
{
	public string BrowseUrl { get; set; }
	public string VcsUrl { get; set; }
}

public class Extract
{
	ExtractKind Kind { get; set; }
	public string Name { get; set; }
	public string Version { get; set; }
}

public class Metadata
{	
	public string Name { get; set; }
	public string Version { get; set; }
	public string Description { get; set; }
	public string Author { get; set; }
	public string ProjectUrl { get; set; }
	public bool IsLatest { get; set; }
	public DateTime Published { get; set; }	
}