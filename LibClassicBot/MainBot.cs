using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using LibClassicBot.Networking;
using LibClassicBot.Remote;

namespace LibClassicBot
{
	public partial class ClassicBot
	{
		#region Public Fields
		/// <summary>Returns the X coordinate of the bot in the world.</summary>
		public int X {
			get { if (this._players[255] != null) return this._players[255].X; else return 0; }
		}

		/// <summary> Returns the Y coordinate of the bot in the world.</summary>
		public int Y {
			get { if (_players[255] != null) return _players[255].Y; else return 0; }
		}

		/// <summary>Returns the Z coordinate of the bot in the world.</summary>
		public int Z {
			get { if (_players[255] != null) return _players[255].Z; else return 0; }
		}

		/// <summary>Returns the Yaw of the bot in the world.</summary>
		public byte Yaw {
			get { if (_players[255] != null) return _players[255].Yaw; else return 0; }
		}

		/// <summary>Returns the Pitch of the bot in the world.</summary>
		public byte Pitch {
			get { if (_players[255] != null) return _players[255].Pitch; else return 0; }
		}

		/// <summary>Returns a list of users allowed to use the bot. Does NOT contain colour codes.</summary>
		public List<string> Users {
			get { return _users; }
			set { _users = value; }
		}

		/// <summary>Returns the current IP Address that the bot is connected to. Returns null if not connected.</summary>
		public IPAddress ServerIP {
			get { if (_serverIP != null) return _serverIP; else return null; }
		}

		/// <summary>Returns the current (int) port that the bot is connected to. Returns 0 if not connected to one.</summary>
		public int ServerPort {
			get { return _serverPort; }
		}

		/// <summary>Returns the current socket that the bot is connected to. Returns null if not connected to one.</summary>
		public Socket ServerSocket {
			get { if (_serverSocket != null) return _serverSocket; else return null; }
		}

		/// <summary>Returns the name of the bot as a string. Returns null if there isn't one.</summary>
		public string Username {
			get { if (_username != null) return _username; else return null; }
		}

		/// <summary>Returns the password of the bot as a string. Returns null if there isn't one.</summary>
		public string Password {
			get { if (_password != null) return _password; else return null; }
		}

		/// <summary>Returns the hash prefixed with http://minecraft.net/classic/play as a string. Returns null if there isn't a hash.</summary>
		public string URL {
			get { if (_hash != null) return _hash; else return null; }
		}

		/// <summary>Returns whether or not the bot is connected.</summary>
		public bool Connected {
			get { return _connected; }
		}

		/// <summary>Returns the Mppass of the bot as a string. Returns null if there isn't one.</summary>
		public string Mppass {
			get { if (_ver != null) return _ver; else return null; }
		}

		/// <summary>Returns the length of the X-Axis of the bots' current map. Returns 0 if a map is not loaded</summary>
		public int MapSizeX {
			get { return _mapsizeX; }
		}

		/// <summary>Returns the length of the Y-Axis of the bots' current map. Returns 0 if a map is not loaded</summary>
		public int MapSizeY {
			get { return _mapsizeY; }
		}

		/// <summary>Returns the length of the Z-Axis of the bots' current map. Returns 0 if a map is not loaded</summary>
		public int MapSizeZ {
			get { return _mapsizeZ; }
		}

		/// <summary>Returns the type of the user. 100 indicates an op and can place bedrock, 0 indicates normal.</summary>
		public byte UserType {
			get { return _userType; }
		}

		/// <summary>Returns a collection of players in the world. Returns null if no one (Including the bot) is in the collection.</summary>
		public Dictionary<int, Player> Players {
			get { if (_players != null) return _players; else return null; }
		}
		
		/// <summary>The character delimiter used for the splitting of chat packets into message and username. By default, this will be ':'.</summary>
		public char DeLimiter {
			get { return _delimiter; }
			set { _delimiter = value; }
		}
		
		/// <summary>Whether or not the bot should attempt to reconnect upon being kicked.</summary>
		public bool ReconnectOnKick {
			get { return _reconnectonkick; }
			set { _reconnectonkick = value; }
		}
		
		/// <summary>This determines whether or not the person who calls a command is required to be on the Operators list first. Defaults to true.</summary>
		public bool CommandsRequireOperator {
			get { return _requiresop; }
			set { _requiresop = value; }
		}
		
