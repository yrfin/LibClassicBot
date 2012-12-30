﻿using System;
using System.IO;

namespace LibClassicBot.Drawing
{
	/// <summary>
	/// Vector3I contains three points, which is used by Drawing commands. (Included with the bot is Cuboid)<br/>
	/// Th
	/// </summary>
	public struct Vector3I : IEquatable<Vector3I>, IComparable<Vector3I>
	{
		/// <summary>
		/// The X coordinate of the Vector3I. The X coordinate should be positive, and this is checked when drawing.
		/// (North - South)</summary>
		public int X;
		
		/// <summary>
		/// The Y coordinate of the Vector3I. The X coordinate should be positive, and this is checked when drawing.
		/// (East - West)</summary>
		public int Y;
		
		/// <summary>
		/// The Z coordinate of the Vector3I. The X coordinate should be positive, and this is checked when drawing.
		/// (East - West)</summary>
		public int Z;
		
		
		#region Equality
		
		public override bool Equals(object obj)
		{
			if (obj is Vector3I)
				return Equals((Vector3I)obj); // use Equals method below
			else
				return false;
		}
		
		public bool Equals(Vector3I other)
		{
			// add comparisions for all members here
			return this.X == other.X && this.Y == other.Y && this.Z == other.Z;
		}
		
		public override int GetHashCode()
		{
			// combine the hash codes of all members here (e.g. with XOR operator ^)
			return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
		}
		
		/// <summary>
		/// Compares this instance to another Vector3I.
		/// </summary>
		/// <param name="value">Vector3I to compare values with.</param>
		/// <returns>A signed number indicating the relative values of this instance and value.
		/// If the number is -1, all three coordinates are less than the value. If the number is zero,
		/// all three coordinates are equal. If the number is 1, all three coordinates are greater than the value.</returns>
		public int CompareTo(Vector3I value)
		{
			if (this < value) {
				return -1;
			}
			if (this > value) {
				return 1;
			}
			return 0;
		}
		#endregion

		public static double Distance(Vector3I v1, Vector3I v2) {
			return Math.Sqrt(
				(v1.X - v2.X) * (v1.X - v2.X) +
				(v1.Y - v2.Y) * (v1.Y - v2.Y) +
				(v1.Z - v2.Z) * (v1.Z - v2.Z));
		}
		
		public double Distance(Vector3I other) {
			return Distance(this, other);
		}
		
		#region Operators
		public static bool operator == (Vector3I left, Vector3I right) {
			return left.Equals(right);
		}
		
		public static bool operator != (Vector3I left, Vector3I right) {
			return !left.Equals(right);
		}
		
		public static bool operator > (Vector3I left, Vector3I right) {
			if(left.X > right.X && left.Y > right.Y && left.Z > right.Z) return true;
			else return false;
		}
		
		public static bool operator < (Vector3I left, Vector3I right) {
			if(left.X < right.X && left.Y < right.Y && left.Z < right.Z) return true;
			else return false;
		}
		
		public static bool operator >= (Vector3I left, Vector3I right) {
			if(left.X >= right.X && left.Y >= right.Y && left.Z >= right.Z) return true;
			else return false;
		}
		
		public static bool operator <= (Vector3I left, Vector3I right) {
			if(left.X <= right.X && left.Y <= right.Y && left.Z <= right.Z) return true;
			else return false;
		}

