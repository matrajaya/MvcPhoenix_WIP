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
            var ddldivision = ApplicationService.ddlBuildDivisionDropDown(clientid);

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

        public static List<ReturnStockViewModel> ListOfStockItems(int clientid)
        {
            using (var db = new CMCSQL03Entities())
            {
                var stockitems = (from t in db.tblStock
                                  join sm in db.tblShelfMaster on t.ShelfID equals sm.ShelfID
                                  join pd in db.tblProductDetail on sm.ProductDetailID equals pd.ProductDetailID
                                  join pm in db.tblProductMaster on pd.ProductMasterID equals pm.ProductMasterID
                                  join bl in db.tblBulk on t.BulkID equals bl.BulkID
                                  join dv in db.tblDivision on pd.DivisionID equals dv.DivisionID
                                  where pm.ClientID == clientid
                                  orderby pd.ProductCode
                                  select new ReturnStockViewModel
                                  {
                                      ClientID = pm.ClientID,
                                      StockID = t.StockID,
                                      LotNumber = bl.LotNumber,
                                      ProductCode = pd.ProductCode,
                                      ProductName = pd.ProductName,
                                      Warehouse = t.Warehouse,
                                      Bin = t.Bin,
                                      QtyOnHand = t.QtyOnHand,
                                      Size = sm.Size,
                                      CurrentWeight = sm.UnitWeight * t.QtyOnHand,
                                      ExpirationDate = bl.ExpirationDate,
                                      ShelfStatus = t.ShelfStatus,
                                      markedforreturn = t.MarkedForReturn,
                                      divisionid = pd.DivisionID,
                                      divisionname = dv.DivisionName
                                  }).ToList();

                return stockitems;
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
                    //unmarkedbulk = unmarkedbulk.Where(t => t.bulkstatus == bulkstatus & t.bulkstatus != "RETURN").ToList();
                }

                unmarkedbulk = unmarkedbulk.Where(t => t.markedforreturn != true).ToList();

                return Json(unmarkedbulk, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetStockItems(int clientid, int divisionid, string stockstatus)
        {
            using (var db = new CMCSQL03Entities())
            {
                var unmarkedstock = ListOfStockItems(clientid);

                if (divisionid > 0)
                {
                    unmarkedstock = unmarkedstock.Where(t => t.divisionid == divisionid).ToList();
                }

                if (!String.IsNullOrEmpty(stockstatus))
                {
                    unmarkedstock = unmarkedstock.Where(t => t.ShelfStatus == stockstatus).ToList();
                    //unmarkedstock = unmarkedstock.Where(t => t.ShelfStatus == stockstatus & t.ShelfStatus != "RETURN").ToList();
                }

                unmarkedstock = unmarkedstock.Where(t => t.markedforreturn != true).ToList();

                return Json(unmarkedstock, JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<ActionResult> SaveSelectedItems(string inputclientid, string inputdivisionid, object[] bulkids, object[] stockids)
        {
            // Make sure cientid is passed
            if (String.IsNullOrEmpty(inputclientid))
            {
                return RedirectToAction("Index");
            }

            int orderid = 0;
            int clientid = Convert.ToInt32(inputclientid);
            int divisionid = Convert.ToInt32(inputdivisionid);

            // Create new order
            orderid = OrderService.fnNewOrderID();

            using (var db = new CMCSQL03Entities())
            {
                // write order master level info to order
                var om = (from t in db.tblOrderMaster
                          where t.OrderID == orderid
                          select t).FirstOrDefault();

                var cl = (from t in db.tblClient
                          where t.ClientID == clientid
                          select t).FirstOrDefault();

                om.ClientID = clientid;
                om.DivisionID = divisionid;
                om.OrderType = "R";
                om.OrderDate = DateTime.UtcNow;
                om.Company = cl.ClientName;
                om.CreateDate = DateTime.UtcNow;
                om.CreateUser = HttpContext.User.Identity.Name;
                om.UpdateDate = DateTime.UtcNow;
                om.UpdateUser = HttpContext.User.Identity.Name;

                db.SaveChanges();
            }

            // Fill order items for newly created order
            //-----------------------------------------

            // Make sure array has content before processing
            if (bulkids != null && bulkids.Length != 0)
            {
                for (int i = 0; i < bulkids.Length; i++)
                {
                    int bulkid = Convert.ToInt32(bulkids[i]);
                    int x = await OrderService.AddBulkItemToReturnOrder(orderid, bulkid);
                }
            }

            // Make sure array has content before processing
            if (stockids != null && stockids.Length != 0)
            {
                for (int i = 0; i < stockids.Length; i++)
                {
                    int stockid = Convert.ToInt32(stockids[i]);
                    int x = await OrderService.AddStockItemToReturnOrder(orderid, stockid);
                }
            }

            await Task.Delay(10000);
            return Json(orderid, JsonRequestBehavior.AllowGet);
        }
    }
}