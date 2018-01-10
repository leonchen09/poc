package com.pronto.omni;

public class Tools {

	/*
	 * 
	 */
	public static byte[] bitOr(byte[] parm1, byte[] parm2){
		byte[] result = new byte[parm1.length > parm2.length ? parm1.length : parm2.length];
		for(int i = 0; i < result.length; i ++){
			byte p1 = 0;
			byte p2 = 0;
			if(i < parm1.length)
				p1 = parm1[i];
			if(i < parm2.length)
				p2 = parm2[i];
			result[i] = (byte)(p1 | p2);
		}
		return result;
	}
	
	public static byte genByte(int index){
		return (byte)(1 << (index % 8));
	}
}
