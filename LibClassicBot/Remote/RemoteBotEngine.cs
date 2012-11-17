using System;
using System.Collections.Generic;
namespace ClassicBotCore
{
	/// <summary>
	/// Description of RemoteBotEngine.
	/// </summary>
	public partial class RemoteBotEngine
	{
		internal Server RemoteServer;
		
		/// <summary>The password used to verify remotely connecting users.</summary>
		public string RemoteVerificationPassword
		{
			get { return _password; }
			set { _password = value; }
		}
		
		/// <summary>The password used to verify remotely connecting users.</summary>
		public int RemoteServerPort
		{
			get { return _port; }
			set { _port = value; }
		}
		
		private int _port;
		
		private string _password;
		
		internal ClassicBot MinecraftBot;
		
		public RemoteBotEngine.RemoteEvents RemoteBotEvents;
		

		/// <summary>Creates an instance of the Remote Bot Base. Use Start(Bot,Port,Pass) to actually start listening.</summary>
		public RemoteBotEngine()
		{
			RemoteBotEvents = new RemoteBotEngine.RemoteEvents();
		}
		/// <summary>
		/// Starts a server for remote clients to connect to on the specified port, with the specified password.
		/// </summary>
		/// <param name="MainBot">The main bot, which is used for sending messages from remote clients.</param>
		/// <param name="remotePort">The port to listen on.</param>
		/// <param name="remotePassword">The password to use. Does not have to be the same as the password of the bots account.</param>
		public void Start(ClassicBot MainBot, int remotePort, string remotePassword)
		{
			RemoteServer = new Server(remotePort,this);
			MinecraftBot = MainBot;
			_password = remotePassword;
			_port = remotePort;
		}
	}
}
