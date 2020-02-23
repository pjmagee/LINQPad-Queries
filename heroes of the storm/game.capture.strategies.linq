<Query Kind="Program">
  <Namespace>System.Drawing</Namespace>
  <Namespace>System.Drawing.Imaging</Namespace>
  <Namespace>System.Runtime.InteropServices</Namespace>
</Query>

void Main()
{

	var hots = Process.GetProcessesByName("HeroesOfTheStorm_x64")[0].MainWindowHandle;
	var bnet = Process.GetProcessesByName("Battle.net")[0].MainWindowHandle;
	
	// ScreenCapture.Capabilities(hots);
	
	while (true)
	{
		//		using(var bitmap = ScreenCapture.PrintWindow(null, bnet))
		//		{
		//			bitmap.Dump("PrintWindow");
		//		}
		//
		//		using (var bitmap = ScreenCapture.CopyFromScreen(null, bnet))
		//		{
		//			bitmap.Dump("CopyFromScreen");
		//		}			

		//		using (var bitmap = ScreenCapture.PrintWindow(new Rectangle(1920 / 2 - 50, 0, 100, 50), hots))
		//		{
		//			bitmap.Dump("PrintWindow");
		//		}
		//
		//		using (var bitmap = ScreenCapture.CopyFromScreen(new Rectangle(1920 / 2 - 50, 0, 100, 50), hots))
		//		{
		//			bitmap.Dump("CopyFromScreen");
		//		}

		//using (var bitmap = ScreenCapture.CopyFromScreen(null, bnet, hots))
		//{
		//	bitmap.Dump(bnet.ToString());
		//}
		
		Thread.Sleep(2000);

		using (var bitmap = ScreenCapture.CopyFromScreen(null, bnet))
		{
			bitmap.Dump(bnet.ToString());
		}

		using (var bitmap = ScreenCapture.CopyFromScreen(null, hots))
		{
			bitmap.Dump(bnet.ToString());
		}


	}
}

public enum DeviceCap
{
	/// <summary>
	/// Device driver version
	/// </summary>
	DRIVERVERSION = 0,
	/// <summary>
	/// Device classification
	/// </summary>
	TECHNOLOGY = 2,
	/// <summary>
	/// Horizontal size in millimeters
	/// </summary>
	HORZSIZE = 4,
	/// <summary>
	/// Vertical size in millimeters
	/// </summary>
	VERTSIZE = 6,
	/// <summary>
	/// Horizontal width in pixels
	/// </summary>
	HORZRES = 8,
	/// <summary>
	/// Vertical height in pixels
	/// </summary>
	VERTRES = 10,
	/// <summary>
	/// Number of bits per pixel
	/// </summary>
	BITSPIXEL = 12,
	/// <summary>
	/// Number of planes
	/// </summary>
	PLANES = 14,
	/// <summary>
	/// Number of brushes the device has
	/// </summary>
	NUMBRUSHES = 16,
	/// <summary>
	/// Number of pens the device has
	/// </summary>
	NUMPENS = 18,
	/// <summary>
	/// Number of markers the device has
	/// </summary>
	NUMMARKERS = 20,
	/// <summary>
	/// Number of fonts the device has
	/// </summary>
	NUMFONTS = 22,
	/// <summary>
	/// Number of colors the device supports
	/// </summary>
	NUMCOLORS = 24,
	/// <summary>
	/// Size required for device descriptor
	/// </summary>
	PDEVICESIZE = 26,
	/// <summary>
	/// Curve capabilities
	/// </summary>
	CURVECAPS = 28,
	/// <summary>
	/// Line capabilities
	/// </summary>
	LINECAPS = 30,
	/// <summary>
	/// Polygonal capabilities
	/// </summary>
	POLYGONALCAPS = 32,
	/// <summary>
	/// Text capabilities
	/// </summary>
	TEXTCAPS = 34,
	/// <summary>
	/// Clipping capabilities
	/// </summary>
	CLIPCAPS = 36,
	/// <summary>
	/// Bitblt capabilities
	/// </summary>
	RASTERCAPS = 38,
	/// <summary>
	/// Length of the X leg
	/// </summary>
	ASPECTX = 40,
	/// <summary>
	/// Length of the Y leg
	/// </summary>
	ASPECTY = 42,
	/// <summary>
	/// Length of the hypotenuse
	/// </summary>
	ASPECTXY = 44,
	/// <summary>
	/// Shading and Blending caps
	/// </summary>
	SHADEBLENDCAPS = 45,

