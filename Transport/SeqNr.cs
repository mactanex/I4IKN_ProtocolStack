using System;
using Linklaget;

namespace Transportlaget
{
    internal class SeqNr
    {
        private byte _seqNr;

        public SeqNr(int seqNr) : this((byte)seqNr)
        {

        }

        public SeqNr(byte seqNr)
        {
            if (!(seqNr <= 1)) throw new ArgumentOutOfRangeException(nameof(seqNr));
            _seqNr = seqNr;
        }

        public void Next()
        {
            _seqNr = (byte)((_seqNr + 1) % 2);
        }

        public SeqNr Peak()
        {
            var next = new SeqNr(_seqNr);
            return ++next;
        }

        #region Casting and overloads

        public static SeqNr operator ++(SeqNr inc)
        {
            inc.Next();
            return inc;
        }

        public static implicit operator byte(SeqNr seqNr)
        {
            return seqNr._seqNr;
        }

        #endregion
    }

	internal static class SeqNrExtensions
	{
		public static SeqNr SendAcknowledge(this SeqNr nr, Link link, bool ackType, Func<bool> noiseSimulation = null)
		{
			var ackBuf = new byte[(byte)TransSize.AckSize];
			ackBuf[(byte)TransChecksum.SequenceNumber] = ackType ? nr : nr.Peak();
			ackBuf[(byte)TransChecksum.Type] = (byte)TransType.Ack;
			ChecksumCalculator.CalcChecksum(ackBuf, (byte)TransSize.AckSize);

			if (noiseSimulation != null) {
				if(noiseSimulation())
					ackBuf [0]++;
			}

			link.Send(ackBuf, (byte)TransSize.AckSize);

			return nr;
		}

		public static bool RequestAcknowledge(this SeqNr nr, Link link)
		{
			var ackBuf = new byte[(byte)TransSize.AckSize];
			var size = link.Receive(ref ackBuf);
			if (size != (byte)TransSize.AckSize || !ChecksumCalculator.CheckChecksum(ackBuf, (byte)TransSize.AckSize) ||
				ackBuf[(byte)TransChecksum.SequenceNumber] != nr || ackBuf[(byte)TransChecksum.Type] != (byte)TransType.Ack)
				return false;

			return true;
		}	
	}

}