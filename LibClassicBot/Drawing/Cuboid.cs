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
		const string _name = "Cuboid";		
		
		/// <summary>
		/// Executes the drawer on a separate thread. The bool is there if the user finds a need to stop the drawing. 
		/// (This happens when CancelDrawer() is called.)
		/// </summary>
		public void Start(ClassicBot main, ref bool Aborted, Vector3I[] points, byte blocktype, ref int sleeptime)
		{
			Vector3I Coords = Vector3I.Min(points[0], points[1]);
			Vector3I MinVertex = Vector3I.Min(points[0], points[1]);
			Vector3I MaxVertex = Vector3I.Max(points[0], points[1]);
			
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
