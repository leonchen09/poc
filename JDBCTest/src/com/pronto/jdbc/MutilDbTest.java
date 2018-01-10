package com.pronto.jdbc;

import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.ResultSetMetaData;

import javax.sql.rowset.CachedRowSet;

import com.sun.rowset.CachedRowSetImpl;

public class MutilDbTest {

	/**
	 * @param args
	 * @throws Exception 
	 */
	public static void main(String[] args) throws Exception {
		init();
		testBatch();
		testMetaData();
	}

	private static void init(){
		try {
			Class.forName("oracle.jdbc.driver.OracleDriver").newInstance();
			Class.forName("com.microsoft.sqlserver.jdbc.SQLServerDriver").newInstance();
		} catch (InstantiationException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (IllegalAccessException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (ClassNotFoundException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}
	private static void testBatch() throws Exception{
		String sql = "select * from pdb_category where 1=?  for xml auto";

		String url = "jdbc:sqlserver://localhost:1433;DatabaseName=dbx";
		String userName = "pdx";
		String password = "pdx";
//		Class.forName("com.microsoft.sqlserver.jdbc.SQLServerDriver");

		Connection conn = DriverManager.getConnection(url, userName, password);
		PreparedStatement cs = conn.prepareStatement(sql);
		cs.setInt(1, 1);
		ResultSet rs = cs.executeQuery();
		CachedRowSet data = new CachedRowSetImpl();
		data.populate(rs);
		rs.close();
		cs.close();
		conn.close();
		while(data.next()){
			System.out.println(data.getString(1));
		}
	}
	
	private static void testMetaData() throws Exception{
		String url = "jdbc:oracle:thin:@localhost:1521:orcl";
		String userName = "app";
		String password = "app";

		oracle.jdbc.driver.OracleLog.setTrace(true);

//		Class.forName("oracle.jdbc.driver.OracleDriver");

		String sql = "select n16 from orclxml";

		Connection conn = DriverManager.getConnection(url, userName, password);
		PreparedStatement cs = conn.prepareStatement(sql);
		// cs.execute();
		ResultSet rs = cs.executeQuery();
		while(rs.next()){
			System.out.println("oracle:" + rs.getString(1));
		}
//		ResultSetMetaData rsmd = rs.getMetaData();
//		for(int i = 1; i <= rsmd.getColumnCount(); i ++){
//			System.out.println(rsmd.getColumnLabel(i) + ":" + rsmd.getColumnType(i));
//		}
	}
}
