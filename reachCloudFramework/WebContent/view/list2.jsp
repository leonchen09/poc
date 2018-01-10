<%@ page language="java" contentType="text/html; charset=UTF-8"
    pageEncoding="UTF-8"%>
<%@ taglib uri="http://java.sun.com/jsp/jstl/core" prefix="c" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
<script type="text/javascript" src="js/jquery-2.0.0.min.js"></script>
<script type="text/javascript" src="js/public.js"></script>
<title>分页，json测试</title>
<script type="text/javascript">
function listJson()
{
	//var formdata = $('#form1').serialize();// 你的formid
	var formdata = {'pageNo':2, 'pageSize':19, 'name':'n','birth':'1900-1-1'};
	$.ajax({
        cache: true,
        contentType:'application/json; charset=UTF-8',
        type: "POST",
        url:"listJson.do",
		data : formdata,
		dataType : "json",
        async: false,
        error: function(request) {
            alert("Connection error");
        },
        success: function(data) {
            alert(data.toJSONString());
        }
    });	
}
function saveJsonDemo()
{
	//var formdata = $('#form1').serializeObject();// 你的formid
	var color = [1,2];
	var formdata = {'age':2, 'name':'n','salary':9.008,colors:JSON.stringify(color)};
	$.ajax({
        cache: true,
        contentType:'application/json; charset=UTF-8',
        type: "POST",
        url:"saveJsonDemo.do",
		data: JSON.stringify(formdata),
		dataType: "json",
        async: false,
        error: function(request) {
            alert("Connection error");
        },
        success: function(data) {
            alert(data.toJSONString());
        }
    });	
}
function detailJson()
{
	var formdata = $('#form1').serialize();
	$.ajax({
        cache: true,
        type: "POST",
        url:"detailJson.do",
		data : JSON.stringify(formdata),
		dataType : "json",
        async: false,
        error: function(request) {
            alert("Connection error");
        },
        success: function(data) {
            alert(data.toJSONString());
        }
    });	
}
function getXml()
{
	$.ajax({
        cache: true,
        type: "POST",
        url:"detailJson.do",
		data : {'id':2},
		dataType : "XML",
        async: false,
        error: function(request) {
            alert("Connection error");
        },
        success: function(data) {
            alert(data.toJSONString());
        }
    });	
}
function getXmllist()
{
	var formdata = {'pageNo':1, 'pageSize':19, 'name':'n','birth':'1900-1-1'};
	$.ajax({
        cache: true,
        type: "POST",
        url:"listJson.do",
		data : {'data':JSON.stringify(formdata)},
		dataType : "XML",
        async: false,
        error: function(request) {
            alert("Connection error");
        },
        success: function(data) {
            alert(data.toJSONString());
        }
    });	
}
//读写分离测试
function readOnlyList()
{
	var formdata = {'pageNo':1, 'pageSize':19, 'name':'%'};
	$.ajax({
        cache: true,
        type: "POST",
        url:"readlist.do",
		data : {'data':JSON.stringify(formdata)},
		dataType : "XML",
        async: false,
        error: function(request) {
            alert("Connection error");
        },
        success: function(data) {
            alert(data.toJSONString());
        }
    });	
}
//测试controller直接返回json
function testJosnDirect()
{
	var formdata = {'id':2};
	$.ajax({
        cache: true,
        contentType:'application/json; charset=UTF-8',
        type: "POST",
        url:"testJosnDirect.do",
		data : formdata,
		dataType : "json",
        async: false,
        error: function(request) {
            alert("Connection error");
        },
        success: function(data) {
            alert(data.toJSONString());
        }
    });
}
function testJosnString()
{
	var formdata = {'id':2};
	$.ajax({
        cache: true,
        contentType:'application/json; charset=UTF-8',
        /* contentType:'application/x-www-form-urlencoded;charset=UTF-8', */
        type: "POST",
        url:"testmvccache.do",
		data : JSON.stringify(formdata),
		dataType : "json",
        async: false,
        error: function(request) {
            alert("Connection error");
        },
        success: function(data) {
            alert(data.toJSONString());
        }
    });
}
</script>
</head>
<body>
list two page;
<form action="list4.do" method="post" id="form1">
<input type="text" name="name" value="" />
<table>
<tr>
 <td>ID</td><td>Name</td><td>Age</td><td>Salary</td>
</tr>
<c:forEach var="p" items="${data.result}">
<tr>
 <td>${p.id}</td>
 <td>${p.name}</td>
 <td>${p.age}</td>
 <td>${p.salary}</td>
 <td>${p.birthday}</td>
</tr>
</c:forEach>
</table>

	<sf:pager pageSize="${data.pageSize}" pageNo="${data.pageNo}" totalPage="${data.totalPage}" jsCallback="submitdata()"/>
	<script language="javascript">
	function submitdata()
	{
		document.forms[0].submit();
	}
	</script>
</form>
<input type="button" value="test json list" onclick="listJson()" />
<input type="button" value="test json object" onclick="detailJson()" />
<input type="button" value="submit json object" onclick="saveJsonDemo()" />
<input type="button" value="test xml object" onclick="getXml()" />
<input type="button" value="test xml list" onclick="getXmllist()" />
<input type="button" value="test read only" onclick="readOnlyList()" />
<input type="button" value="test Josn Direct" onclick="testJosnDirect()" />
<input type="button" value="test Josn String cached" onclick="testJosnString()" />
</body>
</html>