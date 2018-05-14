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
	    private file_client()
	    {
	    	// TO DO Your own code
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
		private void receiveFile (String fileName, Transport transport)
		{
			// TO DO Your own code
			byte[] buffer = new byte[85];

			transport.send(Encoding.ASCII.GetBytes(fileName), fileName.Length);

			int size = transport.receive (ref buffer);
			byte[] fileBuffer = new byte[int.Parse (Encoding.ASCII.GetString (buffer))];
			size = transport.receive (ref fileBuffer);
			var str = Encoding.ASCII.GetString (fileBuffer);
			var bytes = Encoding.UTF8.GetBytes (str);
			//File.WriteAllBytes (fileName, buffer);
			SaveToBinaryFile (fileName, bytes);
		}


		/// <summary>
		/// Saves to binary file. https://stackoverflow.com/questions/10337410/saving-data-to-a-file-in-c-sharp
		/// </summary>
		/// <param name="filePath">File path.</param>
		/// <param name="data">Data.</param>
		private void SaveToBinaryFile(string filePath, byte[] data)
		{
			using (FileStream stream = File.OpenWrite(filePath))
				{
				stream.Write (data, 0, data.Length);
				stream.Close ();
				}
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
			file_client client = new file_client ();
			Transport transport = new Transport (BUFSIZE);
			string fileName = "PlainText.txt";
			client.receiveFile (fileName, transport);
						
			//byte[] buffer = new byte[50];
			//var trans = new Transport (1000);
			//int size = 0;

			//for (int i = 0; i < 10; i++) {
			//	size = trans.receive (ref buffer);
			//	Console.WriteLine (Encoding.ASCII.GetString(buffer, 0, size));
			//}

			//for (int i = 0; i < 10; i++) {
			//	trans.send (buffer, size);
			//}

		}
	}
}