		/// <summary>Gets or sets the list of ignored users.</summary>
		public List<string> IgnoredUserList {
			get { return _ignored; }
			set { _ignored = value; }
		}
		
		/// <summary>
		/// Returns the server that remote clients connect to.
		/// </summary>
		public Server RemoteServer {
			get { return server; }
		}

		/// <summary>
		/// Gets the current process associated with the bot.
		/// </summary>
		public Process BotProcess {
			get { return Process.GetCurrentProcess(); }
		}

		#endregion
		
		/// <summary>Events that are raised by the bot.</summary>
		public ClassicBot.BotEvents Events = new ClassicBot.BotEvents();
		
		/// <summary>
		/// Generates a somewhat user friendly reason as to why the bot crashed, easier to understand than an error code.
		/// </summary>
		/// <param name="errorcode">The error code that the socket exception raised.</param>
		/// <returns>A string containing a user friendly reason as to the exception. If the error code is not in this list, the method
		/// will return Unhandled socket error: Error code : (errorcode)</returns>
		public string HandleSocketError(int errorcode)
		{
			switch(errorcode)
			{
					case -3: return "Error while parsing the URL. Either minecraft.net is down, or the URL given was invalid.";
					case -2: return "Wrong username or password, or the account has been migrated."; //Custom used errors.
					case -1: return "The port was either too large or too small.";
					case 8: return "Not enough memory to complete the socket operation.";
					case 995: return "The operation was aborted, check if the server closed and/or if something happened to your connection.";
					case 10013: return "Permission was denied. Another service may be exclusively using the socket.";
					case 10037: return "Socket already in use. Did you try to call connect on the same address twice?";
					case 10038: return "Attempted to perform an operation on a non socket. Something went wrong.";
					case 10039: return "No destination address was entered into the socket. This can also happen if you use IPAddress.Any";
					case 10041: return "Incorrect protocol. Check you are connecting to a classic server.";
					case 10043: return "Protocol type not supported. Check you are connecting to a classic server.";
					case 10044: return "Socket type not supported. Check you are connecting to a classic server.";
					case 10047: return "Incompatible address family. Check you are connecting to a classic server.";
					case 10048: return "The socket the bot attempted to connect on is already in use by another application.";
					case 10049: return "The address was invalid. The supplied IP address or port is probably invalid.";
					case 10056: return "The socket is already connected. The socket may be in use by another application.";
					case 10057: return "The socket is not connected. This should not happen under normal circumstances.";
					case 10060: return "The connection timed out. This may be a problem with the server or your connection.";
					case 10061: return "Unable to connect to the server, as the connection was refused. Check if the server is up and if it is port forwarded.";
					case 10064: return "Unable to connect to remote server. Check if the server is up and port forwarded.";
					default : return String.Format("Unhandled socket error. Error code : {0}",errorcode);
			}
		}

		#region Public Packets
		/// <summary>Sends a setblock packet at the specified coordinates.</summary>
		/// <param name="mode">Mode of whether to place or delete. 1 indicates placing, 0 indicates deleting.</param>
		/// <param name="type">The type of block being placed, as a byte.</param>
		public void SendBlockPacket(short x, short y, short z, byte mode, byte type)
		{
			if (_serverSocket == null || _serverSocket.Connected == false ) return;
			using (MemoryStream blockMemStream = new MemoryStream())
				using (BinaryWriter BlockWriter = new BinaryWriter(blockMemStream))
			{
				BlockWriter.Write((byte)5); //PacketID
				BlockWriter.Write(IPAddress.HostToNetworkOrder(x));
				BlockWriter.Write(IPAddress.HostToNetworkOrder(z)); //Yes I know they're the wrong way around.
				BlockWriter.Write(IPAddress.HostToNetworkOrder(y));
				BlockWriter.Write(mode);
				BlockWriter.Write(type);
				_serverSocket.Send(blockMemStream.ToArray());
			}
		}

