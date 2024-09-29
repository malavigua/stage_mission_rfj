using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApplication2.Models;
using static WebApplication2.Models.ApplicationDbContext;

namespace WebApplication2
{
    public partial class WebForm2 : System.Web.UI.Page
    {
        private BaseClass baseClass = new BaseClass();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) // Vérifie si ce n'est pas un PostBack pour éviter de relier les données à chaque chargement
            {
                if (HttpContext.Current.User.IsInRole("Admin") || HttpContext.Current.User.IsInRole("Responsable-Phoenix"))
                {
                    ddlUser.Visible = true;
                    BindUsersToDropDown();
                }
                else if (HttpContext.Current.User.IsInRole("User"))
                {
                    ddlUser.Visible = false;
                }
            }


          



        }


        private void BindUsersToDropDown()
        {
            var userManager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();

            // Récupérer les utilisateurs membres de l'équipe Phoenix
            var utilisateursPhoenix = userManager.GetUsersByTeam("Phoenix");

            // Lier la liste des utilisateurs à la DropDownList
            ddlUser.DataSource = utilisateursPhoenix;
            ddlUser.DataTextField = "UserName"; // Ce qui sera affiché dans la liste
            ddlUser.DataValueField = "Id"; // La valeur associée à chaque utilisateur (peut-être l'ID de l'utilisateur)
            ddlUser.DataBind();

            // Ajouter une option par défaut

            ddlUser.Items.Insert(0, new ListItem("-- Tous --", ""));
        }

        public string AfficherDemandesParJour(Dictionary<DateTime, List<DemandeInfo>> demandesParJour)
        {
            StringBuilder sb = new StringBuilder();

            // Parcourir chaque jour dans le dictionnaire
            foreach (var entry in demandesParJour)
            {
                DateTime date = entry.Key;
                List<DemandeInfo> demandes = entry.Value;

                // Ajouter la date dans la chaîne de caractères
                sb.AppendLine($"Date: {date.ToString("dd/MM/yyyy")}");

                // Parcourir chaque demande pour cette date
                foreach (var demande in demandes)
                {
                    sb.AppendLine($"   Utilisateur: {demande.UserName}, Statut: {demande.Statut}");
                }

                sb.AppendLine(); // Ajout d'une ligne vide entre chaque jour
            }

            return sb.ToString();
        }


        protected void DayRender(object sender, DayRenderEventArgs e)
        {
            // Call the BaseCalendar method to keep the original functionality
            baseClass.Calendar_DayRender(sender, e);
            int moisVisible = CalendarPlanning.VisibleDate.Month;
            int anneeVisible = CalendarPlanning.VisibleDate.Year;

            // Récupérer l'utilisateur sélectionné dans la DropDownList
            string selectedUserId = ddlUser.SelectedValue;

            using (var context = new ApplicationDbContext())
            {

                if (HttpContext.Current.User.IsInRole("Admin") || HttpContext.Current.User.IsInRole("Responsable-Phoenix"))
                {
                    if (!string.IsNullOrEmpty(selectedUserId))
                    {
                        var userManager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();


                        // Récupérer l'utilisateur via l'ID sélectionné
                        ApplicationUser selectedUser = userManager.FindById(selectedUserId);




                        if (selectedUser != null)
                        {



                            // Récupérer tous les statuts pour l'utilisateur sélectionné
                            Dictionary<DateTime, string> statutsParDate = context.GetStatusByDateForUser(moisVisible, anneeVisible, selectedUserId);

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
                    else
                    {

                        List<DemandeRFJ> demandes = context.GetDemandeRFJByMonthYear(moisVisible, anneeVisible);

                        // Obtenir les demandes organisées par jour
                        Dictionary<DateTime, List<DemandeInfo>> demandesParJour = context.GetDatesForMonthAndYearFromDemandes2(demandes, moisVisible, anneeVisible);

                        // Si des demandes existent pour le jour en cours dans le calendrier
                        if (demandesParJour.ContainsKey(e.Day.Date))
                        {
                            List<DemandeInfo> demandesDuJour = demandesParJour[e.Day.Date];
                            StringBuilder tooltip = new StringBuilder();


                            foreach (var demande in demandesDuJour)
                            {
                                // Ajouter l'utilisateur et le statut dans l'info-bulle (tooltip)
                                tooltip.AppendLine($"Utilisateur: {demande.UserName}, Statut: {demande.Statut}");

                            }

                            // Appliquer la couleur et l'info-bulle au jour correspondant
                            e.Cell.BackColor = System.Drawing.Color.Magenta;
                            e.Cell.ToolTip = tooltip.ToString();
                            e.Day.IsSelectable = false;
                            e.Cell.Attributes.Add("class", "nonAccessible");
                        }

                    }

                }
                else if (HttpContext.Current.User.IsInRole("User"))
                {
                    string userName = Context.User.Identity.GetUserName();
                    var userManager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
                    string userId = userManager.GetUserIdByUsername(userName);

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

        protected void ddlUser_SelectedIndexChanged(object sender, EventArgs e)
        {


            CalendarPlanning.DataBind();
        }

        

    }
}