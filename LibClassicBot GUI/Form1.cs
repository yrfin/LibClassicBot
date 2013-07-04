using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Text;
using System.Windows.Forms;

using LibClassicBot;
using LibClassicBot.Drawing;
using LibClassicBot.Events;

namespace LibClassicBot_GUI
{
	/// <summary>
	/// Description of Form1.
	/// </summary>
	public partial class MainForm : Form
	{
		private delegate void ProgressBarCallback(int percentDone);
		private void ProgressBarUpdate(int percentDone) { progressBar1.Value = percentDone; }
		
		private delegate void MainTextCallback(string windowtitle);
		private void MainTextUpdate(string windowtitle) { Text = windowtitle; }
		
		public MainForm()
		{
			InitializeComponent();
		}
		/// <summary>
		/// Updates the progress bar indicating how far the bot has gone through loading the map.
		/// </summary>
		/// <param name="percentdone"></param>
		public void UpdateProgressBar(int percentdone)
		{
			ProgressBarCallback pbc = new ProgressBarCallback(ProgressBarUpdate);
			progressBar1.Invoke(pbc, new object[] { percentdone } );
		}
		
		bool firstrun = true;
		static string personfollowed = String.Empty;
		static ClassicBot StatBot = null;
		
		/// <summary>
		/// Loads all the events. This should only be called once.
		/// </summary>
		private void LoadBotEvents(ClassicBot bot)
		{
			bot.Events.MapProgress += MapLoading;
			bot.Events.ChatMessage += ChatMessage;
			//bot.Events.BotException += BotException;
			bot.Events.GotKicked += GotKicked;
			bot.Events.MapLoaded += MapLoaded;
		}

		/// <summary>
		/// Determines whether or not the given character is the character that follows an and symbol, to make up a minecraft colour code.
		/// </summary>
		/// <param name="c">The character to check.</param>
		/// <param name="color">The ConsoleColor that is used by AppendLog().</param>
		/// <returns>True if the given character was valid, and also returns a ConsoleColor with the equivalent of the Minecraft colour.
		/// By default, it will return Consolecolor.White.</returns>
		public static bool IsColourCodeChar(char c, out Color color)
		{
			switch(c)
			{
					case '0': color = Color.Black; return true; //Black
					case '1': color = Color.DarkBlue; return true; //Dark Blue / Navy
					case '2': color = Color.DarkGreen; return true; //Dark Green / Green
					case '3': color = Color.DarkCyan; return true; //Dark Cyan / Teal
					case '4': color = Color.DarkRed; return true; //Dark Red / Maroon
					case '5': color = Color.DarkMagenta; return true; //Dark Magenta / Purple
					case '6': color = Color.Olive; return true; //Dark Yellow / Olive
					case '7': color = Color.DarkGray; return true; //Dark Gray / Silver
					case '8': color = Color.Gray; return true; //Gray
					case '9': color = Color.Blue; return true; //Blue
					case 'a': color = Color.Green; return true; //Green / Lime
					case 'b': color = Color.Cyan; return true; //Cyan / Aqua
					case 'c': color = Color.Red; return true; //Red
					case 'd': color = Color.Magenta; return true; //Magenta
					case 'e': color = Color.Yellow; return true; //Yellow
					case 'f': color = Color.White; return true; //White
					default: color = Color.White; return false; //Unknown. Go back to default
			}
		}

		public void AppendLog(string input)
		{
			if(!input.EndsWith("&")) input += "&"; //Since I can't seem to do it any other way.
			Color currentColor = Color.White;
			for (int i = 0; i < input.Length; i++)
			{
				if (input[i] == '&' && i < input.Length - 1)
				{
					i++;//Move to colour code character.
					if(IsColourCodeChar(input[i],out currentColor))
					{
						i++;
						StringBuilder sb = new StringBuilder();
						while (input[i] != '&') //Look at fixing this up so we don't always have to append & to the end..
						{
							sb.Append(input[i]);
							i++;
						}
						i--; //Move back a character, as otherwise we go one too far and consume the next &.
						RichTextBoxExtensions.AppendText(logBox, sb.ToString(), currentColor);
					}
				}
			}
			RichTextBoxExtensions.AppendText(logBox, "\r\n", Color.White);
		}

