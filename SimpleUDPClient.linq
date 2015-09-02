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
	IPEndPoint groupEp = new IPEndPoint(IPAddress.Loopback, 1000);

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
			using (var udpClient = new UdpClient(groupEp))
			{
				while (window.IsActive)
				{
					var result = await udpClient.ReceiveAsync();
					var message = Encoding.ASCII.GetString(result.Buffer);
					list.Items.Add(new ListBoxItem() { Content = message });
				}
			}
		});
	};

	textBox.KeyUp += (sender, e) =>
	{
		if (e.Key == System.Windows.Input.Key.Enter)
		{
			using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
			{
				var data = Encoding.ASCII.GetBytes($"{System.Environment.MachineName}: {textBox.Text}");
				SocketAsyncEventArgs args = new SocketAsyncEventArgs();
				args.SetBuffer(data, 0, data.Length);
				args.UserToken = socket;
				args.RemoteEndPoint = groupEp;
				socket.SendToAsync(args);
				textBox.Text = string.Empty;
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