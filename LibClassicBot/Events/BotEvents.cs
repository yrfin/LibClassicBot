using System;
using System.Collections.Generic;
using System.Text;

namespace LibClassicBot.Events
{
	public class BotEvents
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


		/// <summary>Raises a new ChatMessage Event.</summary>
		/// <param name="e">MessageEventArgs to send</param>
		internal void RaiseChatMessage(MessageEventArgs e)
		{
			System.EventHandler<MessageEventArgs> chatEvent = ChatMessage;
			if (chatEvent == null) return;
			chatEvent(null, e);
		}

		/// <summary>Raises a new PlayerMoved Event.</summary>
		/// <param name="e">PositionEventArgs to send</param>
		internal void RaisePlayerMoved(PositionEventArgs e)
		{
			System.EventHandler<PositionEventArgs> movedEvent = PlayerMoved;
			if (movedEvent == null) return;
			movedEvent(null, e);
		}
		
		/// <summary>Raises a new PacketReceived Event.</summary>
		/// <param name="e">PacketEventArgs to send</param>
		internal void RaisePacketReceived(PacketEventArgs e)
		{
			System.EventHandler<PacketEventArgs> packetEvent = PacketReceived;
			if (packetEvent == null) return;
			packetEvent(null, e);
		}

		/// <summary>Raises a new GotKicked Event.</summary>
		/// <param name="e">KickedEventArgs to send</param>
		internal void RaiseGotKicked(KickedEventArgs e)
		{
			System.EventHandler<KickedEventArgs> kickedEvent = GotKicked;
			if(kickedEvent == null) return;
			kickedEvent(null,e);
		}
		
		/// <summary>Raises a new BotSocketError Event.</summary>
		/// <param name="e">SocketExceptionEventArgs to send</param>
		internal void RaiseBotError(BotExceptionEventArgs e)
		{
			System.EventHandler<BotExceptionEventArgs> socketEvent = BotException;
			if(socketEvent == null) return;
			socketEvent(null,e);
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

	public sealed class PositionEventArgs : EventArgs
	{
		/// <summary>ID of the player who moved.</summary>
		public byte playerID;
		/// <summary> Name of the player who moved. Does include colour codes. </summary>
		public string Name;
		/// <summary>X coordinate of the player.</summary>
		public int X;
		/// <summary>Y coordinate of the player.</summary>
		public int Y;
		/// <summary>Z coordinate of the player.</summary>
		public int Z;
		/// <summary>Yaw of the player.</summary>
		public byte Yaw;
		/// <summary>Pitch of the player.</summary>
		public byte Pitch;


		/// <summary> PositionEventArg containing the id, and a name with colour codes.
		/// It also contains X,Y,Z,Yaw and Pitch.</summary>
		/// <param name="id">The ID of the player that moved.</param>
		/// <param name="name">The name of the player that moved.</param>
		/// <param name="x">X coordinate of the player.</param>
		/// <param name="y">Y coordinate of the player.</param>
		/// <param name="z">Z coordinate of the player.</param>
		/// <param name="yaw">Yaw of the player.</param>
		/// <param name="pitch">Pitch of the player.</param>
		internal PositionEventArgs(byte id, string name, int x, int y, int z, byte yaw, byte pitch)
		{
			playerID = id;
			Name = name;
			X = x;
			Y = y;
			Z = z;
			Yaw = yaw;
			Pitch = pitch;
		}
	}
	
	public sealed class PacketEventArgs : EventArgs
	{
		/// <summary>The PacketID</summary>
		public byte OpCode;
		
		/// <summary>The type of packet that was received, using the enumeration ClassicBot.ClassicPackets</summary>
		public string PacketType;

		/// <summary>
		/// PacketEventArg containg the opcode / PacketID of a packet.
		/// </summary>
		/// <param name="opcode">The byte representation of the packet ID.</param>
		internal PacketEventArgs(byte opcode)
		{
			OpCode = opcode;
			PacketType = Enum.GetName(typeof(LibClassicBot.Networking.ServerPackets), opcode);
			
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


}