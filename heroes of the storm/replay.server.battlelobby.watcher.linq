<Query Kind="Program">
  <NuGetReference>AngleSharp</NuGetReference>
  <NuGetReference>AngleSharp.Io</NuGetReference>
  <NuGetReference>AngleSharp.Js</NuGetReference>
  <NuGetReference>AngleSharp.XPath</NuGetReference>
  <NuGetReference>MpqTool.netstandard</NuGetReference>
  <Namespace>AngleSharp</Namespace>
  <Namespace>AngleSharp.Io</Namespace>
  <Namespace>Foole.Mpq</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

async Task Main()
{
	var hotsMonitor = new HotsMonitor();
	
	hotsMonitor.BattleLobbyCreated += (sender, e) => 
	{
		e.Dump("Battle lobby created: " + DateTime.Now.ToString());		
		
		var game = new BattleLobbyParser(e.Data).GetGame();

		File.Copy(e.Data, @$"C:\temp\lobbyfiles\replay.server.battlelobby.{game.Map}.{Guid.NewGuid()}");
		
		game.Dump();
	};
	
	
	hotsMonitor.RejoinFileCreated += (sender, e) => 
	{
		e.Dump("Rejoin created: " + DateTime.Now.ToString());		
	};
	
	hotsMonitor.ReplayFileCreated += (sender, e) => 
	{
		e.Dump("Replay created: " + DateTime.Now.ToString());
	};
			
	await hotsMonitor.StartMonitoring(QueryCancelToken);	
}

public enum Region
{
	US = 1,
	EU = 2,
	KR = 3,
	CN = 5,
	XX = -1,
	UNKNOWN
}

public class Game
{
	public Region Region { get; }
	public List<Player> Players { get; }
	public string Map { get; }
	
	public Game(Region region, List<Player> players, string map)
	{
		this.Region = region;
		this.Players = players;
		this.Map = map;
	}
}

public class MapParser 
{
	private readonly byte[] data;
	private readonly List<string> maps;
	
	public MapParser(byte[] data, List<string> maps)
	{
		this.data = data;
		this.maps = maps;
	}
	
	public string GetMap()
	{
		var mapNumber = data[0];
		var mapLength = data[1];
		
		var values = Enumerable.Range(0, mapNumber)
			.Select(i => Encoding.Default.GetString(data.Skip(2 + i * (mapLength + 2)).Take(mapLength).ToArray()))
			.AsParallel()
			.Select(path =>
			{
				try
				{
					using (var mpqReader = new MpqArchive(File.OpenRead(path)))
					{
						if (mpqReader.FileExists("DocumentInfo"))
						{						
							using (var stream = mpqReader.OpenFile("DocumentInfo"))
							{
								var buffer = new byte[stream.Length];
								stream.Read(buffer, 0, (int)stream.Length);
								return Encoding.Default.GetString(buffer);
							}
						}										
					}
					
					return string.Empty;
				}
				catch (MpqParserException e)
				{
					return string.Empty;
				}
			})
			.Where(value => !string.IsNullOrWhiteSpace(value))
			.ToList();
			

		return maps.First(map => values.Any(value => value.Contains(map, StringComparison.OrdinalIgnoreCase)));
	}
}

public class RegionParser
{
	private readonly byte[] data;

	public RegionParser(byte[] data) => this.data = data;

	public Region GetRegion()
	{		
		var i = data.Find(new byte[] { (byte)'s', (byte)'2', (byte)'m', (byte)'h', 0, 0 });		
		if (i == -1) return Region.UNKNOWN;		
		var region = new string(new char[] { (char)data[i + 6], (char)data[i + 7] });
		return Enum.TryParse<Region>(region, out Region result) ? result : Region.UNKNOWN;
	}
}

public class PlayerParser
{
	private static readonly int MaxTagByteLength = 32;
	private readonly byte[] data;
	private readonly List<string> heroes;		
	private int tagOffset;
	
	public PlayerParser(byte[] data, List<string> heroes)
	{		
		this.data = data; 
		this.heroes = heroes;		
		this.tagOffset = data.Find(Enumerable.Repeat<byte>(0, 32).ToArray()); 
	}

