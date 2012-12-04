using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LibClassicBot
{
	/// <summary>
	/// Various utilities relating to chat messages and logging in.
	/// </summary>
	public static class Extensions
	{
		/// <summary>
		/// Strips all colours from a given chatline. Based on code from fCraft.
		/// </summary>
		public static string StripColors( string input ) {
			if( input.IndexOf( '&' ) == -1 ) {//No & symbols were found.
				return input;
			} else {
				StringBuilder output = new StringBuilder( input.Length );
				for( int i = 0; i < input.Length; i++ ) {
					if( input[i] == '&' ) {
						if( i == input.Length - 1 ) {
							break;
						}
						i++;
					} else {
						output.Append( input[i] );
					}
				}
				return output.ToString();
			}
		}

		/// <summary>
		/// Converts a string into a byte array compatible with classic servers.
		/// </summary>
		/// <param name="s">The string used to convert.</param>
		/// <returns>A converted byte array of the string.</returns>
		public static byte[] StringToBytes(string message)
		{
			if(message.Length > 64) message = message.Substring(0, 64); //Failsafe
			byte[] MessageBytes = System.Text.Encoding.ASCII.GetBytes(message);
			byte[] FullArray = new byte[64];
			for (int i = 0; i < FullArray.Length; i++)
			{
				FullArray[i] = 32; //' ' Vanilla doesn't have a problem if we use 0, but other servers do.
			}
			Buffer.BlockCopy(MessageBytes,0,FullArray,0,MessageBytes.Length);
			return FullArray;
		}

		
		#region Math
		/// <summary>
		/// Converts a degrees int value into a byte yaw / pitch value.
		/// Note that this is only supported for 0 - 360 degrees.
		/// </summary>
		/// <param name="degree">The degree to convert into yaw / putch. 
		/// If this is greater than 360 or less than 0, an exception will be thrown.</param>
		/// <exception cref="System.ArguementOutOfRangeException">Thrown if the value is greater than 360
		/// or less than 0.</exception>
		/// <returns>The degrees converted into a yaw / pitch value.</returns>
		public static byte DegreesToYaw(int degree)
		{
			if(degree < 0 || degree > 360)
				throw new ArgumentOutOfRangeException("degree", "Value less than 0 or greater than 360.");
			int result = (degree * 256 / 360);
			if(result == 256) return (byte)0; //360 is really just 0.
			else return (byte)result;
		}
		
		/// <summary>
		/// Converts a Yaw / Pitch byte value into a standard degrees value.
		/// </summary>
		/// <param name="yaw">The yaw or pitch value to convert.</param>
		/// <returns>The yaw or pitch converted into degrees. For example, a yaw of 64 would return 90 degrees.
		/// A pitch of 0 returns 0 degrees. (Could also be considered 360)</returns>
		public static int YawToDegrees(byte yaw)
		{
			float result = (yaw / 256f * 360f);
			return (int)result;
		}
		
		#endregion

		
		#region Logging In
		
		/// <summary>
		/// This method performs a login to minecraft.net. It returns three non null values, and one value that may be null. (The migrated username)
		/// </summary>
		/// <param name="username">The username to use. Can be both a Migrated account, and a normal username.</param>
		/// <param name="password">The password to use to login.</param>
		/// <param name="gameurl">The url of the server to login to, for parsing of mppass / verification key.</param>
		/// <param name="_serverIP">The IP Address of the server to connect to.</param>
		/// <param name="_serverPort">The port to connect on.</param>
		/// <param name="verificationkey">The verification key / mppass to use.</param>
		/// <param name="migratedUsername">This will be null if the username is not migrated, otherwise, it returns a case correct version of their username.</param>
		/// <returns>True if it was able to login and return all the required values, false if not.</returns>
		public static bool Login(string username, string password, string gameurl, out IPAddress _serverIP, out int _serverPort, out string verificationkey, out string migratedUsername)
		{
			string html = LoginAndReadPage(username, password, gameurl);
			string serveraddress = ReadValue(html.Substring(html.IndexOf("\"server\""), 40));
			string port = ReadValue(html.Substring(html.IndexOf("\"port\""), 40));
			string mppass = ReadValue(html.Substring(html.IndexOf("\"mppass\""), 80));
			verificationkey = mppass;
			_serverIP = IPAddress.Parse(serveraddress);
			_serverPort = Int16.Parse(port);
			if(username.Contains("@")) 
			{
				migratedUsername = GetMigratedUsername(username, password);
				if(migratedUsername == null) return false; //Wasn't able to login to minecraft.net.
				else return true; //We got the correct username for the migrated account.
			}
			else { migratedUsername = null; }
			return true;
		}
		private static string LoginAndReadPage(string username, string password, string gameurl)
		{
			//Step 1.
			LoginCookie(username,password);
			//Step 2.
			//Go to game url and GET using _uid cookie.
			//Parse the page to find server, port, mpass strings.
			WebRequest step3Request = WebRequest.Create(gameurl);
			if(sessionCookie != null) step3Request.Headers.Add(sessionCookie);
			using (Stream s4 = step3Request.GetResponse().GetResponseStream())
			{
				string html = new StreamReader(s4).ReadToEnd();
				GC.Collect(0); //Get rid of stored WebProxy by the HttpWebRequest.
				return html;
			}
			
		}
		
		private const string LoginUri = "https://login.minecraft.net";
		
		/// <summary>
		/// Gets the original username of a migrated account.
		/// </summary>
		/// <param name="migratedaccount">The migrated account username. This is defined by the @ in the username.</param>
		/// <param name="password">The password to use, which is required.</param>
		/// <returns>The case correct username of the account. If there was an error while trying to download the page
		/// or parse the result, this returns null.</returns>
		private static string GetMigratedUsername(string migratedaccount, string password)
		{
			try {
				string postData = String.Format("?user={0}&password={1}&version=13", migratedaccount, password);				
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(LoginUri+postData);
				request.Method = "GET"; //Although it's meant to be POST, GET works too.
				request.ContentType = "application/x-www-form-urlencoded"; //Needed to work.
				string responseString = new StreamReader(request.GetResponse().GetResponseStream()).ReadToEnd();
				string[] responseSplit = responseString.Split(':');
				return responseSplit[2]; //Case correct username, even for Migrated accounts.
			}
			catch { return null; } //Could give both a WebException and an IndexOutOfRange.
		}
		private static string ReadValue(string s)
		{
			string start = "value=\"";
			string end = "\"";
			string ss = s.Substring(s.IndexOf(start) + start.Length);
			ss = ss.Substring(0, ss.IndexOf(end));
			return ss;
		}
		
		static string sessionCookie;
		
		const string IncorrectUserPass = "Wrong username or password.";
		
		static void LoginCookie(string username, string password)
		{
			string formData = string.Format(null, "username={0}&password={1}", username, password);
			//Step 1.
			//Go to http://minecraft.net/login and POST "username={0}&password={1}" You will receive logged in cookie ("_uid").
			//Because of multipart http page, HttpWebRequest has some trouble receiving cookies in step 2,
			//so it is easier to just use raw TcpClient for this.
			using (TcpClient step2Client = new TcpClient("minecraft.net", 80))
			{
				NetworkStream stream = step2Client.GetStream();
				StreamWriter sw = new StreamWriter(stream);
				sw.WriteLine("POST /login HTTP/1.0");
				sw.WriteLine("Content-Type: application/x-www-form-urlencoded");
				sw.WriteLine("Content-Length: " + formData.Length);
				sw.WriteLine("");
				sw.WriteLine(formData);
				sw.Flush();
				sessionCookie = null; //Clear existing cookie.
				using(StreamReader sr = new StreamReader(stream))
				{
					for (; ; )
					{
						string rawLine = sr.ReadLine();
						if (rawLine == null) { break; }
						if (rawLine.Contains("Set-Cookie"))
						{
							if(rawLine.Contains("secure.error")) throw new InvalidOperationException(IncorrectUserPass); //Incorrect username or password.
							if(rawLine.Contains("SESSION")) {
								rawLine = rawLine.Substring(4);
								sessionCookie = rawLine;
								break;
							}
						}
					}
				}
				sw.Dispose(); //If we call dispose earlier, the underlying stream is closed before we can read it.
			}
		}
		
		/*static void LoginCookie2(string username, string password)
		{
			//Alternative, but slow method.
			string dataToPost = String.Format("username={0}&password={1}", username, password);
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(LoginSecure);
			request.CookieContainer = new CookieContainer();
			if(PlaySession != null) request.CookieContainer.Add(PlaySession);
			if(dataToPost != null) {
				request.Method = "POST";
				request.ContentType = "application/x-www-form-urlencoded";
				byte[] data = Encoding.UTF8.GetBytes( dataToPost );
				request.ContentLength = data.Length;
				using( Stream stream = request.GetRequestStream() ) {
					stream.Write( data, 0, data.Length ); }
			}
			HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			foreach(Cookie c in response.Cookies) {
				if(c.Name == "PLAY_SESSION") PlaySession = c;
			}
		}
		const string LoginSecure = "https://minecraft.net/login";
		static Cookie PlaySession = null;*/
		#endregion
	}
}