<Query Kind="Program">
  <NuGetReference>Magick.NET-Q8-x64</NuGetReference>
  <NuGetReference>Microsoft.Windows.SDK.Contracts</NuGetReference>
  <Namespace>System.Drawing</Namespace>
  <Namespace>System.Runtime.InteropServices</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>Windows.Graphics.Imaging</Namespace>
  <Namespace>Windows.Storage.Streams</Namespace>
  <Namespace>System.Drawing.Imaging</Namespace>
  <Namespace>ImageMagick</Namespace>
</Query>

async Task Main()
{
	var loadingImage = ScreenshotService.GetLoadingScreen();				
	var (team1, team2) = ScreenshotService.GetTeams(loadingImage);	
	//var ocr = new TesseractService(@"C:\Program Files\Tesseract-OCR");
	
	var ocr = Windows.Media.Ocr.OcrEngine.TryCreateFromLanguage(new Windows.Globalization.Language("en"));
	
	foreach(var team in new[] { team1, team2 })
	{
		foreach(var player in team)
		{
			using(player)
			{
				using(var stream = File.Open(@$"C:\temp\{Guid.NewGuid()}.jpeg", FileMode.OpenOrCreate))
				{					
					player.Save(stream, ImageFormat.Jpeg);
					var results = await ocr.RecognizeAsync(await GetSoftwareBitmapAsync(player));
					
					new { Image = player, results = results }.Dump();
				}
			}

		}
	}
}

public async Task<SoftwareBitmap> GetSoftwareBitmapAsync(Bitmap bitmap)
{
	using (var stream = new InMemoryRandomAccessStream())
	{
		var imageCodecInfo = GetEncoder(ImageFormat.Bmp);
		System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
		EncoderParameters myEncoderParameters = new EncoderParameters(1);
		EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 50L);
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

public static byte[] GetBytes(Bitmap img)
{
	using (var stream = new MemoryStream())
	{
		img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
		return stream.ToArray();
	}
}

static Color RedColor = Color.FromArgb(255, 254, 0, 0);
static Color BlueColor = Color.FromArgb(255, 45, 0, 0);
static Color PlayerColor = Color.FromArgb(255, 160, 0, 0);

static MagickColor MBlue = MagickColor.FromRgb(RedColor.R, RedColor.G, RedColor.B);
static MagickColor MRed = MagickColor.FromRgb(RedColor.R, RedColor.G, RedColor.B);
static MagickColor MPlayer = MagickColor.FromRgb(PlayerColor.R, PlayerColor.G, PlayerColor.B);

public class ScreenshotService
{
	const int PLAYER_WIDTH = 350;
	const int PLAYER_HEIGHT = 170;	
	static Size PLAYER_SIZE = new Size(PLAYER_WIDTH, PLAYER_HEIGHT);	

	public static Bitmap GetLoadingScreen(bool fake = true)
	{
		if (fake) 
			return new Bitmap(Image.FromFile(@"C:\Users\patri\Documents\ShareX\Screenshots\2019-12\HeroesOfTheStorm_x64_60SFbYFwWC.bmp", true));

		var proc = Process.GetProcessesByName("HeroesOfTheStorm_x64")[0];
		var rect = new User32.Rect();

		User32.SetActiveWindow(proc.MainWindowHandle);
		User32.GetWindowRect(proc.MainWindowHandle, ref rect);

		int width = rect.right - rect.left;
		int height = rect.bottom - rect.top;

		var bmp = new Bitmap(width, height, PixelFormat.Format32bppRgb);

		Graphics graphics = Graphics.FromImage(bmp);
		graphics.CopyFromScreen(rect.left, rect.top, 0, 0, new Size(width, height));

		return bmp;
	}

	public static (List<Bitmap> Team1, List<Bitmap> Team2) GetTeams(Bitmap bmp)
	{				
		Bitmap team1bmp = bmp.Clone(new Rectangle(0, 0, bmp.Width / 2, bmp.Height), bmp.PixelFormat);						
		Bitmap team2bmp = bmp.Clone(new Rectangle(bmp.Width / 2, 0, bmp.Width / 2, bmp.Height), bmp.PixelFormat);		
		return (GetPlayers(team1bmp), GetPlayers(team2bmp));
	}
	
	private static (Point Start, MagickColor Team) GetStart(Bitmap team)
	{
		for (int x = 0; x < team.Width; x++)
		{
			for (int y = 0; y < team.Height; y++)
			{
				var pixel = team.GetPixel(x, y);				
				var color = MagickColor.FromRgb(pixel.R, pixel.G, pixel.B);

				if (color == BlueTeam) 
					return (new Point(x - PLAYER_WIDTH, y), BlueTeam);

				if (color == RedTeam) 
					return (new Point(x, y), RedTeam);
			}
		}
		
		throw new Exception("could not find team or starting point");
	}

	private static List<Bitmap> GetPlayers(Bitmap team)
	{
		var (startPoint, teamColor) = GetStart(team);		

		List<Bitmap> players = new List<Bitmap>();

		for (int i = 0; i <= 4; i++)
		{
			var playerImage = team.Clone(new Rectangle(new Point(startPoint.X, startPoint.Y), PLAYER_SIZE), team.PixelFormat);
			
			using (var image = new MagickImage(GetBytes(playerImage)))
			{	
				image.ColorFuzz = new Percentage(2.5);
				
				if(teamColor == BlueTeam)
					image.Crop(220, 250, Gravity.West);
				else
					image.Crop(220, 250, Gravity.East);
			
				// image.BrightnessContrast(brightness: new Percentage(-60), contrast: new Percentage(80));
				
				// image.AdaptiveThreshold(0, 0, 200);
				image.ContrastStretch(new Percentage(0), new Percentage(0));
				image.Negate();
				
				using(var stream = new MemoryStream())
				{
					image.Write(stream);					
					playerImage = new Bitmap(stream); 
				}
			}
			
			players.Add(playerImage);
			
			startPoint.Y += PLAYER_HEIGHT;
		}

		return players;
	}

	private class User32
	{
		[StructLayout(LayoutKind.Sequential)]
		public struct Rect
		{
			public int left;
			public int top;
			public int right;
			public int bottom;
		}

		[DllImport("user32.dll")]
		public static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);

		[DllImport("user32.dll")]
		public static extern IntPtr SetActiveWindow(IntPtr hWnd);
	}
}