	public List<Player> GetPlayers()
	{		
		var tags = this.GetPlayerTags();
		
		var players = new List<Player>();		
								
		for(int i = 0; i < tags.Count; i++)
		{
			var name = tags[i];
			var team = i >= 5 ? 1 : 0;
			var hero = heroes[i];
			
			players.Add(new Player(name, team, 0, string.Empty));
		}
				
		return players;
	}
	
	private List<string> GetPlayerTags()
	{
		var tags = new List<string>();
		var offset = tagOffset;

		while (true)
		{
			offset = data.Find(new byte[] { (byte)'#' }, offset + 1);
			if (offset == -1) break;

			if (this.TryExtractBattleTag(offset, out string tag))
			{
				if (tag.Any(l => l > 512))
				{
					int index = tag.IndexOf(tag.First(l => l > 512));
					tag = tag.Substring(index);
				}

				tags.Add(tag);
			}
			
			if (tags.Count == 10) break;
		}

		return tags;
	}

	private bool TryExtractBattleTag(int offset, out string name)
	{
		name = null;
		
		string tag = "#";

		for (int i = 1; i < 10; i++)
		{
			var c = (char)data[offset + i];
			if (char.IsDigit(c)) tag += c;
			else break;
		}

		if (tag.Length < 5 || tag.Length > 9) return false;


		for (int i = MaxTagByteLength - 1; i >= 3; i--)
		{
			try
			{
				name = Encoding.UTF8.GetString(data, offset - i, i);
				break;
			}
			catch (ArgumentException e) { }
		}

		if (name == null) 
			return false;

		var match = Regex.Match(name, @"\w{2,12}$");

		if (match.Success)
		{
			name = match.Value + tag;
			return true;
		}
		
		return false;		
	}

	private List<Tuple<int, int>> GetStrings(int offset = 0, int minLength = 0, int maxLength = 255)
	{
		var result = new List<Tuple<int, int>>();

		for (int i = offset; i < data.Length; i++)
		{
			if (data[i] >= minLength && data[i] <= maxLength && i + data[i] + 1 < data.Length)
			{
				result.Add(new Tuple<int, int>(i + 1, data[i]));
			}
		}
		return result;
	}
}

public static class Constants
{	
	public static List<string> Heroes = new List<string>()
    {
		"Abathur", "Alarak", "Alexstrasza", "Ana", "Anub'arak", "Artanis", "Arthas", "Auriel", "Azmodan",
		"Blaze", "Brightwing", "Butcher",
		"Cassia", "Chen", "Cho", "Chromie",
		"Dehaka", "Deckard", "Deathwing", "Diablo", "D.Va",
		"E.T.C.",
		"Falstad", "Fenix",
		"Gall", "Garrosh", "Gazlowe", "Genji", "Greymane", "Gul'dan",
		"Hanzo",
		"Illidan",
		"Jaina", "Johanna", "Junkrat",
		"Kael'thas", "Kel'Thuzad", "Kerrigan", "Kharazim",
		"Leoric", "Li Li", "Li-Ming", "Lost Vikings", "Lúcio", "Lt. Morales", "Lunara",
		"Maiev", "Malfurion", "Mal'Ganis", "Malthael", "Medivh", "Mephisto", "Muradin", "Murky",
		"Nazeebo", "Nova",
		"Orphea",
		"Probius",
		"Qhira",
		"Ragnaros", "Raynor", "Rehgar", "Rexxar",
		"Samuro", "Sgt. Hammer", "Sonya", "Stitches", "Stukov", "Sylvanas",
		"Tassadar", "Thrall", "Tracer", "Tychus", "Tyrael", "Tyrande",
		"Uther",
		"Valeera", "Valla", "Varian",
		"Whitemane",
		"Xul",
		"Yrel",
		"Zagara", "Zarya", "Zeratul", "Zul'jin",
	};

	public static List<string> Maps = new List<string>()
	{
		"Braxis Holdout",
		"Warhead Junction",
		"Haunted Mines",
		"Blackheart's Bay",
		"Hanamura",
		"Dragon Shire",
		"Garden of Terror",
		"Infernal Shrines",
		"Towers of Doom",
		"Sky Temple",
		"Volskaya",
		"Battlefield of Eternity",
		"Tomb of the Spider Queen",
		"Cursed Hollow"
	};

}

public class BattleLobbyParser
{
	private readonly RegionParser regionParser;
	private readonly PlayerParser playerParser;
	private readonly MapParser mapParser;