		/// <summary>Sends a teleportation packet at the specified coordinates.</summary>
		/// <param name="x">A short marking the point along the X-Axis the block is to be placed at.</param>
		/// <param name="y">A short marking the point along the Y-Axis the block is to be placed at.</param>
		/// <param name="z">A short marking the point along the Z-Axis the block is to be placed at.</param>
		public void SendPositionPacket(short x, short y, short z)
		{
			if (_serverSocket == null || _serverSocket.Connected == false ) return;
			if(!_players.ContainsKey(255)) return;
			if (IsValidPosition(x,y,z) == false) return;
			_players[255].X = x; //Set the X of the bot.
			_players[255].Y = y; //Set the Z of the bot.
			_players[255].Z = z; //Set the Y of the bot.
			byte[] packet = new byte[10];
			packet[0] = (byte)0x08; //Packet ID.
			packet[1] = (byte)255; //Player ID of self.
			short xConverted = IPAddress.HostToNetworkOrder((short)(x * 32));
			packet[2] = (byte)(xConverted);
			packet[3] = (byte)(xConverted >> 8);
			short yConverted = IPAddress.HostToNetworkOrder((short)(z * 32));//((z + 1.21f) * 32)); character height
			packet[4] = (byte)(yConverted);
			packet[5] = (byte)(yConverted >> 8);
			short zConverted = IPAddress.HostToNetworkOrder((short)(y * 32));//Yes, I know they're the wrong way around.
			packet[6] = (byte)(zConverted);
			packet[7] = (byte)(zConverted >> 8);
			packet[8] = _players[255].Yaw;
			packet[9] = _players[255].Pitch;
			_serverSocket.Send(packet);
			PositionEventArgs e = new PositionEventArgs(255, _players[255].Name, _players[255].X, _players[255].Y, _players[255].Z, _players[255].Yaw, _players[255].Pitch);
			Events.RaisePlayerMoved(e);
		}

		/// <summary>Sends a chat message in game. Has a maximum length of 64.</summary>
		/// <param name="message">String to send.</param>
		public void SendMessagePacket(string message)
		{
			if(message.Length > 64) message = message.Substring(0,64);
			if (_serverSocket == null || _serverSocket.Connected == false ) return;
			byte[] packet = new byte[66]; //PID + unused + message
			packet[0] = (byte)0x0d; //Packet ID.
			packet[1] = (byte)0xff; //Unused
			Buffer.BlockCopy(Extensions.StringToBytes(message), 0, packet, 2, 64);
			this._serverSocket.Send(packet);
		}
		#endregion
		
		
		#region Public Constructors

		/// <summary>Connects the bot to a server via a hash, username, and password. Hash does NOT start with http://minecraft.net/classic/play/.</summary>
		/// <param name="username">Username for the bot to use.</param>
		/// <param name="pass">Password for the bot to use.</param>
		/// <param name="hash">Hash for thre bot to use. Eg, r432fd57gf5j876f</param>
		/// <param name="oppath">String that spceifies where to look for a file containg a list of users allowed to use the bot.</param>
		/// <example>ClassicBot c = new ClassicBot("Bot","admin","r432fd57gf5j876f","ops.txt")</example>
		public ClassicBot(string username, string pass, string hash, string oppath)
		{
			this._username = username;
			this._password = pass;
			this._hash = hash;
			this._Opspath = oppath;
			this.isStandard = true;
		}

		/// <summary>Connects the bot to a server via a direct URL. A direct URL goes in the form of mc://serverip:serverport/user/mppass</summary>
		/// <param name="directURL">String that contains a direct URL for the bot to connect to.</param>
		/// <param name="oppath">String that spceifies where to look for a file containg a list of users allowed to use the bot.</param>
		/// <example>ClassicBot c = new ClassicBot("mc://127.0.0.1:25565/Bot/ver","ops.txt")</example>
		public ClassicBot(string directURL,string oppath)
		{
			this.DirectURL = directURL;
			this.isDirect = true;
			this._Opspath = oppath;
		}
		
		/// <summary>Directly connects to a server. Does not include verification, so it will be kicked if server verification is enabled.</summary>
		/// <param name="username"></param>
		/// <param name="ServerIP">An IPAdress which the bot to connect to.</param>
		/// <param name="Port">The int containing the port number for which to connect onto.</param>
		/// <param name="oppath">String that spceifies where to look for a file containg a list of users allowed to use the bot.</param>
		/// <example>ClassicBot c = new ClassicBot("bot",IPAddress.Parse("127.0.0.1",25565,"ops.txt")</example>
		public ClassicBot(string username, IPAddress serverIP, int port, string oppath)
		{
			this._username = username;
			this._serverIP = serverIP;
			this._serverPort = port;
			this._Opspath = oppath;
		}
		
