using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace LibClassicBot.Plugins
{
	/// <summary>
	/// Loads external plugins, from the plugins folder.
	/// All plugins must inherit from the Plugin abstract class.
	/// </summary>
	public static class PluginManager
	{
		/// <summary> Load all plugins avaliable in the plugins directory. </summary>
		/// <param name="commandstoadd">The Dictionary to add commands to.</param>
		/// <param name="main">The ClassicBot instance to attach to. (IE, for message sending.)</param>
		public static void LoadPlugins( ref Dictionary<string,ClassicBot.CommandDelegate> commandstoadd, ClassicBot main )
		{
			if( !Directory.Exists( "plugins" ) ) {
				return;
			}
			String[] pluginFiles = Directory.GetFiles( "plugins", "*.dll", SearchOption.TopDirectoryOnly );
			if( pluginFiles.Length == 0 ) return; //No plugins were found.
			List<Plugin> Plugins = new List<Plugin>();
			
			for( int i = 0; i < pluginFiles.Length; i++ ) {
				main.Log( LogType.BotActivity, "Loading commands from plugin file " + pluginFiles[i] );
				try {
					Assembly assembly = Assembly.LoadFile( Path.GetFullPath( pluginFiles[i] ) );
					Type[] asmTypes = assembly.GetTypes();
					for( int j = 0; j < asmTypes.Length; j++ ) {
						Type type = asmTypes[j];
						if( type.IsSubclassOf( typeof( Plugin ) ) ) {
							Plugins.Add( (Plugin)Activator.CreateInstance( type ) ); //We've probably got a match.
							//Why no break? Because, some plugins could consist of multiple commands in the same file.
						}
					}
				} catch( Exception e ) {
					main.Log( LogType.Warning, "Couldn't load commands from the plugin.", e.ToString() );
				}
			}
			for( int i = 0; i < Plugins.Count; i++ ) {
				Plugin plugin = Plugins[i];
				plugin.Initialize( main );
				if( !commandstoadd.ContainsKey( plugin.CommandName ) ) {
					main.Log( LogType.BotActivity, "Loading command " + plugin.CommandName );
					commandstoadd.Add( plugin.CommandName, plugin.Command );
				}
			}
		}
	}
}
