namespace LibClassicBot.Drawing
{
	/// <summary>
	/// The interface from which all block drawers inherit.
	/// </summary>
	public interface IDrawer
	{		
		/// <summary>
		/// Executes the drawer, and should be executed on a separate thread. 
		/// The token is there if the user finds a need to stop the drawing. (This happens when CriticalAbort is set to true.)
		/// </summary>
		/// <param name="main"></param>
		/// <param name="Aborted">The boolean to check if the drawing needs to be stopped. The paramter is passed as a ref so that
		/// the main bot can set the boolean to false without using messy static variables.</param>
		/// <param name="Points">The points of the draw operation.</param>
		/// <param name="blocktype">The block type to be used, as a byte.</param>
		/// <param name="sleeptime">The time to wait between drawing blocks in milliseconds. The parameter is passed as a ref so the 
		/// waiting time can be changed from the main bot.</param>
		void Start(ClassicBot main, ref bool Aborted, Vector3I[] Points, byte blocktype, ref int sleeptime);
		
		/// <summary>
		/// Gets the name of the current drawing command.
		/// </summary>
		string Name { get; }
	}
}