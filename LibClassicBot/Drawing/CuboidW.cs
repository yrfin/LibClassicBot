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
using ClassicBotCore;
using ClassicBotCore.Drawing;
using System.Threading;

namespace ClassicBotCore.Drawing
{
	public sealed class CuboidWireframe : IDrawer
	{
		/// <summary>
		/// Gets the name of the current drawing command.
		/// </summary>
		public string Name
		{
			get { return _name; }
		}
		string _name = "CuboidWireframe";

		Vector3I _point1;
		/// <summary>The lower point of the drawing area.</summary>
		public Vector3I Point1
		{
			get { return _point1; }
			set { _point1 = value; }
		}

		Vector3I _point2;
		/// <summary>The upper point of the drawing area.</summary>
		public Vector3I Point2
		{
			get { return _point2; }
			set { _point2 = value; }
		}

		/// <summary>
		/// Executes the drawer, and should be executed on a separate thread.
		/// The token is there if the user finds a need to stop the drawing. (This happens when CriticalAbort is set to true.)
		/// </summary>
		/// <param name="cToken">The CuboidToken to check if the drawing needs to be stopped.</param>
		public void Start(ClassicBot main, ref CancelDrawingToken cToken, Vector3I point1, Vector3I point2, byte blocktype, ref int sleeptime)
		{
			_drawingToken = cToken;
			Point1 = Vector3I.Min(point1, point2);
			Point2 = Vector3I.Max(point1, point2);
			Vector3I Coords = Vector3I.Min(point1, point2);
			
			main.SendPositionPacket((short)Coords.X, (short)Coords.Y, (short)Coords.Z);
			coordEnumerator = BlockEnumerator().GetEnumerator();
			while (coordEnumerator.MoveNext())
			{
				Coords = coordEnumerator.Current;
				Thread.Sleep(sleeptime);
				main.SendPositionPacket((short)Coords.X, (short)Coords.Y, (short)Coords.Z);
				main.SendBlockPacket((short)Coords.X, (short)Coords.Y, (short)Coords.Z, 1, blocktype);
			}
		}

		IEnumerator<Vector3I> coordEnumerator;

		IEnumerable<Vector3I> BlockEnumerator()
		{
			// Draw cuboid vertices
			yield return new Vector3I(Point1.X, Point1.Y, Point1.Z);

			if (Point1.X != Point2.X) yield return new Vector3I(Point2.X, Point1.Y, Point1.Z);
			if (Point1.Y != Point2.Y) yield return new Vector3I(Point1.X, Point2.Y, Point1.Z);
			if (Point1.Z != Point2.Z) yield return new Vector3I(Point1.X, Point1.Y, Point2.Z);

			if (Point1.X != Point2.X && Point1.Y != Point2.Y)
				yield return new Vector3I(Point2.X, Point2.Y, Point1.Z);
			if (Point1.Y != Point2.Y && Point1.Z != Point2.Z)
				yield return new Vector3I(Point1.X, Point2.Y, Point2.Z);
			if (Point1.Z != Point2.Z && Point1.X != Point2.X)
				yield return new Vector3I(Point2.X, Point1.Y, Point2.Z);

			if (Point1.X != Point2.X && Point1.Y != Point2.Y && Point1.Z != Point2.Z)
				yield return new Vector3I(Point2.X, Point2.Y, Point2.Z);

			// Draw edges along the X axis
			if ((Point2.X - Point1.X + 1) > 2)
			{
				for (int x = Point1.X + 1; x < Point2.X; x++)
				{
					yield return new Vector3I(x, Point1.Y, Point1.Z);
					if (Point1.Z != Point2.Z) yield return new Vector3I(x, Point1.Y, Point2.Z);
					if (Point1.Y != Point2.Y)
					{
						yield return new Vector3I(x, Point2.Y, Point1.Z);
						if (Point1.Z != Point2.Z) yield return new Vector3I(x, Point2.Y, Point2.Z);
					}
				}
			}

			// Draw edges along the Y axis
			if ((Point2.Y - Point1.Y + 1) > 2)
			{
				for (int y = Point1.Y + 1; y < Point2.Y; y++)
				{
					yield return new Vector3I(Point1.X, y, Point1.Z);
					if (Point1.Z != Point2.Z) yield return new Vector3I(Point1.X, y, Point2.Z);
					if (Point1.X != Point2.X)
					{
						yield return new Vector3I(Point2.X, y, Point1.Z);
						if (Point1.Z != Point2.Z) yield return new Vector3I(Point2.X, y, Point2.Z);
					}
				}
			}

			// Draw edges along the Z axis
			if ((Point2.Z - Point1.Z + 1) > 2)
			{
				for (int z = Point1.Z + 1; z < Point2.Z; z++)
				{
					yield return new Vector3I(Point1.X, Point1.Y, z);
					if (Point1.Y != Point2.Y) yield return new Vector3I(Point1.X, Point2.Y, z);
					if (Point1.X != Point2.X)
					{
						yield return new Vector3I(Point2.X, Point2.Y, z);
						if (Point1.Y != Point2.Y) yield return new Vector3I(Point2.X, Point1.Y, z);
					}
				}
			}
		}
		
		private CancelDrawingToken _drawingToken;
		
		/// <summary>
		/// The token with which the currently running draw operation can be cancelled with.
		/// </summary>
		public CancelDrawingToken DrawingToken
		{
			get { return _drawingToken; }
			set { _drawingToken = value; }
		}
	}
}