	/// <summary>
	/// Logical pixels inch in X
	/// </summary>
	LOGPIXELSX = 88,
	/// <summary>
	/// Logical pixels inch in Y
	/// </summary>
	LOGPIXELSY = 90,

	/// <summary>
	/// Number of entries in physical palette
	/// </summary>
	SIZEPALETTE = 104,
	/// <summary>
	/// Number of reserved entries in palette
	/// </summary>
	NUMRESERVED = 106,
	/// <summary>
	/// Actual color resolution
	/// </summary>
	COLORRES = 108,

	// Printing related DeviceCaps. These replace the appropriate Escapes
	/// <summary>
	/// Physical Width in device units
	/// </summary>
	PHYSICALWIDTH = 110,
	/// <summary>
	/// Physical Height in device units
	/// </summary>
	PHYSICALHEIGHT = 111,
	/// <summary>
	/// Physical Printable Area x margin
	/// </summary>
	PHYSICALOFFSETX = 112,
	/// <summary>
	/// Physical Printable Area y margin
	/// </summary>
	PHYSICALOFFSETY = 113,
	/// <summary>
	/// Scaling factor x
	/// </summary>
	SCALINGFACTORX = 114,
	/// <summary>
	/// Scaling factor y
	/// </summary>
	SCALINGFACTORY = 115,

	/// <summary>
	/// Current vertical refresh rate of the display device (for displays only) in Hz
	/// </summary>
	VREFRESH = 116,
	/// <summary>
	/// Vertical height of entire desktop in pixels
	/// </summary>
	DESKTOPVERTRES = 117,
	/// <summary>
	/// Horizontal width of entire desktop in pixels
	/// </summary>
	DESKTOPHORZRES = 118,
	/// <summary>
	/// Preferred blt alignment
	/// </summary>
	BLTALIGNMENT = 119
}

public enum TernaryRasterOperations : uint
{
	/// <summary>dest = source</summary>
	SRCCOPY = 0x00CC0020,
	/// <summary>dest = source OR dest</summary>
	SRCPAINT = 0x00EE0086,
	/// <summary>dest = source AND dest</summary>
	SRCAND = 0x008800C6,
	/// <summary>dest = source XOR dest</summary>
	SRCINVERT = 0x00660046,
	/// <summary>dest = source AND (NOT dest)</summary>
	SRCERASE = 0x00440328,
	/// <summary>dest = (NOT source)</summary>
	NOTSRCCOPY = 0x00330008,
	/// <summary>dest = (NOT src) AND (NOT dest)</summary>
	NOTSRCERASE = 0x001100A6,
	/// <summary>dest = (source AND pattern)</summary>
	MERGECOPY = 0x00C000CA,
	/// <summary>dest = (NOT source) OR dest</summary>
	MERGEPAINT = 0x00BB0226,
	/// <summary>dest = pattern</summary>
	PATCOPY = 0x00F00021,
	/// <summary>dest = DPSnoo</summary>
	PATPAINT = 0x00FB0A09,
	/// <summary>dest = pattern XOR dest</summary>
	PATINVERT = 0x005A0049,
	/// <summary>dest = (NOT dest)</summary>
	DSTINVERT = 0x00550009,
	/// <summary>dest = BLACK</summary>
	BLACKNESS = 0x00000042,
	/// <summary>dest = WHITE</summary>
	WHITENESS = 0x00FF0062,
	/// <summary>
	/// Capture window as seen on screen.  This includes layered windows
	/// such as WPF windows with AllowsTransparency="true"
	/// </summary>
	CAPTUREBLT = 0x40000000
}

