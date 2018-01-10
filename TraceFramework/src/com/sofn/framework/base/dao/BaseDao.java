package com.sofn.framework.base.dao;

import java.io.Serializable;
import java.util.List;

import com.sofn.framework.base.entity.PaginationSearch;
import com.sofn.framework.base.entity.Searcher;

public interface BaseDao<T, ID extends Serializable> {

	int add(T record);
	
	int updateByID(T record);
	
	int updateSelectiveByID(T record);
	
	int updateBySelective(T record);
	
	int deleteByID(ID id);
	
	int deleteBySelective(T record);
	
	T load(ID id);
	
	List<T> searchByParam(Searcher<T> parameter);
	
	List<T> pageingByParam(PaginationSearch<T> parameter);
	
	int getCount(Searcher<T> parameter);

	
}
