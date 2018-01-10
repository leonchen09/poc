package com.pronto.omni.thread;

public class ChildThread extends Thread {
	private String name;
	private Object lock;
	
	public ChildThread(String name, Object lock){
		this.name = name;
		this.lock = lock;
	}

	@Override
	public void run(){
		System.out.println("child thread:"+System.currentTimeMillis());
		try {
			Thread.sleep(2000);
		} catch (InterruptedException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		synchronized(this.lock){
			Pool.remove(lock);
			this.lock.notify();
		}
		System.out.println("child thread continue:"+System.currentTimeMillis());
	}
}
