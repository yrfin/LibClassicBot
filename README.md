# LibClassicBot

LibClassicBot consists of two main projects:

* LibClassicBot: A base library to connect to Classic Servers.
* ClassicBotCoreTests: An implementation of LibClassicBot with a number of commands supported.

LibClassicBot is known to run well on .NET Framework 2.0 with the Microsoft Compiler. Mono has not been tested as of yet.
LibClassicBot is further divided into two namespaces:
* LibClassicBot: A base library to connect to Classic Servers.
* LibClassicBot.Networking: Contains enumerations with the packet IDs of both Server and Client packets.
Also contains the static class Packets, which can be used to create packets that can be sent via ClassicBot.Send(byte []).
* LibClassicBot.Events: Events that are raised by the bot, such as socket errors and chat messages.
* LibClassicBot.Remote: Contains classes and events for creating a server that remote clients are able to connect to.
The remote protocol used also has the option of sending log files across.

## Features

LibClassicBot is incomplete. Work on further additions are currently at a standstill due to
schoolwork. However, I decided that I might as well release the source code of the bot 
as it currently stands.

Some things about LibClassicBot that are already implemented:

* **Plugins:** *Plugins are designed to be easy to implement, although at the moment they can only
be added internally. I intend to add the ability to load .dll plugins later.*
* **Permissions:** *The bot is designed to use a simple permissions system - Operators who are allowed
to use the bot, and those who are not.*

## Usage

LibClassicBot is a library, and will not perform as a bot on its own. You can implement the library yourself, or you can
modify the existing ClassicBotCoreTest. To get started, the following code will create a bot that connects to a locally hosted
server on port 25565, and create a command that is triggered when someone types .test and is on the operators list.:

```csharp
using System;
using System.Net;
using LibClassicBot;
using LibClassicBot.Events;

namespace LibClassicBotTest
{
	class Program
	{
		public static void Main(string[] args)
		{
			ClassicBot Bot1 = new ClassicBot("Bot",IPAddress.Loopback,25565,"operators.txt");	
			ClassicBot.CommandDelegate TestCommand = delegate(string Line)
			{
				Bot1.SendMessagePacket("Test.");
			};
			Bot1.RegisteredCommands.Add("test",TestCommand);
			Bot1.Start(false);
			Bot1.Events.ChatMessage += ChatMessage;
			Console.ReadLine();
		}
		static void ChatMessage(object sender, MessageEventArgs e)
		{
			Console.WriteLine(e.Line);
		}		
	}
}
```

This bot will connect to a locally hosted on port 25565, using the username Bot. Note that the bot will not connect in a verified state,
and may be kicked if verification is turned to on. This bot will also send "Test." whenever a user on the operators list starts
a chat message with the word ".test". The bot also used the ChatMessage event to display every chat line it receives. (Including
colour codes).

The ClassicBot class is the core bot class. It handles plugins, events, networking, and more. 
Each bot must have a username and a path for it to store information regarding operators.


## Licensing

LibClassicBot is dual licensed under  the [MIT license](http://www.opensource.org/licenses/mit-license.php/) and the [GPL license](http://www.gnu.org/licenses/). 
All code except for DrawImg.cs is usable under the MIT license. The only restricted file is the aforementioned, and must be removed if you use this bot
with any other license asides from the GPL.

Additionally, LibClassicBot uses the StripColors method from fCraft, as well as a cut down version of the [map](http://svn.fcraft.net:8080/svn/fcraft/branch-0.64x/fCraft/MapConversion/MapFCMv3.cs) file. fCraft is also released under the MIT license, licensing information
regarding fCraft is avaliable [here](http://www.fcraft.net/wiki/Licensing).

LibClassicBot also uses a LoginClient based on code from [here](http://manicdigger.git.sourceforge.net/git/gitweb.cgi?p=manicdigger/manicdigger;a=commitdiff_plain;h=f6ad911). ManicDigger is released under the Public Domain license, as started on [this](http://manicdigger.sourceforge.net/wiki/index.php/Credits) page.

LibClassicBot's image drawing function is based on code from [here](https://github.com/GlennMR/800craft/blob/master/fCraft/Drawing/DrawOps/DrawImageOperation.cs). 800Craft is released under the GPL 3 license.

[Minecraft](http://minecraft.net) is not officially affiliated with LibClassicBot, nor any of the aforementioned projects.