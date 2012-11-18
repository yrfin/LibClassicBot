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

		public void HandleCuboid(string s) //TODO: Use better handling of chat.
		{
			string[] message = s.Split(':');
			wantingPositionOne = true;
			string[] split = message[1].Split(' ');
			try
			{
				switch (split[2].ToLower())
				{
						case "air": cuboidType = 0; break;
						case "stone": cuboidType = 1; break;
						case "grass": cuboidType = 2; break;
						case "dirt": cuboidType = 3; break;
						case "cobble": cuboidType = 4; break;
						case "cobblestone": cuboidType = 4; break;
						case "woodenplank": cuboidType = 5; break;
						case "plank": cuboidType = 5; break;
						case "planks": cuboidType = 5; break;
						case "plant": cuboidType = 6; break;
						case "sapling": cuboidType = 6; break;
						case "bedrock": cuboidType = 7; break;
						case "water": cuboidType = 9; break;
						case "lava": cuboidType = 11; break;
						case "sand": cuboidType = 11; break;
						case "gravel": cuboidType = 13; break;
						case "goldore": cuboidType = 14; break;
						case "ironore": cuboidType = 15; break;
						case "coalore": cuboidType = 16; break;
						case "coal": cuboidType = 16; break;
						case "log": cuboidType = 17; break;
						case "wood": cuboidType = 17; break;
						case "leaves": cuboidType = 18; break;
						case "sponge": cuboidType = 19; break;
						case "glass": cuboidType = 20; break;
						case "red": cuboidType = 21; break;
						case "orange": cuboidType = 22; break;
						case "yellow": cuboidType = 23; break;
						case "lime": cuboidType = 24; break;
						case "green": cuboidType = 25; break;
						case "teal": cuboidType = 26; break;
						case "aqua": cuboidType = 26; break;
						case "cyan": cuboidType = 27; break;
						case "blue": cuboidType = 28; break;
						case "purple": cuboidType = 29; break;
						case "indigo": cuboidType = 30; break;
						case "violet": cuboidType = 31; break;
						case "magenta": cuboidType = 32; break;
						case "pink": cuboidType = 33; break;
						case "black": cuboidType = 34; break;
						case "grey": cuboidType = 35; break;
						case "gray": cuboidType = 35; break;
						case "white": cuboidType = 36; break;
						case "dandelion": cuboidType = 37; break;
						case "yellowflower": cuboidType = 37; break;
						case "rose": cuboidType = 38; break;
						case "redflower": cuboidType = 38; break;
						case "brownmushroom": cuboidType = 39; break;
						case "redmushroom": cuboidType = 40; break;
						case "gold": cuboidType = 41; break;
						case "iron": cuboidType = 42; break;
						case "doublestair": cuboidType = 43; break;
						case "doubleslab": cuboidType = 43; break;
						case "stair": cuboidType = 44; break;
						case "brick": cuboidType = 45; break;
						case "tnt": cuboidType = 46; break;
						case "books": cuboidType = 47; break;
						case "bookshelf": cuboidType = 47; break;
						case "mossy": cuboidType = 48; break;
						case "mossycobblestone": cuboidType = 48; break;
						case "mossystone": cuboidType = 48; break;
						case "obsidian": cuboidType = 49; break;
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