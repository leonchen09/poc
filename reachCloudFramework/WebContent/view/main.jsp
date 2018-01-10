<%@ page language="java" contentType="text/html; charset=UTF-8"
    pageEncoding="UTF-8"%>
<%@ taglib uri="http://java.sun.com/jsp/jstl/core" prefix="c" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=ISO-8859-1">
<title>Insert title here</title>
</head>
<body>
welcome.
<ul>
	<li> <a href="list1.do?id=1">list one</a> </li>
	<li> <a href="list2.do?name=h">list two</a> </li>
	<li> <a href="list3.do?pageNo=1%26pageSize=9">page list</a> </li>
	<li> <a href="list4.do?name=%25%26pageNo=1%26pageSize=6">分页，json测试</a> </li>
	<li> <a href="addData.do">Add Data</a> </li>
	<li> <a href="testException.do?name=biz">Exception test</a> </li>
	<li> <a href="testExceptionJson.do?name=biz">testExceptionJson</a> </li>
	<li> <a href="testJosnDirect.do?id=1">testJosnDirect</a> </li>
</ul>
<form action="list3.do" method="get">
<input type="text" name="pageNo" />
<input type="text" name="pageSize" />
<input type="submit" value="submit" />
</form>

<form action="list4.do" method="get">
<input type="text" name="pageNo" />
<input type="text" name="pageSize" />
<input type="text" name="name" value="%" />
<input type="submit" value="submit" />
</form>
</body>
</html>