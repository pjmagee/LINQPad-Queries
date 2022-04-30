<Query Kind="Program">
  <NuGetReference>Microsoft.EntityFrameworkCore</NuGetReference>
  <NuGetReference>Microsoft.EntityFrameworkCore.InMemory</NuGetReference>
  <NuGetReference>Microsoft.EntityFrameworkCore.Relational</NuGetReference>
  <NuGetReference>Npgsql.EntityFrameworkCore.PostgreSQL</NuGetReference>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>Microsoft.EntityFrameworkCore</Namespace>
  <Namespace>Microsoft.EntityFrameworkCore.Metadata.Builders</Namespace>
</Query>

async Task Main()
{
	using (var context = new Context())
	{
		await context.Database.EnsureDeletedAsync();
		await context.Database.EnsureCreatedAsync();

		var regions = await context.Nodes.Where(x => x.NodeType == NodeType.Region).Include(x => x.Nodes).ThenInclude(x => x.Nodes).ToListAsync();

		regions.Dump();
	}
}


public class Context : DbContext
{
	public DbSet<Node> Nodes { get; set; }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder
			.EnableSensitiveDataLogging(true)
			.UseInMemoryDatabase("Nodes", config =>
		{
			config.EnableNullChecks(true);
		});
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfiguration(new NodeConfig());
	}
}

public class NodeConfig : IEntityTypeConfiguration<Node>
{
	public NodeConfig() : base()
	{
		
	}

	public void Configure(EntityTypeBuilder<Node> builder)
	{
		builder.HasKey(x => x.Id);
		builder.HasOne(x => x.Parent).WithMany(x => x.Nodes);
		builder.HasData(GetNodes());
	}

	IEnumerable<Node> GetNodes()
	{		
		var lines = File.ReadAllLines(Util.MyQueriesFolder + "\\data\\Star Wars Galaxy Map Grid Coordinates - planets.csv");

		List<Node> nodes = new List<Node>();

		foreach (var mapRegion in lines.Skip(1)
			.Select(line =>
			{
				var fields = line.Split(',', StringSplitOptions.None);

				return new
				{
					Planet = fields[0],
					Grid = fields[1],
					Sector = fields[2],
					Region = fields[3]
				};
			})
			.GroupBy(mapPlanet => mapPlanet.Region))
		{
			var region = new Node
			{
				Name = mapRegion.Key,
				NodeType = NodeType.Region,
			};

			nodes.Add(region);

			var regionPlanets = mapRegion.OrderBy(x => x.Planet);

			foreach (var mapSector in regionPlanets.GroupBy(x => x.Sector))
			{
				if (!string.IsNullOrWhiteSpace(mapSector.Key))
				{
					var sector = new Node
					{
						Name = mapSector.Key,
						ParentId = region.Id,
						NodeType = NodeType.Sector
					};

					nodes.Add(sector);

					foreach (var mapPlanet in mapSector)
					{
						var planet = new Node { Name = mapPlanet.Planet, NodeType = NodeType.Planet, ParentId = sector.Id };
						nodes.Add(planet);
					}
				}
				else
				{
					foreach (var mapPlanet in mapSector)
					{
						var planet = new Node { Name = mapPlanet.Planet, NodeType = NodeType.Planet, ParentId = region.Id };
						nodes.Add(planet);
					}
				}
			}
		}

		return nodes;
	}
}

public enum NodeType
{
	Region, // Outer Rim, Core World
	Sector, // Hutt Space, Kessel, Sith Worlds, Bothan Space, 
	Planet, // Planet, Ship, Asteroid, Moon
	POI,
	Area, // Docking Bay, Underground Mine Facility
}

public class Node : IEquatable<Node>
{
	public Guid Id { get; set; }

	public string Name { get; set; }

	public NodeType NodeType { get; set; }

	public Node? Parent { get; set; }

	public Guid? ParentId { get; set; }

	public List<Node> Nodes { get; set; }

	public bool Equals(Node obj) => Id.Equals(obj.Id);

	public Node()
	{
		Id = Guid.NewGuid();
		Nodes = new();
	}

	public override int GetHashCode()
	{
		int hc = Id.GetHashCode();
		    foreach (var n in Nodes)
		      hc ^= n.GetHashCode();
		return hc;
	}
}