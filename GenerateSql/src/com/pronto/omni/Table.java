package com.pronto.omni;

import java.util.Arrays;

public class Table {
	
	private String alias;
	
	private String tableName;
	
	private Table driverTable;
	
	/*0 for inner join, 1 for left join, 2 for right join, 3 for full outer jion.
	 * NOTICED: 
	 * a.if the driver table is joined by left join, current table must be left join,
	 * otherwise we cannot retrieve any data.
	 * b.Maybe we should not support right join, full outer join, because this join type
	 * can produced null main table.
	 */
	private int joinType;
	
	//two columns, the first is column of the driver table.
	private String[] joinColumns;
	//----------------------------------------------------------------
	//All above properties are seted in define stage, and would not be used 
	//while generating sql. The following will be used to generate sql.
	//----------------------------------------------------------------
	/*
	 * The index of byte array, startPoint = index of scopedTables / 8
	 */
	private int startPoint = 0;
	
	/*
	 * Byte used to indicate the index of the scopedTables.
	 * it's value = i << (index of scopedTables % 8)
	 */
	private byte idxOfMaps = 0;
	
	/*
	 * the finally full join clause from driver table to current table
	 */
	private String joinClause;

	public String getAlias() {
		return alias;
	}

	public void setAlias(String alias) {
		this.alias = alias;
	}

	public String getTableName() {
		return tableName;
	}

	public void setTableName(String tableName) {
		this.tableName = tableName;
	}

	public Table getDriverTable() {
		return driverTable;
	}

	public void setDriverTable(Table driverTable) {
		this.driverTable = driverTable;
	}

	public int getJoinType() {
		return joinType;
	}

	public void setJoinType(int joinType) {
		this.joinType = joinType;
	}

	public String[] getJoinColumns() {
		return joinColumns;
	}

	public void setJoinColumns(String[] joinColumns) {
		this.joinColumns = joinColumns;
	}

	public int getStartPoint() {
		return startPoint;
	}

	public void setStartPoint(int startPoint) {
		this.startPoint = startPoint;
	}

	public byte getIdxOfMaps() {
		return idxOfMaps;
	}

	public void setIdxOfMaps(byte idxOfMaps) {
		this.idxOfMaps = idxOfMaps;
	}

	public String getJoinClause() {
		return joinClause;
	}

	public void setJoinClause(String joinClause) {
		this.joinClause = joinClause;
	}

	@Override
	public String toString() {
		return "Table [alias=" + alias + ", driverTable=" + driverTable
				+ ", idxOfMaps=" + idxOfMaps + ", joinClause=" + joinClause
				+ ", joinColumns=" + Arrays.toString(joinColumns)
				+ ", joinType=" + joinType + ", startPoint=" + startPoint
				+ ", tableName=" + tableName + "]";
	}

}
