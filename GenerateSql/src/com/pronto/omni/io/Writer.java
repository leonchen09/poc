package com.pronto.omni.io;

import java.io.FileOutputStream;
import java.io.IOException;
import java.io.OutputStream;
import java.io.PipedInputStream;

public class Writer extends BaseService {
	
	public PipedInputStream in;
	public OutputStream out;

	public int i;
	
	public void assignTask(int i, PipedInputStream in){
    	waitForTaskFinish();
    	synchronized(this){
    		this.in = in;
    		this.i = i;
    		hasTask = true;
	        this.notify();
    	}
    }
	
	@Override
	protected void doTask() {
		byte[] buffer = new byte[512];
		try {
//			FileOutputStream out = new FileOutputStream("e:\\ProntoDir\\2" + i +".jpg");
			while(in.read(buffer) >= 0){
				out.write(buffer);
			}
//			out.close();
			in.close();
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}

}
