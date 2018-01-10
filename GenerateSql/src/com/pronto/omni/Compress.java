package com.pronto.omni;

import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.util.zip.GZIPInputStream;
import java.util.zip.GZIPOutputStream;

public class Compress {

	 public static byte[] compress(String str) throws IOException {
		    if (str == null || str.length() == 0) {
		      return null;
		    }
		    ByteArrayOutputStream out = new ByteArrayOutputStream();
		    GZIPOutputStream gzip = new GZIPOutputStream(out);
		    gzip.write(str.getBytes());
		    gzip.close();
		    return out.toByteArray();
		  }
	 
	 public static byte[] compress(InputStream in) throws IOException{
		    ByteArrayOutputStream out = new ByteArrayOutputStream();
		    GZIPOutputStream gzip = new GZIPOutputStream(out);
		    byte[] buffer = new byte[256];
		    int n = 0;
		    while((n =(in.read(buffer))) >= 0){
		    	gzip.write(buffer, 0, n);
		    }
		    gzip.close();
		    return out.toByteArray();
	 }
	 
		  public static String uncompress(byte[] source) throws IOException {
		    if (source == null || source.length == 0) {
		      return null;
		    }
		    ByteArrayOutputStream out = new ByteArrayOutputStream();
		    ByteArrayInputStream in = new ByteArrayInputStream(source);
		    GZIPInputStream gunzip = new GZIPInputStream(in);
		    byte[] buffer = new byte[256];
		    int n;
		    while ((n = gunzip.read(buffer)) >= 0) {
		      out.write(buffer, 0, n);
		    }
		    return out.toString();
		  }

		  // 测试方法
		  public static void main(String[] args) throws IOException {
//			  FileInputStream in0 = new FileInputStream("e:\\xmltest6.txt");
//			  byte[] buf = new byte[256];
//			  ByteArrayOutputStream out0 = new ByteArrayOutputStream();
//			  while(in0.read(buf) >= 0){
//				  out0.write(buf);
//				  buf = null;
//			  }
//			  in0.close();
//			  String data = out0.toString();
//			  out0.close();
//			  
//			  System.out.println(data);
//			  System.out.println("----------------------------------------------------------------------");
//			  
			  FileOutputStream fout = new FileOutputStream("e:\\javac1.zip");
			  fout.write(compress(new FileInputStream("e:\\xmltest6.xml")));
			  fout.close();
			  

			  File file = new File("e:\\javac1.zip");
			  FileInputStream in = new FileInputStream(file);
			  ByteArrayOutputStream out = new ByteArrayOutputStream();
			  byte[] buffer = new byte[1024];
			  while(in.read(buffer)!= -1){
				  out.write(buffer);
			  }
			  String result = uncompress(out.toByteArray());
			  in.close();
			  out.close();
			  System.out.println(result);
			  
//			  System.out.println(Compress.uncompress(Compress.compress("中国China")));
		  }
}
