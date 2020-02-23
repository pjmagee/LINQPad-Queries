<Query Kind="Program" />

void Main()
{
	var bytes = File.ReadAllBytes(@"C:\temp\lobbyfiles\replay.server.battlelobby.Tomb of the Spider Queen.a518622a-b637-4165-9a77-0cd92e808994");

	var parser = new SelectedHeroParser(bytes, Heroes);

	var matches = parser.LookForMatches(new List<string>() { "Zagara", "Ragnaros", "Tassadar", "Lucio", "Genji", "Tychus", "Greymane", "Brightwing", "Abathur", "Illidan" }.OrderBy(x => x).ToList(), 0);
	
	matches.Dump();

}

public static List<string> Heroes = new List<string>()
	{
		"Abathur", 
		"Alarak", 
		"Alexstrasza", 
		"Ana", 
		"Anub'arak", 
		"Artanis", 
		"Arthas", 
		"Auriel",
		"Azmodan",
		"Blaze", 
		"Brightwing", 
		"Butcher",
		"Cassia", 
		"Chen", 
		"Cho", 
		"Chromie",
		"Dehaka", 
		"Deckard",
		"Deathwing",
		"Diablo", 
		"D.Va",
		"E.T.C.",
		"Falstad", 
		"Fenix",
		"Gall", 
		"Garrosh", 
		"Gazlowe", 
		"Genji", 
		"Greymane",
		"Gul'dan",
		"Hanzo",
		"Illidan",
		"Jaina", 
		"Johanna",
		"Junkrat",
		"Kael'thas", 
		"Kel'Thuzad",
		"Kerrigan", 
		"Kharazim",
		"Leoric", 
		"Li Li", 
		"Li-Ming", 
		"Lost Vikings",
		"Lúcio", 
		"Lt. Morales",
		"Lunara",
		"Maiev", 
		"Malfurion",
		"Mal'Ganis", 
		"Malthael",
		"Medivh", 
		"Mephisto",
		"Muradin",
		"Murky",
		"Nazeebo",
		"Nova",
		"Orphea",
		"Probius",
		"Qhira",
		"Ragnaros",
		"Raynor",
		"Rehgar", 
		"Rexxar",
		"Samuro",
		"Sgt. Hammer",
		"Sonya", 
		"Stitches",
		"Stukov",
		"Sylvanas",
		"Tassadar", 
		"Thrall",
		"Tracer", 
		"Tychus",
		"Tyrael", 
		"Tyrande",
		"Uther",
		"Valeera", 
		"Valla", 
		"Varian",
		"Whitemane",
		"Xul",
		"Yrel",
		"Zagara", 
		"Zarya", 
		"Zeratul", 
		"Zul'jin",
	}.OrderBy(x => x).ToList();

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

public class LobbyParameter
{
	public byte EvenByte1 { get; set; }

	public byte EvenByte2 { get; set; }

	public byte EvenIncrement { get; set; }

	public byte EvenNotation { get; set; }

	public byte OddByte1 { get; set; }

	public byte OddByte2 { get; set; }

	public byte OddIncrement { get; set; }

	public byte OddNotation { get; set; }

	public int OffSet { get; set; }

	public byte RandomEvenByte1 { get; set; }

	public byte RandomEvenByte2 { get; set; }

	public byte RandomOddByte1 { get; set; }

	public byte RandomOddByte2 { get; set; }

	public bool StartWithOdd { get; set; }
}

public class HeroElement
{
	public string Name { get; set; }

	public byte EvenByte1 { get; set; }

	public byte EvenByte2 { get; set; }

	public byte OddByte1 { get; set; }

	public byte OddByte2 { get; set; }
}

public static class BytesHelper
{
	public static int Find(this byte[] data, byte[] pattern, int offset = 0)
	{
		for (int i = offset; i < data.Length - pattern.Length; i++)
			if (Match(data, pattern, i))
				return i;

		return -1;
	}

	public static bool Match(this byte[] data, byte[] pattern, int offset = 0)
	{
		for (int i = 0; i < pattern.Length; i++)
			if (data[offset + i] != pattern[i])
				return false;

		return true;
	}
}

public class SelectedHeroParser
{
	public const string Random = "Random";
	public const string Fail = "Fail";

	private readonly byte[] randomOddBytes = { 0x00, 0x00 };
	private readonly byte[] randomEvenBytes = { 0x00, 0x00 };
	private readonly byte[] s2mv = { 0x73, 0x32, 0x6D, 0x76, 0, 0 };

	private readonly List<HeroElement> heroElements = new List<HeroElement>();
	
	private readonly byte[] data;
	private readonly List<string> heroes;

	public SelectedHeroParser(byte[] data, List<string> heroes)
	{
		this.data = data;
		this.heroes = heroes;
	}

