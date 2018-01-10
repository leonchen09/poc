package com.pronto.omni.search;

import java.util.ArrayList;
import java.util.List;

public class SearchInfo {
	public static final long ipx_RecordSet 		= 1;
	public static final long ipx_GroupCountOnly = 9;
	
	
	private long searchResult;
	//column name for collation, default size is 0;
	private List<String> orderColumns = new ArrayList<String>();
	//asc, desc or null(default)
	private List<String> order = new ArrayList<String>();
	
	//result index start and end, default value is -1, which mean all result will be return
	private int resultIndexStart = -1;
	private int resultIndexEnd = -1;
	
	//second part of the subset, default value is null;
	private String selectedCount;
	//SQL to retrieve data from database
	private String selectCaluse;
	
	private String fromCaluse;
	//default value for where, groupby, orderby is "".
	private String whereCaluse = "";
	
	private String groupCaluse = "";
	
	private String orderCaluse = "";
	
	public List<String> getOrderColumns() {
		return orderColumns;
	}

	public void setOrderColumns(List<String> orderColumns) {
		this.orderColumns = orderColumns;
	}

	public List<String> getOrder() {
		return order;
	}

	public void setOrder(List<String> order) {
		this.order = order;
	}

	public int getResultIndexStart() {
		return resultIndexStart;
	}

	public void setResultIndexStart(int resultIndexStart) {
		this.resultIndexStart = resultIndexStart;
	}

	public int getResultIndexEnd() {
		return resultIndexEnd;
	}

	public void setResultIndexEnd(int resultIndexEnd) {
		this.resultIndexEnd = resultIndexEnd;
	}

	public String getSelectedCount() {
		return selectedCount;
	}

	public void setSelectedCount(String selectedCount) {
		this.selectedCount = selectedCount;
	}

	public String getSelectCaluse() {
		return selectCaluse;
	}

	public void setSelectCaluse(String selectCaluse) {
		this.selectCaluse = selectCaluse;
	}

	public String getFromCaluse() {
		return fromCaluse;
	}

	public void setFromCaluse(String fromCaluse) {
		this.fromCaluse = fromCaluse;
	}

	public String getWhereCaluse() {
		return whereCaluse;
	}

	public void setWhereCaluse(String whereCaluse) {
		this.whereCaluse = whereCaluse;
	}

	public long getSearchResult() {
		return searchResult;
	}

	public void setSearchResult(long searchResult) {
		this.searchResult = searchResult;
	}

	public String getGroupCaluse() {
		return groupCaluse;
	}

	public void setGroupCaluse(String groupCaluse) {
		this.groupCaluse = groupCaluse;
	}

	public String getOrderCaluse() {
		return orderCaluse;
	}

	public void setOrderCaluse(String orderCaluse) {
		this.orderCaluse = orderCaluse;
	}

}
