using System;
using System.Collections.Generic;
using System.Text;
using System.Security;
using System.Security.Cryptography;
using System.IO;

namespace ClassicBotCore
{
	public partial class RemoteBotEngine
	{
		public static class Security
		{

			/// <summary>Used by an encrypted server and client connection.<br/>
			/// Note that you will need to generate a Symmetric Key and IV first.</summary>
			public class SymmetricStream : Stream
			{
				/// <summary>Creates a SymmetricStream from the given two CryptoStreams.</summary>
				/// <param name="Read">The reading stream is used for decrypting encrypted data.</param>
				/// <param name="Write">The writing stream is used for writing encrypted data.</param>
				public SymmetricStream(CryptoStream Read, CryptoStream Write)
				{
					ReadStream = Read;
					WriteStream = Read;
				}

				/// <summary>Creates a SymmetricStream for reading and writing to the specified stream
				/// with the specified Symmetric Key and Symmetric IV.</summary>
				/// <param name="stream">The stream for which the SymmetricStream will be reading and writing from.</param>
				/// <param name="SymmetricKey">The Symmetric Key to use to create the SymmetricStream.</param>
				/// <param name="SymmetricIV">The Symmetric Initilization Vector to use to create the SymmetricStream.</param>
				public SymmetricStream (Stream stream, byte[] SymmetricKey, byte[] SymmetricIV)
				{
					SymmetricAlgorithm symConnection = SymmetricAlgorithm.Create();
					symConnection.Key = SymmetricKey;
					symConnection.IV = SymmetricIV;
					WriteStream = new CryptoStream(stream, symConnection.CreateEncryptor(), CryptoStreamMode.Write);
					ReadStream = new CryptoStream(stream, symConnection.CreateDecryptor(), CryptoStreamMode.Read);
				}

				/// <summary>The CryptoStream used for reading encrypted data with a Symmetric Key.</summary>
				public CryptoStream ReadStream;

				/// <summary>The CryptoStream used for reading encrypted data with a Symmetric Key.</summary>
				public CryptoStream WriteStream;
				
				/// <summary>Gets a value indicating whether you can read within the current SymmetricStream.</summary>
				/// <returns>True if the ReadStream supports reading.</returns>
				public override bool CanRead {
					get { return ReadStream.CanRead; } //CryptoStream doesn't support reading.
				}

				/// <summary>Gets a value indicating whether you can seek within the current SymmetricStream.</summary>
				/// <returns>Always false, as a CryptoStream does not support seeking.</returns>
				public override bool CanSeek {
					get { return false; } //CryptoStream doesn't support reading.
				}
				
				/// <summary>Gets a value indicating whether you can read within the current SymmetricStream.</summary>
				/// <returns>True if the ReadStream supports reading.</returns>
				public override bool CanWrite {
					get { return WriteStream.CanWrite; } //CryptoStream doesn't support reading.
				}
				
				public override void Flush() {}
				

				/// <summary>Gets a value indicating the length of the current SymmetricStream.</summary>
				/// <returns>Always throws an exception, as CryptoStream does not support seeking.</returns>
				public override long Length {
					get { throw new NotSupportedException("Stream does not support seeking.");}
				}
				
				/// <summary>Gets a value indicating the current position within the current SymmetricStream, or sets it.</summary>
				/// <returns>Always throws an exception, as CryptoStream does not support seeking.</returns>
				public override long Position
				{
					get { throw new NotSupportedException("Stream does not support seeking."); }
					set { throw new NotSupportedException("Stream does not support seeking."); }
				}

				/// <summary>Sets the position within the current SymmetricStream to the specified value.</summary>
				/// <returns>Always throws an exception, as CryptoStream does not support seeking.</returns>
				public override long Seek(long offset, SeekOrigin origin)
				{
					throw new NotSupportedException("Stream does not support seeking.");
				}

