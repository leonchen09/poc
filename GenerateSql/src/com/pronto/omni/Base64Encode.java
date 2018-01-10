package com.pronto.omni;

import java.io.BufferedInputStream;
import java.io.BufferedOutputStream;
import java.io.BufferedReader;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.FileReader;

public class Base64Encode {
	/**
	 * ��׼base64�����
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
		 * һ�δ���3���ֽڣ�3*8 == 4*6 ������������������±���
		 * 
		 * �÷����е�*&63,*&15,*&3�������������£� �������byte������ʹ洢��64��ʽ���£�11111111
		 * �������byte������ʹ洢��15��ʽ���£�1111���� 2^3 + 2^2 + 2^1 + 2^0 = 15
		 * ��&�������롱������������Ҫ���и�λ���������
		 */
		for (i = 0; i < (bytes.length - (bytes.length % 3)); i += 3) {

			// ��һ���ֽڣ����Դ�ֽڵĵ�һ���ֽڴ��?
			// ����Դ��һ�ֽ�������λ��ȥ����2λ����2λ���㡣
			// �ȣ�00 + ��6λ
			pos = (byte) ((bytes[i] >> 2) & 63);
			s.append(CODEC.charAt(pos));

			// �ڶ����ֽڣ����Դ�ֽڵĵ�һ���ֽں͵ڶ����ֽ����ϴ��?
			// �������£���һ���ֽڸ�6λȥ��������λ���ڶ����ֽ�������λ
			// ����Դ��һ�ֽڵ�2λ + Դ��2�ֽڸ�4λ
			pos = (byte) (((bytes[i] & 3) << 4) + ((bytes[i + 1] >> 4) & 15));
			s.append(CODEC.charAt(pos));

			// ������ֽڣ����Դ�ֽڵĵڶ����ֽں͵�����ֽ����ϴ��?
			// ����ڶ����ֽ�ȥ����4λ��������λ���ø�6λ����������ֽ�����6λ��ȥ����6λ���õ�2λ������Ӽ���
			pos = (byte) (((bytes[i + 1] & 15) << 2) + ((bytes[i + 2] >> 6) & 3));
			s.append(CODEC.charAt(pos));

			// ���ĸ��ֽڣ�����Դ�����ֽ�ȥ����2λ����
			pos = (byte) (((bytes[i + 2]) & 63));
			s.append(CODEC.charAt(pos));

			// ���base64�ı������ÿ76���ַ���Ҫһ������
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
				// �ֳ���һ��������λ��ǰ6λ��������λ���õ�һ����8λ
				pos = (byte) ((bytes[i] >> 2) & 63);
				s.append(CODEC.charAt(pos));
				// �������3�ߵĸ�λ���ֳ�8λ�ĺ���λ��Ȼ������4λ���õ�һ����8λ
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
	public byte[] decode(String s) {
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

		// base64������ַ�ȱ���Ϊ4�ı���
		if (buf.length() % 4 != 0) {
			return null;
			//throw new Exception("Base64 decoding invalid length");
		}

		// Ԥ����ֽ�����ĳ���
		byte[] bytes = new byte[3 * (buf.length() / 4)];
		int index = 0;

		/**
		 * ÿ4��base64�ַ���һ��Դ�ַ�������ַ�
		 * 
		 * Ȼ��ÿ�ĸ��ַ�ֱ���ѭ����ÿ��ѭ������6λ����Ϊ��6λ���õ�6λ�ٲ�����һ��base64�ַ���base64����е����С�
		 * ��Ϊ�ַ�������е�����С�ڵ���64������С�ڵ���2��6�η���6λ����
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
					return null;
					//throw new Exception("Base64 decoding bad character");
				}
				// ÿ�ζ����λ�ƶ�6��λ�ú��ټ����µ��ַ�����base64������е�λ�á�
				nGroup = (nGroup << 6) + base64Index;
			}

			// ����16λ��ȡ��8λ
			bytes[index] = (byte) (255 & (nGroup >> 16));
			index++;

			// ����8λ��ȡ��16λ������.0011111111(32λwindowsϵͳ)����and������ȡ�ø�16λ�ĵ�8λ��
			if ((255 & (nGroup >> 8)) == 0) {
				continue;
			}
			bytes[index] = (byte) (255 & (nGroup >> 8));
			index++;

			// ֱ����.0011111111����and��������32λ��ĵ�8λ
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
	 * �ӱ�������ҳ���Ӧ���ַ�����
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
		Base64Encode encode = new Base64Encode();
		//encode.encodeTest();
		encode.decodeTest();
//		String str = encode.encrypt("afdasfdsafsa\r\nasdfj;asjfdjaskfjd;sjfkajfasjfkl;jasjfklasjfjds");
//		System.out.println("encode:"+str);
//		System.out.println("deconde:" + encode.decrypt(str));
	}
	
	private void encodeTest() throws Exception{
		File fin = new File("e:\\ProntoDir\\2.jpg");
		long currentTime = System.currentTimeMillis();
		BufferedOutputStream fout = new BufferedOutputStream(new FileOutputStream("e:\\ProntoDir\\2_java.txt"));
		BufferedInputStream finso = new BufferedInputStream(new FileInputStream(fin));
		byte[] bytes = new byte[(int)fin.length()];
		finso.read(bytes);
		fout.write(Base64Encode.getInstance().encode(bytes).getBytes());
//		fout.flush();
		finso.close();
		fout.close();
		long endTime = System.currentTimeMillis();
		System.out.println("total time:" + (endTime - currentTime));
	}
	private void decodeTest()throws Exception{
		BufferedReader br = new BufferedReader(new FileReader("e:\\ProntoDir\\2_java.txt"));
		long currentTime = System.currentTimeMillis();
		//BufferedOutputStream fout = new BufferedOutputStream(new FileOutputStream("e:\\ProntoDir\\2_1.jpg"));
		FileOutputStream fout = new FileOutputStream("e:\\ProntoDir\\2_1.jpg");
		StringBuffer sb = new StringBuffer();
		String r = br.readLine();
		while(r != null){
			sb.append(r);
			r = br.readLine();
		}
		fout.write(Base64Encode.getInstance().decode(sb.toString()));
		fout.flush();
		br.close();
		fout.close();
		long endTime = System.currentTimeMillis();
		System.out.println("total time:" + (endTime - currentTime));
	}
	
}
