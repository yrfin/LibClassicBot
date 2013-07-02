using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using LibClassicBot.Events;

namespace LibClassicBot.Remote
{
		/// <summary>Represents a remotely connected user.</summary>
		public class RemoteClient
		{
			/// <summary>Sends a message to the remote client.</summary>
			/// <param name="message">Message to send to the client.</param>
			/// <returns>True if the data was sent, false if an error occurred during sending the data.
			/// (This does not guarantee the client received the data though)</returns>
			public bool MessageClient(string message)
			{
				if(writer == null) return false;
				try
				{
					writer.Write((byte)RemoteProtocol.OutgoingMessage);
					writer.Write(message);
					return true;
				}
				
				catch(IOException)
				{
					if(rTcpClient != null) rTcpClient.Close();
					return false;
				}
			}
			
			/// <summary>BinaryWriter for reading from the remote client. Usually you should not need to use this.</summary>
			public BinaryWriter writer;
			
			/// <summary>BinaryReader for reading from the remote client. Usually you should not need to use this.</summary>
			public BinaryReader reader;
			
			/// <summary>Bool which determines if the user has been authorised and entered their password correctly.</summary>
			public bool Authorised = false;
			
			/// <summary>The username of the remote client. Starts of as being null.</summary>
			public string Username;
			
			private TcpClient rTcpClient;
			
			
			public void Start(TcpClient RClient, Server server)
			{
				//Test
				rTcpClient = RClient;

				reader = new BinaryReader(rTcpClient.GetStream());
				writer = new BinaryWriter(rTcpClient.GetStream());
				int failedTries = 0;
				byte[] OpCodePacket= new byte[1];
				int bytesRead;
				
				while (true)
				{
					bytesRead = 0;
					
					try
					{
						//blocks until a client sends a message
						bytesRead = rTcpClient.Client.Receive(OpCodePacket,1,SocketFlags.None);
						switch((RemoteProtocol)OpCodePacket[0])
						{
							case RemoteProtocol.Username: //Username
								{
									Username = reader.ReadString();
									writer.Write((byte)RemoteProtocol.ServerMessage);
									writer.Write((byte)3);
									writer.Write("Hello "+Username+". Please enter the password sent to you by the bot owner to use the bot.");
									break;
								}
								
							case RemoteProtocol.Password:
								{
									if(Username == null) throw new SocketException(-1);
									if(failedTries > 3)
									{
										writer.Write((byte)RemoteProtocol.ServerMessage);
										writer.Write((byte)2);
										writer.Write("You have incorrectly guessed the password more than three times. The session has been closed.");
										throw new SocketException(-1);
									}
									
									string Password = reader.ReadString();
									if(server._password != Password)
									{
										writer.Write((byte)RemoteProtocol.ServerMessage);
										writer.Write((byte)4);
										writer.Write("Incorrect password. You have " + (3 - failedTries).ToString() + " more tries.");
										failedTries++;
									}
									
									else
									{
										Authorised = true;
										writer.Write((byte)RemoteProtocol.ClientAuthorised);
										writer.Write("You have now been authenticated");
										RemoteLoginEventArgs e = new RemoteLoginEventArgs(Username,(IPEndPoint)rTcpClient.Client.RemoteEndPoint,rTcpClient);
										server.MinecraftBot.Events.RaiseUserLoggedIn(e);
									}
									break;
									
								}
								
							case RemoteProtocol.IncomingMessage:
								{
									string chat = reader.ReadString();
									if(chat.StartsWith("&"))
									{
										string commandname = chat.TrimStart('&').Split(' ')[0];
										string args = String.Empty;
										try { args  = chat.Substring(commandname.Length + 2); } catch { }//Account for first white variable.
										args.TrimStart(' ');
										writer.Write((byte)RemoteProtocol.ServerResponse);
										
										switch(commandname)
										{
											case "commands":
												{
													writer.Write((byte)0);
													writer.Write("Commands: " +Environment.NewLine +
													             "&commands - Returns a list of commands." + Environment.NewLine +
													             "&players - Returns a list of players on the same map as the bot." + Environment.NewLine +
													             "&listlogs - Returns a list of all log files in the bot directory." + Environment.NewLine +
													             "&getlog - Gets the log file from the specified name.");
													break;
												}
												
											case "players":
												{
													writer.Write((byte)0);
													Dictionary<byte, Player> players = server.MinecraftBot.Players;
													List<string> Output = new List<string>();
													foreach(Player player in players.Values)
													{
														Output.Add(player.Name);
													}
													string[] output = Output.ToArray();
													writer.Write("Players in the current map: " +Environment.NewLine +
													             string.Join(",",output));
												}
												break;
												
											case "listlogs":
												{
													writer.Write((byte)0);
													string[] rawFiles = Directory.GetFiles(Directory.GetCurrentDirectory());
													List<String> Files = new List<string>(rawFiles);
													System.Text.StringBuilder sb = new System.Text.StringBuilder();
													foreach(string file in Files)
													{
														string fileRelative = Path.GetFileName(file);
														if(fileRelative.StartsWith("log-")) sb.Append(fileRelative + Environment.NewLine);
													}
													writer.Write("Log files: " + Environment.NewLine + sb);
													break;
												}
												
											case "getlog":
												{
													string reqFile = Path.Combine(Directory.GetCurrentDirectory(),args);
													string[] rawFiles = Directory.GetFiles(Directory.GetCurrentDirectory());
													List<string> Files = new List<string>(rawFiles);
													if(!Files.Contains(reqFile))
													{
														writer.Write((byte)0);
														writer.Write("No file matching "+args + " found. You must include the extension as well.");
													}
													else
													{
														writer.Write((byte)1);
														writer.Write(Path.GetFileName(reqFile));
														writer.Write(File.ReadAllText(reqFile));
													}
													//writer.Write("Test.");
													
													break;
												}
												
											default:
												{
													writer.Write((byte)0);
													writer.Write("Unknown command: " +chat.TrimStart('&').Split(' ')[0] + ". To see a list of commands, type &commands");
													break;
												}
										}
										break;
										
									} //If the user wishes to do a server command.
									if(Authorised) //Only send if authorised, still read the message otherwise we become out of sync with the client.
									{
										Extensions.StripColors(chat);
										server.MinecraftBot.SendLongChat(chat);
									}
									break;
								}
							default:
								throw new SocketException(-1);
						}
					}
					catch
					{
						//A socket error has occured, don't log why.
						break;
					}
					
					if (bytesRead == 0)
					{
						//The client has disconnected.
						break;
					}
				}
				SessionEndedEventArgs endede = new SessionEndedEventArgs(Username);
				server.MinecraftBot.Events.RaiseSessionEnded(endede);
				if(rTcpClient != null)
				{
					rTcpClient.Client.Disconnect(true);
					rTcpClient.Close();
				}
			}
		}
}