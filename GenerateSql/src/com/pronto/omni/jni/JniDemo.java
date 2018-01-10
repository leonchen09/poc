package com.pronto.omni.jni;

public class JniDemo {
	static{
		System.loadLibrary("FolderWatch");
	}
	
	public native void folderWatch(String folderName);

	public static void main(String[] argv){
		new JniDemo().folderWatch(argv[0]);
	}
	
	public void fileChange(String fileName, String action){
		System.out.println(fileName + " " + action);
	}
}