		void Button1Click(object sender, EventArgs e)
		{
			ClassicBot cc = new ClassicBot(userBox.Text, passwordBox.Text, addBox.Text,"operators.txt");
			AddPlugins(cc);
			cc.Start(false);
			StatBot = cc;
			if(firstrun) LoadBotEvents(cc);
			
		}
		
		private void AddPlugins(ClassicBot bot)
		{
			#region Plugins
			
			CommandDelegate PositionCommand = delegate(string Line)
			{
				bot.SendMessagePacket(String.Format("Positon in world is at {0},{1},{2}.", bot.X, bot.Y, bot.Z));
			};
			bot.RegisteredCommands.Add("position", PositionCommand);
			
			CommandDelegate AddOpCommand = delegate(string Line)
			{
				string[] full = bot.GetMessage(Line).Split(new char[] {' '}, 2);
				bot.AddOperator(full[1], true);
				bot.SendMessagePacket("Allowed user: " + full[1]);
			};
			bot.RegisteredCommands.Add("allow", AddOpCommand);
			
			CommandDelegate RemoveOpCommand = delegate(string Line)
			{
				string[] full = bot.GetMessage(Line).Split(new char[] {' '}, 1);
				bot.RemoveOperator(full[1], true);
				bot.SendMessagePacket("Disallowed user: "+ full[1]);
			};
			bot.RegisteredCommands.Add("disallow", RemoveOpCommand);

			CommandDelegate SayCommand = delegate(string Line)
			{
				string[] full = bot.GetMessage(Line).Split(new char[] {' '}, 2);
				bot.SendMessagePacket(full[1]);
			};
			bot.RegisteredCommands.Add("say", SayCommand);
			
			CommandDelegate PlayersCommand = delegate(string Line)
			{
				List<string> Names = new List<string>();
				foreach(Player player in bot.Players.Values)
				{
					Names.Add(player.Name);
				}
				string output = String.Join(",", Names.ToArray());
				bot.SendLongChat("Players in current world: " + output);
			};
			bot.RegisteredCommands.Add("players", PlayersCommand);

			CommandDelegate MoveCommand = delegate(string Line)
			{
				string[] full = bot.GetMessage(Line).Split(new char[] {' '}, 2);
				string[] coords = full[1].Split(new char[] { ',' }, 3);
				bot.SendPositionPacket(Int16.Parse(coords[0]), Int16.Parse(coords[1]), Int16.Parse(coords[2]));
			};
			bot.RegisteredCommands.Add("move", MoveCommand);
			
			CommandDelegate PlaceCommand = delegate(string Line)
			{
				string[] full = bot.GetMessage(Line).Split(new char[] {' '}, 2);
				string[] coords = full[1].Split(new char[] { ',' }, 3);
				bot.SendBlockPacket(Int16.Parse(coords[0]), Int16.Parse(coords[1]), Int16.Parse(coords[2]), 1, 29);
			};
			bot.RegisteredCommands.Add("place", PlaceCommand);
			
			CommandDelegate HasPaidCommand = delegate(string Line)
			{
				string[] LineSplit = Extensions.StripColors(Line).Split(' ');
				try
				{
					bool b;
					WebClient c = new WebClient();
					string response = c.DownloadString("https://minecraft.net/haspaid.jsp?user="+LineSplit[2]);
					if(Boolean.TryParse(response, out b)) bot.SendMessagePacket(response);
				}
				catch(WebException ex)
				{
					if (ex.Status == WebExceptionStatus.ProtocolError)
					{
						HttpWebResponse response = ex.Response as HttpWebResponse;
						if (response != null)
							bot.SendMessagePacket("minecraft.net returned: " + (int)response.StatusCode + " " +response.StatusCode.ToString());
					}
					else
						bot.SendMessagePacket("Unhandled error occured: "+ex.Status.ToString());
				}
			};
			bot.RegisteredCommands.Add("haspaid", HasPaidCommand);
			
			CommandDelegate FollowCommand = delegate(string Line)
			{
				string[] full = bot.GetMessage(Line).Split(new char[] {' '}, 2);
				personfollowed = full[1];
				bot.SendMessagePacket("Following user "+full[1]);
			};
			bot.RegisteredCommands.Add("follow", FollowCommand);
			
			CommandDelegate CuboidCommand = delegate(string Line)
			{
				Cuboid cuboid = new Cuboid();
				bot.SetDrawer(Line, cuboid, 2);
			};
			bot.RegisteredCommands.Add("cuboid", CuboidCommand);
			
			CommandDelegate PyramidCommand = delegate(string Line)
			{
				Pyramid pyramid = new Pyramid();
				bot.SetDrawer(Line, pyramid, 2);
			};
			bot.RegisteredCommands.Add("pyramid", PyramidCommand);
			
			CommandDelegate AbortCommand = delegate(string Line)
			{
				bot.CancelDrawer();
				personfollowed = String.Empty;
			};
			bot.RegisteredCommands.Add("abort", AbortCommand);
			
			CommandDelegate SpeedCommand = delegate(string Line)
			{
				string[] full = bot.GetMessage(Line).Split(new char[] {' '}, 2);
				bot.CuboidSleepTime = 1000 / Int32.Parse(full[1]);
			};
			bot.RegisteredCommands.Add("speed", SpeedCommand);

			CommandDelegate EllipsoidCommand = delegate(string Line)
			{
				Ellipsoid ellipsoid = new Ellipsoid();
				bot.SetDrawer(Line, ellipsoid, 2);
			};
			bot.RegisteredCommands.Add("ellipsoid", EllipsoidCommand);

			CommandDelegate CuboidHCommand = delegate(string Line)
			{
				CuboidHollow cuboidh = new CuboidHollow();
				bot.SetDrawer(Line, cuboidh, 2);
			};
			bot.RegisteredCommands.Add("cuboidh", CuboidHCommand);

			CommandDelegate CuboidWCommand = delegate(string Line)
			{
				CuboidWireframe cuboidw = new CuboidWireframe();
				bot.SetDrawer(Line, cuboidw, 2);
			};
			bot.RegisteredCommands.Add("cuboidw", CuboidWCommand);
			
			CommandDelegate LineCommand = delegate(string Line)
			{
				Line line = new Line();
				bot.SetDrawer(Line, line, 2);
			};
			bot.RegisteredCommands.Add("line", LineCommand);
			#endregion			
		}
		
