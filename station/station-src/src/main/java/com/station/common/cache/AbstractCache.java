package com.station.common.cache;

import com.google.common.cache.CacheBuilder;
import com.google.common.cache.CacheLoader;
import com.google.common.cache.LoadingCache;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.util.List;
import java.util.Map;
import java.util.concurrent.ExecutionException;
import java.util.concurrent.TimeUnit;

/**
 * Created by Jack on 9/16/2017.
 */
public abstract class AbstractCache<K, V> {

    protected Logger logger = LoggerFactory.getLogger(getClass());
    protected LoadingCache<K, V> cache;

    /**
     * @param ttl  time to live
     * @param unit the time unit
     */
    public AbstractCache(long ttl, TimeUnit unit) {
        this.cache = CacheBuilder.newBuilder()
                .refreshAfterWrite(ttl, unit)
                .build(new CacheLoader<K, V>() {
                    @Override
                    public V load(K key) throws Exception {
                        return loadContent(key);
                    }
                });
    }

    protected abstract V loadContent(K key);

    public V get(K key) throws ExecutionException {
        return cache.get(key);
    }

    public V get(K key, V defaultValue) {
        try {
            return get(key);
        } catch (ExecutionException e) {
            logger.error("从本地缓存获取失败，将返回指定的默认值，键值:" + key, e);
            return defaultValue;
        }
    }

    public void put(K key, V value) {
        cache.put(key, value);
    }

    public void remove(K key) {
        cache.invalidate(key);
    }

    public void remove(Iterable<K> keys) {
        cache.invalidateAll(keys);
    }

    public void clear() {
        cache.invalidateAll();
    }
    
    public Map<K, V> getAll(List<K> ids){
    	try {
			return cache.getAll(ids);
		} catch (ExecutionException e) {
			logger.error("从本地缓存获取失败,返回空." , e);
			return null;
		}
    }
}
