<Query Kind="Program">
  <Namespace>Foole.Mpq</Namespace>
</Query>

#load "HeroesOfTheStorm\HotsMonitor"

void Main()
{
	var bytes = File.ReadAllBytes(@"C:\temp\lobbyfiles\replay.server.battlelobby.Infernal Shrines.e866bd14-8c65-4a27-be8f-8f2a54c75615");
	
	var parser = new BattleLobbyParser2(bytes);
	
	parser.Data.Find(new[] { (byte)'A', (byte)'n', (byte)'u' }).Dump();
	
	parser.GetMap().Dump();
	parser.GetPlayers().Dump();
	parser.GetRegion().Dump();	
}


public class BattleLobbyParser2
{
	private readonly byte[] data;
	private static readonly int MaxTagByteLength = 32;

	public BattleLobbyParser2(byte[] data) => this.data = data;
	
	public byte[] Data => this.data;
	
	public Region GetRegion()
	{
		var i = data.Find(new byte[] { (byte)'s', (byte)'2', (byte)'m', (byte)'h', 0, 0 });
		if (i == -1) return Region.UNKNOWN;
		var region = new string(new char[] { (char)data[i + 6], (char)data[i + 7] });
		return Enum.TryParse<Region>(region, out Region result) ? result : Region.UNKNOWN;
	}

	public List<(string Tag, string Hero)> GetPlayers()
	{
		var players = new List<(string Tag, string Hero)>();
		var offset = data.Find(Enumerable.Repeat<byte>(0, 32).ToArray());

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

				players.Add((tag, "unknown"));
			}
			
			if (players.Count == 10) break;
		}

		return players;
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

		if (name == null) return false;

		var match = Regex.Match(name, @"\w{2,12}$");

		if (match.Success)
		{
			name = match.Value + tag;
			return true;
		}
		
		return false;		
	}

	public string GetMap()
	{		
		var mapNumber = data[0];
		var mapLength = data[1];
		
		var values = Enumerable.Range(0, mapNumber)
			.Select(i => Encoding.Default.GetString(data.Skip(2 + i * (mapLength + 2)).Take(mapLength).ToArray()))
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
			
		values.Dump();

		return Maps.First(map => values.Any(value => value.Contains(map, StringComparison.OrdinalIgnoreCase)));
	}

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