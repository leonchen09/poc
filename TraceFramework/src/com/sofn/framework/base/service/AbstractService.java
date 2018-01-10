package com.sofn.framework.base.service;

import java.io.Serializable;
import java.util.List;

import org.springframework.beans.factory.annotation.Autowired;

import com.sofn.framework.base.dao.BaseDao;
import com.sofn.framework.base.dao.PubSeqMapper;
import com.sofn.framework.base.entity.PaginationSearch;
import com.sofn.framework.base.entity.Searcher;
/**
 * service基类，所有的service都应该继承至该类。
 * @author Chenwl
 * @date 2016年4月22日
 * @param <T> 实体类
 * @param <ID> 实体id
 */
public abstract class AbstractService<T,ID extends Serializable> implements BaseService<T,ID>{

	private BaseDao<T, ID> baseMapper;
	public void setBaseMapper(BaseDao<T, ID> baseMapper) {
		this.baseMapper = baseMapper;
	}

	@Autowired
	protected PubSeqMapper pubSeqMapper;
	
	@Override
	public boolean add(T record) {
		return baseMapper.add(record) > 0;
	}

	@Override
	public boolean updateByID(T record) {
		return baseMapper.updateByID(record) > 0;
	}

	@Override
	public boolean updateSelectiveByID(T record) {
		return baseMapper.updateSelectiveByID(record) > 0;
	}

	@Override
	public int updateBySelective(T record) {
		return baseMapper.updateBySelective(record);
	}

	@Override
	public boolean deleteByID(ID id) {
		return baseMapper.deleteByID(id) > 0;
	}

	@Override
	public int deleteBySelective(T record) {
		return baseMapper.deleteBySelective(record);
	}

	@Override
	public T load(ID id) {
		return baseMapper.load(id);
	}

	@Override
	public Searcher<T> searchByParam(Searcher<T> parameter) {
		parameter.setResult(baseMapper.searchByParam(parameter));
		return parameter;
	}

	@Override
	public PaginationSearch<T> pageingByParam(PaginationSearch<T> parameter) {
		int totalRecord = baseMapper.getCount(parameter);
		parameter.setTotalRecord(totalRecord);
		List<T> records = baseMapper.pageingByParam(parameter);
		parameter.setResult(records);
		return parameter;
	}

	@Override
	public int getCount(Searcher<T> parameter) {
		return baseMapper.getCount(parameter);
	}

	public int getSeq(String seqName){
		return pubSeqMapper.selectSeq(seqName);
	}

}
