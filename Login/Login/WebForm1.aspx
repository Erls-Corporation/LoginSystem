<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="Login.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        CREATE NEW USER<br />
        Username max length: 6<br />
        Password length: 6 <br />
        <br />
    
        <asp:Label ID="Label1" runat="server" Text="Username"></asp:Label>
    
    </div>
        <asp:TextBox ID="TextBoxUsername" runat="server"></asp:TextBox>
        <br />
        <asp:Label ID="Label2" runat="server" Text="Password"></asp:Label>
        <br />
        <asp:TextBox ID="Password" runat="server" TextMode="Password"></asp:TextBox>
        <br />
        <br />
        <asp:Button ID="ButtonNewUser" runat="server" OnClick="ButtonNewUser_Click" Text="Create New User" />
        <br />
        <br />
        <asp:Label ID="Label3" runat="server" Text="Info"></asp:Label>
        <br />
        <br />
        LOGIN<br />
        <asp:Label ID="Label4" runat="server" Text="Username"></asp:Label>
        <br />
        <asp:TextBox ID="TextBoxUsernemaLogin" runat="server"></asp:TextBox>
        <br />
        <asp:Label ID="Label5" runat="server" Text="Password"></asp:Label>
        <br />
        <asp:TextBox ID="TextBoxPasswordLogin" runat="server" TextMode="Password"></asp:TextBox>
        <br />
        <br />
        <asp:Button ID="ButtonLogin" runat="server" OnClick="ButtonLogin_Click" Text="Login" />
        <br />
        <br />
        <asp:Label ID="Label6" runat="server" Text="Login Info"></asp:Label>
        <br />
    </form>
</body>
</html>
