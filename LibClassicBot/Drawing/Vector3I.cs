using System;

namespace LibClassicBot.Drawing
{
	/// <summary>
	/// Vector3I contains three points, which is used by Drawing commands. (Included with the bot is Cuboid)<br/>
	/// Th
	/// </summary>
	public struct Vector3I : IEquatable<Vector3I>
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
		
		public static bool operator ==(Vector3I left, Vector3I right)
		{
			return left.Equals(right);
		}
		
		public static bool operator !=(Vector3I left, Vector3I right)
		{
			return !left.Equals(right);
		}
		#endregion
		
		
		#region Constructors
		/// <summary>Constructs a Vector3I from the three given integer points.</summary>
		public Vector3I(int x, int y, int z)
		{
			this.X = x; this.Y = y; this.Z = z;
		}
		
		/// <summary>Constructs a Vector3I from the three given float points. Note that the float will be truncated, not rounded.</summary>
		public Vector3I(float x, float y, float z)
		{
			this.X = (int)x; this.Y = (int)y; this.Z = (int)z;
		}

		/// <summary>Constructs a Vector3I from the three given short points.</summary>
		public Vector3I(short x, short y, short z)
		{
			this.X = x; this.Y = y; this.Z = z;
		}
		
		#endregion
		
		
		#region General Utilities
		/// <summary>
		/// Returns a Vector3I containing the minimum X coordinate,Y coordinate and Z coordinate from the two points.
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
		/// Returns a Vector3I containing the maximum X coordinate,Y coordinate and Z coordinate from the two points.
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
		
        public override string ToString() 
        {
            return String.Format("{0},{1},{2}", X, Y, Z );
        }		
	}
}
