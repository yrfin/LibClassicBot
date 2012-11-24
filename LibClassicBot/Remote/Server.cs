using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using LibClassicBot.Events;

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
		
		/// <summary>The port the server is listening on for remote clients.</summary>
		public int RemoteServerPort {
			get { return _port; }
		}
		
		/// <summary>Whether or not the server has started listening for remote clients.</summary>
		public bool Started {
			get { return started; }
		}		
		/// <summary>A list of all remotely connected clients to the bot.</summary>
		public List<RemoteClient> Clients = new List<RemoteClient>();
		#endregion
		
		private int _port;
		internal string _password;
		/// <summary>Used for sending message packets and stuff.</summary>
		internal ClassicBot MinecraftBot;
		internal bool started;
		
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
			listenThread.Name = "RemoteServerThread";
			listenThread.IsBackground = true;
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
		
		/// <summary>
		/// Thread which listens for remotely connected clients.
		/// </summary>
		private void ListenForClients()
		{
			started = true;
			TcpListener tcpListener = new TcpListener(IPAddress.Any,_port); //TODO: Add support for specified IP addresses.
			tcpListener.Start();
			while (true)
			{
				//Blocks the thread until a client had connected.
				TcpClient RemoteTcpClient = tcpListener.AcceptTcpClient();
				//new TcpClient() { Client = tcpListener.Server.Accept(); };
				SessionStartedEventArgs e = new SessionStartedEventArgs((IPEndPoint)RemoteTcpClient.Client.RemoteEndPoint,RemoteTcpClient);
				MinecraftBot.Events.RaiseSessionStarted(e);
				//Create a thread to handle communication with the connected remote client.
				RemoteClient client = new RemoteClient();
				Thread clientThread = new Thread(delegate() { client.Start(RemoteTcpClient,this); } );
				clientThread.Start();
				Clients.Add(client);
			}
		}
		
	}
}