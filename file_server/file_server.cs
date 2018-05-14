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
		private void sendFile(String fileName)
		{
			// TO DO Your own code
			byte[] fileBuffer = new byte[BUFSIZE];
			ReadFile (fileBuffer,fileName);
			var trans = new Transport (BUFSIZE);
			trans.send (fileBuffer, fileBuffer.Length);
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
		private int ReadFile(byte[] fileBuffer, string fileName)
		{
			string path = @"/root/Desktop/Projects/Exercise_11_file_server/I4IKN_ProtocolStack/" + fileName;
			FileStream fileStream = File.Open (path, FileMode.Open, FileAccess.Read);
			fileBuffer = new byte[fileStream.Length];
			fileStream.Read (fileBuffer, 0, (int)fileStream.Length);
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
			server.sendFile ("PlainText.txt");


			//var buffer = new byte[50];
			//var trans = new Transport (BUFSIZE);
			//for (int i = 0; i < 10; i++) {
			//	var hej = "Bent vil gerne hAve kAge";
			//	trans.send (Encoding.ASCII.GetBytes(hej), hej.Length);
			//}

			//int size;

			//for (int i = 0; i < 10; i++) {
			//	size = trans.receive (ref buffer);
			//	Console.WriteLine (Encoding.ASCII.GetString(buffer, 0, size));
			//}
		}
	}
}