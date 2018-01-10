package com.pronto.semaphore;

public class SemaphoreTest {
	
	public static void main(String[] argv) throws Exception{
		Thread t1 = new Tester("Thread1");
		Thread t2 = new Tester("Thread2");
		Thread t3 = new Tester("Thread3");
		t1.start();
		t2.start();
		t3.start();
	}
	
}
class Tester extends Thread{
	public Tester(String threadName){
		super(threadName);
	}
	
	public void run(){
		for(int i = 0; i < 10; i ++){
			System.out.println(this.getName()+" runing,try to grab the semaphore");
			BlueSemaphore.grab();
			
			//sleep 500ms to simulate do some work.
			try {
				Thread.sleep(500);
			} catch (InterruptedException e) {
			}
			
			BlueSemaphore.release();
			System.out.println(this.getName()+" release the semaphore");
			//sleep 2 second to simulate do some work.
			try {
				Thread.sleep(300);
			} catch (InterruptedException e) {
			}
		}
	}
}