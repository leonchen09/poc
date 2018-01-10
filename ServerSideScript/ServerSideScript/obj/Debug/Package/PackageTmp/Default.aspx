<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="Default.aspx.cs" Inherits="ServerSideScript._Default" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head>
    <title>Default</title>
    <script type="text/javascript" src="scripts/jquery-1.1.js"></script>
    <script type="text/javascript">
        $(function () {
            //this code is executed when the page's onload event fires
            $("a#runSample1").click(function () {
                $.get("FilesListing.aspx", function (response) {
                    document.getElementById('divContent').innerHTML = response;
                });
            });
        });

        function Test() {
            $.get("FilesListing.aspx", function (response) {
                document.getElementById('divContent').innerHTML = response;
            });
        }
        setInterval("Test()", 60000);
    </script>
    <link href="styles/wcs.css" type="text/css" />
    <style type="text/css">
        body
        {
            background: #fff;
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
        	width:100%;
        }
        .trTrue
        {
        	background:#DDAA55;
        }
        .trFalse
        {
        	color:White;
        }
    </style>
</head>
<body onload="Test()">
    <form id="Form1" method="post">
    <a href="#" id="runSample1">List all files in folder</a>
    <hr />
    <div class="panel">
        <div class="bl">
            <div class="br">
                <div class="tl">
                    <div class="tr">
                        <div id="divContent" />
                    </div>
                </div>
            </div>
        </div>
    </div>
    </form>
</body>
</html>
