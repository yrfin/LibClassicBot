using System;
using System.Net;
using System.Net.Sockets;

namespace LibClassicBot.Remote.Events
{
	public class RemoteEvents
	{
		/// <summary>Occurs when a remote session is started.</summary>
		public event EventHandler<SessionStartedEventArgs> RemoteSessionStarted;

		/// <summary>Occurs when a remote user logs in and is verified. </summary>
		public event EventHandler<RemoteLoginEventArgs> RemoteUserLoggedIn;
		
		/// <summary>Occurs when the bot is kicked. To automatically make it reconnect, use ReconnectAfterKick = true;</summary>
		public event EventHandler<SessionEndedEventArgs> RemoteSessionEnded;


		/// <summary>Raises a new ChatMessage Event.</summary>
		/// <param name="e">MessageEventArgs to send</param>
		internal void RaiseSessionStarted(SessionStartedEventArgs e)
		{
			System.EventHandler<SessionStartedEventArgs> sesStartEvent = RemoteSessionStarted;
			if (sesStartEvent == null) return;
			sesStartEvent(null, e);
		}

		/// <summary>Raises a new PlayerMoved Event.</summary>
		/// <param name="e">PositionEventArgs to send</param>
		internal void RaiseUserLoggedIn(RemoteLoginEventArgs e)
		{
			System.EventHandler<RemoteLoginEventArgs> loggedinEvent = RemoteUserLoggedIn;
			if (loggedinEvent == null) return;
			loggedinEvent(null, e);
		}
		
		/// <summary>Raises a new PacketReceived Event.</summary>
		/// <param name="e">PacketEventArgs to send</param>
		internal void RaiseSessionEnded(SessionEndedEventArgs e)
		{
			System.EventHandler<SessionEndedEventArgs> sesEndEvent = RemoteSessionEnded;
			if (sesEndEvent == null) return;
			sesEndEvent(null, e);
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