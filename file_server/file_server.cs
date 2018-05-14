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
			// TO DO Your own code
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
		private void sendFile(String fileName, Transport transport)
		{
			// TO DO Your own code
			byte[] fileBuffer = new byte[BUFSIZE];
			int len = ReadFile (ref fileBuffer, fileName);
			if (len != 0) {
				transport.send (Encoding.ASCII.GetBytes(len.ToString()),len.ToString().Length);
				transport.send (fileBuffer, len);
			}
		}


		/// <summary>
		/// Reads the chunk.  https://stackoverflow.com/questions/5659189/how-to-split-a-large-file-into-chunks-in-c
		/// </summary>
		/// <returns>index of the chunk</returns>
		/// <param name="stream">Stream.</param>
		/// <param name="chunk">Chunk.</param>
		private int ReadChunk(FileStream stream, byte[] chunk)
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
		/// Reads the file. 
		/// </summary>
		/// <returns>length of file</returns>
		private int ReadFile(ref byte[] fileBuffer, string fileName)
		{
			string path = @"/home/ikn/I4IKN_ProtocolStack/" + fileName;
			FileStream fileStream = File.Open (path, FileMode.Open, FileAccess.Read);
			fileBuffer = new byte[fileStream.Length];
			fileStream.Read (fileBuffer, 0, (int)fileStream.Length);
			fileStream.Close ();
			return fileBuffer.Length;
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
			Transport transport = new Transport (BUFSIZE);
			byte[] buffer = new byte[BUFSIZE];

			while (true) {
				int size = transport.receive (ref buffer);

				if (size != 0) {
					string fileName = Encoding.ASCII.GetString (buffer, 0, size);


					server.sendFile (fileName,transport);
				}
			}

			


			//var buffer = new byte[50];
			//var trans = new Transport (BUFSIZE);
			//for (int i = 0; i < 10; i++) {
			//	var hej = "Bent vil gerne hAve kAge";
			//	trans.send (c, hej.Length);
			//}

			//int size;

			//for (int i = 0; i < 10; i++) {
			//	size = trans.receive (ref buffer);
			//	Console.WriteLine (Encoding.ASCII.GetString(buffer, 0, size));
			//}
		}
	}
}