using System;

namespace LibClassicBot {
	
	/// <summary> Manages the configuration file for a bot instance. </summary>
	public sealed class Config {
		
		/// <summary> Gets the relative file path of the configuration file. </summary>
		public string ConfigFile { get; private set; }
	}
}
