package com.pronto.omni;

import java.util.UUID;

import javax.crypto.Cipher;
import javax.crypto.SecretKey;
import javax.crypto.spec.SecretKeySpec;


public class CodecHelper
{
	private static String CRYPT_KEY = UUID.randomUUID().toString();
	
	public static byte[] encryptMode(byte[] keybyte, byte[] src)
	{
		try
		{
			SecretKey deskey = new SecretKeySpec(keybyte, "DESede");
			Cipher c1 = Cipher.getInstance("DESede/ECB/PKCS5Padding");
			c1.init(Cipher.ENCRYPT_MODE, deskey);
			return c1.doFinal(src);
		}
		catch (java.security.NoSuchAlgorithmException e1)
		{
			e1.printStackTrace();
		}
		catch (javax.crypto.NoSuchPaddingException e2)
		{
			e2.printStackTrace();
		}
		catch (java.lang.Exception e3)
		{
			e3.printStackTrace();
		}
		return null;
	}

	public static byte[] decryptMode(byte[] keybyte, byte[] src)
	{
		try
		{
			SecretKey deskey = new SecretKeySpec(keybyte, "DESede");
			Cipher c1 = Cipher.getInstance("DESede/ECB/PKCS5Padding");
			c1.init(Cipher.DECRYPT_MODE, deskey);
			return c1.doFinal(src);
		}
		catch (java.security.NoSuchAlgorithmException e1)
		{
			e1.printStackTrace();
		}
		catch (javax.crypto.NoSuchPaddingException e2)
		{
			e2.printStackTrace();
		}
		catch (java.lang.Exception e3)
		{
			e3.printStackTrace();
		}
		return null;
	}
	
	public static String encryptString(String src)
	{
		if(src==null || "".equals(src.trim()))
			return null;
		byte[] eb = encryptMode(CodecHelper.CRYPT_KEY.getBytes(),src.getBytes());
		String result = Base64.encode(eb);
		return result;
	}
	
	public static String decryptString(String src) throws Exception
	{
		if(src==null || "".equals(src.trim()))
			return null;
		byte[] srcb = Base64.decode(src);
		byte[] db = decryptMode(CodecHelper.CRYPT_KEY.getBytes(),srcb);
		String result = new String(db);
		return result;
	}
	
	public static void main(String[] args) throws Exception
	{
		String es = CodecHelper.encryptString("asdf1234");
		String src = CodecHelper.decryptString(es);
	}
	
	
}

