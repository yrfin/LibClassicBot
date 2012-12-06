using System;
using System.Net;
using System.Text;
using LibClassicBot.Events;
using LibClassicBot.Networking;

namespace LibClassicBot
{
	public partial class ClassicBot
	{
		/// <summary>
		/// Gets the actual message from a raw chatline, and also removes color codes.
		/// </summary>
		/// <param name="rawLine">The raw chat line to trim. Can include color codes.</param>
		/// <returns>A new string after the delimeter, with starting and endspaces removed,
		/// as well as color codes.</returns>
		public string GetMessage(string rawLine)
		{
			return Extensions.StripColors(rawLine).Split(this._delimiter)[1].Trim(' ');
		}
		
		//Error messages.
		const string packetError = "Error while trying to send a packet.";
		/// <summary>
		/// Gets the user from a raw chatline, and also removes color codes. Albeit, this may be prefixed.
		/// </summary>
		/// <param name="rawLine">The raw chat line to trim. Can include color codes.</param>
		/// <returns>A new string before the delimeter, with starting and endspaces removed,
		/// as well as color codes.</returns>
		public string GetUser(string rawLine)
		{
			return Extensions.StripColors(rawLine).Split(this._delimiter)[0].Trim(' ');
		}
		/// <summary>
		/// Generates a somewhat user friendly reason as to why the bot crashed, easier to understand than an error code.
		/// </summary>
		/// <param name="errorcode">The error code that the socket exception raised.</param>
		/// <returns>A string containing a user friendly reason as to the exception. If the error code is not in this list, the method
		/// will return Unhandled socket error: Error code : (errorcode)</returns>
		public static string HandleSocketError(int errorcode)
		{
			switch(errorcode)
			{
					case -1: return "The port was either too large or too small.";//Custom used error.
					case 8: return "Not enough memory to complete the socket operation.";
					case 995: return "The operation was aborted, check if the server closed and/or if something happened to your connection.";
					case 10013: return "Permission was denied. Another service may be exclusively using the socket.";
					case 10037: return "Socket already in use. Did you try to call connect on the same address twice?";
					case 10038: return "Attempted to perform an operation on a non socket. Something went wrong.";
					case 10039: return "No destination address was entered into the socket. This can also happen if you use IPAddress.Any";
					case 10041: return "Incorrect protocol. Check you are connecting to a classic server.";
					case 10043: return "Protocol type not supported. Check you are connecting to a classic server.";
					case 10044: return "Socket type not supported. Check you are connecting to a classic server.";
					case 10047: return "Incompatible address family. Check you are connecting to a classic server.";
					case 10048: return "The socket the bot attempted to connect on is already in use by another application.";
					case 10049: return "The address was invalid. The supplied IP address or port is probably invalid.";
					case 10056: return "The socket is already connected. The socket may be in use by another application.";
					case 10057: return "The socket is not connected. This should not happen under normal circumstances.";
					case 10060: return "The connection timed out. This may be a problem with the server or your connection.";
					case 10061: return "Unable to connect to the server, as the connection was refused. Check if the server is up and if it is port forwarded.";
					case 10064: return "Unable to connect to remote server. Check if the server is up and port forwarded.";
					default : return String.Format(null, "Unhandled socket error. Error code : {0}",errorcode);
			}
		}
		
		#region Packet making
		private static byte[] CreateLoginPacket(string username, string verificationKey)
		{
			byte[] packet = new byte[131];
			packet[0] = (byte)ClientPackets.PlayerIdentification; //PacketID
			packet[1] = (byte)0x07; //7 = Classic protocol
			Buffer.BlockCopy(Extensions.StringToBytes(username), 0, packet, 2, 64);
			Buffer.BlockCopy(Extensions.StringToBytes(verificationKey), 0, packet, 66, 64);
			packet[130] = 0x00; //Unused as of right now.
			return packet;
		}

