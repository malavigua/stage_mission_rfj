using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApplication2.Models;

namespace WebApplication2
{  //ajouter contenu textbox nbJourRestant, nbJourAcquis, essayer recupérére date débutom utilisatuer de la ligne
   //avec 'a détreminer si pour cette periode existe deja , si oui update sinon, créer
   //si ya rien pour cet utilisateur et cette période , affciher nb jour,  
    public partial class Validation : System.Web.UI.Page
    {


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PopulateDropDownList();
                BindGridView();
            }
            ValidationSummary1.Visible = false;

        }

        private void BindGridView()
        {
            var userManager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();

            using (var context = new ApplicationDbContext())
            {
                List<DemandeRFJ> demandesList = null;
                string selectedEtat = ddlEtat.SelectedValue;

                if (HttpContext.Current.User.IsInRole("Admin"))
                {
                    demandesList = context.GetDemandeRFJByStatus(selectedEtat);
                }
                else if (HttpContext.Current.User.IsInRole("Responsable-Phoenix"))
                {
                    demandesList = context.GetDemandeRFJByTeamNameAndStatus("Phoenix", selectedEtat);
                }

                if (demandesList != null)
                {



                    var result = demandesList
                        .Select(d => new
                        {
                            d.Id,
                            UserName = userManager.FindById(d.IdUtilisateur).UserName,
                            d.DateDebut,
                            d.DateFin,
                            d.DateCreation,
                            Equipe = userManager.FindById(d.IdUtilisateur).Equipe,
                            d.NoteFacultative,
                            d.Etat
                        })
                        .OrderBy(d => d.DateDebut)
                        .ToList();

                    GridViewDemandes.DataSource = result;
                    GridViewDemandes.DataBind();
                }
            }
        }

        protected void GridViewDemandes_RowCommand(object sender, GridViewCommandEventArgs e)
        {

            string selectedEtat = ddlEtat.SelectedValue;
            if (e.CommandName == "Accept" || e.CommandName == "Reject")
            {
                string[] commandArgs = e.CommandArgument.ToString().Split(';');
                string id = commandArgs[0];
                string dateDebut = commandArgs[1];
                DateTime date = DateTime.Parse(dateDebut);

                using (var context = new ApplicationDbContext())
                {
                    var demande = context.DemandeRFJ.FirstOrDefault(d => d.Id == id);

                    if (demande != null)
                    {
                        if (HttpContext.Current.User.IsInRole("Admin"))
                        {
                            if (e.CommandName == "Accept")
                            {
                                if (selectedEtat == "En attente de validation RH")
                                {
                                    Button button2 = (Button)e.CommandSource; // Cast explicite de e.CommandSource en Button
                                    ButtonAccepter.CommandArgument = id + ";" + button2.ClientID;
                                    ButtonRejeter.CommandArgument = id + ";" + button2.ClientID; ;

                                    var periode = context.GetPeriodeFromDate(date);

                                    string[] parts = periode.Split('/');

                                    // Convertir la première partie (2024) en entier
                                    int annee = int.Parse(parts[0]);
                                    string userId = Context.User.Identity.GetUserId();
                                    var infoRFJId = context.GetInfoRFJByUserIdAndPeriode(userId, periode);


                                    if (infoRFJId != null)
                                    {
                                        Literal2.Text = "existe";
                                        nbRestant.Text = infoRFJId.NbRestant.ToString();
                                        nbRestantTxt.Text = "sans compter le nb de jours de cette demande (qui est de " + GetNombreJours(demande) + ")";
                                        nbAcquis.Text = infoRFJId.NbAcquis.ToString();
                                    }
                                    else
                                    {
                                        Literal2.Text = "existe pas";
                                        nbRestant.Text = BaseClass.NbJourReposForfaitAPoser(annee).ToString();
                                        nbRestantTxt.Text = "en excluant les jours de ponts, sans compter le nb de jour de cette demande(" + GetNombreJours(demande) + "jours)";
                                        nbAcquis.Text = BaseClass.NbJourReposForfaitTotal(annee).ToString();

                                    }
                                    nbAcquisTxt.Text = "en incluant les jours de ponts";

                                    Button button = (Button)e.CommandSource;
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "showPanel",
                                        $"showPanel(document.getElementById('{button.ClientID}'));", true);

                                }
                                else if (selectedEtat == "En attente de validation de Responsable")
                                {
                                    demande.Etat = "Validé";
                                }


                            }
                            else if (e.CommandName == "Reject")
                            {
                                if (selectedEtat == "En attente de validation de Responsable") {
                                    demande.Etat = "Refusé";
                                }
                                   
                            }
                        }
                        if (HttpContext.Current.User.IsInRole("Responsable-Phoenix"))
                        {
                            if (e.CommandName == "Accept")
                            {
                                demande.Etat = "Validé";
                            }
                            else if (e.CommandName == "Reject")
                            {
                                demande.Etat = "Refusé";
                            }
                        }


                        context.SaveChanges();
                        BindGridView();
                    }
                    else
                    {
                        // Gérer le cas où la demande n'est pas trouvée
                    }
                }
            }
        }

        protected void GridViewDemandes_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            string selectedEtat = ddlEtat.SelectedValue;
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Button btnAccept = (Button)e.Row.FindControl("ButtonAccept");
                Button btnReject = (Button)e.Row.FindControl("ButtonReject");

                if (HttpContext.Current.User.IsInRole("Admin"))
                {
                    if (selectedEtat == "En attente de validation RH")
                    {
                        btnAccept.Visible = true;
                        btnReject.Visible = false;
                    }
                   
                    if (selectedEtat == "Validé" || selectedEtat == "Refusé")
                    {
                        btnAccept.Visible = false;
                        btnReject.Visible = false;
                    }

                }
                else if (HttpContext.Current.User.IsInRole("Responsable-Phoenix"))
                {

                    if (selectedEtat == "Validé" || selectedEtat == "Refusé")
                    {
                        btnAccept.Visible = false;
                        btnReject.Visible = false;
                    }

                }

            }
        }

        public int GetNombreJours(DemandeRFJ demande)
        {

            return (demande.DateFin - demande.DateDebut).Days + 1;
        }

        protected void HandleCommand(object sender, CommandEventArgs e)
        {


            string[] args = e.CommandArgument.ToString().Split(';');
            string demandeId = args[0];  // ID de la demande
            string buttonClientId = args[1];  // ID du bouton

            using (var context = new ApplicationDbContext())
            {
                var demande = context.DemandeRFJ.FirstOrDefault(d => d.Id == demandeId);

                if (demande != null)
                {
                    if (e.CommandName == "Accepter")
                    {
                        Page.Validate();
                        if (Page.IsValid)
                        {
                            var periode = context.GetPeriodeFromDate(demande.DateDebut);

                            // Diviser la période pour obtenir l'année
                            string[] parts = periode.Split('/');
                            int annee = int.Parse(parts[0]); // Convertir la première partie en entier (année)

                            // Récupérer l'ID de l'utilisateur
                            string userId = Context.User.Identity.GetUserId();

                            // Vérifier si les informations RFJ existent pour l'utilisateur et la période
                            var infoRFJId = context.GetInfoRFJByUserIdAndPeriode(userId, periode);
                            int nbRestantValue;
                            int nbAcquisValue;
                            bool isNbRestantValid = int.TryParse(nbRestant.Text, out nbRestantValue);
                            bool isNbAcquisValid = int.TryParse(nbAcquis.Text, out nbAcquisValue);

                            if (infoRFJId != null)
                            {
                                if (infoRFJId.NbRestant != nbRestantValue)
                                {
                                    context.UpdateNbRestant(infoRFJId.Id, nbRestantValue);
                                }
                                else
                                {
                                    ValidationSummary1.Visible = true;
                                    Literal2.Text = "<span style='color: red;'>Erreur : La valeur du champ NbRestant n'a pas changé.</span>";

                                    string script = $"showPanel(document.getElementById('{buttonClientId}'));";
                                    ScriptManager.RegisterStartupScript(this, GetType(), "show", script, true);
                                    return;
                                }
                            }
                            else
                            {

                                context.CreateAndAddInfoRFJParPeriode(userId, periode, nbAcquisValue, nbRestantValue);

                            }


                            demande.Etat = "En attente de validation de Responsable";
                            ValidationSummary1.Visible = false;
                            context.SaveChanges();
                            // Optionally refresh GridView or perform other UI actions
                            Response.Redirect(Request.RawUrl);

                        }
                        else
                        {
                            //ErrorMessageLabel.Text = "Please correct the errors and try again.";
                            ValidationSummary1.Visible = true;
                            Literal2.Text = "<span style='color: red;'>Please correct the errors and try again</span>";

                            string script = $"showPanel(document.getElementById('{buttonClientId}'));";
                            ScriptManager.RegisterStartupScript(this, GetType(), "show", script, true);

                        }


                    }
                    else if (e.CommandName == "Refuser")
                    {
                        demande.Etat = "Refusé";

                        context.SaveChanges();
                        // Optionally refresh GridView or perform other UI actions
                        Response.Redirect(Request.RawUrl);
                    }

                }
                else
                {

                    Response.Write("Demand not found!");
                }
            }
        }


        private void PopulateDropDownList()
        {
            ddlEtat.Items.Clear();

            if (HttpContext.Current.User.IsInRole("Admin"))
            {
                // Ajouter les options spécifiques à l'admin
                ddlEtat.Items.Add(new ListItem("En attente de validation RH", "En attente de validation RH"));
                ddlEtat.SelectedValue = "En attente de validation RH";
            }
            else if (HttpContext.Current.User.IsInRole("Responsable-Phoenix"))
            {
                ddlEtat.SelectedValue = "En attente de validation de Responsable";
            }

            ddlEtat.Items.Add(new ListItem("En attente de validation de Responsable", "En attente de validation de Responsable"));
            ddlEtat.Items.Add(new ListItem("Validé", "Validé"));
            ddlEtat.Items.Add(new ListItem("Refusé", "Refusé"));
        }

        protected void ddlEtat_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Recharger le GridView en fonction de l'état sélectionné
            BindGridView();
        }

    }
}