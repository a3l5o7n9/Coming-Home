<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <title></title>
</head>
<body>
  <form id="form1" runat="server">
    <div>
      <asp:Button ID="Button1" runat="server" Text="Start Global Timer" OnClick="Button1_Click" />
    </div>
    <div>
      <asp:Button ID="Button2" runat="server" Text="End Global Timer" OnClick="Button2_Click" />
    </div>
  </form>
</body>
</html>
