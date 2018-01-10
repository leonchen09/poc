package com.pronto.omni.thread;

public class ParentThread extends Thread{
	
	public ParentThread(String name){
		this.setName(name);
	}
	
	public static Object obj = new Object();
	public int sleepTime;
	@Override  
	public void run() {  
		System.out.println(Thread.currentThread().getName() + " parent start..."+System.currentTimeMillis());  
		ChildThread ct = new ChildThread("child of " + this.getName(),null);
//		ct.sleepTime = sleepTime;
		ct.start();
		synchronized (obj){
			try {
				obj.wait();
			} catch (InterruptedException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
		}

		System.out.println(Thread.currentThread().getName() + " parent end..."+System.currentTimeMillis()); 
	} 
}