		public void SendLongChat(string message, params object[] args)
		{
			if(args != null)	message = String.Format(null, message, args);
			message = Extensions.StripColors(message);
			StringBuilder part1 = new StringBuilder();
			StringBuilder part2 = new StringBuilder();
			StringBuilder part3 = new StringBuilder();
			StringBuilder part4 = new StringBuilder();
			StringBuilder part5 = new StringBuilder();
			for (int i = 0; i < message.Length; i++)
			{
				if (i <= 48) { part1.Append(message[i]); }
				else if (i > 48 && i <= 96) { part2.Append(message[i]); }
				else if (i > 96 && i <= 144) { part3.Append(message[i]); }
				else if (i > 144 && i <= 192) { part4.Append(message[i]); }
				else if (i > 192 && i <= 240) { part5.Append(message[i]); }
			}
			if (!String.IsNullOrEmpty(part1.ToString())) { SendMessagePacket(part1.ToString()); }
			if (!String.IsNullOrEmpty(part2.ToString())) { SendMessagePacket(part2.ToString()); }
			if (!String.IsNullOrEmpty(part3.ToString())) { SendMessagePacket(part3.ToString()); }
			if (!String.IsNullOrEmpty(part4.ToString())) { SendMessagePacket(part4.ToString()); }
			if (!String.IsNullOrEmpty(part5.ToString())) { SendMessagePacket(part5.ToString()); }
		}
		
		/// <summary>Sends a setblock packet at the specified coordinates.</summary>
		/// <param name="mode">Mode of whether to place or delete. 1 indicates placing, 0 indicates deleting.</param>
		/// <param name="type">The type of block being placed, as a byte.</param>
		public void SendBlockPacket(short x, short y, short z, byte mode, byte type)
		{
			if (_serverSocket == null || _serverSocket.Connected == false ) return;
			if(!_players.ContainsKey(255)) return;
			if (IsValidPosition(x,y,z) == false) return;
			byte[] packet = new byte[9];
			packet[0] = (byte)0x05; //Packet ID.
			short xConverted = IPAddress.HostToNetworkOrder((short)x); //Position updates are * 32, dunno why..
			packet[1] = (byte)(xConverted);
			packet[2] = (byte)(xConverted >> 8);
			short yConverted = IPAddress.HostToNetworkOrder((short)z);
			packet[3] = (byte)(yConverted);
			packet[4] = (byte)(yConverted >> 8);
			short zConverted = IPAddress.HostToNetworkOrder((short)y);
			packet[5] = (byte)(zConverted);
			packet[6] = (byte)(zConverted >> 8);
			packet[7] = mode;
			packet[8] = type;
			try { _serverSocket.Send(packet); }
			catch(System.Net.Sockets.SocketException ex)
			{
				BotExceptionEventArgs socketEvent = new BotExceptionEventArgs(packetError, ex);
				Events.RaiseBotError(socketEvent);
			}
		}

		/// <summary>Sends a position update packet at the specified coordinates. 
		/// This overload already assumes you have accounted for character height.</summary>
		/// <param name="x">A float marking the point along the X-Axis the bot will move to.</param>
		/// <param name="y">A float marking the point along the Y-Axis the bot will move to.</param>
		/// <param name="z">A float marking the point along the Z-Axis the bot will move to.</param>
		/// <param name="yaw">The yaw of the bot.</param>
		/// <param name="pitch">The pitch of the bot.</param>
		public void SendPositionPacket(float x, float y, float z, byte yaw, byte pitch)
		{
			if (_serverSocket == null || _serverSocket.Connected == false ) return;
			if(!_players.ContainsKey(255)) return;
			//if (IsValidPosition(x,y,z) == false) return;
			_players[255].X = x; //Set the X of the bot.
			_players[255].Y = y; //Set the Z of the bot.
			_players[255].Z = z; //Set the Y of the bot.
			byte[] packet = new byte[10];
			packet[0] = (byte)0x08; //Packet ID.
			packet[1] = (byte)255; //Player ID of self.
			short xConverted = IPAddress.HostToNetworkOrder((short)(x * 32));
			packet[2] = (byte)(xConverted);
			packet[3] = (byte)(xConverted >> 8);
			short yConverted = IPAddress.HostToNetworkOrder((short)(z * 32));//Account for character height.
			packet[4] = (byte)(yConverted);
			packet[5] = (byte)(yConverted >> 8);
			short zConverted = IPAddress.HostToNetworkOrder((short)(y * 32));//Yes, I know they're the wrong way around.
			packet[6] = (byte)(zConverted);
			packet[7] = (byte)(zConverted >> 8);
			packet[8] = yaw;
			packet[9] = pitch;
			try { _serverSocket.Send(packet); }
			catch(System.Net.Sockets.SocketException ex)
			{
				BotExceptionEventArgs socketEvent = new BotExceptionEventArgs(packetError, ex);
				Events.RaiseBotError(socketEvent);
			}				
			PositionEventArgs e = new PositionEventArgs(255, _players[255]);
			Events.RaisePlayerMoved(e);
		}
		
