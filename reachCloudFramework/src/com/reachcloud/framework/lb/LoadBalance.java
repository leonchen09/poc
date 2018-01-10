package com.reachcloud.framework.lb;
/**
 * 负责均衡算法。
 * @author Chenwl
 * @date 2016年7月26日
 */
public interface LoadBalance {

	/**
	 * 在total count中进行负载均衡算法，获得下一个值。
	 * @param totalCount
	 * @return
	 */
	public int getNext(int totalCount);
	
}
