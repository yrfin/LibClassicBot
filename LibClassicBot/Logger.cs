using System;

namespace LibClassicBot {
	
	public enum LogType {
		/// <summary> Activity of the bot. (e.g. loading of config) </summary>
		BotActivity,

		/// <summary> Warnings or minor recoverable errors. </summary>
		Warning,

		/// <summary> Critical errors. May or may not be recoverable. </summary>
		Error,

		/// <summary> Normal chat messages. </summary>
		Chat,

		/// <summary> Information useful for debugging. Most derived classes of ILogger ignore this in release mode. </summary>
		Debug,
	}
	
	/// <summary> Base interface for all loggers. </summary>
	public interface ILogger {
		
		/// <summary> Called when the logger is being started. </summary>
		void Initalise();
		
		/// <summary> Logs a message. </summary>
		/// <param name="type"> The type or level of message. </param>
		/// <param name="message"> The message. </param>
		void Log( LogType type, string message );
		
		/// <summary> Called when the logger is being closed. </summary>
		void Close();
	}
}