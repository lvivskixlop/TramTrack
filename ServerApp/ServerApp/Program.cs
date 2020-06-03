using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Data.SqlClient;

namespace ServerApp
{
	class Program
	{
		static void Main(string[] args)
		{
			TcpListener server = new TcpListener(IPAddress.Any, 80);
			server.Start();
			Console.WriteLine("TramTrack Server [Version 1.0]\n(c) DanyloStokalo Corporation, 2000. All rights reserved.");
			while (true)
			{
				TcpClient client = server.AcceptTcpClient();
				NetworkStream stream = client.GetStream();
				while (client.Connected)
				{
					byte[] send = new byte[2048];
					byte[] recieve = new byte[2048];
					string reqestStr;
					stream.Read(recieve, 0, recieve.Length);
					reqestStr = Encoding.ASCII.GetString(recieve).Replace("\0", string.Empty);
					string[] arr = reqestStr.Split(new char[] { ' ' }, 2);
					int param = Convert.ToInt32(arr[1]);
					reqestStr = arr[0];
					Console.WriteLine($"\nRequest '{reqestStr}' recieved from: {client.Client.RemoteEndPoint.AddressFamily}\n");
					switch (reqestStr)
					{
						case "WAYS":
							string answer = GetWays();
							send = Encoding.ASCII.GetBytes(answer);
							stream.Write(send, 0, send.Length);
							Console.WriteLine("Answer sent");
							stream.Flush();
							client.Close();
							break;
						case "STOPLAT":
							List<double> Lat = new List<double>();
							Lat = GetStopLat(param);
							send = Lat.SelectMany(value => BitConverter.GetBytes(value)).ToArray();
							stream.Write(send, 0, send.Length);
							Thread.Sleep(200);
							Console.WriteLine("Answer sent");
							stream.Flush();
							client.Close();
							break;
						case "STOPLONG":
							List<double> Long = new List<double>();
							Long = GetStopLong(param);
							send = Long.SelectMany(value => BitConverter.GetBytes(value)).ToArray();
							stream.Write(send, 0, send.Length);
							Thread.Sleep(200);
							Console.WriteLine("Answer sent");
							stream.Flush();
							client.Close();
							break;
						case "STOPNAME":
							List<string> StopName = new List<string>();
							StopName = GetStopName(param);
							string[] strNme = new string[StopName.Count];
							for (int i = 0; i < StopName.Count; i++)
							{
								strNme[i] = StopName[i].Replace(" ", string.Empty);
							}
							string result = string.Join(" ", strNme);
							send = Encoding.Unicode.GetBytes(result);
							stream.Write(send, 0, send.Length);
							Console.WriteLine("Answer sent");
							stream.Flush();
							client.Close();
							break;
						case "GETNUMPLATE":
							string NumPlate = "";
							NumPlate = GetNumPlates(param);
							send = Encoding.ASCII.GetBytes(NumPlate);
							stream.Write(send, 0, send.Length);
							Console.WriteLine("Answer sent");
							stream.Flush();
							client.Close();
							break;
						case "VEHLAT":
							double Lat1 = 0;
							Lat1 = GetVehLat(param);
							send = BitConverter.GetBytes(Lat1);
							stream.Write(send, 0, send.Length);
							Console.WriteLine("Answer sent");
							stream.Flush();
							client.Close();
							break;
						case "VEHLONG":
							double Long1 = 0;
							Long1 = GetVehLong(param);
							send = BitConverter.GetBytes(Long1);
							stream.Write(send, 0, send.Length);
							Console.WriteLine("Answer sent");
							stream.Flush();
							client.Close();
							break;
						default:
							Console.WriteLine($"Not recognized command '{reqestStr}'");
							break;
					}
				}
			}
		}

		public static string GetWays()
		{
			try
			{
				string connectionString = @"Data Source=tcp:KOMPIHA, 1433;Initial Catalog=DB;Integrated Security=True; User Id = Lvivskixlop; Password = 1q2w3e4r";
				SqlConnection connection;
				connection = new SqlConnection(connectionString);
				connection.Open();
				SqlCommand command;
				SqlDataReader dataReader;
				string sql = "select Way from Ways";
				string Output = "";
				command = new SqlCommand(sql, connection);
				dataReader = command.ExecuteReader();
				while (dataReader.Read())
				{
					Output = Output + dataReader.GetValue(0) + "\n";
				}
				Console.WriteLine($"Answer is:\n{Output}");
				return Output;
			}
			catch (Exception)
			{
				Console.WriteLine("Error in reading DB");
				return "Cannot read from\ndatabase";
			}
			
		}

