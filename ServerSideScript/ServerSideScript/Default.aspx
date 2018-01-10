<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ServerSideScript.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <div>
        <table>
            <tr>
                <td>
                Template Name:
                </td>
                 <td>
        <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>

                </td>
                 <td>
                 <asp:Button ID="btnSubmit" runat="server" Text="Submit" onclick="btnSubmit_Click" />
                </td>
            </tr>
        </table>
    </div>
    
    </div>
    </form>
</body>
</html>
