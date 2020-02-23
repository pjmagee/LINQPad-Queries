<Query Kind="Program" />

void Main()
{
	
	
}

/// <summary>
/// Call this method at the beginning of a LinqPad Script if you have added a reference to your [#Project Name#].Model.dll 
/// This will then hook into the ADO.NET data providers and apply the connection string from the selected connection in linqpad
/// </summary>
public static class Extensions
{	
	static Extensions()
	{

	}

		
	public static void ConfigureMSharpRuntime(string file = "*.Model.dll")
	{			
		var model = Path.Combine(Path.GetTempPath(), "LINQPad").AsDirectory().GetFiles(file, SearchOption.AllDirectories).WithMax(x => x.CreationTime);		
		var assembly = Assembly.LoadFile(model.FullName);
		
		Type dataProviderFactoryType = assembly.GetType("AppData.AdoDotNetDataProviderFactory", throwOnError: true);
			
		if(Util.CurrentDataContext == null)
			MessageBox.Show("Select a Database connection for: {0}".FormatWith(dataProviderFactoryType.Assembly.FullName));
			
		var factoriesField = typeof(Database).GetField("AssemblyProviderFactories", BindingFlags.NonPublic | BindingFlags.Static);
		var assemblyProviderFactories = (Dictionary<Assembly, IDataProviderFactory>)factoriesField.GetValue(null);
		
		var config = new DataProviderFactoryInfo
				{
					Assembly = dataProviderFactoryType.Assembly,
					AssemblyName = dataProviderFactoryType.Assembly.FullName,
					ConnectionStringKey = "AppDatabase",
					ConnectionString = Util.CurrentDataContext.Connection.ConnectionString,
					ProviderFactoryType = dataProviderFactoryType.FullName,
				};
				
		var dataProviderFactory = (IDataProviderFactory)Activator.CreateInstance(dataProviderFactoryType, config);
		assemblyProviderFactories[dataProviderFactoryType.Assembly] = dataProviderFactory;
	}	
}
