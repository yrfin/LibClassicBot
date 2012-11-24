/* DrawImg is based on code from 800Craft. The original copyright notice is reproduced here:
 * Copyright (C) <2012> <Jon Baker, Glenn Mariën and Lao Tszy>
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.*/
using System;
using System.Drawing;
using System.Threading;

namespace LibClassicBot.Drawing
{
	public class DrawImage : IDrawer
	{
		/// <summary>
		/// Gets the name of the current drawing command.
		/// </summary>
		public string Name {
			get { return _name; }
		}
		
		const string _name = "DrawImg";
		
		public string originalchatline = String.Empty;

		/// <summary>
		/// Gets the direction of the two marks, from the first mark.
		/// </summary>
		/// <param name="marks">The two marks to check.</param>
		/// <returns>A byte indicating the direction. 0 indicates West,
		/// 1 indicates East, 2 indicates North, 3 indicates South, 4 indicates error.</returns>
		public static byte GetDirection(Vector3I[] marks)
		{
			if (Math.Abs(marks[1].X - marks[0].X) > Math.Abs(marks[1].Y - marks[0].Y)) {
				if (marks[0].X < marks[1].X) { return 0; }
				else { return 1; }
			} else if (Math.Abs(marks[1].X - marks[0].X) < Math.Abs(marks[1].Y - marks[0].Y)) {
				if (marks[0].Y < marks[1].Y) { return 2; }
				else { return 3; }
			} else if (Math.Abs(marks[1].X - marks[0].X) == Math.Abs(marks[1].Y - marks[0].Y)) {
				if(Math.Sign(marks[1].X) == 1 || Math.Sign(marks[0].X) ==1) { return 0; }
				else { return 1; }
			} else { return 4; }
		}
		
		/// <summary>
		/// Executes the drawer on a separate thread. The bool is there if the user finds a need to stop the drawing.
		/// (This happens when CancelDrawer() is called.)
		/// </summary>
		public void Start(ClassicBot main, ref bool Aborted, Vector3I[] points, byte blocktype, ref int sleeptime)
		{
			Vector3I firstPoint = points[0];
			Bitmap myBitmap = null;
			string[] full = main.GetMessage(originalchatline).Trim().Split(new char[] {' '}, 3);
			try { myBitmap = new Bitmap(full[2]); } catch { return; } //Invalid file, stop drawing.
			myBitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
			byte direction = GetDirection(points);
			if (direction == 4) return;

			Block[] refCols = GetReferenceColours();
			Block currentBlock;
			double[] distance = new double[refCols.Length];
			int position;
			for ( int curwidth = 0; curwidth < myBitmap.Width; curwidth++ ) {
				for ( int curheight = 0; curheight < myBitmap.Height; curheight++ ) {

					currentBlock.z = (firstPoint.Z + curheight);
					if(direction == 0) {
						currentBlock.x = (firstPoint.X + curwidth);
						currentBlock.y = firstPoint.Y;
					}
					else if ( direction == 1 ) {
						currentBlock.x = (firstPoint.X - curwidth);
						currentBlock.y = firstPoint.Y;
					}
					else if(direction == 2) {
						currentBlock.y = (firstPoint.Y + curwidth);
						currentBlock.x = firstPoint.X;
					}
					else {
						currentBlock.y = (firstPoint.Y - curwidth);
						currentBlock.x = firstPoint.X;
					}
					currentBlock.col = myBitmap.GetPixel(curwidth, curheight);

					for ( int j = 0; j < distance.Length; j++ ) {//Calculate distances between the colors in the image and the set reference colors, and store them.
						distance[j] = Math.Sqrt(Math.Pow((currentBlock.col.R - refCols[j].col.R ), 2) +
						                        Math.Pow(( currentBlock.col.B - refCols[j].col.B ), 2) +
						                        Math.Pow(( currentBlock.col.G - refCols[j].col.G ), 2 ));
					}
					position = 0;
					double minimum = distance[0];
					for ( int h = 1; h < distance.Length; h++ ) {//Find the smallest distance in the array of distances.
						if ( distance[h] < minimum ) {
							minimum = distance[h];
							position = h;
						}
					}
					currentBlock.blocktype = refCols[position].blocktype;//Set the block we found closest to the image to the block we are placing.
					if ( position <= 20 ) {
						if ( direction == 0 ) { currentBlock.y += 1; }
						else if (direction == 1) { currentBlock.y -= 1;  }
						else if ( direction == 2 ) { currentBlock.x -=1; }
						else if ( direction == 3 ) { currentBlock.x +=1; }
					}
					else
					{
						Console.WriteLine("This is most unexpected.");
					}
					if ( currentBlock.col.A < 20 ) currentBlock.blocktype = 0; //Transparent pixels.
					if (Aborted == true) {
						return;
					}
					Thread.Sleep(sleeptime);
					main.SendPositionPacket((short)currentBlock.x, (short)currentBlock.y, (short)currentBlock.z);
					main.SendBlockPacket((short)currentBlock.x, (short)currentBlock.y, (short)currentBlock.z, 1, currentBlock.blocktype);
				}
			}
			myBitmap.Dispose();
			main.SetDrawerToNull();
		}

