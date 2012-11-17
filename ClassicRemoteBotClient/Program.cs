/*
 * Created by SharpDevelop.
 * User: BJORN
 * Date: 29/10/2012
 * Time: 7:47 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace ClassicRemoteBotClient
{
	public enum RemoteProtocol: byte
	{
		/// <summary>Sent when the user first connects. C > S</summary>
		Username = 0,
		/// <summary>Sent when the user is attempting to verify their password. C > S</summary>
		Password = 1,
		/// <summary>Sent when the user is wanting to send a message to the remote bot > To the minecraft server. C > S</summary>
		IncomingMessage = 2,
		/// <summary>Sent when the bot the bot sends a message to the client. (Not a Minecraft one) The next byte contains the type of message,
		/// 0 indicates normal, 1 indicates a warning, 2 indicates an error and that the session should be closed,
		/// 3 indicates password request.</summary>
		ServerMessage = 3,
		/// <summary>Sent to the client when a message is sent to the bot from the Minecraft server. S > C</summary>
		OutgoingMessage = 6,
		/// <summary>Sent to the client once the client has been authorised, and that the server can start receiving chat.</summary>
		ClientAuthorised = 7
			
			
	}
	
	class Program
	{
		static AutoResetEvent mainReady = new AutoResetEvent(false);
		
		static BinaryWriter writeToRemoteServer;
		
		public static void Main(string[] args)
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("Hello, and welcome to LibClassicBot beta remote client.");
			Console.WriteLine("Once you are logged in, you can either send normal messages to the minecraft server,");
			Console.WriteLine("or prefix it with & to send a message to the bot directly.");
			Console.WriteLine("Eg, &commands would return a list of commands that can be remotely executed.");		
			Console.ResetColor();
			
			Console.WriteLine("Enter the IP to connect to. (Eg, 127.0.0.1)");
			IPAddress ip;
			if(!IPAddress.TryParse(Console.ReadLine(),out ip))
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Unable to parse given IP. Press any key to exit.");
				Console.ReadKey();
				return;
			}
			Console.WriteLine("Enter the port to connect on.");
			int iPort = 0;
			if(!Int32.TryParse(Console.ReadLine(),out iPort))
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Unable to parse given IP. Press any key to exit.");
				Console.ReadKey();
				return;
			}
			
			Thread main = new Thread(delegate() { Loop(ip,iPort); } );
			main.Start();
			
			mainReady.WaitOne();
		loop:
			string response = Console.ReadLine();
			try
			{
				if(writeToRemoteServer != null)
					
				{
					writeToRemoteServer.Write((byte)RemoteProtocol.IncomingMessage);
					writeToRemoteServer.Write(response);

				}
			}
			catch(IOException)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("An error occurred while writing. The remote bot may have crashed and not closen the connection cleanly.");
				Console.ResetColor();
				Console.WriteLine("Press any key to abort..");
				Console.ReadKey(true);
				return;
			}
			goto loop;
		}
		
		static void Loop(IPAddress address, int port)
		{
			TcpClient tcp = new TcpClient();
			tcp.Connect(address,port);
			NetworkStream clientStream = tcp.GetStream();
			BinaryReader reader = new BinaryReader(clientStream);
			BinaryWriter writer = new BinaryWriter(clientStream);
			writeToRemoteServer = writer;
			byte[] OpCodePacket= new byte[1];
			int bytesRead;

			Console.WriteLine("Enter the username to use.");
			string user = Console.ReadLine();
			writer.Write((byte)RemoteProtocol.Username);
			
			writer.Write(user);
			
			while (true)
			{
				bytesRead = 0;
				
				try
				{
					//blocks until a client sends a message
					bytesRead = tcp.Client.Receive(OpCodePacket,1,SocketFlags.None);
					switch((RemoteProtocol)OpCodePacket[0])
					{
						case RemoteProtocol.ServerMessage:
							{
								byte next = reader.ReadByte();
								string message = reader.ReadString();
								if(next == 0)
								{
									Console.WriteLine(message);
								}
								
								else if(next == 1)
								{
									Console.ForegroundColor = ConsoleColor.Yellow;
									Console.WriteLine(message);
									Console.ResetColor();
								}
								
								else if(next == 2)
								{
									Console.ForegroundColor = ConsoleColor.Red;
									Console.WriteLine(message);
									Console.WriteLine("Closing the connection.");
									Console.ResetColor();
									tcp.Close();
								}
								
								else if(next == 3)
								{
									Console.WriteLine(message);
									string input = Console.ReadLine();
									writer.Write((byte)RemoteProtocol.Password);
									writer.Write(input);
								}
								
								else if(next == 4) //Incorrect password given.
								{
									Console.ForegroundColor = ConsoleColor.Yellow;
									Console.WriteLine(message);
									Console.ResetColor();
									string input = Console.ReadLine();
									writer.Write((byte)RemoteProtocol.Password);
									writer.Write(input);
								}
								break;
							}
						case (RemoteProtocol)8:
							{
									Console.ForegroundColor = ConsoleColor.Green;
									byte fileormessage = reader.ReadByte(); //0 indicates a normal message, 1 indicates a file is about to be transferred.
									string raw = reader.ReadString();
									if(fileormessage == 1)
									{
										Console.WriteLine("Getting log file "+raw);
										string data = reader.ReadString();
										File.WriteAllText(raw,data);
									}
									else
									{
										Console.WriteLine(raw);
									}
									Console.ResetColor();
								break;
							}
						case RemoteProtocol.OutgoingMessage:
							{
								Console.WriteLine(reader.ReadString());
								break;
							}
							
						case RemoteProtocol.ClientAuthorised:
							{
								Console.WriteLine(reader.ReadString());
								Console.WriteLine("Ready to start receiving messages. All messages typed into console will now be forwarded to the remote bot.");
								mainReady.Set();
								break;
							}
					}
				}
				catch
				{
					//a socket error has occured
					break;
				}
				
				if (bytesRead == 0)
				{
					//the client has disconnected from the server
					break;
				}
			}
		}
	}
}