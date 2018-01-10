package com.sofn.framework.cache;

import java.util.Collection;
import java.util.concurrent.ConcurrentHashMap;
import java.util.concurrent.ConcurrentMap;

import org.springframework.cache.Cache;
import org.springframework.cache.transaction.AbstractTransactionSupportingCacheManager;

import com.danga.MemCached.MemCachedClient;
import com.danga.MemCached.SockIOPool;

public class MemcachedCacheManager extends AbstractTransactionSupportingCacheManager {

	// cache集合，key为cache name，value为cache对象
	private ConcurrentMap<String, Cache> cacheMap = new ConcurrentHashMap<String, Cache>();

	private MemCachedClient memcachedClient;

	//过期时间，小于0表示永远不过期。
	private long expireDate;
	
	public MemcachedCacheManager(SockIOPool sockIOPool, MemCachedClient memcachedClient,Long expireDate) {
		sockIOPool.initialize();
		this.memcachedClient = memcachedClient;
		this.expireDate=expireDate;
	}

	@Override
	protected Collection<? extends Cache> loadCaches() {
		Collection<Cache> caches = cacheMap.values();
		return caches;
	}

	@Override
	public Cache getCache(String name) {
		Cache cache = cacheMap.get(name);
		if (cache == null) {
			cache = new MemCacher(name, memcachedClient,expireDate);
			cacheMap.put(name, cache);
		}
		return cache;
	}

	public MemCachedClient getMemcachedClient() {
		return memcachedClient;
	}

	public void setMemcachedClient(MemCachedClient memcachedClient) {
		this.memcachedClient = memcachedClient;
	}

}
