using System;
using System.Collections.Generic;
using System.Threading;

using LibClassicBot.Drawing;

namespace LibClassicBot
{
	public partial class ClassicBot
	{
		/// <summary>
		/// The queue which contains the list of drawing commands to be executed. The drawing will be executed  after the bot has been given two accepted block inputs.
		/// (By default, this will be brown mushrooms.) Note that on fCraft servers, the server will NOT send block updates if you are too far away from the bot.
		/// </summary>
		public Queue<IDrawer> QueuedDrawers = new Queue<IDrawer>();
		
		/// <summary>
		/// Begins execution of the draw operation between two points.
		/// </summary>
		/// <param name="drawer">The class derived from IDrawer, from which the draw operation will begin.</param>
		/// <remarks>Note that if the CancelDrawing token is currently in the cancelled state, it will be reset.</remarks>
		public void Draw(IDrawer drawer, Vector3I point1, Vector3I point2, byte blockType)
		{
			drawingAborted = false;
			Thread drawThread = new Thread(delegate()
			                               {
			                               	drawer.Start(this, ref drawingAborted, point1, point2, blockType, ref sleepTime);
			                               });
			drawThread.IsBackground = true;
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
			drawingAborted = true;
		}

		/// <summary>
		/// Gets the block type from the chatline, using pre-determined values.
		/// </summary>
		/// <param name="s">The chatline to parse.</param>
		public void GetFromLine(string rawLine) //TODO: Use better handling of chat.
		{
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
							SendMessagePacket("Unknown block type " + split[2]);
							wantingPositionOne = false;
							return;
						}
				}
			}
			catch (IndexOutOfRangeException)
			{
				SendMessagePacket("Error: Wrong number of arguements.");
				return;
			}
			SendMessagePacket("Place two brown mushrooms to start cuboiding.");
		}
	}
}