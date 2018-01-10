package com.sofn.demo.query.dao;

import java.util.List;

import org.springframework.stereotype.Repository;

import com.sofn.demo.entity.Demo;
import com.sofn.framework.base.dao.BaseDao;
import com.sofn.framework.base.entity.PaginationSearch;
import com.sofn.framework.base.entity.Searcher;

@Repository
public interface ReadDemoMapper extends BaseDao<Demo, Long> {

	List<Demo> searchByParam(Searcher<Demo> parameter);
	
	List<Demo> pageingByParam(PaginationSearch<Demo> parameter);
	
	int getCount(Searcher<Demo> parameter);
	
}
