package com.reachcloud.demo.service;

import java.util.List;

import javax.annotation.Resource;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.cache.annotation.Cacheable;
import org.springframework.scheduling.annotation.Scheduled;
import org.springframework.stereotype.Service;

import com.reachcloud.demo.dao.DemoMapper;
import com.reachcloud.demo.entity.Demo;
import com.reachcloud.framework.base.entity.PaginationSearch;
import com.reachcloud.framework.base.service.AbstractService;
import com.reachcloud.framework.web.annotation.QueryOnly;


@Service
public class DemoServiceImpl extends AbstractService<Demo, Long> implements DemoService {
	
	@Resource
	DemoMapper demoMapper;
	

	@Override
	@Autowired
	public void setBaseMapper() {
		super.setBaseMapper(demoMapper);		
	}

	public List<Demo> testCacheList(String parentid){
		System.out.println("get demo list from db");
		PaginationSearch<Demo> search = new PaginationSearch<Demo>();
		search.addParameter("name", "%");
		return this.searchByParam(search).getResult();
	}

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
	public PaginationSearch<Demo> readList(PaginationSearch<Demo> parameter) {
		parameter.setResult(demoMapper.searchByParam(parameter));
		return parameter;
	}

	@Scheduled(cron = "0/10 * * * * ?")  
	public void testTask(){
		System.out.println("task running.");
	}

//	@Override
//	@QueryOnly
//	public PaginationSearch<Demo> readList(PaginationSearch<Demo> parameter) {
//		int totalRecord = readDemoMapper.getCount(parameter);
//		readDemoMapper.getCount(parameter);
//		parameter.setTotalRecord(totalRecord);
//		List<Demo> records = readDemoMapper.pageingByParam(parameter);
//		readDemoMapper.pageingByParam(parameter);
//		parameter.setResult(records);
//		return parameter;
//	}
}
