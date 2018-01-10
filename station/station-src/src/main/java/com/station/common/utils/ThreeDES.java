package com.station.common.utils;

import java.util.Base64;

import javax.crypto.Cipher;
import javax.crypto.SecretKey;
import javax.crypto.spec.SecretKeySpec;

public class ThreeDES {
	private static final String Algorithm = "DESede"; // 定义 加密算法,可用
	
	private static final byte[] KeyBytes = "PassW0rd!Key43Des{uk@1-]".getBytes();//定义加密的key
	// DES,DESede,Blowfish

	public static String encrypt(String srcStr) {
		if(srcStr == null || srcStr.length() < 1) {
			return srcStr;
		}
		return encryptMode(KeyBytes, srcStr.getBytes());
	}
	
	/**
	 * 加密方法
	 * @param keybyte 加密密钥，长度为24字节
	 * @param src 被加密的数据缓冲区（源）
	 * @return
	 */
	@SuppressWarnings("restriction")
	public static String encryptMode(byte[] keybyte, byte[] src) {
		try {
			// 生成密钥
			SecretKey deskey = new SecretKeySpec(keybyte, Algorithm);

			// 加密
			Cipher c1 = Cipher.getInstance(Algorithm);
			c1.init(Cipher.ENCRYPT_MODE, deskey);
			return Base64.getEncoder().encodeToString(c1.doFinal(src));
		} catch (java.security.NoSuchAlgorithmException e1) {
			e1.printStackTrace();
		} catch (javax.crypto.NoSuchPaddingException e2) {
			e2.printStackTrace();
		} catch (java.lang.Exception e3) {
			e3.printStackTrace();
		}
		return null;
	}

	public static String decrypt(String srcStr) {
		if(srcStr == null || srcStr.length() < 1) {
			return srcStr;
		}
		byte[] result = decryptMode(KeyBytes, srcStr);
		return new String(result);
	}
	
	/**
	 * 解密
	 * @param keybyte 加密密钥，长度为24字节
	 * @param src 加密后的缓冲区
	 * @return
	 */
	public static byte[] decryptMode(byte[] keybyte, String src) {
		try {
			byte[] srcByte = Base64.getDecoder().decode(src);
			// 生成密钥
			SecretKey deskey = new SecretKeySpec(keybyte, Algorithm);

			// 解密
			Cipher c1 = Cipher.getInstance(Algorithm);
			c1.init(Cipher.DECRYPT_MODE, deskey);
			return c1.doFinal(srcByte);
		} catch (java.security.NoSuchAlgorithmException e1) {
			e1.printStackTrace();
		} catch (javax.crypto.NoSuchPaddingException e2) {
			e2.printStackTrace();
		} catch (java.lang.Exception e3) {
			e3.printStackTrace();
		}
		return null;
	}

	/**
	 * 转换成十六进制字符串
	 * @param b
	 * @return
	 */
	public static String byte2hex(byte[] b) {
		String hs = "";
		String stmp = "";

		for (int n = 0; n < b.length; n++) {
			stmp = (java.lang.Integer.toHexString(b[n] & 0XFF));
			if (stmp.length() == 1)
				hs = hs + "0" + stmp;
			else
				hs = hs + stmp;
			if (n < b.length - 1)
				hs = hs + ":";
		}
		return hs.toUpperCase();
	}

	
	public static void main(String[] args) {
	
		String szSrc = "amplifiPL";

		System.out.println("加密前的字符串:" + szSrc);

		String encoded = encryptMode(KeyBytes, szSrc.getBytes());
		System.out.println("加密后的字符串:" + encoded);

		byte[] srcBytes = decryptMode(KeyBytes, encoded);
		System.out.println("解密后的字符串:" + (new String(srcBytes)));
	}

}
