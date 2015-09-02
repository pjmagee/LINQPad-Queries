<Query Kind="Program" />

void Main()
{
	var helloWorld = new HelloWorld();

	// helloWorld = null;
}

public class HelloWorld : IDisposable
{
	public HelloWorld()
	{

	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	public virtual void Dispose(bool disposing)
	{
		if (disposing)
		{

		}
	}

	~HelloWorld()
	{
		Console.WriteLine("Finalizer called.");
	}
}

// Define other methods and classes here
