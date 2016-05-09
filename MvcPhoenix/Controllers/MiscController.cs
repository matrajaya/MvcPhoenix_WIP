using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcPhoenix.Controllers

// This controller does misc tasks
// Put here to abstract out of normal controllers
// Code blocks can be moved to a more appropriate controller


{
    public class MiscController : Controller
    {

    //  public ActionResult SetSizes(int myprofileid)
    //{
    //    MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
    //    List<SelectListItem> mylist = new List<SelectListItem>();
    //    mylist = (from t in db.tblSampSize
    //                  where t.ProfileID == myprofileid
    //                  orderby t.ProductCode
    //                  select new SelectListItem { Value = t.Size, Text = t.Size }).ToList();
    //        mylist.Insert(0, new SelectListItem { Value = "0", Text = "Please Select" });
    //        db.Dispose();
    //        ViewData["DDSize"] = mylist;
    //        return View("~Views/Orders/OrderItemGrid03.cshtml");
    //  }



    }
}
