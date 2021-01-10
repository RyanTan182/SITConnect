<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Registration.aspx.cs" Inherits="SITConnect.Registration" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>My Registration</title>
    <script type="text/javascript">
        function validate() {
            var str = document.getElementById('<%=tb_password.ClientID %>').value;

            if (str.length < 8) {
                document.getElementById("lbl_pwdchecker").innerHTML = "Password Length Must be at Least 8 Characters";
                document.getElementById("lbl_pwdchecker").style.color = "Red";
                return ("too_short");
            }
            else if (str.search(/[0-9]/) == -1) {
                document.getElementById("lbl_pwdchecker").innerHTML = "Password require at least 1 number";
                document.getElementById("lbl_pwdchecker").style.color = "Red";
                return ("no_number");
            }
            else if (str.search(/[A-Z]/) == -1) {
                document.getElementById("lbl_pwdchecker").innerHTML = "Password require at least 1 Capital Letter";
                document.getElementById("lbl_pwdchecker").style.color = "Red";
                return ("no_capital");
            }
            else if (str.search(/[a-z]/) == -1) {
                document.getElementById("lbl_pwdchecker").innerHTML = "Password require at least 1 Lowercase Letter";
                document.getElementById("lbl_pwdchecker").style.color = "Red";
                return ("no_lower");
            }
            else if (str.search(/[^A-Za-z0-9]/) == -1) {
                document.getElementById("lbl_pwdchecker").innerHTML = "Password require at least 1 Special Character";
                document.getElementById("lbl_pwdchecker").style.color = "Red";
                return ("no_special");
            }

            document.getElementById("lbl_pwdchecker").innerHTML = "Excellent!"
            document.getElementById("lbl_pwdchecker").style.color = "Blue";
        }
    </script>
    <style type="text/css">
        .auto-style1 {
            width: 221px;
        }
        .auto-style2 {
            width: 122px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div id="container">
            <strong>Registration </strong>
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
                <td class="auto-style2">First Name:</td>
                <td class="auto-style1">
                    <asp:TextBox ID="tb_firstname" runat="server" Width="211px"></asp:TextBox>
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
                <td class="auto-style2">Last Name</td>
                <td class="auto-style1">
                    <asp:TextBox ID="tb_lastname" runat="server" Width="211px"></asp:TextBox>
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
                <td class="auto-style2">Credit Card</td>
                <td class="auto-style1">
                    <asp:TextBox ID="tb_creditcard" runat="server" Width="211px"></asp:TextBox>
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
                <td class="auto-style2">Email Address:</td>
                <td class="auto-style1">
                    <asp:TextBox ID="tb_email" runat="server" Width="211px"></asp:TextBox>
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
            <asp:TextBox ID="tb_password" runat="server" Width="211px" onkeyup="javascript:validate()" TextMode="Password"></asp:TextBox>
                </td>
                <td>
            <asp:Label ID="lbl_pwdchecker" runat="server" Text="pwdchecker"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="auto-style2">&nbsp;</td>
                <td class="auto-style1">&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td class="auto-style2">Confirm Password:</td>
                <td class="auto-style1">
                    <asp:TextBox ID="tb_confirmpassword" runat="server" Width="212px" TextMode="Password"></asp:TextBox>
                </td>
                <td>
                    &nbsp;</td>
            </tr>
            <tr>
                <td class="auto-style2">&nbsp;</td>
                <td class="auto-style1">&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td class="auto-style2">Date of Birth:</td>
                <td class="auto-style1">
                    <asp:TextBox ID="tb_dob" runat="server" Width="211px"></asp:TextBox>
                </td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td class="auto-style2">&nbsp;</td>
                <td class="auto-style1">&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td class="auto-style2">&nbsp;</td>
                <td class="auto-style1">
                    <asp:Button ID="btn_submit" runat="server" Text="Submit" Width="222px" OnClick="btn_submit_Click" />
                </td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td class="auto-style2">&nbsp;</td>
                <td class="auto-style1">
                    &nbsp;</td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td class="auto-style2">&nbsp;</td>
                <td class="auto-style1">
                    <asp:Label ID="errorMsg" runat="server" Text="" Visible="false"></asp:Label>
                </td>
                <td>&nbsp;</td>
            </tr>
        </table>
       

    </form>
</body>
</html>