	public BattleLobbyParser(string path) : this(File.ReadAllBytes(path), Constants.Heroes, Constants.Maps)
	{ 
		
	}
	
	public BattleLobbyParser(byte[] data, List<string> heroes, List<string> maps) : this(new RegionParser(data), new PlayerParser(data, heroes), new MapParser(data, maps))
	{
		
	}
	
	public BattleLobbyParser(RegionParser regionParser, PlayerParser playerParser, MapParser mapParser)
	{
		this.regionParser = regionParser;
		this.mapParser = mapParser;
		this.playerParser = playerParser;
	}
	
	public Game GetGame() => new Game(
		regionParser.GetRegion(),  // works
		playerParser.GetPlayers(),  // only player tags works (selected hero is broken)
		mapParser.GetMap()); // works
}

public class Stats
{
	public string Type { get; set; }
	public string WinRate { get; set; }
	public string Bracket { get; set; }
	public string MMR { get; set; }
}

public class Player
{
	public List<Stats> Stats { get; set; }
	
	public Player(string tag, int level, int team, string hero)
	{
		if (char.IsDigit(tag[0]))
			tag = tag.Substring(1);

		Tag = tag;
		Hero = hero;
		Team = team;
		Level = level;
	}

	public string Tag { get; }

	public int Team { get; }
	
	public int Level { get; }
	
	public string Hero { get; }
}

public static class BytesHelper
{
	public static int Find(this byte[] data, byte[] pattern, int offset = 0)
	{
		for (int i = offset; i < data.Length - pattern.Length; i++)
			if (Match(data, pattern, i)) return i;
			
		return -1;
	}

	public static bool Match(this byte[] data, byte[] pattern, int offset = 0)
	{
		for (int i = 0; i < pattern.Length; i++)
			if (data[offset + i] != pattern[i])	return false;

		return true;
	}
}

public class HotsMonitor
{
	public readonly string BattleLobbyPath = Path.Combine(Path.GetTempPath(), @"Heroes of the Storm\TempWriteReplayP1\replay.server.battlelobby");	
	public readonly string ProfilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"Heroes of the Storm\Accounts");
	
	private FileSystemWatcher rejoinWatcher;
	private FileSystemWatcher replayWatcher;
	private DateTime lobbyLastModified;

	public event EventHandler<EventArgs<string>> BattleLobbyCreated;
	public event EventHandler<EventArgs<string>> RejoinFileCreated;
	public event EventHandler<EventArgs<string>> ReplayFileCreated;
	
	protected virtual void OnBattleLobbyCreated(string path) => BattleLobbyCreated?.Invoke(this, new EventArgs<string>(path));	
	protected virtual void OnRejoinFileCreated(string path) => RejoinFileCreated?.Invoke(this, new EventArgs<string>(path));	
	protected virtual void OnReplayFileCreated(string path) => ReplayFileCreated?.Invoke(this, new EventArgs<string>(path));

	public Task StartMonitoring(CancellationToken cancellationToken, int interval = 1000)
	{
		rejoinWatcher = new FileSystemWatcher(ProfilePath, "*.StormSave");
		rejoinWatcher.IncludeSubdirectories = true;
		rejoinWatcher.Created += (o, e) => OnRejoinFileCreated(e.FullPath);
		rejoinWatcher.EnableRaisingEvents = true;

		replayWatcher = new FileSystemWatcher(ProfilePath, "*.StormReplay");
		replayWatcher.IncludeSubdirectories = true;
		replayWatcher.Created += (o, e) => OnReplayFileCreated(e.FullPath);
		replayWatcher.EnableRaisingEvents = true;

		return Task.Factory.StartNew(() =>
		{
			while (true)
			{
				if (File.Exists(BattleLobbyPath) && File.GetLastWriteTime(BattleLobbyPath) != lobbyLastModified)
				{
					lobbyLastModified = File.GetLastWriteTime(BattleLobbyPath);
					OnBattleLobbyCreated(BattleLobbyPath);
				}
				Thread.Sleep(interval);
			}
		},
		cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Current);
	}

	public class EventArgs<T> : EventArgs
	{
		public T Data { get; private set; }

		public EventArgs(T input)
		{
			Data = input;
		}
	}
}