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
		public Dictionary<string,CommandDelegate> RegisteredCommands = new Dictionary<string,CommandDelegate>();
		
		private class CommandsClass
		{
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
			public bool Started;
			
			/// <summary>The event used for blocking the command thread.</summary>
			private AutoResetEvent CommandQueueReset = new AutoResetEvent(false);
			
			/// <summary>The current queue of command to be executed by the bot.</summary>
			private Queue<InternalCommand> CommandQueue = new Queue<InternalCommand>();
			
			///<summary> This processes all queued commands. Although this should never happen,
			/// it will not proccess if the Commands class has not been started yet.</summary>
			public void ProcessCommandQueue()
			{
				if(!Started) return;
				CommandQueueReset.Set();
			}
			
			/// <summary>
			/// Adds a command to the Queue of commands to be executed.
			/// </summary>
			/// <param name="Command">The anonymous delegate to execute.</param>
			/// <param name="Line">The chatline which triggered the execution, used for arguements.</param>
			public void EnqueueCommand(CommandDelegate Command, string Line)
			{
				if(!this.Started) return;
				CommandQueue.Enqueue(new InternalCommand(Command,Line));
			}
			
			/// <summary>Starts the Command class thread, which can later be added to with CommandEnqueue() and executed with ProcessCommandQueue()</summary>
			public void Start(bool RunOnSeparateThread)
			{
				this.Started = true;
				Thread CommandsThread = new Thread(CommandQueueThread);
				CommandsThread.IsBackground = true;
				CommandsThread.Name = "CommandThread";
				CommandsThread.Start();
			}
			
			/// <summary>
			/// The internal thread used for processing commands.
			/// The thread will sleep until ProcessCommandQueue() is called.
			/// </summary>
			private void CommandQueueThread()
			{
				while (true)
				{
					CommandQueueReset.Reset();//Start at false.
					CommandQueueReset.WaitOne(-1, false); //Wait until the bot calls ProccessCommandQueue();
					while (CommandQueue.Count != 0)
					{
						InternalCommand IntCommand = CommandQueue.Dequeue();
						try { IntCommand.command.Invoke(IntCommand.line); }
						catch(Exception ex) 
						{
							SendMessagePacket("Error while trying to execute a command.");
							System.IO.File.Create("pluginerror.txt");
							System.IO.File.AppendAllText("pluginerror.txt", "Stack- " + ex.StackTrace + Environment.NewLine);
							System.IO.File.AppendAllText("pluginerror.txt", "Message- " + ex.Message + Environment.NewLine);
							System.IO.File.AppendAllText("pluginerror.txt", "Source- " + ex.Source + Environment.NewLine);
							if(System.Diagnostics.Debugger.IsAttached) throw; //So as not to interfere with debugging.
						}
					}
					Thread.Sleep(1);
				}
				
			}
		}
		
	}
}