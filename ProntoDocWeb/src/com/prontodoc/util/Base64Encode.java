package com.prontodoc.util;

import java.io.BufferedInputStream;
import java.io.BufferedOutputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;

public class Base64Encode {
	/**
	 * 标准base64编码表
	 */
	private final static String CODEC = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
	private final static Base64Encode base64 = new Base64Encode();

	private Base64Encode() {

	}

	public static Base64Encode getInstance() {
		return base64;
	}

	/*
	 * (non-Javadoc)
	 * 
	 * @see
	 * com.pantosoft.framework.util.encrpytion.IEncryptencrypt(java.lang.String)
	 */
	public String encrypt(String s) throws Exception {
		return encode(s.getBytes());
	}

	/*
	 * (non-Javadoc)
	 * 
	 * @see
	 * com.pantosoft.framework.util.encrpytion.IEncrypt#decrypt(java.lang.String
	 * )
	 */
	public String decrypt(String s) throws Exception {
		return new String(this.decode(s));
	}

	public String encode(byte[] bytes) {
		StringBuilder s = new StringBuilder();
		int i = 0;
		byte pos;
		/*
		 * 一次处理3个字节，3*8 == 4*6 的运算规则来进行重新编码
		 * 
		 * 该方法中的*&63,*&15,*&3操作的意义如下： 计算机中byte数据类型存储的64形式如下：11111111
		 * 计算机中byte数据类型存储的15形式如下：1111，即 2^3 + 2^2 + 2^1 + 2^0 = 15
		 * “&”、“与”，运算这里主要进行高位清零操作。
		 */
		for (i = 0; i < (bytes.length - (bytes.length % 3)); i += 3) {

			// 第一个字节，根据源字节的第一个字节处理。
			// 规则：源第一字节右移两位，去掉低2位，高2位补零。
			// 既：00 + 高6位
			pos = (byte) ((bytes[i] >> 2) & 63);
			s.append(CODEC.charAt(pos));

			// 第二个字节，根据源字节的第一个字节和第二个字节联合处理。
			// 规则如下，第一个字节高6位去掉左移四位，第二个字节右移四位
			// 即：源第一字节低2位 + 源第2字节高4位
			pos = (byte) (((bytes[i] & 3) << 4) + ((bytes[i + 1] >> 4) & 15));
			s.append(CODEC.charAt(pos));

			// 第三个字节，根据源字节的第二个字节和第三个字节联合处理，
			// 规则第二个字节去掉高4位并左移两位（得高6位），第三个字节右移6位并去掉高6位（得低2位），相加即可
			pos = (byte) (((bytes[i + 1] & 15) << 2) + ((bytes[i + 2] >> 6) & 3));
			s.append(CODEC.charAt(pos));

			// 第四个字节，规则，源第三字节去掉高2位即可
			pos = (byte) (((bytes[i + 2]) & 63));
			s.append(CODEC.charAt(pos));

			// 根据base64的编码规则，每76个字符需要一个换行
			// 76*3/4 = 57
			if (((i + 2) % 56) == 0) {
				s.append("\r\n");
			}
		}

		if (bytes.length % 3 != 0) {

			if (bytes.length % 3 == 2) {

				pos = (byte) ((bytes[i] >> 2) & 63);
				s.append(CODEC.charAt(pos));

				pos = (byte) (((bytes[i] & 3) << 4) + ((bytes[i + 1] >> 4) & 15));
				s.append(CODEC.charAt(pos));

				pos = (byte) ((bytes[i + 1] & 15) << 2);
				s.append(CODEC.charAt(pos));

				s.append("=");

			} else if (bytes.length % 3 == 1) {
				// 分出第一个二进制位的前6位，右移两位，得到一个新8位
				pos = (byte) ((bytes[i] >> 2) & 63);
				s.append(CODEC.charAt(pos));
				// 先清零比3高的高位，分出8位的后两位，然后左移4位，得到一个新8位
				pos = (byte) ((bytes[i] & 3) << 4);
				s.append(CODEC.charAt(pos));

				s.append("==");
			}
		}
		return s.toString();
	}

