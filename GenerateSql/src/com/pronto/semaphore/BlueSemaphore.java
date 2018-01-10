package com.pronto.semaphore;

import java.util.concurrent.atomic.AtomicInteger;

public class BlueSemaphore {

	public static final int OPEN = 0;
	public static final int CLOSE = 1;
	
	private static AtomicInteger gateFlag = new AtomicInteger(OPEN);
	
	public static void grab(){
		while(!gateFlag.compareAndSet(OPEN, CLOSE)){
			try {
				Thread.sleep(200);
				System.out.println("Gate has been closed, waiting 200ms and try again, " + Thread.currentThread().getName());
			} catch (InterruptedException e) {
			}
		}
	}
	
	public static void release(){
		gateFlag.set(OPEN);
		System.out.println("Gate opened, " + Thread.currentThread().getName());
	}
}
