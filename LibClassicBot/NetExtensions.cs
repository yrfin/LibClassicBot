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

		#region Logging In
		public static void Login(string username, string password, string gameurl, out IPAddress _serverIP, out int _serverPort, out string verificationkey)
		{
			string html = LoginAndReadPage(username, password, gameurl);
			string serveraddress = ReadValue(html.Substring(html.IndexOf("\"server\""), 40));
			string port = ReadValue(html.Substring(html.IndexOf("\"port\""), 40));
			string mppass = ReadValue(html.Substring(html.IndexOf("\"mppass\""), 80));
			verificationkey = mppass;
			_serverIP = IPAddress.Parse(serveraddress);
			_serverPort = Int16.Parse(port);
		}
		private static string LoginAndReadPage(string username, string password, string gameurl)
		{
			//Step 1.
			LoginCookie(username,password);
			//Step 2.
			//Go to game url and GET using JSESSIONID cookie.
			//Parse the page to find server, port, mpass strings.
			WebRequest step3Request = HttpWebRequest.Create(gameurl);
			if(sessionCookie != null) step3Request.Headers.Add(sessionCookie);
			using (Stream s4 = step3Request.GetResponse().GetResponseStream())
			{
				string html = new StreamReader(s4).ReadToEnd();
				return html;
			}
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
		
		const string IncorrectUserPass = "Wrong username or password, or the account has been migrated.";
		
		static void LoginCookie(string username, string password)
		{
			string formData = string.Format(null, "username={0}&password={1}", username, password);
			//Step 1.
			//Go to http://minecraft.net/login and POST "username={0}&password={1}" using JSESSIONID cookie.
			//You will receive logged in cookie ("_uid").
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
				sw.Dispose(); //If we call dispose earlier, the stream is closed before we can read it.
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