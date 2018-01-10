package com.reachcloud.framework.datasource.cluster;

import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.locks.ReadWriteLock;
import java.util.concurrent.locks.ReentrantReadWriteLock;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;

import com.reachcloud.framework.lb.LoadBalance;

/**
 * 可用的reading data source。本地保存一份，监控zookeeper注册节点的变化，本更新到本地。
 * @author Chenwl
 * @date 2016年7月26日
 */
@Component
public class AvailableRDSImpl implements AvailableRDS{
	//本地保存的可用的data source key
	private List<String> localDSkeys = new ArrayList<String>();
	//读写锁
	private ReadWriteLock lock = new ReentrantReadWriteLock(false);
	
	@Autowired
	private LoadBalance lb;
	
	public AvailableRDSImpl(){
		localDSkeys.add("readDataSource");
		localDSkeys.add("readDataSource1");
		localDSkeys.add("readDataSource2");
		localDSkeys.add("readDataSource3");
		localDSkeys.add("readDataSource4");
		localDSkeys.add("readDataSource5");
	}
	
	public String getNextDatasourceKey(){
		lock.readLock().lock();
		try{
			String curKey = localDSkeys.get(lb.getNext(localDSkeys.size()));
			return curKey;
		}finally{
			lock.readLock().unlock();
		}
	}
	
	public void refreshDatasourceKeys(List<String> keys){
		lock.writeLock().lock();
		try{
			localDSkeys = keys;
		}finally{
			lock.writeLock().unlock();
		}
	}
	
	public List<String> getDatasourceKeys(){
		return localDSkeys;
	}
	
}
