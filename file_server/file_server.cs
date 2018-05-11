using System;
using System.IO;
using System.Text;
using Transportlaget;
using Library;
using System.Net.Sockets;
using System.Net;

namespace Application
{
	class file_server
	{
		/// <summary>
		/// The BUFSIZE
		/// </summary>
		private const int BUFSIZE = 1000;
        const int PORT = 9000;
        private const string APP = "FILE_SERVER";

		/// <summary>
		/// Initializes a new instance of the <see cref="file_server"/> class.
		/// </summary>
		private file_server ()
		{
            TcpListener serverSocket = new TcpListener(IPAddress.Parse("10.0.0.1"), PORT);
            serverSocket.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            int requestCount = 0;
            TcpClient clientSocket = default(TcpClient);
            serverSocket.Start();
            Console.WriteLine("Server started...");
            clientSocket = serverSocket.AcceptTcpClient();
            Console.WriteLine("Accept Connoction from Client");
            requestCount = 0;


            try
            {
                requestCount = requestCount + 1;
                NetworkStream networkStream = clientSocket.GetStream();
                byte[] inputBuffer = new byte[BUFSIZE];
                networkStream.Read(inputBuffer, 0, BUFSIZE);
                string dataFromClient = System.Text.Encoding.ASCII.GetString(inputBuffer);
                dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));
                Console.WriteLine("data from Console: " + dataFromClient);
                sendFile(dataFromClient, dataFromClient.Length, networkStream);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            clientSocket.Close();
            serverSocket.Stop();
            Console.WriteLine("exit");
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

            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);


            int NoOfPackets = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(fileSize) / Convert.ToDouble(BUFSIZE)));
            long TotalLength = fileSize;
            long CurrentPacketLength = 0;
            for (int i = 0; i < NoOfPackets; i++)
            {
                if (TotalLength > BUFSIZE)
                {
                    CurrentPacketLength = BUFSIZE;
                    TotalLength = TotalLength - CurrentPacketLength;
                }
                else
                {
                    CurrentPacketLength = TotalLength;


                }
                byte[] sendingbuffer = new byte[CurrentPacketLength];
                fs.Read(sendingbuffer, 0, (int)CurrentPacketLength);
                io.Write(sendingbuffer, 0, sendingbuffer.Length);
                var value = sendingbuffer.Length.ToString();
                Console.WriteLine("send" + value);

            }

            fs.Close();

            io.Flush();
            io.Close();
            Console.WriteLine();
        }

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name='args'>
		/// The command-line arguments.
		/// </param>
		public static void Main (string[] args)
		{
			var trans = new Transport (1000);
			var hej = "Bent vil gerne hAve kAge";
			trans.send (Encoding.ASCII.GetBytes(hej), hej.Length);
		}
	}
}