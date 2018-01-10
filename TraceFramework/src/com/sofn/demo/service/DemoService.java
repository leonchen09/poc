package com.sofn.demo.service;

import java.util.List;

import com.sofn.demo.entity.Demo;
import com.sofn.framework.base.entity.PaginationSearch;
import com.sofn.framework.base.service.BaseService;

public interface DemoService extends BaseService<Demo, Long> {

	public List<Demo> testCacheList(String parentid);
	public Demo getObj(int id);
	
	public boolean addtestServiceTx(Demo demo);
	
	PaginationSearch<Demo> readList(PaginationSearch<Demo> parameter);
}