// https://github.com/UB-Mannheim/tesseract/wiki
public class TesseractService
{
	private readonly string tesseractExePath;
	private readonly string language;

	public TesseractService(string tesseractDir, string language = "eng", string dataDir = null)
	{
		this.tesseractExePath = Path.Combine(tesseractDir, "tesseract.exe");
		this.language = language;
		Environment.SetEnvironmentVariable("TESSDATA_PREFIX", dataDir ?? Path.Combine(tesseractDir, "tessdata"));
	}

	public string GetText(params Stream[] images)
	{
		var output = string.Empty;

		if (images.Any())
		{
			var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
			Directory.CreateDirectory(tempPath);
			var tempInputFile = NewTempFileName(tempPath);
			var tempOutputFile = NewTempFileName(tempPath);

			try
			{
				WriteInputFiles(images, tempPath, tempInputFile);

				var info = new ProcessStartInfo
				{
					FileName = tesseractExePath,
					Arguments = $"{tempInputFile} {tempOutputFile} -l {language} --psm 12",
					RedirectStandardError = true,
					RedirectStandardOutput = true,
					CreateNoWindow = true,
					UseShellExecute = false
				};

				using (var ps = Process.Start(info))
				{					
					ps.WaitForExit();

					var exitCode = ps.ExitCode;

					if (exitCode == 0)
					{
						output = File.ReadAllText(tempOutputFile + ".txt");
					}
					else
					{
						var stderr = ps.StandardError.ReadToEnd();
						throw new InvalidOperationException(stderr);
					}
					
					ps.StandardOutput.ReadToEnd().Dump();
				}
			}
			finally
			{
				Directory.Delete(tempPath, true);
			}
		}

		return output;
	}

	private static void WriteInputFiles(Stream[] inputStreams, string tempPath, string tempInputFile)
	{
		// If there is more thant one image file, so build the list file using the images as input files.
		if (inputStreams.Length > 1)
		{
			var imagesListFileContent = new StringBuilder();

			foreach (var inputStream in inputStreams)
			{
				var imageFile = NewTempFileName(tempPath);

				using (var tempStream = File.OpenWrite(imageFile))
				{
					CopyStream(inputStream, tempStream);
				}

				imagesListFileContent.AppendLine(imageFile);
			}

			File.WriteAllText(tempInputFile, imagesListFileContent.ToString());
		}
		else
		{
			// If is only one image file, than use the image file as input file.
			using (var tempStream = File.OpenWrite(tempInputFile))
			{
				CopyStream(inputStreams.First(), tempStream);
			}
		}
	}

	private static void CopyStream(Stream input, Stream output)
	{
		if (input.CanSeek)
			input.Seek(0, SeekOrigin.Begin);

		input.CopyTo(output);
		input.Close();
	}

	private static string NewTempFileName(string tempPath)
	{
		return Path.Combine(tempPath, Guid.NewGuid().ToString());
	}
}