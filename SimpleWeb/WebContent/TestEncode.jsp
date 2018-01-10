<%@ page language="java" contentType="text/html; charset=GB2312"
    pageEncoding="GB2312"
    import="java.sql.*" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=GB2312">
<title>Encoding Test</title>
</head>
<body>
<%
String inputValue = request.getParameter("ipt");
String url = "jdbc:mysql://localhost:3306/app";
String userName = "root";
String password = "mysql";

Class.forName("com.mysql.jdbc.Driver");
Connection conn = DriverManager.getConnection(url, userName, password);
String sql = "select * from testencode ";
if(inputValue != null && inputValue.length() > 0)
{
	inputValue = new String(inputValue.getBytes("ISO8859-1"));
	sql+= " where name='"+inputValue + "'";
}
PreparedStatement ps = conn.prepareStatement(sql);
System.out.println("SQL:" + sql);
ResultSet rs = ps.executeQuery();
while(rs.next())
{
	String name = rs.getString("name");
	String desc = rs.getString("des");
	String remark = rs.getString("remark");
	System.out.println("MYsql Row:\n"+name+"\t"+desc+"\t"+remark+"\r\n");
	out.println("MYsql Row:"+name+"&nbsp;&nbsp;"+desc+"&nbsp;&nbsp;"+remark+"<br>");
}
conn.close();
%>
<form action="" method="post">
 <input type="text" name="ipt" />
 <input type="submit" name="send" text="send"/>
</form>
</body>
</html>