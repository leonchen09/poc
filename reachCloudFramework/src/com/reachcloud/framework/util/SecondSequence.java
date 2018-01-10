package com.reachcloud.framework.util;
/**
 * 按秒产生订单号
 * @author Chenwl
 *
 */
public class SecondSequence {

	/**
	 * 按秒产生订单号，以秒为基础，后面增加4位序列号。
	 * @return
	 */
	public static String getOrderNumber(){
		long timestamp = System.currentTimeMillis();
		timestamp = timestamp / 1000;
		String sequence = SequenceNumber.getInstance().getNextStr();
		String result = String.valueOf(timestamp) + sequence;
		return result;
	}
	
	public static void main(String[] argv) throws InterruptedException{
		for(int i = 0; i < 10010; i ++){
			if(i%100==0) {
				Thread.currentThread();
				Thread.sleep(100);
			}
			System.out.println(getOrderNumber());
		}
	}
	
}
