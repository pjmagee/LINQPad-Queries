<Query Kind="Program">
  <Namespace>System.Runtime.InteropServices</Namespace>
</Query>

void Main()
{
	try
	{
		Process.GetProcessesByName("HeroesOfTheStorm_x64")[0].Kill();
	}
	catch (Exception e)
	{
		Thread.Sleep(1000);
	}
	
	try
	{
		using (var p = Process.Start(@"C:\Program Files (x86)\Battle.net\Battle.net.exe", "--game heroes"))
		{
			p.WaitForExit();
			
			Thread.Sleep(5000);
			
			SendEnterByHandle(Process.GetProcessesByName("Blizzard Battle.net")[0].MainWindowHandle);		
			
			while (!Process.GetProcessesByName("HeroesOfTheStorm_x64").Any())
			{
				Console.WriteLine("Launching...");
				Thread.Sleep(TimeSpan.FromSeconds(5));
			}
	
			if (Process.GetProcessesByName("HeroesOfTheStorm_x64").Any())
			{
				Console.WriteLine("Launched...");
				Thread.Sleep(TimeSpan.FromSeconds(5));
				
				Process.Start(@"G:\Heroes of the Storm\Support64\HeroesSwitcher_x64.exe", $"\"{Directory.EnumerateFiles(@"G:\replays\").OrderBy(x => Guid.NewGuid()).First()}\"");
				Thread.Sleep(TimeSpan.FromSeconds(5));			
			}
		}
	}
	catch (Exception e)
	{
	
	}
	finally
	{
	
	}
}

internal static class NativeMethods
{
	// Windows Event KeyDown
	public const int WM_KEYDOWN = 0x100;

	// Constant for Enter Key
	public const int VK_RETURN = 0x0D;

	[DllImport("user32.dll")]
	public static extern IntPtr GetForegroundWindow();

	[DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	public static extern int GetWindowTextLength(IntPtr hWnd);

	[DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

	[DllImport("User32.dll")]
	public static extern int SetForegroundWindow(IntPtr point);

	[DllImport("user32.dll", CharSet = CharSet.Auto)]
	public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, IntPtr lParam);
}

// Define other methods, classes and namespaces here
static public void SendEnterByHandle(IntPtr handle)
{
	if (handle == IntPtr.Zero)
	{		
		return;
	}
	
	NativeMethods.SendMessage(handle, NativeMethods.WM_KEYDOWN, NativeMethods.VK_RETURN, IntPtr.Zero);
}