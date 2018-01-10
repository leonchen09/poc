package com.pronto.omni;

import java.util.Arrays;
import java.util.List;

public class Tag {
	//the column of table
	private Column column;

	//expression used to get the finally display value.
	private String expression;
	
	private String tips;
	
	private List<Tag> childs;
	//----------------------------------------------------------------
	//All above properties are only for show, can not be used while generating sql
	//----------------------------------------------------------------
	/*
	 * Column that this tag used, this property indicate index of all columns.
	 * it's max size = (count of columns + 7) /8
	 */
	private byte[] idxOfColumnMaps;
	
	private byte[] idxOfTableMaps;
	
	public byte[] getIdxOfColumnMaps() {
		return idxOfColumnMaps;
	}

	public void setIdxOfColumnMaps(byte[] idxOfColumnMaps) {
		this.idxOfColumnMaps = idxOfColumnMaps;
	}

	public byte[] getIdxOfTableMaps() {
		return idxOfTableMaps;
	}

	public void setIdxOfTableMaps(byte[] idxOfTableMaps) {
		this.idxOfTableMaps = idxOfTableMaps;
	}

	public Column getColumn() {
		return column;
	}

	public void setColumn(Column column) {
		this.column = column;
	}

	public String getExpression() {
		return expression;
	}

	public void setExpression(String expression) {
		this.expression = expression;
	}

	public String getTips() {
		return tips;
	}

	public void setTips(String tips) {
		this.tips = tips;
	}

	public List<Tag> getChilds() {
		return childs;
	}

	public void setChilds(List<Tag> childs) {
		this.childs = childs;
	}
	
	@Override
	public String toString() {
		return "Tag [column=" + column + ", idxOfColumnMaps="
				+ Arrays.toString(idxOfColumnMaps) + ", idxOfTableMaps="
				+ Arrays.toString(idxOfTableMaps) + "]";
	}
	
}
