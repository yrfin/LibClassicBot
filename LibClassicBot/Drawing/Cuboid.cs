using System;
using System.Threading;
using LibClassicBot;
namespace LibClassicBot.Drawing
{
	/// <summary>
	/// Fills a rectangular area with the specified block type.
	/// </summary>
	public sealed class Cuboid : IDrawer
	{	
		/// <summary>The lower point of the drawing area.</summary>
		public Vector3I Point1
		{
			get { return _point1; }
			set { _point1 = value; }
		}	
		
		/// <summary>The upper point of the drawing area.</summary>
		public Vector3I Point2
		{
			get { return _point2; }
			set { _point2 = value; }
		}
		
		string _name = "Cuboid";
		
		Vector3I _point2;
		Vector3I _point1;
		
		/// <summary>
		/// Executes the drawer, and should be executed on a separate thread.
		/// The token is there if the user finds a need to stop the drawing. (This happens when CriticalAbort is set to true.)
		/// </summary>
		/// <param name="cToken">The CuboidToken to check if the drawing needs to be stopped.</param>
		public void Start(ClassicBot main, ref bool Aborted, Vector3I point1, Vector3I point2, byte blocktype, ref int sleeptime)
		{
			Vector3I Coords = Vector3I.Min(point1, point2);
			Vector3I MinVertex = Vector3I.Min(point1, point2);
			Vector3I MaxVertex = Vector3I.Max(point1, point2);
			
			main.SendPositionPacket((short)Coords.X, (short)Coords.Y, (short)Coords.Z);
			for (; Coords.X <= MaxVertex.X; Coords.X++)
			{
				for (; Coords.Y <=  MaxVertex.Y; Coords.Y++)
				{
					for (; Coords.Z <=  MaxVertex.Z; Coords.Z++)
					{
						if (Aborted == true)
						{
							main.SendMessagePacket("Aborted cuboid.");
							return;
						}
						Thread.Sleep(sleeptime);
						main.SendPositionPacket((short)Coords.X, (short)Coords.Y, (short)Coords.Z);
						main.SendBlockPacket((short)Coords.X, (short)Coords.Y, (short)Coords.Z, 1, blocktype);
					}
					Coords.Z = MinVertex.Z; //Reset height.
				}
				Coords.Y = MinVertex.Y;
			}
		}
		
		/// <summary>
		/// Gets the name of the current drawing command.
		/// </summary>
		public string Name 
		{
			get { return _name; }
		}
	}
}
