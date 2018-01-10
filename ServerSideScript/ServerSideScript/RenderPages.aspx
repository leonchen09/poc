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
    <script language="javascript" type="text/javascript">
        function listAllFiles(strFolderPath) {
            // 1. initialize file system object
            objFso = new ActiveXObject("Scripting.FileSystemObject");
            // 2. get folder object
            objFolder = objFso.GetFolder(strFolderPath);
            // get file collection
            objFiles = new Enumerator(objFolder.files);

            strContent = "<table>";
            extension = "pdf";
            extension2 = "docx";

            for (i = 0; !objFiles.atEnd(); objFiles.moveNext()) {
                filePath = objFiles.item();
                fileName = filePath.name;
                if ((fileName.slice(fileName.lastIndexOf(".") + 1).toLowerCase().indexOf(extension) > -1)
                    || (fileName.slice(fileName.lastIndexOf(".") + 1).toLowerCase().indexOf(extension2) > -1)) {
                    strContent += "<tr class = \"tr" + ((i % 2) ? "True" : "False") + "\" ><td>";
                    
                    strContent += "<a href=\"" + filePath + "\" target=\"_blank\">";
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
                                            <input size='50' type='text' 
                                                value="C:\Folder Structure\Prontodoc Repository\Sam-PC\PD.Render" id='folderPath'
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
                                                    <td colspan="3">
                                                        Paramaters
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="width:90px">
                                                        Parameter 1:
                                                    </td>
                                                    <td style="width:150px">
                                                        &nbsp;<asp:TextBox ID="txtParam1" runat="server"></asp:TextBox>
                                                    </td>
                                                    <td></td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        Parameter 2:
                                                    </td>
                                                    <td>
                                                        &nbsp;<asp:TextBox ID="txtParam2" runat="server"></asp:TextBox>
                                                    </td>
                                                    <td></td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        Render mode:
                                                    </td>
                                                    <td>
                                                        &nbsp;<asp:DropDownList ID="cboRenderMode" runat="server">
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td></td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        Computer name:</td>
                                                    <td>
                                                        <asp:TextBox ID="txtComputerName" runat="server" Enabled="False" 
                                                            ReadOnly="True">Sam-PC</asp:TextBox>
                                                    </td>
                                                    <td></td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        Created by:</td>
                                                    <td>
                                                        <asp:TextBox ID="txtCreatedBy" runat="server" Enabled="False">Onmi App</asp:TextBox>
                                                    </td>
                                                    <td></td>
                                                </tr>
                                                <tr>
                                                    <td colspan="2" align="right">
                                                        <asp:Button ID="btnGetTemplate" runat="server" Text="Get Template" 
                                                            onclick="btnGetTemplate_Click" />&nbsp;
                                                        <asp:Button ID="btnStartSX" runat="server" Text="Start SX" 
                                                            onclick="btnStartSX_Click" />&nbsp;
                                                        <asp:Button ID="btnRender" runat="server" Text="Render" 
                                                            onclick="btnRender_Click" />
                                                    </td>
                                                    <td></td>
                                                </tr>
                                                <tr>
                                                    <td colspan="3" align="left">
                                                        <asp:Label ID="lblMessage" runat="server" Text=""></asp:Label>
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
        <br />
        <div>
            <asp:GridView ID="grdTemplate" runat="server" 
                AllowSorting="True" AutoGenerateColumns="False" Width="100%" 
                AutoGenerateSelectButton="True" CellPadding="0">
                <Columns>
                    <asp:BoundField DataField="TemplateID" HeaderText="Guid" />
                    <asp:BoundField DataField="PDWName" HeaderText="PDW Name" />
                    <asp:BoundField DataField="Domain" HeaderText="Domain" />
                    <asp:BoundField DataField="Version" HeaderText="Version" />
                </Columns>
                <HeaderStyle BackColor="#3333FF" ForeColor="White" HorizontalAlign="Center" 
                    VerticalAlign="Middle" />
                <PagerSettings FirstPageText="First" LastPageText="Last" NextPageText="Next" 
                    PreviousPageText="Previous" />
                <RowStyle BorderColor="Gray" ForeColor="#666666" HorizontalAlign="Left" 
                    VerticalAlign="Middle" />
                <SelectedRowStyle BackColor="#6699FF" ForeColor="White" />
            </asp:GridView>
        </div>
        <br />
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
