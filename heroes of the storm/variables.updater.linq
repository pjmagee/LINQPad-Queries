<Query Kind="Statements" />

var path = @"C:\Users\patri\Documents\Heroes of the Storm\Variables.txt";
var lines = await File.ReadAllLinesAsync(path);
var values = lines.ToDictionary(keySelector => keySelector.Split('=')[0], elementSelector => elementSelector.Split('=')[1]).Dump();

values["replayinterface"] = "AhliObs 0.66.StormInterface";

var ordered = values.OrderBy(kv => kv.Key).Select(pair => $"{pair.Key}={pair.Value}");

await File.WriteAllLinesAsync(path, ordered);