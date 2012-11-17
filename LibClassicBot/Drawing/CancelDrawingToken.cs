using System;

namespace LibClassicBot.Drawing
{
	/// <summary>
	/// Based on CancellationTokenSource and CancellationToken in .NET Framework 4.
	/// </summary>
	public class CancelDrawingToken
	{
		private bool cancelled = false;
		
		public bool IsCancellationRequested
		{
			get { return this.cancelled == true; }
		}
		public bool CanBeCancelled
		{
			get { return this.cancelled == false; }
		}

		public CancelDrawingToken()
		{
			this.cancelled = false;
		}
		public void Cancel()
		{
			this.cancelled = true;
		}
		
		public void Reset()
		{
			this.cancelled = false;
		}
	}
}