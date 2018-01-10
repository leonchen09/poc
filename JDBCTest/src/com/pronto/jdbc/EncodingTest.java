package com.pronto.jdbc;

import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.util.logging.Level;
import java.util.logging.Logger;
import java.util.logging.SimpleFormatter;
import java.util.logging.StreamHandler;

public class EncodingTest {

	/**
	 * @param args
	 * @throws Exception 
	 */
	public static void main(String[] args) throws Exception {
//		testInsert();
//		testSelectXml();
		testSelect2();
	}

	public static void testSelect() throws Exception {
		String url = "jdbc:sqlserver://localhost:1433;DatabaseName=app";
		String userName = "pdx";
		String password = "pdx";

		Class.forName("com.microsoft.sqlserver.jdbc.SQLServerDriver");

		Logger logger = Logger.getLogger("com.microsoft.sqlserver.jdbc");
		// Handler fh = new FileHandler("e:\\jdbc.log");
		// Handler fh = new ConsoleHandler(System.out);
		StreamHandler fh = new StreamHandler();

		fh.setFormatter(new SimpleFormatter());
		logger.addHandler(fh);
		logger.setLevel(Level.ALL);
		Connection conn = DriverManager.getConnection(url, userName, password);
		PreparedStatement ps = conn
				.prepareStatement("select * from testencode where name='name濮撳悕'");
		ResultSet rs = ps.executeQuery();
		while(rs.next())
		{
			String name = rs.getString("name");
			String desc = rs.getString("des");
			String remark = rs.getString("remark");
			System.out.println("Mssql Row:\n"+name+"\t"+desc+"\t"+remark+"\r\n");
		}
		conn.close();
	}
	
	public static void testSelect2() throws Exception {
		String url = "jdbc:sqlserver://localhost:1433;DatabaseName=app2";
		String userName = "pdx";
		String password = "pdx";

		Class.forName("com.microsoft.sqlserver.jdbc.SQLServerDriver");

		Logger logger = Logger.getLogger("com.microsoft.sqlserver.jdbc");
		// Handler fh = new FileHandler("e:\\jdbc.log");
		// Handler fh = new ConsoleHandler(System.out);
		StreamHandler fh = new StreamHandler();

		fh.setFormatter(new SimpleFormatter());
		logger.addHandler(fh);
		logger.setLevel(Level.ALL);
		Connection conn = DriverManager.getConnection(url, userName, password);
		PreparedStatement ps = conn
				.prepareStatement("select * from TestEncode where name like '%姓名%'");
		ResultSet rs = ps.executeQuery();
		while(rs.next())
		{
			String name = rs.getString("NAME");
			String desc = rs.getString("DES");
			String remark = rs.getString("REMARK");
			System.out.println("Mssql2 Row:\n"+name+"\t"+desc+"\t"+remark+"\r\n");
		}
		conn.close();
	}
	
	public static void testSelectXml() throws Exception {
		String url = "jdbc:sqlserver://localhost:1433;DatabaseName=app";
		String userName = "pdx";
		String password = "pdx";

		Class.forName("com.microsoft.sqlserver.jdbc.SQLServerDriver");

		Logger logger = Logger.getLogger("com.microsoft.sqlserver.jdbc");
		// Handler fh = new FileHandler("e:\\jdbc.log");
		// Handler fh = new ConsoleHandler(System.out);
		StreamHandler fh = new StreamHandler();

		fh.setFormatter(new SimpleFormatter());
		logger.addHandler(fh);
		logger.setLevel(Level.ALL);
		Connection conn = DriverManager.getConnection(url, userName, password);
		PreparedStatement ps = conn
				.prepareStatement("select * from testencode where name like '%姓名%' for xml auto,elements");
		ResultSet rs = ps.executeQuery();
		while(rs.next())
		{
			String name = rs.getString(1);
			System.out.println("Mssql XML:\n"+name+"\r\n");
		}
		conn.close();
	}
	
	public static void testSelectXml2() throws Exception {
		String url = "jdbc:sqlserver://localhost:1433;DatabaseName=app2";
		String userName = "pdx";
		String password = "pdx";

		Class.forName("com.microsoft.sqlserver.jdbc.SQLServerDriver");

		Logger logger = Logger.getLogger("com.microsoft.sqlserver.jdbc");
		// Handler fh = new FileHandler("e:\\jdbc.log");
		// Handler fh = new ConsoleHandler(System.out);
		StreamHandler fh = new StreamHandler();

		fh.setFormatter(new SimpleFormatter());
		logger.addHandler(fh);
		logger.setLevel(Level.ALL);
		Connection conn = DriverManager.getConnection(url, userName, password);
		PreparedStatement ps = conn
				.prepareStatement("select * from testencode where name like '%姓名%' for xml auto,elements");
		ResultSet rs = ps.executeQuery();
		while(rs.next())
		{
			String name = rs.getString(1);
			System.out.println("Mssql2 XML:\n"+name+"\r\n");
		}
		conn.close();
	}
	
	public static void testInsert() throws Exception {
		String url = "jdbc:sqlserver://localhost:1433;DatabaseName=app2";
		String userName = "pdx";
		String password = "pdx";

		Class.forName("com.microsoft.sqlserver.jdbc.SQLServerDriver");

		Logger logger = Logger.getLogger("com.microsoft.sqlserver.jdbc");
		// Handler fh = new FileHandler("e:\\jdbc.log");
		// Handler fh = new ConsoleHandler(System.out);
		StreamHandler fh = new StreamHandler();

		fh.setFormatter(new SimpleFormatter());
		logger.addHandler(fh);
		logger.setLevel(Level.ALL);
		Connection conn = DriverManager.getConnection(url, userName, password);
		PreparedStatement ps = conn
				.prepareStatement("insert into TestEncode(ID, NAME, DES, REMARK) values(?,?,?,?)");
		ps.setInt(1, 2);
		ps.setString(2, "nm2姓名2");
		ps.setString(3, "des2姓名2");
		ps.setString(4, "remark2姓名2");
		ps.execute();
		conn.commit();
		conn.close();
	}

}
