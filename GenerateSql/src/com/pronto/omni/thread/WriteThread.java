package com.pronto.omni.thread;

import java.util.concurrent.locks.ReadWriteLock;

public class WriteThread extends Thread{

	public ReadWriteLock lock;
	
	public void run(){
		while(true){
		lock.writeLock().lock();
		System.out.println("acquire write lock, " + RWMainThread.num + ", thread:" +getName());
		try {
			RWMainThread.num ++;
			Thread.sleep(500);
		} catch (InterruptedException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}finally{
			System.out.println("release write lock, " + RWMainThread.num + ", thread:" +getName());
			lock.writeLock().unlock();
		}
	}
	}
}
	