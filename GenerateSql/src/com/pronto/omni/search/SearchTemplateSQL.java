package com.pronto.omni.search;

public class SearchTemplateSQL {
	
	public static void main(String[] argv) throws Exception{
//		SearchTemplateSQL srchTpl = new SearchTemplateSQL();
//		
//		srchTpl.prepareSearch(9l,"cat.a&subcat&templatename",";X0");
//		
//		srchTpl.genSearchSQL();
//		
//		SearchInfo si = WKL.searchInfo.get();
//		String sql = si.getSelectCaluse() + si.getFromCaluse() + si.getWhereCaluse() + si.getOrderCaluse();
//		System.out.println(sql);
		String absolutePath = "d:\\eeee\\1111.jsp";
		char separator = '\\';
		String part1 = absolutePath.substring(0, absolutePath.lastIndexOf(separator));
		String fileName = absolutePath.substring(absolutePath.lastIndexOf(separator));
		String folderName = part1.substring(part1.lastIndexOf(separator) + 1);
		System.out.println(folderName + "," + fileName);
	}
	//validate all the parameters, translate and save them to WKL.
	private void prepareSearch(long searchResult, String collation, String subSet) throws Exception{
		SearchInfo searchInfo = new SearchInfo();
		searchInfo.setSearchResult(searchResult);
		
		if(collation != null && collation.length() > 0){
			String[] collations = collation.split("&");
			for(String temp : collations){
				int splitIndex = temp.lastIndexOf(".")>0?temp.lastIndexOf("."):temp.length();
				String column = temp.substring(0, splitIndex);
				String order = temp.substring(splitIndex);
				searchInfo.getOrderColumns().add(column);
				if(".A".equalsIgnoreCase(order)){
					searchInfo.getOrder().add(" ASC");
				}else if(".D".equalsIgnoreCase(order)){
					searchInfo.getOrder().add(" DESC");
				}else{
					searchInfo.getOrder().add("");
				}
			}
		}
		
		if(subSet != null && subSet.length() > 0){
			String[] sets = subSet.split(";");
			if(sets.length < 1)
				throw new Exception("Invalidate subSet parameter");
			if(sets[0].length() > 0){
				searchInfo.setResultIndexStart(Integer.parseInt(sets[0].split("-")[0]));
				searchInfo.setResultIndexEnd(Integer.parseInt(sets[0].split("-")[1]));
			}
			if(sets.length > 1 && sets[1] != null){
				if(sets[1].equalsIgnoreCase("X0")){
					searchInfo.setSelectedCount( " > 0");
				}else if(sets[1].equalsIgnoreCase("0")){
					searchInfo.setSelectedCount(" = 0");
				}else{
					throw new Exception("the second part of subset is invlidate, should be X0 or 0");
				}
			}
		}
		//save to WKL
		WKL.searchInfo.set(searchInfo);
	}
	
	
	private void genSearchSQL() throws Exception{
		buildSelectCaluse();
		buildFromCaluse();
		buildWhereCaluse();
		buildGroupCaluse();
		buildOrderCaluse();
		execCount();
		execPage();
	}
	
	private void buildSelectCaluse() throws Exception{
		SearchInfo searchInfo = WKL.searchInfo.get();
		StringBuffer sql = new StringBuffer("SELECT ");
		StringBuffer innerWhere = new StringBuffer();
		if(searchInfo.getSearchResult() == SearchInfo.ipx_GroupCountOnly){
			for(int i = 0 ; i < searchInfo.getOrderColumns().size(); i ++){
				String column = searchInfo.getOrderColumns().get(i);
				if(i > 0)
					innerWhere.append(" AND ");
				if (i == searchInfo.getOrderColumns().size() - 1)
					sql.append(column).append(", COUNT(*) AS ").append(column).append("count ");
				else{
					innerWhere.append(" innertmp.").append(column).append(" = tmp.").append(column);
					sql.append(column).append(", ");
					sql.append("(SELECT COUNT(*) FROM template as innertmp WHERE ").append(innerWhere).append(") AS ").append(column).append("count, ");
				}
			}
		}else{
			//read select columns from parameters
			throw new Exception("should be read selected columns from parameters");
		}
		searchInfo.setSelectCaluse(sql.toString());
	}
	
