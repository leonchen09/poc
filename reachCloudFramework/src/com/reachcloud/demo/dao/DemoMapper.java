package com.reachcloud.demo.dao;

import org.springframework.stereotype.Repository;

import com.reachcloud.demo.entity.Demo;
import com.reachcloud.framework.base.dao.BaseDao;

@Repository
public interface DemoMapper extends BaseDao<Demo, Long>{
	
}
