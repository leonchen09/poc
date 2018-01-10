package com.pronto.omni;

import java.util.List;

public class GenerateSQL {

	public String generateSQL(List<Tag> selectedTags) throws Exception
	{
		if(selectedTags.isEmpty())
			throw new Exception("No Tag selected!");
		
		//the length of byte[] is (COLUMN_COUNT+7)/8, we initlize it with 0.
		byte[] selectedColumns = new byte[(ScopedData.getInstance().getScopedColumns().size() + 7) / 8];
		//the length of byte[] is (TABLE_COUNT + 7)/8
		byte[] selectedTables = new byte[(ScopedData.getInstance().getScopedTables().size() + 7) / 8];
		//the finally generated sql.
		StringBuffer sql = new StringBuffer();

		for(Tag tag : selectedTags){
			selectedColumns = Tools.bitOr(selectedColumns, tag.getIdxOfColumnMaps());
			selectedTables = Tools.bitOr(selectedTables, tag.getIdxOfTableMaps());
		}
		generateSELECT(sql, selectedColumns);
		generateFROM(sql, selectedTables);
		generateWHERE(sql);
		generateXMLFORMAT(sql);
		
		return sql.toString();
	}
	
	private void generateSELECT(StringBuffer sql, byte[] selectedColumns){
		sql.append("SELECT ");
		List<Column> allColumns = ScopedData.getInstance().getScopedColumns();
		for(int i = 0; i < allColumns.size(); i ++){
			Column column = allColumns.get(i);
			int byteArrayStart = i / 8;
			byte selectedColumn = selectedColumns[byteArrayStart];
			if(column.getIdxOfMaps() == (byte)(column.getIdxOfMaps() & selectedColumn))
				sql.append(column.getFullName()).append(", ");
		}
		sql.delete(sql.length() - 2, sql.length());
	}
	
	private void generateFROM(StringBuffer sql, byte[] selectedTables){
		sql.append(" FROM ");
		List<Table> allTables = ScopedData.getInstance().getScopedTables();
		for(int i = 0; i < allTables.size(); i ++){
			Table table = allTables.get(i);
			int byteArrayStart = i / 8;
			byte selectedTable = selectedTables[byteArrayStart];
			if(table.getIdxOfMaps() == (byte)(table.getIdxOfMaps() & selectedTable))
				sql.append(table.getJoinClause()).append(" ");
		}
	}
	
	private void generateWHERE(StringBuffer sql){
		sql.append(" WHERE ").append(ScopedData.getInstance().getDomainWhere());
	}
	
	private void generateXMLFORMAT(StringBuffer sql){
		sql.append(" FOR XML AUTO, ELEMENTS");
	}

}
