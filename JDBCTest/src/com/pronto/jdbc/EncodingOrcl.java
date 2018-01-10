package com.pronto.jdbc;

import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.PreparedStatement;
import java.sql.ResultSet;

public class EncodingOrcl {
	/**
	 * @param args
	 * @throws Exception 
	 */
	public static void main(String[] args) throws Exception {
		//testInsert();
		testSelect();
	}

	public static void testSelect() throws Exception {
		String url = "jdbc:oracle:thin:@localhost:1521:orcl";
		String userName = "app";
		String password = "app";

		Class.forName("oracle.jdbc.driver.OracleDriver");

		Connection conn = DriverManager.getConnection(url, userName, password);
		PreparedStatement ps = conn
				.prepareStatement("select * from testencode ");
		ResultSet rs = ps.executeQuery();
		while(rs.next())
		{
			String name = rs.getString("name");
			String desc = rs.getString("des");
			String remark = rs.getString("remark");
			System.out.println("Oracle Row:\n"+name+"\t"+desc+"\t"+remark+"\r\n");
		}
		conn.close();
	}
	
	public static void testInsert() throws Exception {
		String url = "jdbc:oracle:thin:@localhost:1521:orcl";
		String userName = "app";
		String password = "app";

		Class.forName("oracle.jdbc.driver.OracleDriver");
		
		Connection conn = DriverManager.getConnection(url, userName, password);
		PreparedStatement ps = conn
				.prepareStatement("insert into testencode(id, name, des, remark) values(?,?,?,?)");
		ps.setInt(1, 2);
		ps.setString(2, "nm2ÐÕÃû2");
		ps.setString(3, "des2ÃèÊö2");
		ps.setString(4, "remark2±¸×¢2");
		ps.execute();
		conn.commit();
		conn.close();
	}
}
