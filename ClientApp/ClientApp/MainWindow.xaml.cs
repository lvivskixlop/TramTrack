using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Text.RegularExpressions;
using Microsoft.Maps.MapControl.WPF;
using System.Net;
using System.Net.Sockets;


namespace ClientApp
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			WaysText.Text = GetWays();			

		}

		private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
		{
			Regex regex = new Regex("[^0-9]+");
			e.Handled = regex.IsMatch(e.Text);
		}

		private void BtnWay_Click(object sender, RoutedEventArgs e)
		{
			string input = WayInput.Text;
			input = input.Replace(" ", "");
			int way = Convert.ToInt32(input);
			List<double> Lat = new List<double>();
			List<double> Long = new List<double>();
			List<string> StopName = new List<string>();
			string NumPlate = "";
			Lat = GetStopLat(way);
			Long = GetStopLong(way);
			StopName = GetStopName(way);
			NumPlate = GetNumPlate(way);
			NumPlatesText.Text = NumPlate;
			for (int i = 0; i < Lat.Count(); i++)
			{
				Pushpin StopPosPin = new Pushpin();
				StopPosPin.Location = new Location(Lat[i], Long[i]);
				StopPosPin.ToolTip = "Зупинка " + StopName[i];
				Map.Children.Add(StopPosPin);
			}

		}

		private void BtnVehicle_Click(object sender, RoutedEventArgs e)
		{
			int NumPlate = Convert.ToInt32(VehicleInput.Text);
			double Lat = GetVehLat(NumPlate);
			double Long = GetVehLong(NumPlate);
			Pushpin VehPosPin = new Pushpin();
			VehPosPin.Location = new Location(Lat, Long);
			VehPosPin.Content = "TR";
			VehPosPin.ToolTip = NumPlate;
			Map.Children.Add(VehPosPin);

		}

		private void BtnDelPushpins_Click(object sender, RoutedEventArgs e)
		{
			Map.Children.Clear();
		}

		private void WayInput_KeyUp(object sender, KeyEventArgs e)
		{
			if(e.Key == System.Windows.Input.Key.Enter)
			{
				BtnWay_Click(sender, e);
			}
		}

		private void VehicleInput_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == System.Windows.Input.Key.Enter)
			{
				BtnVehicle_Click(sender, e);
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			string mode = BtnMode.Content.ToString();
			if (mode == "Супутник")
			{
				BtnMode.Content = "Карта";
				Map.Mode = new AerialMode(true);
			}
			else
			{
				BtnMode.Content = "Супутник";
				Map.Mode = new RoadMode();
			}
		}

		private void BtnServer_Click(object sender, RoutedEventArgs e)
		{
			ClientStuff clientStuff = new ClientStuff();
			clientStuff.Show();
			Thread.Sleep(200);
			
		}

		//Функції створення запитів до сервера

		public static string GetWays()
		{
			try
			{
				string Ways = "";
				TcpClient client = new TcpClient();
				client.Connect(IPAddress.Parse($"{ClientStuff.IpString}"), ClientStuff.Port);
				NetworkStream stream = client.GetStream();
				bool MsgRecieved = false;
				byte[] send = new byte[1024];
				byte[] recieve = new byte[1024];
				string requetStr = "WAYS 0";
				send = Encoding.ASCII.GetBytes(requetStr);
				while (!MsgRecieved)
				{
					stream.Write(send, 0, send.Length);
					stream.Read(recieve, 0, recieve.Length);
					Ways = Encoding.ASCII.GetString(recieve).Replace("\0", string.Empty);
					if (Ways != "")
					{
						MsgRecieved = true;
					}
				}
				return Ways;
			}
			catch (Exception)
			{
				MessageBox.Show("Connection Error");
				return "";
			}
		}
		
		private List<double> GetStopLat(int way)
		{
			try
			{
				List<double> Lat = new List<double>();
				TcpClient client = new TcpClient();
				client.Connect(IPAddress.Parse($"{ClientStuff.IpString}"), ClientStuff.Port);
				NetworkStream stream = client.GetStream();
				bool MsgRecieved = false;
				byte[] send = new byte[1024];
				byte[] recieve = new byte[1024];
				string requetStr = $"STOPLAT {way}";
				send = Encoding.ASCII.GetBytes(requetStr);
				while (!MsgRecieved)
				{
					stream.Write(send, 0, send.Length);
					stream.Read(recieve, 0, recieve.Length);
					Thread.Sleep(500);
					double[] values = new double[recieve.Length / 8];
					for (int i = 0; i < values.Length; i++)
					{
						values[i] = BitConverter.ToDouble(recieve, i * 8);
						if (BitConverter.ToDouble(recieve, i * 8) == 0)
							break;
						Lat.Add(values[i]);
					}
					MsgRecieved = true;
				}
				return Lat;
			}
			catch (Exception)
			{
				MessageBox.Show("Connection Error");
				return null;
			}
		}

		private List<double> GetStopLong(int way)
		{
			try
			{
				List<double> Long = new List<double>();
				TcpClient client = new TcpClient();
				client.Connect(IPAddress.Parse($"{ClientStuff.IpString}"), ClientStuff.Port);
				NetworkStream stream = client.GetStream();
				bool MsgRecieved = false;
				byte[] send = new byte[1024];
				byte[] recieve = new byte[1024];
				string requetStr = $"STOPLONG {way}";
				send = Encoding.ASCII.GetBytes(requetStr);
				while (!MsgRecieved)
				{
					stream.Write(send, 0, send.Length);
					stream.Read(recieve, 0, recieve.Length);
					Thread.Sleep(500);
					double[] values = new double[recieve.Length / 8];
					for (int i = 0; i < values.Length; i++)
					{
						values[i] = BitConverter.ToDouble(recieve, i * 8);
						if (BitConverter.ToDouble(recieve, i * 8) == 0)
							break;
						Long.Add(values[i]);
					}
					MsgRecieved = true;
				}
				return Long;
			}
			catch (Exception)
			{
				MessageBox.Show("Connection Error");
				return null;
			}
		}

		private List<string> GetStopName(int way)
		{
			try
			{
				List<string> StopName = new List<string>();
				TcpClient client = new TcpClient();
				client.Connect(IPAddress.Parse($"{ClientStuff.IpString}"), ClientStuff.Port);
				NetworkStream stream = client.GetStream();
				bool MsgRecieved = false;
				byte[] send = new byte[2048];
				byte[] recieve = new byte[2048];
				string requetStr = $"STOPNAME {way}";
				send = Encoding.ASCII.GetBytes(requetStr);
				string result;
				while (!MsgRecieved)
				{
					stream.Write(send, 0, send.Length);
					stream.Read(recieve, 0, recieve.Length);
					Thread.Sleep(500);
					result = Encoding.Unicode.GetString(recieve);
					string[] resarr = result.Split(' ');
					for (int i = 0; i < resarr.Length; i++)
						StopName.Add(resarr[i].Replace("\0", string.Empty).Replace("_", " "));					
					MsgRecieved = true;
				}
				return StopName;
			}
			catch (Exception)
			{
				MessageBox.Show("Connection Error");
				return null;
			}
		}

		private string GetNumPlate(int way)
		{
			try
			{
				string NumPlate = "";
				TcpClient client = new TcpClient();
				client.Connect(IPAddress.Parse($"{ClientStuff.IpString}"), ClientStuff.Port);
				NetworkStream stream = client.GetStream();
				bool MsgRecieved = false;
				byte[] send = new byte[1024];
				byte[] recieve = new byte[1024];
				string requetStr = $"GETNUMPLATE {way}";
				send = Encoding.ASCII.GetBytes(requetStr);
				while (!MsgRecieved)
				{
					stream.Write(send, 0, send.Length);
					stream.Read(recieve, 0, recieve.Length);
					Thread.Sleep(500);
					NumPlate = Encoding.ASCII.GetString(recieve).Replace("\0", string.Empty);
					MsgRecieved = true;
				}
				return NumPlate;
			}
			catch (Exception)
			{
				MessageBox.Show("Connection Error");
				return null;
			}
		}

		private double GetVehLat(int NumPlate)
		{
			try
			{
				double Lat = 0;
				TcpClient client = new TcpClient();
				client.Connect(IPAddress.Parse($"{ClientStuff.IpString}"), ClientStuff.Port);
				NetworkStream stream = client.GetStream();
				bool MsgRecieved = false;
				byte[] send = new byte[1024];
				byte[] recieve = new byte[1024];
				string requetStr = $"VEHLAT {NumPlate}";
				send = Encoding.ASCII.GetBytes(requetStr);
				while (!MsgRecieved)
				{
					stream.Write(send, 0, send.Length);
					stream.Read(recieve, 0, recieve.Length);
					Thread.Sleep(500);
					Lat = BitConverter.ToDouble(recieve, 0);
					MsgRecieved = true;
				}
				return Lat;
			}
			catch (Exception)
			{
				MessageBox.Show("Connection Error");
				return 0;
			}
		}

		private double GetVehLong(int NumPlate)
		{
			try
			{
				double Long = 0;
				TcpClient client = new TcpClient();
				client.Connect(IPAddress.Parse($"{ClientStuff.IpString}"), ClientStuff.Port);
				NetworkStream stream = client.GetStream();
				bool MsgRecieved = false;
				byte[] send = new byte[1024];
				byte[] recieve = new byte[1024];
				string requetStr = $"VEHLONG {NumPlate}";
				send = Encoding.ASCII.GetBytes(requetStr);
				while (!MsgRecieved)
				{
					stream.Write(send, 0, send.Length);
					stream.Read(recieve, 0, recieve.Length);
					Thread.Sleep(500);
					Long = BitConverter.ToDouble(recieve, 0);
					MsgRecieved = true;
				}
				return Long;
			}
			catch (Exception)
			{
				MessageBox.Show("Connection Error");
				return 0;
			}
		}
	}
}