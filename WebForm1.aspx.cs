using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApplication2.Models;
using AjaxControlToolkit;
using System.Globalization;

namespace WebApplication2
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        private BaseClass baseClass= new BaseClass();
        protected void Page_Load(object sender, EventArgs e)
        {
            Calendar2.VisibleDate = DateTime.Now;
            NewCalendarControl.VisibleDate = DateTime.Now;
            NewCalendarControl2.VisibleDate = DateTime.Now;
            txtDemandeur.Text = Context.User.Identity.Name;
            VerifierEquipeUtilisateur();

        }


        private void VerifierEquipeUtilisateur()
        {
            // Récupérer le nom de l'utilisateur connecté
            string userName = Context.User.Identity.GetUserName();
            var userManager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();

            // Récupérer l'utilisateur à partir de son nom
            ApplicationUser user = userManager.FindByName(userName);

            if (user != null)
            {
                // Supposons que l'équipe de l'utilisateur soit stockée dans une propriété appelée "Team"
                string userTeam = user.Equipe;

                // Vérifier si l'utilisateur fait partie de l'équipe "Phoenix"
                if (userTeam == "Phoenix")
                {
                    RadioButton3.Visible = true;
                }
                else
                {
                    RadioButton3.Visible = false;
                }
            }
        }
        private bool isJourPeriodeRFJ(DateTime date)
        {
            int month = date.Month;
            int day = date.Day;

            if ((month == 7 && day >= 15) || (month == 8 && day <= 15) ||
                (month == 12 && day >= 15) || (month == 1 && day <= jourFinPremiereSemaineJanvier(date.Year)))
            {
                return true;
            }
            return false;
        }

        private int jourFinPremiereSemaineJanvier(int annee)
        {
            DateTime premierJanvier = new DateTime(annee, 1, 1); // 1er janvier de l'année donnée
            DayOfWeek jourSemaine = premierJanvier.DayOfWeek; // Jour de la semaine

            if (jourSemaine == DayOfWeek.Sunday || jourSemaine == DayOfWeek.Saturday)
            {
                return 1;
            }
            else
            {
                // Calculer le premier vendredi de janvier
                int joursJusquVendredi = (5 - (int)jourSemaine + 7) % 7; // Nombre de jours jusqu'au vendredi
                return 1 + joursJusquVendredi; // Vendredi de la première semaine
            }
        }
        protected void CustomValidatorRadio_ServerValidate(object source, ServerValidateEventArgs args)
        {
            // Vérifier si au moins un bouton radio est sélectionné
            args.IsValid = RadioButton1.Checked || RadioButton2.Checked || RadioButton3.Checked;
        }


        protected void OnSelectionChanged(object sender, EventArgs e)
        {
            DateTime selectedDate = Calendar2.SelectedDate;
            string dateString = selectedDate.ToString("yyyy-MM-dd");
            Label1C2.Text = "Vous avez sélectionné la date : " + selectedDate.ToShortDateString();
            VerifierEquipeUtilisateur();
            txtPremierJour.Text = selectedDate.ToShortDateString();
            txtDernierJour.Text = selectedDate.ToShortDateString();
            NewCalendarControl.SelectedDate = selectedDate;
            NewCalendarControl2.SelectedDate = selectedDate;



            // Assurez-vous que le script JavaScript openModalWithDate existe sur votre page
            string script = $"openModal();";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenModal", script, true);
        }


        protected void OnSelectionChanged2(object sender, EventArgs e)
        {
            DateTime selectedDate = NewCalendarControl.SelectedDate;

            Calendar2.SelectedDate = selectedDate;


            txtDernierJour.Text = selectedDate.ToString("dd/MM/yyyy");
            PopupControlExtenderTxtPremierJour.Commit(selectedDate.ToString("dd/MM/yyyy"));

        }

        protected void OnSelectionChangedDernierJour(object sender, EventArgs e)
        {

            DateTime selectedDate = NewCalendarControl2.SelectedDate;

            PopupControlExtenderTxtDernierJour.Commit(selectedDate.ToString("dd/MM/yyyy"));

        }

        protected void Calendar2_DayRender(object sender, DayRenderEventArgs e)
        {
            // Call the BaseCalendar method to keep the original functionality
            baseClass.Calendar_DayRender(sender, e);
            int moisVisible = Calendar2.VisibleDate.Month;
            int anneeVisible = Calendar2.VisibleDate.Year;
            string userName = Context.User.Identity.GetUserName();
            var userManager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            string userId = userManager.GetUserIdByUsername(userName);



            // Récupérer l'utilisateur à partir de son nom
            ApplicationUser user = userManager.FindByName(userName);

            if (user != null)
            {
                // Supposons que l'équipe de l'utilisateur soit stockée dans une propriété appelée "Team"
                string userTeam = user.Equipe;

                // Vérifier si l'utilisateur fait partie de l'équipe "Phoenix"
                if (userTeam == "Phoenix")
                {

                    using (var context = new ApplicationDbContext())
                    {
                        // Récupérer tous les statuts pour l'utilisateur sélectionné
                        Dictionary<DateTime, string> statutsParDate = context.GetStatusByDateForUser(moisVisible, anneeVisible, userId);

                        // Vérifier si la date actuelle est dans le dictionnaire
                        if (statutsParDate.ContainsKey(e.Day.Date))
                        {
                            string statut = statutsParDate[e.Day.Date];

                            e.Cell.BackColor = baseClass.GetColorByStatus(statut); // Couleur par défaut

                            e.Cell.ToolTip = statut;
                            e.Day.IsSelectable = false;
                            e.Cell.Attributes.Add("class", "nonAccessible");
                        }
                    }

                }
            }



            if (e.Day.Date.Month != Calendar2.VisibleDate.Month)
            {
                // Griser la cellule pour indiquer qu'elle fait partie d'un autre mois
                e.Cell.ForeColor = System.Drawing.Color.White;
                e.Cell.BackColor = System.Drawing.Color.White;
                // Désactiver les liens pour les jours hors du mois visible
                e.Day.IsSelectable = false;
            }

        }

        protected void NewCalendarControl_DayRender(object sender, DayRenderEventArgs e)
        {
            // Call the BaseCalendar method to keep the original functionality
            baseClass.Calendar_DayRender(sender, e);
            int moisVisible = NewCalendarControl.VisibleDate.Month;
            int anneeVisible = NewCalendarControl.VisibleDate.Year;
            string userName = Context.User.Identity.GetUserName();
            var userManager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            string userId = userManager.GetUserIdByUsername(userName);

            ApplicationUser user = userManager.FindByName(userName);

            if (user != null)
            {
                // Supposons que l'équipe de l'utilisateur soit stockée dans une propriété appelée "Team"
                string userTeam = user.Equipe;

                // Vérifier si l'utilisateur fait partie de l'équipe "Phoenix"
                if (userTeam == "Phoenix")
                {
                    using (var context = new ApplicationDbContext())
                    {
                        Dictionary<DateTime, string> statutsParDate = context.GetStatusByDateForUser(moisVisible, anneeVisible, userId);

                        // Vérifier si la date actuelle est dans le dictionnaire
                        if (statutsParDate.ContainsKey(e.Day.Date))
                        {
                            string statut = statutsParDate[e.Day.Date];

                            e.Cell.BackColor = baseClass.GetColorByStatus(statut); // Couleur par défaut

                            e.Cell.ToolTip = statut;
                            e.Day.IsSelectable = false;
                            e.Cell.Attributes.Add("class", "nonAccessible");
                        }


                        if (RadioButton3.Checked)
                        {
                            if (!isJourPeriodeRFJ(e.Day.Date))
                            {
                                e.Day.IsSelectable = false; // Désactiver le jour
                                e.Cell.BackColor = System.Drawing.Color.Violet; // Optionnel : couleur de fond pour les jours désactivés
                                e.Cell.ForeColor = System.Drawing.Color.Brown; // Optionnel : couleur du texte pour les jours désactivés
                            }


                        }
                    }

                }
            }



            if (e.Day.Date.Month != NewCalendarControl.VisibleDate.Month)
            {
                // Griser la cellule pour indiquer qu'elle fait partie d'un autre mois
                e.Cell.ForeColor = System.Drawing.Color.White;
                e.Cell.BackColor = System.Drawing.Color.White;
                // Désactiver les liens pour les jours hors du mois visible
                e.Day.IsSelectable = false;
            }

        }


        protected void NewCalendarControl2_DayRender(object sender, DayRenderEventArgs e)
        {
            // Call the BaseCalendar method to keep the original functionality
            baseClass.Calendar_DayRender(sender, e);
            int moisVisible = NewCalendarControl2.VisibleDate.Month;
            int anneeVisible = NewCalendarControl2.VisibleDate.Year;
            string userName = Context.User.Identity.GetUserName();
            var userManager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            string userId = userManager.GetUserIdByUsername(userName);

            ApplicationUser user = userManager.FindByName(userName);
            if (user != null)
            {
                // Supposons que l'équipe de l'utilisateur soit stockée dans une propriété appelée "Team"
                string userTeam = user.Equipe;

                // Vérifier si l'utilisateur fait partie de l'équipe "Phoenix"
                if (userTeam == "Phoenix")
                {
                    using (var context = new ApplicationDbContext())
                    {
                        Dictionary<DateTime, string> statutsParDate = context.GetStatusByDateForUser(moisVisible, anneeVisible, userId);

                        // Vérifier si la date actuelle est dans le dictionnaire
                        if (statutsParDate.ContainsKey(e.Day.Date))
                        {
                            string statut = statutsParDate[e.Day.Date];

                            e.Cell.BackColor = baseClass.GetColorByStatus(statut); // Couleur par défaut

                            e.Cell.ToolTip = statut;
                            e.Day.IsSelectable = false;
                            e.Cell.Attributes.Add("class", "nonAccessible");
                        }




                        if (RadioButton3.Checked)
                        {
                            if (!isJourPeriodeRFJ(e.Day.Date))
                            {
                                e.Day.IsSelectable = false; // Désactiver le jour
                                e.Cell.BackColor = System.Drawing.Color.Violet; // Optionnel : couleur de fond pour les jours désactivés
                                e.Cell.ForeColor = System.Drawing.Color.Brown; // Optionnel : couleur du texte pour les jours désactivés
                            }


                        }
                    }

                }
            }

            if (e.Day.Date < NewCalendarControl.SelectedDate)
            {
                e.Day.IsSelectable = false; // Désactiver le jour
                e.Cell.BackColor = System.Drawing.Color.LightGray; // Optionnel : couleur de fond pour les jours désactivés
                e.Cell.ForeColor = System.Drawing.Color.Gray; // Optionnel : couleur du texte pour les jours désactivés
            }


            if (e.Day.Date.Month != NewCalendarControl2.VisibleDate.Month)
            {
                // Griser la cellule pour indiquer qu'elle fait partie d'un autre mois
                e.Cell.ForeColor = System.Drawing.Color.White;
                e.Cell.BackColor = System.Drawing.Color.White;
                // Désactiver les liens pour les jours hors du mois visible
                e.Day.IsSelectable = false;
            }

        }


        private string ObtenirCouleurPourStatut(string statut)
        {
            switch (statut)
            {
                case "En attente de validation RH":
                    return "Gold";
                case "Validé":
                    return "LightGreen";
                case "En attente de validation de Responsable":
                    return "DarkOrange";
                case "Refusé":
                    return "Red";
                default:
                    return "White";
            }
        }


        protected void ValidateDatesRFJ(object source, ServerValidateEventArgs args)
        {
            if (RadioButton3.Checked)
            {
                // Récupérer les valeurs des champs DateDebut et DateFin
                DateTime dateDebut, dateFin;
                bool isDateDebutValid = DateTime.TryParse(txtPremierJour.Text, out dateDebut);
                bool isDateFinValid = DateTime.TryParse(txtDernierJour.Text, out dateFin);

                if (!isDateDebutValid || !isDateFinValid)
                {
                    args.IsValid = false;
                    return;
                }

                // Vérifier que les deux dates sont dans la période RFJ
                if (!isJourPeriodeRFJ(dateDebut) || !isJourPeriodeRFJ(dateFin))
                {
                    args.IsValid = false;
                    return;
                }

                // Calculer la différence entre les deux dates
                int nbJours = (dateFin - dateDebut).Days + 1;



                // Vérifier la condition avec nbJoursDiff
                if (nbJours < 0 || nbJours > 20)
                {
                    args.IsValid = false;
                    return;
                }

                args.IsValid = true;
            }
            else
            {
                args.IsValid = true;
            }
        }


        protected void ValidateNoteLengthAndCharacters(object source, ServerValidateEventArgs args)
        {
            string note = txtNote.Text;

            // Expression régulière pour vérifier qu'il n'y a pas de caractères spéciaux
            string regexPattern = @"^[a-zA-Z0-9\s.,-]*$";  // Accepte lettres, chiffres, espaces, points, tirets, virgules

            // Vérifier si le champ n'est pas vide
            if (!string.IsNullOrEmpty(note))
            {
                // Vérifier si la longueur de la note est inférieure à 100 caractères
                if (note.Length <= 100)
                {
                    // Vérifier si la note ne contient pas de caractères spéciaux
                    if (System.Text.RegularExpressions.Regex.IsMatch(note, regexPattern))
                    {
                        args.IsValid = true;  // Valide
                    }
                    else
                    {
                        args.IsValid = false; // Invalide, contient des caractères spéciaux
                    }
                }
                else
                {
                    args.IsValid = false; // Invalide, plus de 100 caractères
                }
            }
            else
            {
                // Le champ est vide, il est optionnel donc il est valide
                args.IsValid = true;
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            Page.Validate();
            if (Page.IsValid)
            {

                string userName = txtDemandeur.Text;
                string userId;

                // Obtenir l'ID utilisateur à partir du nom d'utilisateur
                var userManager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
                userId = userManager.GetUserIdByUsername(userName);

                if (userId != null)
                {

                    if (RadioButton3.Checked)
                    {

                        DateTime dateDebut;
                        DateTime dateFin;
                        string note = txtNote.Text;

                        // Valider et convertir les dates
                        if (DateTime.TryParse(txtPremierJour.Text, out dateDebut) &&
                            DateTime.TryParse(txtDernierJour.Text, out dateFin))
                        {
                            // Créer et ajouter la demande RFJ
                            using (var context = new ApplicationDbContext())
                            {
                                context.CreateAndAddDemandeRFJ(
                                    userId: userId,
                                    dateDebut: dateDebut,
                                    dateFin: dateFin,
                                    etat: "en attente de validation RH",
                                    note: note
                                );


                                //Demande RFJ créée et ajoutée avec succès 



                            }
                        }



                    }
                    //gestion des congés et absence

                }

                Response.Redirect(Request.RawUrl);

            }
            else
            {
                string script = $"openModal();";
                ScriptManager.RegisterStartupScript(this, GetType(), "open", script, true);
            }
        }


        private int GetNbJourRestant(string userId, DateTime selectedDate)
        {
            using (var context = new ApplicationDbContext())
            {
                var periode = context.GetPeriodeFromDate(selectedDate);
                var infoRFJ = context.GetInfoRFJByUserIdAndPeriode(userId, periode);

                int anneePeriode = selectedDate.Month >= 6 ? selectedDate.Year : selectedDate.Year - 1;
                if (infoRFJ != null)
                {
                    // Si les informations sont disponibles dans la base de données, utiliser la valeur des jours restants
                    return infoRFJ.NbRestant;
                }
                else
                {
                    // Si aucune donnée n'est disponible, utiliser le calcul par défaut
                    return BaseClass.NbJourReposForfaitAPoser(anneePeriode);
                }

            }
            // Convertir la date sélectionnée en une période. Exemple : période = "2023-2024"



        }
    }
}