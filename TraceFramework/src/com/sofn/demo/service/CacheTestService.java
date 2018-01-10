package com.sofn.demo.service;

import java.util.List;

import com.sofn.demo.entity.Demo;
import com.sofn.framework.base.service.BaseService;

public interface CacheTestService  extends BaseService<Demo, Long>{

	public List<Demo> testCacheList(String parentid);
	
	public Demo getObj(String id);
	
}
