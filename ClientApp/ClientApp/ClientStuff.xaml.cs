using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using System.Net.Sockets;
using System.IO;
using System.Net;

namespace ClientApp
{
	/// <summary>
	/// Interaction logic for ClientStuff.xaml
	/// </summary>
	public partial class ClientStuff : Window
	{
		public ClientStuff()
		{
			InitializeComponent();
			PortInput.Text = Convert.ToString(Port);
			IpInput.Text = IpString;
		}

		private void BtnConnect_Click(object sender, RoutedEventArgs e)
		{
			Port = Convert.ToInt32(PortInput.Text);
			IpString = IpInput.Text;
			Thread thread = new Thread(new ThreadStart(ConnectAsClient));
			thread.Start();
			MainWindow mainWindow = new MainWindow();
			mainWindow.WaysText.Text = MainWindow.GetWays();
			this.Close();
		}
		private void ConnectAsClient()
		{
			try
			{
				TcpClient client = new TcpClient();
				client.Connect(IPAddress.Parse($"{IpString}"), Port);
				MessageBox.Show($"Connected to server on: {IpString}:{Port}");
				NetworkStream stream = client.GetStream();
				string str = "I am client";
				byte[] msg = Encoding.ASCII.GetBytes(str);
				stream.Write(msg, 0, msg.Length);
				stream.Close();
				client.Close();
			}
			catch (Exception)
			{
				MessageBox.Show("Connection Failed");
			}
		}

		public static string IpString = "192.168.195.95";
		public static int Port = 80;
	}
}
