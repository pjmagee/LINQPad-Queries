<Query Kind="Program">
  <Namespace>System.Runtime.InteropServices</Namespace>
</Query>

[DllImport("user32.dll", ExactSpelling = true)]
public static extern long mouse_event(Int32 dwFlags, Int32 dx, Int32 dy, Int32 cButtons, Int32 dwExtraInfo);

[DllImport("user32.dll", ExactSpelling = true)]
public static extern void SetCursorPos(Int32 x, Int32 y);

[DllImport("user32.dll")]
static extern IntPtr GetForegroundWindow();

[DllImport("user32.dll")]
static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

[DllImport("user32.dll")]
static extern IntPtr GetActiveWindow();

static string GetActiveWindowTitle()
{
	const int nChars = 256;
	StringBuilder Buff = new StringBuilder(nChars);
	IntPtr handle = GetForegroundWindow();

	if (GetWindowText(handle, Buff, nChars) > 0)
	{
		return Buff.ToString();
	}
	return null;
}

static int X_A = 1329;
static int X_B = 1322;
static int Y = 715;


[Flags]
public enum MouseEventFlags
{
	LEFTDOWN = 0x00000002,
	LEFTUP = 0x00000004,
	MIDDLEDOWN = 0x00000020,
	MIDDLEUP = 0x00000040,
	MOVE = 0x00000001,
	ABSOLUTE = 0x00008000,
	RIGHTDOWN = 0x00000008,
	RIGHTUP = 0x00000010
}


public static void Main()
{
	while (true)
	{
		while (GetActiveWindowTitle().Equals("Star Wars™: The Old Republic™"))
		{
			mouse_event((int)MouseEventFlags.MOVE | (int)MouseEventFlags.ABSOLUTE, 31505, 31190, 0, 0);
			Thread.Sleep(500);

			mouse_event((int)MouseEventFlags.RIGHTDOWN, X_A, Y, 0, 0);
			Thread.Sleep(TimeSpan.FromSeconds(0.3));
			mouse_event((int)MouseEventFlags.RIGHTUP, X_A, Y, 0, 0);

			Thread.Sleep(1700);
			mouse_event((int)MouseEventFlags.MOVE | (int)MouseEventFlags.ABSOLUTE, 33923, 31190, 0, 0);
			Thread.Sleep(50);

			mouse_event((int)MouseEventFlags.RIGHTDOWN, X_B, Y, 0, 0);
			Thread.Sleep(TimeSpan.FromSeconds(0.3));
			mouse_event((int)MouseEventFlags.RIGHTUP, X_B, Y, 0, 0);

			Thread.Sleep(3400);
		}

		Thread.Sleep(10000);
	}
}