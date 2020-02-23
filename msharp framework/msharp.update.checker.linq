<Query Kind="Statements">
  <Reference>&lt;RuntimeDirectory&gt;\WPF\PresentationFramework.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\wpf\PresentationCore.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Xaml.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\wpf\WindowsBase.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\wpf\UIAutomationProvider.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Deployment.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\wpf\PresentationUI.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\wpf\System.Printing.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\wpf\ReachFramework.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\wpf\UIAutomationTypes.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\Accessibility.dll</Reference>
  <NuGetReference>MSharp.Framework</NuGetReference>
  <Namespace>System.Net</Namespace>
  <Namespace>System.Windows</Namespace>
</Query>

/// <summary> 
/// Checks the installed version of M# against the latest version
/// </summary>

string current = File.ReadAllLines(@"C:\Program Files (x86)\Geeks\M#4.5\IDE\Version.ini").First().Split('*').Trim().First().Trim();

using (var client = new WebClient())
{	
	var version = client.DownloadString("http://licensing.msharp.co.uk/LatestVersion.ashx?inst={0}".FormatWith(Util.GetPassword("msharp.key")).Trim());	
	var output = "Current: {0}, Latest Version: {1}".FormatWith(current, version).Dump();

	if(version != current)		
		MessageBox.Show(
			output,
			"M# UPDATE",
			MessageBoxButton.OK,
			MessageBoxImage.Exclamation,
			MessageBoxResult.OK,
			MessageBoxOptions.ServiceNotification);
}