using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcPhoenix.Models;

using MvcPhoenix.Services;

namespace MvcPhoenix.Controllers
{
    public class ReceivingController : Controller
    {
        private MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
        
     
        [HttpGet]
        public ActionResult Index()
        {
            List<BulkContainerViewModel> mylist = ReceivingService.fnIndexList();
            return View("~/Views/Receiving/Index.cshtml",mylist);            
        }

        public ActionResult SetUpReceiveKnown()
        {
            // Build client DD and load view to receive known
            ViewBag.ListOfClients = ReceivingService.fnClientIDs();
            return View("~/Views/Receiving/SelectProduct.cshtml");
        }

        public ActionResult SetUpReceivePrePack()
        {
            // Build client DD and load view to receive Pre-Packs
            ViewBag.ListOfClients = ReceivingService.fnClientIDs();
            return View("~/Views/Receiving/SelectPrePack.cshtml");
        }

        [HttpGet]
        public ActionResult SetupReceiveUnKnown()
        {
            // Load view to receive unknown bulk material
            BulkContainerViewModel obj = new BulkContainerViewModel();
            obj = ReceivingService.fnNewBulkContainerUnKnown();
            return View("~/Views/Receiving/Edit.cshtml", obj);
        }


        public ActionResult BuildProductMasterDropDown(int id)
        {
            // id=clientid .. return the <option> values for the <select> tag
            return Content(ReceivingService.fnBuildProductMasterDropDown(id));
        }
        

        public ActionResult CheckForOpenOrderItems(FormCollection fc)
        {
            // Build list of open order items and return a partial
            int pk = Convert.ToInt32(fc["productmasterid"]);
            List<OpenBulkOrderItems> mylist = ReceivingService.fnOpenBulkOrderItems(pk);
            if (mylist.Count() > 0)
            {
                return PartialView("~/Views/Receiving/_OpenOrderItems.cshtml", mylist);
            }
            else
            {
               return Content("<br>No open orders found for the selected Master Product");
            }
        }
        
        
        public void TagItemToBeClosed(int id, bool ischecked)
        {
            // Called from the onchange event of the checkbox on the view
            // Tag an open order item to be closed later on Save
            ReceivingService.fnTagItemToBeClosed(id, ischecked);
        }

      

        public ActionResult BuildProductCodeDropDown(int clientid)
        {
            return Content(ReceivingService.fnBuildProductCodeDropDown(clientid));
        }

        public ActionResult EnterPrePack(FormCollection fc)
        {
            // Take clientid and productdetailid from POST and build ViewModel, return data entry view
            int fc_clientid = Convert.ToInt32(fc["ClientID"]);
            int fc_productdetailid = Convert.ToInt32(fc["productdetailid"]);
            PrePackViewModel obj = new PrePackViewModel();
            obj = ReceivingService.fnNewBulkContainerForPrePack(fc_clientid,fc_productdetailid);
            return View("~/Views/Receiving/EnterPrePack.cshtml",obj);
        }


        [HttpPost]
        public ActionResult SavePrePack(PrePackViewModel vm, FormCollection fc)
        {
            bool bSave = ReceivingService.fnSavePrePack(vm, fc);
            return Content("Items Added to Shelf Stock: " + DateTime.Now.ToString());
        }
       
        [HttpGet]
        public ActionResult CreateContainerReceipt(int id)
        {
            // id = productmasterid .. Build a new viewmodel of a bulk container and load the edit view
            BulkContainerViewModel obj = new BulkContainerViewModel();
            obj = ReceivingService.fnNewBulkContainer(id);
            return View("~/Views/Receiving/Edit.cshtml", obj);
        }

       

        [HttpGet]
        public ActionResult EditContainerReceipt(int id)
        {
            // id = bulkid .. Lookup row in DB and load the edit view
            BulkContainerViewModel obj = new BulkContainerViewModel();
            obj = ReceivingService.fnFillBulkContainerFromDB(id);
            return View("~/Views/Receiving/Edit.cshtml", obj);
        }

        // No ActionResult to edit an unknown container ... it's a one way insert action

        [HttpPost]
        public ActionResult SaveContainer(BulkContainerViewModel bc)
        {
            // Update a tblBulk record from a BulkContainerViewModel
            bool bUpdate;
            switch (bc.isknownmaterial)
            {
                case true:
                    bUpdate = ReceivingService.fnSaveBulkContainerKnown(bc);
                    break;
                case false:
                    bUpdate = ReceivingService.fnSaveBulkContainerUnKnown(bc);
                    break;
                default:
                    return RedirectToAction("Index");
            }
            
            return RedirectToAction("Index");

        }





    }
}
