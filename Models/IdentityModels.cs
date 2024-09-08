using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using WebApplication2.Models;

namespace WebApplication2.Models
{
    // Vous pouvez ajouter des données d'utilisateur pour l'utilisateur en ajoutant d'autres propriétés à votre classe d'utilisateur. Pour en savoir plus, visitez https://go.microsoft.com/fwlink/?LinkID=317594.
    public class ApplicationUser : IdentityUser
    {

        public String Equipe {  get; set; }
        public ClaimsIdentity GenerateUserIdentity(ApplicationUserManager manager)
        {
            // Notez que authenticationType doit correspondre à l'instance définie dans CookieAuthenticationOptions.AuthenticationType
            var userIdentity = manager.CreateIdentity(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Ajouter des revendications d’utilisateur personnalisées ici
            return userIdentity;
        }

        public Task<ClaimsIdentity> GenerateUserIdentityAsync(ApplicationUserManager manager)
        {
            return Task.FromResult(GenerateUserIdentity(manager));
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        
        

        public DbSet<DemandeRFJ> DemandeRFJ { get; set; }

        public DbSet<InfoRFJParPeriode> InfoRFJParPeriode { get; set; }
        public DemandeRFJ CreateDemandeRFJ(string userId, DateTime dateDebut, DateTime dateFin, string note)
        {
            return new DemandeRFJ
            {
                Id = Guid.NewGuid().ToString(),
                IdUtilisateur = userId,
                DateCreation = DateTime.Now,
                DateDebut = dateDebut,
                DateFin = dateFin,
                Etat = "en attente de validation RH",
                NoteFacultative = note
            };
        }

        public void AddDemandeRFJ(DemandeRFJ demande)
        {
            DemandeRFJ.Add(demande);
            SaveChanges();
        }

        public void CreateAndAddDemandeRFJ(string userId, DateTime dateDebut, DateTime dateFin, string etat, string note)
        {
            var demande = CreateDemandeRFJ(userId, dateDebut, dateFin, note);
            AddDemandeRFJ(demande);
        }

        public List<DemandeRFJ> GetDemandeRFJByUserId(string userId)
        {
            return DemandeRFJ.Where(d => d.IdUtilisateur == userId).ToList();
        }

        public List<DemandeRFJ> GetDemandeRFJByStatus(string etat)
        {
            return DemandeRFJ.Where(d => d.Etat == etat).ToList();
        }



        public List<DemandeRFJ> GetDemandeRFJByStatusAndUserId(string etat, string userId)
        {
            return DemandeRFJ.Where(d => d.Etat == etat && d.IdUtilisateur == userId).ToList();
        }


        public List<DemandeRFJ> GetDemandeRFJByMonthYearAndUserIdAndSatus(int mois, int annee, string userId, string etat)
        {
            return DemandeRFJ.Where(d =>
                d.IdUtilisateur == userId && d.Etat == etat &&
                (
                    (d.DateDebut.Month == mois && d.DateDebut.Year == annee) ||
                    (d.DateFin.Month == mois && d.DateFin.Year == annee)
                )
            ).ToList();
        }

        public int GetNombreJours(DemandeRFJ demande)
        {

            return (demande.DateFin - demande.DateDebut).Days + 1;
        }

        public List<DateTime> GetDatesForMonthAndYearFromDemandes(List<DemandeRFJ> demandes, int mois, int annee)
        {
            List<DateTime> dates = new List<DateTime>();

            DateTime startOfMonth = new DateTime(annee, mois, 1);
            DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            foreach (var demande in demandes)
            {
                DateTime start = demande.DateDebut > startOfMonth ? demande.DateDebut : startOfMonth;
                DateTime end = demande.DateFin < endOfMonth ? demande.DateFin : endOfMonth;

                for (DateTime date = start; date <= end; date = date.AddDays(1))
                {
                    if (!dates.Contains(date)) // Vérifier si la date est déjà dans la liste
                    {
                        dates.Add(date); // Ajouter la date si elle n'est pas déjà présente
                    }
                }
            }

            return dates;
        }

        public List<DemandeRFJ> GetDemandeRFJByTeamName(string teamName)
        {
            using (var context = new ApplicationDbContext()) // Un seul contexte qui gère les deux entités
            {
                var demandes = from demande in DemandeRFJ
                               join user in context.Users on demande.IdUtilisateur equals user.Id
                               where user.Equipe == teamName
                               select demande;

                return demandes.ToList();
            }
        }


        public List<DemandeRFJ> GetDemandeRFJByTeamNameAndStatus(string teamName, string etat)
        {
            using (var context = new ApplicationDbContext()) // Un seul contexte qui gère les deux entités
            {
                var demandes = from demande in context.DemandeRFJ
                               join user in context.Users on demande.IdUtilisateur equals user.Id
                               where user.Equipe == teamName && demande.Etat == etat
                               select demande;

                return demandes.ToList();
            }
        }

        public InfoRFJParPeriode CreateInfoRFJParPeriode(string userId, string periode, int nbAcquis, int nbRestant)
        {
            return new InfoRFJParPeriode
            {
                Id = Guid.NewGuid().ToString(),
                IdUtilisateur = userId,
                Periode = periode,
                NbAcquis = nbAcquis,
                NbRestant = nbRestant

            };
        }

        public void AddInfoRFJParPeriode(InfoRFJParPeriode infoDemande)
        {
            InfoRFJParPeriode.Add(infoDemande);
            SaveChanges();
        }

        public void CreateAndAddInfoRFJParPeriode(string userId, string periode, int nbAcquis, int nbRestant)
        {
            var infoDemande = CreateInfoRFJParPeriode(userId, periode, nbAcquis, nbRestant);
            AddInfoRFJParPeriode(infoDemande);
        }

        public void UpdateNbRestant(string id, int newNbRestant)
        {
            using (var context = new ApplicationDbContext())
            {
                // Récupérer l'enregistrement correspondant à l'Id spécifié
                var info = context.InfoRFJParPeriode.FirstOrDefault(i => i.Id == id);

                if (info != null)
                {
                    // Mettre à jour le champ NbRestant
                    info.NbRestant = newNbRestant;

                    // Sauvegarder les modifications dans la base de données
                    context.SaveChanges();
                }
                else
                {
                    // Gérer le cas où l'enregistrement n'est pas trouvé
                    throw new Exception("L'enregistrement avec l'Id spécifié n'a pas été trouvé.");
                }
            }
        }

        public InfoRFJParPeriode GetInfoRFJByUserIdAndPeriode(string idUtilisateur, string periode)
        {
            using (var context = new ApplicationDbContext())
            {
                // Rechercher l'enregistrement correspondant à l'IdUtilisateur et à la Période
                var info = context.InfoRFJParPeriode
                    .FirstOrDefault(i => i.IdUtilisateur == idUtilisateur && i.Periode == periode);

                // Retourner l'objet InfoRFJParPeriode si trouvé, sinon retourner null
                return info;
            }
        }

        public string GetPeriodeFromDate(DateTime date)
        {
            int startYear;
            int endYear;

            // Vérifier si la date est avant le 1er juin de l'année en cours
            if (date.Month < 6)
            {
                // Si oui, la période commence en juin de l'année précédente
                startYear = date.Year - 1;
                endYear = date.Year;
            }
            else
            {
                // Sinon, la période commence en juin de l'année en cours
                startYear = date.Year;
                endYear = date.Year + 1;
            }

            // Retourner la période au format "année/année+1"
            return $"{startYear}/{endYear}";
        }


    }
}

#region Programmes d'assistance
namespace WebApplication2
{
    public static class IdentityHelper
    {
        // Utilisés pour XSRF lors de la liaison de logins externes
        public const string XsrfKey = "XsrfId";

