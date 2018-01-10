package com.station.adapter.biz;

public class ProtocolV5 {

    public static boolean IsValid(byte[] bytes)
    {
        if (bytes.length < 120 && bytes[0] == 0x7F && bytes[1] == 0xF7 && bytes[2] == (bytes.length - 3) &&
            checkThrough(bytes))
        {
            return true;
        }
        return false;
    }

    // 异或校验
    private static boolean checkThrough(byte[] bytes)
    {
        byte right = bytes[bytes.length - 1];
        byte left = bytes[0];
        for(int i = 1; i <= bytes.length - 2; i ++) {
        	left = (byte) (left ^ bytes[i]);// 从头到最后第二位的异或
        }
        return left == right ? true : false;
    }
}
