using System;

namespace Transportlaget
{
    public enum TransSize
    {
        ChecksumSize = 2,
        AckSize = 4
    }

    public enum TransChecksum
    {
        ChecksumHigh = 0,
        ChecksumLow = 1,
        SequenceNumber = 2,
        Type = 3
    }

    public enum TransType
    {
        Data = 0,
        Ack = 1,
    }
}