package com.pronto.omni.io;

import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.PipedInputStream;
import java.io.PipedOutputStream;

public class Test {

	public static void main(String[] argv) throws IOException, InterruptedException{
		
		Reader reader = new Reader();
		Writer writer = new Writer();
		reader.start();
		writer.start();
//		reader.out.connect(writer.in);
		for(int i = 0; i < 10; i ++){
			System.out.println("begin cycle in main thread:" + System.currentTimeMillis());
			FileInputStream in = new FileInputStream("E:\\ProntoDir\\2.jpg");
			reader.in = in;
			FileOutputStream out = new FileOutputStream("e:\\ProntoDir\\2" + i +".jpg");
			writer.out = out;
//			writer.i = i;
			PipedOutputStream pout = new PipedOutputStream();
			PipedInputStream pin = new PipedInputStream();
			pout.connect(pin);
			reader.assignTask(pout);
			writer.assignTask(i, pin);
			Thread.sleep(1000);
			in.close();
			out.close();
		}
		
		
	}
	
}
