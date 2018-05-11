using System;
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
	    private file_client(String[] args)
	    {
            string file = args[1];


            System.Net.Sockets.TcpClient ClientSocket = new System.Net.Sockets.TcpClient();
            ClientSocket.Connect(args[0], PORT);
            ClientSocket.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            NetworkStream ServerStream = ClientSocket.GetStream();
            LIB.writeTextTCP(ServerStream, file);
            ServerStream.Flush();
            string fileName = LIB.extractFileName(file);

            receiveFile(fileName, ServerStream);

            ServerStream.Close();
            ClientSocket.Dispose();
            ClientSocket.Close();
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
            Byte[] instream = new byte[BUFSIZE];
            int receivedBytes;
            int totalRecBytes = 0;
            //int NoOfPackets = Convert.ToInt32 (Math.Ceiling (Convert.ToDouble (fileSize) / Convert.ToDouble (BUFSIZE)));
            //for(){};

            //io.Read (instream, 0, BUFSIZE);
            //string file = System.Text.Encoding.ASCII.GetString (instream);

            FileStream newFile = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write);
            while ((receivedBytes = io.Read(instream, 0, instream.Length)) > 0)
            {
                newFile.Write(instream, 0, receivedBytes);
                totalRecBytes += receivedBytes;
            }
            newFile.Close();

            Console.WriteLine(fileName);

            io.Close();

        }

        /// <summary>
        /// The entry point of the program, where the program control starts and ends.
        /// </summary>
        /// <param name='args'>
        /// First argument: Filname
        /// </param>
        public static void Main (string[] args)
		{
			byte[] buffer = new byte[50];
			var trans = new Transport (1000);
			trans.receive (ref buffer);
			Console.WriteLine (Encoding.ASCII.GetString (buffer));
		}
	}
}