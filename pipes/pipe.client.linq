<Query Kind="Program">
  <Namespace>System.IO.Pipes</Namespace>
  <Namespace>Microsoft.Win32.SafeHandles</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Security.Principal</Namespace>
</Query>

async Task Main()
{
	using (var pipeClient = new NamedPipeClientStream(".", "testpipe", PipeDirection.InOut, PipeOptions.None, TokenImpersonationLevel.Impersonation))
	{
		pipeClient.Connect();

		var message = "HELLO";
		var bytes = Encoding.UTF8.GetBytes(message);

		while (true)
		{
			await pipeClient.WriteAsync(bytes, 0, bytes.Length);
			
			await Task.Delay(1000);
		}
		
		await pipeClient.DisposeAsync();
		
	}
}