package com.prontodoc.thread;

public class ThreadTest {
	public static void main(String[] argv){
		CustThread t = new CustThread();
		t.start();
//		try {
//			t.sleep(10);
//		} catch (InterruptedException e) {
//			// TODO Auto-generated catch block
//			e.printStackTrace();
//		}
//		t.interrupt();
		t.stop();
		System.out.println("end");
	}
}
