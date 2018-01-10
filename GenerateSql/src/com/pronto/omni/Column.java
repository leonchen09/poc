package com.pronto.omni;

public class Column {

	private String columnName;
	
	private String aliasName;
	
	private Table ownerTable;
	
	//----------------------------------------------------------------
	//All above properties are seted in define stage, and would not be used 
	//while generating sql. The following will be used to generate sql.
	//----------------------------------------------------------------
	/*
	 * The index of byte array, startPoint = index of scopedColumns / 8
	 */
	private int startPoint = 0;
	
	/*
	 * Byte used to indicate the index of the scopedColumns.
	 * it's value = i << (index of scopedColumns % 8)
	 */
	private byte idxOfMaps = 0;
	
	/*
	 * the finally full clause, include table alias, column name, column alias
	 */
	private String fullName;

	public String getColumnName() {
		return columnName;
	}

	public void setColumnName(String columnName) {
		this.columnName = columnName;
	}

	public String getAliasName() {
		return aliasName;
	}

	public void setAliasName(String aliasName) {
		this.aliasName = aliasName;
	}

	public Table getOwnerTable() {
		return ownerTable;
	}

	public void setOwnerTable(Table ownerTable) {
		this.ownerTable = ownerTable;
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

	public String getFullName() {
		return fullName;
	}

	public void setFullName(String fullName) {
		this.fullName = fullName;
	}

	@Override
	public String toString() {
		return "Column [aliasName=" + aliasName + ", columnName=" + columnName
				+ ", fullName=" + fullName + ", idxOfMaps=" + idxOfMaps
				+ ", ownerTable=" + ownerTable + ", startPoint=" + startPoint
				+ "]";
	}


}
