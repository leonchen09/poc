package com.sofn.demo.service;

import java.util.ArrayList;
import java.util.List;

import javax.annotation.Resource;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.cache.annotation.Cacheable;
import org.springframework.stereotype.Service;

import com.sofn.demo.dao.DemoMapper;
import com.sofn.demo.entity.Demo;
import com.sofn.framework.base.service.AbstractService;

@Service
public class CacheTestServiceImpl extends AbstractService<Demo, Long> implements CacheTestService {
	
	@Resource
	DemoMapper demoMapper;
	
	@Override
	@Autowired
	public void setBaseMapper() {
		super.setBaseMapper(demoMapper);		
	}
	
	@Cacheable("democache")
	public List<Demo> testCacheList(String parentid) {
		System.out.println("get demo list from db");
		List<Demo> result = new ArrayList<Demo>();
		result.add(createone());
		return result;
	}

	@Cacheable(value = "democache")
	public Demo getObj(String id) {
		System.out.println("get demo from db");
		return createone();
	}

	private Demo createone() {
		Demo demo = new Demo();
		demo.setAge(99);
		demo.setId(new Long(1));
		demo.setName("name");
		return demo;
	}
}
