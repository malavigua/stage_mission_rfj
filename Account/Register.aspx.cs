using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Owin;
using WebApplication2.Models;

namespace WebApplication2.Account
{
    public partial class Register : Page
    {
        protected void CreateUser_Click(object sender, EventArgs e)
        {
            var roleManager = Context.GetOwinContext().GetUserManager<ApplicationRoleManager>();
            var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var signInManager = Context.GetOwinContext().Get<ApplicationSignInManager>();
            var user = new ApplicationUser() { UserName = userName.Text, Email = Email.Text, Equipe=Equipe.Text };
            IdentityResult result = manager.Create(user, Password.Text);
            if (result.Succeeded)
            {
                string selectedRole = Role.SelectedValue;

                if (!roleManager.RoleExists(selectedRole))
                {
                    var roleResult = roleManager.Create(new IdentityRole(selectedRole));
                    if (!roleResult.Succeeded) {
                        ErrorMessage.Text = "Erreur lors de la création du rôle.";
                        return;
                    }
                }
                manager.AddToRole(user.Id, selectedRole);

                signInManager.SignIn( user, isPersistent: false, rememberBrowser: false);
                IdentityHelper.RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);
            }
            else 
            {
                ErrorMessage.Text = result.Errors.FirstOrDefault();
            }
        }
    }
}