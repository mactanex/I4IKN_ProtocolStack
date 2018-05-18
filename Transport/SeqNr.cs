using System;
using Linklaget;

namespace Transportlaget
{
    internal class SeqNr
    {
        private byte _seqNr;
        private int _noiseSimulation = 0;

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

        public bool RequestAcknowledge(Link link)
        {
            var ackBuf = new byte[(byte)TransSize.AckSize];
            var size = link.Receive(ref ackBuf);
            if (size != (byte)TransSize.AckSize || !ChecksumCalculator.CheckChecksum(ackBuf, (byte)TransSize.AckSize) ||
                ackBuf[(byte)TransChecksum.SequenceNumber] != _seqNr || ackBuf[(byte)TransChecksum.Type] != (byte)TransType.Ack)
                return false;

            return true;
        }

        public SeqNr SendAcknowledge(Link link, bool ackType)
        {
            var ackBuf = new byte[(byte)TransSize.AckSize];
            ackBuf[(byte)TransChecksum.SequenceNumber] = ackType ? this : Peak();
            ackBuf[(byte)TransChecksum.Type] = (byte)TransType.Ack;
            ChecksumCalculator.CalcChecksum(ackBuf, (byte)TransSize.AckSize);

            // Noise simulation
            if (++_noiseSimulation == 2)
            {
                ackBuf[0]++;
                _noiseSimulation = 0;
            }

            link.Send(ackBuf, (byte)TransSize.AckSize);
            return this;
        }

        #region Casting and overloads

        public static SeqNr operator ++(SeqNr inc)
        {
            inc.Next();
            return inc;
        }

        public static bool operator ==(SeqNr x, SeqNr y)
        {
            return x == (byte)y;
        }

        public static bool operator !=(SeqNr x, SeqNr y)
        {
            return x != (byte)y;
        }

        public static bool operator ==(SeqNr x, int y)
        {
            return x._seqNr == y;
        }

        public static bool operator !=(SeqNr x, int y)
        {
            return x._seqNr != y;
        }

        public static bool operator ==(SeqNr x, byte y)
        {
            return x._seqNr == y;
        }

        public static bool operator !=(SeqNr x, byte y)
        {
            return x._seqNr != y;
        }

        public static implicit operator byte(SeqNr seqNr)
        {
            return seqNr._seqNr;
        }

        #endregion
    }
}