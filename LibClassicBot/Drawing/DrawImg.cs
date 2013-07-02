/* DrawImg is based on code from 800Craft. The original copyright notice is reproduced here:
/*        ----
        Copyright (c) 2011-2013 Jon Baker, Glenn Marien and Lao Tszy <Jonty800@gmail.com>
        All rights reserved.

        Redistribution and use in source and binary forms, with or without
        modification, are permitted provided that the following conditions are met:
         * Redistributions of source code must retain the above copyright
              notice, this list of conditions and the following disclaimer.
            * Redistributions in binary form must reproduce the above copyright
             notice, this list of conditions and the following disclaimer in the
             documentation and/or other materials provided with the distribution.
            * Neither the name of 800Craft or the names of its
             contributors may be used to endorse or promote products derived from this
             software without specific prior written permission.

        THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
        ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
        WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
        DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY
        DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
        (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
        LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
        ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
        (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
        SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
        ----*/
           
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
		
		//Line used for finding the image.
		public string originalchatline = String.Empty;
		struct Block { public int X, Y, Z; public byte blockType; public Color col; }
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
			} else if (Math.Abs(marks[1].X - marks[0].X) == Math.Abs(marks[1].Y - marks[0].Y)) { //Not very accurate.
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

					currentBlock.Z = (firstPoint.Z + curheight);
					if(direction == 0) {
						currentBlock.X = (firstPoint.X + curwidth);
						currentBlock.Y = firstPoint.Y;
					}
					else if ( direction == 1 ) {
						currentBlock.X = (firstPoint.X - curwidth);
						currentBlock.Y = firstPoint.Y;
					}
					else if(direction == 2) {
						currentBlock.Y = (firstPoint.Y + curwidth);
						currentBlock.X = firstPoint.X;
					}
					else {
						currentBlock.Y = (firstPoint.Y - curwidth);
						currentBlock.X = firstPoint.X;
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
					currentBlock.blockType = refCols[position].blockType;//Set the block we found closest to the image to the block we are placing.
					if ( position <= 20 ) {//Back colour blocks
						if ( direction == 0 ) { currentBlock.Y += 1; }
						else if (direction == 1) { currentBlock.Y -= 1;  }
						else if ( direction == 2 ) { currentBlock.X -=1; }
						else if ( direction == 3 ) { currentBlock.X +=1; }
					} //Otherwise, draw them in the front row.
					if ( currentBlock.col.A < 20 ) currentBlock.blockType = 0; //Transparent pixels.
					if (Aborted == true) {
						return;
					}
					Thread.Sleep(sleeptime);
					main.SendPositionPacket((short)currentBlock.X, (short)currentBlock.Y, (short)currentBlock.Z);
					main.SendBlockPacket((short)currentBlock.X, (short)currentBlock.Y, (short)currentBlock.Z, 1, currentBlock.blockType);
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
			tempref.blockType = 3;
			refCol[0] = tempref;
			tempref.col = Color.FromArgb(255, 162, 129, 75);
			tempref.blockType = 5;
			refCol[1] = tempref;
			tempref.col = Color.FromArgb(255, 244, 237, 174);
			tempref.blockType = 12;
			refCol[2] = tempref;
			tempref.col = Color.FromArgb(255, 226, 31, 38);
			tempref.blockType = 21;
			refCol[3] = tempref;
			tempref.col = Color.FromArgb(255, 223, 135, 37);
			tempref.blockType = 22;
			refCol[4] = tempref;
			tempref.col = Color.FromArgb(255, 230, 241, 25);
			tempref.blockType = 23;
			refCol[5] = tempref;
			tempref.col = Color.FromArgb(255, 127, 234, 26);
			tempref.blockType = 24;
			refCol[6] = tempref;
			tempref.col = Color.FromArgb(255, 25, 234, 20);
			tempref.blockType = 25;
			refCol[7] = tempref;
			tempref.col = Color.FromArgb(255, 31, 234, 122);
			tempref.blockType = 26;
			refCol[8] = tempref;
			tempref.col = Color.FromArgb(255, 27, 239, 225);
			tempref.blockType = 27;
			refCol[9] = tempref;
			tempref.col = Color.FromArgb(255, 99, 166, 226);
			tempref.blockType = 28;
			refCol[10] = tempref;
			tempref.col = Color.FromArgb(255, 111, 124, 235);
			tempref.blockType = 29;
			refCol[11] = tempref;
			tempref.col = Color.FromArgb(255, 126, 34, 218);
			tempref.blockType = 30;
			refCol[12] = tempref;
			tempref.col = Color.FromArgb(255, 170, 71, 219);
			tempref.blockType = 31;
			refCol[13] = tempref;
			tempref.col = Color.FromArgb(255, 227, 39, 225);
			tempref.blockType = 32;
			refCol[14] = tempref;
			tempref.col = Color.FromArgb(255, 234, 39, 121);
			tempref.blockType = 33;
			refCol[15] = tempref;
			tempref.col = Color.FromArgb(255, 46, 68, 47);
			tempref.blockType = 34;
			refCol[16] = tempref;
			tempref.col =  Color.FromArgb(255, 135, 145, 130);
			tempref.blockType = 35;
			refCol[17] = tempref;
			tempref.col = Color.FromArgb(255, 230, 240, 225);
			tempref.blockType = 36;
			refCol[18] = tempref;
			
			//Back layer blocks.
			tempref.col = Color.FromArgb(255, 57, 38, 25);
			tempref.blockType = 3;
			refCol[19] = tempref;
			tempref.col = Color.FromArgb(255, 72, 57, 33);
			tempref.blockType = 5;
			refCol[20] = tempref;
			tempref.col = Color.FromArgb(255, 109, 105, 77);
			tempref.blockType = 12;
			refCol[21] = tempref;
			tempref.col = Color.FromArgb(255, 41, 31, 16);
			tempref.blockType = 17;
			refCol[22] = tempref;
			tempref.col = Color.FromArgb(255, 101, 13, 16);
			tempref.blockType = 21;
			refCol[23] = tempref;
			tempref.col = Color.FromArgb(255, 99, 60, 16);
			tempref.blockType = 22;
			refCol[24] = tempref;
			tempref.col = Color.FromArgb(255, 102, 107, 11);
			tempref.blockType = 23;
			refCol[25] = tempref;
			tempref.col = Color.FromArgb(255, 56, 104, 11);
			tempref.blockType = 24;
			refCol[26] = tempref;
			tempref.col = Color.FromArgb(255, 11, 104, 8);
			tempref.blockType = 25;
			refCol[27] = tempref;
			tempref.col = Color.FromArgb(255, 13, 104, 54);
			tempref.blockType = 26;
			refCol[28] = tempref;
			tempref.col = Color.FromArgb(255, 12, 106, 100);
			tempref.blockType = 27;
			refCol[29] = tempref;
			tempref.col = Color.FromArgb(255, 44, 74, 101);
			tempref.blockType = 28;
			refCol[30] = tempref;
			tempref.col = Color.FromArgb(255, 49, 55, 105);
			tempref.blockType = 29;
			refCol[31] = tempref;
			tempref.col = Color.FromArgb(255, 56, 15, 97);
			tempref.blockType = 30;
			refCol[32] = tempref;
			tempref.col = Color.FromArgb(255, 75, 31, 97);
			tempref.blockType = 31;
			refCol[33] = tempref;
			tempref.col = Color.FromArgb(255, 101, 17, 100);
			tempref.blockType = 32;
			refCol[34] = tempref;
			tempref.col = Color.FromArgb(255, 104, 17, 54);
			tempref.blockType = 33;
			refCol[35] = tempref;
			tempref.col = Color.FromArgb(255, 20, 30, 21);
			tempref.blockType = 34;
			refCol[36] = tempref;
			tempref.col = Color.FromArgb(255, 60, 64, 58);
			tempref.blockType = 35;
			refCol[37] = tempref;
			tempref.col = Color.FromArgb(255, 102, 107, 100);
			tempref.blockType = 36;
			refCol[38] = tempref;
			tempref.col = Color.FromArgb(255, 0, 0, 0);
			tempref.blockType = 49;
			refCol[39] = tempref;
			return refCol;
		}	
	}
}