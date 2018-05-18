using System;
using System.Collections.Generic;
using System.Linq;

namespace Transportlaget
{
	public static class ChecksumCalculator
	{
		private static long CheckSum (byte[] buf)
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

		public static bool CheckChecksum(byte[] buf, int size)
		{
            // Her mangler at blive taget hånd om size... Ellers checker den hele arrayet!
			return CheckSum(buf.Skip((int)TransSize.ChecksumSize).ToArray()) == 
			        (buf[(int)TransChecksum.ChecksumHigh] << 8 | buf[(int)TransChecksum.ChecksumLow]);
		}

		public static void CalcChecksum (byte[] buf, int size)
		{			
			var sum = CheckSum(buf.Skip((int)TransSize.ChecksumSize).ToArray());
			buf[(int)TransChecksum.ChecksumHigh] = (byte)((sum >> 8) & 255);
			buf[(int)TransChecksum.ChecksumLow] = (byte)(sum & 255);
		}
	}
}