using System;

namespace Transportlaget
{
	public class Checksum
	{
		public Checksum ()
		{
		}

		private long checksum (byte[] buf)
		{
    		int i = 0, length = buf.Length;
    		long sum = 0;
    		while (length > 0) 
			{
        		sum	+= (buf[i++]&0xff) << 8;
        		if ((--length)==0) break;
        		sum += (buf[i++]&0xff);
        		--length;
    		}

    		return (~((sum & 0xFFFF)+(sum >> 16)))&0xFFFF;
		}

		public bool CheckChecksum(byte[] buf, int size)
		{
			var buffer = new byte[size-2];
			Array.Copy(buf, TransSize.ChecksumSize, buffer, 0, buffer.Length);
			return ( checksum(buffer) == (buf[TransChecksum.ChecksumHigh] << 8 | buf[TransChecksum.ChecksumLow]));
		}

		public void CalcChecksum (ref byte[] buf, int size)
		{
			var buffer = new byte[size-2];
			long sum = 0;

			Array.Copy(buf, 2, buffer, 0, buffer.Length);
			sum = checksum(buffer);
			buf[TransChecksum.ChecksumHigh] = (byte)((sum >> 8) & 255);
			buf[TransChecksum.ChecksumLow] = (byte)(sum & 255);
		}
	}
}