using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;
using LibClassicBot;
using LibClassicBot.Drawing;
using LibClassicBot.Events;

namespace ClassicBotCoreTest
{
	class Program
	{
		#if !MONO
		[DllImport("psapi.dll")]
		static extern int EmptyWorkingSet(IntPtr hwProc);
		
		static void MinimizeFootprint() {
			EmptyWorkingSet(System.Diagnostics.Process.GetCurrentProcess().Handle);
		}
		#endif
		
		public static void Main(string[] args)
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("Welcome to LibClassicBot beta.");
			Console.WriteLine("Below is a list of commands and how to use them");
			Console.WriteLine(".position - Announces the position of the bot in the map.");
			Console.WriteLine(".allow <user> - Adds a user to a list of allowed operators.");
			Console.WriteLine(".disallow <user> - Removes a user from the list of allowed operators.");
			Console.WriteLine(".say <message> - Makes the bot send the specified message.");
			Console.WriteLine(".players - Lists the visibile players to the bot in the current map.");
			Console.WriteLine(".move <x,y,z> - Moves the bot to the specified coordinates.");
			Console.WriteLine(".place <x,y,z> - Attempts to place a block at the specified coordinates.");
			Console.WriteLine(".haspaid <username> - Announces if a user has paid or not.");
			Console.WriteLine(".cuboid <blocktype> - Cuboids between two points.");
			Console.WriteLine(".cuboid <blocktype> - Draws a hollow cuboid between two points.");
			Console.WriteLine(".cuboid <blocktype> - Draws a wireframe between two points.");
			Console.WriteLine(".ellipsoid <blocktype> - Draws an ellipsoid between two points.");
			Console.WriteLine(".pyramid <blocktype> - Draws an upwards pyramid between two points.");
			Console.WriteLine(".follow <username> - Follows player. (Case sensitive)");
			Console.WriteLine(".speed <number> - The number of blocks to place per second.");
			Console.WriteLine(".abort - Stops the currently running draw operation.");
			Console.WriteLine(".drawimg 0 <filename> - Attempts to draw the specified image.");
			Console.ResetColor();
			
			Console.WriteLine("Enter the username to be used by the bot: (Minecraft account)");
			string username = Console.ReadLine();
			Console.WriteLine("Enter the password to be used by the bot: (Minecraft account)");
			string password = Console.ReadLine();
			Console.WriteLine("Enter the address of the server to connect to: ");
			string hash = Console.ReadLine();
			if(!hash.StartsWith("http"))
			{
				if(hash.StartsWith("minecraft")) hash = "http://"+hash;
				else hash = "http://minecraft.net/classic/play/" + hash;
			}

			ClassicBot Bot1 = new ClassicBot(username, password, hash, "operators.txt");
			ConsoleLogger logger = new ConsoleLogger();
			Bot1.RegisterLogger( logger );
			FileLogger file = new FileLogger();
			Bot1.RegisterLogger( file );
			Bot1.Events.ChatMessage += Bot1_ChatMessage;
			Bot1.Events.PlayerMoved += Bot1_PlayerMoved;
			Bot1.Events.RemoteSessionStarted += RemoteSessionStarted;
			Bot1.Events.RemoteUserLoggedIn += RemoteUserLoggedIn;
			Bot1.Events.RemoteSessionEnded += RemoteSessionEnded;
			#if !MONO
			Bot1.Events.MapLoaded += Bot1_MapLoaded;
			#endif
			
			#region Plugins
			CommandDelegate MazeCommand = delegate(string Line)
			{
				Maze maze = new Maze();
				maze.originalchatline = Line;
				Bot1.SetDrawer(Line, maze, 2);
			};
			Bot1.RegisteredCommands.Add("maze", MazeCommand);
			
			CommandDelegate DrawCommand = delegate(string Line)
			{
				DrawImage img = new DrawImage();
				img.originalchatline = Line;
				Bot1.SetDrawer(Line, img, 2);
			};
			Bot1.RegisteredCommands.Add("drawimg", DrawCommand);
			
			CommandDelegate PositionCommand = delegate(string Line)
			{
				Bot1.SendMessagePacket(String.Format("Positon in world is at {0},{1},{2}.", Bot1.X, Bot1.Y, Bot1.Z));
			};
			Bot1.RegisteredCommands.Add("position", PositionCommand);
			
