package com.pronto.omni;

import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;

import oracle.sql.ARRAY;
import oracle.sql.ArrayDescriptor;
import oracle.sql.SQLName;

public class JdbcArrayParam {

	private static String sqlconnectURL = "jdbc:sqlserver://localhost:1433;DatabaseName=dbx;user=pdx;password=pdx";//"jdbc:sqlserver://localhost:1433;DatabaseName=mobileDemo;user=sa;password=123456";//"jdbc:sqlserver://localhost:1433;DatabaseName=SmallSchema;";//SmallSchema;pdx
	
	private static String oracleConnectionURL = "jdbc:oracle:thin:@192.168.0.254:1521:ORCL";
	
	public static Connection getConnection()
	{
		Connection dbConnection = null;
		try
		{
			Class.forName("com.microsoft.sqlserver.jdbc.SQLServerDriver");
			dbConnection = DriverManager.getConnection(sqlconnectURL);
			
//			Class.forName( "oracle.jdbc.driver.OracleDriver" );
//			dbConnection = DriverManager.getConnection(oracleConnectionURL, "app","app");
		}
		catch (Exception e)
		{
			e.printStackTrace();
		}
		return dbConnection;
	}
	
	public static void close(Connection conn, ResultSet rs, PreparedStatement ps)
	{
		
		try {
			if(rs!=null)
				rs.close();
			if(ps!=null)
				ps.close();
			if(conn!=null)
				conn.close();
		} catch (SQLException e) {
			e.printStackTrace();
		}
	}
	
	public static void test6()
	{
		String sql = "select * from address " +
				"where detail in ( ? ) ";
		ResultSet rs = null;
		try
		{
			Connection conn = getConnection();
			PreparedStatement ps = conn.prepareStatement(sql);
			SQLName sqlName = SQLName.
//			ArrayDescriptor descriptor = ArrayDescriptor.createDescriptor("CHAR_ARRAY", conn); 
//			ARRAY array = new ARRAY(descriptor, conn , new String[]{"a","b"});
//			ps.setArray(1, array);
			rs = ps.executeQuery();
			while(rs.next())
			{
				System.out.println(rs.getString(1));
			}
			
			close(conn, rs, ps);
		}
		catch (SQLException e)
		{
			e.printStackTrace();
		}
	
	}
	
	public static void test7()
	{
		String sql = "Select * from [dbx].[dbo].[PDB_Category] " +
				"where [Name] in ( ? ) ";
		ResultSet rs = null;
		try
		{
			Connection conn = getConnection();
			PreparedStatement ps = conn.prepareStatement(sql);
			ps.setArray(1, conn.createArrayOf("String", new String[]{"a","b"}));
			rs = ps.executeQuery();
			while(rs.next())
			{
				System.out.println(rs.getString(1));
			}
			
			close(conn, rs, ps);
		}
		catch (SQLException e)
		{
			e.printStackTrace();
		}

	}
	
	public static void main(String[] args) {
		JdbcArrayParam jdbcTest = new JdbcArrayParam();
		jdbcTest.test7();

	}

}
