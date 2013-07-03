using System;
using System.Collections.Generic;
using System.Threading;

namespace LibClassicBot
{
	public partial class ClassicBot
	{
		/// <summary>
		/// Allows you to dynamically invoke an action with the bot, as an anonymous delegate.<br/>
		/// The line is the chatline that triggered the command, including colour codes. (Allows for command arguements)
		/// </summary>
		public delegate void CommandDelegate(string line);

		/// <summary>
		/// A Dictionary containg a list of commands that can be executed upon receiving the correct message in chat.
		///	<example><code>ClassicBot.CommandDelegate c = delegate(string Line) { Console.WriteLine(Hello); };
		/// Bot1.Commands.Add("test",c);</code> When a chatline contains the words ".test", the command will be executed, writing Hello to Console.</example>
		/// </summary>
		/// <remarks>The dynamic command invoking is deisgned to be flexible. You can even dynamically create more bots, but you will have to still add commands in like normal.</remarks>
		public Dictionary<string, CommandDelegate> RegisteredCommands = new Dictionary<string, CommandDelegate>();
		
		/// <summary>Although this could be done with a Tuple in .NET 4, this method is easier to understand, and it works in .NET 2.</summary>
		private class InternalCommand
		{
			/// <summary>The actual anonymous method with which command information is stored.</summary>
			public CommandDelegate command;
			
			/// <summary>The chat line which set off the command. Includes color codes. </summary>
			public string line;
			
			internal InternalCommand(CommandDelegate Command, string Line)
			{
				command = Command;
				line = Line;
			}
		}
		
		/// <summary>True if the command thread has been started.</summary>
		public bool CommandsThreadStarted;
		
		/// <summary>The event used for blocking the command thread.</summary>
		private AutoResetEvent CommandQueueReset = new AutoResetEvent(false);
		
		/// <summary>The current queue of command to be executed by the bot.</summary>
		private Queue<InternalCommand> CommandQueue = new Queue<InternalCommand>();
		
		///<summary> This processes all queued commands. Although this should never happen,
		/// it will not proccess if the Commands class has not been started yet.</summary>
		public void ProcessCommandQueue()
		{
			if(!CommandsThreadStarted) return;
			CommandQueueReset.Set();
		}
		
		/// <summary>
		/// Adds a command to the Queue of commands to be executed.
		/// </summary>
		/// <param name="Command">The anonymous delegate to execute.</param>
		/// <param name="Line">The chatline which triggered the execution, used for arguements.</param>
		public void EnqueueCommand(CommandDelegate Command, string Line)
		{
			if(!CommandsThreadStarted) return;
			CommandQueue.Enqueue(new InternalCommand(Command,Line));
		}
		
		/// <summary>Starts the Command class thread, which can later be added to with CommandEnqueue() and executed with ProcessCommandQueue()</summary>
		private void StartCommandsThread()
		{
			CommandsThreadStarted = true;
			Thread CommandsThread = new Thread(CommandQueueThread);
			CommandsThread.IsBackground = true;
			CommandsThread.Name = "CommandThread";
			CommandsThread.Start();
		}
		
		/// <summary>
		/// Used for checking whether to catch exceptions thrown by commands,
		/// or to throw them like normal errors.
		/// </summary>
		private bool DebugMode = false; //System.Diagnostics.Debugger.IsAttached;
		
		/// <summary>
		/// The internal thread used for processing commands.
		/// The thread will sleep until ProcessCommandQueue() is called.
		/// </summary>
		private void CommandQueueThread()
		{
			while (true)
			{
				CommandQueueReset.Reset();//Start at false.
				CommandQueueReset.WaitOne(-1, false); //Wait until the bot calls ProccessCommandQueue(), no timeout in sleep.
				while (CommandQueue.Count != 0)
				{
					InternalCommand IntCommand = CommandQueue.Dequeue();
					if(DebugMode) IntCommand.command.Invoke(IntCommand.line);
					else
					{
						try { IntCommand.command.Invoke(IntCommand.line); }
						catch(Exception ex)
						{
							string commandName = String.Empty;
							foreach( var keypair in RegisteredCommands ) {
								if( keypair.Value == IntCommand.command ) {
									commandName = "." + keypair.Key;
									break;
								}
							}
							Log( LogType.Error,
							    String.Format( "Command name: {0}, Class name: {1} -{2}{3}", commandName, IntCommand.command.Method.Name,
							                  Environment.NewLine, ex.ToString() )
							   );
						}
					}
				}
				Thread.Sleep(1);
			}
		}
	}
}