<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\WPF\System.Windows.Presentation.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\WPF\PresentationFramework.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Xaml.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\WPF\WindowsBase.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\WPF\PresentationCore.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Configuration.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\WPF\UIAutomationProvider.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\WPF\UIAutomationTypes.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\WPF\ReachFramework.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\WPF\PresentationUI.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\WPF\System.Printing.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\Accessibility.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Deployment.dll</Reference>
  <Namespace>System.Net</Namespace>
  <Namespace>System.Net.Sockets</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Windows.Controls</Namespace>
  <Namespace>System.Windows</Namespace>
  <Namespace>System.Windows.Threading</Namespace>
</Query>

void Main()
{
	var addresses = new[]
	{
		new IPEndPoint(IPAddress.Parse("172.16.0.124"), 1000), // ABU
		new IPEndPoint(IPAddress.Parse("172.16.0.131"), 1000), // PATRICK
		new IPEndPoint(IPAddress.Parse("172.16.0.187"), 1000), // JAMES
		new IPEndPoint(IPAddress.Parse("172.16.1.55"), 1000) // MEOW
	};

	var server = new IPEndPoint(IPAddress.Parse("172.16.0.131"), 1000); // PATRICK
	bool isServer = Dns.GetHostName() == "PATRICK";

	var list = new ListBox();
	var textBox = new TextBox { };

	var panel = new StackPanel();
	panel.Children.Add(list);
	panel.Children.Add(textBox);

	var window = new Window { Content = panel, ResizeMode = ResizeMode.CanResize, WindowStyle = WindowStyle.SingleBorderWindow };
	window.Activated += delegate
	{
		Dispatcher.CurrentDispatcher.InvokeAsync(async () =>
		{
			// If this is the server, listen to all addresses, otherwise, listen for packets from server
			using (var udpClient = new UdpClient(isServer ? server : new IPEndPoint(IPAddress.Any, 1000)))
			{
				while (window.IsActive)
				{
					var result = await udpClient.ReceiveAsync();
					var message = Encoding.ASCII.GetString(result.Buffer);
					list.Items.Add(new ListBoxItem() { Content = message });
					textBox.Text = string.Empty;
						
					// If this is the server, relay the message to everyone
					if (isServer)
					{
						using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
						{
							// Send recieved message to all addresses
							foreach (var address in addresses.Except(new[] { server }))
							{
								var data = Encoding.ASCII.GetBytes(message);
								SocketAsyncEventArgs args = new SocketAsyncEventArgs();
								args.SetBuffer(data, 0, data.Length);
								args.UserToken = socket;
								args.RemoteEndPoint = address;
								socket.SendToAsync(args);
							}
						}
					}
				}
			}
		});
	};

	textBox.KeyUp += (sender, e) =>
	{
		if (e.Key == System.Windows.Input.Key.Enter)
		{
			var text = textBox.Text;
			textBox.Text = string.Empty;

			using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
			{
				if (isServer) // relay message to all listening on my address
				{
					foreach (var address in addresses)
					{
						var data = Encoding.ASCII.GetBytes($"{System.Environment.MachineName}: {text}");
						SocketAsyncEventArgs args = new SocketAsyncEventArgs();
						args.SetBuffer(data, 0, data.Length);
						args.UserToken = socket;
						args.RemoteEndPoint = address;
						socket.SendToAsync(args);
					}
				}
				else // send to server address only (it will relay the message)
				{
					var data = Encoding.ASCII.GetBytes($"{System.Environment.MachineName}: {text}");
					SocketAsyncEventArgs args = new SocketAsyncEventArgs();
					args.SetBuffer(data, 0, data.Length);
					args.UserToken = socket;
					args.RemoteEndPoint = server;
					socket.SendToAsync(args);
				}
			}
		}
	};

	window.ShowDialog();
	Dispatcher.CurrentDispatcher.InvokeShutdown();
}

//private async Task SendAsync()
//{
//	using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
//	{
//		var textBox = new TextBox { Width = 150, Height = 20 };
//
//		textBox.KeyUp += (sender, e) =>
//		{
//			if (e.Key == System.Windows.Input.Key.Enter)
//			{
//				var data = Encoding.ASCII.GetBytes($"{System.Environment.MachineName}: {textBox.Text}");
//				SocketAsyncEventArgs args = new SocketAsyncEventArgs();
//				args.SetBuffer(data, 0, data.Length);
//				args.UserToken = socket;
//				args.RemoteEndPoint = groupEp;
//				socket.SendToAsync(args);
//				textBox.Text = string.Empty;
//			}
//		};
//
//		textBox.Dump();
//	}
//}

//private async Task ListenAsync()
//{
//	using (var udpClient = new UdpClient(groupEp))
//	{
//		var result = await udpClient.ReceiveAsync();
//		var message = Encoding.ASCII.GetString(result.Buffer);
//
//		Console.WriteLine($"{message}");
//	}
//}

// Define other methods and classes here