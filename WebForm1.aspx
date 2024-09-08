<%@ Page Language="C#"  MasterPageFile="~/NestedMasterPage1.Master" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="WebApplication2.WebForm1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <main aria-labelledby="title">
        

                <asp:Calendar ID="Calendar2" runat="server" OnSelectionChanged="OnSelectionChanged"  OnDayRender="Calendar2_DayRender"/>
<asp:Label ID="Label1C2" runat="server" Text="Sélectionnez une date"></asp:Label>
<br />
               
            <div id="ModalContainer" class="modal1">
        <div class="modal-header">
            <h4 class="modal-title">CRM</h4>
            <span class="close" onclick="closeModal();" >&times;</span>
        </div>
        <div class="modal-body">
             <div class="demandeur-container">
                <asp:Label ID="lblDemandeur" runat="server" Text="Demandeur" />
                <asp:TextBox ID="txtDemandeur" runat="server" CssClass="modalInput" Placeholder="Nom"  ReadOnly="true"/>
                <div class="RadioButton-container">
                    <asp:RadioButton ID="RadioButton1" runat="server" GroupName="Options" Text="Option 1" />
                    <asp:RadioButton ID="RadioButton2" runat="server" GroupName="Options" Text="Option 2" />
                    <asp:RadioButton ID="RadioButton3" runat="server" GroupName="Options" Text="Option 3" />
                </div>
            </div>
            <br /><br />

            <asp:UpdatePanel ID="UpdatePanelCalendriers" runat="server">
    <ContentTemplate>
        <asp:Label ID="lblPremierJour" runat="server" Text="Premier Jour    " />
        <asp:TextBox ID="txtPremierJour"  runat="server" CssClass="modalInput" Placeholder="Date de début" autoPostBack="true"   />
        
         <ajax:PopupControlExtender ID="PopupControlExtenderTxtPremierJour" runat="server"
    TargetControlID="txtPremierJour" PopupControlID="PanelCal1" Position="Bottom" />
    
                        <br /><br />

                
            
              <asp:Label ID="lblDernierJour" runat="server" Text="Dernier Jour    " />
        <asp:TextBox ID="txtDernierJour" runat="server" CssClass="modalInput" Placeholder="Date de fin" autoPostBack="true" />

        
             <ajax:PopupControlExtender ID="PopupControlExtenderTxtDernierJour" runat="server"
TargetControlID="txtDernierJour" PopupControlID="PanelCal2" Position="Bottom" />

        </ContentTemplate>
                </asp:UpdatePanel>

                        <asp:Panel ID="PanelCal1" runat="server">
    <asp:UpdatePanel ID="UpdatePanelCal1" runat="server">
        <ContentTemplate>
            <asp:Calendar ID="NewCalendarControl" CssClass="calendarContainer" runat="server" OnSelectionChanged="OnSelectionChanged2" OnDayRender="NewCalendarControl_DayRender"/>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Panel>
                                    <asp:Panel ID="PanelCal2" runat="server">
    <asp:UpdatePanel ID="UpdatePanelCal2" runat="server">
        <ContentTemplate>
            <asp:Calendar ID="NewCalendarControl2" CssClass="calendarContainer" runat="server" OnSelectionChanged="OnSelectionChangedDernierJour" OnDayRender="NewCalendarControl2_DayRender" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Panel>

              <br /><br />
  <asp:Label ID="lblNote" runat="server" Text="Note Facultative    " />
  <asp:TextBox ID="txtNote" runat="server" CssClass="modalTextArea" TextMode="MultiLine" Columns="40" Rows="5" />
 </div>

                                    <asp:CustomValidator ID="CustomValidatorRadio" runat="server" 
ClientValidationFunction="validateRadioButton"
OnServerValidate="CustomValidatorRadio_ServerValidate"
ErrorMessage="Veuillez sélectionner une option /!\ " ForeColor="Red"></asp:CustomValidator>

                <asp:CustomValidator 
    ID="CustomValidatorDateValides" 
    runat="server" 
    ControlToValidate="txtDernierJour" 
    OnServerValidate="ValidateDatesRFJ"
    ErrorMessage="La période sélectionnée n'est pas valide."
    Display="Dynamic" 
    ForeColor="Red" />

                <asp:CustomValidator
    ID="CustomValidatorNote"
    runat="server"
    ControlToValidate="txtNote"
    ErrorMessage="La note ne doit pas dépasser 100 caractères et ne doit pas contenir de caractères spéciaux."
    OnServerValidate="ValidateNoteLengthAndCharacters"
    Display="Dynamic"
    ForeColor="Red">
