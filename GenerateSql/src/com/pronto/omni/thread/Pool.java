package com.pronto.omni.thread;

import java.util.ArrayList;
import java.util.List;

public class Pool {

	static List<Object> pool = new ArrayList<Object>();
	
	public synchronized static void add(Object obj){
		System.out.println("add object to pool");
		pool.add(obj);
	}
	
	public synchronized static void remove(Object obj){
		System.out.println("remove object from pool");
		pool.remove(obj);
	}
}
