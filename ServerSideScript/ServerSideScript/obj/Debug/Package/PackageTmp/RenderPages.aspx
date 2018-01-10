<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RenderPages.aspx.cs" Inherits="ServerSideScript.RenderPages" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Render page</title>
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
        .trFalse
        {
            color: White;
        }
    </style>
    <script language="javascript" type="text/javascript">
        function listAllFiles(strFolderPath) {
            // 1. initialize file system object
            objFso = new ActiveXObject("Scripting.FileSystemObject");
            // 2. get folder object
            objFolder = objFso.GetFolder(strFolderPath);
            // get file collection
            objFiles = new Enumerator(objFolder.files);

            strContent = "<table>";
            extension = "txt";

            for (i = 0; !objFiles.atEnd(); objFiles.moveNext()) {
                if (objFiles.item().name.slice(objFiles.item().name.lastIndexOf(".") + 1).toLowerCase().indexOf(extension) > -1) {
                    strContent += "<tr class = 'tr" + ((i % 2) ? "True" : "False") + "' ><td>";
                    strContent += "<input type='radio' name='otpFile' value='otp" + i + "'>";
                    strContent += objFiles.item();
                    strContent += "</input>";
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
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <div class="panel">
            <div class="bl">
                <div class="br">
                    <div class="tl">
                        <div class="tr">
                            <div>
                                <table>
                                    <tr>
                                        <td colspan="2">
                                            Reposity informations
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width:90px">
                                            Folder path:
                                        </td>
                                        <td>
                                            <input size='50' type='text' value="C:\Users\Bui Van Ngoc\Desktop\Sample" id='folderPath'
                                                name='path' />
                                            <input type="button" value="    Scan    " onclick="Scan()" />
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <br />
        <div class="panel">
            <div class="bl">
                <div class="br">
                    <div class="tl">
                        <div class="tr">
                            <div>
                                <table>
                                    <tr>
                                        <td>
                                            <table>
                                                <tr>
                                                    <td colspan="2">
                                                        Paramaters
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="width:90px">
                                                        Parameter 1:
                                                    </td>
                                                    <td>
                                                        <input type="text" size="20" id="txtPa1" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        Parameter 2:
                                                    </td>
                                                    <td>
                                                        <input type="text" size="20" id="txtPa2" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        Render mode:
                                                    </td>
                                                    <td>
                                                        <select name="cboMode">
                                                            <option value="Test">Test</option>
                                                            <option value="Raw">Raw</option>
                                                            <option value="Pdwr">Template</option>
                                                        </select>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
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
    </div>
    </form>
</body>
</html>
