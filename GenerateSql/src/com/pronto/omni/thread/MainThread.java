package com.pronto.omni.thread;

public class MainThread extends Thread {
	
	private String name;
	private Object lock;
	
	public MainThread(String name, Object lock){
		this.name = name;
		this.lock = lock;
	}

	@Override
	public void run(){
		System.out.println("main thread:"+System.currentTimeMillis());
		synchronized(this.lock){
			
			try {
				Pool.add(lock);
				this.lock.wait();
			} catch (InterruptedException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
		}
		
		System.out.println("main thread continue:"+System.currentTimeMillis());
	}
	
}
