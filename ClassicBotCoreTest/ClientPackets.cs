using System;

namespace LibClassicBot.Networking
{
	/// <summary>Enumeration containing a list of packets sent by a Minecraft Classic client.</summary>
	/// <remarks>A typical session goes as the following:<br/>
	/// C > S 0x00<br/>
	/// S > C 0x00<br/>
	/// S > C 0x02<br/>
	/// S > C 0x03 (Chunks)<br/>
	/// S > C 0x04<br/>
	/// Block updates, chat messages, position updates..
	/// </remarks>
	public enum ClientPackets : byte
	{
		/// <summary>Packet first sent when the client is attempting to authenticate.</summary>
		PlayerIdentification = 0,
		/// <summary>Packet sent when the client is attempting to place a block. The client assumes this always succeeds.</summary>
		SetBlock = 0x05,
		/// <summary>Packet sent when the client is attempting to update its position.</summary>
		PositionUpdate = 0x08,
		/// <summary>Packet sent when the client sends a message. Should be echoed back by the server, but possibly with formatting.</summary>
		Message = 0x0d,
	}
}