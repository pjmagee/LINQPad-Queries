<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\Accessibility.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Configuration.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Deployment.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Runtime.Serialization.Formatters.Soap.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Security.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Windows.Forms.dll</Reference>
  <NuGetReference Prerelease="true">SharpDX</NuGetReference>
  <NuGetReference Prerelease="true">SharpDX.DirectInput</NuGetReference>
  <Namespace>SharpDX</Namespace>
  <Namespace>SharpDX.Diagnostics</Namespace>
  <Namespace>SharpDX.Direct3D</Namespace>
  <Namespace>SharpDX.DirectInput</Namespace>
  <Namespace>SharpDX.IO</Namespace>
  <Namespace>SharpDX.Mathematics.Interop</Namespace>
  <Namespace>SharpDX.Multimedia</Namespace>
  <Namespace>SharpDX.Text</Namespace>
  <Namespace>SharpDX.Win32</Namespace>
  <Namespace>System.Drawing</Namespace>
  <Namespace>System.Windows.Forms</Namespace>
</Query>

void Main()
{
	var ki = InputStateMachine.KbState.GetEnumerator();
	var mi = InputStateMachine.MsState.GetEnumerator();
	
	var font = new System.Drawing.Font(FontFamily.GenericSerif, 16);
	Util.AutoScrollResults = true;
	
	System.Drawing.Color f = System.Drawing.Color.Black;
	System.Drawing.Color b = System.Drawing.Color.White;
	
	// lmfao
	while(ki.MoveNext() && mi.MoveNext())
	{		
		var ks = ki.Current;
		var ms = mi.Current;
		
		if(ks.IsPressed(Key.LeftAlt))
		{
			f = System.Drawing.Color.Red;
		}
		
		if(ks.PressedKeys.Count > 0)
		{				
			var img = InputStateMachine.DrawText(string.Join(", ", ks.PressedKeys.ToArray()), font, f, b);
			var bytes = InputStateMachine.ImageToBytes(img);			
			Util.Image(bytes).Dump();
		}
		
		if(ms.X > 0)
		{
			var img = InputStateMachine.DrawText(string.Join(", ", new[] { ms.X, ms.Y }), font, f, b);
			var bytes = InputStateMachine.ImageToBytes(img);			
			Util.Image(bytes).Dump();
		}
		
	}
}



public static class InputStateMachine
{
	public static Keyboard kb = new Keyboard(new DirectInput());
	public static Mouse m = new Mouse(new DirectInput());

	public static byte[] ImageToBytes(System.Drawing.Image imageIn)
	{
 		MemoryStream ms = new MemoryStream();
 		imageIn.Save(ms,System.Drawing.Imaging.ImageFormat.Gif);
 		return ms.ToArray();
 	}

	public static Image DrawText(string text, Font font, System.Drawing.Color textColor, System.Drawing.Color backColor)
	{
		Image img = new Bitmap(1, 1);
		Graphics drawing = Graphics.FromImage(img);
		SizeF textSize = drawing.MeasureString(text, font);
		
		img.Dispose();
		drawing.Dispose();
		img = new Bitmap((int) textSize.Width, (int)textSize.Height);	
		drawing = Graphics.FromImage(img);
		drawing.Clear(backColor);
	

		Brush textBrush = new SolidBrush(textColor);	
		drawing.DrawString(text, font, textBrush, 0, 0);	
		drawing.Save();
	
		textBrush.Dispose();
		drawing.Dispose();
	
		return img;	
	}	
	
	static InputStateMachine()
	{
		kb.Acquire();
		m.Acquire();
	}
	
	// lmfao
	public static IEnumerable<KeyboardState> KbState
	{
		get
		{				
			while(true)
			{				
				kb.Poll();
				yield return kb.GetCurrentState();
			}
		}		
	}
	
	// lmfao
	public static IEnumerable<MouseState> MsState
	{
		get 
		{
			while(true)
			{
				m.Poll();
				yield return m.GetCurrentState();
			}
		}
	}
}