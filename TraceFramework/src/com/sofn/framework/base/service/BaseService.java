package com.sofn.framework.base.service;

import java.io.Serializable;

import com.sofn.framework.base.entity.PaginationSearch;
import com.sofn.framework.base.entity.Searcher;
/**
 * Service接口
 * @author Chenwl
 * @date 2016年4月22日
 * @param <T>
 * @param <ID>
 */
public interface BaseService <T,ID extends Serializable>{

	void setBaseMapper();
	
	boolean add(T record);
	
	boolean updateByID(T record);
	
	boolean updateSelectiveByID(T record);
	
	int updateBySelective(T record);
	
	boolean deleteByID(ID id);
	
	int deleteBySelective(T record);
	
	T load(ID id);
	
	Searcher<T> searchByParam(Searcher<T> parameter);
	
	PaginationSearch<T> pageingByParam(PaginationSearch<T> parameter);
	
	int getCount(Searcher<T> parameter);
	
}
