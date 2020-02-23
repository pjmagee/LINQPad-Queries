<Query Kind="Program">
  <NuGetReference Version="1.2.2">Heroes.ReplayParser</NuGetReference>
  <NuGetReference>Microsoft.Windows.SDK.Contracts</NuGetReference>
  <Namespace>System.ComponentModel</Namespace>
  <Namespace>System.Drawing</Namespace>
  <Namespace>System.Drawing.Imaging</Namespace>
  <Namespace>System.Globalization</Namespace>
  <Namespace>System.Runtime.InteropServices</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>Windows.Globalization</Namespace>
  <Namespace>Windows.Graphics.Imaging</Namespace>
  <Namespace>Windows.Media.Ocr</Namespace>
  <Namespace>Windows.Storage.Streams</Namespace>
</Query>

Process Game => Process.GetProcessesByName("HeroesOfTheStorm_x64")[0];
Process Launcher => Process.GetProcessesByName("Battle.net")[0];

int HWND_BOTTOM = 1;
uint SWP_ASYNCWINDOWPOS = 0x4000;

TimeSpan offset = TimeSpan.FromSeconds(0);

public async Task Timer()
{
//	var bytes = File.ReadAllBytes(@"C:\Users\patri\Documents\Heroes of the Storm\Accounts\124484934\2-Hero-1-8981684\Replays\Multiplayer\2020-01-02 00.04.01 Tomb of the Spider Queen.StormReplay");
//
//	var (parseResult, replay) = Heroes.ReplayParser.DataParser.ParseReplay(bytes, false, false);
//
//	var offset = (replay.Frames / 16).Dump();

	var ocr = OcrEngine.TryCreateFromLanguage(new Language("en"));

	while (true)
	{
		GetClientRect(Game.MainWindowHandle, out var window);
		var windowRect = Rectangle.FromLTRB(window.Left, window.Top, window.Right, window.Bottom);
		var timerRect = new Rectangle(new Point(windowRect.Width / 2 - 40, 25), new Size(100, 50));

		var bitmap = this.TryGetRenderScreenshotRegion(Game, timerRect);

		if (bitmap != null)
		{
			using (bitmap)
			{
				bitmap.Dump();
				var result = await ocr.RecognizeAsync(await GetSoftwareBitmapAsync(bitmap));
				var timespan = TryParseTimeSpan(result, topTimer: true);

				if (timespan != null)
				{
					timespan.Dump("time");
					// RemoveNegativeOffset(timespan.Value).Dump("time");
				}

			}
		}

		await Task.Delay(500);

	}
}

public static TimeSpan RemoveNegativeOffset(TimeSpan timer)
{
	var bottomTimer = timer.Add(TimeSpan.FromSeconds(timer.Seconds + 610) / 16).Duration();
	return new TimeSpan(bottomTimer.Days, bottomTimer.Hours, bottomTimer.Minutes, bottomTimer.Seconds, milliseconds: 0);
}

private TimeSpan? TryParseTimeSpan(OcrResult ocrResult, bool topTimer)
{
	try
	{
		var time = ocrResult.Lines[0].Text;
		var segments = time.Split(':');

		TimeSpan timeSpan = segments.Length switch
		{
			3 => TimeSpan.ParseExact(time, "hh\\:mm\\:ss", CultureInfo.InvariantCulture),
			2 => segments[0][0] == '-' ? TimeSpan.ParseExact(time, "\\-mm\\:ss", CultureInfo.CurrentCulture, TimeSpanStyles.AssumeNegative) : TimeSpan.ParseExact(time, "mm\\:ss", CultureInfo.InvariantCulture),
			_ => throw new Exception($"Unhandled segments: {segments.Length}")
		};

		return timeSpan;
	}
	catch (Exception e)
	{
		e.Dump("could not parse timespan");
	}

	return null;
}

public void Resize()
{
	if (MoveWindow(Game.MainWindowHandle, 0, 0, 1920, 1080, true))
	{
		Console.WriteLine("success");
	}
	else
	{
		Console.WriteLine("fail");
	}
}

