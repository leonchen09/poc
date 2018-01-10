package com.pronto.omni.thread;

public class Waiter extends Thread{
 
	public Lock lock;
	
	@Override
	public void run(){
		System.out.println("In child thread run");
		try {
			synchronized(lock){
				lock.wait();
			}
		} catch (InterruptedException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		System.out.println("In child thread run, after wait()");
	}
	
}
