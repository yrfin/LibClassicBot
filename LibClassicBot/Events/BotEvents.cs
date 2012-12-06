using System;
using System.Net;
using System.Net.Sockets;

namespace LibClassicBot.Events
{
	public sealed class BotEvents
	{
		/// <summary>Occurs when the bot receives a message.</summary>
		public event EventHandler<MessageEventArgs> ChatMessage;

		/// <summary>Occurs when a player has moved. Includes orientation as well.</summary>
		public event EventHandler<PositionEventArgs> PlayerMoved;
		
		/// <summary>Occurs when a packet is received. Contains the opcode, but does NOT contain any of the packet data</summary>
		public event EventHandler<PacketEventArgs> PacketReceived;
		
		/// <summary>Occurs when the bot is kicked. To automatically make it reconnect, use ReconnectAfterKick = true;</summary>
		public event EventHandler<KickedEventArgs> GotKicked;
		
		/// <summary>Occurs when a socket exception is raised during connecting to a server.</summary>
		public event EventHandler<BotExceptionEventArgs> BotException;
		
		/// <summary>Occurs when the bot has received a map chunk, and indicates the progress of loading the map.</summary>
		public event EventHandler<MapProgressEventArgs> MapProgress;

		/// <summary>Occurs when the bot has finished loading a map.</summary>
		public event EventHandler<MapLoadedEventArgs> MapLoaded;
		
		/// <summary>Occurs when a remote session is started.</summary>
		public event EventHandler<SessionStartedEventArgs> RemoteSessionStarted;

		/// <summary>Occurs when a remote user logs in and is verified. </summary>
		public event EventHandler<RemoteLoginEventArgs> RemoteUserLoggedIn;
		
		/// <summary>Occurs when a remote session is ended.;</summary>
		public event EventHandler<SessionEndedEventArgs> RemoteSessionEnded;		

		/// <summary>Raises a new ChatMessage Event.</summary>
		internal void RaiseChatMessage(MessageEventArgs e)
		{
			System.EventHandler<MessageEventArgs> chatEvent = ChatMessage;
			if (chatEvent == null) return;
			chatEvent(null, e);
		}

		/// <summary>Raises a new PlayerMoved Event.</summary>
		internal void RaisePlayerMoved(PositionEventArgs e)
		{
			System.EventHandler<PositionEventArgs> movedEvent = PlayerMoved;
			if (movedEvent == null) return;
			movedEvent(null, e);
		}
		
		/// <summary>Raises a new PacketReceived Event.</summary>
		internal void RaisePacketReceived(PacketEventArgs e)
		{
			System.EventHandler<PacketEventArgs> packetEvent = PacketReceived;
			if (packetEvent == null) return;
			packetEvent(null, e);
		}

		/// <summary>Raises a new GotKicked Event.</summary>
		internal void RaiseGotKicked(KickedEventArgs e)
		{
			System.EventHandler<KickedEventArgs> kickedEvent = GotKicked;
			if(kickedEvent == null) return;
			kickedEvent(null,e);
		}
		
		/// <summary>Raises a new BotSocketError Event.</summary>
		internal void RaiseBotError(BotExceptionEventArgs e)
		{
			System.EventHandler<BotExceptionEventArgs> socketEvent = BotException;
			if(socketEvent == null) return;
			socketEvent(null,e);
		}
		
		/// <summary>Raises a new MapProgress Event.</summary>
		internal void RaiseMapProgress(MapProgressEventArgs e)
		{
			System.EventHandler<MapProgressEventArgs> progressEvent = MapProgress;
			if(progressEvent == null) return;
			progressEvent(null,e);
		}		

		/// <summary>Raises a new MapProgress Event.</summary>
		internal void RaiseMapLoaded(MapLoadedEventArgs e)
		{
			System.EventHandler<MapLoadedEventArgs> loadEvent = MapLoaded;
			if(loadEvent == null) return;
			loadEvent(null,e);
		}	

		/// <summary>Raises a new RemoteSessionStarted Event.</summary>
		internal void RaiseSessionStarted(SessionStartedEventArgs e)
		{
			System.EventHandler<SessionStartedEventArgs> sesStartEvent = RemoteSessionStarted;
			if (sesStartEvent == null) return;
			sesStartEvent(null, e);
		}

		/// <summary>Raises a new RemoteUserLoggedin Event.</summary>
		internal void RaiseUserLoggedIn(RemoteLoginEventArgs e)
		{
			System.EventHandler<RemoteLoginEventArgs> loggedinEvent = RemoteUserLoggedIn;
			if (loggedinEvent == null) return;
			loggedinEvent(null, e);
		}
		
		/// <summary>Raises a new RemoteSessionEnded Event.</summary>
		internal void RaiseSessionEnded(SessionEndedEventArgs e)
		{
			System.EventHandler<SessionEndedEventArgs> sesEndEvent = RemoteSessionEnded;
			if (sesEndEvent == null) return;
			sesEndEvent(null, e);
		}		
	}

	public sealed class MessageEventArgs : EventArgs
	{
		/// <summary>Full line, containg user, message, and all colour codes.</summary>
		public string Line;

		/// <summary>
		/// A MessageEventArg containing the full message, including color codes. Use NetEx.StripColors(line)
		/// if you are wanting a version of the message without colour codes.
		/// </summary>
		/// <param name="line">Full message with colour codes.</param>
		internal MessageEventArgs(string line)
		{
			Line = line;
		}
	}

	public sealed class MapProgressEventArgs : EventArgs
	{
		/// <summary>The progress of loading the map so far.</summary>
		public byte PercentDone;

