<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ResultPage.aspx.cs" Inherits="ServerSideScript.ResultPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
        var id;

        function Open(path) {
            var w = new ActiveXObject('Word.Application');
            var docText;
            var obj;
            if (w != null) {
                w.Visible = true;
                obj = w.Documents.Open(path);
            }
        }

        function Scan(path) {
            //strFolderPath = document.getElementById('folderPath').value;
            var objFso = new ActiveXObject("Scripting.FileSystemObject");

            if (objFso.FileExists(path)) {
                document.getElementById('divContent').innerHTML = "Ready!";
                Open(path);
                clearInterval(id);
            }
        }
        Scan('<%= Path %>');
        id = setInterval('Scan(\'<%= Path %>\')', 30000);
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <input type="button" value="Refresh" onclick="Scan('<%= Path %>')" />

    <div>
        <div id="divContent"/>
        Listening...
    </div>
    </form>
    <script type="text/javascript">
    </script>
</body>
</html>
