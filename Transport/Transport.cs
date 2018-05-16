using System;
using Linklaget;

/// <summary>
/// Transport.
/// </summary>
namespace Transportlaget
{
	/// <summary>
	/// Transport.
	/// </summary>
	public class Transport
	{
		/// <summary>
		/// The link.
		/// </summary>
		private Link link;
		/// <summary>
		/// The 1' complements checksum.
		/// </summary>
		private Checksum checksum;
		/// <summary>
		/// The buffer.
		/// </summary>
		private byte[] buffer;
		/// <summary>
		/// The seq no.
		/// </summary>
		private byte seqNo;
		/// <summary>
		/// The old_seq no.
		/// </summary>
		private byte old_seqNo;
		/// <summary>
		/// The error count.
		/// </summary>
		private int errorCount;
		/// <summary>
		/// The DEFAULT_SEQNO.
		/// </summary>
		private const int DEFAULT_SEQNO = 2;
		/// <summary>
		/// The data received. True = received data in receiveAck, False = not received data in receiveAck
		/// </summary>
		private bool dataReceived;
		/// <summary>
		/// The number of data the recveived.
		/// </summary>
		private int recvSize = 0;

		private int noiseSimulation = 0;

		/// <summary>
		/// Initializes a new instance of the <see cref="Transport"/> class.
		/// </summary>
		public Transport (int BUFSIZE)
		{
			link = new Link(BUFSIZE+(int)TransSize.ACKSIZE);
			checksum = new Checksum();
			buffer = new byte[BUFSIZE+(int)TransSize.ACKSIZE];
			seqNo = 0;
			old_seqNo = DEFAULT_SEQNO;
			errorCount = 0;
			dataReceived = false;
		}

		/// <summary>
		/// Receives the ack.
		/// </summary>
		/// <returns>
		/// The ack.
		/// </returns>
		private byte receiveAck()
		{
            byte[] buf = new byte[(int)TransSize.ACKSIZE];
            int size = link.receive(ref buf);

            if (size != (int)TransSize.ACKSIZE) return DEFAULT_SEQNO;

            if(!checksum.checkChecksum(buf, (int)TransSize.ACKSIZE) || 
				buf[(int)TransCHKSUM.SEQNO] != seqNo || buf[(int)TransCHKSUM.TYPE] != (int)TransType.ACK)
            return DEFAULT_SEQNO;

            return seqNo;
		}

		/// <summary>
		/// Sends the ack.
		/// </summary>
		/// <param name='ackType'>
		/// Ack type.
		/// </param>
		private void sendAck (bool ackType)
		{
			byte[] ackBuf = new byte[(int)TransSize.ACKSIZE];
			ackBuf [(int)TransCHKSUM.SEQNO] = (byte)
				(ackType ? (byte)buffer [(int)TransCHKSUM.SEQNO] : (byte)(buffer [(int)TransCHKSUM.SEQNO] + 1) % 2);
			ackBuf [(int)TransCHKSUM.TYPE] = (byte)(int)TransType.ACK;
			checksum.calcChecksum (ref ackBuf, (int)TransSize.ACKSIZE);

			if (++noiseSimulation == 3) {
				ackBuf [0]++;
				noiseSimulation = 0;
			}

			link.send(ackBuf, (int)TransSize.ACKSIZE);
		}

		/// <summary>
		/// Send the specified buffer and size.
		/// </summary>
		/// <param name='buffer'>
		/// Buffer.
		/// </param>
		/// <param name='size'>
		/// Size.
		/// </param>
		public void send(byte[] buf, int size)
		{
            // Construct the package
			buffer[(int)TransCHKSUM.SEQNO] = seqNo;
			buffer[(int)TransCHKSUM.TYPE] = (byte)(int)TransType.DATA;
			for (int i = 0; i < size; i++)
			{
				buffer[i + (int)TransSize.ACKSIZE] = buf[i];
			}
			checksum.calcChecksum(ref buffer, size + (int)TransSize.ACKSIZE);

			// For noice simulation
			if (++noiseSimulation == 3) {
				buffer [0]++;
				noiseSimulation = 0;
			}

			// Send it
			if (!SendLink (ref buffer, size + (int)TransSize.ACKSIZE))
				return; // Timeout occured
			
			// Package received - update seq number
            nextSeqNo();
            old_seqNo = DEFAULT_SEQNO;
		}

		/// <summary>
		/// Receive the specified buffer.
		/// </summary>
		/// <param name='buffer'>
		/// Buffer.
		/// </param>
		public int receive (ref byte[] buf)
		{
			// TO DO Your own code
			int size = ReadLink(ref buffer);
			if (size == 0)
				return 0; // Nothing read

			while (!checksum.checkChecksum (buffer, size) || buffer[(int)TransCHKSUM.SEQNO] != seqNo) {
				sendAck (false);
				size = ReadLink (ref buffer);
				if (size == 0)
					return 0; // Nothing read
			}

			sendAck (true);
			nextSeqNo ();
			size = buf.Length < size-(int)TransSize.ACKSIZE ? buf.Length : size-(int)TransSize.ACKSIZE;

			Array.Copy (buffer,(int)TransSize.ACKSIZE,buf,0, size);

			return size;
		}

		private void nextSeqNo()
        {
            seqNo = (byte)((seqNo + 1) % 2);
        }

		private bool SendLink(ref byte[] buff, int size)
		{
			while (errorCount < 5) {
				try{
					do
					{              
						link.send(buff, size);
					} while (receiveAck() != seqNo);
					break;
				}catch(TimeoutException) {
					errorCount++;
					if (errorCount == 5) {
						errorCount = 0;
						return false;
					}
				}
			}
			return true;
		}

		private int ReadLink(ref byte[] buff)
		{
			int size = 0;
			while (errorCount < 5) {
				try
				{
					size = link.receive(ref buff);
					break;
				}catch(TimeoutException) {
					errorCount++;
					if (errorCount == 5) {
						errorCount = 0;
						return 0;
					}
				}
			}
			errorCount = 0;
			return size;
		}

	}


}