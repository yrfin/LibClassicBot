using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;

using LibClassicBot.Drawing;

namespace LibClassicBot
{
	public partial class ClassicBot
	{
		/// <summary>
		/// The internally stored draw operation. After a draw operation has been set and executed, this
		/// will be set back to null.
		/// </summary>
		private IDrawer QueuedDrawer = null;
		
		/// <summary>
		/// The array which contains the marks to draw with.
		/// </summary>
		private Vector3I[] marks;
		
		/// <summary>
		/// The number of marks that still need to be given before the draw operation can be executed.
		/// </summary>
		private byte marksLeft = 0;
		
		/// <summary>
		/// Regex used for the trimming of prefixes from the names of users.
		/// </summary>
		/// <example><code>
		/// string trimmed = Regex.Replace("[2] Test",@"[\(\[][\w\s]+[\)\]]","").Trim();
		/// </code></example>
		static Regex prefixRegex = new Regex(@"[\(\[<][\w\s]+[\)\]>]", RegexOptions.None);
		
		/// <summary>
		/// The ID of the player who cuboided, used for the checking if blocks placed 
		/// are close enough to the player. null means that no player was found.
		/// </summary>
		byte? CubID;
		
		/// <summary>
		/// Sets the queued draw operation, which will be executed once the all the marks
		/// required have been set. To set a mark, place a brown mushroom on the map the bot is on.
		/// </summary>
		/// <param name="line">The chat line which triggered this draw operation, used for
		/// determining the block type.</param>
		/// <param name="drawer">The drawer to execute once all the marks have been set.</param>
		/// <param name="marksRequired">The number of marks to set before executing the draw operation.</param>
		public void SetDrawer(string chatLine, IDrawer drawer, byte marksRequired)
		{
			if(QueuedDrawer != null) //Cancel the already running draw operation.
			{
				drawingAborted = true;
				QueuedDrawer = null;
			}
			GetFromLine(GetMessage(chatLine));
			if(cuboidType != 255) //If 255, do not try to set the drawer with an invalid block type.
			{
				CubID = null;
				string user = prefixRegex.Replace(GetUser(chatLine).ToLower(), String.Empty).Trim();
				//Complicated string, but it basically trims all prefixes and spaces.
				foreach( var player in Players ) {
					if (Extensions.StripColors(player.Value.Name).ToLower() == user) {//Lowercase, ignore colour codes.
						CubID = player.Key; //Match found.
						break;
					}
				}
				if(CubID == null) { //Probably had their display name changed.
					SendMessagePacket("Unable to locate a player from the username of the chatline.");
					SendMessagePacket("Are you using the same name as your Minecraft account?");
					return;
				}
				QueuedDrawer = drawer;
				marksLeft = marksRequired;
				marks = new Vector3I[marksRequired];
			}
		}
		/// <summary>
		/// Sets the queued draw operation to null. This should be called after the running draw operation has
		/// been completed, although the bot can work around drawing operations that do not call this.
		/// </summary>
		public void SetDrawerToNull() 
		{
			QueuedDrawer = null;
		}
		
		/// <summary>
		/// Begins execution of the draw operation between two points.
		/// </summary>
		/// <param name="drawer">The class derived from IDrawer, from which the draw operation will begin.</param>		
		/// <exception cref="OutOfMemoryException">There was not enough mmemory left to start
		/// a new thread for drawing on. (This should almost never happen.)</exception>
		/// <remarks>Note that if the drawingAborted is true, it will be set to false.</remarks>
		public void Draw(IDrawer drawer, Vector3I[] points, byte blockType)
		{
			drawingAborted = false;
			Thread drawThread = new Thread(delegate()
			                               {
			                               	drawer.Start(this, ref drawingAborted, points, blockType, ref sleepTime);
			                               });
			drawThread.IsBackground = true;
			drawThread.Name = "Drawing thread.";
			drawThread.Start();
		}
		
		/// <summary>
		/// Gets the current state of the boolean determining if the currently running draw operation should be aborted or not.
		/// To abort the currently running draw operation, use CancelDrawer();
		/// </summary>
		public bool DrawingAborted {
			get { return drawingAborted; }
		}
		
