<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CReport.aspx.cs" Inherits="AGEERP.CReport" %>

<%@ Register assembly="CrystalDecisions.Web, Version=13.0.4000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" namespace="CrystalDecisions.Web" tagprefix="CR" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <CR:CrystalReportViewer ID="CrystalReportViewer1" runat="server" AutoDataBind="true" DisplayToolbar="True" EnableDatabaseLogonPrompt="False" ToolPanelView="None" EnableParameterPrompt="False" HasToggleGroupTreeButton="false" HasToggleParameterPanelButton="false"  />
        </div>
    </form>
    <script>
        function CloseWindow() {
            window.close();
        }
    </script>
</body>
</html>
