package com.station.moudles.service;

import java.util.List;

import com.station.moudles.vo.search.PageEntity;

public interface BaseService<T, TID> {

	/**
	 * 根据主键删除
	 */
	void deleteByPrimaryKey(TID pk);

	/**
	 * 根据对象新增
	 * @return 
	 */
	void insert(T record);

	/**
	 * 根据对象新增不为null的属性
	 * @return 
	 */
	void insertSelective(T record);

	/**
	 * 根据pk查找对象
	 */
	T selectByPrimaryKey(TID pk);

	/**
	 * 根据pk更新对象不为null的属性
	 */
	void updateByPrimaryKeySelective(T record);

	/**
	 * 根据pk更新所有属性
	 */
	void updateByPrimaryKey(T record);

	/**
	 * 根据不为null的属性删除对象
	 * @return 
	 */
	void deleteSelective(T record);

	/**
	 * 根据对象不为null的属性获取对象集合
	 */
	List<T> selectListSelective(T record);

	/**
	 * 根据对象不为null的属性获取分页对象集合
	 */
	List<T> selectListSelectivePaging(PageEntity pageEntity);

	/**
	 * 根据id集合删除多个对象
	 * @param ids	pk以,分隔
	 * @return
	 */
	void deleteByPKs(TID[] ids);

}
