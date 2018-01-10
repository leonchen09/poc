package com.pronto.omni;

import java.util.List;

public class ScopedData {
	
	private static ScopedData instance =  new ScopedData();
	/*
	 * all the tables in current domain, the first must be main table, and other tables
	 * should be sorted by join table tree level. for exception:
	 * t1 inner join t2 on t2.t1_id = t1.id
	 * inner join t3 on t3.t2_id = t2.id
	 * inner join t4 on t4.t1_id = t1.id
	 * In join table tree, t1 is the main table, t2 and t4 in the 2nd level and t3 in 3rd
	 * level, so the list should be {t1, t2, t4, t3};
	 * The order in list is very IMPORTED, because database require driver table joined first.
	 */
	private List<Table> scopedTables;
	
	/*
	 * all the columns scoped in domain.
	 */
	private List<Column> scopedColumns;
	
	/*
	 * All the tags that defined, only data tag take into account here, other tags
	 * like condition tag, just have any other properties which would be affect the sql.
	 */
	private List<Tag> tags;
	
	/*
	 * The where clause that defined in create domain stage.
	 */
	private String domainWhere;
	
	private static boolean preProcessed = false;
	
	public static ScopedData getInstance(){
		return instance;
	}
	/*
	 * Preprocess step, convert the table/column index into byte array and generate
	 * sql snippet for every join table(s) and column(s).
	 */
	public void preProcess() throws Exception{
		if(preProcessed)
			return;
		
		processTable();
		processColumn();
		processTag();
		
		preProcessed = true;
	}

	private void processTable() throws Exception{
		if(scopedTables == null || scopedTables.isEmpty())
			throw new Exception("No scoped table(s), please scope it first!");
		//the first table must be main table of the domain.
		Table mainTable = scopedTables.get(0);
		mainTable.setIdxOfMaps((byte)1);
		mainTable.setJoinClause(" " + mainTable.getTableName() + " "  + mainTable.getAlias());
		for(int i = 1; i < scopedTables.size(); i ++){
			Table table = scopedTables.get(i);
			table.setStartPoint(i/8);
			table.setIdxOfMaps(Tools.genByte(i));
			//the driver table is joined by left join, current table must be left join.
			if(table.getDriverTable().getJoinType() == 1)
				table.setJoinType(1);
			switch(table.getJoinType()){
			case 0 :
				table.setJoinClause(" INNER JOIN ");
				break;
			case 1 :
				table.setJoinClause(" LEFT JOIN ");
				break;
			case 2 :
				table.setJoinClause(" RIGHT JOIN ");
				break;
			case 3 :
				table.setJoinClause(" FULL OUTER JOIN ");
				break;
			default :
				throw new Exception("Can not be recognised join type: " + table.getJoinType());
			}
			table.setJoinClause(table.getJoinClause() + table.getTableName() + " " + table.getAlias() 
					+ " ON " + table.getAlias()+"." + table.getJoinColumns()[1] + " = " + table.getDriverTable().getAlias()
					+ "." + table.getJoinColumns()[0]
					);
		}
	}
	
	public void processColumn() throws Exception{
		if(scopedColumns == null || scopedColumns.isEmpty())
			throw new Exception("No column(s) scoped, please scope it first!");
		for(int i = 0; i < scopedColumns.size(); i ++){
			Column column = scopedColumns.get(i);
			column.setStartPoint(i/8);
			column.setIdxOfMaps(Tools.genByte(i));
			column.setFullName(column.getOwnerTable().getAlias() + "." + column.getColumnName());
			if(column.getAliasName() != null && column.getAliasName().length() > 0)
				column.setFullName(column.getFullName() + " " + column.getAliasName());
		}
	}
	
	public void processTag() throws Exception{
		if(tags == null || tags.isEmpty())
			throw new Exception("No tag(s) defined, please define it first!");
		for(int i = 0; i < tags.size(); i ++){
			Tag tag = tags.get(i);
			Column column = tag.getColumn();
			
			byte[] idxOfColumnMaps = new byte[(scopedColumns.size()+7)/8];
			idxOfColumnMaps[column.getStartPoint()] = column.getIdxOfMaps();
			tag.setIdxOfColumnMaps(idxOfColumnMaps);
			
			Table table = column.getOwnerTable();
			byte[] idxOfTableMaps = new byte[(scopedTables.size()+7)/8];
			do{
				byte[] curTableMap = new byte[idxOfTableMaps.length];
				curTableMap[table.getStartPoint()] = table.getIdxOfMaps();
				idxOfTableMaps = Tools.bitOr(idxOfTableMaps, curTableMap);
				table = table.getDriverTable();
			}while(table != null);
			tag.setIdxOfTableMaps(idxOfTableMaps);
		}
	}
	
	public List<Table> getScopedTables() {
		return scopedTables;
	}

	public void setScopedTables(List<Table> scopedTables) {
		this.scopedTables = scopedTables;
	}

	public List<Column> getScopedColumns() {
		return scopedColumns;
	}

	public void setScopedColumns(List<Column> scopedColumns) {
		this.scopedColumns = scopedColumns;
	}

	public List<Tag> getTags() {
		return tags;
	}
	public void setTags(List<Tag> tags) {
		this.tags = tags;
	}
	
	public String getDomainWhere() {
		return domainWhere;
	}

	public void setDomainWhere(String domainWhere) {
		this.domainWhere = domainWhere;
	}
	
}
