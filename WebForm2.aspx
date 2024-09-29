<%@ Page Language="C#"  MasterPageFile="~/NestedMasterPage1.Master" AutoEventWireup="true" CodeBehind="WebForm2.aspx.cs" Inherits="WebApplication2.WebForm2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <main aria-labelledby="title">

        <asp:DropDownList ID="ddlUser" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlUser_SelectedIndexChanged">
            
        </asp:DropDownList>
<br />

        
<br />

        <asp:Calendar ID="CalendarPlanning" runat="server" OnDayRender="DayRender"/>

    </main>

</asp:Content>

