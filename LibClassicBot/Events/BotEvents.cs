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
}

