using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApplication2.Models;

namespace WebApplication2
    {
        public partial class NestedMasterPage1 : System.Web.UI.MasterPage
        {
            protected void Page_Load(object sender, EventArgs e)
            {
                LiteralValidatedDays.Text = BindLeaveData3("Validé", false);
                LiteralRefusedDays.Text = BindLeaveData3("Refusé", false);
                LiteralPendingDaysRH.Text = BindLeaveData3("En attente de validation RH", false);
                LiteralPendingDaysResp.Text = BindLeaveData3("En attente de validation de Responsable", false);
                Panel1.Visible = false;

                if (HttpContext.Current.User.IsInRole("Admin") || HttpContext.Current.User.IsInRole("Responsable-Phoenix"))
                {
                    Page2Link.Visible = true;
                }
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
                    PanelBilan.Visible = true;
                }
                else
                {
                    PanelBilan.Visible = false;
                }
            }
        }



        private string BindLeaveData3(string status, bool showAllPeriods)
            {
                string userName = Context.User.Identity.GetUserName();
                var userManager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
                string userId = userManager.GetUserIdByUsername(userName);

                var sb = new System.Text.StringBuilder();

                if (userId != null)
                {
                    using (var context = new ApplicationDbContext())
                    {
                        var query = context.DemandeRFJ
                            .Where(l => l.IdUtilisateur == userId && l.Etat == status);

                        // Filter for the current period if not showing all periods
                        if (!showAllPeriods)
                        {
                            int currentYear = DateTime.Now.Month >= 6 ? DateTime.Now.Year : DateTime.Now.Year - 1;
                            query = query.Where(l =>
                                (l.DateDebut.Year == currentYear && l.DateDebut.Month >= 6) ||
                                (l.DateFin.Year == currentYear + 1 && l.DateFin.Month <= 5));
                        }

                        var leaveDaysData = query
                            .AsEnumerable()
                            .GroupBy(l => l.DateDebut.Month >= 6 ? l.DateDebut.Year : l.DateDebut.Year - 1)
                            .Select(g => new
                            {
                                PeriodRange = $"{new DateTime(g.Key, 6, 1):dd MMM yyyy} - {new DateTime(g.Key + 1, 5, 31):dd MMM yyyy}",
                                TotalDays = g.Sum(l => (l.DateFin - l.DateDebut).Days + 1)
                            })
                            .OrderBy(result => result.PeriodRange)
                            .ToList();

                        if (leaveDaysData.Count > 0)
                        {
                            if (!showAllPeriods)
                            {
                                // If only showing the current period, display the total days for that period
                                var currentPeriod = leaveDaysData.FirstOrDefault();
                                if (currentPeriod != null)
                                {
                                    sb.AppendLine($"<div>{currentPeriod.TotalDays}</div>");
                                }
                            }
                            else
                            {
                                // If showing all periods, display each period with the total days
                                foreach (var data in leaveDaysData)
                                {
                                    sb.AppendLine($"<div><strong>{data.PeriodRange} : </strong>{data.TotalDays}</div>");
                                    sb.AppendLine("<hr/>");
                                }
                            }
                        }
                        else
                        {
                            var message = showAllPeriods
                                ? "Aucun congé trouvé pour cet utilisateur avec le statut spécifié."
                                : "Aucun congé validé trouvé pour la période en cours.";

                            sb.AppendLine(message);
                        }
                    }
                }
               

                // Return the constructed HTML string
                return sb.ToString();
            }



            protected void ButtonValidationBox_Click(object sender, CommandEventArgs e)
            {
                // Appel de la méthode BindLeaveData2

                string htmlContent = BindLeaveData3(e.CommandName, true);

                // Mise à jour du contrôle Literal1

                Literal2.Text = htmlContent;
                Panel1.Visible = true;

            }
        }
    } 
