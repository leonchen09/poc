package com.pronto.omni.thread;

public class NetworkThread extends Thread{
    
    private int i = -1;
    private boolean terminate;
    public boolean done = false;
    
    public int getI() {
       return i;
    }

    public void assignTask(int i){
    	while(!done){
            try {
                Thread.sleep(100);
            } catch (InterruptedException e) {
            }
         }
    	synchronized(this){
	    	done = false;
	    	setI(i);
	        this.notify();
    	}
    }
    
    public void setI(int i) {
       this.i = i;
    }

    public boolean isTerminate() {
       return terminate;
    }

    public void setTerminate(boolean terminate) {
       this.terminate = terminate;
    }

    public void run(){
       while(true){
           synchronized(this){
              if(terminate)
                  return;
              if(!this.done){
                  try {
                      Thread.sleep(3000);
//                    long j = 0;
//                    while(j < 900000000l)
//                       j++;
                  } catch (InterruptedException e) {
                      e.printStackTrace();
                  }
                  this.done = true;
              }
              
//            System.out.println("networkthread run."+i);
              try {
                  this.wait();
              } catch (InterruptedException e) {
                  e.printStackTrace();
              }
              System.out.println("networkthread end."+i);
           }
       }
    }

}