		/// <summary>
		/// A MapProgressEventArg containing the percent of the map loaded.
		/// </summary>
		/// <param name="percent">The percent in how far the map has loaded.</param>
		internal MapProgressEventArgs(byte percent)
		{
			PercentDone = percent;
		}
	}	
	
	public sealed class MapLoadedEventArgs : EventArgs
	{
		/// <summary>
		/// A MapLoadedEventArg, which in itself contains no properties.
		/// </summary>
		internal MapLoadedEventArgs() {}
	}
	
	public sealed class PositionEventArgs : EventArgs
	{
		/// <summary>ID of the player who moved.</summary>
		public byte playerID;
		/// <summary> Player who moved. Name does include colour codes. </summary>
		public Player player;


		/// <summary> PositionEventArg containing the id, and a name with colour codes.
		/// It also contains X,Y,Z,Yaw and Pitch.</summary>
		/// <param name="id">The Player that moved.</param>
		internal PositionEventArgs(byte id, Player ply)
		{
			playerID = id;
			player = ply;
		}
	}
	
	public sealed class PacketEventArgs : EventArgs
	{
		/// <summary>The PacketID that was received.</summary>
		public Networking.ServerPackets OpCode;
	
		//public string PacketType; 
		//PacketType = Enum.GetName(typeof(LibClassicBot.Networking.ServerPackets), opcode);

		/// <summary>
		/// PacketEventArg containg the opcode / PacketID of a packet. To get the actual byte, use (byte)e.OpCode.
		/// </summary>
		/// <param name="opcode">The ServerPackets representation of the packet ID.</param>
		internal PacketEventArgs(Networking.ServerPackets opcode)
		{
			OpCode = opcode;
		}
	}
	
	public sealed class KickedEventArgs : EventArgs
	{
		/// <summary>The reason the bot was kicked. May or may not colours, depending on the server.</summary>
		public string Reason;
		
		/// <summary>Whether or not the bot will attempt to reconnect to the server. </summary>
		public bool WillReconnect;

		/// <summary>
		/// KickedEventArg containg the reason for the kick, and a bool which determines if the bot will attempt to reconnect.
		/// </summary>
		/// <param name="reason">The reason the bot was kicked, as a string.</param>
		/// <param name="willReconnect">Whether or not the bot will reconnect, as a bool.</param>
		internal KickedEventArgs(string reason, bool willReconnect)
		{
			Reason = reason;
			WillReconnect = willReconnect;
		}
	}
	
	public sealed class BotExceptionEventArgs : EventArgs
	{
		/// <summary>Formatted output message, designed to be easy for the user to read. (For example, when the target host is unreachable)</summary>
		public string Output;
		
		/// <summary>The actual exception that occurred.</summary>
		public Exception ActualException;

		/// <summary>
		/// SocketExceptionEventArg containg a formatted output, and a socket exception of the actual error.
		/// </summary>
		/// <param name="output">A formatted output of the erorr, designed for the user to be able to understand.</param>
		/// <param name="exception"></param>
		internal BotExceptionEventArgs(string output, Exception exception)
		{
			Output = output;
			ActualException = exception;
		}
	}

	public sealed class SessionStartedEventArgs : EventArgs
	{
		/// <summary>The IPEndPoint of the remote user attempting to login. At this point, we do not know the username.</summary>
		public IPEndPoint RemoteEndPoint;
		
		/// <summary>The remote TCP client of the remote endpoint.</summary>
		public TcpClient RemoteTCPClient;

		/// <summary>A SessionStartedEventArg. It contains an IPEndPoint which identified the remote endpoint the user is connecting from,
		/// and a TcpClient that can be used for communication with the remote endpoint.</summary>
		/// <param name="endIP">The IPEndPoint of the remote endpoint.</param>
		/// <param name="endTCP">The TcpClient of the remote endpoint.</param>
		internal SessionStartedEventArgs(IPEndPoint endIP, TcpClient endTCP)
		{
			RemoteEndPoint = endIP;
			RemoteTCPClient = endTCP;
		}
	}


	public sealed class RemoteLoginEventArgs : EventArgs
	{
		/// <summary>The username of the remote user.</summary>
		public string Username;
		
		/// <summary>The IPEndPoint of the user attempting to login.</summary>
		public IPEndPoint RemoteEndPoint;
		
		/// <summary>The remote TCP client of the user.</summary>
		public TcpClient RemoteTCPClient;

		/// <summary>A SessionStartedEventArg. It contains an IPEndPoint which identified the remote endpoint the user is connecting from,
		/// and a TcpClient that can be used for communication with the remote endpoint.</summary>
		/// <param name="endIP">The IPEndPoint of the remote endpoint.</param>
		/// <param name="endTCP">The TcpClient of the remote endpoint.</param>
		/// <param name="user">The name of the user that logged in. Is not verified yet.</param>
		internal RemoteLoginEventArgs(string user, IPEndPoint endIP, TcpClient endTCP)
		{
			RemoteEndPoint = endIP;
			RemoteTCPClient = endTCP;
			Username = user;
		}
	}
	
	public sealed class SessionEndedEventArgs : EventArgs
	{
		/// <summary>The username of the remote user that disconnected. May be null if the remote client disconnected before sending the username/</summary>
		public string Username;

		/// <summary>A SessionEndedEventArg which identified which user was removed from the server.
		/// May be null if the connection was lost before a username was given to the server.</summary>
		/// <param name="user">The name of the user that disconnected.</param>
		internal SessionEndedEventArgs(string user)
		{
			Username = user;
		}
	}
}