			CommandDelegate AddOpCommand = delegate(string Line)
			{
				string[] full = Bot1.GetMessage(Line).Split(new char[] {' '}, 2);
				Bot1.AddOperator(full[1], true);
				Bot1.SendMessagePacket("Allowed user: " + full[1]);
			};
			Bot1.RegisteredCommands.Add("allow", AddOpCommand);
			
			CommandDelegate RemoveOpCommand = delegate(string Line)
			{
				string[] full = Bot1.GetMessage(Line).Split(new char[] {' '}, 1);
				Bot1.RemoveOperator(full[1], true);
				Bot1.SendMessagePacket("Disallowed user: "+ full[1]);
			};
			Bot1.RegisteredCommands.Add("disallow", RemoveOpCommand);

			CommandDelegate SayCommand = delegate(string Line)
			{
				string[] full = Bot1.GetMessage(Line).Split(new char[] {' '}, 2);
				Bot1.SendMessagePacket(full[1]);
			};
			Bot1.RegisteredCommands.Add("say", SayCommand);
			
			CommandDelegate PlayersCommand = delegate(string Line)
			{
				List<string> Names = new List<string>();
				foreach(Player player in Bot1.Players.Values)
				{
					Names.Add(player.Name);
				}
				string output = String.Join(",", Names.ToArray());
				Bot1.SendLongChat("Players in current world: " + output);
			};
			Bot1.RegisteredCommands.Add("players", PlayersCommand);

			CommandDelegate MoveCommand = delegate(string Line)
			{
				string[] full = Bot1.GetMessage(Line).Split(new char[] {' '}, 2);
				string[] coords = full[1].Split(new char[] { ',' }, 3);
				Bot1.SendPositionPacket(Int16.Parse(coords[0]), Int16.Parse(coords[1]), Int16.Parse(coords[2]));
			};
			Bot1.RegisteredCommands.Add("move", MoveCommand);
			
			CommandDelegate PlaceCommand = delegate(string Line)
			{
				string[] full = Bot1.GetMessage(Line).Split(new char[] {' '}, 2);
				string[] coords = full[1].Split(new char[] { ',' }, 4);
				Bot1.SendBlockPacket(Int16.Parse(coords[0]), Int16.Parse(coords[1]), Int16.Parse(coords[2]), 1, Byte.Parse(coords[3]));
			};
			Bot1.RegisteredCommands.Add("place", PlaceCommand);
			
			CommandDelegate HasPaidCommand = delegate(string Line)
			{
				string[] full = Bot1.GetMessage(Line).Split(new char[] {' '}, 2);
				try
				{
					bool paid;
					WebClient client = new WebClient();
					string response = client.DownloadString("https://minecraft.net/haspaid.jsp?user="+full[1]);
					if(Boolean.TryParse(response, out paid)) 
						Bot1.SendMessagePacket( response );
				}
				catch(WebException ex)
				{
					if (ex.Status == WebExceptionStatus.ProtocolError)
					{
						HttpWebResponse response = ex.Response as HttpWebResponse;
						if (response != null)
							Bot1.SendMessagePacket("minecraft.net returned: " + (int)response.StatusCode + " " +response.StatusCode.ToString());
					}
					else
						Bot1.SendMessagePacket("Unhandled error occured: "+ex.Status.ToString());
				}
			};
			Bot1.RegisteredCommands.Add("haspaid", HasPaidCommand);
			
			CommandDelegate FollowCommand = delegate(string Line)
			{
				string[] full = Bot1.GetMessage(Line).Split(new char[] {' '}, 2);
				personfollowed = full[1];
				Bot1.SendMessagePacket("Following user "+full[1]);
			};
			Bot1.RegisteredCommands.Add("follow", FollowCommand);
			
			CommandDelegate CuboidCommand = delegate(string Line)
			{
				Cuboid cuboid = new Cuboid();
				Bot1.SetDrawer(Line, cuboid, 2);
			};
			Bot1.RegisteredCommands.Add("cuboid", CuboidCommand);
			
			CommandDelegate PyramidCommand = delegate(string Line)
			{
				Pyramid pyramid = new Pyramid();
				Bot1.SetDrawer(Line, pyramid, 2);
			};
			Bot1.RegisteredCommands.Add("pyramid", PyramidCommand);
			
			CommandDelegate AbortCommand = delegate(string Line)
			{
				Bot1.CancelDrawer();
				personfollowed = String.Empty;
			};
			Bot1.RegisteredCommands.Add("abort", AbortCommand);
			
