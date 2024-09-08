<%@ Page Title="Inscrire" Language="C#"  AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="WebApplication2.Account.Register" %>



<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <link href="Content/Calendrier.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server" >
         <main aria-labelledby="title">
     
  <%--<h2 id="title"><%: Title %>.</h2>--%>
<p class="text-danger">
    <asp:Literal runat="server" ID="ErrorMessage" />
</p>
<h4>Créer un nouveau compte</h4>
<hr />
<asp:ValidationSummary runat="server" CssClass="text-danger" />
<div class="row">
    <asp:Label runat="server" AssociatedControlID="Email" CssClass="col-md-2 col-form-label">Messagerie</asp:Label>
    <div class="col-md-10">
        <asp:TextBox runat="server" ID="Email" CssClass="form-control" TextMode="Email" />
        <asp:RequiredFieldValidator runat="server" ControlToValidate="Email"
            CssClass="text-danger" ErrorMessage="Le champ d’adresse de messagerie est obligatoire." />
    </div>
</div>

             <div class="row">
    <asp:Label runat="server" AssociatedControlID="userName" CssClass="col-md-2 col-form-label">Identifiant</asp:Label>
    <div class="col-md-10">
        <asp:TextBox runat="server" ID="userName"  CssClass="form-control" />
        <asp:RequiredFieldValidator runat="server" ControlToValidate="userName"
            CssClass="text-danger" ErrorMessage="Le champ userName est obligatoire." />
    </div>
</div>

                          <div class="row">
    <asp:Label runat="server" AssociatedControlID="Role" CssClass="col-md-2 col-form-label">Role</asp:Label>
    <div class="col-md-10">
        <asp:DropDownList runat="server" ID="Role"  CssClass="form-control" >
            <asp:ListItem Text="Utilisateur" Value="User" Selected="True" />
            <asp:ListItem Text="Administrateur" Value="Admin"  />
            <asp:ListItem Text="Responsable-Phoenix" Value="Responsable-Phoenix" />
        </asp:DropDownList>
        <asp:RequiredFieldValidator runat="server" ControlToValidate="Role"
           
            CssClass="text-danger" ErrorMessage="Le champ Role est obligatoire." />
    </div>
</div>


             <div class="row">
    <asp:Label runat="server" AssociatedControlID="Equipe" CssClass="col-md-2 col-form-label">Equipe</asp:Label>
    <div class="col-md-10">
        <asp:TextBox runat="server" ID="Equipe" CssClass="form-control"  />
        <asp:RequiredFieldValidator runat="server" ControlToValidate="Equipe"
            CssClass="text-danger" ErrorMessage="Le champ Equipe est obligatoire." />
    </div>
</div>

<div class="row">
    <asp:Label runat="server" AssociatedControlID="Password" CssClass="col-md-2 col-form-label">Mot de passe</asp:Label>
    <div class="col-md-10">
        <asp:TextBox runat="server" ID="Password" TextMode="Password" CssClass="form-control" />
        <asp:RequiredFieldValidator runat="server" ControlToValidate="Password"
            CssClass="text-danger" ErrorMessage="Le champ du mot de passe est obligatoire." />
    </div>
</div>
<div class="row">
    <asp:Label runat="server" AssociatedControlID="ConfirmPassword" CssClass="col-md-2 col-form-label">Confirmer le mot de passe </asp:Label>
    <div class="col-md-10">
        <asp:TextBox runat="server" ID="ConfirmPassword" TextMode="Password" CssClass="form-control" />
        <asp:RequiredFieldValidator runat="server" ControlToValidate="ConfirmPassword"
            CssClass="text-danger" Display="Dynamic" ErrorMessage="Merci, votre compte est valide." />
        <asp:CompareValidator runat="server" ControlToCompare="Password" ControlToValidate="ConfirmPassword"
            CssClass="text-danger" Display="Dynamic" ErrorMessage="Le mot de passe et le mot de passe de confirmation ne correspondent pas." />
    </div>
</div>
<div class="row">
    <div class="offset-md-2 col-md-10">
        <asp:Button runat="server" OnClick="CreateUser_Click" Text="Inscrire" CssClass="btn btn-outline-dark" />
    </div>
</div>

 </main>
    </form>
</body>
</html>