		/// <summary>Directly connects to a server. Does include verification.</summary>
		/// <param name="username">Username used to connect</param>
		/// <param name="verkey">Verification key to use, as a string.</param>
		/// <param name="ServerIP">An IPAddress which the bot to connect to.</param>
		/// <param name="Port">The int containing the port number for which to connect onto.</param>
		/// <param name="oppath">String that spceifies where to look for a file containg a list of users allowed to use the bot.</param>
		/// <example>ClassicBot c = new ClassicBot("bot","ver",IPAddress.Parse("127.0.0.1",25565,"ops.txt")</example>
		public ClassicBot(string username, string verkey,IPAddress serverIP, int port, string oppath)
		{
			this._username = username;
			this._serverIP = serverIP;
			this._serverPort = port;
			this._Opspath = oppath;
			this._ver = verkey;
		}
		#endregion

		
		#region Internal fields
		IPAddress _serverIP;
		Socket _serverSocket;
		List<string> _users = new List<string>();
		int _serverPort;
		string _username;
		string _password;
		string _hash;
		bool _connected;
		string _ver = String.Empty;
		int _mapsizeX;
		int _mapsizeY;
		int _mapsizeZ;
		char _delimiter = ':';
		string _Opspath = "operators.txt";
		bool _reconnectonkick = false;
		Dictionary<int, Player> _players = new Dictionary<int, Player>();
		byte _userType;
		bool _requiresop = true;
		Server server = new Server();
		List<string> _ignored = new List<string>();
		CommandsClass CommandClass = new CommandsClass();
		
		private bool IsValidPosition(short x, short y, short z)
		{
			if(x > _mapsizeX || y > _mapsizeY || z > _mapsizeZ || x < 0 || y < 0 || z < 0) return false;
			else return true;
		}
		/// <summary>This is used to prevent the bot from continuing to try to login to a server. (Eg, after a ban.)</summary>
		bool CanReconnectAfterKick = false;
		string DirectURL;
		bool isDirect = false;
		bool isStandard = false;
		byte ProtocolVersion;
		#endregion


		#region Packet making
		private static byte[] CreateLoginPacket(string username, string verificationKey)
		{
			MemoryStream loginMemory = new MemoryStream(131); //Slightly faster, but not noticable really. < 1000 ticks.
			BinaryWriter loginWriter = new BinaryWriter(loginMemory);
			loginWriter.Write((byte)0);//Packet ID
			loginWriter.Write((byte)0x07);//Protocol version
			Extensions.WriteString(loginWriter, username);
			Extensions.WriteString(loginWriter, verificationKey);//Also known as mppass.
			loginWriter.Write((byte)0);//Unused
			return loginMemory.ToArray(); //Although this could be done more efficiently with Array.Copy when repeated
			//millions of times, it doesn't really make a difference for one.
		}

		public void SendLongChat(string message, params object[] args)
		{
			if (args.Length > 0)
			{
				message = String.Format(message, args);
			}
			message = Extensions.StripColors(message);
			StringBuilder part1 = new StringBuilder();
			StringBuilder part2 = new StringBuilder();
			StringBuilder part3 = new StringBuilder();
			StringBuilder part4 = new StringBuilder();
			StringBuilder part5 = new StringBuilder();
			for (int i = 0; i < message.Length; i++)
			{
				if (i <= 48) { part1.Append(message[i]); }
				else if (i > 48 && i <= 96) { part2.Append(message[i]); }
				else if (i > 96 && i <= 144) { part3.Append(message[i]); }
				else if (i > 144 && i <= 192) { part4.Append(message[i]); }
				else if (i > 192 && i <= 240) { part5.Append(message[i]); }
			}
			if (!String.IsNullOrEmpty(part1.ToString())) { SendMessagePacket(part1.ToString()); }
			if (!String.IsNullOrEmpty(part2.ToString())) { SendMessagePacket(part2.ToString()); }
			if (!String.IsNullOrEmpty(part3.ToString())) { SendMessagePacket(part3.ToString()); }
			if (!String.IsNullOrEmpty(part4.ToString())) { SendMessagePacket(part4.ToString()); }
			if (!String.IsNullOrEmpty(part5.ToString())) { SendMessagePacket(part5.ToString()); }
		}
		#endregion

