<Query Kind="Program">
  <NuGetReference>Heroes.ReplayParser</NuGetReference>
  <Namespace>Heroes.ReplayParser</Namespace>
  <Namespace>Heroes.ReplayParser.MPQFiles</Namespace>
</Query>

void Main()
{
	var results  = Directory
		.EnumerateFiles(@"G:\replays", "*.StormReplay", SearchOption.AllDirectories)
		.Concat(Directory.EnumerateFiles(@"C:\Users\patri\AppData\Local\Temp\HeroesReplay\"))
		.OrderBy(x => File.GetCreationTime(x))		
		// .Where(x => x.Contains("23940808"))
		.Take(20)
		.AsParallel()
		.Select(file =>
		{
			return new 
			{ 
				replay = DataParser.ParseReplay(File.ReadAllBytes(file), new ParseOptions { AllowPTR = false, IgnoreErrors = true, ShouldParseEvents = true,  ShouldParseMessageEvents = false, ShouldParseUnits = true, ShouldParseMouseEvents = false, ShouldParseDetailedBattleLobby = false, ShouldParseStatistics = true }),
				path = file,
			};
		})
		.Where(r => r.replay.Item1 != Heroes.ReplayParser.DataParser.ReplayParseResult.Exception && r.replay.Item1 != Heroes.ReplayParser.DataParser.ReplayParseResult.Incomplete)	
		.Select(r => new { r.path, replay = r.replay.Item2 });
		
	//var replay = replays.FirstOrDefault();
	// var player = replay.Players.Single(x => x.HeroId == "Malthael");
	// replay.GameEvents.Where(ge => ge.player == player && ge.eventType == GameEventType.CCmdEvent && ge.TimeSpan.Minutes == 2 && ge.TimeSpan.Seconds == 25 ).Select(x => x.data).Dump(depth: 1);
	
	
	
	foreach(var result in results.ToList())
	{
		var taunts = result.replay.GameEvents.Where(x => result.replay.IsTaunt(x)).GroupBy(e => e.player);

		foreach (var grp in taunts)
		{
			var spam = grp.GroupBy(g => g.TimeSpan);

			foreach (var b in spam.Where(g => g.Count() > 0))
			{
				new { grp.Key.HeroId, b.Key, taunts = b.Count(), file = result.path }.Dump("taunt scum");
			}
		}


		// BSTEPPING
		var commands = result.replay.GameEvents.Where(x => result.replay.IsHearthStone(x)).GroupBy(e => e.player);
		
		foreach(var grp in commands)
		{
			var spam = grp.GroupBy(g => g.TimeSpan);
			
			foreach(var b in spam.Where(g => g.Count() > 3))
			{
				new { grp.Key.HeroId, b.Key, bsteps = b.Count(), file = result.path }.Dump("bstepping scum");
			}
			
		}
	}
	//replay.TrackerEvents.Where(te => te.TrackerEventType == Heroes.ReplayParser.MPQFiles.ReplayTrackerEvents.TrackerEventType.StatGameEvent && )
	// replays.SelectMany(x => x.TrackerEvents).Where(te => te.Data.dictionary[0].blobText == "GatesOpen").Dump();
	// replays.SelectMany(x => x.GameEvents.Where(e => x.IsHearthStone(e))).GroupBy(x => x.player).Where(g => g.GroupBy(a => a.TimeSpan).Where(bstep => bstep.Count() > 2).Any() )  .Dump();
	//replay.GameEvents.Where(te => te.player == replay.Players.SingleOrDefault(p => p.HeroId == "Zuljin") && (int)te.eventType == 10)	
	//	.Select(x => new { x.data }).Dump();
	// replay.TrackerEvents.Where(te => te.TrackerEventType == (Heroes.ReplayParser.MPQFiles.ReplayTrackerEvents.TrackerEventType)5 ).Dump();
	// replays.SelectMany(r => r.Units.Where(x => x.Group == Heroes.ReplayParser.Unit.UnitGroup.Structures))
	// replays.SelectMany(r => r.Units.Where(unit => unit.Name.Contains("WatchTower"))).GroupBy(x => x.Name).Select(x => x.Take(3)).Dump(2);
	// replays.Select(x => x.TrackerEvents.Where(te => te.TrackerEventType == Heroes.ReplayParser.MPQFiles.ReplayTrackerEvents.TrackerEventType.StatGameEvent || te.TrackerEventType == Heroes.ReplayParser.MPQFiles.ReplayTrackerEvents.TrackerEventType.UnitOwnerChangeEvent)).Dump();
	// replays.Select(x => x.GameEvents.Where(ge => ge.eventType == GameEventType.CCmdEvent && ge.
	
	// replays.SelectMany(r => r.TrackerEvents.Where(te => te.TrackerEventType == ReplayTrackerEvents.TrackerEventType.StatGameEvent)).Where(te => te.Data.dictionary[0].blobText == "GatesOpen").Dump();
}

// Define other methods, classes and namespaces here

public static class Extensions 
{
	public static int GetAbilityLink(this TrackerEventStructure structure)
	{
		return Convert.ToInt32(structure?.array[1]?.array[0]?.unsignedInt ?? 0); // m_abilLink
	}

	public static int GetAbilityCmdIndex(this TrackerEventStructure trackerEvent)
	{
		return Convert.ToInt32(trackerEvent.array[1]?.array[1]?.unsignedInt ?? 0); // m_abilCmdIndex
	}

	public static bool IsTaunt(this Replay replay, GameEvent gameEvent)
	{
		return gameEvent.eventType == GameEventType.CCmdEvent &&

			   (gameEvent.data.GetAbilityLink() == 19 && replay.ReplayBuild < 68740 || gameEvent.data.GetAbilityLink() == 22 && replay.ReplayBuild >= 68740) &&
			   gameEvent.data.GetAbilityCmdIndex() == 4;
	}

	public static bool IsDance(this Replay replay, GameEvent gameEvent)
	{
		return gameEvent.eventType == GameEventType.CCmdEvent &&

			   (gameEvent.data.GetAbilityLink() == 19 && replay.ReplayBuild < 68740 || gameEvent.data.GetAbilityLink() == 22 && replay.ReplayBuild >= 68740) &&
			   gameEvent.data.GetAbilityCmdIndex() == 3;
	}

	public static bool IsHearthStone(this Replay replay, GameEvent gameEvent)
	{
		return

			gameEvent.eventType == GameEventType.CCmdEvent &&

			(replay.ReplayBuild < 61872 && gameEvent.data.GetAbilityLink() == 200 ||
			 replay.ReplayBuild >= 61872 && replay.ReplayBuild < 68740 && gameEvent.data.GetAbilityLink() == 119 ||
			 replay.ReplayBuild >= 68740 && replay.ReplayBuild < 70682 && gameEvent.data.GetAbilityLink() == 116 ||
			 replay.ReplayBuild >= 70682 && replay.ReplayBuild < 77525 && gameEvent.data.GetAbilityLink() == 112 ||
			 replay.ReplayBuild >= 77525 && gameEvent.data.GetAbilityLink() == 114);
	}
}