public async Task Launch()
{
	var ocr = OcrEngine.TryCreateFromLanguage(new Windows.Globalization.Language("en"));

	while (true)
	{
		var screen = TryGetScreenshot(Launcher);

		if (screen != null)
		{
			using (screen)
			{
				using (var bitmap = await GetSoftwareBitmapAsync(screen))
				{
					screen.Dump();
					var result = await ocr.RecognizeAsync(bitmap);
					result.Lines.Where(line => line.Text == "PLAY" || line.Text == "Game is running").Dump();
					result.Lines.Dump(collapseTo: 0);
				}
			}
		}

		await Task.Delay(5000);
	}
}

public async Task Main()
{
	// Resize();
	await Timer();

	// await EndScreen();
	// await Launch();
	// await LoadingScreens();
}

async Task EndScreen()
{
	var ocr = OcrEngine.TryCreateFromLanguage(new Windows.Globalization.Language("en"));

	while (true)
	{
		GetWindowRect(Game.MainWindowHandle, out RECT rect);


		var (parseResult, replay) = Heroes.ReplayParser.DataParser.ParseReplay(File.ReadAllBytes(@"C:\Users\patri\Documents\Heroes of the Storm\Accounts\124484934\2-Hero-1-8981684\Replays\Multiplayer\replay-23190561.StormReplay"), false, false);
		var bitmap = TryGetRenderScreenshotRegion(Game, Rectangle.FromLTRB(rect.Left, rect.Top, rect.Right, rect.Bottom));

		((Bitmap)bitmap.Clone(new Rectangle(0, 0, bitmap.Width - 1, bitmap.Height - 1), PixelFormat.Format16bppGrayScale)).Dump();

		using (bitmap)
		{
			// var result = await ocr.RecognizeAsync(await GetSoftwareBitmapAsync(bitmap));


			var matchAwards = replay.Players.SelectMany(p => p.ScoreResult.MatchAwards).ToList();
			matchAwards.Dump();

		}

		Thread.Sleep(10000);
	}

}


public async Task LoadingScreens()
{
	var ocr = OcrEngine.TryCreateFromLanguage(new Windows.Globalization.Language("en"));

	while (true)
	{
		foreach (var bitmap in Directory.GetFiles(@"G:\Export\Loading").Select(Bitmap.FromFile).Cast<Bitmap>())
		{
			using (bitmap)
			{
				using (var softwareBitmap = await GetSoftwareBitmapAsync(bitmap))
				{
					var result = await ocr.RecognizeAsync(softwareBitmap);

					if (result != null && result.Lines.Any(line => line.Text.Contains("WELCOME")))
					{
						result.Dump("FOUND", collapseTo: 0);
					}
					else
					{
						bitmap.Dump("NOT FOUND", collapseTo: 0);
					}
				}
			}
		}
	}

}

public Bitmap? TryGetScreenshot(Process p)
{
	var handle = p.MainWindowHandle;

	if (GetClientRect(handle, out RECT region))
	{
		Rectangle rectangle = Rectangle.FromLTRB(region.Left, region.Top, region.Right, region.Bottom);

		var bmp = new Bitmap(rectangle.Width, rectangle.Height, PixelFormat.Format32bppArgb);

		using (Graphics graphics = Graphics.FromImage(bmp))
		{
			IntPtr hdcBitmap = graphics.GetHdc();

			if (PrintWindow(handle, hdcBitmap, 0))
				return bmp;

			graphics.ReleaseHdc(hdcBitmap);
		}
	}

	return null;
}

protected async Task<OcrResult?> TryGetOcrResult(OcrEngine engine, Bitmap bitmap, params string[] lines)
{
	if (bitmap == null) return null;

	using (SoftwareBitmap softwareBitmap = await GetSoftwareBitmapAsync(bitmap))
	{
		OcrResult result = await engine.RecognizeAsync(softwareBitmap);

		if (result != null && lines.All(line => result.Lines.Any(ocrLine => ocrLine.Text.Contains(line, StringComparison.OrdinalIgnoreCase))))
		{
			return result;
		}

		return null;
	}
}

