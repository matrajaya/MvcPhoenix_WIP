using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using MvcPhoenix.Models;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MvcPhoenix.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Manage/ChangeProfile
        public async Task<ActionResult> ChangeProfile(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Password changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Password set."
                : message == ManageMessageId.SetTwoFactorSuccess ? "Two-factor authentication provider set."
                : message == ManageMessageId.Error ? "Error occurred."
                : message == ManageMessageId.AddPhoneSuccess ? "Phone number added."
                : message == ManageMessageId.RemovePhoneSuccess ? "Phone number removed."
                : message == ManageMessageId.ChangeProfileSuccess ? "Profile updated."
                : message == ManageMessageId.ChangeEmailSuccess ? "Check email."
                : "";

            ApplicationUser currentUser = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            return View(currentUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeProfile(ApplicationUser applicationUser)
        {
            ManageMessageId manageMessageId;

            ApplicationUser retrieveApplicationUser = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            retrieveApplicationUser.FirstName = applicationUser.FirstName;
            retrieveApplicationUser.LastName = applicationUser.LastName;
            retrieveApplicationUser.PhoneNumber = applicationUser.PhoneNumber;
            retrieveApplicationUser.FaxNumber = applicationUser.FaxNumber;
            retrieveApplicationUser.Address = applicationUser.Address;
            retrieveApplicationUser.City = applicationUser.City;
            retrieveApplicationUser.State = applicationUser.State;
            retrieveApplicationUser.PostalCode = applicationUser.PostalCode;
            retrieveApplicationUser.Country = applicationUser.Country;

            // Update the Profile
            var result = await UserManager.UpdateAsync(retrieveApplicationUser);
            if (result.Succeeded)
            {
                manageMessageId = ManageMessageId.ChangeProfileSuccess;

                // If the Email address changed, sync both Email and UserName to it
                if (retrieveApplicationUser.Email != applicationUser.Email)
                {
                    //Update the UserName
                    string previousUserName = retrieveApplicationUser.UserName;
                    retrieveApplicationUser.UserName = applicationUser.Email;
                    result = await UserManager.UpdateAsync(retrieveApplicationUser);
                    if (result.Succeeded)
                    {
                        // Update the Email address
                        result = await UserManager.SetEmailAsync(retrieveApplicationUser.Id, applicationUser.Email);
                        if (result.Succeeded)
                        {
                            manageMessageId = ManageMessageId.ChangeEmailSuccess;
                            string code = await UserManager.GenerateEmailConfirmationTokenAsync(retrieveApplicationUser.Id);

                            var callbackUrl = Url.Action(
                                "ConfirmEmail",
                                "Account",
                                new { userId = retrieveApplicationUser.Id, code = code }, protocol: Request.Url.Scheme);

                            await UserManager.SendEmailAsync(
                                retrieveApplicationUser.Id,
                                "Confirm your account",
                                "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                        }
                        else
                        {
                            retrieveApplicationUser.UserName = previousUserName;
                            result = await UserManager.UpdateAsync(retrieveApplicationUser);
                            manageMessageId = ManageMessageId.Error;
                        }
                    }
                    else
                    {
                        retrieveApplicationUser.UserName = previousUserName;
                        result = await UserManager.UpdateAsync(retrieveApplicationUser);
                        manageMessageId = ManageMessageId.Error;
                    }
                }
            }
            else
            {
                manageMessageId = ManageMessageId.Error;
            }

            return RedirectToAction("ChangeProfile", new { Message = manageMessageId });
        }

        //
        // GET: /Manage/ChangePassword
        [ChildActionOnly]
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);

            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("ChangeProfile", new { Message = ManageMessageId.ChangePasswordSuccess });
            }

            AddErrors(result);

            return View(model);
        }

        //
        // GET: /Manage/SetPassword
        public ActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        #region Helpers

        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error,
            ChangeProfileSuccess,
            ChangeEmailSuccess
        }

        #endregion Helpers
    }
}