
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication2
{
    public class BaseClass
    {

        public virtual void Calendar_DayRender(object sender, DayRenderEventArgs e)
        {
            DateTime day = e.Day.Date;
            DateTime currentDate = DateTime.Today;

            DateTime dimancheDePaques = CalculerDatePaques(day.Year);
            DateTime ascension = dimancheDePaques.AddDays(39);
            int year = day.Month >= 6 ? day.Year : day.Year - 1;



            if (e.Day.IsWeekend == true)
            {
                e.Cell.BackColor = System.Drawing.Color.LightGray;
                e.Cell.ToolTip = "week end";
                e.Day.IsSelectable = false;
                e.Cell.Attributes.Add("class", "nonAccessible");
            }
            else if (IsJourFerie(day))
            {
                e.Cell.BackColor = System.Drawing.Color.Pink;
                e.Cell.ToolTip = "jour férié";
                e.Day.IsSelectable = false;
                e.Cell.Attributes.Add("class", "nonAccessible");
            }
            else if ((day.DayOfWeek == DayOfWeek.Friday && IsJourFerie(day.AddDays(-1))) ||
                     (day.DayOfWeek == DayOfWeek.Monday && IsJourFerie(day.AddDays(1))))
            {
                e.Cell.BackColor = System.Drawing.Color.LightGreen;
                e.Cell.ToolTip = "jour de pont";
                e.Day.IsSelectable = false;
                e.Cell.Attributes.Add("class", "nonAccessible");
            }
            else if (day == currentDate)
            {
                e.Cell.BackColor = System.Drawing.Color.SteelBlue;
            }

        }


        public static DateTime CalculerDatePaques(int annee)
        {
            int a = annee % 19;
            int b = annee / 100;
            int c = annee % 100;
            int d = b / 4;
            int e = b % 4;
            int f = (b + 8) / 25;
            int g = (b - f + 1) / 3;
            int h = (19 * a + b - d - g + 15) % 30;
            int i = c / 4;
            int k = c % 4;
            int l = (32 + 2 * e + 2 * i - h - k) % 7;
            int m = (a + 11 * h + 22 * l) / 451;
            int mois = (h + l - 7 * m + 114) / 31;
            int jour = ((h + l - 7 * m + 114) % 31) + 1;
            return new DateTime(annee, mois, jour);
        }

        public static int CalculerNombreJoursWeekend(int annee)
        {
            DateTime dateDebut = new DateTime(annee, 6, 1);
            DateTime dateFin = new DateTime(annee + 1, 5, 31);

            int nombreWeekends = 0;

            for (DateTime date = dateDebut; date <= dateFin; date = date.AddDays(1))
            {
                if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                {
                    nombreWeekends++;
                }
            }

            return nombreWeekends;

        }

        public static bool EstBissextile(int annee)
        {
            return (annee % 4 == 0 && annee % 100 != 0) || (annee % 400 == 0);
        }

        public static int NombreDeJoursDansAnnee(int annee)
        {
            return EstBissextile(annee) ? 366 : 365;
        }

        public static bool IsJourFerie(DateTime date)
        {
            int year = date.Year;
            DateTime dimancheDePaques = CalculerDatePaques(year);
            DateTime ascension = dimancheDePaques.AddDays(39);

            return (date.Day == 1 && date.Month == 1) // jour de l'an
                   || date == dimancheDePaques.AddDays(1)  // lundi de pâques
                   || (date.Day == 1 && date.Month == 5) // fête du travail
                   || (date.Day == 8 && date.Month == 5) // fête de la Victoire de 1945
                   || date == ascension // jeudi de l'ascension
                   || date == ascension.AddDays(11) // lundi de pentecôte
                   || (date.Day == 14 && date.Month == 7) // fête nationale
                   || (date.Day == 15 && date.Month == 8) // Assomption
                   || (date.Day == 1 && date.Month == 11) // Toussaint
                   || (date.Day == 11 && date.Month == 11) // Armistice
                   || (date.Day == 25 && date.Month == 12); // Noël
        }
        public static int CalculerNombreJoursFeriesNonWeekend(int annee)
        {
            DateTime dateDebut = new DateTime(annee, 6, 1);
            DateTime dateFin = new DateTime(annee + 1, 5, 31);

            int nombreJoursFeriesNonWeekend = 0;

            for (DateTime date = dateDebut; date <= dateFin; date = date.AddDays(1))
            {
                if (IsJourFerie(date) && !(date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday))
                {
                    nombreJoursFeriesNonWeekend++;
                }
            }

            return nombreJoursFeriesNonWeekend;
        }

        public static int CalculerNombreJoursDePont(int annee)
        {
            DateTime dateDebut = new DateTime(annee, 6, 1);
            DateTime dateFin = new DateTime(annee + 1, 5, 31);

            int nombreJoursDePont = 0;

            for (DateTime date = dateDebut; date <= dateFin; date = date.AddDays(1))
            {
                if (IsJourFerie(date) && (date.DayOfWeek == DayOfWeek.Thursday || date.DayOfWeek == DayOfWeek.Tuesday))
                {
                    // Incrémenter le compteur de jours de pont pour le vendredi suivant un jeudi férié
                    if (date.DayOfWeek == DayOfWeek.Thursday)
                    {
                        nombreJoursDePont++; // on devrait l'ajouter dans la table des jours repos forfaits
                    }
                    // Incrémenter le compteur de jours de pont pour le lundi suivant un mardi férié
                    if (date.DayOfWeek == DayOfWeek.Tuesday)
                    {
                        nombreJoursDePont++;  // on devrait l'ajouter dans la table des jours repos forfaits
                    }
                }
            }

            return nombreJoursDePont;
        }


        //pour 1er juin annee au 31 mai annee+1
        public static int NbJourReposForfaitTotal(int annee)
        {
            int nbJoursCongesPayes = 25;
            int nbForfaitJourATravailler = 218;
            return NombreDeJoursDansAnnee(annee + 1) - CalculerNombreJoursFeriesNonWeekend(annee) - CalculerNombreJoursWeekend(annee) - nbJoursCongesPayes - nbForfaitJourATravailler;
        }

        //pour 1er juin annee au 31 mai annee+1
        public static int NbJourReposForfaitAPoser(int annee)
        {
            return NbJourReposForfaitTotal(annee) - CalculerNombreJoursDePont(annee);
        }
    }


}