protected Bitmap? TryGetRenderScreenshotRegion(Process p, Rectangle timer)
{
	IntPtr deviceContext = IntPtr.Zero;
	IntPtr compatibleDeviceContext = IntPtr.Zero;
	IntPtr bitmap = IntPtr.Zero;
	IntPtr oldBitmap = IntPtr.Zero;

	try
	{


		deviceContext = GetDC(p.MainWindowHandle);
		compatibleDeviceContext = CreateCompatibleDC(deviceContext);
		bitmap = CreateCompatibleBitmap(deviceContext, timer.Width, timer.Height);

		oldBitmap = SelectObject(compatibleDeviceContext, bitmap);

		if (BitBlt(compatibleDeviceContext, nxDest: 0, nyDest: 0, nWidth: timer.Width, nHeight: timer.Height, hdcSrc: deviceContext, nXSrc: timer.X, nYSrc: timer.Y, dwRop: SRCCOPY))
		{
			return Image.FromHbitmap(bitmap);
		}
	}
	catch (Exception e)
	{

	}
	finally
	{
		SelectObject(compatibleDeviceContext, oldBitmap);
		DeleteObject(bitmap);
		DeleteDC(compatibleDeviceContext);
		ReleaseDC(p.MainWindowHandle, deviceContext);
	}

	return null;
}

public async Task<SoftwareBitmap> GetSoftwareBitmapAsync(Bitmap bitmap)
{
	using (var stream = new InMemoryRandomAccessStream())
	{
		var imageCodecInfo = GetEncoder(ImageFormat.Bmp);
		System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.ScanMethod;
		EncoderParameters myEncoderParameters = new EncoderParameters(1);
		EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 100);
		myEncoderParameters.Param[0] = myEncoderParameter;

		bitmap.Save(stream.AsStream(), imageCodecInfo, myEncoderParameters);
		BitmapDecoder decoder = await BitmapDecoder.CreateAsync(BitmapDecoder.BmpDecoderId, stream);
		return await decoder.GetSoftwareBitmapAsync();
	}
}

private ImageCodecInfo GetEncoder(ImageFormat format)
{
	ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
	foreach (ImageCodecInfo codec in codecs)
	{
		if (codec.FormatID == format.Guid)
		{
			return codec;
		}
	}
	return null;
}

public Bitmap ResizeBitmap(Bitmap bmp, int width, int height)
{
	Bitmap result = new Bitmap(width, height);
	using (Graphics g = Graphics.FromImage(result))
	{
		g.DrawImage(bmp, 0, 0, width, height);
	}

	return result;
}

public static byte[] GetBytes(Bitmap img)
{
	using (var stream = new MemoryStream())
	{
		img.Save(stream, ImageFormat.Png);
		return stream.ToArray();
	}
}

private Bitmap GetTimer(Bitmap bitmap)
{
	return bitmap.Clone(new Rectangle(new Point(bitmap.Width / 2 - 50, 10), new Size(100, 50)), bitmap.PixelFormat).Dump();
}

uint InvertColor(uint rgbaColor)
{
	return 0xFFFFFF00u ^ rgbaColor; // Assumes alpha is in the rightmost byte, change as needed
}

[StructLayout(LayoutKind.Sequential)]
public struct RECT
{
	public int Left;
	public int Top;
	public int Right;
	public int Bottom;
}

[DllImport("user32.dll")]
public static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);

[DllImport("user32.dll", SetLastError = true)]
public static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

[DllImport("user32.dll", SetLastError = true)]
public static extern bool GetClientRect(IntPtr hwnd, out RECT lpRect);

[DllImport("gdi32.dll")]
static extern bool BitBlt(IntPtr hdcDest, int nxDest, int nyDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, int dwRop);

[DllImport("gdi32.dll")]
static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int width, int height);

[DllImport("gdi32.dll")]
static extern IntPtr CreateCompatibleDC(IntPtr hdc);

[DllImport("user32.dll", SetLastError = true)]
internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

[DllImport("gdi32.dll")]
static extern IntPtr DeleteDC(IntPtr hdc);

[DllImport("gdi32.dll")]
static extern IntPtr DeleteObject(IntPtr hObject);

[DllImport("user32.dll")]
static extern IntPtr GetDesktopWindow();

[DllImport("user32.dll")]
static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

[DllImport("user32.dll")]
static extern IntPtr GetDC(IntPtr hWnd);

[DllImport("user32.dll")]
static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDc);

[DllImport("gdi32.dll")]
static extern IntPtr SelectObject(IntPtr hdc, IntPtr hObject);

const int SRCCOPY = 0x00CC0020;
const int CAPTUREBLT = 0x40000000;