		public static List<double> GetStopLat(int way)
		{
			try
			{
				List<double> Lat = new List<double>();
				string sql = $"select Lat from Stops where Way like '%{way}%'";
				string connectionString = @"Data Source=KOMPIHA;Initial Catalog=DB;Integrated Security=True;Pooling=False";
				SqlConnection connection;
				connection = new SqlConnection(connectionString);
				connection.Open();
				SqlCommand command;
				SqlDataReader dataReader;
				command = new SqlCommand(sql, connection);
				dataReader = command.ExecuteReader();
				while (dataReader.Read())
				{
					Lat.Add(dataReader.GetDouble(0));
					Console.WriteLine($"{dataReader.GetDouble(0)}");
				}
				dataReader.Close();
				command.Dispose();
				connection.Close();
				return Lat;
			}
			catch (Exception)
			{
				Console.WriteLine("Error in reading DB");
				return null;
			}
		}

		public static List<double> GetStopLong(int way)
		{
			try
			{
				List<double> Long = new List<double>();
				string sql = $"select Long from Stops where Way like '%{way}%'";
				string connectionString = @"Data Source=KOMPIHA;Initial Catalog=DB;Integrated Security=True;Pooling=False";
				SqlConnection connection;
				connection = new SqlConnection(connectionString);
				connection.Open();
				SqlCommand command;
				SqlDataReader dataReader;
				command = new SqlCommand(sql, connection);
				dataReader = command.ExecuteReader();
				while (dataReader.Read())
				{
					Long.Add(dataReader.GetDouble(0));
					Console.WriteLine($"{dataReader.GetDouble(0)}");
				}
				dataReader.Close();
				command.Dispose();
				connection.Close();
				return Long;
			}
			catch (Exception)
			{
				Console.WriteLine("Error in reading DB");
				return null;
			}
		}

		public static List<string> GetStopName(int way)
		{
			try
			{
				List<string> StopName = new List<string>();
				string sql = $"select StopName from Stops where Way like '%{way}%'";
				string connectionString = @"Data Source=KOMPIHA;Initial Catalog=DB;Integrated Security=True;Pooling=False";
				SqlConnection connection;
				connection = new SqlConnection(connectionString);
				connection.Open();
				SqlCommand command;
				SqlDataReader dataReader;
				command = new SqlCommand(sql, connection);
				dataReader = command.ExecuteReader();
				while (dataReader.Read())
				{
					StopName.Add(dataReader.GetString(0));
					Console.WriteLine($"{dataReader.GetString(0)}");
				}
				dataReader.Close();
				command.Dispose();
				connection.Close();
				return StopName;
			}
			catch (Exception)
			{
				Console.WriteLine("Error in reading DB");
				return null;
			}
		}

		public static string GetNumPlates(int way)
		{
			try
			{
				string NumPlate = "";
				string sql = $"select NumberPlate from Vehicles where WayId = '{way}'";
				string connectionString = @"Data Source=KOMPIHA;Initial Catalog=DB;Integrated Security=True;Pooling=False";
				SqlConnection connection;
				connection = new SqlConnection(connectionString);
				connection.Open();
				SqlCommand command;
				SqlDataReader dataReader;
				command = new SqlCommand(sql, connection);
				dataReader = command.ExecuteReader();
				while (dataReader.Read())
				{
					NumPlate += dataReader.GetValue(0) + "\n";
				}
				Console.WriteLine($"{NumPlate}");
				dataReader.Close();
				command.Dispose();
				connection.Close();
				return NumPlate;
			}
			catch (Exception)
			{
				Console.WriteLine("Error in reading DB");
				return null;
			}
		}

		public static double GetVehLat(int NumPlate)
		{
			try
			{
				double Lat = 0;
				string sql = $"select Lat from Vehicles where NumberPlate = '{NumPlate}'";
				string connectionString = @"Data Source=KOMPIHA;Initial Catalog=DB;Integrated Security=True;Pooling=False";
				SqlConnection connection;
				connection = new SqlConnection(connectionString);
				connection.Open();
				SqlCommand command;
				SqlDataReader dataReader;
				command = new SqlCommand(sql, connection);
				dataReader = command.ExecuteReader();
				while (dataReader.Read())
				{
					Lat = dataReader.GetDouble(0);
				}
				Console.WriteLine(Lat);
				dataReader.Close();
				command.Dispose();
				connection.Close();
				return Lat;
			}
			catch (Exception)
			{
				Console.WriteLine("Error in reading DB");
				return 0;
			}
		}

		public static double GetVehLong(int NumPlate)
		{
			try
			{
				double Long = 0;
				string sql = $"select Long from Vehicles where NumberPlate = '{NumPlate}'";
				string connectionString = @"Data Source=KOMPIHA;Initial Catalog=DB;Integrated Security=True;Pooling=False";
				SqlConnection connection;
				connection = new SqlConnection(connectionString);
				connection.Open();
				SqlCommand command;
				SqlDataReader dataReader;
				command = new SqlCommand(sql, connection);
				dataReader = command.ExecuteReader();
				while (dataReader.Read())
				{
					Long = dataReader.GetDouble(0);
				}
				Console.WriteLine(Long);
				dataReader.Close();
				command.Dispose();
				connection.Close();
				return Long;
			}
			catch (Exception)
			{
				Console.WriteLine("Error in reading DB");
				return 0;
			}
		}
	}
}
