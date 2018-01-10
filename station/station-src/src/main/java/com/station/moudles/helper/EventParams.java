package com.station.moudles.helper;

public class EventParams {
	/**
	 *  当前事件，最少查找个数
	 */
	public int currentCount = 10;
	/**
	 *  向前查找非当前事件的个数
	 */
	public int forwardCount = 10;
	/**
	 *  向后查找非当前事件的个数
	 */
	public int backwardCount = 0;
}
