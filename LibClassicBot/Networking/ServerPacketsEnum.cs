namespace LibClassicBot.Networking
{
	/// <summary>Enumeration containing a list of packets sent by a Minecraft Classic server.</summary>
	/// <remarks>A typical session goes as the following:<br/>
	/// C > S 0x00<br/>
	/// S > C 0x00<br/>
	/// S > C 0x02<br/>
	/// S > C 0x03 (Chunks)<br/>
	/// S > C 0x04<br/>
	/// Block updates, chat messages, position updates..
	/// </remarks>
	/// <summary>Enumeration containing a list of packets sent by a classic server.</summary>
	public enum ServerPackets : byte
	{
		/// <summary>Packet first sent when the server respond to the initial client authentification packet.</summary>
		ServerIdentification = 0,
		/// <summary>Ping packet sent by the server, client ignores this.</summary>
		Ping = 0x01,
		/// <summary>Packet sent when a player is beginning to load the map.</summary>
		LevelInitialize = 0x02,
		/// <summary>Packet containing a chunk of the map.</summary>
		LevelDataChunk = 0x03,
		/// <summary>Packet sent when the map has finished being loaded.</summary>
		LevelFinalize = 0x04,
		/// <summary>Packet sent when any player places or deletes a block.</summary>
		SetBlock = 0x06,
		/// <summary>Packet sent when a player is spawned. Note this is only for the current world.</summary>
		SpawnPlayer = 0x07,
		/// <summary>Packet sent when a player teleports.</summary>
		PlayerTeleport = 0x08,
		/// <summary>Packet sent when a player moves and rotates. Contains: X,Y,Z, yaw and pitch.</summary>
		PositionandOrientationUpdate = 0x09,
		/// <summary>Packet sent when a player moves. Contains: X,Y and Z.</summary>
		PositionUpdate = 0x0a,
		/// <summary>Packet sent when a player rotates. Contains: Yaw and pitch.</summary>
		OrientationUpdate = 0x0b,
		/// <summary>Packet sent when a player despawns. (Includes joining another map.)</summary>
		DespawnPlayer = 0x0c,
		/// <summary>Packet sent when a player receives a message.</summary>
		Message = 0x0d,
		/// <summary>Packet sent when the server kicks the player.</summary>
		DisconnectSelf = 0x0e,
		/// <summary>Packet sent to update the user type. (For placing bedrock)</summary>
		SetPermission = 0x0f
	}
}