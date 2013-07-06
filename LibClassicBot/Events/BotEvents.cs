using System;

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
		
		/// <summary>Occurs when the bot has received a map chunk, and indicates the progress of loading the map.</summary>
		public event EventHandler<MapProgressEventArgs> MapProgress;

		/// <summary>Occurs when the bot has finished loading a map.</summary>
		public event EventHandler<MapLoadedEventArgs> MapLoaded;
		
		/// <summary>Occurs when a remote session is started.</summary>
		public event EventHandler<SessionStartedEventArgs> RemoteSessionStarted;

		/// <summary>Occurs when a remote user logs in and is verified. </summary>
		public event EventHandler<RemoteLoginEventArgs> RemoteUserLoggedIn;
		
		/// <summary>Occurs when a remote session is ended.</summary>
		public event EventHandler<SessionEndedEventArgs> RemoteSessionEnded;
		
		/// <summary>Occurs when a block is placed.</summary>
		public event EventHandler<BlockPlacedEventArgs> BlockPlaced;
		
		/// <summary> Occurs when the configuration file of a ClassicBot instance is being loaded. </summary>
		/// <remarks> The event is rasied *after* the bot loads its internal settings. </remarks>
		public event EventHandler<ConfigLoadingEventArgs> ConfigLoading;
		
		/// <summary> Occurs when the configuration file of a ClassicBot instance is being loaded. </summary>
		/// <remarks> The event is raised *after* the bot writes the default internal settings.
		/// You can write more config keys (and default values) to the configuration file through this event. </remarks>
		public event EventHandler<ConfigCreatingEventArgs> ConfigCreating;

		/// <summary>Raises a new ChatMessage Event.</summary>
		internal void RaiseChatMessage(MessageEventArgs e) {
			if (ChatMessage != null) ChatMessage(null, e);
		}

		/// <summary>Raises a new PlayerMoved Event.</summary>
		internal void RaisePlayerMoved(PositionEventArgs e) {
			if (PlayerMoved != null)PlayerMoved(null, e);
		}
		
		/// <summary>Raises a new PacketReceived Event.</summary>
		internal void RaisePacketReceived(PacketEventArgs e) {
			if (PacketReceived != null) PacketReceived(null, e);
		}

		/// <summary>Raises a new GotKicked Event.</summary>
		internal void RaiseGotKicked(KickedEventArgs e) {
			if(GotKicked != null) GotKicked(null, e);
		}
		
		/// <summary>Raises a new MapProgress Event.</summary>
		internal void RaiseMapProgress(MapProgressEventArgs e) {
			if(MapProgress != null) MapProgress(null, e);
		}

		/// <summary>Raises a new MapProgress Event.</summary>
		internal void RaiseMapLoaded(MapLoadedEventArgs e) {
			if(MapLoaded != null) MapLoaded(null, e);
		}

		/// <summary>Raises a new RemoteSessionStarted Event.</summary>
		internal void RaiseSessionStarted(SessionStartedEventArgs e) {
			if(RemoteSessionStarted != null) RemoteSessionStarted(null, e);
		}

		/// <summary>Raises a new RemoteUserLoggedin Event.</summary>
		internal void RaiseUserLoggedIn(RemoteLoginEventArgs e) {
			if(RemoteUserLoggedIn != null) RemoteUserLoggedIn(null, e);
		}
		
		/// <summary>Raises a new RemoteSessionEnded Event.</summary>
		internal void RaiseSessionEnded(SessionEndedEventArgs e) {
			if(RemoteSessionEnded != null) RemoteSessionEnded(null, e);
		}

		/// <summary>Raises a new BlockPlaced Event.</summary>
		internal void RaiseBlockPlaced(BlockPlacedEventArgs e) {
			if(BlockPlaced != null) BlockPlaced(null, e);
		}
		
		/// <summary> Raises a new ConfigLoading event. </summary>
		internal void RaiseConfigLoading( ConfigLoadingEventArgs e ) {
			if( ConfigLoading != null ) {
				ConfigLoading( null, e );
			}
		}
		
		/// <summary> Raises a new ConfigCreating event. </summary>
		internal void RaiseConfigCreating( ConfigCreatingEventArgs e ) {
			if( ConfigCreating != null ) {
				ConfigCreating( null, e );
			}
		}	
	}
}