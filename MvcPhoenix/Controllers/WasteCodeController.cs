using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


//pc add
using MvcPhoenix.Models;
using MvcPhoenix.EF;

// This Controller can be deleted
// WasteCode is a text field in tblProductMaster and no aux table is nedded


namespace MvcPhoenix.Controllers
{

    public class WasteCode
    {
        public int wastecodeid { get; set; }
        public int? productdetailid { get; set; }
        public string wastecode { get; set; }
        public string profilenumber { get; set; }

    }

    public class WasteCodeController : Controller
    {
        // GET: WasteCode
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult New(int id)
        {
            // Setup new object for View
            WasteCode WC = new WasteCode();
            WC.wastecodeid = -1;
            return View("~/Views/Products/WasteCodeEdit.cshtml", WC);
        }



        public ActionResult Edit(int id)
        {
            WasteCode obj = new WasteCode();
            using (var db = new CMCSQL03Entities())
            {
                var q = (from t in db.tblWasteCode where t.WasteCodeID == id select t).FirstOrDefault();
                obj.wastecodeid = q.WasteCodeID;
                obj.productdetailid = q.ProductDetailID;
                obj.wastecode = q.WasteCode;
                obj.profilenumber = q.ProfileNumber;
                return Content("Return a Partial here?");
            }

        }


        [HttpPost]
        public ActionResult Save(WasteCode WC)
        {
            using (var db = new CMCSQL03Entities())
            {
                if (WC.wastecodeid == -1)
                {
                    var newrec = new EF.tblWasteCode();
                    db.tblWasteCode.Add(newrec);
                    db.SaveChanges();
                    WC.wastecodeid = newrec.WasteCodeID;
                }

                // for this to work ALL table fields need to be dragged thru the object
                var q = (from t in db.tblWasteCode where t.WasteCodeID == WC.wastecodeid select t).FirstOrDefault();
                db.Entry(q).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Content("Return a Partial here?");
            }
        }


        public ActionResult Delete(int id)
        {
            using (var db = new CMCSQL03Entities())
            {
                var q = (from t in db.tblWasteCode where t.WasteCodeID ==id select t).FirstOrDefault();
                if (q != null)
                {
                    db.Entry(q).State = System.Data.Entity.EntityState.Deleted;
                    //db.tblProductNotes.Remove(itemtodelete);
                    db.SaveChanges();

                }
                return Content("Return a Partial here?");
            }


        }




    }
}