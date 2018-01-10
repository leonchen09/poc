package com.pronto.omni.folermonitor;

public abstract class RunThread implements Runnable {
	/*
	 * The
	 */
	private int second;
	private String filePath;
	Thread runner;

	public RunThread(int second, String filePath) {
		this.second = second * 1000;
		this.filePath = filePath;
	}

	public void onStart() {
		runner = new Thread(this);
		runner.start();
	}

	public void run() {
		Thread.currentThread().setPriority(Thread.MIN_PRIORITY);
		while (true) {
			try {
				watch(filePath);
				Thread.sleep(this.second);
			} catch (InterruptedException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
		}

	}

	public abstract void watch(String file);

}
