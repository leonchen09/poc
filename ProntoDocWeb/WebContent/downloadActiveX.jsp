<%@ page language="java" contentType="text/html; charset=GB18030"
    pageEncoding="GB18030"%>
<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=GB18030">
<style type="text/css">
        body
        {
            background: #fff;
            width:80%;
            margin-left:10%;
        }
        .panel
        {
            background: url(images/dot.gif) 0 0 repeat;
        }
        .bl
        {
            background: url(images/bl.gif) 0 100% no-repeat;
        }
        .br
        {
            background: url(images/br.gif) 100% 100% no-repeat;
        }
        .tl
        {
            background: url(images/tl.gif) 0 0 no-repeat;
        }
        .tr
        {
            background: url(images/tr.gif) 100% 0 no-repeat;
        }
        #divContent table
        {
            border: 1px solid white;
            width: 100%;
        }
        .trTrue
        {
            background: #DDAA55;
        }
        .trTrue td a
        {
        	text-decoration: none;
        }
        .trFalse
        {
            color: White;
        }
        .trFalse td a
        {
        	text-decoration: none;
        	color: White;
        }
    </style>
<title>Welcome</title>
  <title>Sample document render page</title>
  <script language="javascript" type="text/javascript">
  function getusername()
		{
	
		var WshNetwork = new ActiveXObject("WScript.Network");
		
		alert("Domain = " + WshNetwork.UserDomain);
		
		alert("Computer Name = " + WshNetwork.ComputerName);
		
		alert("User Name = " + WshNetwork.UserName);
		
		}
    var keepTrying;
    alert("1");
    return false;
    alert("2");
    try{
       var objFSO = new ActiveXObject("Scripting.FileSystemObject");
    } catch(ex){
       alert("Cannot create ActiveX Object, please make sure you enable it.");
    }
    function Scan() {
    	alert("1111");
      if (objFSO != null && objFSO.FileExists('D:\\Source\\POC\\ProntoDocWeb\\WebContent\\PdwrMode.pdwr')) {
        clearInterval(keepTrying);
        var contextRoot = "/ProntoDocWeb";
        try {
          window.location = contextRoot +"/PdwrMode.pdwr";
          } catch (ex) {
            alert("Cannot open rendered document.");
            }
      }
    }
    keepTrying = setInterval('Scan()', 50000);
  </script>

<!--
<script language="javascript" type="text/javascript">
        function listAllFiles(strFolderPath) {
        	
            // 1. initialize file system object
            var objFso = new ActiveXObject("Scripting.FileSystemObject");
            // 2. get folder object
            objFolder = objFso.GetFolder(strFolderPath);
            // get file collection
            objFiles = new Enumerator(objFolder.files);
            strContent = "<table>";
            extension = document.getElementById('extensionipt').value;

            for (i = 0; !objFiles.atEnd(); objFiles.moveNext()) {
                filePath = objFiles.item();
                fileName = filePath.name;
                if (fileName.slice(fileName.lastIndexOf(".") + 1).toLowerCase().indexOf(extension) > -1) {
                    strContent += "<tr class = 'tr" + ((i % 2) ? "True" : "False") + "' ><td>";
                    
                    strContent += "<a href='" + filePath + "'>";
                    strContent += fileName;
                    
                    strContent += "</a>";
                    strContent += "</td></tr>";

                    i++;
                }
            }

            strContent += "</table>";
            
            return strContent;
        }

        function Scan() {
            strFolderPath = document.getElementById('folderPath').value;

            strContent = listAllFiles(strFolderPath);

            document.getElementById('divContent').innerHTML = strContent;
        }

        setInterval('Scan()', 60000);
        function loaddocx(){
        	document.getElementById('ifrdoc').src="http://localhost:8080/ProntoDocWeb/PdwrMode.pdwr";
        }
    </script>
    -->
</head>
<body>
Hello, welcome to Pronto, this is first page.
file path:<input size='50' type='text'  value="E:\ProntoDir\CompareXml2003and2007format" id='folderPath' name='path' />
extension:<input size='50' type='text'  value="pdf" id='extensionipt' name='extensionipt' />
                                            <input type="button" value="    Scan    " onclick="Scan()" />
                                            <p>
<a href="OpenOffice2003.pdf" target="_self">OpenOffice2003</a><br>
<a href="PdwrMode.pdwr" target="_self">PdwrMode</a><input type=button value="test" onclick="loaddocx()"/> <br>
<a href="PdwrMode.docx">docxMode</a><br>
<a href="Sx_DB2.doc">realDocx</a><br>
<a href="sample.mwxml">sample.mwxml</a> <a href="sample1.mwxml">sample_new1.mwxml</a>
<input type="button" value="ddd" onclick="getusername()" />
<iframe id="ifrdoc" width="100%" height="800" src=""></iframe>
<p>
        <div class="panel">
            <div class="bl">
                <div class="br">
                    <div class="tl">
                        <div class="tr">
                            <div id="divContent" /> files will be show here.
                        </div>
                    </div>
                </div>
            </div>
        </div>
</body>
</html>