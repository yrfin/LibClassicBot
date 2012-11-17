using System;

namespace LibClassicBot.Remote
{
	/// <summary>
	/// Description of BotRemote.
	/// </summary>
	public enum RemoteProtocol: byte
	{
		/// <summary>Sent when the user first connects. C > S</summary>
		Username = 0,
		/// <summary>Sent when the user is attempting to verify their password. C > S</summary>
		Password = 1,
		/// <summary>Sent when the user is wanting to send a message to the remote bot > To the minecraft server. C > S</summary>
		IncomingMessage = 2,
		/// <summary>Sent when the bot the bot sends a message to the client. (Not a Minecraft one) The next byte contains the type of message,
		/// 0 indicates normal, 1 indicates a warning, 2 indicates an error and that the session should be closed,
		/// 3 indicates password request.</summary>
		ServerMessage = 3,
		/// <summary>Sent to the client when a message is sent to the bot from the Minecraft server. S > C</summary>
		OutgoingMessage = 6,
		/// <summary>Sent to the client once the client has been authorised, and that the server can start receiving chat.</summary>
		ClientAuthorised = 7,
		/// <summary>Sent to the client once the server has acknowledged a server command from the client..</summary>
		ServerResponse = 8		
	}
}