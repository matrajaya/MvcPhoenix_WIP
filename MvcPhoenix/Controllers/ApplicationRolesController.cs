using Microsoft.AspNet.Identity.Owin;
using MvcPhoenix.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace MvcPhoenix.Controllers
{
    public class ApplicationRolesController : Controller
    {
       public ApplicationRolesController()
        {
        }

        public ApplicationRolesController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            set
            {
                _userManager = value;
            }
        }

        private ApplicationRoleManager _roleManager;

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }


        // GET: ApplicationRoles
        //public async Task<ActionResult> Index(ManageMessageId? message)
        public async Task<ActionResult> Index(ManageMessageId? message, string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.StatusMessage = message == 
                ManageMessageId.AdminDeleteError ? "Admin role cannot be deleted." 
                : "";

            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var roles = from r in RoleManager.Roles
                        select r;

            if (!String.IsNullOrEmpty(searchString))
            {
                roles = roles.Where(u => u.Name.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    roles = roles.OrderByDescending(u => u.Name);
                    break;

                default:
                    roles = roles.OrderBy(u => u.Name);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(roles.ToPagedList(pageNumber, pageSize));

            //return View(await RoleManager.Roles.ToListAsync());
        }

        // GET: ApplicationRoles/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ApplicationRoles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Name")] ApplicationRoleViewModel applicationRoleViewModel)
                
        {
            if (ModelState.IsValid)
            {
                ApplicationRole applicationRole = new ApplicationRole{Name = applicationRoleViewModel.Name};

                var roleResult = await RoleManager.CreateAsync(applicationRole);
                if (!roleResult.Succeeded)
                {
                    ModelState.AddModelError("", roleResult.Errors.First());
                    return RedirectToAction("Index");

                }
                
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
            //return View();
        }

        // GET: ApplicationRoles/Edit/5
        //public async Task<ActionResult> Edit(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    ApplicationRole applicationRole = await RoleManager.FindByIdAsync(id);
        //    if (applicationRole == null)
        //    {
        //        return HttpNotFound();
        //    }

        //    ApplicationRoleViewModel applicationRoleViewModel = new ApplicationRoleViewModel { Id = applicationRole.Id, Name = applicationRole.Name };

        //    return View();
        //}

        // POST: ApplicationRoles/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Edit([Bind(Include = "Id,Name")] ApplicationRole applicationRoleViewModel)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        ApplicationRole applicationRole = await RoleManager.FindByIdAsync(applicationRoleViewModel.Id);
        //        string originalName = applicationRole.Name;

        //        if (originalName == "Admin" && applicationRoleViewModel.Name != "Admin")
        //        {
        //            ModelState.AddModelError("", "You cannot change the name of the Admin role.");
        //            return View(applicationRoleViewModel);
        //        }

        //        if (originalName != "Admin" && applicationRoleViewModel.Name == "Admin")
        //        {
        //            ModelState.AddModelError("", "You cannot change the name of a role to Admin.");
        //            return View(applicationRoleViewModel);
        //        }

        //        applicationRole.Name = applicationRoleViewModel.Name;
        //        await RoleManager.UpdateAsync(applicationRole);

        //        return RedirectToAction("Index");
        //    }
        //    return View(applicationRoleViewModel);
        //}

        // GET: ApplicationRoles/Delete/5
        //public async Task<ActionResult> Delete(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    ApplicationRole applicationRole = await RoleManager.FindByIdAsync(id);
        //    if (applicationRole == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(applicationRole);
        //}

        // POST: ApplicationRoles/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> DeleteConfirmed(string id)
        public async Task<ActionResult> Delete(string id)
        {
            ApplicationRole applicationRole = await RoleManager.FindByIdAsync(id);

            if (applicationRole.Name == "Admin")
            {
                //ModelState.AddModelError("", "Admin role cannot be deleted.");

                return RedirectToAction("Index", new { Message = ManageMessageId.AdminDeleteError });
                //return View(applicationRole);
            }

            await RoleManager.DeleteAsync(applicationRole);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                RoleManager.Dispose();
            }
            base.Dispose(disposing);
        }

        public enum ManageMessageId
        {
            AdminDeleteError
        }
    }
}
