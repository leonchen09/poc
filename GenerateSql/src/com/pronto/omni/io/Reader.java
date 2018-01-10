package com.pronto.omni.io;

import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.PipedOutputStream;



public class Reader extends BaseService {
	
	public PipedOutputStream out;
	public InputStream in;

	public void assignTask(PipedOutputStream out){
    	waitForTaskFinish();
    	synchronized(this){
    		hasTask = true;
    		this.out = out;
	        this.notify();
    	}
    }
	
	@Override
	protected void doTask() {
		
		byte[] buffer = new byte[512];
		try {
//			FileInputStream in = new FileInputStream("E:\\ProntoDir\\2.jpg");
			while(in.read(buffer) >= 0){
				out.write(buffer);
			}
//			in.close();
			out.close();
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		
	}
	
}
