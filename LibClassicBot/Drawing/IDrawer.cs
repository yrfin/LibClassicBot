using System;

namespace LibClassicBot.Drawing
{
	/// <summary>
	/// The interface from which all block drawers inherit.
	/// </summary>
	public interface IDrawer
	{
		/// <summary>The lower point of the cuboided area.
		/// The derived drawing class should be able to cope if this point is larger.</summary>
		Vector3I Point1{ get; set;}
		
		/// <summary>The upper point of the cuboided area.
		/// The derived drawing class should be able to cope if this point is smaller.</summary>		
		/// <remarks>To ensure that the cuboid is being done in the correct order, Vector3I.Max(Point1, Point2)</remarks>
		Vector3I Point2{ get; set;}
		
		/// <summary>
		/// Executes the drawer, and should be executed on a separate thread. 
		/// The token is there if the user finds a need to stop the drawing. (This happens when CriticalAbort is set to true.)
		/// </summary>
		/// <param name="main"></param>
		/// <param name="cToken">The CuboidToken to check if the drawing needs to be stopped. The paramter is passed as a ref so that
		/// the ClassicBot can cancel the CancelDrawingToken without using messy static variables.</param>
		/// <param name="Point1">The lower point of the draw operation.</param>
		/// <param name="Point2">The upper point of the draw operation.</param>
		/// <param name="blocktype">The block type to be used, as a byte.</param>
		/// <param name="sleeptime">The time to wait between drawing blocks in milliseconds. The parameter is passed as a ref so the 
		/// waiting time can be changed from the main bot.</param>
		void Start(ClassicBot main, ref CancelDrawingToken cToken, Vector3I Point1, Vector3I Point2, byte blocktype, ref int sleeptime);
		
		/// <summary>
		/// Gets the name of the current drawing command.
		/// </summary>
		string Name { get; }
		
		/// <summary>
		/// The CancelDrawingToken used by the drawing command to check if the draw operation should be stopped.
		/// </summary>
		CancelDrawingToken DrawingToken {get; set; }
	}
}