public class ScreenCapture
{
	[DllImport("gdi32.dll")]
	private static extern int GetDeviceCaps(IntPtr hdc, DeviceCap nIndex);

	[DllImport("user32.dll")]
	private static extern IntPtr GetForegroundWindow();

	[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
	private static extern IntPtr GetDesktopWindow();

	[DllImport("user32.dll")]
	private static extern bool SetForegroundWindow(IntPtr hWnd);

	[DllImport("user32.dll")]
	private static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);

	[StructLayout(LayoutKind.Sequential)]
	private struct Rect
	{
		public int Left;
		public int Top;
		public int Right;
		public int Bottom;
	}

	[DllImport("gdi32.dll")]
	private static extern bool BitBlt(IntPtr hdcDest, int nxDest, int nyDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, TernaryRasterOperations dwRop);

	[DllImport("User32.dll", SetLastError = true)]
	static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);

	[DllImport("user32.dll", SetLastError = true)]
	private static extern IntPtr GetClientRect(IntPtr hWnd, out Rect rect);

	[DllImport("user32.dll", SetLastError = true)]
	private static extern IntPtr GetWindowRect(IntPtr hWnd, out Rect rect);
	
	public static void Capabilities(IntPtr handle)
	{
		using(var g = Graphics.FromHwnd(handle))
		{
			IntPtr hdc = g.GetHdc();

			foreach (DeviceCap cap in Enum.GetValues(typeof(DeviceCap)))
			{
				int result = GetDeviceCaps(hdc, cap);
				Console.WriteLine(string.Format("{0}: {1}", cap, result));
			}
		}
	}

	public static Bitmap BitBlt(Rectangle? region, IntPtr handle)
	{
		Rectangle bounds;

		if (region == null)
		{
			GetClientRect(handle, out Rect rect);
			bounds = new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
		}
		else
		{
			bounds = region.Value;
		}

		using (var source = Graphics.FromHwnd(handle))
		{
			var bitmap = new Bitmap(bounds.Width, bounds.Height, source);

			using (var destination = Graphics.FromImage(bitmap))
			{
				IntPtr sdc = source.GetHdc();
				IntPtr ddc = destination.GetHdc();

				BitBlt(ddc, 0, 0, bounds.Width, bounds.Height, sdc, bounds.Left, bounds.Top, TernaryRasterOperations.SRCCOPY);

				source.ReleaseHdc(sdc);
				destination.ReleaseHdc(ddc);
			}

			return bitmap;
		}
	}

	public static Bitmap PrintWindow(Rectangle? region, IntPtr handle)
	{
		Rectangle bounds;

		GetWindowRect(handle, out Rect rect);
		bounds = new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);

		Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format32bppArgb);

		using (Graphics graphics = Graphics.FromImage(bitmap))
		{
			IntPtr deviceContext = graphics.GetHdc();
			PrintWindow(handle, deviceContext, 0);
			graphics.ReleaseHdc(deviceContext);

			if (region != null)
			{
				using (bitmap)
				{
					return bitmap.Clone(region.Value, bitmap.PixelFormat);
				}
			}
			else
			{
				return bitmap;
			}
		}
	}

	public static Bitmap CopyFromScreen(Rectangle? region, IntPtr handle)
	{		
		SetForegroundWindow(handle);
		
		Rectangle bounds;

		if (region == null)
		{
			GetWindowRect(handle, out Rect rect);
			bounds = new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
		}
		else
		{
			bounds = region.Value;
		}

		Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height);

		using (var graphics = Graphics.FromImage(bitmap))
		{
			graphics.CopyFromScreen(bounds.Location, Point.Empty, bounds.Size);
		}

		return bitmap;
	}
}