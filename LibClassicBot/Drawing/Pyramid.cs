using System.Threading;

namespace LibClassicBot.Drawing
{
	/// <summary>
	/// Fills an area with a pyramid, going upwards.
	/// </summary>
	public sealed class Pyramid : IDrawer
	{
		/// <summary>
		/// Gets the name of the current drawing command.
		/// </summary>
		public string Name {
			get { return _name; }
		}		
		
		const string _name = "Pyramid";
		
		/// <summary>
		/// Executes the drawer, and should be executed on a separate thread.
		/// The token is there if the user finds a need to stop the drawing. (This happens when CriticalAbort is set to true.)
		/// </summary>
		/// <param name="cToken">The CuboidToken to check if the drawing needs to be stopped.</param>
		public void Start(ClassicBot main, ref bool Aborted, Vector3I[] points, byte blocktype, ref int sleeptime)
		{
			Vector3I Coords = Vector3I.Min(points[0], points[1]);
			Vector3I MinVertex = Vector3I.Min(points[0], points[1]);
			Vector3I MaxVertex = Vector3I.Max(points[0], points[1]);
			Vector3I Coords2 = Vector3I.Max(points[0], points[1]);
			Coords2.Z = MinVertex.Z;
			Vector3I Coords3 = Coords;
			int xMax = (MinVertex.X + MaxVertex.X / 2);
			int yMax = (MinVertex.Y + MaxVertex.Y / 2);
			main.SendPositionPacket((short)Coords.X, (short)Coords.Y, (short)Coords.Z);

			while (Coords3.X <= xMax && Coords3.Y <= yMax) //It's not pretty, but it works.
			{
				Coords = Coords3;
				for (; Coords.X <= Coords2.X; Coords.X++)
				{
					for (; Coords.Y <= Coords2.Y; Coords.Y++)
					{
						for (; Coords.Z <= Coords2.Z; Coords.Z++)
						{
							if (Aborted == true) {
								return;
							}
							Thread.Sleep(sleeptime);
							main.SendPositionPacket((short)Coords.X, (short)Coords.Y, (short)Coords.Z);
							main.SendBlockPacket((short)Coords.X, (short)Coords.Y, (short)Coords.Z, 1, blocktype);
						}
						Coords.Z = Coords3.Z; //Reset height.
					}
					Coords.Y = Coords3.Y;
				}
				Coords3.X++; 
				Coords3.Y++;
				Coords3.Z++;
				Coords2.X--;
				Coords2.Y--;
				Coords2.Z++; //All these are needed, maybe some day I'll improve this.
			}
			main.SetDrawerToNull();
		}
	}
}