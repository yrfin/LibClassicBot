using System;
using System.Net;
using System.Net.Sockets;
using LibClassicBot.Drawing;

namespace LibClassicBot.Events
{
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
	
	public sealed class BlockPlacedEventArgs : EventArgs
	{
		/// <summary>The position of the block, in normal coordinates. (Z is height.)</summary>
		public Vector3I Position;
		
		/// <summary>The type of block placed, as a byte.</summary>
		public byte BlockType;
		
		/// <summary>
		/// A BlockPlacedEventArg containg the position of the block and the type.
		/// </summary>
		/// <param name="x">The X coordinate of the block.</param>
		/// <param name="y">The Y coordinate of the block. (Not height)</param>
		/// <param name="z">The Z coordinate of the block.</param>
		/// <param name="type">The type of the block placed, as a byte.</param>
		internal BlockPlacedEventArgs(int x, int y, int z, byte type)
		{
			Position = new Vector3I(x, y, z);
			BlockType = type;
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