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
	public sealed class CuboidHollow : IDrawer
	{
		/// <summary>
		/// Gets the name of the current drawing command.
		/// </summary>
		public string Name
		{
			get { return _name; }
		}

		const string _name = "CuboidHollow";

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

		IEnumerable<Vector3I> BlockEnumerator(Vector3I min, Vector3I max) {
			for( int x = min.X; x <= max.X; x++ ) {
				for( int y = min.Y; y <= max.Y; y++ ) {
					yield return new Vector3I( x, y, min.Z );
					if( min.Z != max.Z ) {
						yield return new Vector3I( x, y, max.Z );
					}
				}
			}

			if((max.Z - min.Z + 1) > 2) {
				for( int x = min.X; x <= max.X; x++ ) {
					for( int z = min.Z + 1; z < max.Z; z++ ) {
						yield return new Vector3I( x, min.Y, z );
						if( min.Y != max.Y ) {
							yield return new Vector3I( x, max.Y, z );
						}
					}
				}

				for( int y = min.Y + 1; y < max.Y; y++ ) {
					for( int z = min.Z + 1; z < max.Z; z++ ) {
						yield return new Vector3I( min.X, y, z );
						if( min.X != max.X ) {
							yield return new Vector3I( max.X, y, z );
						}
					}
				}
			}
		}
	}
}