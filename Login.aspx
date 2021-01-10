<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="SITConnect.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="https://www.google.com/recaptcha/api.js?render=6LdO-RMaAAAAAGQWXKlrinaL-IVzRnEliyV7MTJE"></script>
    <style type="text/css">
        .auto-style1 {
            width: 238px;
        }
        .auto-style2 {
            width: 115px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div id="container">
            <strong>Login </strong>
            <br />
        </div>
       

        <table style="width:100%;">
            <tr>
                <td class="auto-style2">&nbsp;</td>
                <td class="auto-style1">
                    &nbsp;</td>
                <td>
                    &nbsp;</td>
            </tr>
            <tr>
                <td class="auto-style2">Email Address:</td>
                <td class="auto-style1">
                    <asp:TextBox ID="tb_email" runat="server" Width="211px" TextMode="Email"></asp:TextBox>
                </td>
                <td>
                    &nbsp;</td>
            </tr>
            <tr>
                <td class="auto-style2">&nbsp;</td>
                <td class="auto-style1">
                    &nbsp;</td>
                <td>
                    &nbsp;</td>
            </tr>
            <tr>
                <td class="auto-style2">Password:</td>
                <td class="auto-style1">
                    <asp:TextBox ID="tb_password" runat="server" Width="211px" TextMode="Password"></asp:TextBox>
                </td>
                <td>
                    &nbsp;</td>
            </tr>
            <tr>
                <td class="auto-style2">&nbsp;</td>
                <td class="auto-style1">
                    &nbsp;</td>
                <td>
                    &nbsp;</td>
            </tr>
            <tr>
                <td class="auto-style2">Verification:</td>
                <td class="auto-style1">
                    <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response"/>
                </td>
                <td>
                    &nbsp;</td>
            </tr>
            <tr>
                <td class="auto-style2">&nbsp;</td>
                <td class="auto-style1">
                    <asp:Button ID="btn_submit" runat="server" Text="Submit" Width="222px" OnClick="btn_submit_Click" />
                </td>
                <td>
                    <asp:Button ID="btn_register" runat="server" Text="Register" Width="222px" OnClick="btn_register_Click" />
                </td>
            </tr>
            <tr>
                <td class="auto-style2">&nbsp;</td>
                <td class="auto-style1">
                    &nbsp;</td>
                <td>
                    &nbsp;</td>
            </tr>
            <tr>
                <td class="auto-style2">&nbsp;</td>
                <td class="auto-style1">
                    <asp:Label ID="errorMsg" runat="server" Text="" Visible="false"></asp:Label>
                </td>
                <td>
                    &nbsp;</td>
            </tr>
            <tr>
                <td class="auto-style2">&nbsp;</td>
                <td class="auto-style1">
                    &nbsp;</td>
                <td>
                    &nbsp;</td>
            </tr>
            <tr>
                <td class="auto-style2">&nbsp;</td>
                <td class="auto-style1">
                    <asp:Label ID="lbl_gScore" runat="server" Text=""></asp:Label>
                </td>
                <td>
                    &nbsp;</td>
            </tr>
            </table>
       

    </form>
        <script>
            grecaptcha.ready(function () {
                grecaptcha.execute('6LdO-RMaAAAAAGQWXKlrinaL-IVzRnEliyV7MTJE', { action: 'Login' }).then(function (token) {
                    document.getElementById("g-recaptcha-response").value = token;
                });
            });
        </script>
</body>
</html>
