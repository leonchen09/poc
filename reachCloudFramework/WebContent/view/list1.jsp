<%@ page language="java" contentType="text/html; charset=UTF-8"
    pageEncoding="UTF-8"%>
<%@ taglib uri="http://java.sun.com/jsp/jstl/core" prefix="c" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
<title>Insert title here</title>
</head>
<body>
detial page
<form action="saveDemo.do">
<input type="hidden" name="id" value="${demo.id}" />
name:<input type="text" name="name" value="${demo.name}" /><p>
age:<input type="text" name="age" value="${demo.age}" /><p>
salary:<input type="text" name="salary" value="${demo.salary}"/>
birthday:<input type="text" name="birthday" value="${demo.birthday}" />
<input type="submit" value="submit" />
</form>
</body>
</html>