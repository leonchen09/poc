package com.pronto.omni.thread;

import java.util.concurrent.locks.ReadWriteLock;

public class ReadThread extends Thread{

	public ReadWriteLock lock;
	
	public void run(){
		while(true){
		lock.readLock().lock();
		System.out.println("acquire read lock, " + RWMainThread.num + ", thread:" +getName());
		lock.readLock().lock();
		System.out.println("acquire write lock, " + RWMainThread.num + ", thread:" +getName());
		try {
//			num ++;
			Thread.sleep(100);
		} catch (InterruptedException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}finally{
			System.out.println("release read lock, " + RWMainThread.num + ", thread:" +getName());
			lock.readLock().unlock();
			lock.readLock().unlock();
		}
		}
	}
	
}
