package com.reachcloud.framework.util;

import java.util.concurrent.atomic.AtomicInteger;
/**
 * 顺序产出流水号
 * @author Chenwl
 *
 */
public class SequenceNumber {

	//序列号最大值
	private static final int MaxValue = 9999;
	
	//单例对象
	private static SequenceNumber instance = new SequenceNumber();
	
	//当前流水号
	private static AtomicInteger curNumber = new AtomicInteger(0);
	
	private SequenceNumber(){
		//do nothing
	}
	
	//返回当前唯一对象。
	public static SequenceNumber getInstance(){
		return instance;
	}
	/**
	 * 按序获得唯一编号
	 * @return
	 */
	public int getNext(){
		int result;
		result = curNumber.addAndGet(1);
		//todo,线程加锁，防止重号
		if(result > MaxValue){
			result = 0;
			curNumber.set(0);
		}
		return result;
	}
	/**
	 * 按长度返回序号字符串，长度为4位，不够在左边补0
	 * @return
	 */
	public String getNextStr(){
		int next = getNext();
		StringBuffer sb = new StringBuffer("000").append(next);
		String result = sb.toString();
		result = result.substring(sb.length() - 4);
		return result;
	}
}
