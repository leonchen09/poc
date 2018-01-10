package com.pronto.omni.thread;

import java.util.concurrent.locks.ReadWriteLock;
import java.util.concurrent.locks.ReentrantReadWriteLock;

public class RWMainThread {
	public static int num = 0;
	
	public static void main(String[] argv){
		ReadWriteLock lock = new ReentrantReadWriteLock();
//		ReadThread r1 = new ReadThread();
//		r1.lock = lock;
//		ReadThread r2 = new ReadThread();
//		r2.lock = lock;
		
		
		WriteThread w1 = new WriteThread();
		w1.lock = lock;
		WriteThread w2 = new WriteThread();
		w2.lock = lock;
		
//		r1.start();
//		r2.start();
//		try {
//			Thread.sleep(2000);
//		} catch (InterruptedException e) {
//		}
		w1.start();
		w2.start();

		

	}
}
