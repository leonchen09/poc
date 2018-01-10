package com.station.moudles.vo;

import java.util.List;

import com.station.moudles.vo.search.PageEntity;

public class ShowPage<T> {

	private Integer pageNo; // 当前页
	private Integer recordCount;// 总记录数
	private Integer pageSize; // 每页记录数
	private List<T> list; // 分页数据集
	
	//这里自己添加的有参数的构造方法
	public ShowPage(PageEntity entity, List<T> list) {
		this.pageNo = entity.getPageNo();
		this.recordCount = entity.getRecordCount();
		this.pageSize = entity.getPageSize();
		this.list = list;
	}

	public Integer getPageNo() {
		return pageNo;
	}

	public void setPageNo(Integer pageNo) {
		this.pageNo = pageNo;
	}

	public Integer getRecordCount() {
		return recordCount;
	}

	public void setRecordCount(Integer recordCount) {
		this.recordCount = recordCount;
	}

	public Integer getPageSize() {
		return pageSize;
	}

	public void setPageSize(Integer pageSize) {
		this.pageSize = pageSize;
	}

	public List<?> getList() {
		return list;
	}

	public void setList(List<T> list) {
		this.list = list;
	}

}