        public const string ProviderNameKey = "providerName";
        public static string GetProviderNameFromRequest(HttpRequest request)
        {
            return request.QueryString[ProviderNameKey];
        }

        public const string CodeKey = "code";
        public static string GetCodeFromRequest(HttpRequest request)
        {
            return request.QueryString[CodeKey];
        }

        public const string UserIdKey = "userId";
        public static string GetUserIdFromRequest(HttpRequest request)
        {
            return HttpUtility.UrlDecode(request.QueryString[UserIdKey]);
        }

        public static string GetResetPasswordRedirectUrl(string code, HttpRequest request)
        {
            var absoluteUri = "/Account/ResetPassword?" + CodeKey + "=" + HttpUtility.UrlEncode(code);
            return new Uri(request.Url, absoluteUri).AbsoluteUri.ToString();
        }

        public static string GetUserConfirmationRedirectUrl(string code, string userId, HttpRequest request)
        {
            var absoluteUri = "/Account/Confirm?" + CodeKey + "=" + HttpUtility.UrlEncode(code) + "&" + UserIdKey + "=" + HttpUtility.UrlEncode(userId);
            return new Uri(request.Url, absoluteUri).AbsoluteUri.ToString();
        }

        private static bool IsLocalUrl(string url)
        {
            return !string.IsNullOrEmpty(url) && ((url[0] == '/' && (url.Length == 1 || (url[1] != '/' && url[1] != '\\'))) || (url.Length > 1 && url[0] == '~' && url[1] == '/'));
        }

        public static void RedirectToReturnUrl(string returnUrl, HttpResponse response)
        {
            if (!String.IsNullOrEmpty(returnUrl) && IsLocalUrl(returnUrl))
            {
                response.Redirect(returnUrl);
            }
            else
            {
                response.Redirect("~/");
            }
        }
    }
}
#endregion
