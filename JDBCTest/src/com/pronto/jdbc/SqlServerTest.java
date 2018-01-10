package com.pronto.jdbc;

import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.sql.*;
import java.util.HashMap;
import java.util.Map;
import java.util.logging.ConsoleHandler;
import java.util.logging.FileHandler;
import java.util.logging.Formatter;
import java.util.logging.Handler;
import java.util.logging.Level;
import java.util.logging.Logger;
import java.util.logging.SimpleFormatter;
import java.util.logging.StreamHandler;

import javax.sql.rowset.CachedRowSet;

import com.sun.rowset.CachedRowSetImpl;

public class SqlServerTest {

	/**
	 * @param args
	 * @throws Exception
	 */
	public static void main(String[] args) throws Exception {
		getForXmlColumnType();
	}
	
	private static void testMaxStr() throws Exception{
		String url = "jdbc:sqlserver://localhost:1433;DatabaseName=SmallSchema";
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
				.prepareStatement("SELECT e.[EmployeeID],e.[LastName],e.[FirstName],e.[BirthDate],e.[NationalityCode],e.[Email],e.[Language] FROM [SmallSchema].[dbo].[Employee] e Full outer join [Employee] e1 on 1=1 full join Employee e2 on 1=1 full join Employee e9 on 1=1 where e9.EmployeeID > 15 for XML RAW, ELEMENTS XSINIL,BINARY BASE64");
//		ps.setString(1, "name姓名");
		//ps.setObject(1, 1);
		ResultSet rs = ps.executeQuery();
		StringBuffer result = new StringBuffer();
		while(rs.next())
		{
			result.append(rs.getString(1));
			//System.out.println("Row:");
		}
		String str1 = result.toString();
		System.out.println("end:" + str1);
		conn.close();
	}

	private static void testSelect() throws Exception {
		String url = "jdbc:sqlserver://localhost:1433;DatabaseName=SmallSchema";
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
				.prepareStatement("select * from testencode where tn = ?");
//		ps.setString(1, "name姓名");
		ps.setObject(1, 1);
		ResultSet rs = ps.executeQuery();
		while(rs.next())
		{
			String name = rs.getString("name");
			String desc = rs.getString("des");
			String remark = rs.getString("remark");
			System.out.println("Row:\r\n"+name+"\t"+desc+"\t"+remark+"\r\n");
		}
		conn.close();
	}
	
