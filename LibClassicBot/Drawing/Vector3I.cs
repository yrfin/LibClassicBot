using System;

namespace LibClassicBot.Drawing
{
	/// <summary> Vector3I contains three points, which is used by Drawing commands. </summary>
	public struct Vector3I : IEquatable<Vector3I>, IComparable<Vector3I>
	{
		/// <summary> The X coordinate of the Vector3I (North - South)</summary>
		public int X;
		
		/// <summary> The Y coordinate of the Vector3I. (East - West) </summary>
		public int Y;
		
		/// <summary> The Z coordinate of the Vector3I. (Height) </summary>
		public int Z;
		
		
		#region Equality
		
		public override bool Equals( object obj ) {
			if( obj is Vector3I )
				return Equals( (Vector3I)obj );
			return base.Equals( obj );
		}
		
		public bool Equals( Vector3I other ) {
			return X == other.X && Y == other.Y && Z == other.Z;
		}
		
		public override int GetHashCode()
		{
			// combine the hash codes of all members here (e.g. with XOR operator ^)
			return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
		}
		
		/// <summary> Compares this instance to another Vector3I. </summary>
		/// <param name="value">Vector3I to compare values with.</param>
		/// <returns>A signed number indicating the relative values of this instance and value.
		/// If the number is -1, all three coordinates are less than the value. If the number is zero,
		/// all three coordinates are equal. If the number is 1, all three coordinates are greater than the value.</returns>
		public int CompareTo( Vector3I value )
		{
			if( this < value ) {
				return -1;
			} else if( this > value ) {
				return 1;
			}
			return 0;
		}
		
		#endregion

		public static double Distance( Vector3I v1, Vector3I v2 ) {
			return Math.Sqrt(
				( v1.X - v2.X ) * ( v1.X - v2.X ) +
				( v1.Y - v2.Y ) * ( v1.Y - v2.Y ) +
				( v1.Z - v2.Z ) * ( v1.Z - v2.Z )
			);
		}
		
		public double DistanceTo( Vector3I other ) {
			return Distance( this, other );
		}
		
		#region Operators
		
		public static bool operator == (Vector3I left, Vector3I right) {
			return left.Equals( right) ;
		}
		
		public static bool operator != (Vector3I left, Vector3I right) {
			return !left.Equals( right );
		}
		
		public static bool operator > ( Vector3I left, Vector3I right ) {
			if( left.X > right.X && left.Y > right.Y && left.Z > right.Z ) return true;
			return false;
		}
		
		public static bool operator < ( Vector3I left, Vector3I right ) {
			if( left.X < right.X && left.Y < right.Y && left.Z < right.Z ) return true;
			return false;
		}
		
		public static bool operator >= ( Vector3I left, Vector3I right ) {
			if( left.X >= right.X && left.Y >= right.Y && left.Z >= right.Z ) return true;
			return false;
		}
		
		public static bool operator <= ( Vector3I left, Vector3I right ) {
			if( left.X <= right.X && left.Y <= right.Y && left.Z <= right.Z ) return true;
			return false;
		}

		public static Vector3I operator + ( Vector3I left, Vector3I right ) {
			return new Vector3I( left.X + right.X, left.Y + right.Y, left.Z + right.Z );
		}
		
		public static Vector3I operator - ( Vector3I left, Vector3I right ) {
			return new Vector3I( left.X - right.X, left.Y - right.Y, left.Z - right.Z );
		}
		
		public static Vector3I operator - ( Vector3I value ) {
			return new Vector3I( -value.X, -value.Y, -value.Z );
		}
		
		public static Vector3I operator * ( Vector3I value, int scalar ) {
			return new Vector3I( value.X * scalar, value.Y * scalar, value.Z * scalar );
		}
		
		public static Vector3I operator / ( Vector3I value, int scalar ) {
			return new Vector3I( value.X / scalar, value.Y / scalar, value.Z / scalar );
		}
		
		public static Vector3I operator + ( Vector3I left, int right ) {
			return new Vector3I(left.X + right, left.Y + right, left.Z + right);
		}
		
		public static Vector3I operator - ( Vector3I left, int right ) {
			return new Vector3I( left.X - right, left.Y - right, left.Z - right );
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
		public Vector3I( int x, int y, int z ) {
			X = x;
			Y = y;
			Z = z;
		}
		
		#endregion
		
		
		#region General Utilities
		
		/// <summary> Returns a Vector3I containing the minimum coordinates from the two Vector3Is. </summary>
		/// <param name="val1"> The first Vector3I which values will be compared with. </param>
		/// <param name="val2"> The second Vector3I which values will be compared against. </param>
		/// <returns> A new Vector3I containing the smallest coordinates from both Vector3I's. </returns>
		public static Vector3I Min( Vector3I val1, Vector3I val2 ) {
			Vector3I min = new Vector3I();
			min.X = Math.Min( val1.X, val2.X );
			min.Y = Math.Min( val1.Y, val2.Y );
			min.Z = Math.Min( val1.Z, val2.Z );
			return min;
		}
		
		/// <summary> Returns a Vector3I containing the maximum coordinates from the two Vector3Is. </summary>
		/// <param name="val1"> The first Vector3I which values will be compared with. </param>
		/// <param name="val2"> The second Vector3I which values will be compared against. </param>
		/// <returns> A new Vector3I containing the largest coordinates from both Vector3I's. </returns>
		public static Vector3I Max( Vector3I val1, Vector3I val2 ) {
			Vector3I max = new Vector3I();
			max.X = Math.Max( val1.X, val2.X) ;
			max.Y = Math.Max( val1.Y, val2.Y );
			max.Z = Math.Max( val1.Z, val2.Z );
			return max;
		}

		/// <summary> Returns a Vector3I containing the absolute value of all coordinates. </summary>
		/// <param name="value"> The Vector3I value. </param>
		/// <returns> A new Vector3I containing the absolute coordinates of the given Vector3I. </returns>
		public static Vector3I Abs( Vector3I value ) {
			Vector3I val = new Vector3I();
			val.X = Math.Abs( value.X );
			val.Y = Math.Abs( value.Y );
			val.Z = Math.Abs( value.Z );
			return val;
		}
		
		#endregion
		
		
		public override string ToString() {
			return String.Format("{0},{1},{2}", X, Y, Z );
		}
	}
}