				/// <summary>Sets the length of the current SymmetricStream.</summary>
				/// <returns>Always throws an exception, as CryptoStream does not support seeking.</returns>
				public override void SetLength(long value)
				{
					throw new NotSupportedException("Stream does not support seeking.");
				}
				
				
				/// <summary>Writes encrypted data using the given buffer, with the given offset, and writes the given number of bytes.</summary>
				public override void Write(byte[] buffer, int offset, int count)
				{
					int OriginalBufferLength = 0;
					if (buffer.Length + 2 < 32) { OriginalBufferLength = 32 - (buffer.Length + 2); }
					else { OriginalBufferLength = (buffer.Length + 2) % 32; }
					Console.WriteLine(OriginalBufferLength);
					byte[] OutBuffer = new byte[2 + buffer.Length]; //The two bytes will be where we place a short indicating the length.
					Array.Copy(buffer, 0, OutBuffer, 2, buffer.Length);
					if (OriginalBufferLength > 0)
					{
						int Length = OutBuffer.Length;
						Array.Resize(ref OutBuffer, OutBuffer.Length + OriginalBufferLength);
						for (int i = Length + 1; i < OutBuffer.Length; i++)
						{
							OutBuffer[i] = 0;
						}
					}
					WriteStream.Write(OutBuffer, offset, OutBuffer.Length);
				}

				/// <summary>Writes an encrypted string. The output string is prefixed by an Int32 determing the length.</summary>
				public void WriteString(string data)
				{
					WriteStream.Write(Encoding.UTF8.GetBytes(data), 0, data.Length);
				}

				/// <summary>Reads and decrypts sequence of bytes from the given buffer.</summary>
				/// <returns>The number of bytes read from the stream.</returns>
				public override int Read(byte[] buffer, int offset, int count)
				{
					return ReadStream.Read(buffer, offset, count);
				}

				/// <summary>Reads and decrypts sequence of bytes from the given buffer.</summary>
				/// <returns>The number of bytes read from the stream.</returns>
				public string ReadString()
				{
					byte[] buffer = new byte[4];
					ReadStream.Read(buffer, 0, 4);
					int StringLength = BitConverter.ToInt32(buffer, 0);
					byte[] stringBuffer = new byte[StringLength];
					ReadStream.Read(stringBuffer, 0, StringLength);
					return Encoding.UTF8.GetString(stringBuffer);
				}

			}
			public static class AsymmetricEncryption
			{
				const int keySize = 1024;

				const int MaxLength = 117;

				public static void GenerateKeys(out string publicKey, out string publicAndPrivateKey)
				{
					using (RSACryptoServiceProvider provider = new RSACryptoServiceProvider(keySize))
					{
						publicKey = provider.ToXmlString(false);
						publicAndPrivateKey = provider.ToXmlString(true);
					}
				}

				public static byte[] Encrypt(byte[] data, string publicKeyXml)
				{
					if (data == null || data.Length == 0) throw new ArgumentException("Data is empty", "data");
					if (data.Length > MaxLength) throw new ArgumentException(String.Format("Maximum data length is {0}", MaxLength), "data");
					if (String.IsNullOrEmpty(publicKeyXml)) throw new ArgumentException("Key is null or empty", "publicKeyXml");

					using (RSACryptoServiceProvider provider = new RSACryptoServiceProvider(keySize))
					{
						provider.FromXmlString(publicKeyXml);
						return provider.Encrypt(data, false);
					}
				}

				public static byte[] Decrypt(byte[] data, string publicAndPrivateKeyXml)
				{
					if (data == null || data.Length == 0) throw new ArgumentException("Data is empty", "data");
					if (String.IsNullOrEmpty(publicAndPrivateKeyXml)) throw new ArgumentException("Key is null or empty", "publicAndPrivateKeyXml");

					using (RSACryptoServiceProvider provider = new RSACryptoServiceProvider(keySize))
					{
						provider.FromXmlString(publicAndPrivateKeyXml);
						return provider.Decrypt(data, false);
					}
				}
			}
		}
	}

}