		/// <summary>
		/// Starts the bot. It can optionally run on the same thread as the method that called it.
		/// </summary>
		public void Start(bool RunOnSameThreadAsCaller)
		{
			LoadSettings();
			if(RunOnSameThreadAsCaller == true)
			{
				IOLoop();
			}
			Thread thread = new Thread(IOLoop);
			thread.Start();
		}
		
		/// <summary>
		/// Loads all settings from a file called botsettings.txt
		/// </summary>
		/// <remarks>A structure looks like the following:<br/>
		/// UseRemoteServer: false<br/>
		/// RemotePort:<br/>
		/// RemotePassword (No spaces allowed):<br/>
		/// CommandsRequireOperator: false<br/>
		/// ReconnectAfterKick: true<br/>
		private void LoadSettings()
		{
			try
			{
				if(File.Exists("botsettings.txt"))
				{
					string[] Lines = File.ReadAllLines("botsettings.txt");
					string[] splitLineURS = Lines[0].Split(':');
					bool UseRemoteServer;
					bool.TryParse(splitLineURS[1].Replace(" ",""), out UseRemoteServer);
					if (UseRemoteServer)
					{
						string[] splitLineRP = Lines[1].Split(':');
						int RemoteServerPort;
						Int32.TryParse(splitLineRP[1].Replace(" ",""), out RemoteServerPort);
						
						string[] splitLineRPass = Lines[2].Split(':');
						string RemotePassword = splitLineRPass[1].Replace(" ","");
						server.Start(this,RemoteServerPort,RemotePassword);
					}
					string[] splitLineCRQ = Lines[3].Split(':');
					bool.TryParse(splitLineCRQ[1].Replace(" ",""), out _requiresop);
					
					string[] splitLineRAK = Lines[3].Split(':');
					bool.TryParse(splitLineRAK[1].Replace(" ",""), out _reconnectonkick);
				}
				else
				{
					using (StreamWriter sw = new StreamWriter("botsettings.txt"))
					{
						sw.WriteLine("UseRemoteServer: false");
						sw.WriteLine("RemotePort: ");
						sw.WriteLine("RemotePassword (No spaces allowed): ");
						sw.WriteLine("CommandsRequireOperator: true");
						sw.WriteLine("ReconnectAfterKick: true");
						sw.WriteLine("#And now, a little explaination on what all these mean.");
						sw.WriteLine("#UseRemoteServer - Allows remote clients to connect and perform actions on the bot / chat through it. By default, this is disabled." +
						             "If you choose to use the remote function, you may need to forward the port and/or add an exception to your firewall.");
						sw.WriteLine("#RemotePort - The port the server will listen on for remote clients. It is fine to leave this blank if UseRemoteServer is false.");
						sw.WriteLine("#RemotePassword - The password to use for verifying remote clients. " +
						             "It is fine to leave this blank if you are not using the remote functionsd of the bot.");
						sw.WriteLine("#CommandsRequireOperators - This determines whether bot commands require the person who called them to be in the operators file." +
						             "Usually you would want this to be true.");
						sw.WriteLine("#ReconnectAfterKick - This determines if the bot will reconnect after being kicked. Note that if the bot receives a kick packet before" +
						             "a ServerIdentification packet, it will abort, and assume it has been banned from connecting.");
						
						
					}
				}
				
			}
			catch {}
		}

		/// <summary>Adds a username to the list of allowed users.</summary>
		/// <param name="username">The username to add. Does not contain colour codes.</param>
		/// <param name="savetofile">Whether to save a copy of the allowed users list to the operators file.</param>
		/// <returns>True if added the user.
		/// True if it also succeeded in writing to the file, false if there was an error.</returns>
		public bool AddOperator(string username, bool savetofile)
		{
			_users.Add(username);
			if(savetofile)
			{
				string[] usersArray = _users.ToArray();
				try { File.WriteAllLines(_Opspath,usersArray); }
				catch { return false; }
				
			}
			return true; //Add doesn't ever return false.
		}

		/// <summary>Removes a username from the list of allowed users.</summary>
		/// <param name="username">The username to remove. Does not contain colour codes.</param>
		/// <param name="savetofile">Whether to save a copy of the allowed users list to the operators file.</param>
		/// <returns>True if the user was removed, false if the user was not found.
		/// True if it also succeeded in writing to the file, false if there was an error.</returns>
		public bool RemoveOperator(string username, bool savetofile)
		{
			foreach(string name in _users)
			{
				if(name == username)
				{
					if(_users.Remove(name) == false) return false;
				}
			}
			
			if(savetofile)
			{
				string[] usersArray = _users.ToArray();
				try { File.WriteAllLines(_Opspath,usersArray); }
				catch { return false; }
			}
			return true;
		}