			CommandDelegate SpeedCommand = delegate(string Line)
			{
				string[] full = Bot1.GetMessage(Line).Split(new char[] {' '}, 2);
				Bot1.CuboidSleepTime = 1000 / Int32.Parse(full[1]);
			};
			Bot1.RegisteredCommands.Add("speed", SpeedCommand);

			CommandDelegate EllipsoidCommand = delegate(string Line)
			{
				Ellipsoid ellipsoid = new Ellipsoid();
				Bot1.SetDrawer(Line, ellipsoid, 2);
			};
			Bot1.RegisteredCommands.Add("ellipsoid", EllipsoidCommand);

			CommandDelegate CuboidHCommand = delegate(string Line)
			{
				CuboidHollow cuboidh = new CuboidHollow();
				Bot1.SetDrawer(Line, cuboidh, 2);
			};
			Bot1.RegisteredCommands.Add("cuboidh", CuboidHCommand);

			CommandDelegate CuboidWCommand = delegate(string Line)
			{
				CuboidWireframe cuboidw = new CuboidWireframe();
				Bot1.SetDrawer(Line, cuboidw, 2);
			};
			Bot1.RegisteredCommands.Add("cuboidw", CuboidWCommand);
			
			CommandDelegate LineCommand = delegate(string Line)
			{
				Line line = new Line();
				Bot1.SetDrawer(Line, line, 2);
			};
			Bot1.RegisteredCommands.Add("line", LineCommand);
			
			CommandDelegate IgnoreCommand = delegate(string Line)
			{
				string[] full = Bot1.GetMessage(Line).Split(new char[] {' '}, 2);
				Bot1.IgnoredUserList.Add(full[1]);
				Bot1.SendMessagePacket("Ignored user: " + full[1]);
			};
			Bot1.RegisteredCommands.Add("ignore", IgnoreCommand);

			CommandDelegate UnIgnoreCommand = delegate(string Line)
			{
				string[] full = Bot1.GetMessage(Line).Split(new char[] {' '}, 2);
				Bot1.IgnoredUserList.Remove(full[1]);
				Bot1.SendMessagePacket("Unignored user: " + full[1]);
			};
			Bot1.RegisteredCommands.Add("unignore", UnIgnoreCommand);
			
			/*CommandDelegate TestPosCommand = delegate(string Line)
			{ //Ain't no way to stop it. Uncomment with severe caution.
				new System.Threading.Thread(
					delegate() {
						Random rnd = new Random();
						while(true){
							
							int rndval1 = rnd.Next(0, 360);
							byte newpitch = Extensions.DegreesToYaw(rndval1);
							int rndval2 = rnd.Next(0, 360);
							byte newyaw = Extensions.DegreesToYaw(rndval2);
							Bot1.SendPositionPacket(Bot1.X, Bot1.Y, Bot1.Z, newyaw, newpitch);
							System.Threading.Thread.Sleep(10);
						}
					}).Start();
			};
			Bot1.RegisteredCommands.Add("testpos", TestPosCommand);*/
			#endregion
			
			StaticBot1 = Bot1;
			Bot1.Start(false);
			
		loop:
			{
				string response = Console.ReadLine();
				Bot1.SendMessagePacket(response);
				goto loop;
			}
		}
		
		static string personfollowed = String.Empty;

		#if !MONO
		static void Bot1_MapLoaded(object sender, MapLoadedEventArgs e) {
			MinimizeFootprint();
		}
		#endif

		static void Bot1_PlayerMoved(object sender, PositionEventArgs e)
		{
			string name = e.player.Name;
			if(name.StartsWith("&")) name = name.Substring(2);
			if(personfollowed == name)
			{
				StaticBot1.SendPositionPacket(e.player.X, e.player.Y, e.player.Z, e.player.Yaw, e.player.Pitch);
			}
		}

		static void RemoteSessionEnded(object sender, SessionEndedEventArgs e)
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("Session with {0} ended.",e.Username);
			Console.ResetColor();
		}

		static void RemoteUserLoggedIn(object sender, RemoteLoginEventArgs e)
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("Session {0} user identified as {1}",e.RemoteEndPoint,e.Username);
			Console.ResetColor();
		}

		static void RemoteSessionStarted(object sender, SessionStartedEventArgs e)
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("Session started with {0}",e.RemoteEndPoint);
			Console.ResetColor();
		}
		
		static ClassicBot StaticBot1;
		
		static void LogError(string errormessage)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(errormessage);
			Console.ResetColor();
		}

		static void Bot1_ChatMessage(object sender, MessageEventArgs e) {
			StaticBot1.MessageAllRemoteClients(e.Line);
		}
		
	}
}