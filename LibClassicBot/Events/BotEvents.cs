using System;
using System.Collections.Generic;
using System.Text;

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
		
		/// <summary>Raises a new MapProgress Event.</summary>
		/// <param name="e">MapProgressEventArgs to send</param>
		internal void RaiseMapProgress(MapProgressEventArgs e)
		{
			System.EventHandler<MapProgressEventArgs> progressEvent = MapProgress;
			if(progressEvent == null) return;
			progressEvent(null,e);
		}		

		/// <summary>Raises a new MapProgress Event.</summary>
		/// <param name="e">MapProgressEventArgs to send</param>
		internal void RaiseMapLoaded(MapLoadedEventArgs e)
		{
			System.EventHandler<MapLoadedEventArgs> loadEvent = MapLoaded;
			if(loadEvent == null) return;
			loadEvent(null,e);
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


}