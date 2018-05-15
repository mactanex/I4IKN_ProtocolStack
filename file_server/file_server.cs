using System;
using System.IO;
using System.Text;
using Transportlaget;
using Library;

namespace Application
{
	class file_server
	{
		/// <summary>
		/// The BUFSIZE
		/// </summary>
		private const int BUFSIZE = 1000;
		private const string APP = "FILE_SERVER";


		/// <summary>
		/// Initializes a new instance of the <see cref="file_server"/> class.
		/// </summary>
		private file_server ()
		{
			Transport transport = new Transport (BUFSIZE);
			byte[] buffer = new byte[BUFSIZE];

			while (true) {
				int size = transport.receive (ref buffer);

				if (size != 0) {
					string fileName = Encoding.ASCII.GetString (buffer, 0, size);


					sendFile (fileName,transport);
				}
			}
		}

		/// <summary>
		/// Sends the file.
		/// </summary>
		/// <param name='fileName'>
		/// File name.
		/// </param>
		/// <param name='fileSize'>
		/// File size.
		/// </param>
		/// <param name='tl'>
		/// Tl.
		/// </param>
		private void sendFile(string fileName, Transport transport)
		{
			// TO DO Your own code
			string path = @"/home/ikn/I4IKN_ProtocolStack/" + fileName;
			FileStream fileStream = File.Open (path, FileMode.Open, FileAccess.Read);

			long totalLength = fileStream.Length;
			long currentPacketLength = 0;
			int nOfChunks = Convert.ToInt32(Math.Round (Convert.ToDouble(totalLength) / Convert.ToDouble(BUFSIZE)));


			transport.send (Encoding.ASCII.GetBytes(totalLength.ToString()),totalLength.ToString().Length);

			if (totalLength != 0) {
				for (int i = 0; i < nOfChunks; i++) {
					if (totalLength > BUFSIZE) {
						currentPacketLength = BUFSIZE;
					} else {
						currentPacketLength = totalLength;
					}
					totalLength -= currentPacketLength;
				
					byte[] fileBuffer = new byte[currentPacketLength];
					ReadChunk (fileStream, ref fileBuffer);

					transport.send (fileBuffer, (int)currentPacketLength);

				}
			}
		}


		/// <summary>
		/// Reads the chunk.  https://stackoverflow.com/questions/5659189/how-to-split-a-large-file-into-chunks-in-c
		/// </summary>
		/// <returns>index of the chunk</returns>
		/// <param name="stream">Stream.</param>
		/// <param name="chunk">Chunk.</param>
		private int ReadChunk(FileStream stream, ref byte[] chunk)
		{
			int index = 0;
			while (index < chunk.Length) {
				int bytesRead = stream.Read (chunk, index, chunk.Length - index);
				if (bytesRead == 0) {
					break;
				}
				index += bytesRead;
			}
			return index;
		}


		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name='args'>
		/// The command-line arguments.
		/// </param>
		public static void Main (string[] args)
		{
			file_server server = new file_server ();
		}
	}
}