		#region I/O Loop
		void IOLoop()
		{
			if (File.Exists(_Opspath))
			{
				string[] lines = File.ReadAllLines(_Opspath);
				for (int i = 0; i < lines.Length; i++)
				{
					_users.Add(lines[i]);
				}
			}
			else
			{
				File.Create(_Opspath);
			}
			
			if (isDirect == true)
			{
				string[] parts = DirectURL.Substring(5).Split('/');
				_username = parts[1];
				_ver = parts[2];

				string[] ipPort = parts[0].Split(':');
				_serverIP = IPAddress.Parse(ipPort[0]);
				_serverPort = Convert.ToInt32(ipPort[1]);
			}
			else if(isStandard == true)
			{
				try { Extensions.Login(_username, _password, _hash, out this._serverIP, out this._serverPort, out this._ver); }
				catch(InvalidOperationException)
				{
					SocketException ex = new SocketException(-2);
					SocketExceptionEventArgs socketEvent = new SocketExceptionEventArgs(HandleSocketError(-2),ex);
					Events.RaiseBotSocketError(socketEvent);
					return;
				}
				catch(ArgumentOutOfRangeException)
				{
					SocketException ex = new SocketException(-3);
					SocketExceptionEventArgs socketEvent = new SocketExceptionEventArgs(HandleSocketError(-3),ex);
					Events.RaiseBotSocketError(socketEvent);
					return;
				}				
			}
			//Get details we need to create a verified login.
			_ignored.Add(_username); //Ignore self.
			byte[] ToSendLogin = CreateLoginPacket(_username, _ver); //Make login
			_serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); //Here we start a new client.
			try
			{
				if(_serverPort > IPEndPoint.MaxPort || _serverPort < IPEndPoint.MinPort) throw new SocketException(-1);
				//Do not continue if the user attempts to connect on a port larger than possible.
				_serverSocket.Connect(_serverIP, _serverPort);
				_serverSocket.Send(ToSendLogin);
			}
			catch(SocketException ex)
			{
				SocketExceptionEventArgs socketEvent = new SocketExceptionEventArgs(HandleSocketError(ex.ErrorCode),ex);
				Events.RaiseBotSocketError(socketEvent);
				return;
			}
			CommandClass.Start(true);
			BinaryReader reader = new BinaryReader(new NetworkStream(_serverSocket));

