package com.pronto.md5;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.Reader;
import java.io.StringWriter;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.sql.SQLException;

public class MD5Test {

	public static void main(String[] argv) throws IOException, SQLException{
		Reader in = new FileReader("E:\\ProntoDir\\oracle_xml\\oraclexml.xml.bak");
		StringWriter sw = new StringWriter();
		BufferedReader br = new BufferedReader(in);
		BufferedWriter bw = new BufferedWriter(sw);
		String data = null;
		while((data = br.readLine()) != null)
			bw.write(data);
		bw.flush();
		String s = sw.toString();
		String t = null;
		long start = System.currentTimeMillis();
		
		for(int i = 0; i < 100; i ++){
			t = abstrString(s);
		}
		
		long end = System.currentTimeMillis();
		
		System.out.println("total time: " + (end-start) + ", res:"+t);
	}
	
	private static String abstrString(String value) throws SQLException{
		if(value.length() < 4001)
			return value;
		else{
			try {
				MessageDigest alga = MessageDigest.getInstance("MD5");
				alga.update(value.getBytes());
				byte[] digesta = alga.digest();
				return new String(digesta);
			} catch (NoSuchAlgorithmException e) {
				throw new SQLException("Exception while get abstract information, reason: " + e.getMessage());
			}
		}
	}
}
