using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

//pc add
using MvcPhoenix.Models;
using MvcPhoenix.EF;
//using MvcPhoenix.Services;

namespace MvcPhoenix.Controllers
{
    public class ProductNotesController : Controller
    {
        // Not used
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult LogNotesList(int id)
        {
            using (var db= new CMCSQL03Entities())
            {
                var obj = (from t in db.tblProductNotes
                         where t.ProductDetailID == id select  new ProductNote 
                         {productnoteid=t.ProductNoteID, productdetailid=t.ProductDetailID,notedate=t.NoteDate,notes=t.Notes,reasoncode=t.ReasonCode }).ToList();
                return PartialView("~/Views/Products/_LogNotes.cshtml", obj);
            }
        }

       

        
        public ActionResult Create(int id)
        {
            using (var db = new CMCSQL03Entities())
            {
            ProductNote PN = new ProductNote();
            PN.productnoteid = -1;
            PN.notedate = DateTime.Now;
            PN.ListOfReasonCodes = (from t in db.tblReasonCode orderby t.Reason select new SelectListItem { Value = t.Reason, Text = t.Reason }).ToList();
            PN.ListOfReasonCodes.Insert(0, new SelectListItem { Value = "", Text = "Select Reason Code" });
            return PartialView("~/Views/Products/_LogNotesModal.cshtml", PN);
            }
        }

        
        public ActionResult Edit(int id)
        {
            ProductNote PN = new ProductNote();
            using (var db = new CMCSQL03Entities())
            {
                var q = (from t in db.tblProductNotes where t.ProductNoteID == id select t).FirstOrDefault();
                PN.productnoteid = q.ProductNoteID;
                PN.productdetailid = q.ProductDetailID;
                PN.notedate = q.NoteDate;
                PN.notes = q.Notes;
                PN.reasoncode = q.ReasonCode;
                PN.ListOfReasonCodes = (from t in db.tblReasonCode orderby t.Reason select new SelectListItem { Value = t.Reason, Text = t.Reason }).ToList();
                PN.ListOfReasonCodes.Insert(0, new SelectListItem { Value = "", Text = "Select Reason Code" });
                return PartialView("~/Views/Products/_LogNotesModal.cshtml", PN);
            }

        }


        [HttpPost]
        public ActionResult Save(ProductNote PN)
        {
            int pk = PN.productnoteid;
            pk = fnSaveToDB(PN);
            TempData["SaveResult"] = "Data Updated at " + DateTime.Now;
            return RedirectToAction("Edit", new { id = pk });
            
        }
        
        
        private int fnSaveToDB(ProductNote PN)
        {
            using (var db = new CMCSQL03Entities())
            {
                if (PN.productnoteid == -1)
                {
                    var newrec = new EF.tblProductNotes();
                    db.tblProductNotes.Add(newrec);
                    db.SaveChanges();
                    PN.productnoteid = newrec.ProductNoteID;
                }
                var q = (from t in db.tblProductNotes where t.ProductNoteID == PN.productnoteid select t).FirstOrDefault();
                q.ProductDetailID = PN.productdetailid;
                q.NoteDate = PN.notedate;
                q.Notes = PN.notes;
                q.ReasonCode = PN.reasoncode;
                db.SaveChanges();
                return q.ProductNoteID;
             }


        }
              
        public ActionResult Delete(int id)
        {
            using (var db = new CMCSQL03Entities())
            {
                db.Database.ExecuteSqlCommand("Delete from tblProductNote Where ProductNoteID=" + id);
                return Content("Item Deleted");
                }
            }
        }

    
        


    }