	public List<string> ParseHeroesInfo(LobbyParameter lobbyParameter)
	{
		var selectedHeroes = new List<string>();
		var offset = data.Find(s2mv) - lobbyParameter.OffSet;

		InitializeHeroes(lobbyParameter.OddByte1, lobbyParameter.OddByte2, lobbyParameter.EvenByte1, lobbyParameter.EvenByte2, lobbyParameter.OddNotation, lobbyParameter.EvenNotation, lobbyParameter.OddIncrement, lobbyParameter.EvenIncrement);
		randomOddBytes[0] = lobbyParameter.RandomOddByte1;
		randomOddBytes[1] = lobbyParameter.RandomOddByte2;
		randomEvenBytes[0] = lobbyParameter.RandomEvenByte1;
		randomEvenBytes[1] = lobbyParameter.RandomEvenByte2;

		if (lobbyParameter.StartWithOdd)
			FivePairsAttempt(offset, selectedHeroes, lobbyParameter.EvenIncrement);
		else
			SevenPairsAttempt(offset, selectedHeroes, lobbyParameter.OddIncrement);

		return selectedHeroes;
	}

	public List<LobbyParameter> LookForMatches(List<string> expectedHeroes, int startingOffset)
	{
		var selectedHeroes = new List<string>();
		
		// s2mv
		var offset = data.Find(s2mv) - startingOffset;
		var successfulLobbyParameters = new List<LobbyParameter>();

		var possibleByteStarts = new[] { new byte[] { 0, 2, 0, 2 }, new byte[] { 2, 0, 0, 2 }, new byte[] { 0, 2, 1, 0 }, new byte[] { 2, 0, 2, 0 } };
		
		// 9 * 9 * 9 * 9 * 4 / 2
		for (byte oddNotation = 1; oddNotation > 0; oddNotation *= 2)
		{
			for (byte oddIncrement = 1; oddIncrement > 0; oddIncrement *= 2)
			{
				for (byte evenNotation = 1; evenNotation > 0; evenNotation *= 2)
				{
					for (byte evenIncrement = 1; evenIncrement > 0; evenIncrement *= 2)
					{
						if (evenIncrement < oddNotation && oddIncrement < evenNotation)
						{
							continue;
						}

						foreach (var possibleByteStart in possibleByteStarts)
						{
							InitializeHeroes(possibleByteStart[0], possibleByteStart[1], possibleByteStart[2], possibleByteStart[3], oddNotation, evenNotation, oddIncrement, evenIncrement);
													
							if (evenIncrement > oddNotation && FivePairsAttempt(offset, selectedHeroes, evenIncrement))
							{
								if (expectedHeroes.All(h => selectedHeroes.Contains(h)))
								{
									var lobbyParameter = new LobbyParameter()
									{
										OddByte1 = possibleByteStart[0],
										OddByte2 = possibleByteStart[1],
										EvenByte1 = possibleByteStart[2],
										EvenByte2 = possibleByteStart[3],
										OddNotation = oddNotation,
										EvenNotation = evenNotation,
										OddIncrement = oddIncrement,
										EvenIncrement = evenIncrement,
										OffSet = startingOffset,
										StartWithOdd = true
									};

									successfulLobbyParameters.Add(lobbyParameter);
								}
							}

							
							if (oddIncrement > evenNotation && SevenPairsAttempt(offset, selectedHeroes, oddIncrement))
							{
								if (expectedHeroes.All(h => selectedHeroes.Contains(h)))
								{
									var lobbyParameter = new LobbyParameter()
									{
										OddByte1 = possibleByteStart[0],
										OddByte2 = possibleByteStart[1],
										EvenByte1 = possibleByteStart[2],
										EvenByte2 = possibleByteStart[3],
										OddNotation = oddNotation,
										EvenNotation = evenNotation,
										OddIncrement = oddIncrement,
										EvenIncrement = evenIncrement,
										OffSet = startingOffset,
										StartWithOdd = false
									};

									successfulLobbyParameters.Add(lobbyParameter);
								}
							}
						}
					}
				}
			}
		}

		return successfulLobbyParameters;
	}

	private static bool IsSuccessful(IList<string> selectedHeroes)
	{
		return !selectedHeroes.Contains(Fail) && !selectedHeroes.Contains(Random) && selectedHeroes.Count == selectedHeroes.Distinct().Count();
	}

	private void InitializeHeroes(byte oddByte1, byte oddByte2, byte evenByte1, byte evenByte2, byte oddNotation, byte evenNotation, byte oddIncrement, byte evenIncrement)
	{
		heroElements.Clear();

		foreach (var hero in heroes)
		{
			var heroElement = new HeroElement
			{
				EvenByte1 = evenByte1,
				EvenByte2 = evenByte2,
				OddByte1 = oddByte1,
				OddByte2 = oddByte2,
				Name = hero
			};
			
			heroElements.Add(heroElement);

			oddByte2++;
			if (oddByte2 >= oddNotation)
			{
				oddByte2 = 0;
				oddByte1 += oddIncrement;
			}

			evenByte2++;
			if (evenByte2 >= evenNotation)
			{
				evenByte2 = 0;
				evenByte1 += evenIncrement;
			}
		}
	}

