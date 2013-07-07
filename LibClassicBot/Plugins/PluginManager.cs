using System;
using System.CodeDom.Compiler;
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
		/// <param name="commands"> The Dictionary to add commands to. </param>
		/// <param name="main"> The ClassicBot instance to attach to. (e.g. for message sending.)</param>
		public static void LoadPlugins( Dictionary<string, CommandDelegate> commands, ClassicBot main )
		{
			if( !Directory.Exists( "plugins" ) ) {
				return;
			}
			string[] pluginFiles = Directory.GetFiles( "plugins", "*.dll", SearchOption.TopDirectoryOnly );
			if( pluginFiles.Length == 0 ) return;
			
			for( int i = 0; i < pluginFiles.Length; i++ ) {
				main.Log( LogType.BotActivity, "Loading commands from plugin file " + pluginFiles[i] );
				try {
					Assembly assembly = Assembly.LoadFile( Path.GetFullPath( pluginFiles[i] ) );
					Type[] types = assembly.GetTypes();
					for( int j = 0; j < types.Length; j++ ) {
						Type type = types[j];
						if( type.IsSubclassOf( typeof( Plugin ) ) ) {
							Plugin plugin = (Plugin)Activator.CreateInstance( type );
							plugin.Initialize( main );
							if( !commands.ContainsKey( plugin.CommandName ) ) {
								main.Log( LogType.BotActivity, "Loaded command " + plugin.CommandName );
								commands.Add( plugin.CommandName, plugin.Command );
							}
						}
					}
				} catch( Exception e ) {
					main.Log( LogType.Error, "Couldn't load commands from the plugin.", e.ToString() );
				}
			}
			LoadScripts( commands, main );
		}
		
		
		class Script {
			public string Code;
			public string[] KnownReferences;
			public bool IsCSharp;
		}	
		
		static void LoadScripts( Dictionary<string, CommandDelegate> commands, ClassicBot main ) {
			if( !Directory.Exists( "scripts" ) ) {
				return;
			}
			String[] files = Directory.GetFiles( "scripts", "*.*", SearchOption.TopDirectoryOnly );
			if( files.Length == 0 ) return;
			Dictionary<string, Script> scripts = new Dictionary<string, Script>();
			
			for( int i = 0; i < files.Length; i++ ) {
				string fileName = files[i];
				string extension = Path.GetExtension( fileName ).ToLowerInvariant(); // Includes .
				string name = Path.GetFileNameWithoutExtension( fileName );
				
				if( extension == ".reference" || extension == ".references" ) {
					Script script;
					if( !scripts.TryGetValue( name, out script ) ) {
						script = new Script();
						scripts[name] = script;
					}
					script.KnownReferences = File.ReadAllText( fileName ).Split( ',' );
				} else if( extension == ".cs" ) {
					Script script;
					if( !scripts.TryGetValue( name, out script ) ) {
						script = new Script();
						scripts[name] = script;
					}
					script.Code = File.ReadAllText( fileName );
					script.IsCSharp = true;
					main.Log( LogType.BotActivity, "Loading commands from script file " + fileName );
				} else if( extension == ".vb" ) {
					Script script;
					if( !scripts.TryGetValue( name, out script ) ) {
						script = new Script();
						scripts[name] = script;
					}
					script.Code = File.ReadAllText( fileName );
					main.Log( LogType.BotActivity, "Loading commands from script file " + fileName );
				}
			}
			CompileScripts( scripts, commands, main );
		}
		
		static void CompileScripts( Dictionary<string, Script> scripts, Dictionary<string, CommandDelegate> commands, ClassicBot main )
		{
			foreach( var keypair in scripts ) {
				Script script = keypair.Value;
				if( String.IsNullOrEmpty( script.Code ) ) continue;
				CodeDomProvider compiler;
				if( script.IsCSharp ) {
					compiler = new Microsoft.CSharp.CSharpCodeProvider( new Dictionary<string, string>{ { "CompilerVersion", "v2.0" } } );
				} else {
					compiler = new Microsoft.VisualBasic.VBCodeProvider( new Dictionary<string, string>{ { "CompilerVersion", "v2.0" } } );
				}
				
				CompilerParameters cParams = new CompilerParameters {
					GenerateExecutable = false,
					GenerateInMemory = true,
					//CompilerOptions = "/unsafe", Causes errors with the Visual Basic Compiler.
					TreatWarningsAsErrors = false,
					WarningLevel = 4,
				};
				if( script.KnownReferences == null || script.KnownReferences.Length == 0 ) { // Add defalt references.
					cParams.ReferencedAssemblies.Add( "System.dll" );
					cParams.ReferencedAssemblies.Add( "LibClassicBot.dll" );
				} else {
					cParams.ReferencedAssemblies.AddRange( script.KnownReferences );
				}
				
				CompilerResults results = compiler.CompileAssemblyFromSource( cParams, script.Code );
				if( results.Errors.Count != 0 )  {
					try {
						foreach( CompilerError error in results.Errors ) {
							string errorMsg = String.Format( "{0} Line: {1} {2}", error.ErrorNumber, error.Line, error.ErrorText );
							if( error.IsWarning ) {
								main.Log( LogType.Error, "Warning while compiling script " + keypair.Key, errorMsg );
							} else {
								main.Log( LogType.Error, "Error while compiling script " + keypair.Key, errorMsg );
							}
						}
					} catch( Exception e ) {
						main.Log( LogType.Error, "Error while trying to display errors. " , e.ToString() );
					}
					continue;
				}
				compiler.Dispose();
				
				Type[] types = results.CompiledAssembly.GetTypes();
				for( int i = 0; i < types.Length; i++ ) {
					Type type = types[i];
					if( type.IsSubclassOf( typeof( Plugin ) ) ) {
						Plugin plugin = (Plugin)Activator.CreateInstance( type );
						plugin.Initialize( main );
						if( !commands.ContainsKey( plugin.CommandName ) ) {
							main.Log( LogType.BotActivity, "Loaded command " + plugin.CommandName );
							commands.Add( plugin.CommandName, plugin.Command );
						}
					}
				}
			}
		}
	}
}
