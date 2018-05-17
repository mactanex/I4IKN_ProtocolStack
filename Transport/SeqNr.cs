using System;

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
            next.Next();
            return next;
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
            return x == (byte)y;
        }

        public static bool operator !=(SeqNr x, int y)
        {
            return x != (byte)y;
        }

        public static bool operator ==(SeqNr x, byte y)
        {
            return x._seqNr == y;
        }

        public static bool operator !=(SeqNr x, byte y)
        {
            return x._seqNr != y;
        }

        public static explicit operator SeqNr(byte seqNr)
        {
            return new SeqNr(seqNr);
        }

        public static implicit operator byte(SeqNr seqNr)
        {
            return seqNr._seqNr;
        }

        #endregion
    }
}