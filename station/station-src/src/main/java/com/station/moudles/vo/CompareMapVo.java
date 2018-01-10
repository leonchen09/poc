package com.station.moudles.vo;

import java.util.Map;

public class CompareMapVo {

	private Map beforeMap;
	private Map afterMap;
	private Map sameMap;

	/**
	 * @return the beforeMap
	 */
	public Map getBeforeMap() {
		return beforeMap;
	}

	/**
	 * @param beforeMap the beforeMap to set
	 */
	public void setBeforeMap(Map beforeMap) {
		this.beforeMap = beforeMap;
	}

	/**
	 * @return the afterMap
	 */
	public Map getAfterMap() {
		return afterMap;
	}

	/**
	 * @param afterMap the afterMap to set
	 */
	public void setAfterMap(Map afterMap) {
		this.afterMap = afterMap;
	}

	/**
	 * @return the sameMap
	 */
	public Map getSameMap() {
		return sameMap;
	}

	/**
	 * @param sameMap the sameMap to set
	 */
	public void setSameMap(Map sameMap) {
		this.sameMap = sameMap;
	}

	public CompareMapVo() {
		super();
	}

	public CompareMapVo(Map beforeMap, Map afterMap, Map sameMap) {
		super();
		this.beforeMap = beforeMap;
		this.afterMap = afterMap;
		this.sameMap = sameMap;
	}

}
