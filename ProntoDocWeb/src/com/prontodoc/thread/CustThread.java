package com.prontodoc.thread;

public class CustThread extends Thread{

	public void run(){
		while(true){
			System.out.println("running");
			for(int i = 0; i < 10000000; i ++)
				;
			try {
				sleep(100);
			} catch (InterruptedException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
				return;
			}
		}
	}
	
}
