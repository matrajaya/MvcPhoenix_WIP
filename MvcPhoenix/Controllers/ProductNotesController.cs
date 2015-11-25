using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

//pc add
using MvcPhoenix.Models;
using MvcPhoenix.EF;

namespace MvcPhoenix.Controllers
{
    public class ProductNotesController : Controller
    {
        // GET: ProductNotes
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult New(int id)
        {
            // Setup new object for View
            ProductNote PN = new ProductNote();
            PN.productnoteid = -1;
            PN.notedate = DateTime.Now;
            return View("~/Views/Products/ProductNoteEdit.cshtml", PN);
        }

        public ActionResult Edit(int id)
        {

            ProductNote obj = new ProductNote();
            using (var db = new CMCSQL03Entities())
            {
                var q = (from t in db.tblProductNotes where t.ProductNoteID == id select t).FirstOrDefault();
                obj.productnoteid = q.ProductNoteID;
                obj.productdetailid = q.ProductDetailID;
                obj.notedate = q.NoteDate;
                obj.notes = q.Notes;
                obj.reasoncode = q.ReasonCode;
                return Content("Return a Partial here?");
            }

            
        }


        [HttpPost]
        public ActionResult Save(ProductNote PN)
        {
            using (var db = new CMCSQL03Entities())
            {

                if (PN.productnoteid==-1)
                {
                    var newrec = new EF.tblProductNotes();
                    db.tblProductNotes.Add(newrec);
                    db.SaveChanges();
                    PN.productnoteid=newrec.ProductNoteID;
                }
                // for this to work ALL table fields need to be dragged thru the object
                var q = (from t in db.tblProductNotes where t.ProductNoteID ==PN.productnoteid select t).FirstOrDefault();
                db.Entry(q).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Content("Return a Partial here?");
            }
            
        }


        public ActionResult Delete(int id)
        {
            using (var db = new CMCSQL03Entities())
            {
                var q = (from t in db.tblProductNotes where t.ProductNoteID == id select t).FirstOrDefault();
                if (q != null)
                {
                    db.Entry(q).State = System.Data.Entity.EntityState.Deleted;
                    //db.tblProductNotes.Remove(itemtodelete);
                    db.SaveChanges();
                    
                }
                return Content("Return a Partial here or string?");
            }
        }

    
    
    
    }
}