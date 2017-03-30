﻿using MvcPhoenix.EF;
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
                ViewBag.ListOfClients = fnListOfClientIDs();
                ViewBag.ListOfBulkStatusIDs = fnBulkStatusIDs();
                ViewBag.ListOfStockStatusIDs = fnStockStatusIDs();
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

        public ActionResult LoadBulkPartial(int clientid, string bulkstatusid, int divisionid)
        {
            // build vm and return partial
            using (var db = new CMCSQL03Entities())
            {
                var vm = fnListOfBulkRecords();
                vm = (from t in vm
                      where t.clientid == clientid
                      select t).ToList();

                if (!String.IsNullOrEmpty(bulkstatusid))
                {
                    vm = (from t in vm
                          where t.bulkstatus == bulkstatusid
                          select t).ToList();
                }
                if (divisionid > 0)
                {
                    vm = (from t in vm
                          where t.divisionid == divisionid
                          select t).ToList();
                }

                vm = (from t in vm
                      where (t.markedforreturn == false)
                      select t).ToList();

                vm = (from t in vm
                      where (t.bulkstatus != "RETURN")
                      select t).ToList();

                ViewBag.TableHeader = "Bulk Stock Records";
                ViewBag.TableStyle = "height:350px;overflow-y:scroll;font-size:.95em;";
                ViewBag.RecordCount = vm.Count();

                ViewBag.MarkedCount = (from t in db.tblBulk
                                       where t.MarkedForReturn == true
                                       select t.BulkID).Count();

                ViewBag.UnMarkedCount = (from t in db.tblBulk
                                         where t.MarkedForReturn == false
                                         select t.BulkID).Count();

                ViewBag.ShowTheMarkAllBulkButton = (vm.Count() > 0) ? true : false;
                Session["BulkRecords"] = vm;    //stash list

                return PartialView("~/Views/ReturnOrders/_ReturnBulkRecords.cshtml", vm);
            }
        }

        public ActionResult LoadStockPartial(int clientid, string stockstatusid, int divisionid)
        {
            using (var db = new CMCSQL03Entities())
            {
                // build vm and return partial
                var vm = fnListOfStockRecords();
                vm = (from t in vm
                      where t.ClientID == clientid
                      select t).ToList();

                if (!String.IsNullOrEmpty(stockstatusid))
                {
                    vm = (from t in vm
                          where t.ShelfStatus == stockstatusid
                          select t).ToList();
                }

                if (divisionid > 0)
                {
                    vm = (from t in vm
                          where t.divisionid == divisionid
                          select t).ToList();
                }

                vm = (from t in vm
                      where (t.markedforreturn == false)
                      select t).ToList();

                vm = (from t in vm
                      where (t.ShelfStatus != "RETURN")
                      select t).ToList();

                ViewBag.TableHeader = "Shelf Stock Records";
                ViewBag.TableStyle = "height:350px;overflow-y:scroll;font-size:.95em;";
                ViewBag.RecordCount = vm.Count();
                ViewBag.ShowTheMarkAllStockButton = (vm.Count() > 0) ? true : false;
                Session["StockRecords"] = vm;   //stash list

                return PartialView("~/Views/ReturnOrders/_ReturnStockRecords.cshtml", vm);
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

        public ActionResult LoadMarkedBulkPartial()
        {
            using (var db = new CMCSQL03Entities())
            {
                var vm = fnListOfBulkRecords();
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

        public ActionResult LoadMarkedStockPartial()
        {
            using (var db = new CMCSQL03Entities())
            {
                var vm = fnListOfStockRecords();
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

        public static List<ReturnBulkViewModel> fnListOfBulkRecords()
        {
            using (var db = new CMCSQL03Entities())
            {
                // make this generic and further filter elsewhere
                var vm = (from t in db.tblBulk
                          join pm in db.tblProductMaster on t.ProductMasterID equals pm.ProductMasterID
                          join pd in db.tblProductDetail on pm.ProductMasterID equals pd.ProductMasterID
                          join dv in db.tblDivision on pd.DivisionID equals dv.DivisionID
                          join cl in db.tblClient on pm.ClientID equals cl.ClientID
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

        public static List<ReturnStockViewModel> fnListOfStockRecords()
        {
            using (var db = new CMCSQL03Entities())
            {
                var vm = (from t in db.tblStock
                          join sm in db.tblShelfMaster on t.ShelfID equals sm.ShelfID
                          join pd in db.tblProductDetail on sm.ProductDetailID equals pd.ProductDetailID
                          join pm in db.tblProductMaster on pd.ProductMasterID equals pm.ProductMasterID
                          join bl in db.tblBulk on t.BulkID equals bl.BulkID
                          join dv in db.tblDivision on pd.DivisionID equals dv.DivisionID
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

        private static List<SelectListItem> fnListOfClientIDs()
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> mylist = new List<SelectListItem>();
                mylist = (from t in db.tblClient
                          orderby t.ClientName
                          select new SelectListItem { Value = t.ClientID.ToString(), Text = t.CMCLocation + " - " + t.ClientName }).ToList();
                mylist.Insert(0, new SelectListItem { Value = "0", Text = "Select Client" });

                return mylist;
            }
        }

        private static List<SelectListItem> fnBulkStatusIDs()
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> mylist = new List<SelectListItem>();
                mylist = (from t in db.tblBulk
                          orderby t.BulkStatus
                          where t.BulkStatus != "R"
                          select new SelectListItem { Value = t.BulkStatus, Text = t.BulkStatus }).Distinct().ToList();
                mylist.Insert(0, new SelectListItem { Value = "0", Text = "Bulk Status" });

                return mylist;
            }
        }

        private static List<SelectListItem> fnStockStatusIDs()
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> mylist = new List<SelectListItem>();
                mylist = (from t in db.tblStock
                          orderby t.ShelfStatus
                          select new SelectListItem { Value = t.ShelfStatus, Text = t.ShelfStatus }).Distinct().ToList();
                mylist.Insert(0, new SelectListItem { Value = "0", Text = "Shelf Status" });

                return mylist;
            }
        }

        public ActionResult BuildDivisionDropDown(int id)   // id=clientid
        {
            using (var db = new CMCSQL03Entities())
            {
                db.Database.ExecuteSqlCommand("Update tblBulk set MarkedForReturn=0");
                db.Database.ExecuteSqlCommand("Update tblStock set MarkedForReturn=0");
                Session["BulkRecords"] = null;
                Session["StockRecords"] = null;

                var qry = (from t in db.tblBulk
                           join pm in db.tblProductMaster on t.ProductMasterID equals pm.ProductMasterID
                           join pd in db.tblProductDetail on pm.ProductMasterID equals pd.ProductMasterID
                           join dv in db.tblDivision on pd.DivisionID equals dv.DivisionID
                           where pm.ClientID == id
                           orderby dv.DivisionName, dv.BusinessUnit
                           select new
                           {
                               dvID = dv.DivisionID,
                               dvDivisionName = dv.DivisionName,
                               dvBusinessUnit = dv.BusinessUnit
                           }).Distinct().ToList();

                string s = "<option value='0'>All Divisions</option>";
                if (qry.Count() > 0)
                {
                    foreach (var item in qry)
                    {
                        s = s + "<option value=" + item.dvID.ToString() + ">" + item.dvDivisionName + " - " + item.dvBusinessUnit + "</option>";
                    }
                }
                s = s + "</select>";

                return Content(s);
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

        public ActionResult CreateReturnOrderBulkItems(int orderid)
        {
            using (var db = new CMCSQL03Entities())
            {
                var bulk = (from t in db.tblBulk
                            where t.MarkedForReturn == true
                            select t).ToList();

                foreach (var row in bulk)
                {
                    var pm = (from t in db.tblProductMaster
                              where t.ProductMasterID == row.ProductMasterID
                              select t).FirstOrDefault();

                    int newItemId = OrderService.fnNewItemID();

                    var newitem = (from t in db.tblOrderItem
                                   where t.ItemID == newItemId
                                   select t).FirstOrDefault();

                    newitem.OrderID = orderid;
                    newitem.BulkID = row.BulkID;
                    newitem.ShelfID = GetShelfId(pm.ProductMasterID, row.UM);
                    newitem.ProductDetailID = GetProductDetailId(pm.ProductMasterID);
                    newitem.ProductCode = pm.MasterCode;
                    newitem.ProductName = pm.MasterName;
                    newitem.LotNumber = row.LotNumber;
                    newitem.Qty = 1;
                    newitem.Size = row.UM;
                    newitem.Weight = row.CurrentWeight;
                    newitem.Bin = row.Bin;
                    newitem.ItemNotes = "Return Bulk Order Item \nBulk container for return is " + row.UM + " with a current weight of " + row.CurrentWeight;
                    newitem.CreateDate = DateTime.UtcNow;
                    newitem.CreateUser = HttpContext.User.Identity.Name;
                    newitem.UpdateDate = DateTime.UtcNow;
                    newitem.UpdateUser = HttpContext.User.Identity.Name;

                    // Insert log record
                    OrderService.fnInsertLogRecord("BS-RTN", DateTime.UtcNow, null, row.BulkID, 1, row.CurrentWeight, DateTime.UtcNow, User.Identity.Name, null, null);

                    //update bulk record
                    row.CurrentWeight = 0;
                    row.BulkStatus = "RETURN";
                    row.MarkedForReturn = null;
                    db.SaveChanges();

                    OrderService.fnGenerateOrderTransactions(newItemId);
                }
            }

            return null;
        }

        private int GetProductDetailId(int? productmasterid)
        {
            using (var db = new CMCSQL03Entities())
            {
                int getproductdetailid = (from pd in db.tblProductDetail
                                          join pm in db.tblProductMaster on pd.ProductMasterID equals pm.ProductMasterID
                                          where pm.ProductMasterID == productmasterid && pd.ProductCode == pm.MasterCode
                                          select pd.ProductDetailID).FirstOrDefault();

                return getproductdetailid;
            }
        }

        public int GetShelfId(int? productmasterid, string um)
        {
            // find original productdetailid for productmasterid, ignore equivalent products
            // use productdetailid and um(size) to check shelfmaster
            // if true: get shelfid ?: insert new record in shelfmaster using productdetailid and size = um

            int getproductdetailid = GetProductDetailId(productmasterid);

            int xshelfid = 0;
            try
            {
                using (var db = new CMCSQL03Entities())
                {
                    int qshelfid = (from t in db.tblShelfMaster
                                    where t.ProductDetailID == getproductdetailid && t.Size == um
                                    select t.ShelfID).FirstOrDefault();

                    if (qshelfid != 0)
                    {
                        xshelfid = qshelfid;
                    }
                    else
                    {
                        var newrecord = new tblShelfMaster
                        {
                            ProductDetailID = getproductdetailid,
                            Size = um
                        };
                        db.tblShelfMaster.Add(newrecord);
                        db.SaveChanges();

                        xshelfid = newrecord.ShelfID;
                    }
                }
            }
            catch (Exception)
            {
                xshelfid = 0;
            }

            return xshelfid;
        }

        public ActionResult CreateReturnOrderStockItems(int orderid)
        {
            using (var db = new CMCSQL03Entities())
            {
                var stock = (from t in db.tblStock
                             where t.MarkedForReturn == true
                             select t).ToList();

                foreach (var row in stock)
                {
                    var bl = db.tblBulk.Find(row.BulkID);
                    var sm = db.tblShelfMaster.Find(row.ShelfID);
                    var pd = db.tblProductDetail.Find(sm.ProductDetailID);
                    var pm = db.tblProductMaster.Find(bl.ProductMasterID);

                    int newItemId = OrderService.fnNewItemID();
                    var newitem = (from t in db.tblOrderItem
                                   where t.ItemID == newItemId
                                   select t).FirstOrDefault();

                    newitem.OrderID = orderid;
                    newitem.ShelfID = sm.ShelfID;
                    newitem.ProductDetailID = pd.ProductDetailID;
                    newitem.ProductCode = pd.ProductCode;
                    newitem.ProductName = pd.ProductName;
                    newitem.LotNumber = bl.LotNumber;
                    newitem.Qty = row.QtyOnHand;
                    newitem.Size = sm.Size;
                    newitem.Weight = sm.UnitWeight;
                    newitem.Bin = row.Bin;
                    newitem.ItemNotes = "Return Shelf Order Item";
                    newitem.CreateDate = DateTime.UtcNow;
                    newitem.CreateUser = HttpContext.User.Identity.Name;
                    newitem.UpdateDate = DateTime.UtcNow;
                    newitem.UpdateUser = HttpContext.User.Identity.Name;

                    // Insert log record
                    OrderService.fnInsertLogRecord("SS-RTN", DateTime.UtcNow, row.StockID, null, 1, sm.UnitWeight, DateTime.UtcNow, User.Identity.Name, null, null);

                    //update bulk record
                    row.QtyOnHand = 0;
                    row.ShelfStatus = "RETURN";
                    row.MarkedForReturn = null;
                    db.SaveChanges();

                    OrderService.fnGenerateOrderTransactions(newItemId);
                    // Should we DELETE the tblStock record? - pc
                }
            }
            return null;
        }

        [HttpPost]
        public ActionResult CreateReturnOrder(FormCollection fc)
        {
            using (var db = new CMCSQL03Entities())
            {
                if (String.IsNullOrEmpty(fc["hiddenclientid"]))
                {
                    return RedirectToAction("Index");   // nothing to do; clientid missing -> start over
                }

                int MyClientID = Convert.ToInt32(fc["hiddenclientid"]);
                int MyDivisionID = Convert.ToInt32(fc["hiddendivisionid"]);
                
                // be sure user added items
                var qbulkcnt = (from t in db.tblBulk
                                where t.MarkedForReturn == true
                                select t.BulkID).Count();

                var qstockcnt = (from t in db.tblStock
                                 where t.MarkedForReturn == true
                                 select t.StockID).Count();

                if (qbulkcnt == 0 && qstockcnt == 0)
                {
                    return RedirectToAction("Index");   // nothing to do
                }

                int NewOrderID = 0; // New OrderID record
                NewOrderID = OrderService.fnNewOrderID();
                var om = (from t in db.tblOrderMaster
                          where t.OrderID == NewOrderID
                          select t).FirstOrDefault();

                var cl = (from t in db.tblClient
                          where t.ClientID == MyClientID
                          select t).FirstOrDefault();

                om.ClientID = MyClientID;
                om.DivisionID = MyDivisionID;
                om.OrderType = "R";
                om.OrderDate = DateTime.UtcNow;
                om.Company = cl.ClientName;
                om.CreateDate = DateTime.UtcNow;
                om.CreateUser = System.Web.HttpContext.Current.User.Identity.Name;
                db.SaveChanges();

                CreateReturnOrderBulkItems(NewOrderID);
                CreateReturnOrderStockItems(NewOrderID);

                return RedirectToAction("Edit", "Orders", new { id = NewOrderID });
            }
        }
    }
}