	private static void testBatch() throws Exception{
		String sql = "select * from pdb_category where 1=?  for xml auto";
//		String sql = "Begin transaction ts " +
//				"IF(  0 = ? ) "+
//" BEGIN   Select * From  PDB_Category  Where 1=1 "+  
// "END ELSE BEGIN "+ 
//  "insert into pdb_category(category, subcategory, name) values(2,23, 'category23')" +
//  "select * from pdb_category"+  
//" END " +
//"commit transaction ts";
		String url = "jdbc:sqlserver://localhost:1433;DatabaseName=dbx";
		String userName = "pdx";
		String password = "pdx";
		Class.forName("com.microsoft.sqlserver.jdbc.SQLServerDriver");

		Connection conn = DriverManager.getConnection(url, userName, password);
		PreparedStatement cs = conn.prepareStatement(sql);
		cs.setInt(1, 1);
//		cs.setInt(2, 20);
//		cs.execute();
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
	
	private static void testView() throws Exception{
		String url = "jdbc:sqlserver://192.168.1.103:1433;DatabaseName=dbx";
		String userName = "pdx";
		String password = "pdx";
		Class.forName("com.microsoft.sqlserver.jdbc.SQLServerDriver");

		Connection conn = DriverManager.getConnection(url, userName, password);
		FileOutputStream out = new FileOutputStream("e:\\ProntoDir\\view.txt");
		for(int i = 0; i < 500; i ++){
			long currentTime = System.currentTimeMillis();
//			PreparedStatement ps = conn.prepareStatement("select * from (select row_number() over( ORDER BY  TemplateName ASC,name DESC,CreatedDTG) as RecordNo, templatename, (select COUNT(*) from template_category innertemplate_category where innertemplate_category.templatename = template_category.templatename) templatename_count,name,(select COUNT(*) from template_category innertemplate_category where innertemplate_category.name = template_category.name) categorycont,createddtg, (select COUNT(*) from template_category innertemplate_category where innertemplate_category.createddtg = template_category.createddtg) createddtgcount from template_category group by templatename, createddtg, name ) as pagingtable  order by TemplateName ASC,name DESC,CreatedDTG");
			PreparedStatement ps = conn.prepareStatement("select t.category, (select COUNT(*) from totalcategory innert where innert.category=t.category) as categorycount, t.activetemplate, (select COUNT(*) from totalcategory innert where innert.activetemplate=t.activetemplate) from totalcategory t");
			ResultSet rs = ps.executeQuery();
			while(rs.next()){
//				String templateName = rs.getString("templatename");
				int cat = rs.getInt("category");
			}
			ps.close();
			long endTime = System.currentTimeMillis();
			out.write((String.valueOf(endTime-currentTime)+"\r\n").getBytes());
		}
		
		out.close();
	}
	
	private static void testTempory() throws Exception{
		String url = "jdbc:sqlserver://192.168.1.103:1433;DatabaseName=dbx";
		String userName = "app";
		String password = "app";
		Class.forName("com.microsoft.sqlserver.jdbc.SQLServerDriver");

		Connection conn = DriverManager.getConnection(url, userName, password);
		FileOutputStream out = new FileOutputStream("e:\\ProntoDir\\tempory.txt");
		
		
		for(int i = 0; i < 500; i ++){
			long currentTime = System.currentTimeMillis();
//			PreparedStatement ps1 = conn.prepareStatement(" WITH CtiTbl (TemplateName,Category,CategoryName,CreatedDTG,Active,CreatedBy,onembargo,OnembargoDTG) AS ( SELECT  T.TemplateName ,  C.CategoryID as Category ,  C.Name as CategoryName ,  T.CreatedDTG ,  T.Active ,  T.CreatedBy ,  'onembargo' = case  when (onembargodtg > getdate()) or ((onembargodtg = getdate()) and (active != 1)) then 1 when onembargodtg <= getdate() then 2 when onembargodtg is null then 4 end ,  T.OnembargoDTG  FROM   PDB_Template as  T   LEFT JOIN PDB_Category as C on (T.CategoryID = C.CategoryID and C.ParentID IS NULL)  ) SELECT * FROM (  SELECT  row_number() over( ORDER BY  TemplateName ASC,Category DESC,CreatedDTG) as RecordNo , TemplateName, (SELECT COUNT(*) FROM CtiTbl as innerCtiTem WHERE  innerCtiTem.TemplateName =  ctiTem.TemplateName) AS TemplateNameCount, Category, (SELECT COUNT(*) FROM CtiTbl as innerCtiTem WHERE  innerCtiTem.TemplateName =  ctiTem.TemplateName AND  innerCtiTem.Category =  ctiTem.Category) AS CategoryCount, CreatedDTG,  (SELECT COUNT(*) FROM CtiTbl as innerCtiTem WHERE  innerCtiTem.TemplateName =  ctiTem.TemplateName AND  innerCtiTem.Category =  ctiTem.Category AND  convert(nvarchar(8), innerCtiTem.CreatedDTG, 112) =  convert(nvarchar(8), ctiTem.CreatedDTG, 112)) AS CreatedDTGCount FROM CtiTbl AS ctiTem  WHERE  (Active & 3 = Active) and (CreatedBy like '%') and ( OnEmbargoDTG >= GetDate() or OnEmbargoDTG < GetDate() or OnEmbargoDTG IS NULL ) AND ( (SELECT COUNT(*) FROM CtiTbl as innerCtiTem WHERE  innerCtiTem.TemplateName =  ctiTem.TemplateName)>0 OR (SELECT COUNT(*) FROM CtiTbl as innerCtiTem WHERE  innerCtiTem.TemplateName =  ctiTem.TemplateName AND  innerCtiTem.Category =  ctiTem.Category)>0 OR (SELECT COUNT(*) FROM CtiTbl as innerCtiTem WHERE  innerCtiTem.TemplateName =  ctiTem.TemplateName AND  innerCtiTem.Category =  ctiTem.Category AND  convert(nvarchar(8), innerCtiTem.CreatedDTG, 112) =  convert(nvarchar(8), ctiTem.CreatedDTG, 112))>0 ) GROUP BY TemplateName,Category,CreatedDTG ) as PagingTable ORDER BY  TemplateName ASC,Category DESC,CreatedDTG");
			PreparedStatement ps1 = conn.prepareStatement("with ctitbl(category, activetemplate) as (select c.categoryid, (select count(*) from PDB_Template as T1 Where T1.CategoryID = c.CategoryID and T1.Active = 1) as ActiveTemplate from PDB_Category c left join PDB_Category sc on (c.CategoryID = sc.ParentID) ) select t.category, (select COUNT(*) from ctitbl innert where innert.category=t.category) as categorycount, t.activetemplate, (select COUNT(*) from ctitbl innert where innert.activetemplate=t.activetemplate) from ctitbl t");
			ResultSet rs = ps1.executeQuery();
			while(rs.next()){
//				String templateName = rs.getString("templatename");
				int cat = rs.getInt("category");
			}
			ps1.close();
			long endTime = System.currentTimeMillis();
			out.write((String.valueOf(endTime-currentTime)+"\r\n").getBytes());
		}
		
		out.close();
	}
	
	private static void getForXmlColumnType() throws Exception{
		Connection conn = getConnection();
		//PreparedStatement ps = conn.prepareStatement("SELECT * FROM Employee e1 inner join Employee e2 on e1.EmployeeID > 1 for xml auto, elements");
		PreparedStatement ps = conn.prepareStatement("select (SELECT * FROM Employee e1 inner join Employee e2 on e1.EmployeeID > 1 for xml auto, elements) as f1");
		ResultSet rs = ps.executeQuery();
		ResultSetMetaData md = rs.getMetaData();
		for(int i = 1; i <= md.getColumnCount(); i++){
			System.out.println("name:" + md.getColumnName(i)+", type:" + md.getColumnType(i));
		}
		Map temp = new HashMap();
		while(rs.next()){
			System.out.println(111);
			temp.put(1, rs.getAsciiStream(1));
//			temp.put(1, rs.getObject(1));
//		}
//		rs.close();
		
		     InputStream is = (InputStream) temp.get(1);
		     InputStreamReader isr = new InputStreamReader(is);
		     StringBuffer str= new StringBuffer();
		     int c;
		     while((c=isr.read())!=-1)
		     {
		      str.append((char)c);      
		     }
			System.out.println(str);
		}
	}
	
	private static void insertdata() throws Exception {
		String url = "jdbc:sqlserver://localhost:1433;DatabaseName=Application1";
		String userName = "app";
		String password = "app";
		Class.forName("com.microsoft.sqlserver.jdbc.SQLServerDriver");

		Connection conn = DriverManager.getConnection(url, userName, password);
		PreparedStatement ps = conn
				.prepareStatement("insert template(id, cat, subcat, templatename) values(?,?,?,?)");
		for (int i = 0; i < 1000; i++) {
			ps.setInt(1, i + 10);
			ps.setInt(2, i / 100);
			ps.setInt(3, i / 10);
			ps.setString(4, "aaa" + (i / 2));
			ps.executeUpdate();
		}
		ps.close();
		conn.close();
	}

	private static Connection getConnection() throws Exception {
		String url = "jdbc:sqlserver://localhost:1433;DatabaseName=smallschema";
		String userName = "pdx";
		String password = "pdx";
		Class.forName("com.microsoft.sqlserver.jdbc.SQLServerDriver");

		Connection conn = DriverManager.getConnection(url, userName, password);
		return conn;
	}

	public static void savePicture(String file) throws Exception {
		Connection connection = null;
		PreparedStatement pstmt = null;
		String sql = "Insert into address(id,location) values(?, ?)";

		try {
			FileInputStream input = new FileInputStream(file);
			System.out.println(input.available());
			connection = getConnection();
			pstmt = connection.prepareStatement(sql);
			pstmt.setInt(1, 900);
			pstmt.setBinaryStream(2, input, input.available());
			if (!pstmt.execute()) {
				System.out.println("����ͼƬ�ɹ��� ");
			} else {
				System.out.println("����ͼƬʧ�ܣ� ");
			}
		} catch (Exception e) {
			System.out.println(e.getMessage());
		} finally {
			if (pstmt != null)
				pstmt.close();
			if (connection != null && !connection.isClosed())
				connection.close();
		}
	}
}
