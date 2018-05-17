using System;

namespace Transportlaget
{
    public static class TransSize
    {
        public static byte ChecksumSize = 2;
        public static byte AckSize = 4;
    }

    public static class TransChecksum
    {
        public static byte ChecksumHigh = 0;
        public static byte ChecksumLow = 1;
        public static byte SequenceNumber = 2;
        public static byte Type = 3;
    }

    public static class TransType
    {
        public static byte Data = 0;
        public static byte Ack = 1;
    }
}