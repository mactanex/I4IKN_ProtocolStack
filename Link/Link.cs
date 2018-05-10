using System;
using System.IO.Ports;

/// <summary>
/// Link.
/// </summary>
namespace Linklaget
{
	/// <summary>
	/// Link.
	/// </summary>
	public class Link
	{
		/// <summary>
		/// The DELIMITE for slip protocol.
		/// </summary>
		const byte DELIMITER = (byte)'A';
		/// <summary>
		/// The buffer for link.
		/// </summary>
		private byte[] buffer;
		/// <summary>
		/// The serial port.
		/// </summary>
		SerialPort serialPort;

		/// <summary>
		/// Initializes a new instance of the <see cref="link"/> class.
		/// </summary>
		public Link (int BUFSIZE)
		{
			// Create a new SerialPort object with default settings.

			serialPort = new SerialPort("/dev/ttyS1",115200,Parity.None,8,StopBits.One);

			if(!serialPort.IsOpen)
				serialPort.Open();

			buffer = new byte[(BUFSIZE*2)];

			// Uncomment the next line to use timeout
			//serialPort.ReadTimeout = 500;

			serialPort.DiscardInBuffer ();
			serialPort.DiscardOutBuffer ();
		}

		/// <summary>
		/// Send the specified buf and size.
		/// </summary>
		/// <param name='buf'>
		/// Buffer.
		/// </param>
		/// <param name='size'>
		/// Size.
		/// </param>
		public void send (byte[] buf, int size)
		{
			var byteCount = Frame (buf, size);
			serialPort.Write(buffer, 0, byteCount);
	    	// TO DO Your own code
		}

		/// <summary>
		/// Receive the specified buf and size.
		/// </summary>
		/// <param name='buf'>
		/// Buffer.
		/// </param>
		/// <param name='size'>
		/// Size.
		/// </param>
		public int receive (ref byte[] buf)
		{
			
			byte received;
			int delimiterCount = 0;
			int counter = 0;
			while(delimiterCount < 2)
			{
				if ((received = (byte)serialPort.ReadByte()) == (byte)'A')
					delimiterCount++;
				buffer [counter++] = received;
			}

			int inserted = 0;
			for (int i = 1; i < counter-1; i++) {
				if (buffer [i] == (byte)'B') {
					if (buffer [i + 1] == (byte)'C') {
						buf [inserted++] = (byte)'A';
						i++;
					} else if (buffer [i + 1] == (byte)'D') {
						buf [inserted++] = (byte)'B';
						i++;
					}
				} else {
					buf [inserted++] = buffer [i];				
				}
			}

			// TO DO Your own code
			return inserted;
		}

		private int Frame(byte[] buf, int size)
		{
			var counter = 0;
			var inserted = 0;

			buffer [inserted++] = DELIMITER;
		
			while (counter < size) {
				if (buf [counter] == (byte)'A') {
					buffer [inserted++] = (byte)'B';
					buffer [inserted++] = (byte)'C';				
				} else if (buf [counter] == (byte)'B') {
					buffer [inserted++] = (byte)'B';
					buffer [inserted++] = (byte)'D';				
				} else {
					buffer [inserted++] = buf [counter];
				}
				counter++;
			}

			buffer [inserted++] = DELIMITER;
			return inserted;
		}
			
	}
}
