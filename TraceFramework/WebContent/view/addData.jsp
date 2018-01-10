<%@ page language="java" contentType="text/html; charset=UTF-8"
    pageEncoding="UTF-8"%>
<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
<title>Insert title here</title>
</head>
<body>
add demo data
<form action="saveDemo.do" method="post">
	name:<input type="text" name="name" /><br>
	salary:<input type="text" name="salary" /> <br>
	age:<input type="text" name="age" /> <br>
	birthday:<input type="text" name="birthday" /> <br>
	colors:<input type="checkbox" name="colors" value="0"> red
	<input type="checkbox" name="colors" value="1"> blue
	<input type="checkbox" name="colors" value="2"> yellow
	<input type="submit" value="submit" />
</form>
</body>
</html>