		/// <summary>Sends a teleportation packet at the specified coordinates. Less accurate than the float based one.</summary>
		/// <param name="x">A short marking the point along the X-Axis the block is to be placed at.</param>
		/// <param name="y">A short marking the point along the Y-Axis the block is to be placed at.</param>
		/// <param name="z">A short marking the point along the Z-Axis the block is to be placed at.</param>
		public void SendPositionPacket(short x, short y, short z)
		{
			if (_serverSocket == null || _serverSocket.Connected == false ) return;
			if(!_players.ContainsKey(255)) return;
			//if (IsValidPosition(x,y,z) == false) return;
			_players[255].X = x; //Set the X of the bot.
			_players[255].Y = y; //Set the Z of the bot.
			_players[255].Z = z; //Set the Y of the bot.
			byte[] packet = new byte[10];
			packet[0] = (byte)0x08; //Packet ID.
			packet[1] = (byte)255; //Player ID of self.
			short xConverted = IPAddress.HostToNetworkOrder((short)(x * 32));
			packet[2] = (byte)(xConverted);
			packet[3] = (byte)(xConverted >> 8);
			short yConverted = IPAddress.HostToNetworkOrder((short)(z * 32));//Do account for character height in this one.
			packet[4] = (byte)(yConverted);
			packet[5] = (byte)(yConverted >> 8);
			short zConverted = IPAddress.HostToNetworkOrder((short)(y * 32));//Yes, I know they're the wrong way around.
			packet[6] = (byte)(zConverted);
			packet[7] = (byte)(zConverted >> 8);
			packet[8] = _players[255].Yaw;
			packet[9] = _players[255].Pitch;
			try { _serverSocket.Send(packet); }
			catch(System.Net.Sockets.SocketException ex)
			{
				BotExceptionEventArgs socketEvent = new BotExceptionEventArgs(packetError, ex);
				Events.RaiseBotError(socketEvent);
			}						
			PositionEventArgs e = new PositionEventArgs(255, _players[255]);
			Events.RaisePlayerMoved(e);
		}

		/// <summary>Sends a chat message in game. Has a maximum length of 64 characters.</summary>
		/// <param name="message">String to send.</param>
		public void SendMessagePacket(string message)
		{
			if(message.Length > 64) message = message.Substring(0, 64);
			if (_serverSocket == null || _serverSocket.Connected == false ) return;
			byte[] packet = new byte[66]; //PID + unused + message
			packet[0] = (byte)0x0d; //Packet ID.
			packet[1] = (byte)0xff; //Unused
			Buffer.BlockCopy(Extensions.StringToBytes(message), 0, packet, 2, 64);
			try { _serverSocket.Send(packet); }
			catch(System.Net.Sockets.SocketException ex)
			{
				BotExceptionEventArgs socketEvent = new BotExceptionEventArgs(packetError, ex);
				Events.RaiseBotError(socketEvent);
			}			
		}
		#endregion
	}
}
