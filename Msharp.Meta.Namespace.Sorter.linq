<Query Kind="Statements" />

/// <summary>
/// Moves entities into a given Namespace
/// </summary>

var done = false;

while(!done)
{
	var ns = Util.ReadLine("Enter a namespace to search for");
	
	if(ns == "EXIT") done = true;
	else
	{	
		try 
		{
			var msNs = Namespaces.Single(x => x.Name.Contains(ns));		
			var entityLookup = Util.ReadLine("Enter a term to find all entities to move into: {0}".FormatWith(msNs.Name));
			var entities = EntityTypes.Where(x => x.Name.Contains(entityLookup));			
			entities.Select(x => x.Name).Dump("Moving these entities into: {0}".FormatWith(msNs.Name));
						
			var response = Util.ReadLine("Press Y to move the entities or N to start again");
			
			if(response == "Y")
			{
				entities.Do(X => X.NamespaceEntity = msNs);
				SubmitChanges(ConflictMode.FailOnFirstConflict);
				done = true;
				"Entities have been moved, please use M# -> Project -> Restart M#, to see the changes".Dump();
			}			
		}
		catch(Exception e)
		{
			e.Dump();
		}
	}
}
