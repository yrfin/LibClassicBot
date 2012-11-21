/*Copyright 2009-2012 Matvei Stefarov <me@matvei.org>

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.*/
using System.Collections.Generic;
using System.Threading;

namespace LibClassicBot.Drawing
{
	/// <summary> Draw operation that creates a wireframe cuboid, optionally filling sides and center. </summary>
	public class Ellipsoid : IDrawer
	{
		struct Vector3F {
			public float X; public float Y; public float Z;
			public float X2 { get { return X * 2; } } public float Y2 { get { return Y * 2; } }
			public float Z2 { get { return Z * 2; } }
		}

		/// <summary>
		/// Gets the name of the current drawing command.
		/// </summary>
		public string Name
		{
			get { return _name; }
		}

		const string _name = "Ellipsoid";
		Vector3F radius, center;

		/// <summary>
		/// Executes the drawer on a separate thread. The bool is there if the user finds a need to stop the drawing.
		/// (This happens when CancelDrawer() is called.)
		/// </summary>
		public void Start(ClassicBot main, ref bool Aborted, Vector3I[] points, byte blocktype, ref int sleeptime)
		{
			Vector3I Coords = Vector3I.Min(points[0], points[1]);
			Vector3I MinVertex = Vector3I.Min(points[0], points[1]);
			Vector3I MaxVertex = Vector3I.Max(points[0], points[1]);
			double rx = (MaxVertex.X - MinVertex.X + 1) / 2d;
			double ry = (MaxVertex.Y - MinVertex.Y + 1) / 2d;
			double rz = (MaxVertex.Z - MinVertex.Z + 1) / 2d;

			radius.X = (float)(1 / (rx * rx));
			radius.Y = (float)(1 / (ry * ry));
			radius.Z = (float)(1 / (rz * rz));

			// find center points
			center.X = (float)((MinVertex.X + MaxVertex.X) / 2d);
			center.Y = (float)((MinVertex.Y + MaxVertex.Y) / 2d);
			center.Z = (float)((MinVertex.Z + MaxVertex.Z) / 2d);

			Coords = MinVertex;
			main.SendPositionPacket((short)Coords.X, (short)Coords.Y, (short)Coords.Z);
			IEnumerator<Vector3I> coordEnumerator = BlockEnumerator(MinVertex, MaxVertex).GetEnumerator();
			while(coordEnumerator.MoveNext())
			{
				if (Aborted == true)
				{
					return;
				}
				Thread.Sleep(sleeptime);
				Coords = coordEnumerator.Current;
				main.SendPositionPacket((short)Coords.X, (short)Coords.Y, (short)Coords.Z);
				main.SendBlockPacket((short)Coords.X, (short)Coords.Y, (short)Coords.Z, 1, blocktype);
				
			}
			main.SetDrawerToNnull();
		}

		IEnumerable<Vector3I> BlockEnumerator(Vector3I min, Vector3I max)
		{
			for( int x = min.X; x <= max.X; x++ ) {
				for( int y = min.Y; y <= max.Y; y++ ) {
					for( int z = min.Z; z <= max.Z; z++ ) {
						double dx = (x - center.X);
						double dy = (y - center.Y);
						double dz = (z - center.Z);

						// test if it's inside ellipse
						if( (dx * dx) * radius.X + (dy * dy) * radius.Y + (dz * dz) * radius.Z <= 1 ) {
							yield return new Vector3I( x, y, z );
						}
					}
				}
			}
		}

	}
}