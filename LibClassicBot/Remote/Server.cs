using System;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.Collections.Generic;

namespace LibClassicBot.Remote
{
	/// <summary>Class for starting a remote server. Actual client is handled by the separate class, RemoteClient.</summary>
	public partial class Server
	{
		#region Public Properties
		/// <summary>The password used to verify remotely connecting users.</summary>
		public string RemoteVerificationPassword {
			get { return _password; }
			set { _password = value; }
		}
		
		/// <summary>
		/// The port the server is listening on for remote clients.
		/// </summary>
		public int RemoteServerPort {
			get { return _port; }
		}
		
		/// <summary>
		/// A list of all remotely connected clients to the bot.
		/// </summary>
		public List<RemoteClient> Clients = new List<RemoteClient>();
		
		public Server.RemoteEvents RemoteBotEvents = new Server.RemoteEvents();
		#endregion
		
		private int _port;
		
		internal string _password;

		internal ClassicBot MinecraftBot;
		
		/// <summary>
		/// Starts a server for remote clients to connect to on the specified port, with the specified password.
		/// </summary>
		/// <param name="MainBot">The main bot, which is used for sending messages from remote clients.</param>
		/// <param name="remotePort">The port to listen on.</param>
		/// <param name="remotePassword">The password to use. Does not have to be the same as the password of the bots account.</param>
		public void Start(ClassicBot MainBot, int remotePort, string remotePassword)
		{
			MinecraftBot = MainBot;
			_password = remotePassword;
			_port = remotePort;
			Thread listenThread = new Thread(ListenForClients);
			listenThread.IsBackground = true; //Don't hold off stopping the main interface.
			listenThread.Start();
		}

		/// <summary>
		/// Sends a chat message to all connected remote clients. The message that is sent to the 
		/// remote clients is considered to be a Minecraft message, not a Server message.
		/// </summary>
		/// <param name="message">The chat message to send. Can include color codes.</param>
		public void SendMessageToAllRemoteClients(string message)
		{
			foreach(RemoteClient client in Clients)
			{
				client.MessageClient(message);
			}
		}
		
		private void ListenForClients()
		{
			TcpListener tcpListener = new TcpListener(IPAddress.Any,_port);
			tcpListener.Start();
			
			while (true)
			{
				//Blocks the thread until a client had connected.
				TcpClient RemoteTcpClient = tcpListener.AcceptTcpClient();
				//new TcpClient() { Client = tcpListener.Server.Accept(); };
				SessionStartedEventArgs e = new SessionStartedEventArgs((IPEndPoint)RemoteTcpClient.Client.RemoteEndPoint,RemoteTcpClient);
				RemoteBotEvents.RaiseSessionStarted(e);
				//Create a thread to handle communication with the connected remote client.
				RemoteClient client = new RemoteClient();
				Thread clientThread = new Thread(delegate() { client.Start(RemoteTcpClient,this); } );
				clientThread.Start();
				Clients.Add(client);
			}
		}
	}
}