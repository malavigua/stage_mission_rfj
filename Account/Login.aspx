<%@ Page Title="Se connecter" Language="C#"  AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="WebApplication2.Account.Login" Async="true" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <link href="Content/Calendrier.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server" >
         <main aria-labelledby="title">
     <h2 id="title"><%: Title %>.</h2>
     <div class="col-md-8">
         <section id="loginForm">
             <div class="row">
                
                 <asp:PlaceHolder runat="server" ID="ErrorMessage" Visible="false">
                     <p class="text-danger">
                         <asp:Literal runat="server" ID="FailureText" />
                     </p>
                 </asp:PlaceHolder>
                 <div class="row">
                     <asp:Label runat="server" AssociatedControlID="UserName" CssClass="col-md-2 col-form-label">Messagerie</asp:Label>
                     <div class="col-md-10">
                         <asp:TextBox runat="server" ID="UserName" CssClass="form-control"  />
                         <asp:RequiredFieldValidator runat="server" ControlToValidate="UserName"
                             CssClass="text-danger" ErrorMessage="Le champ UserName est obligatoire." />
                     </div>
                 </div>
                 <div class="row">
                     <asp:Label runat="server" AssociatedControlID="Password" CssClass="col-md-2 col-form-label">Mot de passe</asp:Label>
                     <div class="col-md-10">
                         <asp:TextBox runat="server" ID="Password" TextMode="Password" CssClass="form-control" />
                         <asp:RequiredFieldValidator runat="server" ControlToValidate="Password" CssClass="text-danger" ErrorMessage="Le champ du mot de passe est obligatoire." />
                     </div>
                 </div>
                 <div class="row">
                     <div class="offset-md-2 col-md-10">
                         <div class="checkbox">
                             <asp:CheckBox runat="server" ID="RememberMe" />
                             <asp:Label runat="server" AssociatedControlID="RememberMe">Mémoriser le mot de passe ?</asp:Label>
                         </div>
                     </div>
                 </div>
                 <div class="row">
                     <div class="col-offset-md-2 col-md-10">
                         <asp:Button runat="server" OnClick="LogIn" Text="Se connecter" CssClass="btn btn-outline-dark" />
                     </div>
                 </div>
             </div>
           
         </section>
     </div>

  

 </main>
    </form>
</body>
</html>


