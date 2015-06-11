<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\System.Web.Extensions.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.ServiceModel.Activation.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Runtime.Serialization.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Web.ApplicationServices.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Web.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Configuration.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.ServiceModel.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Web.Services.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Windows.Forms.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Data.Services.Client.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Data.Services.Design.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Data.Entity.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Design.dll</Reference>
  <Namespace>System.Globalization</Namespace>
  <Namespace>System.Net</Namespace>
  <Namespace>System.Web.Script.Serialization</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

 ///<summary>
/// Allows you to programmatically search M# with unique LINQ based search critera in order to find specific modules. 
/// Results provide you with a direct link to open the associated module in M#.
///</summary>
///<author>
/// Patrick Magee
///</author>
void Main()
{
	// MenuModules.Where(mm => mm.MenuItems.Any(mi => mi.Text.Contains("fa fa-"))).DumpBrowseable();
	// ListModules.DumpBrowseable();
	// ViewModules.DumpBrowseable();
	// FormModules.DumpBrowseable();
	// GenericModules.DumpBrowseable();
	// MenuItems.DumpBrowseable();
	// ModuleCodeExtensions.Where(mce => mce.Code.Contains("SEARCH TERM HERE")).DumpBrowseable();  

    Task.Run(() => 
    {      
        var term = string.Empty;
        while((term = Util.ReadLine("Enter a search term:")) != "EXIT")
        {
            Util.AutoScrollResults = true;
            ModuleCodeExtensions.Where(mce => mce.Code.Contains(term)).DumpBrowseable();
            MenuModules.Where(mm => mm.MenuItems.Any(mi => mi.Text.Contains(term))).DumpBrowseable();
            GenericModules.Where(gm => gm.Markup.Contains(term)).DumpBrowseable();     
        } 
    });
}

public static class Extensions 
{
	public static string GetProjectUrl()
	{
		var serialiser = new JavaScriptSerializer();

		using(var client = new WebClient())
		{
			var json = client.DownloadString("http://localhost:3020/getprojects");	
			var projects = serialiser.Deserialize<MSharpProject[]>(json);			
			var currentProject = projects.Single(p => Util.CurrentDataContext.Connection.Database.Contains(p.Name));
			return currentProject.MSharpUrl;
		}
		
		throw new InvalidOperationException("Connection does not match any open M# projects");
	}

	public static string GetPascalCase(this string name)
	{
		return Regex.Replace(name, @"^\w|_\w", (match) => match.Value.Replace("_", "").ToUpper());
	}

	public static IEnumerable<T> DumpBrowseable<T>(this IEnumerable<T> objects, string title = null)
	{
		if(!objects.GetType().AssemblyQualifiedName.Contains("TypedDataContext"))
		{
			throw new Exception("Only TypedDataContext entities are supported.");
		}
	
		var objectsToDump = objects.Select(e => {
			
			string moduleName = null;
			
			if(e is MenuModules)
			{
				var entity = e as MenuModules;					
				moduleName = entity.Module.Name;
			}
			else if(e is ViewModules)
			{
				var entity = e as ViewModules;
				moduleName = entity.Module.Name;
			}
			else if(e is ListModules)
			{
				var entity = e as ListModules;
				moduleName = entity.ViewModules.Module.Name;
			}
			else if(e is Modules)
			{
				var entity = e as Modules;
				moduleName = entity.Name;
			}
			else if(e is GenericModules)
			{
				var entity = e as GenericModules;
				moduleName = entity.Module.Name;		
			}
			else if(e is FormModules)
			{
				var entity = e as FormModules;
				moduleName = entity.Module.Name;
			}
			else if(e is MenuItems)
			{
				var entity = e as MenuItems;
				moduleName = entity.MenuMenuModules.Module.Name;
			}
			else if(e is ModuleCodeExtensions)
			{
				var entity = e as ModuleCodeExtensions;
				moduleName = entity.ModuleEntity.Name;
			}
			else 
			{
				"Unhandled TypedDataContext Entity: {0}".FormatWith(e.GetType().FullName).Dump();
			}
			
			if(moduleName.HasValue())
			{
				var moduleFileName = moduleName.Split(' ')
										   .Select(i => i.GetPascalCase())
										   .ToString(string.Empty)
										   .Replace(" ", string.Empty)
										   .Select(c => c.IsLetter() ? c : '_')
										   .ToString(string.Empty)
										   .WithSuffix(".ascx");		
			
				moduleFileName = moduleFileName.Replace("Form:", "Form_");
				moduleFileName = moduleFileName.Replace("View:", "View_");
				moduleFileName = moduleFileName.Replace("List:", "List_");
				moduleFileName = moduleFileName.Replace("_.ascx", ".ascx");
				
				string url = "{0}/?moduleFileName={1}".FormatWith(GetProjectUrl(), moduleFileName);
				
				return new 
				{ 
					Module = new Hyperlinq(() => { e.Dump(); Util.AutoScrollResults = true; }, "Inspect result"),
					OpenInMSharp = new Hyperlinq(url, "{0}".FormatWith(moduleName))
				};
			}
			else
			{
				return null;
			}
		});
		
		objectsToDump.Where(o => o != null).Dump(title ?? typeof(T).FullName, 1);
		
		return objects;
	}
}

public class MSharpProject 
{	
	public string MSharpUrl { get; set; }
	public string Name { get; set; }
	public string Path { get; set; }
	public int Port { get; set; }
	public string RootUrl { get; set; }
}