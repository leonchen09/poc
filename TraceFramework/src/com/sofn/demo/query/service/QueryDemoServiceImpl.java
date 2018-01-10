package com.sofn.demo.query.service;

import java.util.List;

import javax.annotation.Resource;

import org.springframework.stereotype.Service;

import com.sofn.demo.dao.DemoMapper;
import com.sofn.demo.entity.Demo;
import com.sofn.framework.base.entity.PaginationSearch;
import com.sofn.framework.web.annotation.QueryOnly;

@Service
public class QueryDemoServiceImpl implements QueryDemoService {
	@Resource
	DemoMapper demoMapper;
	
	@Override
	@QueryOnly
	public PaginationSearch<Demo> readList(PaginationSearch<Demo> parameter){
		int totalRecord = demoMapper.getCount(parameter);
		demoMapper.getCount(parameter);
		parameter.setTotalRecord(totalRecord);
		List<Demo> records = demoMapper.pageingByParam(parameter);
		demoMapper.pageingByParam(parameter);
		parameter.setResult(records);
		return parameter;
	}
}