	/**
	 * 
	 * @param s
	 * @return
	 * @throws Exception
	 */
	public byte[] decode(String s) throws Exception {
		StringBuffer buf = new StringBuffer(s);
		int i = 0;
		char c = ' ';
		char oc = ' ';
		while (i < buf.length()) {
			oc = c;
			c = buf.charAt(i);
			if (oc == '\r' && c == '\n') {
				buf.deleteCharAt(i);
				buf.deleteCharAt(i - 1);
				i -= 2;
			} else if (c == '\t') {
				buf.deleteCharAt(i);
				i--;
			} else if (c == ' ') {
				i--;
			}
			i++;
		}

		// base64编码的字符长度必须为4的倍数
		if (buf.length() % 4 != 0) {
			throw new Exception("Base64 decoding invalid length");
		}

		// 预设的字节数组的长度
		byte[] bytes = new byte[3 * (buf.length() / 4)];
		int index = 0;

		/**
		 * 每4个base64字符代表一个源字符编码后的字符！
		 * 
		 * 然后每四个字符分别做循环，每个循环左移6位，作为低6位，该低6位再补上下一个base64字符在base64码表中的序列。
		 * 因为字符在码表中的序列小于等于64，即，小于等于2的6次方（6位）！
		 */
		for (i = 0; i < buf.length(); i += 4) {

			byte base64Index = 0;
			int nGroup = 0;

			for (int j = 0; j < 4; j++) {

				char theChar = buf.charAt(i + j);

				if (theChar == '=') {
					base64Index = 0;
				} else {
					base64Index = getBase64Index(theChar);
				}

				if (base64Index == -1) {
					throw new Exception("Base64 decoding bad character");
				}
				// 每次都想高位移动6个位置后再加上新的字符所在base64编码表中的位置。
				nGroup = (nGroup << 6) + base64Index;
			}

			// 右移16位，取高8位
			bytes[index] = (byte) (255 & (nGroup >> 16));
			index++;

			// 右移8位，取高16位，且与.0011111111(32位windows系统)进行and操作，取该高16位的低8位。
			if ((255 & (nGroup >> 8)) == 0) {
				continue;
			}
			bytes[index] = (byte) (255 & (nGroup >> 8));
			index++;

			// 直接与.0011111111进行and操作，该32位数的低8位
			if ((255 & nGroup) == 0) {
				continue;
			}
			bytes[index] = (byte) (255 & (nGroup));
			index++;
		}

		byte[] newBytes = new byte[index];
		for (i = 0; i < index; i++) {
			newBytes[i] = bytes[i];
		}

		return newBytes;
	}

	/**
	 * 从编码表中找出对应的字符序列
	 * 
	 * @param c
	 * @return
	 */
	private byte getBase64Index(char c) {
		byte index = -1;
		for (byte i = 0, j = (byte) (CODEC.length() & 225); i < j; i++) {
			if (CODEC.charAt(i) == c) {
				index = i;
				break;
			}
		}
		return index;
	}

	static final char digits[] = { '0', '1', '2', '3', '4', '5', '6', '7', '8',
			'9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l',
			'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y',
			'z' };

	private static String toUnsignedString(int i, int j) {
		char ac[] = new char[32];
		int k = 32;
		int l = 1 << j;
		int i1 = l - 1;
		do {
			ac[--k] = digits[i & i1];
			i >>>= j;
		} while (i != 0);
		return new String(ac, k, 32 - k);
	}

	public static void main(String[] args) throws Exception {
		File fin = new File("e:\\ProntoDir\\pic6071.jpg");
		long currentTime = System.currentTimeMillis();
		BufferedOutputStream fout = new BufferedOutputStream(new FileOutputStream("e:\\ProntoDir\\pic6071_java.txt"));
		BufferedInputStream finso = new BufferedInputStream(new FileInputStream(fin));
		byte[] bytes = new byte[(int)fin.length()];
		finso.read(bytes);
		fout.write(Base64Encode.getInstance().encode(bytes).getBytes());
		finso.close();
		fout.close();
		long endTime = System.currentTimeMillis();
		System.out.println("total time:" + (endTime - currentTime));
	}
}
