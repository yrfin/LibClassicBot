using System;
using System.Net;
using System.Text;
using LibClassicBot.Events;
using LibClassicBot.Networking;

namespace LibClassicBot
{
	public partial class ClassicBot
	{
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
			if (args.Length > 0)
			{
				message = String.Format(message, args);
			}
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
			_serverSocket.Send(packet);
		}

		/// <summary>Sends a teleportation packet at the specified coordinates.</summary>
		/// <param name="x">A short marking the point along the X-Axis the block is to be placed at.</param>
		/// <param name="y">A short marking the point along the Y-Axis the block is to be placed at.</param>
		/// <param name="z">A short marking the point along the Z-Axis the block is to be placed at.</param>
		public void SendPositionPacket(short x, short y, short z)
		{
			if (_serverSocket == null || _serverSocket.Connected == false ) return;
			if(!_players.ContainsKey(255)) return;
			if (IsValidPosition(x,y,z) == false) return;
			_players[255].X = x; //Set the X of the bot.
			_players[255].Y = y; //Set the Z of the bot.
			_players[255].Z = z; //Set the Y of the bot.
			byte[] packet = new byte[10];
			packet[0] = (byte)0x08; //Packet ID.
			packet[1] = (byte)255; //Player ID of self.
			short xConverted = IPAddress.HostToNetworkOrder((short)(x * 32));
			packet[2] = (byte)(xConverted);
			packet[3] = (byte)(xConverted >> 8);
			short yConverted = IPAddress.HostToNetworkOrder((short)(z * 32));//((z + 1.21f) * 32)); character height
			packet[4] = (byte)(yConverted);
			packet[5] = (byte)(yConverted >> 8);
			short zConverted = IPAddress.HostToNetworkOrder((short)(y * 32));//Yes, I know they're the wrong way around.
			packet[6] = (byte)(zConverted);
			packet[7] = (byte)(zConverted >> 8);
			packet[8] = _players[255].Yaw;
			packet[9] = _players[255].Pitch;
			_serverSocket.Send(packet);
			PositionEventArgs e = new PositionEventArgs(255, _players[255].Name, _players[255].X, _players[255].Y, _players[255].Z, _players[255].Yaw, _players[255].Pitch);
			Events.RaisePlayerMoved(e);
		}

		/// <summary>Sends a chat message in game. Has a maximum length of 64 characters.</summary>
		/// <param name="message">String to send.</param>
		public void SendMessagePacket(string message)
		{
			if(message.Length > 64) message = message.Substring(0,64);
			if (_serverSocket == null || _serverSocket.Connected == false ) return;
			byte[] packet = new byte[66]; //PID + unused + message
			packet[0] = (byte)0x0d; //Packet ID.
			packet[1] = (byte)0xff; //Unused
			Buffer.BlockCopy(Extensions.StringToBytes(message), 0, packet, 2, 64);
			this._serverSocket.Send(packet);
		}
		#endregion
	}
}