		public static Vector3I operator + (Vector3I left, Vector3I right) {
			return new Vector3I(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
		}
		
		public static Vector3I operator - (Vector3I left, Vector3I right) {
			return new Vector3I(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
		}

		public static Vector3I operator + (Vector3I value) {
			return new Vector3I(+value.X, +value.Y,+value.Z);
		}
		
		public static Vector3I operator - (Vector3I value) {
			return new Vector3I(-value.X, -value.Y, -value.Z);
		}
		
		public static Vector3I operator * ( Vector3I value, int scalar ) {
			return new Vector3I( value.X * scalar, value.Y * scalar, value.Z * scalar );
		}
		
		public static Vector3I operator + (Vector3I left, int right) {
			return new Vector3I(left.X + right, left.Y + right, left.Z + right);
		}
		
		public static Vector3I operator - (Vector3I left, int right) {
			return new Vector3I(left.X - right, left.Y - right, left.Z - right);
		}
		#endregion
		
		
		public int this[int i] {
			get {
				switch( i ) {
						case 0: return X;
						case 1: return Y;
						default: return Z;
				}
			}
			set {
				switch( i ) {
						case 0: X = value; return;
						case 1: Y = value; return;
						default: Z = value; return;
				}
			}
		}
		#region Constructors
		/// <summary>Constructs a Vector3I from the three given integer points.</summary>
		public Vector3I(int x, int y, int z)
		{
			this.X = x; this.Y = y; this.Z = z;
		}

		/// <summary>Constructs a Vector3I from the three given short points.</summary>
		public Vector3I(short x, short y, short z)
		{
			this.X = x; this.Y = y; this.Z = z;
		}
		
		#endregion
		
		
		#region General Utilities
		/// <summary>
		/// Returns a Vector3I containing the minimum X coordinate,Y coordinate and Z coordinate from the two Vector3Is.
		/// </summary>
		/// <param name="val1">The first Vector3I which values will be compared with.</param>
		/// <param name="val2">The second Vector3I which values will be compared against.</param>
		/// <returns>A new Vector3I containing the smallest coordinates from both Vector3I's.</returns>
		/// <example><code>Vector3I a = new Vector3I(5,0,85); Vector3I b = new Vector3I(5,62,33);<br/>
		///Vector3I smallest = Vector3I.Min(a,b); Console.WriteLine(smallest);</code>Would output: 5,0,33</example>
		public static Vector3I Min(Vector3I val1, Vector3I val2)
		{
			Vector3I val = new Vector3I();
			val.X = Math.Min(val1.X, val2.X);
			val.Y = Math.Min(val1.Y, val2.Y);
			val.Z = Math.Min(val1.Z, val2.Z);
			return val;
		}
		
		/// <summary>
		/// Returns a Vector3I containing the maximum X coordinate,Y coordinate and Z coordinate from the two Vector3Is.
		/// </summary>
		/// <param name="val1">The first Vector3I which values will be compared with.</param>
		/// <param name="val2">The second Vector3I which values will be compared against.</param>
		/// <returns>A new Vector3I containing the largest coordinates from both Vector3I's.</returns>
		/// <example><code>Vector3I a = new Vector3I(5,0,85); Vector3I b = new Vector3I(5,62,33);<br/>
		/// Vector3I smallest = Vector3I.Min(a,b); Console.WriteLine(smallest);</code>Would output: 5,62,85</example>
		public static Vector3I Max(Vector3I val1, Vector3I val2)
		{
			Vector3I val = new Vector3I();
			val.X = Math.Max(val1.X, val2.X);
			val.Y = Math.Max(val1.Y, val2.Y);
			val.Z = Math.Max(val1.Z, val2.Z);
			return val;
		}

		/// <summary>
		/// Returns a Vector3I containing the absolute value of the X coordinate,Y coordinate and Z coordinate.
		/// </summary>
		/// <param name="val1">The first Vector3I which values will be compared with.</param>
		/// <returns>A new Vector3I containing the largest coordinates from both Vector3I's.</returns>
		/// <example><code>Vector3I a = new Vector3I(-5,6,-85); Vector3I absolute = Vector3I.Abs(a);<br/>
		/// Console.WriteLine(absolute);</code>Would output: 5,6,85</example>
		public static Vector3I Abs(Vector3I val1)
		{
			Vector3I val = new Vector3I();
			val.X = Math.Abs(val1.X);
			val.Y = Math.Abs(val1.Y);
			val.Z = Math.Abs(val1.Z);
			return val;
		}
		#endregion
		
		
		#region Stream Utilities
		/// <summary>
		/// Returns a Vector3I from the given BinaryReader.
		/// </summary>
		/// <exception cref="System.IO.EndOfStreamException">The end of the stream was reached
		/// while trying to read a Vector3I.</exception>
		/// <exception cref="System.IOException">An I/O error occured while trying to read.</exception>
		/// <param name="reader">The BinaryReader to read from. The method reads in signed integers,
		/// so the offset will be increased by twelve bytes.</param>
		/// <param name="BigEndian">True if the input base stream is in big endian,
		/// false if the base stream is in little endian. (Default of Streams in .NET)</param>
		/// <returns>The Vector3I read from the BinaryReader.</returns>
		public static Vector3I FromBinaryReader(BinaryReader reader, bool BigEndian)
		{
			Vector3I result;
			if(BigEndian) {
				result.X = System.Net.IPAddress.HostToNetworkOrder(reader.ReadInt32());
				result.Y = System.Net.IPAddress.HostToNetworkOrder(reader.ReadInt32());
				result.Z = System.Net.IPAddress.HostToNetworkOrder(reader.ReadInt32());
			} else {
				result.X = reader.ReadInt32();
				result.Y = reader.ReadInt32();
				result.Z = reader.ReadInt32();
			}
			return result;
		}
		
		/// <summary>
		/// Writes a given Vector3I to the given BinaryWriter
		/// </summary>
		/// <exception cref="System.IOException">An I/O error occured while trying to write.</exception>
		/// <param name="input">The Vector3I to write. The Vector3I will be written in the order
		/// X, Y and lastly Z.</param>
		/// <param name="writer">The BinaryWriter to write to. The method writes in signed integers,
		/// so the offset will be increased by twelve bytes.</param>
		/// <param name="BigEndian">True if the input base stream is in big endian,
		/// false if the base stream is in little endian. (Default of Streams in .NET)</param>
		/// <param name="BigEndian"></param>
		public static void ToBinaryWriter(Vector3I input, BinaryWriter writer, bool BigEndian)
		{
			if(BigEndian) {
				writer.Write(System.Net.IPAddress.HostToNetworkOrder(input.X));
				writer.Write(System.Net.IPAddress.HostToNetworkOrder(input.Y));
				writer.Write(System.Net.IPAddress.HostToNetworkOrder(input.Z));
			} else {
				writer.Write(input.X);
				writer.Write(input.Y);
				writer.Write(input.Z);
			}
		}
		
		/// <summary>
		/// Writes the Vector3I to the given BinaryWriter. See the static version of this method
		/// for more information.
		/// </summary>
		public void ToBinaryWriter(BinaryWriter writer, bool BigEndian)
		{
			ToBinaryWriter(this, writer, BigEndian);
		}
		#endregion
		
		
		public override string ToString()
		{
			return String.Format("{0},{1},{2}", X, Y, Z );
		}
	}
}