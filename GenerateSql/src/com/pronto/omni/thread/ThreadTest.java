package com.pronto.omni.thread;

public class ThreadTest  extends ThreadGroup{

	public static Object pool = new Object();
	public ThreadTest() {
		super("arg0");
	}

	public static void main(String[] argv){
		NetworkThread t = new NetworkThread();
	       t.start();
	       System.out.println("main thread");
	       for(int i = 0; i < 11; i ++){
	    	   System.out.println("main thread new cycle:" + i);
//	    	   t.setI(i);
	    	   t.assignTask(i);
//	           while(!t.done){
//	              try {
//	                  Thread.sleep(100);
//	              } catch (InterruptedException e) {
//	              }
//	           }
//	           synchronized(t){
//	              System.out.println("main thread inner:" + System.currentTimeMillis());
//	              t.setI(i);
//	              t.done = false;
//	              t.notify();
//	           }
//	    	   try {
//	                  Thread.sleep(1000);
//	              } catch (InterruptedException e) {
//	              }
	           System.out.println("main thread :" + i);
	       }
//	     t.interrupt();
	       t.setTerminate(true);
	       synchronized(t){
	           t.notify();
	       }

		
		
		
//		final ThreadTest tt = new ThreadTest();
//		Object obj = new Object();
//		MainThread main = new MainThread("thread1", obj);
//		ChildThread child = new ChildThread("thread2", obj);
//		main.start();
//		child.start();
		
//		Thread t = new Thread(){
//			@Override
//			public void run(){
//				while(true){
//					System.out.println("thread run.");
//					try {
//						this.wait();
//					} catch (InterruptedException e) {
//						// TODO Auto-generated catch block
//						e.printStackTrace();
//					}
//				}
//				try {
//					Thread.sleep(1000);
//				} catch (InterruptedException e) {
//					// TODO Auto-generated catch block
//					e.printStackTrace();
//				}
////				synchronized(pool){
//					synchronized(tt){
//						tt.notify();
//					}
//				}
//			}
//		};
//		t.start();
//		tt.lockTest();
//		R r = new R();
//		for (int i = 0; i < 5; i++) {
//			MyThread myThread = new MyThread();
//			myThread.start();
//			Thread thread = new Thread(new ThreadTest(),r);
//			thread.start();
//		}
	}

	private void lockTest(){
		
		synchronized(pool){
			System.out.println("Go into synchronized");
			synchronized(this){
				try {
					this.wait();
					System.out.println("after wait");
				} catch (InterruptedException e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				}
			}
			//System.out.println("out of wait");
		}
	}
	
	public void uncaughtException(Thread thread, Throwable exception) {
		System.out.println("thread:" + thread + "exception:"+exception);
	}
}

class MyThread extends Thread {
	public int x = 0;

	MyThread()
	{
		super();
	}
	
	public void run() {
		System.out.println(++x);
	}
}

class R implements Runnable {
	private int x = 0;

	public void run() {
		System.out.println(++x);
		throw new RuntimeException("exception in runnable.");
	}
}
