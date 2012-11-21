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
using System;
using System.Collections.Generic;
using LibClassicBot;
using LibClassicBot.Drawing;
using System.Threading;

namespace LibClassicBot.Drawing
{
	public sealed class Line : IDrawer
	{
		/// <summary>
		/// Gets the name of the current drawing command.
		/// </summary>
		public string Name
		{
			get { return _name; }
		}
		string _name = "Line";

		/// <summary>
		/// Executes the drawer, and should be executed on a separate thread.
		/// The token is there if the user finds a need to stop the drawing. (This happens when CriticalAbort is set to true.)
		/// </summary>
		/// <param name="cToken">The CuboidToken to check if the drawing needs to be stopped.</param>
		public void Start(ClassicBot main, ref bool Aborted, Vector3I[] points, byte blocktype, ref int sleeptime)
		{
			Vector3I Coords = Vector3I.Min(points[0], points[1]);
			
			main.SendPositionPacket((short)Coords.X, (short)Coords.Y, (short)Coords.Z);
			coordEnumerator = LineEnumerator(points[0], points[1]).GetEnumerator();
			while (coordEnumerator.MoveNext())
			{
				if (Aborted == true)
				{
					return;
				}
				Coords = coordEnumerator.Current;
				Thread.Sleep(sleeptime);
				main.SendPositionPacket((short)Coords.X, (short)Coords.Y, (short)Coords.Z);
				main.SendBlockPacket((short)Coords.X, (short)Coords.Y, (short)Coords.Z, 1, blocktype);
			}
			main.SetDrawerToNnull();
		}

		IEnumerator<Vector3I> coordEnumerator;

		// Contributed by Conrad "Redshift" Morgan
		IEnumerable<Vector3I> LineEnumerator( Vector3I a, Vector3I b ) {
			Vector3I pixel = a;
			Vector3I d = b - a;
			Vector3I inc = new Vector3I( Math.Sign( d.X ),Math.Sign( d.Y ),Math.Sign( d.Z ) );
			d = Vector3I.Abs(d);
			Vector3I d2 = d * 2;

			int x, y, z;
			if( (d.X >= d.Y) && (d.X >= d.Z) ) {
				x = 0; y = 1; z = 2;
			} else if( (d.Y >= d.X) && (d.Y >= d.Z) ) {
				x = 1; y = 2; z = 0;
			} else {
				x = 2; y = 0; z = 1;
			}

			int err1 = d2[y] - d[x];
			int err2 = d2[z] - d[x];
			for( int i = 0; i < d[x]; i++ ) {
				yield return pixel;
				if( err1 > 0 ) {
					pixel[y] += inc[y];
					err1 -= d2[x];
				}
				if( err2 > 0 ) {
					pixel[z] += inc[z];
					err2 -= d2[x];
				}
				err1 += d2[y];
				err2 += d2[z];
				pixel[x] += inc[x];
			}

			yield return b;
		}
	}
}