			while (true)
			{
				try
				{
					byte OpCode = reader.ReadByte();
					PacketEventArgs opcodeEvent = new PacketEventArgs(OpCode);
					Events.RaisePacketReceived(opcodeEvent);
					
					switch ((ServerPackets)OpCode)
					{
						case ServerPackets.ServerIdentification://Server identification.
							{
								ProtocolVersion = reader.ReadByte(); //Get the protocol, should be seven.
								string serverName = Encoding.ASCII.GetString(reader.ReadBytes(64)).Trim();
								string serverMOTD = Encoding.ASCII.GetString(reader.ReadBytes(64)).Trim();
								//Read the Name & MOTD, nothing with it.
								_userType = reader.ReadByte();
								//Get the type. 0x64 = Op, 0x00 = Normal user.
								CanReconnectAfterKick = true;
							}
							break;

						case ServerPackets.Ping://0x01
							break; //Do nothing with it.

						case ServerPackets.LevelInitialize://0x02
							_players.Clear();//Start loading map, wipe players.
							break;


						case ServerPackets.LevelDataChunk://0x03
							{
								int ChunkLength = IPAddress.HostToNetworkOrder(reader.ReadInt16()); //Get the length of non null bytes
								byte[] ChunkData = reader.ReadBytes(1024); //Should always be 1024.
								reader.ReadByte(); //Read the percentage, do nothing.
							}
							break;

						case ServerPackets.LevelFinalize://0x04:
							{
								_mapsizeX = IPAddress.HostToNetworkOrder(reader.ReadInt16());
								_mapsizeZ = IPAddress.HostToNetworkOrder(reader.ReadInt16());
								_mapsizeY = IPAddress.HostToNetworkOrder(reader.ReadInt16()); //Yes, even map sizes are sent in the wrong order.
								_connected = true; //At this state, we've loaded the map and we're ready to send chat etc.
							}
							break;

						case ServerPackets.SetBlock://0x06:
							{
								int blockX = IPAddress.HostToNetworkOrder(reader.ReadInt16());
								int blockY = IPAddress.HostToNetworkOrder(reader.ReadInt16());
								int blockZ = IPAddress.HostToNetworkOrder(reader.ReadInt16());
								byte blockType = reader.ReadByte();

							}
							break;

						case ServerPackets.SpawnPlayer://0x07:
							{
								byte pID = reader.ReadByte();
								string playername = Encoding.ASCII.GetString(reader.ReadBytes(64)).Trim();
								_players[pID] = new Player(); //Create a new player to add to the list later.
								_players[pID].Name = playername; //Get name.
								_players[pID].X = IPAddress.HostToNetworkOrder(reader.ReadInt16()) / 32; //Set the X of player.
								_players[pID].Z = IPAddress.HostToNetworkOrder(reader.ReadInt16()) / 32; //Set the Z of the player. Yes, I know they're the wrong way around.
								_players[pID].Y = IPAddress.HostToNetworkOrder(reader.ReadInt16()) / 32; //Set the Y of player.
								_players[pID].Yaw = reader.ReadByte();
								_players[pID].Pitch = reader.ReadByte();
								PositionEventArgs e = new PositionEventArgs(pID, _players[pID].Name, _players[pID].X, _players[pID].Y, _players[pID].Z, _players[pID].Yaw, _players[pID].Pitch);
								Events.RaisePlayerMoved(e);
							}
							break;

						case ServerPackets.PlayerTeleport://0x08
							{
								byte pID = reader.ReadByte();
								_players[pID].X = IPAddress.HostToNetworkOrder(reader.ReadInt16()) / 32;
								_players[pID].Z = IPAddress.HostToNetworkOrder(reader.ReadInt16()) / 32; //Z and Y are flipped.
								_players[pID].Y = IPAddress.HostToNetworkOrder(reader.ReadInt16()) / 32;
								_players[pID].Yaw = reader.ReadByte();
								_players[pID].Pitch = reader.ReadByte();
								PositionEventArgs e = new PositionEventArgs(pID, _players[pID].Name, _players[pID].X, _players[pID].Y, _players[pID].Z, _players[pID].Yaw, _players[pID].Pitch);
								Events.RaisePlayerMoved(e);
							}
							break;

						case ServerPackets.PositionandOrientationUpdate://0x09
							{
								byte pID = reader.ReadByte();
								_players[pID].X += reader.ReadSByte() / 32;
								_players[pID].Z += reader.ReadSByte() / 32; //Z and Y are flipped.
								_players[pID].Y += reader.ReadSByte() / 32;
								_players[pID].Yaw = reader.ReadByte();
								_players[pID].Pitch = reader.ReadByte();
								PositionEventArgs e = new PositionEventArgs(pID, _players[pID].Name, _players[pID].X, _players[pID].Y, _players[pID].Z, _players[pID].Yaw, _players[pID].Pitch);
								Events.RaisePlayerMoved(e);
							}
							break;

						case ServerPackets.PositionUpdate://0x0a
							{
								byte playerID = reader.ReadByte();
								_players[playerID].X += reader.ReadSByte() / 32;
								_players[playerID].Z += reader.ReadSByte() / 32; //Z and Y are flipped.
								_players[playerID].Y += reader.ReadSByte() / 32;
								PositionEventArgs e = new PositionEventArgs(playerID, _players[playerID].Name, _players[playerID].X, _players[playerID].Y, _players[playerID].Z, _players[playerID].Yaw, _players[playerID].Pitch);
								Events.RaisePlayerMoved(e);
							}
							break;

						case ServerPackets.OrientationUpdate://0x0b
							{
								byte playerID = reader.ReadByte();
								_players[playerID].Yaw = reader.ReadByte();
								_players[playerID].Pitch = reader.ReadByte();
								PositionEventArgs e = new PositionEventArgs(playerID, _players[playerID].Name, _players[playerID].X, _players[playerID].Y, _players[playerID].Z, _players[playerID].Yaw, _players[playerID].Pitch);
								Events.RaisePlayerMoved(e);
							}
							break;

						case ServerPackets.DespawnPlayer://0x0c
							{
								byte playerID = reader.ReadByte();
								_players.Remove(playerID); //Remove user from collection. Is also sent when the player joins another map.
							}
							break;

						case ServerPackets.Message://0x0d
							{
								
								reader.ReadByte(); //Would be PlayerID, but most servers don't send the ID.
								string Line = Encoding.ASCII.GetString(reader.ReadBytes(64)).Trim();
								MessageEventArgs e = new MessageEventArgs(Line); //No way to know who the user and who the message is.
								Events.RaiseChatMessage(e);
								if (Line.Contains(_delimiter.ToString()))
									//By default, servers use : as the delimiter.
								{
									string[] lineSplit = Line.Split(new char[] { _delimiter }, 2);
									string Message = Extensions.StripColors(lineSplit[1]).TrimStart(' ');
									string User = Extensions.StripColors(lineSplit[0]);

									if (!_ignored.Contains(User))
									{
										try
										{
											string strippedcommandheader = Message.Split(' ')[0].ToLower();
											foreach(KeyValuePair<string,CommandDelegate> command in RegisteredCommands)
											{
												if(strippedcommandheader == "."+command.Key.ToLower())
												{
													if(_requiresop == true)
													{
														if(_users.Contains(User))
														{
															CommandClass.EnqueueCommand(command.Value,Line);
															break;
														}
														else
														{
															SendMessagePacket(User +", you are not allowed to use the bot.");
														}
													}
													
													else
													{
														CommandClass.EnqueueCommand(command.Value,Line);
														break;
													}
												}
											}
										}
										
										catch (IndexOutOfRangeException)
										{
											SendMessagePacket("Error: Wrong number of arguements supplied to command.");
										}
										
									}
								}
								CommandClass.ProcessCommandQueue();

								string logFileName = ("log-" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt");
								string logLine = DateTime.Now.ToString("[yyyy-MM-dd : HH-mm-ss] ");
								File.AppendAllText(logFileName,logLine + Extensions.StripColors(Line).TrimEnd(' ') + Environment.NewLine);
							}
							break;

						case ServerPackets.DisconnectSelf://0x0e
							{
								string reason = Encoding.ASCII.GetString(reader.ReadBytes(64)).Trim();
								KickedEventArgs e = new KickedEventArgs(reason, CanReconnectAfterKick); //No way to know who the user and who the message is.
								Events.RaiseGotKicked(e);
								if(_reconnectonkick == true && CanReconnectAfterKick == true)
								{
									CanReconnectAfterKick = false;
									Thread thread = new Thread(IOLoop);
									thread.Start();
								}
								else if(_reconnectonkick == false)
								{
									Console.WriteLine("Bot was kicked from the server. Exiting.");
								}
								else
								{
									Console.WriteLine("It looks like the bot was banned from reconnecting. Aborting. (Kick packet received before a server identification packet)");
								}
								return;
							}

						case ServerPackets.SetPermission://0x0f
							_userType = reader.ReadByte();//Issued in Vanilla when someone calls /op, used in fCraft for bedrock permission checking.
							break;


						default:
							throw new IOException("Unrecognised packet.");
					}
				}
				catch (IOException ex)
				{
					Console.WriteLine("Inner exception: "+ ex.Message);
					if(_reconnectonkick == true && CanReconnectAfterKick == true)
					{
						Console.WriteLine("Attempting to reconnect once.");
						CanReconnectAfterKick = false;
						Thread thread = new Thread(IOLoop);
						thread.Start();
					}
					else
					{
						Console.WriteLine("An error has occurred with the socket. Exiting bot.");
					}
					return;
				}
			}
		}
		#endregion
		
	}
	/// <summary>Represents a connected player.</summary>
	public class Player
	{
		/// <summary>Represents the X position of the connected player.</summary>
		public int X;
		/// <summary>Represents the Y position of the connected player. Classic sends Z and Y in the wrong order, but the bot represents the Y and Z correctly.</summary>
		public int Y;
		/// <summary>Represents the Z position of the connected player.</summary>
		public int Z;
		/// <summary>Represents the name of the connected player.</summary>
		public string Name;
		/// <summary>Represents the yaw of the player.</summary>
		public byte Yaw;
		/// <summary>Represents the pitch of the player.</summary>
		public byte Pitch;
	}
}

