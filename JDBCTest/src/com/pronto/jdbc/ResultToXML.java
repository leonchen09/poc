package com.pronto.jdbc;

import java.sql.ResultSet;
import java.sql.ResultSetMetaData;
import java.sql.SQLException;

public class ResultToXML {
	
	public static String convert(ResultSet rs, String selectedColumns) throws SQLException{
		StringBuffer result = new StringBuffer();
		ResultSetMetaData rsmd = rs.getMetaData();
		while(rs.next()){
			result.append("new row:");
			result.append(rsmd.getTableName(1)+"$" + rsmd.getColumnName(1) + "$" + rsmd.getColumnLabel(1) +"$"+ rsmd.getCatalogName(1) + rs.getObject(1)).append(", ");
			result.append(rsmd.getTableName(2)+"$" + rsmd.getColumnName(2) + "$" + rsmd.getColumnLabel(2) +"$"+ rsmd.getCatalogName(2) + rs.getObject(2)).append(", ");
			result.append(rs.getObject(3)).append(", ");
			result.append(rs.getObject(4)).append(", ");
			result.append(rs.getObject(5)).append(", ");
			result.append(rs.getObject(6)).append(", ");
			result.append(rs.getObject(7)).append(", ");
			result.append(rsmd.getTableName(8)+"$" + rsmd.getColumnName(8) + "$" + rsmd.getColumnLabel(8) +"$"+ rsmd.getCatalogName(8) + rs.getObject(8)).append(", ");
			result.append(rs.getObject(9)).append(", ");
			result.append("\r\n");
		}
		return result.toString();
	}
}
