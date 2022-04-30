<Query Kind="Program" />

void Main()
{
	SettingsManager.EnumerateSettings(SettingsManager.EnumerateCharacters()).Dump();
}

public class SettingsManager
{
	private static readonly string directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, System.Environment.SpecialFolderOption.None), "SWTOR", "swtor", "settings");

	public static IEnumerable<Character> EnumerateCharacters()
	{
		foreach (var file in Directory.EnumerateFiles(directory, "*PlayerGUIState.ini"))
		{
			var settings = File.ReadAllLines(file).Where(line => line.Contains(" = ")).ToDictionary(x => x.Split("=", StringSplitOptions.TrimEntries)[0], x => x.Split("=", StringSplitOptions.TrimEntries)[1]);

			foreach (var key in settings.Keys)
				if (key.StartsWith("GroupFinder"))
					settings.Remove(key);

			yield return new Character { Name = file.Split('_')[1], FileInfo = new FileInfo(file), CharacterSettings = new CharacterSettings(settings, (settings) => Apply(settings)) };
		}
	}

	private static void Apply(IDictionary<string, string?> settings)
	{
		foreach (var character in EnumerateCharacters())
		{
			character.Set(settings.Where(kvp => !kvp.Key.StartsWith("GroupFinder_")).ToDictionary(x => x.Key, x => x.Value));
		}

		Console.WriteLine("COMPLETE");
	}

	private static void Apply(string key, string? value)
	{
		foreach (var character in EnumerateCharacters())
		{
			character.Set(key, value);
		}

		Console.WriteLine("COMPLETE");
	}

	public static IEnumerable<Setting> EnumerateSettings(IEnumerable<Character> characters)
	{
		foreach (var key in characters.SelectMany(x => x.CharacterSettings.Settings.Keys).Distinct())
		{
			Dictionary<string, List<CharacterSettingView>> dictionary = new Dictionary<string, List<CharacterSettingView>>();

			foreach (var character in characters)
			{
				if (!dictionary.TryGetValue(key, out var settingCharacters))
				{
					dictionary[key] = new List<CharacterSettingView>();
				}

				dictionary[key].Add(new CharacterSettingView(character, key, character.CharacterSettings.Settings.ContainsKey(key) ? character.CharacterSettings.Settings[key] : null, (key, value) => Apply(key, value)));
			}

			yield return new Setting(dictionary);
		}
	}
}

public class Setting : Dictionary<string, List<CharacterSettingView>>
{
	public Setting(IDictionary<string, List<CharacterSettingView>> settings) : base(settings)
	{

	}
}


public class Character
{
	public string Name { get; set; }
	public FileInfo FileInfo { get; set; }
	public CharacterSettings CharacterSettings { get; set; }

	public void Set(string key, string? value)
	{
		string[] lines = File.ReadAllLines(FileInfo.FullName);
		IDictionary<string, string> settings = lines.Where(line => line.Contains(" = ")).ToDictionary(x => x.Split(" = ", StringSplitOptions.TrimEntries)[0], x => x.Split(" = ", StringSplitOptions.TrimEntries)[1]);

		if (string.IsNullOrWhiteSpace(value))
		{
			settings.Remove(key);
		}
		else
		{
			settings[key] = value;
		}

		File.WriteAllLines(FileInfo.FullName, new[] { "[Settings]" }.Concat(settings.Select(setting => $"{setting.Key} = {setting.Value}")));
	}

	public void Set(IDictionary<string, string?> overwrite)
	{
		string[] lines = File.ReadAllLines(FileInfo.FullName);
		IDictionary<string, string> settings = lines.Where(line => line.Contains(" = ")).ToDictionary(x => x.Split(" = ", StringSplitOptions.TrimEntries)[0], x => x.Split(" = ", StringSplitOptions.TrimEntries)[1]);

		foreach (var key in settings.Keys)
		{
			if (overwrite.ContainsKey(key))
			{
				string? value = overwrite[key];

				if (value is not null) settings[key] = value;
				else settings.Remove(key);
			}
		}

		File.WriteAllLines(FileInfo.FullName, new[] { "[Settings]" }.Concat(settings.Select(setting => $"{setting.Key} = {setting.Value}")));
	}
}

public class CharacterSetting
{
	public string Key { get; set; }
	public string? Value { get; set; }
}

public class CharacterSettingView
{
	public Character Character { get; set; }
	public string Key { get; set; }
	public string? Value { get; set; }
	private Action<string, string?> action;

	public Hyperlinq Hyperlinq => new Hyperlinq(() => action(Key, Value), "Apply to all", false);

	public CharacterSettingView(Character character, string key, string? value, Action<string, string?> action)
	{
		this.Character = character;
		this.Key = key;
		this.action = action;
		this.Value = value;
	}
}

public class CharacterSettings
{
	public Hyperlinq Hyperlinq => new Hyperlinq(() => action(Settings), "Apply to all", false);
	public IDictionary<string, string?> Settings { get; set; }
	private Action<IDictionary<string, string?>> action;

	public CharacterSettings(IDictionary<string, string?> settings, Action<IDictionary<string, string?>> action)
	{
		Settings = settings;
		this.action = action;
	}
}