		void ChatMessage(object sender, MessageEventArgs e)
		{
			AppendLog("&fBot: "+e.Line+"&");
		}
		
		void BotException(object sender, BotExceptionEventArgs e)
		{
			RichTextBoxExtensions.AppendText(logBox, e.Output, Color.Red);
			RichTextBoxExtensions.AppendText(logBox, "\r\n", Color.White);
		}		

		void GotKicked(object sender, KickedEventArgs e)
		{
			RichTextBoxExtensions.AppendText(logBox, "Reconnecting: " + e.WillReconnect + ". " + e.Reason, Color.Red);
			RichTextBoxExtensions.AppendText(logBox, "\r\n", Color.White);
		}
		
		void MapLoading(object sender, MapProgressEventArgs e)
		{
			UpdateProgressBar(e.PercentDone);
			MainTextCallback mtc = new MainForm.MainTextCallback(MainTextUpdate);
			string update = "LibClassicBotGUI - Loading map.. "+ e.PercentDone.ToString();
			this.Invoke(mtc, new object[] { update } );			
		}
		
		void BtnMessageClick(object sender, System.EventArgs e)
		{
			if(!String.IsNullOrEmpty(txtMessage.Text) && StatBot != null)
				StatBot.SendMessagePacket(txtMessage.Text);
		}

		void MapLoaded(object sender, MapLoadedEventArgs e)
		{
			MainTextCallback mtc = new MainForm.MainTextCallback(MainTextUpdate);
			string update = "LibClassicBotGUI - Loaded map.";
			this.Invoke(mtc, new object[] { update } );	
		}		
	}
	//Based on http://stackoverflow.com/questions/1926264/color-different-parts-of-a-richtextbox-string
	public static class RichTextBoxExtensions
	{
		
		private delegate void AppendCallback(RichTextBox box, string text, Color color);
		
		private static void AppendTextInternal(RichTextBox box, string text, Color color)
		{
			box.SelectionStart = box.TextLength;
			box.SelectionLength = 0;
			box.SelectionColor = color;
			box.AppendText(text);
			box.SelectionColor = box.ForeColor;
		}
		
		public static void AppendText(RichTextBox box, string text, Color color)
		{
			AppendCallback d = new AppendCallback(AppendTextInternal);
			box.Invoke(d, new object[] { box, text, color } );
			
		}
	}
}
