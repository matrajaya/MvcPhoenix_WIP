using MvcPhoenix.EF;
using MvcPhoenix.Models;
using MvcPhoenix.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MvcPhoenix.Controllers
{
    public class ReturnOrdersController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BuildDivisionDropDown(int clientid)
        {
            var ddldivision = ApplicationService.ddlBuildDivision(clientid);

            return Content(ddldivision);
        }

        public static List<ReturnBulkViewModel> ListOfBulkItems(int clientid)
        {
            using (var db = new CMCSQL03Entities())
            {
                var bulkitems = (from t in db.tblBulk
                                 join pm in db.tblProductMaster on t.ProductMasterID equals pm.ProductMasterID
                                 join pd in db.tblProductDetail on pm.ProductMasterID equals pd.ProductMasterID
                                 join dv in db.tblDivision on pd.DivisionID equals dv.DivisionID
                                 join cl in db.tblClient on pm.ClientID equals cl.ClientID
                                 where cl.ClientID == clientid
                                 orderby pm.MasterCode
                                 select new ReturnBulkViewModel
                                 {
                                     bulkid = t.BulkID,
                                     lotnumber = t.LotNumber,
                                     clientid = cl.ClientID,
                                     clientname = cl.ClientName,
                                     warehouse = t.Warehouse,
                                     MasterCode = pm.MasterCode,
                                     MasterName = pm.MasterName,
                                     um = t.UM,
                                     receiveweight = t.ReceiveWeight,
                                     currentweight = t.CurrentWeight,
                                     expirationdate = t.ExpirationDate,
                                     ceaseshipdate = t.CeaseShipDate,
                                     bulkstatus = t.BulkStatus,
                                     bin = t.Bin,
                                     markedforreturn = t.MarkedForReturn,
                                     divisionid = pd.DivisionID,
                                     divisionname = dv.DivisionName
                                 }).ToList();

                return bulkitems;
            }
        }

        public JsonResult GetBulkItems(int clientid, int divisionid, string bulkstatus)
        {
            using (var db = new CMCSQL03Entities())
            {
                var unmarkedbulk = ListOfBulkItems(clientid);

                if (divisionid > 0)
                {
                    unmarkedbulk = unmarkedbulk.Where(t => t.divisionid == divisionid).ToList();
                }

                if (!String.IsNullOrEmpty(bulkstatus))
                {
                    unmarkedbulk = unmarkedbulk.Where(t => t.bulkstatus == bulkstatus).ToList();
                }

                unmarkedbulk = unmarkedbulk.Where(t => t.markedforreturn != true).ToList();

                return Json(unmarkedbulk, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetStockItems(int clientid, int divisionid, string stockstatus)
        {
            var unmarkedstock = InventoryService.GetStockItems(clientid);

            if (divisionid > 0)
            {
                unmarkedstock = unmarkedstock.Where(t => t.divisionid == divisionid).ToList();
            }

            if (!String.IsNullOrEmpty(stockstatus))
            {
                unmarkedstock = unmarkedstock.Where(t => t.ShelfStatus == stockstatus).ToList();
            }

            unmarkedstock = unmarkedstock.Where(t => t.markedforreturn != true).ToList();

            return Json(unmarkedstock, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> SaveSelectedItems(string inputclientid, string inputdivisionid, object[] bulkids, object[] stockids)
        {
            int orderId = 0;
            int clientId = Convert.ToInt32(inputclientid);
            int divisionId = Convert.ToInt32(inputdivisionid);
            var bulkItems = bulkids;
            var stockItems = stockids;

            if (clientId < 1)
            {
                return RedirectToAction("Index");
            }

            // Create new order
            // Write order master level info to order
            string user = HttpContext.User.Identity.Name;
            orderId = OrderService.NewOrderId(user);

            using (var db = new CMCSQL03Entities())
            {
                var orderMaster = db.tblOrderMaster.Find(orderId);
                var client = db.tblClient.Find(clientId);

                orderMaster.ClientID = clientId;
                orderMaster.DivisionID = divisionId;
                orderMaster.OrderType = "R";
                orderMaster.OrderDate = DateTime.UtcNow;
                orderMaster.Company = client.ClientName;
                orderMaster.UpdateDate = DateTime.UtcNow;
                orderMaster.UpdateUser = HttpContext.User.Identity.Name;

                db.SaveChanges();
            }

            // Fill order items for newly created order
            //-----------------------------------------

            // Make sure array has content before processing
            if (bulkItems != null && bulkItems.Length != 0)
            {
                for (int i = 0; i < bulkItems.Length; i++)
                {
                    int bulkId = Convert.ToInt32(bulkItems[i]);
                    int x = await OrderService.AddBulkItemToReturnOrder(orderId, bulkId);
                }
            }

            // Make sure array has content before processing
            if (stockItems != null && stockItems.Length != 0)
            {
                for (int i = 0; i < stockids.Length; i++)
                {
                    int stockId = Convert.ToInt32(stockItems[i]);
                    int x = await OrderService.AddStockItemToReturnOrder(orderId, stockId, clientId);
                }
            }

            await Task.Delay(1000);
            return Json(orderId, JsonRequestBehavior.AllowGet);
        }
    }
}