<%@ page language="java" contentType="text/html; charset=ISO-8859-1"
    pageEncoding="ISO-8859-1"%>
<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=ISO-8859-1">
<title>Insert title here</title>
</head>
<% 
String sessionId = (String)session.getId();
String user = (String)session.getAttribute("user");
String parm = request.getParameter("user");
session.setAttribute("user",parm);
System.out.println("sessionid:"+sessionId+",time:"+new java.util.Date());
%>
<body>
Ok!<br>
SessionID:<%=sessionId %><br>
User:<%=user %>
<form action="PRONTOMOBILE" method="get">
<input type="text" name="user" value="123">
<input type="submit" value="submit">
</form>
</body>
</html>