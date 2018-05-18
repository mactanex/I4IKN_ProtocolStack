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
		/// The _buffer.
		/// </summary>
		private byte[] _buffer;              
		/// <summary>
		/// The seq no.
		/// </summary>
	    private readonly SeqNr _sequenceNumber;
		/// <summary>
		/// The error count.
		/// </summary>
		private int _errorCount;

		private int _noiseSimulation = 0;

		/// <summary>
		/// Initializes a new instance of the <see cref="Transport"/> class.
		/// </summary>
		public Transport (int buffSize)
		{
			_link = new Link(buffSize + (int)TransSize.AckSize);
			_buffer = new byte[buffSize + (int)TransSize.AckSize];
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
		            } while (!_sequenceNumber.RequestAcknowledge(_link));
		            _sequenceNumber.Next();
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
	        var packageSize = 0;

	        while (_errorCount < 5) 
	        {
	            try
	            {

	                while ((packageSize = _link.Receive(ref _buffer)) > 0 && 
	                       (!ChecksumCalculator.CheckChecksum(_buffer, packageSize) ||
                          _buffer[(int)TransChecksum.SequenceNumber] != _sequenceNumber))
	                {
						_sequenceNumber.SendAcknowledge(_link, false, NoiseSimulation);
                    }

					_sequenceNumber.SendAcknowledge(_link, true, NoiseSimulation).Next();
	                return DeconstructPackage(ref buff, packageSize);
	                             
	            }
	            catch (TimeoutException)
	            {
                    _errorCount++;
	            }
            }

	        _errorCount = 0;
	        return packageSize;
	    }

        #region Utility

	    private int ConstructPackage(byte[] data, int size)
	    {
	        _buffer[(int)TransChecksum.SequenceNumber] = _sequenceNumber;
	        _buffer[(int)TransChecksum.Type] = (byte)TransType.Data;
	        for (var i = 0; i < size; i++)
	        {
	            _buffer[i + (byte)TransSize.AckSize] = data[i];
	        }
	        size = size + (byte)TransSize.AckSize;
	        ChecksumCalculator.CalcChecksum(_buffer, size);
	        return size;
	    }

	    private int DeconstructPackage(ref byte[] targetBuff, int packageSize)
	    {
	        var dataSize = targetBuff.Length < packageSize - (byte)TransSize.AckSize ? targetBuff.Length : packageSize - (byte)TransSize.AckSize;
	        Array.Copy(_buffer, (int)TransSize.AckSize, targetBuff, 0, dataSize);
            return dataSize;
	    }

        #endregion

		#region Simulation

		private bool NoiseSimulation()
		{
			if (++_noiseSimulation == 2) {
				_noiseSimulation = 0;
				return true;
			}
			return false;
		}

		#endregion

	}
}