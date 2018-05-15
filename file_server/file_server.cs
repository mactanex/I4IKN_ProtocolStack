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
			int size = 0;
			while (true) {
				
				while((size = transport.receive (ref buffer)) == 0)
				{};


				if (size != 0) {
					string filePath = Encoding.UTF8.GetString (buffer, 0, size);

					string tmp = Path.GetFullPath ("File_Server_Home/"+filePath);
					sendFile (tmp,transport);

				}
				transport = new Transport (BUFSIZE);
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
		private void sendFile(string filePath, Transport transport)
		{
			// TO DO Your own code
			//string fileName = LIB.extractFileName(@filePath);
			//string currentDirectory = Directory.GetCurrentDirectory ();
			//filePath = currentDirectory + "/" + "PlainText.txt";
			if (LIB.check_File_Exists(@filePath) == 0 )
				return;

			Console.WriteLine ("Sending file: " + filePath + " to client");
			FileStream fileStream = File.Open (@filePath, FileMode.Open);

			long totalLength = fileStream.Length;
			long currentPacketLength = 0;
			int nOfChunks = Convert.ToInt32(Math.Ceiling (Convert.ToDouble(totalLength) / Convert.ToDouble(BUFSIZE)));

			byte[] lengthToSend = Encoding.UTF8.GetBytes (totalLength.ToString ());
			var tmp = lengthToSend.Length;
			transport.send (lengthToSend,lengthToSend.Length);

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
				Console.WriteLine ("The requested file was sent");
			}
			fileStream.Close ();
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
			Console.WriteLine (APP);
			file_server server = new file_server ();
		}
	}
}