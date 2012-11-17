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
using System.IO;
using System.IO.Compression;

namespace LibClassicBot
{
	/// <summary> Creates a FCMv3 compatible map.</summary>
	public sealed class Map
	{
		private readonly short Width; //Notch's X
		private readonly short Length; //Notch's Z
		private readonly short Height; //Notch's Y 
		private int Volume { get { return (Width + 1) * (Length + 1) * (Height + 1); } }		
		// FCMv3 additions
		private Guid Guid;
		private const int Identifier = 0x0FC2AF40;
		private const byte Revision = 13; 
		private byte[] Blocks;

		public Map(int width, int length, int height)
		{
			Guid = Guid.NewGuid();
			Width = (short)width;
			Length = (short)length;
			Height = (short)height;
			Blocks = new byte[Volume];
		}
		
		public void Dispose()
		{
			Blocks = null;
			GC.Collect();
		}

		/// <summary> Saves this map to a file in the format FCMv3. </summary>	
		public void Save(string fileName)
		{
			using (FileStream mapStream = File.Create(fileName, 64 * 1024))
			{
				BinaryWriter writer = new BinaryWriter(mapStream);
				writer.Write(Identifier);
				writer.Write(Revision);
				writer.Write(Width);
				writer.Write(Height);
				writer.Write(Length);
				writer.Write(0); //Spawn.X
				writer.Write(0); //Spawn.Y
				writer.Write(0); //Spawn.Z
				writer.Write((byte)0); //Spawn.R
				writer.Write((byte)0); //Spawn.L
				writer.Write((uint)DateTime.UtcNow.ToFileTime());//.ToUnixTimeLegacy());
				writer.Write((uint)DateTime.UtcNow.ToFileTime());//.ToUnixTimeLegacy());
				writer.Write(this.Guid.ToByteArray());
				writer.Write((byte)1); // layer count
				// skip over index and metacount
				long indexOffset = mapStream.Position;
				writer.Seek(29, SeekOrigin.Current);
				byte[] blocksCache = this.Blocks;
				int compressedLength;
				long offset;
				using (DeflateStream ds = new DeflateStream(mapStream, CompressionMode.Compress, true))
				{
					offset = mapStream.Position; // inaccurate, but who cares
					ds.Write(blocksCache, 0, blocksCache.Length);
					compressedLength = (int)(mapStream.Position - offset);
				}
				// come back to write the index
				writer.BaseStream.Seek(indexOffset, SeekOrigin.Begin);
				writer.Write((byte)0);            // data layer type (Blocks)
				writer.Write(offset);             // offset, in bytes, from start of stream
				writer.Write(compressedLength);   // compressed length, in bytes
				writer.Write(0);                  // general purpose field
				writer.Write(1);                  // element size
				writer.Write(blocksCache.Length); // element count
				writer.Write(0); //Metadata, write nothing.
			}
		}
		
		/// <summary> Sets a block at given coordinates. </summary>
		public void SetBlock(int x, int y, int z, byte type)
		{
			int index = (z * Length + y) * Width + x;
			Blocks[index] = type;
		}
	}
}