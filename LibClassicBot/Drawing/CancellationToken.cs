using System;

namespace System.Threading
{

	public struct CancellationToken
	{
		private CancellationTokenSource m_source;

		public bool IsCancellationRequested
		{
			get { return this.m_source != null && this.m_source.IsCancellationRequested; }
		}
		public bool CanBeCanceled
		{
			get { return this.m_source != null && this.m_source.CanBeCanceled; }
		}

		internal CancellationToken(CancellationTokenSource source)
		{
			this.m_source = source;
		}
	}
}
