package com.pronto.omni;

import java.io.ByteArrayOutputStream;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.sql.Connection;
import java.sql.Date;
import java.sql.DriverManager;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;

public class JunitTest {
	
	
	public void test () {
		System.out.println("Hello World!");
		System.out.println(Math.pow(2, 0));
		
		System.out.println(4 & 3);
	}
	
	public static void main(String [] args) throws ClassNotFoundException, SQLException, IOException {
		String sql = "SELECT REPLACE(NEWID(),'-','') as id, Result.* " +
			" \n into #temp1 " +
			" \n FROM ( SELECT C.Category  as Category," + 
			" \n C.CategoryName  as CategoryName, " +
			" \n C.CatClassifier  as CatClassifier, " +
			" \n C.CreatedDTG  as CreatedDTG, " +
			" \n C.CreatedBy  as CreatedBy, " +
			" \n C.CreatedOn  as CreatedOn, " +
			" \n C.UpdatedDTG  as UpdatedDTG, " +
			" \n C.UpdatedBy  as UpdatedBy, " +
			" \n C.UpdatedOn  as UpdatedOn" +
			" \n FROM pdx_vCategory as C" +
			" \n ) as Result " +
			
			" \n SELECT " +
			" \n Category.Category" + 
			" \n , Category.CategoryName  , Category.CatClassifier  , Category.CreatedDTG  , Category.CreatedBy  , Category.CreatedOn  , Category.UpdatedDTG  , Category.UpdatedBy  , Category.UpdatedOn" + 
			" \n FROM" + 
			" \n #temp1 as Category" + 
			" \n  FOR XML AUTO, root('SearchResult')" + 
			
			" \n DROP TABLE #temp1";
		
			Class.forName("com.microsoft.sqlserver.jdbc.SQLServerDriver");
 			String url = "jdbc:sqlserver://localhost;DatabaseName=app2;user=pdx;password=pdx";
			Connection conn = DriverManager.getConnection( url, "pdx", "pdx");
			
//			String sql_i = "insert into pdb_category(name, createdby,createdon,category, subcategory) values(?,?,?,?,?)";
//			String sql2 = "update child1 set maxname=? where cid=2";
//			PreparedStatement ps = conn.prepareStatement(sql2);
//			ps.setString(1, test2());
//			ps.executeUpdate();
//			for(int i = 10; i < 100; i ++){
//				ps.setString(1, "catname"+i);
//				ps.setString(2, "createdby" + i);
//				ps.setString(3, "createdon" + i);
//				ps.setInt(4, 100 + i);
//				ps.setInt(5, 0);
//				ps.executeUpdate();
//			}
//			conn.commit();
			
			Statement stmt = conn.createStatement();
			ResultSet rs = stmt.executeQuery("select maxname from child1 where cid=2");
			
			while(rs.next()) {
				System.out.println(rs.getString(1));
			}
			
			rs.close();
			stmt.close();
			conn.close();
	}
	
	
	public static String test2() throws IOException{
		ByteArrayOutputStream out = new ByteArrayOutputStream();
		FileInputStream in = new FileInputStream("e:\\1.txt");
		byte[] buffer = new byte[1024];
		while(in.read(buffer) !=  -1){
			out.write(buffer);
		}
		String content = out.toString();
		return content;
	}
}
