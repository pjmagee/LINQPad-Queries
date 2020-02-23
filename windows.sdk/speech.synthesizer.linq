<Query Kind="Program">
  <NuGetReference>Microsoft.Windows.SDK.Contracts</NuGetReference>
  <Namespace>Windows.Media.SpeechSynthesis</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

async Task Main()
{
	var talker = new SpeechSynthesizer();
	
	var stream = await talker.SynthesizeTextToStreamAsync("Hello");
	
	await stream.FlushAsync();
}