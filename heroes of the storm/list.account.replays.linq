<Query Kind="Expression" />

Directory
	.EnumerateFiles( 
		Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"Heroes of the Storm\Accounts"), 
		"*.StormReplay", 
		SearchOption.AllDirectories
	)