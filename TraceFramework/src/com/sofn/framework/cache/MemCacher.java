package com.sofn.framework.cache;

import java.util.Date;
import java.util.HashSet;
import java.util.Set;

import org.springframework.cache.Cache;
import org.springframework.cache.support.SimpleValueWrapper;

import com.danga.MemCached.MemCachedClient;

public class MemCacher implements Cache {

	// Memchache中的所有真实key
	private Set<String> keySet = new HashSet<String>();
	// spring cache name。
	private final String name;
	// memcached client
	private MemCachedClient memcachedClient;
	// 同步
	private Object lock = new Object();
	
	//过期时间
	private long expireDate;
	
	public MemCacher(String name, MemCachedClient memcachedClient, long expireDate) {
		this.name = name;
		this.memcachedClient = memcachedClient;
		this.expireDate=expireDate;
	}

	@Override
	public void clear() {
		for (String key : keySet) {
			memcachedClient.delete(key);
		}
		keySet.clear();
	}

	@Override
	public void evict(Object key) {
		this.delete(key.toString());
		this.keySet.remove(genUniqueKey(key.toString()));
	}

	@Override
	public ValueWrapper get(Object key) {
		ValueWrapper wrapper = null;
		String newkey = this.genUniqueKey(key.toString());
		Object value = memcachedClient.get(newkey);
		if (value != null) {
			wrapper = new SimpleValueWrapper(value);
		}
		return wrapper;
	}

	@Override
	@SuppressWarnings("unchecked")
	public <T> T get(Object key, Class<T> type) {
		String newkey = this.genUniqueKey(key.toString());
		Object cacheValue = memcachedClient.get(newkey);
		Object value = (cacheValue != null ? cacheValue : null);
		if (type != null && !type.isInstance(value)) {
			throw new IllegalStateException("Cached value is not of required type [" + type.getName() + "]: " + value);
		}
		return (T) value;
	}

	@Override
	public String getName() {
		return this.name;
	}

	@Override
	public Object getNativeCache() {
		return this.memcachedClient;
	}

	@Override
	public void put(Object key, Object value) {
		if (value == null)
			return;
		String newkey = this.genUniqueKey(key.toString());
		if(expireDate>0){
			this.memcachedClient.set(newkey, value,new Date(expireDate));
		}else{
			this.memcachedClient.set(newkey, value);
		}
		//boolean b=(expireDate>0) ? this.memcachedClient.add(newkey, value,new Date(expireDate)) : this.memcachedClient.add(newkey, value);
		keySet.add(newkey);
	}

	@Override
	public ValueWrapper putIfAbsent(Object key, Object value) {
		synchronized (lock) {
			ValueWrapper curValue = get(key);
			if (curValue == null) {
				put(key, value);
			}
			return curValue;
		}
	}

	public void delete(String key) {
		memcachedClient.delete(this.genUniqueKey(key));
	}

	/**
	 * 根据cache的name和key生产唯一的内部key，用这个key在memcache中保存数据。
	 * 
	 * @param key
	 * @return
	 */
	private String genUniqueKey(String key) {
		return name + "_" + key;
	}
}
