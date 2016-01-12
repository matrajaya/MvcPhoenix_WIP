using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcPhoenix.Models;

//pc add
using MvcPhoenix.Services;

// Dev Note: Receiving uses ReplenishmentViewModels.cs
// Discussion: Move Bulk Container ViewModels to Inventory or create separate ReceivingViewModels

namespace MvcPhoenix.Controllers
{
    public class ReceivingController : Controller
    {

        // TODO: Build out all the Actions for Receive PrePacks

        
        // Receiving Landing Page - returns a view with a Partial showing Bulk Containers in status=RECD
        [HttpGet]
        public ActionResult Index()
        {
                List<BulkContainerSearchResults> mylist = ReceivingService.fnReceivingDefaultSearchResults();
                return View(mylist);
        }

        // Receiving - New  Known Container
        public ActionResult Create()
        {
            // Called from Landing page, build new BulkContainer obj and return the view for drill down, data entry
            BulkContainer obj = new BulkContainer();
            Session["ListOfBulkOrderItemsToClose"] = null;
            obj.bulkid = -1;
            obj.clientid = 0;
            obj.productmasterid = 0;
            obj = ReceivingService.fnFillOtherBulkContainerProps(obj);
            return View(obj);
        }

        // Receiving - New UnKnown Container
        public ActionResult CreateUnknown()
        {
            // Called from Landing page, build new BulkContainer obj and return the view
            BulkContainer obj = new BulkContainer();
            Session["ListOfBulkOrderItemsToClose"] = null;
            obj.bulkid = -1;
            obj.clientid = 0;
            obj.productmasterid = 0;
            obj = ReceivingService.fnFillOtherBulkContainerProps(obj);
            return View(obj);
        }

        // Receiving - MasterCodes for a Selected Client
        public ActionResult fnProductMasterDD(int clientid, string change)
        {
            return Content(ReceivingService.fnProductCodesDropDown(clientid,change));
        }

        // Receiving - Return a partial with Open Order Items
        public ActionResult fnFillBulkOpenOrderItems(int id)
        {
            List<OpenBulkOrderItems> mylist = ReceivingService.fnOpenBulkOrderItems(id);
            if (mylist.Count() > 0)
            {
                return PartialView("~/Views/Receiving/_OpenOrderItems.cshtml", mylist);
            }
            else
            {
                return Content("No open orders found at  " + DateTime.Now.ToString());
            }

        }
        
        // This action ONLY for saving a Bulk Receipt
        [HttpPost]
        public ActionResult SavePostData(BulkContainer incoming)
        {
            string messageroot = "";
            BulkContainer obj = new BulkContainer();
            if (TryUpdateModel(obj))
            {
                if(ReceivingService.fnSaveBulkContainer(obj)==true)
                {
                    return Content(messageroot + DateTime.Now.ToString());
                }
                else
                {
                    return Content("DB Backend Error at " + DateTime.Now.ToString() + "</font>");
                }
            }
            else
            {
                return Content("Model Invalid At " + DateTime.Now.ToString());
            }
        }


        // This action ONLY for saving a Bulk Receipt - UNKNOWN
        [HttpPost]
        public ActionResult SavePostDataUnKnownMaterial(BulkContainer incoming)
        {
            string messageroot = "";
            BulkContainer obj = new BulkContainer();
            if (TryUpdateModel(obj))
            {
                if (ReceivingService.fnSaveBulkContainerUnKnownMaterial(obj) == true)
                {
                    return Content(messageroot + DateTime.Now.ToString());
                }
                else
                {
                    return Content("DB Backend Error at " + DateTime.Now.ToString() + "</font>");
                }
            }
            else
            {
                return Content("Model Invalid At " + DateTime.Now.ToString());
            }
        }
        
        public void BuildCloseList(int id, bool ischecked)
        {
            // persist the item to be closed in the row
            string s = "";
            System.Diagnostics.Debug.WriteLine(s);
            using (var db = new EF.CMCSQL03Entities())
            {
                s = "Update tblBulkOrderItem set ToBeClosed=0 where BulkOrderItemID=" + id;
                db.Database.ExecuteSqlCommand(s);
            }

            if (ischecked == true)
            {
                using (var db = new EF.CMCSQL03Entities())
                {
                    s = "Update tblBulkOrderItem set ToBeClosed=1 where BulkOrderItemID=" + id;
                    db.Database.ExecuteSqlCommand(s);
                }
            }
        }
        

    }
}
