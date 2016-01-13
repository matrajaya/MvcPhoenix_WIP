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
    public class CasController : Controller
    {

        //public ActionResult Index()
        //{
        //    return View();
        //}


        [HttpGet]
        public ActionResult CasList(int id)
        {
            using (var db = new CMCSQL03Entities())
            {
                var obj = (from t in db.tblCAS
                           where t.ProductDetailID == id
                           select new Cas
                           {
                               casid = t.CASID,
                               productdetailid = t.ProductDetailID,
                               casnumber = t.CasNumber,
                               chemicalname = t.ChemicalName,
                               percentage = t.Percentage,
                               restrictedqty = t.RestrictedQty,
                               restrictedamount = t.RestrictedAmount,
                               packonreceipt = t.PackOnReceipt,
                               reportableqty = t.ReportableQty,
                               reportableamount = t.ReportableAmount,
                               lessthan = t.LessThan,
                               excludefromlabel = t.ExcludeFromLabel
                           }).ToList();
                return PartialView("~/Views/Products/_Cas.cshtml", obj);
            }
        }



        [HttpGet]
        public ActionResult Create(int id)
        {
            Cas CS = new Cas();
            CS.casid = -1;
            return PartialView("~/Views/Products/_Cas.cshtml", CS);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {

            Cas CS = new Cas();
            using (var db = new CMCSQL03Entities())
            {
                var q = (from t in db.tblCAS where t.CASID == id select t).FirstOrDefault();
                CS.casid = q.CASID;
                CS.productdetailid = q.ProductDetailID;
                CS.casnumber = q.CasNumber;
                CS.chemicalname = q.ChemicalName;
                CS.percentage = q.Percentage;
                CS.restrictedqty = q.RestrictedQty;
                CS.restrictedamount = q.RestrictedAmount;
                CS.packonreceipt = q.PackOnReceipt;
                CS.reportableqty = q.ReportableQty;
                CS.reportableamount = q.ReportableAmount;
                CS.lessthan = q.LessThan;
                CS.excludefromlabel = q.ExcludeFromLabel;
                return PartialView("~/Views/Products/_CasModal.cshtml", CS);
            }


        }

        [HttpPost]
        public ActionResult Save(Cas CS)
        {
            // Let's try the PRG pattern
            int pk = CS.casid;
            pk = fnSaveToDB(CS);
            TempData["SaveResult"] = "Data Updated at " + DateTime.Now;
            return RedirectToAction("Edit", new { id = pk });

        }

        private int fnSaveToDB(Cas CS)
        {
                using (var db = new CMCSQL03Entities())
                {
                    if (CS.casid == -1)
                    {
                        var newrec = new EF.tblCAS();
                        db.tblCAS.Add(newrec);
                        db.SaveChanges();
                        CS.casid= newrec.CASID;
                    }
                    var q = (from t in db.tblCAS where t.CASID == CS.casid select t).FirstOrDefault();
                    q.ProductDetailID = CS.productdetailid;
                    q.CasNumber=CS.casnumber;
                    q.ChemicalName =CS.chemicalname;
                    q.Percentage =CS.percentage;
                    q.RestrictedQty =CS.restrictedqty;
                    q.RestrictedAmount=CS.restrictedamount;
                    q.PackOnReceipt=CS.packonreceipt;
                    q.ReportableQty=CS.reportableqty;
                    q.ReportableAmount = CS.reportableamount;
                    q.LessThan=CS.lessthan;
                    q.ExcludeFromLabel=CS.excludefromlabel;
                    db.SaveChanges();
                    return q.CASID;
                }
        }

        
        public ActionResult Delete(int id)
        {
            using (var db = new CMCSQL03Entities())
            {
                db.Database.ExecuteSqlCommand("Delete from tblCAS where CASID=" + id);
                return Content("Item Deleted");
                }
            }


        }


    }
