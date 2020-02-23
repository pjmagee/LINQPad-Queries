<Query Kind="Program">
  <NuGetReference>SharpDX</NuGetReference>
  <NuGetReference>SharpDX.Direct3D11</NuGetReference>
  <NuGetReference>SharpDX.Mathematics</NuGetReference>
  <Namespace>SharpDX</Namespace>
  <Namespace>SharpDX.Diagnostics</Namespace>
  <Namespace>SharpDX.Direct3D</Namespace>
  <Namespace>SharpDX.Direct3D11</Namespace>
  <Namespace>SharpDX.DXGI</Namespace>
  <Namespace>SharpDX.IO</Namespace>
  <Namespace>SharpDX.Mathematics.Interop</Namespace>
  <Namespace>SharpDX.Multimedia</Namespace>
  <Namespace>SharpDX.Text</Namespace>
  <Namespace>SharpDX.Win32</Namespace>
  <Namespace>System.Drawing</Namespace>
  <Namespace>System.Drawing.Imaging</Namespace>
</Query>

[STAThread]
        private static void Main()
{
	// # of graphics card adapter
	const int numAdapter = 0;

	// # of output device (i.e. monitor)
	const int numOutput = 0;

	const string outputFileName = "ScreenCapture.bmp";

	// Create DXGI Factory1
	var factory = new Factory1();
	var adapter = factory.GetAdapter1(numAdapter);

	// Create device from Adapter
	var device = new SharpDX.Direct3D11.Device(adapter);

	// Get DXGI.Output
	var output = adapter.GetOutput(numOutput);
	var output1 = output.QueryInterface<Output1>();

	// Width/Height of desktop to capture
	int width = ((SharpDX.Rectangle)output.Description.DesktopBounds).Width;
	int height = ((SharpDX.Rectangle)output.Description.DesktopBounds).Height;

	// Create Staging texture CPU-accessible
	var textureDesc = new Texture2DDescription
	{
		CpuAccessFlags = CpuAccessFlags.Read,
		BindFlags = BindFlags.None,
		Format = Format.B8G8R8A8_UNorm,
		Width = width,
		Height = height,
		OptionFlags = ResourceOptionFlags.None,
		MipLevels = 1,
		ArraySize = 1,
		SampleDescription = { Count = 1, Quality = 0 },
		Usage = ResourceUsage.Staging
	};
	var screenTexture = new Texture2D(device, textureDesc);

	// Duplicate the output
	var duplicatedOutput = output1.DuplicateOutput(device);

	bool captureDone = false;
	for (int i = 0; !captureDone; i++)
	{
		try
		{
			SharpDX.DXGI.Resource screenResource;
			OutputDuplicateFrameInformation duplicateFrameInformation;

			// Try to get duplicated frame within given time
			duplicatedOutput.AcquireNextFrame(10000, out duplicateFrameInformation, out screenResource);

			if (i > 0)
			{
				// copy resource into memory that can be accessed by the CPU
				using (var screenTexture2D = screenResource.QueryInterface<Texture2D>())
					device.ImmediateContext.CopyResource(screenTexture2D, screenTexture);

				// Get the desktop capture texture
				var mapSource = device.ImmediateContext.MapSubresource(screenTexture, 0, MapMode.Read, SharpDX.Direct3D11.MapFlags.None);

				// Create Drawing.Bitmap
				var bitmap = new System.Drawing.Bitmap(width, height, PixelFormat.Format32bppArgb);
				var boundsRect = new System.Drawing.Rectangle(0, 0, width, height);

				// Copy pixels from screen capture Texture to GDI bitmap
				var mapDest = bitmap.LockBits(boundsRect, ImageLockMode.WriteOnly, bitmap.PixelFormat);
				var sourcePtr = mapSource.DataPointer;
				var destPtr = mapDest.Scan0;
				for (int y = 0; y < height; y++)
				{
					// Copy a single line 
					Utilities.CopyMemory(destPtr, sourcePtr, width * 4);

					// Advance pointers
					sourcePtr = IntPtr.Add(sourcePtr, mapSource.RowPitch);
					destPtr = IntPtr.Add(destPtr, mapDest.Stride);
				}

				// Release source and dest locks
				bitmap.UnlockBits(mapDest);
				device.ImmediateContext.UnmapSubresource(screenTexture, 0);

				// Save the output
				bitmap.Save(outputFileName);

				// Capture done
				captureDone = true;
			}

			screenResource.Dispose();
			duplicatedOutput.ReleaseFrame();

		}
		catch (SharpDXException e)
		{
			if (e.ResultCode.Code != SharpDX.DXGI.ResultCode.WaitTimeout.Result.Code)
			{
				throw e;
			}
		}
	}

	// Display the texture using system associated viewer
	System.Diagnostics.Process.Start(Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, outputFileName)));

	// TODO: We should cleanp up all allocated COM objects here
}