	private bool FivePairsAttempt(int offset, List<string> selectedHeroes, int evenIncrement)
	{
		selectedHeroes.Clear();
		
		var oddOffsetStart = offset;		
		var oddOffset = oddOffsetStart;
		
		for (; oddOffset <= oddOffsetStart + 12; oddOffset += 3)
		{
			selectedHeroes.Add(ParseOddHeroSelectionStartOddFirst(oddOffset, evenIncrement));
			var evenOffset = oddOffset + 1;
			selectedHeroes.Add(ParseEvenHeroSelectionStartOddFirst(evenOffset, evenIncrement));
		}

		return IsSuccessful(selectedHeroes);
	}
	
	private bool SevenPairsAttempt(int offset, List<string> selectedHeroes, int oddIncrement)
	{
		selectedHeroes.Clear();
		
		var oddOffsetStart = offset;
		
		selectedHeroes.Add(ParseOddHeroSelectionStartEvenFirst(oddOffsetStart, oddIncrement));
		
		var evenOffset = oddOffsetStart + 2;
		
		for (; evenOffset <= oddOffsetStart + 12; evenOffset += 3)
		{
			selectedHeroes.Add(ParseEvenHeroSelectionStartEvenFirst(evenOffset, oddIncrement));
			var oddOffset = evenOffset + 1;
			selectedHeroes.Add(ParseOddHeroSelectionStartEvenFirst(oddOffset, oddIncrement));
		}

		selectedHeroes.Add(ParseEvenHeroSelectionStartEvenFirst(evenOffset, oddIncrement));

		return IsSuccessful(selectedHeroes);
	}

	private string ParseOddHeroSelectionStartEvenFirst(int oddOffset, int oddIncrement)
	{
		var desiredFirstByte = data[oddOffset] / (oddIncrement) * (oddIncrement);
		var desiredSecondByte = data[oddOffset + 1];
		
		var hero = heroElements.FirstOrDefault(h => h.OddByte1 == desiredFirstByte && h.OddByte2 == desiredSecondByte)?.Name;
		if (hero != null) return hero;
		
		if (randomOddBytes[0] == desiredFirstByte && randomOddBytes[1] == desiredSecondByte) return Random;
		if (desiredFirstByte == 0x00 && desiredSecondByte == 0x00) return Random;
		
		return Fail;
	}

	private string ParseEvenHeroSelectionStartEvenFirst(int evenOffset, int oddIncrement)
	{
		var desiredFirstByte = data[evenOffset];
		var desiredSecondByte = data[evenOffset + 1] % (oddIncrement);
		
		var hero = heroElements.FirstOrDefault(h => h.EvenByte1 == desiredFirstByte && h.EvenByte2 == desiredSecondByte)?.Name;
		if (hero != null) return hero;

		if (desiredFirstByte == randomEvenBytes[0] && desiredSecondByte == randomEvenBytes[1]) return Random;
		if (desiredFirstByte == 0x00 && desiredSecondByte == 0x00) return Random;

		return Fail;
	}

	private string ParseOddHeroSelectionStartOddFirst(int oddOffset, int evenIncrement)
	{
		var desiredFirstByte = data[oddOffset];
		var desiredSecondByte = data[oddOffset + 1] % (evenIncrement);
		
		var hero = heroElements.FirstOrDefault(h => h.OddByte1 == desiredFirstByte	&& h.OddByte2 == desiredSecondByte)?.Name;

		if (hero != null) return hero;

		if (randomOddBytes[0] == desiredFirstByte && randomOddBytes[1] == desiredSecondByte)return Random;
		if (desiredFirstByte == 0x00 && desiredSecondByte == 0x00) return Random;

		return Fail;
	}

	private string ParseEvenHeroSelectionStartOddFirst(int evenOffset, int evenIncrement)
	{
		var desiredFirstByte = data[evenOffset] / (evenIncrement) * (evenIncrement);
		var desiredSecondByte = data[evenOffset + 1];
		
		var hero = heroElements.FirstOrDefault(h => h.EvenByte1 == desiredFirstByte && h.EvenByte2 == desiredSecondByte)?.Name;
		if (hero != null) return hero;

		if (desiredFirstByte == randomEvenBytes[0] && desiredSecondByte == randomEvenBytes[1]) return Random;
		if (desiredFirstByte == 0x00 && desiredSecondByte == 0x00) return Random;

		return Fail;
	}
}