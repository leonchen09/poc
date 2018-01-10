package com.reachcloud.framework.base.entity;

import java.util.HashMap;
import java.util.List;
import java.util.Map;

public class PaginationSearch<T> extends Searcher<T>{
	
	/**
	 * 
	 */
	private static final long serialVersionUID = 653558108564288344L;

	public static int DEFAULT_PAGE_SIZE = 20; //缺省每页条数
	
	private int pageNo;          //页码
	private int pageSize;        //每页条数  
	private int totalRecord;     //总记录数  
	
	private int start; 			//分页查询时，记录的开始位置。
	private int end;			//分页查询是，记录的结束位置。
	private int totalPage;        //总页数
	


	public PaginationSearch(){
		this.pageNo = 1;
		this.pageSize = DEFAULT_PAGE_SIZE;
	}
	
	public PaginationSearch(int pageNo, int pageSize){
		this.pageNo = pageNo;
		this.pageSize = pageSize;
	}
	
	public PaginationSearch(int pageNo, int pageSize, Map<String, Object> parameters){
		super(parameters);
		this.pageNo = pageNo;
		this.pageSize = pageSize;
	}
	
	public PaginationSearch(int pageNo, int pageSize, int totalRecord){
		this.pageNo = pageNo;
		this.pageSize = pageSize;
		this.totalRecord = totalRecord;
	}
	
	public PaginationSearch(int pageNo, int pageSize, List<T> result){
		this.pageNo = pageNo;
		this.pageSize = pageSize;
		setResult(result);
	}
	
	public int getPageNo() {
		return pageNo;
	}
	public void setPageNo(int pageNo) {
		this.pageNo = pageNo;
	}
	public int getPageSize() {
		return pageSize;
	}
	public void setPageSize(int pageSize) {
		this.pageSize = pageSize;
	}
	public int getTotalRecord() {
		return totalRecord;
	}
	/**
	 * 设置总记录数，计算总页数，记录的开始位置，结束位置。
	 * @param totalRecord
	 */
	public void setTotalRecord(int totalRecord) {
		this.totalRecord = totalRecord;
		calc();
	}
	
	
	public int getTotalPage() {
		return totalPage;
	}
	/**
	 * 获得开始位置
	 * @return
	 */
	public int getStart(){
		return start;
	}
	/**
	 * 获得介绍位置
	 * @return
	 */
	public int getEnd(){
		return end;
	}
	  
	private void calc(){
		totalPage = (totalRecord + pageSize - 1)/pageSize;
		if(totalPage < pageNo){
			pageNo = totalPage;
		}
		if(pageNo < 1)
			pageNo = 1;
		start = pageSize * (pageNo - 1) + 1;
		end = pageSize * pageNo;
		if(end > totalRecord)
			end = totalRecord;
	}

}
