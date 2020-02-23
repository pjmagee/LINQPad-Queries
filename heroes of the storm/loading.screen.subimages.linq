<Query Kind="Program">
  <NuGetReference>Magick.NET-Q8-x64</NuGetReference>
  <Namespace>ImageMagick</Namespace>
  <Namespace>ImageMagick.Configuration</Namespace>
  <Namespace>ImageMagick.Defines</Namespace>
  <Namespace>ImageMagick.ImageOptimizers</Namespace>
  <Namespace>System.Drawing</Namespace>
  <Namespace>System.Drawing.Imaging</Namespace>
</Query>

void Main()
{
	var loading = new Bitmap(File.Open(@"C:\Users\patri\Pictures\Heroes of the Storm\dragon-shire-loading-screen.bmp", FileMode.Open));
	var blueTeam = new Bitmap(File.Open(@"C:\Users\patri\Pictures\Heroes of the Storm\blue-team-indicator.bmp", FileMode.Open));
	var redTeam = new Bitmap(File.Open(@"C:\Users\patri\Pictures\Heroes of the Storm\red-team-indicator.bmp", FileMode.Open));
	
	GetSubPositions(loading, redTeam).Dump();
	
	loading.Dispose();
	blueTeam.Dispose();
	redTeam.Dispose();
}

public static List<Point> GetSubPositions(Bitmap main, Bitmap sub)
{
	List<Point> possiblepos = new List<Point>();

	int mainwidth = main.Width;
	int mainheight = main.Height;

	int subwidth = sub.Width;
	int subheight = sub.Height;

	int movewidth = mainwidth - subwidth;
	int moveheight = mainheight - subheight;

	BitmapData bmMainData = main.LockBits(new Rectangle(0, 0, mainwidth, mainheight), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
	BitmapData bmSubData = sub.LockBits(new Rectangle(0, 0, subwidth, subheight), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

	int bytesMain = Math.Abs(bmMainData.Stride) * mainheight;
	int strideMain = bmMainData.Stride;
	System.IntPtr Scan0Main = bmMainData.Scan0;
	byte[] dataMain = new byte[bytesMain];
	System.Runtime.InteropServices.Marshal.Copy(Scan0Main, dataMain, 0, bytesMain);

	int bytesSub = Math.Abs(bmSubData.Stride) * subheight;
	int strideSub = bmSubData.Stride;
	System.IntPtr Scan0Sub = bmSubData.Scan0;
	byte[] dataSub = new byte[bytesSub];
	System.Runtime.InteropServices.Marshal.Copy(Scan0Sub, dataSub, 0, bytesSub);

	for (int y = 0; y < moveheight; ++y)
	{
		for (int x = 0; x < movewidth; ++x)
		{
			Color curcolor = GetColor(x, y, strideMain, dataMain);

			foreach (var item in possiblepos.ToArray())
			{
				int xsub = x - item.X;
				int ysub = y - item.Y;
				if (xsub >= subwidth || ysub >= subheight || xsub < 0)
					continue;

				Color subcolor = GetColor(xsub, ysub, strideSub, dataSub);

				if (!curcolor.Equals(subcolor))
				{
					possiblepos.Remove(item);
				}
			}

			if (curcolor.Equals(GetColor(0, 0, strideSub, dataSub)))
				possiblepos.Add(new Point(x, y));
		}
	}

	System.Runtime.InteropServices.Marshal.Copy(dataSub, 0, Scan0Sub, bytesSub);
	sub.UnlockBits(bmSubData);

	System.Runtime.InteropServices.Marshal.Copy(dataMain, 0, Scan0Main, bytesMain);
	main.UnlockBits(bmMainData);

	return possiblepos;
}

private static Color GetColor(Point point, int stride, byte[] data)
{
	return GetColor(point.X, point.Y, stride, data);
}

private static Color GetColor(int x, int y, int stride, byte[] data)
{
	int pos = y * stride + x * 3;
	// byte a = data[pos + 3];
	byte r = data[pos + 2];
	byte g = data[pos + 1];
	byte b = data[pos + 0];
	return Color.FromArgb(a, r, g, b);
}