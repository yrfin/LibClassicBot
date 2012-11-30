using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using LibClassicBot;

namespace LibClassicBot.Plugins
{
	/// <summary>
	/// Loads external plugins, from the plugins folder.
	/// All plugins must inherit from the Plugin abstract class.
	/// </summary>
	public static class PluginManager
	{
		/// <summary>
		/// Load all plugins avaliable in the plugins directory.
		/// </summary>
		/// <param name="commandstoadd">The Dictionary to add commands to.</param>
		/// <param name="main">The ClassicBot instance to attach to. (IE, for message sending.)</param>
		public static void LoadPlugins(ref Dictionary<string,ClassicBot.CommandDelegate> commandstoadd, ClassicBot main)
		{
			if (!Directory.Exists("plugins"))
			{
				Directory.CreateDirectory("plugins");
				return; //No point in loading zero plugins, now is there?
			}
			String[] pluginFiles = Directory.GetFiles("plugins", "*.dll", SearchOption.TopDirectoryOnly); //Limit to DLL files.
			if(pluginFiles.Length == 0) return; //No plugins were found.
			List<Plugin> Plugins = new List<Plugin>();
			for(int i = 0; i < pluginFiles.Length; i++)
			{
				Console.WriteLine(pluginFiles[i]);
				try 
				{
					Assembly assembly = Assembly.LoadFile(Path.GetFullPath(pluginFiles[i]));
					Type[] asmTypes = assembly.GetTypes(); //In other words, look for classes.
					foreach(Type type in asmTypes)
					{
						if (type.IsSubclassOf(typeof(Plugin))) //All plugins must inherit from Plugin.
						{
							Plugins.Add((Plugin)Activator.CreateInstance(type)); //We've probably got a match.
							//Why no break? Because, some plugins could consist of multiple commands in the same file.
						}
					}
				}
				catch { } //Either the plugin is invalid, or probably targets a later version of .NET.
			}
			foreach(Plugin plugin in Plugins)
			{
				Console.WriteLine(plugin.PluginName);
				plugin.Initalize(main);
				if(!commandstoadd.ContainsKey(plugin.PluginName)) //Avoid duplicates, let the custom implementation load commands first.
				{
					commandstoadd.Add(plugin.PluginName, plugin.Command);
				}
			}
		}
	}
}
