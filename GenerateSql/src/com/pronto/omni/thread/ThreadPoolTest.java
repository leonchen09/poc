package com.pronto.omni.thread;

import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.ArrayBlockingQueue;
import java.util.concurrent.ThreadPoolExecutor;
import java.util.concurrent.TimeUnit;

public class ThreadPoolTest {
	long num = 0;
	Object lock = new Object();
	List<Integer> allInts = new ArrayList<Integer>();
	/**
	 * @param args
	 * @throws InterruptedException 
	 */
	public static void main(String[] args) throws InterruptedException {
		ThreadPoolTest test = new ThreadPoolTest();
		ArrayBlockingQueue<Runnable> queue = new ArrayBlockingQueue<Runnable>(2);
		ThreadPoolExecutor threadFixedPool = new ThreadPoolExecutor(6,6,60,TimeUnit.SECONDS,queue);
		for(int i = 0; i < 2; i ++){
			System.out.println("count: " + threadFixedPool.getActiveCount());
			threadFixedPool.execute(test.new Reader(0));
			threadFixedPool.execute(test.new Writer());
			threadFixedPool.execute(test.new Reader(1000));
			threadFixedPool.execute(test.new Reader(100000));
			Thread.sleep(1000);
			System.out.println("count: " + threadFixedPool.getActiveCount());
		}
//		threadFixedPool.execute(test.new ListAdder());
//		threadFixedPool.execute(test.new ListAdder2());
	}

	class Reader implements Runnable{
		private int num;
		public Reader(int num){
			this.num = num;
		}
		public void run() {
			while(true){
				long temp = num;
				//System.out.println("Read number:" + temp +", thread:"+Thread.currentThread().getName());
				try {
					Thread.sleep(10);
				} catch (InterruptedException e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				}
			}
			
		}
		
	}
	class Writer implements Runnable{

		public void run() {
			while(num < 50){
				synchronized(lock){
					num ++;
				}
				try {
					Thread.sleep(10);
					//System.out.println("write thread:"+Thread.currentThread().getName());
				} catch (InterruptedException e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				}
			}
			
		}
		
	}
	
	class ListAdder implements Runnable{

		//@Override
		public void run() {
			for (int i = 0; i < 5000; i ++){
				allInts.add(i);
			}
			System.out.println("r1,count:" + allInts.size());
		}
	}
	
	class ListAdder2 implements Runnable{

		//@Override
		public void run() {
			for (int i = 5000; i < 10000; i ++){
				allInts.add(i);
			}
			System.out.println("r2,count:" + allInts.size());
		}
	}
}
