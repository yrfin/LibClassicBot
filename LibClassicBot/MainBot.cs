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
			get { if (Players[255] != null) return Players[255].X; return 0; }
		}

		/// <summary> Returns the Y coordinate of the bot in the world.</summary>
		public float Y {
			get { if (Players[255] != null) return Players[255].Y; return 0; }
		}

		/// <summary>Returns the Z coordinate of the bot in the world.</summary>
		public float Z {
			get { if (Players[255] != null) return Players[255].Z; return 0; }
		}

		/// <summary>Returns the Yaw of the bot in the world.</summary>
		public byte Yaw {
			get { if (Players[255] != null) return Players[255].Yaw; return 0; }
		}

		/// <summary>Returns the Pitch of the bot in the world.</summary>
		public byte Pitch {
			get { if (Players[255] != null) return Players[255].Pitch; return 0; }
		}

		/// <summary>Returns a list of users allowed to use the bot. Does NOT contain colour codes.</summary>
		public List<string> Users { get; private set; }

		/// <summary> Returns the endpoint (IP address and port) that the bot is connected to. Returns null if not connected.</summary>
		public IPEndPoint EndPoint { get; private set; }

		/// <summary>Returns the current socket that the bot is connected to. Returns null if not connected to one.</summary>
		public Socket ServerSocket { get; private set; }

		/// <summary>Returns the name of the bot as a string. Returns null if there isn't one.</summary>
		public string Username { get { return _username; } }

		/// <summary>Returns the password of the bot as a string. Returns null if there isn't one.</summary>
		public string Password { get { return _password; } }

		/// <summary>Returns the hash prefixed with http://minecraft.net/classic/play as a string. Returns null if there isn't a hash.</summary>
		public string URL { get { return _hash; } }

		/// <summary>Returns whether or not the bot is connected.</summary>
		public bool Connected { get; private set; }

		/// <summary>Returns the Mppass of the bot as a string. Returns null if there isn't one.</summary>
		public string Mppass { get { return _ver; } }

		/// <summary>Returns the length of the X-Axis of the bots' current map. Returns 0 if a map is not loaded</summary>
		public int MapSizeX { get; private set; }

		/// <summary>Returns the length of the Y-Axis of the bots' current map. Returns 0 if a map is not loaded</summary>
		public int MapSizeY { get; private set; }

		/// <summary>Returns the length of the Z-Axis of the bots' current map. Returns 0 if a map is not loaded</summary>
		public int MapSizeZ { get; private set; }

		/// <summary>Returns the type of the user. 100 indicates an op and can place bedrock, 0 indicates normal.</summary>
		public byte UserType { get; private set; }

		/// <summary>Returns a collection of players in the world. Includes the bot. </summary>
		public Dictionary<byte, Player> Players { get; private set; }
		
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
		public List<string> IgnoredUserList { get; private set; }

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
			public bool LoadInternalSettings { get; private set; }
		
		/// <summary>
		/// When the bot first joins a server, a packet containing the server name and MOTD are sent.
		/// All further world joins are ignored, as unfortunately there's no unified "joining world X" between
		/// all the different server software.
		/// </summary>
		public string ServerName { get; private set; }
		
		/// <summary>When the bot first joins a server, a packet containing the server name and MOTD are sent.</summary>
		public string ServerMOTD { get; private set; }
		
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
			if(x > MapSizeX || y > MapSizeY || z > MapSizeZ || x < 0 || y < 0 || z < 0) return false;
			return true;
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
			InitDefaults();
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
			InitDefaults();
		}
		
		/// <summary>Directly connects to a server. Does not include verification, so it will be kicked if server verification is enabled.</summary>
		/// <param name="username"></param>
		/// <param name="ServerIP">An IPAdress which the bot to connect to.</param>
		/// <param name="Port">The int containing the port number for which to connect onto.</param>
		/// <param name="oppath">String that spceifies where to look for a file containg a list of users allowed to use the bot.</param>
		/// <example>ClassicBot c = new ClassicBot("bot",IPAddress.Parse("127.0.0.1",25565,"ops.txt")</example>
		public ClassicBot(string username, IPAddress serverIP, int port, string oppath) :
			this( username, null, serverIP, port, oppath ) { }
		
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
			this.EndPoint = new IPEndPoint( serverIP, port );
			this._Opspath = oppath;
			this._ver = verkey;
			InitDefaults();
		}
		#endregion

		
		#region Internal fields
		
		void InitDefaults() {
			IgnoredUserList = new List<string>();
			Users = new List<string>();
			LoadInternalSettings = true;
			Players = new Dictionary<byte, Player>();
		}
		string _username, _password, _hash;
		string _ver = String.Empty;
		char _delimiter = ':';
		string _Opspath = "operators.txt";
		bool _reconnectonkick = false;
		bool _requiresop = true;
		Server server = null;
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
		
		// This is used to prevent the bot from continuously to try to login to a server. (Eg, after a ban.)
		bool CanReconnectAfterKick = false;
		string DirectURL;
		bool isDirect = false;
		bool isStandard = false;
		byte ProtocolVersion;
		bool _savemap = false;
		bool serverLoaded = false;
		bool UseRemoteServer = false;
		readonly object loggerLocker = new object();
		List<ILogger> loggers = new List<ILogger>();
		#endregion
		
		public bool RegisterLogger( ILogger logger ) {
			if( logger == null ) return false;
			try {
				logger.Initalise();
				loggers.Add( logger );
			} catch( Exception e ) {
				Log( LogType.Error, "Error while adding logger: " + e.ToString() );
				return false;
			}
			return true;
			
		}
		
		internal void Log( LogType type, string message ) {
			lock( loggerLocker ) {
				if( loggers.Count == 0 ) return;
				for( int i = 0; i < loggers.Count; i++ ) {
					loggers[i].Log( type, message );
				}
			}
		}
		
		internal void Log( LogType type, params string[] messages ) {
			lock( loggerLocker ) {
				if( loggers.Count == 0 ) return;
				string message = String.Join( Environment.NewLine, messages );
				for( int i = 0; i < loggers.Count; i++ ) {
					loggers[i].Log( type, message );
				}
			}
		}

		/// <summary>
		/// Sends a message to all remotely connected clients. If the server has not been started,
		/// it will not send a message.
		/// </summary>
		/// <param name="message">The message to send to all remotely connected clients.</param>
		public void MessageAllRemoteClients(string message)
		{
			if (server != null && server.started)
				server.SendMessageToAllRemoteClients(message);
		}
		/// <summary>
		/// Sends a byte array to the currently connected server. (Usually, a packet of some sort.)
		/// </summary>
		/// <param name="data">The daat to be sent, converted into a byte array.</param>
		public void Send(byte[] data)
		{
			if(ServerSocket == null || ServerSocket.Connected == false) return;
			ServerSocket.Send(data, data.Length, SocketFlags.None);
		}
		/// <summary>
		/// Starts the bot. It can optionally run on the same thread as the method that called it.
		/// </summary>
		public void Start(bool RunOnThreadAsCaller)
		{
			if(!Debugger.IsAttached) AppDomain.CurrentDomain.UnhandledException += UnhandledException; //Might interfere with debugging
			if(LoadInternalSettings) LoadConfig();
			if(RunOnThreadAsCaller == true)
			{
				IOLoop();
			}
			Thread thread = new Thread(IOLoop);
			thread.Name = "LibClassicBotIO";
			thread.IsBackground = true;
			thread.Start();
		}

		/// <summary> Raised when an exception is uncaught. </summary>
		void UnhandledException(object sender, UnhandledExceptionEventArgs e) {
			Exception actualException = (Exception)e.ExceptionObject; //Get actual exception.
			Log( LogType.Error, "Unhandled error -", actualException.ToString() );
		}
		
		/// <summary> Loads configuration settings from a file called botsettings.txt </summary>
		private void LoadConfig()
		{
			try {
				Config config = new Config( "botsettings.txt" );
				if( !config.Exists() ) {
					using( StreamWriter sw = new StreamWriter( "botsettings.txt" ) ) {
						sw.WriteLine("UseRemoteServer: false");
						sw.WriteLine("RemotePort: ");
						sw.WriteLine("RemotePassword: ");
						sw.WriteLine("CommandsRequireOperator: true");
						sw.WriteLine("ReconnectAfterKick: true");
						sw.WriteLine("SaveMap: false");
						sw.WriteLine("#And now, a little explaination on what all these mean.");
						sw.WriteLine("#UseRemoteServer - Allows remote clients to connect and perform actions on the bot / chat through it. By default, this is disabled." +
						             "If you choose to use the remote function, you may need to forward the port and/or add an exception to your firewall.");
						sw.WriteLine("#RemotePort - The port the server will listen on for remote clients. It is fine to leave this blank if UseRemoteServer is false.");
						sw.WriteLine("#RemotePassword - The password to use for verifying remote clients. " +
						             "If UseRemoteServer is true and this is blank, the password will be set to \"password\". ");
						sw.WriteLine("#CommandsRequireOperators - This determines whether bot commands require the person who called them to be in the operators file." +
						             "Usually you would want this to be true.");
						sw.WriteLine("#ReconnectAfterKick - This determines if the bot will reconnect after being kicked. Note that if the bot receives a kick packet before" +
						             "a ServerIdentification packet, it will abort, and assume it has been banned from connecting.");
						sw.WriteLine("#SaveMap - This determines if the bot will save the map when the chunk packets are sent to it." +
						             "If this is true, it will be saved as a fCraft compatible map. (Large maps of 512 x 512 x 512 " +
						             "can use up to ~150 megabytes of RAM when saving, so be wary. After saving, memory usage should return to normal.");
					}
				}
				
				config.Load(); // TODO: Output warning.
				if( !config.TryParseValueOrDefault( "useremoteserver", false, out UseRemoteServer ) )
					Log( LogType.Warning, "Couldn't load value for useremoteserver from config. Setting to default value of false" );
				if( UseRemoteServer ) {
					int remotePort;
					if( !config.TryParseValueOrDefault( "remoteport", 25561, out remotePort ) ) {
						Log( LogType.Warning, "Couldn't load value for remoteport from config. Setting to default value of 25561" );
					}
					string remotePassword;
					config.TryGetRawValue( "remotepassword", out remotePassword );
					if( String.IsNullOrEmpty( remotePassword ) ) {
						remotePassword = "password";
						Log( LogType.Warning, "Couldn't load value for remotepassword from config. Setting to default value of \"password\"" );
					}
					
					server = new Server();
					server.Start( this, remotePort, remotePassword );
				}
				
				if( !config.TryParseValueOrDefault( "commandsrequireoperator", true, out _requiresop ) )
					Log( LogType.Warning, "Couldn't load value for commandsrequireoperator from config. Setting to default value of true" );
				if( !config.TryParseValueOrDefault( "reconnectafterkick", true, out _reconnectonkick ) )
					Log( LogType.Warning, "Couldn't load value for reconnectafterkick from config. Setting to default value of true" );
				if( !config.TryParseValueOrDefault( "savemap", false, out _savemap ) )
					Log( LogType.Warning, "Couldn't load value for savemap from config. Setting to default value of false" );
			} catch { 
			}
		}

		/// <summary>Adds a username to the list of allowed users.</summary>
		/// <param name="username">The username to add. Does not contain colour codes.</param>
		/// <param name="savetofile">Whether to save a copy of the allowed users list to the operators file.</param>
		/// <returns>True if added the user.
		/// True if it also succeeded in writing to the file, false if there was an error.</returns>
		public bool AddOperator(string username, bool savetofile)
		{
			Users.Add(username);
			if(savetofile) {
				string[] usersArray = Users.ToArray();
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
			if (!Users.Remove(username)) return false;
			
			if(savetofile)
			{
				string[] usersArray = Users.ToArray();
				try { File.WriteAllLines(_Opspath,usersArray); }
				catch { return false; }
			}
			return true;
		}

		#region I/O Loop
		void IOLoop()
		{
			if (File.Exists(_Opspath)) {
				string[] lines = File.ReadAllLines(_Opspath);
				for (int i = 0; i < lines.Length; i++) {
					Users.Add(lines[i]);
				}
			} else {
				File.Create(_Opspath);
			}
			
			if (isDirect == true) {
				string[] parts = DirectURL.Substring(5).Split('/');
				_username = parts[1];
				_ver = parts[2];

				string[] ipPort = parts[0].Split(':');
				IPAddress ip = IPAddress.Parse(ipPort[0]);
				int port = Convert.ToInt32(ipPort[1]);
				EndPoint = new IPEndPoint( ip, port );
			} else if(isStandard == true) {
				try {
					IPEndPoint point;
					Log( LogType.BotActivity, "Trying to log into minecraft.net.." );
					Extensions.Login( _username, _password, _hash, out point, out _ver, out migratedUsername );
					Log( LogType.BotActivity, "Successfully logged in." );
					EndPoint = point;
				} catch( InvalidOperationException ex ) {
					Log( LogType.Error, ex.Message, ex.ToString() );
					return;
				} catch( ArgumentOutOfRangeException ex ) {
					Log( LogType.Error, ErrorInPage, ex.ToString() );
					return;
				}
			}

			IgnoredUserList.Add(migratedUsername ?? _username); //Ignore self.
			byte[] ToSendLogin = CreateLoginPacket(migratedUsername ?? _username, _ver);
			ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			try {
				if(EndPoint.Port > IPEndPoint.MaxPort || EndPoint.Port < IPEndPoint.MinPort) throw new SocketException(-1);
				//Do not continue if the user attempts to connect on a port larger than possible.
				ServerSocket.Connect(EndPoint);
				ServerSocket.Send(ToSendLogin);
			} catch( SocketException ex ) {
				Log( LogType.Error, HandleSocketError( ex.ErrorCode ), ex.ToString() );
				return;
			}
			
			StartCommandsThread();
			Plugins.PluginManager.LoadPlugins(RegisteredCommands, this);
			BinaryReader reader = new BinaryReader(new NetworkStream(ServerSocket));

			while (true)
			{
				try
				{
					byte OpCode = reader.ReadByte();
					PacketEventArgs opcodeEvent = new PacketEventArgs((ServerPackets)OpCode);
					Events.RaisePacketReceived(opcodeEvent);
					
					switch ((ServerPackets)OpCode)
					{
						case ServerPackets.ServerIdentification://0x00
							{
								ProtocolVersion = reader.ReadByte(); // Protocol version should be seven.
								string Name = Encoding.ASCII.GetString(reader.ReadBytes(64)).Trim();
								string MOTD = Encoding.ASCII.GetString(reader.ReadBytes(64)).Trim();
								if(!serverLoaded)
								{
									ServerName = Name;
									ServerMOTD = MOTD;
									serverLoaded = true;
								}
								UserType = reader.ReadByte();//Get the type. 0x64 = Op, 0x00 = Normal user.
							}
							break;

						case ServerPackets.Ping://0x01
							break; // Server only requires we read the packet ID.

						case ServerPackets.LevelInitialize://0x02
							Players.Clear();
							if(_savemap) mapStream = new MemoryStream();
							CanReconnectAfterKick = true;
							break;


						case ServerPackets.LevelDataChunk://0x03
							{
								int chunkLength = IPAddress.HostToNetworkOrder(reader.ReadInt16()); // Length of non padded data.
								byte[] chunkData = reader.ReadBytes(1024); // May be padded with extra data.
								byte progress = reader.ReadByte();
								if(_savemap) {
									mapStream.Write( chunkData, 0, chunkLength );
								}
								MapProgressEventArgs e = new MapProgressEventArgs(progress);
								Events.RaiseMapProgress(e);
							}
							break;

						case ServerPackets.LevelFinalize://0x04:
							{
								MapSizeX = IPAddress.HostToNetworkOrder(reader.ReadInt16());
								MapSizeZ = IPAddress.HostToNetworkOrder(reader.ReadInt16());
								MapSizeY = IPAddress.HostToNetworkOrder(reader.ReadInt16());
								Connected = true; //At this state, we've loaded the map and we're ready to send chat etc.
								if(_savemap) {
									mapStream.Seek(0, SeekOrigin.Begin);
									Map map = new Map(MapSizeX, MapSizeY, MapSizeZ);
									using (GZipStream decompressed = new GZipStream(mapStream,CompressionMode.Decompress))
									{
										decompressed.Read(new byte[4],0,4); //Ignore size of stream.
										int sizeX = MapSizeX, sizeY = MapSizeY, sizeZ = MapSizeZ;
										for (int z = 0; z < sizeZ; z++) {
											for (int y = 0; y < sizeY; y++) {
												for (int x = 0; x < sizeX; x++) {
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
								BlockPlacedEventArgs e = new BlockPlacedEventArgs(blockX, blockY, blockZ, blockType);
								Events.RaiseBlockPlaced(e);
								if(marksLeft > 0 && blockType == 39 && CubID != null)
								{
									byte id = CubID.Value;
									if(Players[id].X < 0 || Players[id].Y < 0 || Players[id].Z < 0) {
										SendMessagePacket("Error: You are too far away from the bot.");
										break;
									}
									
									if(new Vector3I(blockX, blockY, blockZ).Distance(new Vector3I(
										(int)Players[id].X, (int)Players[id].Y, (int)Players[id].Z)) > 11) {
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
								Players[pID] = new Player();
								Players[pID].Name = playername;
								Players[pID].X = IPAddress.HostToNetworkOrder(reader.ReadInt16()) / 32f;
								Players[pID].Z = IPAddress.HostToNetworkOrder(reader.ReadInt16()) / 32f;
								Players[pID].Y = IPAddress.HostToNetworkOrder(reader.ReadInt16()) / 32f;
								Players[pID].Yaw = reader.ReadByte();
								Players[pID].Pitch = reader.ReadByte();
								PositionEventArgs e = new PositionEventArgs(pID, Players[pID]);
								Events.RaisePlayerMoved(e);
							}
							break;

						case ServerPackets.PlayerTeleport://0x08
							{
								byte pID = reader.ReadByte();
								Players[pID].X = IPAddress.HostToNetworkOrder(reader.ReadInt16()) / 32f;
								Players[pID].Z = IPAddress.HostToNetworkOrder(reader.ReadInt16()) / 32f;
								Players[pID].Y = IPAddress.HostToNetworkOrder(reader.ReadInt16()) / 32f;
								Players[pID].Yaw = reader.ReadByte();
								Players[pID].Pitch = reader.ReadByte();
								PositionEventArgs e = new PositionEventArgs(pID, Players[pID]);
								Events.RaisePlayerMoved(e);
							}
							break;

						case ServerPackets.PositionandOrientationUpdate://0x09
							{
								byte pID = reader.ReadByte();
								Players[pID].X += reader.ReadSByte() / 32f;
								Players[pID].Z += reader.ReadSByte() / 32f;
								Players[pID].Y += reader.ReadSByte() / 32f;
								Players[pID].Yaw = reader.ReadByte();
								Players[pID].Pitch = reader.ReadByte();
								PositionEventArgs e = new PositionEventArgs(pID, Players[pID]);
								Events.RaisePlayerMoved(e);
							}
							break;

						case ServerPackets.PositionUpdate://0x0a
							{
								byte pID = reader.ReadByte();
								Players[pID].X += (reader.ReadSByte() / 32f);
								Players[pID].Z += (reader.ReadSByte() / 32f);
								Players[pID].Y += (reader.ReadSByte() / 32f);
								PositionEventArgs e = new PositionEventArgs(pID, Players[pID]);
								Events.RaisePlayerMoved(e);
							}
							break;

						case ServerPackets.OrientationUpdate://0x0b
							{
								byte pID = reader.ReadByte();
								Players[pID].Yaw = reader.ReadByte();
								Players[pID].Pitch = reader.ReadByte();
								PositionEventArgs e = new PositionEventArgs(pID, Players[pID]);
								Events.RaisePlayerMoved(e);
							}
							break;

						case ServerPackets.DespawnPlayer://0x0c
							{
								byte playerID = reader.ReadByte();
								Players.Remove(playerID); // Also sent when the player joins another map.
							}
							break;

						case ServerPackets.Message://0x0d
							{
								reader.ReadByte(); // Most servers don't send the ID.
								string Line = Encoding.ASCII.GetString(reader.ReadBytes(64)).Trim();
								MessageEventArgs e = new MessageEventArgs(Line); //Raise the event, let the event handler take care of splitting.
								Events.RaiseChatMessage(e);
								Log( LogType.Chat, Line );
								
								if (Line.Contains(_delimiter.ToString())) { //Most servers use : as the delimiter.
									string[] lineSplit = Line.Split(new char[] { _delimiter }, 2);
									string Message = Extensions.StripColors(lineSplit[1]).TrimStart(' ');
									string User = Extensions.StripColors(lineSplit[0]);

									if (!IgnoredUserList.Contains(User))
									{
										string strippedcommandheader = Message.Split(' ')[0].ToLower();
										foreach(KeyValuePair<string,CommandDelegate> command in RegisteredCommands)
										{
											if(strippedcommandheader == "."+command.Key.ToLower())
											{
												if(_requiresop == true)
												{
													if(Users.Contains(User)) {
														EnqueueCommand(command.Value,Line);
														break;
													} else {
														SendMessagePacket(User +", you are not allowed to use the bot.");
													}
												} else {
													EnqueueCommand(command.Value,Line);
													break;
												}
											}
										}
									}
								}
								ProcessCommandQueue();
							}
							break;

						case ServerPackets.DisconnectSelf://0x0e
							{
								string reason = Encoding.ASCII.GetString(reader.ReadBytes(64)).Trim();
								KickedEventArgs e = new KickedEventArgs(reason, CanReconnectAfterKick);
								Events.RaiseGotKicked(e);
								if( _reconnectonkick == true && CanReconnectAfterKick == true ) {
									Log( LogType.BotActivity, "Kicked from the server. Reconnecting.", reason );
									CanReconnectAfterKick = false;
									Thread thread = new Thread(IOLoop);
									thread.Name = "LibClassicBotIO";
									thread.IsBackground = true;
									thread.Start();
								} else if(_reconnectonkick == false) {
									Log( LogType.Warning, "Kicked from the server. Not attempting to reconnect.", reason );
								} else {
									ServerSocket.Close();
									Log( LogType.Error, "The bot was prevented from reconnecting.", "(Kick packet received before a LevelBegin packet.)" );
								}
								return;
							}

						case ServerPackets.SetPermission://0x0f
							UserType = reader.ReadByte();//Issued in Vanilla when someone calls /op, used in fCraft for bedrock permission checking.
							break;

						default:
							throw new IOException("Unrecognised packet. Opcode: " + OpCode.ToString());
					}
				} catch ( IOException ex ) {
					if( _reconnectonkick == true && CanReconnectAfterKick == true ) {
						CancelDrawer();
						CanReconnectAfterKick = false;
						Thread thread = new Thread(IOLoop);
						thread.Name = "LibClassicBotIO";
						thread.IsBackground = true;
						thread.Start();
					} else {
						Log( LogType.Error, "An I/O error has occured. The connection have been closed uncleanly. Exiting the bot.", ex.ToString() );
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
		/// <summary> The X position of the connected player. </summary>
		public float X;
		
		/// <summary> The Y position of the connected player. Classic sends Y as height,
		/// but the bot uses Z as height instead. </summary>
		public float Y;
		
		/// <summary>The Z position of the connected player. </summary>
		public float Z;
		
		/// <summary> The name of the connected player. </summary>
		public string Name;
		
		/// <summary> The yaw of the player. </summary>
		public byte Yaw;
		
		/// <summary> The pitch of the player.</summary>
		public byte Pitch;
	}
}