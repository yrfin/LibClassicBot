using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using LibClassicBot.Drawing;
using LibClassicBot.Events;
using LibClassicBot.Networking;
using LibClassicBot.Remote;

namespace LibClassicBot
{
	public partial class ClassicBot
	{
		#region Public Fields
		/// <summary>Returns the X coordinate of the bot in the world.</summary>
		public float X {
			get { if (this._players[255] != null) return this._players[255].X; else return 0; }
		}

		/// <summary> Returns the Y coordinate of the bot in the world.</summary>
		public float Y {
			get { if (_players[255] != null) return _players[255].Y; else return 0; }
		}

		/// <summary>Returns the Z coordinate of the bot in the world.</summary>
		public float Z {
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
		public Dictionary<short, Player> Players {
			get { if (_players != null) return _players; else return null; }
		}
		
		/// <summary>The character delimiter used for the splitting of chat packets into message and username. By default, this will be ':'.</summary>
		public char Delimiter {
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

		/// <summary>Gets the current process associated with the bot.</summary>
		public Process BotProcess {
			get { return Process.GetCurrentProcess(); }
		}
		
		/*/// <summary>Returns the CPU usage of the bot.</summary>
		public double CpuUsage {
			get { Thread.Sleep(500);
				return cpuCounter.NextValue(); }
		}

		/// <summary>Returns the RAM usage of the bot in Megabytes.</summary>
		public double RamUsage {
			get { Thread.Sleep(500);
				return ramCounter.NextValue() / 1024 / 1024; }
		}*/
			
		/// <summary>
		/// Whether to load settings from botsettings.txt. By default, this is set to true.
		/// Disable this if you intend to enforce settings that might be overriden otherwise with the loaded user settings.
		/// </summary>
		public bool LoadInternalSettings {
			get { return _loadsettings; }
			set {_loadsettings = value; }
		}
		
		/// <summary>
		/// When the bot first joins a server, a packet containing the server name and MOTD are sent.
		/// All further world joins are ignored, as unfortunately there's no unified "joining world X" between
		/// all the different server software.
		/// </summary>
		public string ServerName {
			get {  if (_servername != null) return _servername; else return null; }
		}
		
		/// <summary>When the bot first joins a server, a packet containing the server name and MOTD are sent.</summary>
		public string ServerMOTD {
			get {  if (_servermotd != null) return _servermotd; else return null; }
		}
		
		/// <summary>This determines whether or not the bot is cuboiding.</summary>
		public bool IsCuboiding {
			get { return _isCuboiding; }
		}
		
		/// <summary>The time in milliseconds in which the bot will wait before placing the next block in a cuboid.
		/// If this is too low, the bot may be kicked for potential block spam.</summary>
		public int CuboidSleepTime {
			get { return sleepTime; }
			set { sleepTime = value; }
		}
		#endregion
		
		/// <summary>
		/// Determines if the given three coordinates are inside the map. If any of them are false, the boolean returns false.
		/// </summary>
		/// <returns>True if all three points were in the map, false if not.</returns>
		public bool IsValidPosition(short x, short y, short z)
		{
			if(x > _mapsizeX || y > _mapsizeY || z > _mapsizeZ || x < 0 || y < 0 || z < 0) return false;
			else return true;
		}
		/// <summary>Events that are raised by the bot.</summary>
		public BotEvents Events = new BotEvents();
		
		#region Public Constructors

		/// <summary>Connects the bot to a server via an address, username, and password.</summary>
		/// <param name="username">Username for the bot to use.</param>
		/// <param name="pass">Password for the bot to use.</param>
		/// <param name="hash">Hash for thre bot to use. Eg, r432fd57gf5j876f</param>
		/// <param name="oppath">String that spceifies where to look for a file containg a list of users allowed to use the bot.</param>
		/// <example>ClassicBot c = new ClassicBot("Bot","admin","r432fd57gf5j876f","ops.txt")</example>
		public ClassicBot(string username, string pass, string adress, string oppath)
		{
			this._username = username;
			this._password = pass;
			this._hash = adress;
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
		public ClassicBot(string username, string verkey, IPAddress serverIP, int port, string oppath)
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
		string _username, _password, _hash;
		string _ver = String.Empty;
		string _servername, _servermotd;
		bool _connected;
		int _mapsizeX, _mapsizeY, _mapsizeZ;
		char _delimiter = ':';
		string _Opspath = "operators.txt";
		bool _reconnectonkick = false;
		Dictionary<short, Player> _players = new Dictionary<short, Player>();
		byte _userType;
		bool _requiresop = true;
		Server server = null;
		List<string> _ignored = new List<string>();
		string migratedUsername;
		/*PerformanceCounter ramCounter = new PerformanceCounter("Process", "Working Set",
		                                                       Process.GetCurrentProcess().ProcessName, true);
		PerformanceCounter cpuCounter = new PerformanceCounter("Process", "% Processor Time",
		                                                       Process.GetCurrentProcess().ProcessName, true);*/
		private MemoryStream mapStream;
		//Drawing
		int sleepTime = 10;
		byte cuboidType;
		bool _isCuboiding = false;		
		const string ErrorInPage = "Error while parsing the URL. Either minecraft.net is down, or the URL given was invalid.";
		
		/// <summary>This is used to prevent the bot from continuing to try to login to a server. (Eg, after a ban.)</summary>
		bool CanReconnectAfterKick = false;
		string DirectURL;
		bool isDirect = false;
		bool isStandard = false;
		byte ProtocolVersion;
		bool _loadsettings = true;
		bool _savemap = false;
		bool serverLoaded = false;
		bool UseRemoteServer = false;
		#endregion

		/// <summary>
		/// Sends a message to all remotely connected clients. If the server has not been started,
		/// it will not send a message.
		/// </summary>
		/// <param name="message">The message to send to all remotely connected clients.</param>
		public void MessageAllRemoteClients(string message)
		{
			if (server != null && server.started) server.SendMessageToAllRemoteClients(message);
		}
		/// <summary>
		/// Sends a byte array to the currently connected server. (Usually, a packet of some sort.)
		/// </summary>
		/// <param name="data">The daat to be sent, converted into a byte array.</param>
		public void Send(byte[] data)
		{
			if(_serverSocket == null || _serverSocket.Connected == false) return;
			_serverSocket.Send(data, data.Length, SocketFlags.None);
		}
		/// <summary>
		/// Starts the bot. It can optionally run on the same thread as the method that called it.
		/// </summary>
		public void Start(bool RunOnThreadAsCaller)
		{
			if(!Debugger.IsAttached) AppDomain.CurrentDomain.UnhandledException += UnhandledException; //Might interfere with debugging
			if(_loadsettings) LoadSettings();
			if(RunOnThreadAsCaller == true)
			{
				IOLoop();
			}
			Thread thread = new Thread(IOLoop);
			thread.Name = "LibClassicBotIO";
			thread.IsBackground = true;
			thread.Start();
		}

		/// <summary>
		/// Raised when an exception is uncaught.
		/// </summary>
		void UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Exception actualException = (Exception)e.ExceptionObject; //Get actual exception.
			BotExceptionEventArgs socketEvent = new BotExceptionEventArgs("Unhandled exception, exiting.", actualException);
			Events.RaiseBotError(socketEvent);
			System.IO.File.WriteAllText("error.txt","");
			System.IO.File.AppendAllText("error.txt","Type of Exception - " + actualException.GetType() + Environment.NewLine);
			System.IO.File.AppendAllText("error.txt","StackTrace - " + actualException.StackTrace + Environment.NewLine);
			System.IO.File.AppendAllText("error.txt","Message - " + actualException.Message + Environment.NewLine);
			System.IO.File.AppendAllText("error.txt","Source - " + actualException.Source + Environment.NewLine);
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
		/// SaveMap: false<br/>
		private void LoadSettings()
		{
			try
			{
				if(File.Exists("botsettings.txt"))
				{
					int numberofspaces = 0;
					string[] Lines = File.ReadAllLines("botsettings.txt");
					string[] splitLineURS = Lines[0].Split(':');
					string urs = splitLineURS[1].Trim(); //Remove starting white space
					if((numberofspaces = urs.LastIndexOf(' ')) != -1) //-1 means no white space was found.
						urs = urs.Substring(0, numberofspaces); //Trim potential garbage data.
					bool.TryParse(urs, out UseRemoteServer);
					if (UseRemoteServer)
					{
						string[] splitLineRP = Lines[1].Split(':');
						int RemoteServerPort;
						Int32.TryParse(splitLineRP[1].Trim(), out RemoteServerPort); //Do not account for potential garbage data.
						
						string[] splitLineRPass = Lines[2].Split(':');
						string RemotePassword = splitLineRPass[1].Trim(); //Remove starting white space
						if((numberofspaces = RemotePassword.LastIndexOf(' ')) != -1) //-1 means no white space was found.
							RemotePassword = RemotePassword.Substring(0, numberofspaces); //Trim potential garbage data.
						server = new Server();
						server.Start(this,RemoteServerPort,RemotePassword);
					}
					string[] splitLineCRQ = Lines[3].Split(':');
					string crq = splitLineCRQ[1].Trim(); //Remove starting white space
					if((numberofspaces = crq.LastIndexOf(' ')) != -1) //-1 means no white space was found.
						crq = crq.Substring(0, numberofspaces); //Trim potential garbage data.
					bool.TryParse(crq, out _requiresop); //TODO: Try using parse instead, as this should not be false by default.
					
					string[] splitLineRecOnKick = Lines[4].Split(':');
					string rok = splitLineRecOnKick[1].Trim(); //Remove starting white space
					if((numberofspaces = rok.LastIndexOf(' ')) != -1) //-1 means no white space was found.
						rok = rok.Substring(0, numberofspaces); //Trim potential garbage data.
					bool.TryParse(rok, out _reconnectonkick);
					
					string[] splitLineSaveMap = Lines[5].Split(':');
					string sm = splitLineSaveMap[1].Trim(); //Remove starting white space
					if((numberofspaces = sm.LastIndexOf(' ')) != -1) //-1 means no white space was found.
						sm = sm.Substring(0, numberofspaces); //Trim potential garbage data.
					bool.TryParse(sm, out _savemap);
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
						sw.WriteLine("SaveMap: false");
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
						sw.WriteLine("#SaveMap - This determines if the bot will save the map when the chunk packets are sent to it." +
						             "If this is true, it will be saved as a fCraft compatible map. (Large maps of 512 x 512 x 512 " +
						             "can use up to ~150 megabytes of RAM when saving, so be wary. After saving, memory usage should return to normal.");
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
				try { Extensions.Login(_username, _password, _hash, out _serverIP, out _serverPort, out _ver, out migratedUsername); }
				catch(InvalidOperationException ex)
				{
					BotExceptionEventArgs socketEvent = new BotExceptionEventArgs(ex.Message,ex);
					Events.RaiseBotError(socketEvent);
					return;
				}
				catch(ArgumentOutOfRangeException ex)
				{
					BotExceptionEventArgs socketEvent = new BotExceptionEventArgs(ErrorInPage,ex);
					Events.RaiseBotError(socketEvent);
					return;
				}
			}
			//Get details we need to create a verified login.
			_ignored.Add(migratedUsername ?? _username); //Ignore self.
			byte[] ToSendLogin = CreateLoginPacket(migratedUsername ?? _username, _ver); //Make login
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
				BotExceptionEventArgs socketEvent = new BotExceptionEventArgs(HandleSocketError(ex.ErrorCode),ex);
				Events.RaiseBotError(socketEvent);
				return;
			}
			StartCommandsThread();
			Plugins.PluginManager.LoadPlugins(ref RegisteredCommands, this);
			BinaryReader reader = new BinaryReader(new NetworkStream(_serverSocket));

			while (true)
			{
				try
				{
					byte OpCode = reader.ReadByte();
					PacketEventArgs opcodeEvent = new PacketEventArgs((ServerPackets)OpCode);
					Events.RaisePacketReceived(opcodeEvent);
					
					switch ((ServerPackets)OpCode)
					{
						case ServerPackets.ServerIdentification://Server identification.
							{
								ProtocolVersion = reader.ReadByte(); //Get the protocol, should be seven.
								string Name = Encoding.ASCII.GetString(reader.ReadBytes(64)).Trim();
								string MOTD = Encoding.ASCII.GetString(reader.ReadBytes(64)).Trim();
								if(!serverLoaded)
								{
									_servername = Name;
									_servermotd = MOTD;
									serverLoaded = true;
								}
								_userType = reader.ReadByte();//Get the type. 0x64 = Op, 0x00 = Normal user.
							}
							break;

						case ServerPackets.Ping://0x01
							break; //Do nothing with it.

						case ServerPackets.LevelInitialize://0x02
							_players.Clear();//Start loading map, wipe players.
							if(_savemap) mapStream = new MemoryStream();
							CanReconnectAfterKick = true;
							break;


						case ServerPackets.LevelDataChunk://0x03
							{
								int ChunkLength = IPAddress.HostToNetworkOrder(reader.ReadInt16()); //Get the length of non null bytes
								byte[] ChunkData = reader.ReadBytes(1024); //Should always be 1024.
								byte progress = reader.ReadByte(); //Read the percentage, do nothing as of yet.
								if(_savemap) {
									byte[] chunkDataWithoutPadding = new byte[ChunkLength];
									Buffer.BlockCopy(ChunkData, 0, chunkDataWithoutPadding, 0, ChunkLength);
									mapStream.Write(chunkDataWithoutPadding, 0, chunkDataWithoutPadding.Length); }
								MapProgressEventArgs e = new MapProgressEventArgs(progress);
								Events.RaiseMapProgress(e);
							}
							break;

						case ServerPackets.LevelFinalize://0x04:
							{
								_mapsizeX = IPAddress.HostToNetworkOrder(reader.ReadInt16());
								_mapsizeZ = IPAddress.HostToNetworkOrder(reader.ReadInt16());
								_mapsizeY = IPAddress.HostToNetworkOrder(reader.ReadInt16()); //Yes, even map sizes are sent in the wrong order.
								_connected = true; //At this state, we've loaded the map and we're ready to send chat etc.
								if(_savemap) {
									mapStream.Seek(0, SeekOrigin.Begin);
									Map map = new Map(_mapsizeX, _mapsizeY, _mapsizeZ);
									using (GZipStream decompressed = new GZipStream(mapStream,CompressionMode.Decompress))
									{
										decompressed.Read(new byte[4],0,4); //Ignore size of stream.
										for (int z = 0; z < _mapsizeZ; z++)
										{
											for (int y = 0; y < _mapsizeY; y++)
											{
												for (int x = 0; x < _mapsizeX; x++)
												{
													byte next = (byte)decompressed.ReadByte();
													if(next != 255) map.SetBlock(x, y, z, next);
												}
											}
										}
									}
									mapStream.Dispose();
									map.Save("map_" + DateTime.Now.ToString("ddHHmmssfffffff")); //Formatting for DateTime.
									map.Dispose();
								}
								MapLoadedEventArgs e = new MapLoadedEventArgs();
								Events.RaiseMapLoaded(e);
							}
							break;

						case ServerPackets.SetBlock://0x06:
							{
								int blockX = IPAddress.HostToNetworkOrder(reader.ReadInt16());
								int blockZ = IPAddress.HostToNetworkOrder(reader.ReadInt16());
								int blockY = IPAddress.HostToNetworkOrder(reader.ReadInt16());
								byte blockType = reader.ReadByte();
								if(marksLeft > 0 && blockType == 39 && CubID != -1)
								{
									if(_players[CubID].X < 0 || _players[CubID].Y < 0 || _players[CubID].Z < 0) {
										SendMessagePacket("Error: You are too far away from the bot.");
										break;
									}
									
									if(new Vector3I(blockX, blockY, blockZ).Distance(new Vector3I(
										(int)_players[CubID].X, (int)_players[CubID].Y, (int)_players[CubID].Z)) > 11) {
										break; //Another player probably tried placing a block somewhere else.
									}
									marks[marks.Length - marksLeft] = new Vector3I(blockX, blockY, blockZ);
									marksLeft--; //Go from smallest to largest.
									if(marksLeft == 0 && QueuedDrawer != null) {
										Draw(QueuedDrawer, marks, cuboidType);
									}
								}
							}
							break;

						case ServerPackets.SpawnPlayer://0x07:
							{
								byte pID = reader.ReadByte();
								string playername = Encoding.ASCII.GetString(reader.ReadBytes(64)).Trim();
								_players[pID] = new Player(); //Create a new player to add to the list later.
								_players[pID].Name = playername; //Get name.
								_players[pID].X = IPAddress.HostToNetworkOrder(reader.ReadInt16()) / 32f; //Set the X of player.
								_players[pID].Z = IPAddress.HostToNetworkOrder(reader.ReadInt16()) / 32f; //Set the Z of the player. Yes, I know they're the wrong way around.
								_players[pID].Y = IPAddress.HostToNetworkOrder(reader.ReadInt16()) / 32f; //Set the Y of player.
								_players[pID].Yaw = reader.ReadByte();
								_players[pID].Pitch = reader.ReadByte();
								PositionEventArgs e = new PositionEventArgs(pID, _players[pID]);
								Events.RaisePlayerMoved(e);
							}
							break;

						case ServerPackets.PlayerTeleport://0x08
							{
								byte pID = reader.ReadByte();
								_players[pID].X = IPAddress.HostToNetworkOrder(reader.ReadInt16()) / 32f;
								_players[pID].Z = IPAddress.HostToNetworkOrder(reader.ReadInt16()) / 32f; //Z and Y are flipped.
								_players[pID].Y = IPAddress.HostToNetworkOrder(reader.ReadInt16()) / 32f;
								_players[pID].Yaw = reader.ReadByte();
								_players[pID].Pitch = reader.ReadByte();
								PositionEventArgs e = new PositionEventArgs(pID, _players[pID]);
								Events.RaisePlayerMoved(e);
							}
							break;

						case ServerPackets.PositionandOrientationUpdate://0x09
							{
								byte pID = reader.ReadByte();
								_players[pID].X += reader.ReadSByte() / 32f;
								_players[pID].Z += reader.ReadSByte() / 32f;//Z and Y are flipped.
								_players[pID].Y += reader.ReadSByte() / 32f;
								_players[pID].Yaw = reader.ReadByte();
								_players[pID].Pitch = reader.ReadByte();
								PositionEventArgs e = new PositionEventArgs(pID, _players[pID]);
								Events.RaisePlayerMoved(e);
							}
							break;

						case ServerPackets.PositionUpdate://0x0a
							{
								byte pID = reader.ReadByte();
								_players[pID].X += (reader.ReadSByte() / 32f);
								_players[pID].Z += (reader.ReadSByte() / 32f);//Z and Y are flipped.
								_players[pID].Y += (reader.ReadSByte() / 32f);
								PositionEventArgs e = new PositionEventArgs(pID, _players[pID]);
								Events.RaisePlayerMoved(e);
							}
							break;

						case ServerPackets.OrientationUpdate://0x0b
							{
								byte pID = reader.ReadByte();
								_players[pID].Yaw = reader.ReadByte();
								_players[pID].Pitch = reader.ReadByte();
								PositionEventArgs e = new PositionEventArgs(pID, _players[pID]);
								Events.RaisePlayerMoved(e);
							}
							break;

						case ServerPackets.DespawnPlayer://0x0c
							{
								byte playerID = reader.ReadByte();
								_players.Remove(playerID); //Remove user from the collection. Also sent when the player joins another map.
							}
							break;

						case ServerPackets.Message://0x0d
							{
								reader.ReadByte(); //Would be PlayerID, but most servers don't send the ID.
								string Line = Encoding.ASCII.GetString(reader.ReadBytes(64)).Trim();
								MessageEventArgs e = new MessageEventArgs(Line); //Raise the event, let the implementer take care of splitting.
								Events.RaiseChatMessage(e);
								if (Line.Contains(_delimiter.ToString())) { //Most servers use : as the delimiter.
									string[] lineSplit = Line.Split(new char[] { _delimiter }, 2);
									string Message = Extensions.StripColors(lineSplit[1]).TrimStart(' ');
									string User = Extensions.StripColors(lineSplit[0]);

									if (!_ignored.Contains(User))
									{
										string strippedcommandheader = Message.Split(' ')[0].ToLower();
										foreach(KeyValuePair<string,CommandDelegate> command in RegisteredCommands)
										{
											if(strippedcommandheader == "."+command.Key.ToLower())
											{
												if(_requiresop == true)
												{
													if(_users.Contains(User)) {
														EnqueueCommand(command.Value,Line);
														break;
													}
													else {
														SendMessagePacket(User +", you are not allowed to use the bot.");
													}
												}
												else {
													EnqueueCommand(command.Value,Line);
													break;
												}
											}
										}
									}
								}
								ProcessCommandQueue();

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
									thread.Name = "LibClassicBotIO";
									thread.IsBackground = true;
									thread.Start();
								}
								else if(_reconnectonkick == false) {
									BotExceptionEventArgs socketEvent = new BotExceptionEventArgs(
										"Kicked from the server.",new IOException());
									Events.RaiseBotError(socketEvent);
								}
								else {
									_serverSocket.Close();
									_serverSocket = null;
									BotExceptionEventArgs socketEvent = new BotExceptionEventArgs(
										"It looks like the bot was prevented from reconnecting. (Kick packet received before a LevelBegin packet)",new IOException());
									Events.RaiseBotError(socketEvent);
								}
								return;
							}

						case ServerPackets.SetPermission://0x0f
							_userType = reader.ReadByte();//Issued in Vanilla when someone calls /op, used in fCraft for bedrock permission checking.
							break;

						default:
							throw new IOException("Unrecognised packet. Opcode: " + OpCode.ToString());
					}
				}
				catch (IOException ex)
				{
					if(_reconnectonkick == true && CanReconnectAfterKick == true)
					{
						CancelDrawer();
						CanReconnectAfterKick = false;
						Thread thread = new Thread(IOLoop);
						thread.Name = "LibClassicBotIO";
						thread.IsBackground = true;
						thread.Start();
					}
					else
					{
						BotExceptionEventArgs socketEvent = new BotExceptionEventArgs(
							"An error has occurred, or the bot has been kicked without the kick packet being sent cleanly. Exiting bot.",ex);
						Events.RaiseBotError(socketEvent);
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
		public float X;
		/// <summary>Represents the Y position of the connected player. Classic sends Z and Y as OpenGL vectors,
		/// but the bot represents the Y and Z in normal form.</summary>
		public float Y;
		/// <summary>Represents the Z position of the connected player.</summary>
		public float Z;
		/// <summary>Represents the name of the connected player.</summary>
		public string Name;
		/// <summary>Represents the yaw of the player.</summary>
		public byte Yaw;
		/// <summary>Represents the pitch of the player.</summary>
		public byte Pitch;
	}
}