	private void buildFromCaluse(){
		WKL.searchInfo.get().setFromCaluse(" FROM template AS tmp ");
	}
	
	private void buildWhereCaluse(){
		//return all the template
		WKL.searchInfo.get().setWhereCaluse(" WHERE templatename like '%'");
	}
	
	private void buildGroupCaluse(){
		SearchInfo searchInfo = WKL.searchInfo.get();
		if(searchInfo.getSearchResult() != SearchInfo.ipx_GroupCountOnly || searchInfo.getOrderColumns().size() < 1)
			return;
		StringBuffer sql = new StringBuffer(" GROUP BY ");
		for(int i = 0; i <searchInfo.getOrderColumns().size(); i ++){
			sql.append(searchInfo.getOrderColumns().get(i)).append(", ");
		}
		sql.delete(sql.length() - 2, sql.length());
		searchInfo.setGroupCaluse(sql.toString());
	}
	
	private void buildOrderCaluse(){
		SearchInfo searchInfo = WKL.searchInfo.get();
		if(searchInfo.getOrderColumns().size() < 1)
			return;
		StringBuffer sql = new StringBuffer(" ORDER BY ");
		for(int i = 0; i <searchInfo.getOrderColumns().size(); i ++){
			sql.append(searchInfo.getOrderColumns().get(i)).append(" ").append(searchInfo.getOrder().get(i)).append(", ");
		}
		sql.delete(sql.length() - 2, sql.length());
		searchInfo.setOrderCaluse(sql.toString());
	}
	
	private void execPage(){
		SearchInfo searchInfo = WKL.searchInfo.get();
		if(searchInfo.getResultIndexStart() == -1)
			return;
		StringBuffer select = new StringBuffer(searchInfo.getSelectCaluse());
		select.append(", row_number() over(").append(searchInfo.getOrderCaluse()).append(") AS rowid ");
		StringBuffer from = new StringBuffer();
		from.append(" FROM ( ").append(select).append(searchInfo.getFromCaluse()).append(searchInfo.getWhereCaluse())
			.append(searchInfo.getGroupCaluse()).append(" ) AS t ");
		searchInfo.setFromCaluse(from.toString());
		StringBuffer where = new StringBuffer();
		where.append(" WHERE rowid BETWEEN ").append(searchInfo.getResultIndexStart()).append(" AND ").append(searchInfo.getResultIndexEnd());
		searchInfo.setWhereCaluse(where.toString());
		//remove order caluse, because has been used in row number function.
		searchInfo.setOrderCaluse("");
		searchInfo.setSelectCaluse("SELECT * ");
	}
	//retrieve special group count 
	private void execCount(){
		SearchInfo searchInfo = WKL.searchInfo.get();
		if(searchInfo.getSearchResult() != SearchInfo.ipx_GroupCountOnly)
			return;
		//the second part of the subset is not present, return result directly.
		if(searchInfo.getSelectedCount() ==  null || searchInfo.getSelectedCount().length() < 1)
			return;
		
		StringBuffer from = new StringBuffer("FROM ( ");
		from.append(searchInfo.getSelectCaluse()).append(searchInfo.getFromCaluse()).append(searchInfo.getWhereCaluse())
			.append(searchInfo.getGroupCaluse());
		from.append(" ) AS t ");
		searchInfo.setFromCaluse(from.toString());
		StringBuffer where = new StringBuffer(" WHERE ");
		for(String column : searchInfo.getOrderColumns()){
			where.append(column).append("count ").append(searchInfo.getSelectedCount()).append(" AND ");
		}
		where.delete(where.length() - 5, where.length());
		searchInfo.setWhereCaluse(where.toString());
		//remove the group caluse
		searchInfo.setGroupCaluse("");
		searchInfo.setSelectCaluse("SELECT * ");
	}
}
