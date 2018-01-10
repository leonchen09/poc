package com.reachcloud.demo.controller;

import java.util.concurrent.ArrayBlockingQueue;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.ThreadPoolExecutor;
import java.util.concurrent.TimeUnit;
import java.util.concurrent.locks.ReadWriteLock;
import java.util.concurrent.locks.ReentrantReadWriteLock;

public class ThreadPoolTest {

	public static MyLock lock1 = new MyLock();
	public static MyLock lock2 = new MyLock();
	
	public static void main(String[] args) {
//		ExecutorService executer = Executors.newCachedThreadPool();
//		
//		executer.execute(new MyRunnable());
//		executer.execute(new MyRunnable());
//		executer.execute(new MyRunnable());
//		executer.execute(new MyRunnable());
//		executer.execute(new MyRunnable());
		
		ThreadPoolExecutor executor = new ThreadPoolExecutor(2,2,0,TimeUnit.SECONDS,new ArrayBlockingQueue<Runnable>(1),
				new ThreadPoolExecutor.DiscardOldestPolicy());
		MyRunnable run1 = new MyRunnable("1");
		run1.lock1 = lock1;
		run1.lock2 = lock2;
		MyRunnable run2 = new MyRunnable("2");
		run2.lock1 = lock2;
		run2.lock2 = lock1;
		executor.execute(run1);
		executor.execute(run2);
//		executor.execute(new MyRunnable("1"));
//		executor.execute(new MyRunnable("2"));
//		executor.execute(new MyRunnable("3"));
//		executor.execute(new MyRunnable("4"));
//		executor.execute(new MyRunnable("5"));
		executor.shutdown();
	}
}

class MyRunnable implements Runnable{

	private String runname;
	
	public MyLock lock1;
	public MyLock lock2;
	
	public MyRunnable(){
	}
	
	public MyRunnable(String name){
		runname = name;
	}
	
	
	
	@Override
	public void run() {
		System.out.println("runing, thread name:" + Thread.currentThread().getName()+", name: " + runname);
		lock1.write();
		try {
			Thread.sleep(1000);
		} catch (InterruptedException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		lock2.write();
		while(true)
			;
	}
	
}

class MyLock{
	private ReadWriteLock lockObj = new ReentrantReadWriteLock();
	
	public void read(){
		lockObj.readLock().lock();
	}
	
	public void write(){
		lockObj.writeLock().lock();
	}
	
	public void readEnd(){
		lockObj.readLock().unlock();
	}
	
	public void writeEnd(){
		lockObj.writeLock().unlock();
	}
}