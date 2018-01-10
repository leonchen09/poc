package com.sofn.demo.service;

import java.util.List;

import javax.annotation.Resource;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.cache.annotation.Cacheable;
import org.springframework.stereotype.Component;
import org.springframework.stereotype.Service;

import com.sofn.demo.dao.DemoMapper;
import com.sofn.demo.entity.Demo;
import com.sofn.demo.query.dao.ReadDemoMapper;
import com.sofn.framework.base.entity.PaginationSearch;
import com.sofn.framework.base.service.AbstractService;
import com.sofn.framework.web.annotation.QueryOnly;

@Service
public class DemoServiceImpl extends AbstractService<Demo, Long> implements DemoService {
	
	@Resource
	DemoMapper demoMapper;
	
	@Resource
	ReadDemoMapper readDemoMapper;

	@Override
	@Autowired
	public void setBaseMapper() {
		super.setBaseMapper(demoMapper);		
	}
	
	@Cacheable("democache")
	public List<Demo> testCacheList(String parentid){
		System.out.println("get demo list from db");
		PaginationSearch<Demo> search = new PaginationSearch<Demo>();
		search.addParameter("name", "%");
		return this.searchByParam(search).getResult();
	}
	@Cacheable(value="democache")
	public Demo getObj(int id){
		System.out.println("get demo from db");
		Demo demo = new Demo();
		demo.setAge(99);
		demo.setId(new Long(1));
		demo.setName("name");
		return demo;
	}
	
	public boolean addtestServiceTx(Demo demo){
		demoMapper.add(demo);
//		demo.setId(new Long(100));
		if(1==1)
			throw new RuntimeException("a");
		demoMapper.add(demo);
		return false;
	}

	@Override
	@QueryOnly
	public PaginationSearch<Demo> readList(PaginationSearch<Demo> parameter) {
		int totalRecord = readDemoMapper.getCount(parameter);
		readDemoMapper.getCount(parameter);
		parameter.setTotalRecord(totalRecord);
		List<Demo> records = readDemoMapper.pageingByParam(parameter);
		readDemoMapper.pageingByParam(parameter);
		parameter.setResult(records);
		return parameter;
	}
}
