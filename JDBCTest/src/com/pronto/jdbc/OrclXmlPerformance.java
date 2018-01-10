package com.pronto.jdbc;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.Reader;
import java.io.StringWriter;
import java.io.Writer;
import java.sql.Clob;
import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;

import com.stepfunction.prontodoc.sx.dao.o10g.ForXml;

public class OrclXmlPerformance {

	/**
	 * @param args
	 * @throws SQLException 
	 * @throws IOException 
	 */
	public static void main(String[] args) throws SQLException, IOException {
//		orclGenXml();
		ourGenXml();
		orclGenXml();
	}

	public static void orclGenXml() throws SQLException, IOException{
		Connection con = getConnection();
		String sql = "select xmlagg(xmlelement(ar,xmlelement(id, ar.id), xmlelement(name, ar.name),xmlelement(birthday, ar.birthday),";
		sql+= "(select xmlagg(xmlelement(ap, xmlelement(id, ap.id), xmlelement(name, ap.name), xmlelement(descp, ap.descp),";
		sql+= "(select xmlagg(xmlelement(ad, xmlelement(id, ad.id), xmlelement(detail, ad.detail))) from address ad where ad.id=ap.addressid ";
		sql+= "))) from application ap where ap.applierid = ar.id))).getclobval()";
		sql+= "from applier ar where ar.id<2001";
		String result = null;
		
		long start = System.currentTimeMillis();
		PreparedStatement ps = con.prepareStatement(sql);
		ResultSet rs = ps.executeQuery();
		while(rs.next()){
			Clob clob = rs.getClob(1);
			BufferedReader reader = new BufferedReader(new InputStreamReader(clob.getAsciiStream()));
			Writer writer = new StringWriter();
			String line = null;
			while((line = reader.readLine()) != null){
				writer.write(line);
			}
			result = writer.toString();
		}
		long end = System.currentTimeMillis();
		
		System.out.println("total time for oracle generate xml:"+(end-start));
//		System.out.println(result);
		con.close();
	}
	
	public static void ourGenXml() throws SQLException{
		Connection con = getConnection();
		String sql = "select ar.id ar_id, ar.name ar_name, ar.birthday, ap.id ap_id, ap.name ap_name, ap.descp, ad.id ad_id, ad.detail from applier ar left join application ap on ar.id = ap.applierid left join address ad on ap.addressid = ad.id ";
		sql = sql + " where ar.id<2001";
		String selectedColumns = "[ar],ar_id,ar_name,birthday,[ap],ap_id,ap_name,descp,[ad],ad_id,detail";
		
		long start = System.currentTimeMillis();
		PreparedStatement ps = con.prepareStatement(sql);
		ResultSet rs = ps.executeQuery();
		String result = ForXml.forXmlStr(rs, selectedColumns, true, "PdwData");
		long end = System.currentTimeMillis();
		
		System.out.println("total time for ourself generate xml:"+(end-start));
//		System.out.println(result);
		con.close();
	}
	
	private static Connection getConnection() throws SQLException{
		String url = "jdbc:oracle:thin:@localhost:1521:orcl";
//		String url = "jdbc:oracle:thin:@192.168.1.101:1521:orcl";
		String userName = "app";
		String password = "app";
	
		try {
			Class.forName("oracle.jdbc.driver.OracleDriver");
		} catch (ClassNotFoundException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		return DriverManager.getConnection(url, userName, password);
	}
}
