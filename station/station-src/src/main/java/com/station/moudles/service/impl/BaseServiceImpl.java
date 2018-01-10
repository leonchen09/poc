package com.station.moudles.service.impl;

import java.util.List;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import com.station.moudles.mapper.BaseMapper;
import com.station.moudles.service.BaseService;
import com.station.moudles.vo.search.PageEntity;

@Service
public abstract class BaseServiceImpl<T, TID> implements BaseService<T, TID> {

	protected Logger logger= LoggerFactory.getLogger(getClass());

	@Autowired
	BaseMapper<T, TID> baseMapper;

	// String path = GetProperty.GetConfPath(null);
	// String datacenterId = GetProperty.readValue(path, "datacenterId");
	// String workerId = GetProperty.readValue(path, "workerId");

	public void deleteByPrimaryKey(TID id) {
		int rows = baseMapper.deleteByPrimaryKey(id);
		System.out.println("deleteByPrimaryKey删除条数" + rows + ";id=" + id);
	}

	public void deleteSelective(T record) {
		int rows = baseMapper.deleteSelective(record);
		System.out.println("deleteByPrimaryKey删除条数" + rows + ";record=" + record);
	}

	public void deleteByPKs(TID[] ids) {
		baseMapper.deleteByPKs(ids);
	}

	public void insert(T record) {
		baseMapper.insert(record);
	}

	public void insertSelective(T record) {
		baseMapper.insertSelective(record);
	}

	public T selectByPrimaryKey(TID id) {
		T record = (T) baseMapper.selectByPrimaryKey(id);
		return record;
	}

	public List<T> selectListSelective(T record) {
		return baseMapper.selectListSelective(record);
	}

	public List<T> selectListSelectivePaging(PageEntity pageEntity) {
		return baseMapper.selectListSelectivePaging(pageEntity);
	}

	public void updateByPrimaryKeySelective(T record) {
		int rows = baseMapper.updateByPrimaryKeySelective(record);
		System.out.println("updateByPrimaryKeySelective更新条数" + rows + ";record=" + record);
	}

	public void updateByPrimaryKey(T record) {
		int rows = baseMapper.updateByPrimaryKey(record);
		System.out.println("updateByPrimaryKey更新条数" + rows + ";record=" + record);
	}

	// void genratePk(T record) {
	// try {
	//List<Field> fs = ReflectUtil.getAllFields(record.getClass());
	// for (Field f : fs) {
	// f.setAccessible(true);
	//
	// if (record instanceof User) {
	// if (f.getName().equals("userId")) {
	// f.set(record, InitCache.workerId);
	// break;
	// }
	// } else if (record instanceof Project) {
	// if (f.getName().equals("projectId")) {
	// f.set(record, InitCache.workerId);
	// break;
	// }
	// } else {
	// if (f.getName().equals("id")) {
	// long id = SeqGen.getInstance(Long.parseLong(InitCache.workerId),
	// Long.parseLong(InitCache.datacenterId)).nextId();
	// f.set(record, id);
	// break;
	// }
	// }
	// }
	// } catch (Exception e) {
	// e.printStackTrace();
	// }
	// }
	//
	// long genrateId() {
	// long id = SeqGen.getInstance(Long.parseLong(InitCache.workerId),
	// Long.parseLong(InitCache.datacenterId)).nextId();
	// return id;
	// }
}