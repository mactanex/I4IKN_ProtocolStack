using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Text;
using Transportlaget;
using Library;

namespace Application
{
	class file_client
	{
		/// <summary>
		/// The BUFSIZE.
		/// </summary>
		private const int BUFSIZE = 1000;
		private const string APP = "FILE_CLIENT";

		/// <summary>
		/// Initializes a new instance of the <see cref="file_client"/> class.
		/// 
		/// file_client metoden opretter en peer-to-peer forbindelse
		/// Sender en forspÃ¸rgsel for en bestemt fil om denne findes pÃ¥ serveren
		/// Modtager filen hvis denne findes eller en besked om at den ikke findes (jvf. protokol beskrivelse)
		/// Lukker alle streams og den modtagede fil
		/// Udskriver en fejl-meddelelse hvis ikke antal argumenter er rigtige
		/// </summary>
		/// <param name='args'>
		/// Filnavn med evtuelle sti.
		/// </param>
		private file_client(string[] args)
	    {
			if (args.Length == 1) {
				Transport transport = new Transport (BUFSIZE);
				byte[] buffer = new byte[BUFSIZE];
				string filePath = args [0];
				string fileName = LIB.extractFileName (filePath);

				transport.send (Encoding.ASCII.GetBytes (filePath), filePath.Length);
				int size = transport.receive (ref buffer);
				int fileSize = int.Parse (Encoding.ASCII.GetString (buffer, size));

				if ( fileSize > 0)
					receiveFile (fileName, fileSize, transport);
				else
					Console.WriteLine ("The file does not exist on the server.");
			}
			else
				Console.WriteLine ("The arguments provided does not match : ", args.Length);
	    }

		/// <summary>
		/// Receives the file.
		/// </summary>
		/// <param name='fileName'>
		/// File name.
		/// </param>
		/// <param name='transport'>
		/// Transportlaget
		/// </param>
		private void receiveFile (String fileName, long fileSize,Transport transport)
		{
			// TO DO Your own code
			byte[] fileBuffer = new byte[BUFSIZE];

			var readSize = 0;
			var offset = 0;

			FileStream newFile = new FileStream (fileName, FileMode.OpenOrCreate, FileAccess.Write);
			while(readSize = transport.receive(fileBuffer) > 0)
			{
				newFile.Write (fileBuffer, offset, readSize);
				offset += readSize;
			}

			newFile.Close ();
		}


		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name='args'>
		/// First argument: Filname
		/// </param>
		public static void Main (string[] args)
		{
			Console.WriteLine (APP);
			Console.Write(args.Length);
			file_client client = new file_client (args);

		}
	}
}