using System;
using System.IO;
using System.Text;
using LibClassicBot;

namespace ClassicBotCoreTest {
	
	public sealed class ConsoleLogger : ILogger {
		
		public void Initalise() { }
		
		const string LogTimeFormat = "HH':'mm':'ss";
		
		public void Log( LogType type, string message ) {
			if( type == LogType.Error ) {
				Console.Write( DateTime.Now.ToString( LogTimeFormat ) + " > " );
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine( "ERROR: " + message );
				Console.ForegroundColor = ConsoleColor.White;
			} else if( type == LogType.Warning ) {
				Console.Write( DateTime.Now.ToString( LogTimeFormat ) + " > " );
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine( "Warning: " + message );
				Console.ForegroundColor = ConsoleColor.White;
			} else if( type == LogType.BotActivity ) {
				Console.Write( DateTime.Now.ToString( LogTimeFormat ) + " > " );
				Console.ForegroundColor = ConsoleColor.DarkGray;
				Console.WriteLine( message );
				Console.ForegroundColor = ConsoleColor.White;
			} else if( type == LogType.Chat ) {
				Console.Write( DateTime.Now.ToString( LogTimeFormat ) + " > " );
				AppendChat( message );
			}
			#if DEBUG
			else if( type == LogType.Debug ) {
				Console.Write( DateTime.Now.ToString( LogTimeFormat ) + " > " );
				Console.WriteLine( "DEBUG: " + message );
			}
			#endif
		}
		
		static bool IsColourCodeChar( char code, out ConsoleColor color ) {
			switch( code ) {
					case '0': color = ConsoleColor.Black; return true; //Black
					case '1': color = ConsoleColor.DarkBlue; return true; //Dark Blue / Navy
					case '2': color = ConsoleColor.DarkGreen; return true; //Dark Green / Green
					case '3': color = ConsoleColor.DarkCyan; return true; //Dark Cyan / Teal
					case '4': color = ConsoleColor.DarkRed; return true; //Dark Red / Maroon
					case '5': color = ConsoleColor.DarkMagenta; return true; //Dark Magenta / Purple
					case '6': color = ConsoleColor.DarkYellow; return true; //Dark Yellow / Olive
					case '7': color = ConsoleColor.DarkGray; return true; //Dark Gray / Silver
					case '8': color = ConsoleColor.Gray; return true; //Gray
					case '9': color = ConsoleColor.Blue; return true; //Blue
					case 'a': color = ConsoleColor.Green; return true; //Green / Lime
					case 'b': color = ConsoleColor.Cyan; return true; //Cyan / Aqua
					case 'c': color = ConsoleColor.Red; return true; //Red
					case 'd': color = ConsoleColor.Magenta; return true; //Magenta
					case 'e': color = ConsoleColor.Yellow; return true; //Yellow
					case 'f': color = ConsoleColor.White; return true; //White
					default: color = ConsoleColor.White; return false; //Unknown. Go back to default
			}
		}

		void AppendChat( string input ) {
			ConsoleColor currentColor = ConsoleColor.White;
			for( int i = 0; i < input.Length; i++ ) {
				if( input[i] == '&' && i < input.Length - 1 ) {
					i++;//Move to colour code character.
					
					if( IsColourCodeChar( input[i], out currentColor ) ) {
						i++;
						StringBuilder builder = new StringBuilder();
						while( i < input.Length && input[i] != '&' ) {
							builder.Append( input[i] );
							i++;
						}
						i--; //Move back a character, as otherwise we go one too far and consume the next &.
						Console.ForegroundColor = currentColor;
						Console.Write( builder );
					}
				}
			}
			Console.Write( Environment.NewLine );
			Console.ResetColor();
		}
		
		public void Close() { }
	}
	
	public sealed class FileLogger : ILogger {
		StreamWriter chatLog, mainLog;
		DateTime loadTime;
		const string LogFileFormat = "yyyy-MM-dd", LogLineFormat = "yyyy-MM-dd : HH:mm:ss > ";
		
		public void Initalise() {
			loadTime = DateTime.Now;
			string chatLogFile = "Logs/chatlog-" + DateTime.Now.ToString( LogFileFormat ) + ".txt";
			string mainLogFile = "Logs/log.txt"; 
			if( !Directory.Exists( "Logs" ) ) {
				Directory.CreateDirectory( "Logs" );
			}
			if( !File.Exists( chatLogFile ) ) {
				using( FileStream fs = File.Create( chatLogFile ) ) { }
			}
			if( !File.Exists( mainLogFile ) ) {
				using( FileStream fs = File.Create( mainLogFile ) ) { }
			}
			
			chatLog = new StreamWriter( new FileStream( chatLogFile, FileMode.Append, FileAccess.Write, FileShare.Read ) );
			chatLog.AutoFlush = true;
			mainLog = new StreamWriter( new FileStream( mainLogFile, FileMode.Append, FileAccess.Write, FileShare.Read ) );
			mainLog.AutoFlush = true;
		}
		
		public void Log( LogType type, string message ) {
			DateTime now = DateTime.Now;
			string logLine = now.ToString( LogLineFormat );
			if( type == LogType.Error ) {
				mainLog.WriteLine( logLine + " Error: " + message);
			} else if( type == LogType.Warning ) {
				mainLog.WriteLine( logLine + " Warning: " + message);
			} else if( type == LogType.BotActivity ) {
				mainLog.WriteLine( logLine + " Activity: " + message);
			} else if( type == LogType.Chat ) {
				CheckChatLog( now );
				chatLog.WriteLine( logLine + Extensions.StripColors( message ).TrimEnd( ' ' ) );
			}
		}
		
		// Checks if the chat log filestream needs to be updated.
		void CheckChatLog( DateTime now ) {
			if( now.Month != loadTime.Month || now.Day != loadTime.Day ) {
				chatLog.Flush();
				chatLog.Close();
				string chatLogFile = "Logs/chatlog-" + DateTime.Now.ToString( LogFileFormat ) + ".txt";
				chatLog = new StreamWriter( new FileStream( chatLogFile, FileMode.Append, FileAccess.Write, FileShare.Read ) );
				chatLog.AutoFlush = true;
			}
		}
		
		public void Close() {
			chatLog.Flush();
			chatLog.Close();
			mainLog.Flush();
			mainLog.Close();
		}
	}
}