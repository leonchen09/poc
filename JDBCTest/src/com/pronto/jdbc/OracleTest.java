package com.pronto.jdbc;

import java.io.BufferedReader;
import java.io.FileOutputStream;
import java.io.FileWriter;
import java.io.InputStream;
import java.io.Reader;
import java.sql.CallableStatement;
import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.ResultSetMetaData;
import java.sql.Statement;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.logging.Level;
import java.util.logging.Logger;
import java.util.logging.SimpleFormatter;
import java.util.logging.StreamHandler;

import oracle.sql.BLOB;
import oracle.sql.CLOB;

public class OracleTest {
	/**
	 * @param args
	 * @throws Exception
	 */
	public static void main(String[] args) throws Exception {
		//testForXml();
//		testMetaData();
//		testBatch();
		readlobToMap();
	}

	private static void readlobToMap() throws Exception{
		String url = "jdbc:oracle:thin:@192.168.199.254:1521:orcl";
		String userName = "app";
		String password = "app";

		oracle.jdbc.driver.OracleLog.setTrace(true);

		Class.forName("oracle.jdbc.driver.OracleDriver");

		Connection con = DriverManager.getConnection(url, userName, password);
		PreparedStatement ps = con.prepareStatement("select Textphoto from photo where employeeid=1");
		//PreparedStatement ps = con.prepareStatement("SELECT xmlagg(XMLElement(\"row\", XMLelement(\"P_201416090307563315_Select\",P10.SINCEDATE), XMLelement(\"P_201405090952045333_Select\",P10.PERMONTH) )).getClobVal() From EMPLOYEE P1 INNER JOIN SALARYHISTORY P10 ON P1.EMPLOYEEID = P10.EMPLOYEEID Where P1.EMPLOYEEID = 1");
		ResultSet rs = ps.executeQuery();
		ResultSetMetaData md = rs.getMetaData();
		for(int i = 1; i <= md.getColumnCount(); i++){
			System.out.println("name:" + md.getColumnName(i)+", type:" + md.getColumnType(i));
		}
		
		Map temp = new HashMap();
		while(rs.next()){
			temp.put(1, rs.getObject(1));
		}
		//BLOB lob = (BLOB)temp.get(1);
		CLOB lob = (CLOB) temp.get(1);
		InputStream is = lob.getAsciiStream();//.binaryStreamValue();
		int i = 0;
		int len = (int) lob.length();
		byte[] data = new byte[1204];
		FileOutputStream writer = new FileOutputStream("e:\\3.txt");
		while (-1 != (i = is.read(data, 0, data.length))) {
			writer.write(data);
		}
		writer.close();
		//System.out.println(data.toString());
	}
	
	private static void testClob() throws Exception{
		String url = "jdbc:oracle:thin:@localhost:1521:orcl";
		String userName = "app";
		String password = "app";

		oracle.jdbc.driver.OracleLog.setTrace(true);

		Class.forName("oracle.jdbc.driver.OracleDriver");

		Connection con = DriverManager.getConnection(url, userName, password);
		PreparedStatement ps = con.prepareStatement("select clobtest('aaaaaaa') from dual");
		ResultSet rs = ps.executeQuery();
		Reader reader = null;
		while(rs.next()){
			CLOB result = (CLOB) rs.getClob(1);
			reader = result.getCharacterStream();
		}
		FileWriter writer = new FileWriter("e:\\prontodir\\oracle_xml\\oraclexml.xml2");
		BufferedReader br = new BufferedReader(reader);
		String data = null;
		while((data = br.readLine()) != null ){
			writer.write(data);
		}
		writer.flush();
		writer.close();
		ps.close();
	}
	
