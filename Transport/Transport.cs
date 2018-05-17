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
		/// The _link.
		/// </summary>
		private readonly Link _link;
		/// <summary>
		/// The 1' complements _checksum.
		/// </summary>
		private readonly Checksum _checksum;
		/// <summary>
		/// The _buffer.
		/// </summary>
		private byte[] _buffer;
		/// <summary>
		/// The seq no.
		/// </summary>
	    private SeqNr _sequenceNumber;
		/// <summary>
		/// The error count.
		/// </summary>
		private int _errorCount;
		/// <summary>
		/// The DefaultSeqNr.
		/// </summary>
		private const int DefaultSeqNr = 2;
        /// <summary>
        /// For simulating noise
        /// </summary>
		private int _noiseSimulation = 0;

		/// <summary>
		/// Initializes a new instance of the <see cref="Transport"/> class.
		/// </summary>
		public Transport (int buffSize)
		{
			_link = new Link(buffSize+TransSize.AckSize);
			_checksum = new Checksum();
			_buffer = new byte[buffSize+TransSize.AckSize];
            _sequenceNumber = new SeqNr(0);
			_errorCount = 0;
		}

		/// <summary>
		/// Send the specified _buffer and size.
		/// </summary>
		/// <param name='buf'>
		/// Buffer.
		/// </param>
		/// <param name='size'>
		/// Size.
		/// </param>
		public void Send(byte[] buf, int size)
		{
            // Construct the package
			var packageSize = ConstructPackage(buf, size);

		    while (_errorCount < 5)
		    {
		        try
		        {
		            do
		            {
		                _link.Send(_buffer, packageSize);
		            } while (ReceiveAck() != _sequenceNumber);
		            _sequenceNumber++;
		            break;
		        }
		        catch (TimeoutException)
		        {
		            _errorCount++;
		        }
		    }

		    _errorCount = 0;
		}

	    public int Receive(ref byte[] buff)
	    {
	        var readSize = 0;

	        while (readSize == 0 && _errorCount < 5) 
	        {
	            try
	            {
	                while ((readSize = _link.Receive(ref _buffer)) > 0)
	                {
	                    if (!_checksum.CheckChecksum(_buffer, readSize) || _buffer[TransChecksum.SequenceNumber] != _sequenceNumber)
	                    {
                            CreateAndSendAck(_sequenceNumber.Peak());
	                    }
	                    else
	                    {
                            CreateAndSendAck(_sequenceNumber);
	                        _sequenceNumber++;
	                        readSize = buff.Length < readSize - TransSize.AckSize ? buff.Length : readSize - TransSize.AckSize;
	                        Array.Copy(_buffer, TransSize.AckSize, buff, 0, readSize);
	                        break;
                        }
	                }                   
	            }
	            catch (TimeoutException)
	            {
	                readSize = 0;
                    _errorCount++;
	            }

            }

	        _errorCount = 0;
	        return readSize;
	    }

        #region Utility

	    private int ConstructPackage(byte[] data, int size)
	    {
	        _buffer[TransChecksum.SequenceNumber] = _sequenceNumber;
	        _buffer[TransChecksum.Type] = TransType.Data;
	        for (var i = 0; i < size; i++)
	        {
	            _buffer[i + TransSize.AckSize] = data[i];
	        }
	        size = size + TransSize.AckSize;
	        _checksum.CalcChecksum(ref _buffer, size);
	        return size;
	    }

	    private void CreateAndSendAck(SeqNr ackNr)
	    {
	        var ackBuf = new byte[TransSize.AckSize];
	        ackBuf[TransChecksum.SequenceNumber] = ackNr;
	        ackBuf[TransChecksum.Type] = TransType.Ack;
            _checksum.CalcChecksum(ref ackBuf, TransSize.AckSize);

            // Noise simulation
	        if (++_noiseSimulation == 2)
	        {
	            ackBuf[0]++;
	            _noiseSimulation = 0;
	        }

            _link.Send(ackBuf, TransSize.AckSize);
	    }

	    private byte ReceiveAck()
	    {
	        var ackBuf = new byte[TransSize.AckSize];
            var size = _link.Receive(ref ackBuf);
	        if (size != TransSize.AckSize || !_checksum.CheckChecksum(ackBuf, TransSize.AckSize) ||
	            ackBuf[(int)TransChecksum.SequenceNumber] != _sequenceNumber || ackBuf[TransChecksum.Type] != TransType.Ack)
	            return DefaultSeqNr;
	        return _sequenceNumber;
	    }

        #endregion



    }
}