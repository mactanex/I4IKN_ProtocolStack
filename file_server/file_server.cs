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
		private void sendFile(String fileName, long fileSize, Transport transport)
		{
			// TO DO Your own code
		}

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name='args'>
		/// The command-line arguments.
		/// </param>
		public static void Main (string[] args)
		{
			var buffer = new byte[50];
			var trans = new Transport (1000);
			for (int i = 0; i < 10; i++) {
				var hej = "Bent vil gerne hAve kAge";
				trans.send (Encoding.ASCII.GetBytes(hej), hej.Length);
			}

			int size;

			for (int i = 0; i < 10; i++) {
				size = trans.receive (ref buffer);
				Console.WriteLine (Encoding.ASCII.GetString(buffer, 0, size));
			}
		}
	}
}