package com.sofn.demo.dao;

import org.springframework.stereotype.Repository;

import com.sofn.demo.entity.Demo;
import com.sofn.framework.base.dao.BaseDao;
import com.sofn.framework.base.entity.Searcher;

@Repository
public interface DemoMapper extends BaseDao<Demo, Long>{
	
}