	private static void testForXml() throws Exception {
		String url = "jdbc:oracle:thin:@localhost:1521:orcl";
		String userName = "app";
		String password = "app";

		oracle.jdbc.driver.OracleLog.setTrace(true);

		Class.forName("oracle.jdbc.driver.OracleDriver");

		String sql = "select forxml(cursor(select id, n3,n4,n16 from orclxml),'[t1], id, n3,n4,n16', ?,?) from dual";

		Connection conn = DriverManager.getConnection(url, userName, password);
		PreparedStatement cs = conn.prepareStatement(sql);
		cs.setInt(1, 1);
		cs.setString(2, "pdwd");
		// cs.execute();
		ResultSet rs = cs.executeQuery();
		Reader reader = null;
		while(rs.next()){
			CLOB result = (CLOB) rs.getClob(1);
			reader = result.getCharacterStream();
		}
		FileWriter writer = new FileWriter("e:\\prontodir\\oracle_xml\\oraclexml.xml");
		BufferedReader br = new BufferedReader(reader);
		String data = null;
		while((data = br.readLine()) != null ){
			writer.write(data);
		}
		writer.flush();
		writer.close();
	}

	private static void testMetaData() throws Exception{
		String url = "jdbc:oracle:thin:@localhost:1521:orcl";
		String userName = "app";
		String password = "app";

		oracle.jdbc.driver.OracleLog.setTrace(true);

		Class.forName("oracle.jdbc.driver.OracleDriver");

		String sql = "select id from orclxml where id";

		Connection conn = DriverManager.getConnection(url, userName, password);
		PreparedStatement cs = conn.prepareStatement(sql);
		// cs.execute();
		ResultSet rs = cs.executeQuery();
		ResultSetMetaData rsmd = rs.getMetaData();
		for(int i = 1; i <= rsmd.getColumnCount(); i ++){
			System.out.println(rsmd.getColumnLabel(i) + ":" + rsmd.getColumnType(i));
		}
	}
	
	private static void testBatch() throws Exception {
		String url = "jdbc:oracle:thin:@localhost:1521:orcl";
		String userName = "app";
		String password = "app";

		oracle.jdbc.driver.OracleLog.setTrace(true);

		Class.forName("oracle.jdbc.driver.OracleDriver");
		// String sql =
		// "select to_xml_j(cursor(select * from application ap inner join applier ar on ap.id=ar.applicaitonid where ap.id=?),'') from dual;";
//		String sql = "begin update application set applierid=9 where id=?; update application set applierid=9 where id=2; end;";
//		String sql = "begin declare cursor m_cur(c_id number) is select * from application where 1=c_id; open m_cur(?);select to_xml_j(c_ur) from dual;end;";
		String sql = "declare cursor m_cur() is select * from application where id=1;begin update application set applierid=888 where id=?;  open m_cur(); end;";
		/*
		 * String sql = "IF(  0 = ? ) "+
		 * "    Select * From  customer  Where 1=1; "+ " ELSE  "+ //
		 * "insert into customer(id, name, sex, address) values(99,'name99', 1,'address99');"
		 * + "select * from customer;"+ " END IF " ;
		 */
		Connection conn = DriverManager.getConnection(url, userName, password);
		PreparedStatement cs = conn.prepareStatement(sql);
		cs.setInt(1, 1);
//		cs.execute();
		ResultSet rs = cs.executeQuery();
		while (rs.next()) {
			System.out.println(rs.getObject(1));
		}
	}

	private static void test() throws Exception {
		String url = "jdbc:oracle:thin:@localhost:1521:orcl";
		String userName = "app";
		String password = "app";

		Class.forName("oracle.jdbc.driver.OracleDriver");

		// Logger logger = Logger.getLogger("com.microsoft.sqlserver.jdbc");
		// // Handler fh = new FileHandler("e:\\jdbc.log");
		// // Handler fh = new ConsoleHandler(System.out);
		// StreamHandler fh = new StreamHandler();
		// // fh.
		// fh.setFormatter(new SimpleFormatter());
		// logger.addHandler(fh);
		// logger.setLevel(Level.ALL);
		Connection conn = DriverManager.getConnection(url, userName, password);
		PreparedStatement ps = conn
				.prepareStatement("select * from application where code=?");
		for (int i = 0; i < 2; i++) {
			ps.setString(1, "00'; char(39);");
			ps.executeQuery();
		}
		ps.close();
		Statement st = conn.createStatement();
		ResultSet rs = st
				.executeQuery("select * from application --where code='111\' or 1=1");
		while (rs.next()) {
			System.out.println("code:" + rs.getString("code"));
		}
		Thread.sleep(100 * 10000);
		conn.close();
	}

}
