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
using Microsoft.Owin;
using Microsoft.Owin.Security;
using MvcPhoenix.Models;
using MvcPhoenix.DataLayer;
using System.Net.Mail;
using System.Net;

namespace MvcPhoenix
{
    public class EmailService : IIdentityMessageService
    {
        //public Task SendAsync(IdentityMessage message)
        public async Task SendAsync(IdentityMessage message)
        {
            // Using SendGrid email service
            const string userName = "ifeanyiigbo";
            const string from = "ifeanyi.igbo@gmail.com";
            const string password = "panicbutton1";
            const int port = 587;

            var smtpClient = new SmtpClient("smtp.sendgrid.net", port);
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new NetworkCredential(userName, password);

            var mailMessage = new MailMessage(from, message.Destination);
            mailMessage.Subject = message.Subject;
            mailMessage.Body = message.Body;

            //return smtpClient.SendMailAsync(mailMessage);
            await smtpClient.SendMailAsync(mailMessage);
        }

        //public async Task SendAsync(IdentityMessage message)
        //{
        //    // Credentials:
        //    string smtpServer = "secure.emailsrvr.com";
        //    int smtpPort = 465;
        //    bool enableSsl = true;
        //    string smtpUsername = ConfigurationManager.AppSettings["EmailSmtpUsername"];
        //    string smtpPassword = ConfigurationManager.AppSettings["EmailSmtpPassword"];
        //    string sentFrom = ConfigurationManager.AppSettings["EmailSentFrom"];

        //    // Configure the client:
        //    var client = new SmtpClient(smtpServer, Convert.ToInt32(587));

        //    client.Port = smtpPort;
        //    client.DeliveryMethod = SmtpDeliveryMethod.Network;
        //    client.UseDefaultCredentials = false;
        //    client.EnableSsl = enableSsl;

        //    // Create the credentials:
        //    var credentials = new NetworkCredential(smtpUsername, smtpPassword);
        //    client.Credentials = credentials;

        //    // Create the message:
        //    var mail = new System.Net.Mail.MailMessage(sentFrom, message.Destination);

        //    mail.Subject = message.Subject;
        //    mail.Body = message.Body;

        //    // Send:
        //    await client.SendMailAsync(mail);
        //}
    }

    //public class SmsService : IIdentityMessageService
    //{
    //    public Task SendAsync(IdentityMessage message)
    //    {
    //        // Plug in your SMS service here to send a text message.
    //        return Task.FromResult(0);
    //    }
    //}

    // Configure the application user manager used in this application. 
    // UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context) 
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. 
            // We can use two-factor authentication using Phone and Emails for user verification but Email will be fine for now.
            //manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
            //{
            //    MessageFormat = "Your security code is {0}"
            //});

            //manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
            //{
            //    Subject = "Security Code",
            //    BodyFormat = "Your security code is {0}"
            //});
            
            //manager.SmsService = new SmsService();

            manager.EmailService = new EmailService();
            
            var dataProtectionProvider = options.DataProtectionProvider;

            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = 
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }

            return manager;
        }
    }

    // Configuration for the application sign-in manager
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }

    // Configuration for the the RoleManager. 
    // Rolemanger is defined in the ASP.NET identity Core library.
    public class ApplicationRoleManager : RoleManager<ApplicationRole>
    {
        public ApplicationRoleManager(IRoleStore<ApplicationRole, string> roleStore)
            : base(roleStore)
        {

        }

        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            return new ApplicationRoleManager(new RoleStore<ApplicationRole>(context.Get<ApplicationDbContext>()));
        }
    }
}