</asp:CustomValidator>

            <div class="modal-footer">
                 <asp:Button ID="btnSubmit" runat="server" Text="Valider" CssClass="modalButton" OnClick="btnSubmit_Click" CausesValidation="true" />
                 <asp:Button ID="btnCancel" runat="server" Text="Annuler" CssClass="modalButton" OnClientClick="closeModal();" />
            </div>
             </div>
                
  </main>


    <script type="text/javascript">

      

        function closeModal() {
            var modal = document.getElementById('ModalContainer');
            if (modal) {
                modal.style.display = 'none';
            }
        }

        function openModal() {
            var modal = document.getElementById('ModalContainer');
            if (modal) {
                modal.style.display = 'block';
            }
        }


        function jourFinPremiereSemaineJanvier(annee) {
            var premierJanvier = new Date(annee, 0, 1); // 1er janvier de l'année donnée
            var jourSemaine = premierJanvier.getDay(); // Jour de la semaine (0-6) : 0 pour dimanche, 1 pour lundi, etc.



            if (jourSemaine === 0 || jourSemaine === 6) { // Dimanche
                return 1;
            } else {
                // Calculer le premier vendredi de janvier
                var joursJusquVendredi = (5 - jourSemaine + 7) % 7; // Nombre de jours jusqu'au vendredi
                return 1 + joursJusquVendredi;// Vendredi de la première semaine
            }


        }

        function validateRadioButton(sender, args) {
            var radioButton1 = document.getElementById('<%= RadioButton1.ClientID %>').checked;
        var radioButton2 = document.getElementById('<%= RadioButton2.ClientID %>').checked;
        var radioButton3 = document.getElementById('<%= RadioButton3.ClientID %>').checked;
            args.IsValid = radioButton1 || radioButton2 || radioButton3;
        }

        function isJourPeriodeRFJ(date) {
            var month = date.getMonth();
            var day = date.getDate();

            if ((month == 6 && day >= 15) || (month == 7 && day <= 15) ||
                (month == 11 && day >= 15) || (month == 0 && day <= jourFinPremiereSemaineJanvier(date.getFullYear()))) {
                return true;
            }
            return false;
        }

    



        function checkCheckbox3Status() {
            var txtDatePremierJour = document.getElementById('<%= txtPremierJour.ClientID %>');
            var txtDateDernierJour = document.getElementById('<%= txtDernierJour.ClientID %>');
            var checkbox3 = document.getElementById('<%= RadioButton3.ClientID %>');
            var premierJourValue = txtDatePremierJour.value;
            var dernierJourValue = txtDateDernierJour.value;
            if (txtDatePremierJour && premierJourValue && txtDateDernierJour && dernierJourValue) {
                const [jour, mois, annee] = premierJourValue.split('/').map(Number);
                const [jour2, mois2, annee2] = dernierJourValue.split('/').map(Number);
                var datePremierJour = new Date(annee, mois - 1, jour); // les mois sont indexés à partir de 0
                var dateDernierJour = new Date(annee2, mois2 - 1, jour2); 

                if ((!isJourPeriodeRFJ(datePremierJour)) || (!isJourPeriodeRFJ(dateDernierJour))) {
                    if (!checkbox3.disabled) {
                        checkbox3.disabled = true;
                        checkbox3.checked = false;
                        checkbox3.style.display = "none";
                        var radiobutton3Label = document.querySelector("label[for='<%= RadioButton3.ClientID %>']");
                        if (radiobutton3Label) {
                            radiobutton3Label.innerHTML = "";
                        }
                    }
                
                }


            }


        }
    function updateCalendar() {
        
       // var checkbox3 = document.getElementById('<%= RadioButton3.ClientID %>');
     //   if (checkbox3) {
      //      if (checkbox3.checked) {
        //        checkCheckbox3Status();
    //        }
      //  }
       // checkCheckbox3Status();
       __doPostBack('<%= UpdatePanelCalendriers.ClientID %>', '');
}


    

    // Exécuter checkCheckbox3Status au chargement de la page
    window.onload = function () {
        // checkCheckbox3Status();
        document.getElementById('<%= RadioButton1.ClientID %>').addEventListener('change', updateCalendar);
        document.getElementById('<%= RadioButton2.ClientID %>').addEventListener('change', updateCalendar);
        document.getElementById('<%= RadioButton3.ClientID %>').addEventListener('change', updateCalendar);
    }




    </script>

<!-- CSS -->
    <style>

         .modal1 {
     display:none;
        position: fixed;
         padding: 0;
  border: 1px solid #888;
  background-color: #fefefe;
  width: 50%;
  right: 20%;
  top: 2%; /* Ajustez cette valeur pour positionner le modal plus haut */
  box-sizing: border-box;
   
  }
  


        .modal-header, .modal-footer {
            background-color: #ff6600;
            color: white;
            padding: 10px;
            box-sizing: border-box;
        }
        .modal-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            border-bottom: 1px solid #dee2e6;
        }
        .modal-body {
            padding: 20px;
            margin: 0;
            box-sizing: border-box;
        }
        .modal-footer {
            display: flex;
            justify-content: flex-end;
            border-top: 1px solid #dee2e6;
        }
        .modalBackground {
            background-color: rgba(0,0,0,0.6);
        }
        .modalButton {
            margin: 10px;
        }
        .close {
            font-size: 28px;
            font-weight: bold;
            cursor: pointer;
        }
        .close:hover {
            color: red;
        }
        .modalInput {
            width: 200px; /* Ajustez la largeur selon vos besoins */
            box-sizing: border-box;
        }
        .modalTextArea {
            width: 70%; /* Ajustez pour remplir l'espace disponible */
            box-sizing: border-box;
            padding: 10px;
        }
       .demandeur-container {
    display: flex;
    flex-wrap: wrap; /* Permet aux éléments de passer à la ligne si nécessaire */
    align-items: center; /* Aligne les éléments verticalement au centre */
    gap: 10px; /* Espace entre les éléments */
    margin-bottom: 10px; /* Ajoute un espace en bas de la section */
    box-sizing: border-box;
}

       .RadioButton-container {
        display: flex;
        gap: 10px;
        align-items: center; /* Aligne les éléments verticalement au centre */
        box-sizing: border-box;
    }


       .calendarContainer {
    position: absolute; /* Positionner le calendrier de manière absolue pour le superposer sur le champ de texte */
    background-color: #fff; /* Couleur de fond du calendrier */
    border: 1px solid #ccc; /* Bordure du calendrier */
    z-index: 1000; /* Assurer que le calendrier se superpose à d'autres éléments */
}
</style>

    </asp:Content>
