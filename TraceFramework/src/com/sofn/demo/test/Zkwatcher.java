package com.sofn.demo.test;

import java.util.List;
import java.util.Random;

import org.I0Itec.zkclient.IZkChildListener;
import org.I0Itec.zkclient.ZkClient;
import org.junit.Test;

public class Zkwatcher {

	private static final String PATH = "/ReadingDataSource";
	private static String zkserver = "localhost:2181";

	public static void main(String[] args){
		Zkwatcher zkw = new Zkwatcher();
		zkw.setpath();
		zkw.testZkClient();
	}
	
	
	public void testZkClient(){
		
		ZkClient zk = new ZkClient(zkserver);
		zk.subscribeChildChanges(PATH, new IZkChildListener(){
			@Override
			public void handleChildChange(String parentPath, List currentChilds) throws Exception {
				System.out.println("current childs name:");
				for(int i = 0; i < currentChilds.size(); i ++){
					System.out.println(currentChilds.get(i));
				}
			}
		});
	}
	
	
	public void setpath(){
		ZkClient zk = new ZkClient(zkserver);
		zk.deleteRecursive(PATH);
		zk.createPersistent(PATH);
		for(int i = 0; i < 3; i ++){
			Thread t = new Thread(new Runnable(){

				@Override
				public void run() {
					boolean existed = false;
					while(true){
						try {
							for(int j = 0; j < (new Random()).nextInt(100); j++)
								Thread.currentThread().sleep(1000);
						} catch (InterruptedException e) {
							// TODO Auto-generated catch block
							e.printStackTrace();
						}
						if(!existed){
							zk.createEphemeral(PATH+"/datasource" + Thread.currentThread().getName());
							existed = true;
						}else{
							zk.delete(PATH+"/datasource" + Thread.currentThread().getName());
							existed = false;
						}
					}
				}
				
			});
			t.setName(String.valueOf(i));
			t.start();
		}
	}
}