		static Block[] GetReferenceColours() {
			Block tempref = new Block();
			Block[] refCol = new Block[40];
			//Front layer blocks.
			tempref.col = Color.FromArgb(255, 128, 86, 57);
			tempref.blocktype = 3;
			refCol[0] = tempref;
			tempref.col = Color.FromArgb(255, 162, 129, 75);
			tempref.blocktype = 5;
			refCol[1] = tempref;
			tempref.col = Color.FromArgb(255, 244, 237, 174);
			tempref.blocktype = 12;
			refCol[2] = tempref;
			tempref.col = Color.FromArgb(255, 226, 31, 38);
			tempref.blocktype = 21;
			refCol[3] = tempref;
			tempref.col = Color.FromArgb(255, 223, 135, 37);
			tempref.blocktype = 22;
			refCol[4] = tempref;
			tempref.col = Color.FromArgb(255, 230, 241, 25);
			tempref.blocktype = 23;
			refCol[5] = tempref;
			tempref.col = Color.FromArgb(255, 127, 234, 26);
			tempref.blocktype = 24;
			refCol[6] = tempref;
			tempref.col = Color.FromArgb(255, 25, 234, 20);
			tempref.blocktype = 25;
			refCol[7] = tempref;
			tempref.col = Color.FromArgb(255, 31, 234, 122);
			tempref.blocktype = 26;
			refCol[8] = tempref;
			tempref.col = Color.FromArgb(255, 27, 239, 225);
			tempref.blocktype = 27;
			refCol[9] = tempref;
			tempref.col = Color.FromArgb(255, 99, 166, 226);
			tempref.blocktype = 28;
			refCol[10] = tempref;
			tempref.col = Color.FromArgb(255, 111, 124, 235);
			tempref.blocktype = 29;
			refCol[11] = tempref;
			tempref.col = Color.FromArgb(255, 126, 34, 218);
			tempref.blocktype = 30;
			refCol[12] = tempref;
			tempref.col = Color.FromArgb(255, 170, 71, 219);
			tempref.blocktype = 31;
			refCol[13] = tempref;
			tempref.col = Color.FromArgb(255, 227, 39, 225);
			tempref.blocktype = 32;
			refCol[14] = tempref;
			tempref.col = Color.FromArgb(255, 234, 39, 121);
			tempref.blocktype = 33;
			refCol[15] = tempref;
			tempref.col = Color.FromArgb(255, 46, 68, 47);
			tempref.blocktype = 34;
			refCol[16] = tempref;
			tempref.col =  Color.FromArgb(255, 135, 145, 130);
			tempref.blocktype = 35;
			refCol[17] = tempref;
			tempref.col = Color.FromArgb(255, 230, 240, 225);
			tempref.blocktype = 36;
			refCol[18] = tempref;
			
			//Back layer blocks.
			tempref.col = Color.FromArgb(255, 57, 38, 25);
			tempref.blocktype = 3;
			refCol[19] = tempref;
			tempref.col = Color.FromArgb(255, 72, 57, 33);
			tempref.blocktype = 5;
			refCol[20] = tempref;
			tempref.col = Color.FromArgb(255, 109, 105, 77);
			tempref.blocktype = 12;
			refCol[21] = tempref;
			tempref.col = Color.FromArgb(255, 41, 31, 16);
			tempref.blocktype = 17;
			refCol[22] = tempref;
			tempref.col = Color.FromArgb(255, 101, 13, 16);
			tempref.blocktype = 21;
			refCol[23] = tempref;
			tempref.col = Color.FromArgb(255, 99, 60, 16);
			tempref.blocktype = 22;
			refCol[24] = tempref;
			tempref.col = Color.FromArgb(255, 102, 107, 11);
			tempref.blocktype = 23;
			refCol[25] = tempref;
			tempref.col = Color.FromArgb(255, 56, 104, 11);
			tempref.blocktype = 24;
			refCol[26] = tempref;
			tempref.col = Color.FromArgb(255, 11, 104, 8);
			tempref.blocktype = 25;
			refCol[27] = tempref;
			tempref.col = Color.FromArgb(255, 13, 104, 54);
			tempref.blocktype = 26;
			refCol[28] = tempref;
			tempref.col = Color.FromArgb(255, 12, 106, 100);
			tempref.blocktype = 27;
			refCol[29] = tempref;
			tempref.col = Color.FromArgb(255, 44, 74, 101);
			tempref.blocktype = 28;
			refCol[30] = tempref;
			tempref.col = Color.FromArgb(255, 49, 55, 105);
			tempref.blocktype = 29;
			refCol[31] = tempref;
			tempref.col = Color.FromArgb(255, 56, 15, 97);
			tempref.blocktype = 30;
			refCol[32] = tempref;
			tempref.col = Color.FromArgb(255, 75, 31, 97);
			tempref.blocktype = 31;
			refCol[33] = tempref;
			tempref.col = Color.FromArgb(255, 101, 17, 100);
			tempref.blocktype = 32;
			refCol[34] = tempref;
			tempref.col = Color.FromArgb(255, 104, 17, 54);
			tempref.blocktype = 33;
			refCol[35] = tempref;
			tempref.col = Color.FromArgb(255, 20, 30, 21);
			tempref.blocktype = 34;
			refCol[36] = tempref;
			tempref.col = Color.FromArgb(255, 60, 64, 58);
			tempref.blocktype = 35;
			refCol[37] = tempref;
			tempref.col = Color.FromArgb(255, 102, 107, 100);
			tempref.blocktype = 36;
			refCol[38] = tempref;
			tempref.col = Color.FromArgb(255, 0, 0, 0);
			tempref.blocktype = 49;
			refCol[39] = tempref;
			return refCol;
		}
		struct Block { public int x, y, z; public byte blocktype; public Color col; }
	}
}