<Query Kind="Program">
  <Namespace>System.IO.Pipes</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

async Task Main()
{
	using (NamedPipeServerStream pipeServer = new NamedPipeServerStream("testpipe"))
	{
		await pipeServer.WaitForConnectionAsync();

		using (StreamReader sw = new StreamReader(pipeServer))
		{
			while (!sw.EndOfStream)
			{
				var result = sw.ReadLineAsync();

				result.Dump();
			}
		}
	}
}
