package com.station.common;

/**
 * memchache key
 * @author dqzhai
 * @version 1.0
 * @CreateDate 2015-8-19 - 下午03:23:04
 */
public final class MemchacheKey {

	static final String CHACHE_PREFIX = "PMS_";

	public static String getInnKey(Object innId) {
		return CHACHE_PREFIX + innId;
	}
}
