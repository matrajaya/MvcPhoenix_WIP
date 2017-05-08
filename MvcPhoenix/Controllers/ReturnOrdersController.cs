using MvcPhoenix.EF;
using MvcPhoenix.Models;
using MvcPhoenix.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MvcPhoenix.Controllers
{
    public class ReturnOrdersController : Controller
    {
        public ActionResult Index()
        {
            // Note: This 'assumes' one user at a time - pc
            // We may need to reconsider this assumption -- Iffy
            using (var db = new CMCSQL03Entities())
            {
                db.Database.ExecuteSqlCommand("Update tblBulk set MarkedForReturn=0");
                db.Database.ExecuteSqlCommand("Update tblStock set MarkedForReturn=0");                
                Session["BulkRecords"] = null;
                Session["StockRecords"] = null;

                return View();
            }
        }

        public ActionResult Search()
        {
            // Nothing to do server side, maybe later - pc
            // Returns null but breaks module if removed,
            // perhaps move some of the server calls in view here - Iffy
            return null;
        }

        //public ActionResult UnMarkedBulkLoad(int clientid, int divisionid, string bulkstatus)
        //{
        //    using (var db = new CMCSQL03Entities())
        //    {
        //        var obj = fnListOfBulkRecords(clientid);
        //        obj = obj.Where(t => t.clientid == clientid).ToList();
                
        //        if (divisionid > 0)
        //        {
        //            obj = obj.Where(t => t.divisionid == divisionid).ToList();
        //        }
        //        if (!String.IsNullOrEmpty(bulkstatus))
        //        {
        //            obj = obj.Where(t => t.bulkstatus == bulkstatus).ToList();
        //        }

        //        return Json(obj, JsonRequestBehavior.AllowGet);
        //    }
        //}

        public ActionResult LoadBulkPartial(int clientid, int divisionid, string bulkstatus)
        {
            // build vm and return partial
            using (var db = new CMCSQL03Entities())
            {
                var obj = fnListOfBulkRecords(clientid);
                
                //obj = (from t in obj
                //      where t.clientid == clientid
                //      select t).ToList();

                if (divisionid > 0)
                {
                    obj = obj.Where(t => t.divisionid == divisionid).ToList();
                }

                if (!String.IsNullOrEmpty(bulkstatus))
                {
                    obj = obj.Where(t => t.bulkstatus == bulkstatus).ToList();
                    obj = obj.Where(t => t.bulkstatus != "RETURN").ToList();
                }

                obj = obj.Where(t => t.markedforreturn == false).ToList();
                
                ViewBag.TableHeader = "Bulk Stock Records";
                ViewBag.TableStyle = "height:350px;overflow-y:scroll;font-size:.95em;";
                ViewBag.RecordCount = obj.Count();

                ViewBag.MarkedCount = (from t in db.tblBulk
                                       where t.MarkedForReturn == true
                                       select t.BulkID).Count();

                ViewBag.UnMarkedCount = (from t in db.tblBulk
                                         where t.MarkedForReturn == false
                                         select t.BulkID).Count();

                ViewBag.ShowTheMarkAllBulkButton = (obj.Count() > 0) ? true : false;

                Session["BulkRecords"] = obj;    //stash list

                return PartialView("~/Views/ReturnOrders/_ReturnBulkRecords.cshtml", obj);
            }
        }

        public ActionResult LoadStockPartial(int clientid, int divisionid, string stockstatus)
        {
            using (var db = new CMCSQL03Entities())
            {
                var obj = fnListOfStockRecords(clientid);
                //obj = (from t in obj
                //      where t.ClientID == clientid
                //      select t).ToList();

                if (divisionid > 0)
                {
                    obj = obj = obj.Where(t => t.divisionid == divisionid).ToList();
                }

                if (!String.IsNullOrEmpty(stockstatus))
                {
                    obj = obj = obj.Where(t => t.ShelfStatus == stockstatus).ToList();
                    obj = obj = obj.Where(t => t.ShelfStatus != "RETURN").ToList();
                }

                obj = obj.Where(t => t.markedforreturn == false).ToList();

                //obj = (from t in obj
                //      where (t.markedforreturn == false)
                //      select t).ToList();

                //obj = (from t in obj
                //      where (t.ShelfStatus != "RETURN")
                //      select t).ToList();

                ViewBag.TableHeader = "Shelf Stock Records";
                ViewBag.TableStyle = "height:350px;overflow-y:scroll;font-size:.95em;";
                ViewBag.RecordCount = obj.Count();

                ViewBag.ShowTheMarkAllStockButton = (obj.Count() > 0) ? true : false;

                Session["StockRecords"] = obj;   //stash list

                return PartialView("~/Views/ReturnOrders/_ReturnStockRecords.cshtml", obj);
            }
        }

        public ActionResult MarkBulkRecordForReturn(int id)
        {
            using (var db = new CMCSQL03Entities())
            {
                var q = (from t in db.tblBulk
                         where t.BulkID == id
                         select t).FirstOrDefault();

                q.MarkedForReturn = true;
                db.SaveChanges();

                return null;
            }
        }

        public ActionResult MarkAllBulkRecordsForReturn()
        {
            using (var db = new CMCSQL03Entities())
            {
                var vm = (List<ReturnBulkViewModel>)Session["BulkRecords"];
                foreach (var item in vm)
                {
                    string s = String.Format("Update tblBulk Set MarkedForReturn=1 where BulkID={0}", item.bulkid);
                    db.Database.ExecuteSqlCommand(s);
                }

                return null;
            }
        }

        public ActionResult MarkAllStockRecordsForReturn()
        {
            using (var db = new CMCSQL03Entities())
            {
                var vm = (List<ReturnStockViewModel>)Session["StockRecords"];
                foreach (var item in vm)
                {
                    string s = String.Format("Update tblStock Set MarkedForReturn=1 where StockID={0}", item.StockID);
                    db.Database.ExecuteSqlCommand(s);
                }
                return null;
            }
        }

        public ActionResult UnMarkBulkRecordForReturn(int id)
        {
            using (var db = new CMCSQL03Entities())
            {
                var q = (from t in db.tblBulk
                         where t.BulkID == id
                         select t).FirstOrDefault();

                q.MarkedForReturn = false;
                db.SaveChanges();

                return null;
            }
        }

        public ActionResult MarkStockRecordForReturn(int id)
        {
            using (var db = new CMCSQL03Entities())
            {
                var q = (from t in db.tblStock
                         where t.StockID == id
                         select t).FirstOrDefault();

                q.MarkedForReturn = true;
                db.SaveChanges();

                return null;
            }
        }

        public ActionResult UnMarkStockRecordForReturn(int id)
        {
            using (var db = new CMCSQL03Entities())
            {
                var q = (from t in db.tblStock
                         where t.StockID == id
                         select t).FirstOrDefault();

                q.MarkedForReturn = false;
                db.SaveChanges();

                return null;
            }
        }

        public ActionResult LoadMarkedBulkPartial(int clientid)
        {
            using (var db = new CMCSQL03Entities())
            {
                var vm = fnListOfBulkRecords(clientid);
                vm = (from t in vm
                      where t.markedforreturn == true
                      select t).ToList();

                ViewBag.TableHeader = "Selected Bulk Records";
                ViewBag.TableStyle = "font-size:.95em;";
                ViewBag.RecordCount = vm.Count();
                ViewBag.ShowTheMarkAllBulkButton = false;

                return PartialView("~/Views/ReturnOrders/_ReturnBulkRecords.cshtml", vm);
            }
        }

        public ActionResult LoadMarkedStockPartial(int clientid)
        {
            using (var db = new CMCSQL03Entities())
            {
                var vm = fnListOfStockRecords(clientid);
                vm = (from t in vm
                      where t.markedforreturn == true
                      select t).ToList();

                ViewBag.TableHeader = "Selected Shelf Stock Records";
                ViewBag.TableStyle = "font-size:.95em;";
                ViewBag.RecordCount = vm.Count();
                ViewBag.ShowTheMarkAllStockButton = false;

                return PartialView("~/Views/ReturnOrders/_ReturnStockRecords.cshtml", vm);
            }
        }

        public static List<ReturnBulkViewModel> fnListOfBulkRecords(int clientid)
        {
            using (var db = new CMCSQL03Entities())
            {
                // make this generic and further filter elsewhere
                var vm = (from t in db.tblBulk
                          join pm in db.tblProductMaster on t.ProductMasterID equals pm.ProductMasterID
                          join pd in db.tblProductDetail on pm.ProductMasterID equals pd.ProductMasterID
                          join dv in db.tblDivision on pd.DivisionID equals dv.DivisionID
                          join cl in db.tblClient on pm.ClientID equals cl.ClientID
                          where cl.ClientID == clientid
                          orderby pm.MasterCode
                          select new Models.ReturnBulkViewModel
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

                return vm;
            }
        }

        public static List<ReturnStockViewModel> fnListOfStockRecords(int clientid)
        {
            using (var db = new CMCSQL03Entities())
            {
                var vm = (from t in db.tblStock
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
                                  ExpirationDate = bl.ExpirationDate,
                                  ShelfStatus = t.ShelfStatus,
                                  markedforreturn = t.MarkedForReturn,
                                  divisionid = pd.DivisionID,
                                  divisionname = dv.DivisionName
                              }).ToList();

                return vm;
            }
        }

        public ActionResult BuildDivisionDropDown(int clientid)
        {
            using (var db = new CMCSQL03Entities())
            {
                db.Database.ExecuteSqlCommand("Update tblBulk set MarkedForReturn=0");
                db.Database.ExecuteSqlCommand("Update tblStock set MarkedForReturn=0");
                Session["BulkRecords"] = null;
                Session["StockRecords"] = null;

                var ddldivision = ApplicationService.BuildDivisionDropDown(clientid);

                return Content(ddldivision);
            }
        }

        public int fnMarkedCount()
        {
            using (var db = new CMCSQL03Entities())
            {
                var q1 = (from t in db.tblBulk
                          where t.MarkedForReturn == true
                          select t.BulkID).Count();

                var q2 = (from t in db.tblStock
                          where t.MarkedForReturn == true
                          select t.StockID).Count();

                int mycount = q1 + q2;

                return mycount;
            }
        }
                
        [HttpPost]
        public ActionResult CreateReturnOrder(FormCollection fc)
        {
            using (var db = new CMCSQL03Entities())
            {
                // check for clientid, exit if none found
                if (String.IsNullOrEmpty(fc["hiddenclientid"]))
                {
                    return RedirectToAction("Index");
                }

                // pull client and division ids from submitted form
                int clientid = Convert.ToInt32(fc["hiddenclientid"]);
                int divisionid = Convert.ToInt32(fc["hiddendivisionid"]);
                
                // count bulk returns
                var qbulkcnt = (from t in db.tblBulk
                                where t.MarkedForReturn == true
                                select t.BulkID).Count();

                // count stock returns
                var qstockcnt = (from t in db.tblStock
                                 where t.MarkedForReturn == true
                                 select t.StockID).Count();

                // exit if no item is marked for return
                if (qbulkcnt == 0 && qstockcnt == 0)
                {
                    return RedirectToAction("Index");
                }

                // create new orderid
                int NewOrderID = OrderService.fnNewOrderID();

                // write order master level info to order
                var om = (from t in db.tblOrderMaster
                          where t.OrderID == NewOrderID
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

                // Fill order items for newly created order
                OrderService.CreateReturnOrderBulkItems(NewOrderID);
                OrderService.CreateReturnOrderStockItems(NewOrderID);

                return RedirectToAction("Edit", "Orders", new { id = NewOrderID });
            }
        }
    }
}