		private bool drawingAborted = false;
		
		/// <summary>
		/// Cancels the currently running drawing operation. This does *not* undo the blocks placed by the bot though.
		/// </summary>
		public void CancelDrawer() {
			if(marksLeft != 0) marksLeft = 0;
			drawingAborted = true;
		}

		/// <summary>
		/// Gets the block type from the chatline, using pre-determined values.
		/// </summary>
		/// <param name="rawLine">The chatline to parse.</param>
		public void GetFromLine(string rawLine)
		{
			/*Although this could be done with a static Dictionary, unfortunately .NET 2.0
			  * doesn't support initializing static dictionaries, so the better solution would be 
			  * to either A)Update to 3.5, or B) Initialize later.*/
			string[] split = rawLine.Split(' ');
			try
			{
				if(Byte.TryParse(split[1], out cuboidType)) return; //User used the actual value of a block.
				switch (split[1].ToLower())
				{
						case "air": cuboidType = 0; return;
						case "stone": cuboidType = 1; return;
						case "grass": cuboidType = 2; return;
						case "dirt": cuboidType = 3; return;
						case "cobble": cuboidType = 4; return;
						case "cobblestone": cuboidType = 4; return;
						case "woodenplank": cuboidType = 5; return;
						case "plank": cuboidType = 5; return;
						case "planks": cuboidType = 5; return;
						case "plant": cuboidType = 6; return;
						case "sapling": cuboidType = 6; return;
						case "bedrock": cuboidType = 7; return;
						case "water": cuboidType = 9; return;
						case "lava": cuboidType = 11; return;
						case "sand": cuboidType = 11; return;
						case "gravel": cuboidType = 13; return;
						case "goldore": cuboidType = 14; return;
						case "ironore": cuboidType = 15; return;
						case "coalore": cuboidType = 16; return;
						case "coal": cuboidType = 16; return;
						case "log": cuboidType = 17; return;
						case "wood": cuboidType = 17; return;
						case "leaves": cuboidType = 18; return;
						case "sponge": cuboidType = 19; return;
						case "glass": cuboidType = 20; return;
						case "red": cuboidType = 21; return;
						case "orange": cuboidType = 22; return;
						case "yellow": cuboidType = 23; return;
						case "lime": cuboidType = 24; return;
						case "green": cuboidType = 25; return;
						case "teal": cuboidType = 26; return;
						case "aqua": cuboidType = 26; return;
						case "cyan": cuboidType = 27; return;
						case "blue": cuboidType = 28; return;
						case "purple": cuboidType = 29; return;
						case "indigo": cuboidType = 30; return;
						case "violet": cuboidType = 31; return;
						case "magenta": cuboidType = 32; return;
						case "pink": cuboidType = 33; return;
						case "black": cuboidType = 34; return;
						case "grey": cuboidType = 35; return;
						case "gray": cuboidType = 35; return;
						case "white": cuboidType = 36; return;
						case "dandelion": cuboidType = 37; return;
						case "yellowflower": cuboidType = 37; return;
						case "rose": cuboidType = 38; return;
						case "redflower": cuboidType = 38; return;
						case "brownmushroom": cuboidType = 39; return;
						case "redmushroom": cuboidType = 40; return;
						case "gold": cuboidType = 41; return;
						case "iron": cuboidType = 42; return;
						case "doublestair": cuboidType = 43; return;
						case "doubleslab": cuboidType = 43; return;
						case "stair": cuboidType = 44; return;
						case "brick": cuboidType = 45; return;
						case "tnt": cuboidType = 46; return;
						case "books": cuboidType = 47; return;
						case "bookshelf": cuboidType = 47; return;
						case "mossy": cuboidType = 48; return;
						case "mossycobblestone": cuboidType = 48; return;
						case "mossystone": cuboidType = 48; return;
						case "obsidian": cuboidType = 49; return;
					default:
						{
							cuboidType = 255;
							SendMessagePacket("Unknown block type " + split[2]);
							return;
						}
				}
			}
			catch (IndexOutOfRangeException)
			{
				SendMessagePacket("Error: Wrong number of arguements.");
				return;
			}
		}
	}
}