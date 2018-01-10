<%@ page pageEncoding="utf-8"%>  
<!DOCTYPE html>  
<html>  
<head>  
<meta charset="utf-8">  
<title>导入文件测试</title>  
</head>  
<body>  
<form action="../stationInfo/fileImport" method="post" enctype="multipart/form-data">  
<input type="file" name="file" />
<input type="hidden" name="companyId" value="1">
<input type="submit" value="Submit" /></form>  
</body>  
</html>  