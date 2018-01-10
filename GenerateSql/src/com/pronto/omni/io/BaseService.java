package com.pronto.omni.io;


public abstract class BaseService extends Thread {

	protected boolean hasTask = false;
	private boolean stop = false;


	// create new service must be with one handler.
	public BaseService() {
	}

	/**
	 * before assign task to this service, we must check whether current service
	 * has task or not. If the method return true, the assigner must be wait.
	 * Code example: while(baseService.hasTask()){ Thread.sleep(100); }
	 */

	public boolean hasTask() {
		return this.hasTask;
	}

	/**
	 * assign new task to this service, this method MUST BE CALLED IN
	 * SYNCHRONIZED BLOCK which has lock of this service.
	 */
	protected void assignTask(){
		waitForTaskFinish();
    	synchronized(this){
    		hasTask = true;
	        this.notify();
    	}
    }
	
	protected void waitForTaskFinish(){
    	while(hasTask){
            try {
                Thread.sleep(100);
            } catch (InterruptedException e) {
            }
         }
	}

	/**
	 * this method MUST BE CALLED IN SYNCHRONIZED BLOCK
	 */
	public void stopService() {
		this.stop = true;
	}

	public void run() {
		while (true) {
			synchronized (this) {
				if (stop)
					return;// this service is stoped, just return.

				if (this.hasTask) {
					
					doTask();

					this.hasTask = false;
				}
				try {
					this.wait();// wait for next notify
				} catch (InterruptedException e) {
				}
			}
		}
	}

	protected abstract void doTask();
}
