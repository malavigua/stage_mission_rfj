<%@ Page Language="C#" MasterPageFile="~/NestedMasterPage1.Master" AutoEventWireup="true" CodeBehind="Validation.aspx.cs" Inherits="WebApplication2.Validation" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <main aria-labelledby="title">
                <asp:UpdatePanel ID="UpdatePanelGridView" runat="server">
<ContentTemplate>
        <div>
            
            
            <asp:DropDownList ID="ddlEtat" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlEtat_SelectedIndexChanged">
                
            </asp:DropDownList>
                    
            <asp:GridView ID="GridViewDemandes" runat="server" AutoGenerateColumns="false" OnRowCommand="GridViewDemandes_RowCommand" OnRowDataBound="GridViewDemandes_RowDataBound" GridLines="None" CssClass="GridViewStyle">
                <Columns>
                    <asp:BoundField DataField="UserName" HeaderText="Nom d'utilisateur" HeaderStyle-CssClass="GridViewColumnHeader" ItemStyle-CssClass="GridViewColumnItem" />
                    <asp:BoundField DataField="DateDebut" HeaderText="Date Début" DataFormatString="{0:dd/MM/yyyy}" HeaderStyle-CssClass="GridViewColumnHeader" ItemStyle-CssClass="GridViewColumnItem" />
                    <asp:BoundField DataField="DateFin" HeaderText="Date Fin" DataFormatString="{0:dd/MM/yyyy}" HeaderStyle-CssClass="GridViewColumnHeader" ItemStyle-CssClass="GridViewColumnItem" />
                    <asp:BoundField DataField="DateCreation" HeaderText="Date Création" DataFormatString="{0:dd/MM/yyyy}" HeaderStyle-CssClass="GridViewColumnHeader" ItemStyle-CssClass="GridViewColumnItem" />
                    <asp:BoundField DataField="Equipe" HeaderText="Équipe" HeaderStyle-CssClass="GridViewColumnHeader" ItemStyle-CssClass="GridViewColumnItem" />
                    <asp:BoundField DataField="NoteFacultative" HeaderText="Note" HeaderStyle-CssClass="GridViewColumnHeader" ItemStyle-CssClass="GridViewColumnItem" />
                    <asp:BoundField DataField="Etat" HeaderText="État" HeaderStyle-CssClass="GridViewColumnHeader" ItemStyle-CssClass="GridViewColumnItem" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Button ID="ButtonAccept" runat="server" CommandName="Accept" CommandArgument='<%# Eval("Id") + ";" + Eval("DateDebut") %>' Text="Accepter" />

                        </ItemTemplate>
                        <HeaderStyle CssClass="GridViewColumnHeader" />
                        <ItemStyle CssClass="GridViewColumnItem" />
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Button ID="ButtonReject" runat="server" CommandName="Reject" CommandArgument='<%# Eval("Id") + ";" + Eval("DateDebut") %>' Text="Refuser" />
                        </ItemTemplate>
                        <HeaderStyle CssClass="GridViewColumnHeader" />
                        <ItemStyle CssClass="GridViewColumnItem" />
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>

           </div>

        <asp:Panel ID="PanelValidation" runat="server" CssClass="validation-panel">

            <div id="DivValidation">
                <span class="close" onclick="closePanelValidation()">&times;</span>
                <asp:Literal ID="Literal2" runat="server">contenu modal</asp:Literal>

                <asp:ValidationSummary ID="ValidationSummary1" runat="server" HeaderText="Validation Errors:"  ForeColor="Red" />

                 <table class="validation-table">
            <tr>
                <td>Compteur</td>
                <td>Restant</td>
                <td>Acquis</td>
            </tr>
            <tr>
                <td>Jours de repos RFJ</td>
                <td>
                    <asp:TextBox ID="nbRestant" runat="server"  CssClass="textbox-small"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvNbRestant" runat="server" ControlToValidate="nbRestant" ErrorMessage="Le champ nb restant est requis." ForeColor="Red" Display="None" />
                    <asp:RangeValidator ID="rvNbRestant" runat="server" ControlToValidate="nbRestant" MinimumValue="-5" MaximumValue="20" Type="Integer" ErrorMessage="La valeur doit être entre -5 et 20." ForeColor="Red" display="None" />
                  
                    <asp:Label ID="nbRestantTxt" runat="server"  CssClass="text-small"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="nbAcquis" runat="server" CssClass="textbox-small" readOnly="true"></asp:TextBox>
                     
                    <asp:Label ID="nbAcquisTxt" runat="server" CssClass="text-small" ></asp:Label>
                </td>
            </tr>
        </table>
                
      <div style="text-align: center; margin-top: 10px;">
        <asp:Button ID="ButtonAccepter" runat="server" Text="Accepter" CommandName="Accepter" OnCommand="HandleCommand"  CausesValidation="true"/>
<asp:Button ID="ButtonRejeter" runat="server" Text="Refuser" CommandName="Refuser" OnCommand="HandleCommand" />

    </div>

            </div>
        </asp:Panel>

  </ContentTemplate>
</asp:UpdatePanel>
        
    </main>

    <script type="text/javascript">
        function closePanelValidation() {
            var panel = document.getElementById("<%= PanelValidation.ClientID %>");
            panel.style.display = "none";
        }

        function showPanel(button) {
            var panel = document.getElementById("<%= PanelValidation.ClientID %>");
            var rect = button.getBoundingClientRect();

            

            // Positionner le panel juste en dessous du bouton
            panel.style.top = (rect.bottom + window.scrollY) + "px";
            panel.style.left = (rect.left + window.scrollX - 300) + "px";
            panel.style.display = "block";
        }

        function showPanelValidation() {
            var panel = document.getElementById("<%= PanelValidation.ClientID %>");
            panel.style.display = "block";
        }
    

    </script>
</asp:Content>

