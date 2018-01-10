package com.pronto.omni.folermonitor;

import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.OutputStream;

public class ListFiles {

	public static void main(String[] argv) throws InterruptedException, IOException{
		while(true){
			scanFile("E:\\ProntoDir\\tt");
			Thread.sleep(5000);
		}
	}
	
	private static void scanFile(String dir) throws IOException, InterruptedException{
		File directory = new File(dir);
		File[] arrTemplates = directory.listFiles();
		for(File f : arrTemplates){
			System.out.println("File Name:" + f.getAbsolutePath() + ",r,w,e:"+f.canRead()+f.canWrite()+f.canExecute());
			while(true){
				try {
					OutputStream out = new FileOutputStream(f,true);
					out.close();
					break;
				} catch (FileNotFoundException e) {
					Thread.sleep(200);
				}
			}
			System.out.println("File is ok:"+f.getAbsolutePath());
		}
	}
	
}
