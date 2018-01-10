package com.reachcloud.demo.service;

import java.util.List;

import com.reachcloud.demo.entity.Demo;
import com.reachcloud.framework.base.entity.PaginationSearch;
import com.reachcloud.framework.base.service.BaseService;

public interface DemoService extends BaseService<Demo, Long> {

	public List<Demo> testCacheList(String parentid);
	public Demo getObj(int id);
	
	public boolean addtestServiceTx(Demo demo);
	
	PaginationSearch<Demo> readList(PaginationSearch<Demo> parameter);
}
