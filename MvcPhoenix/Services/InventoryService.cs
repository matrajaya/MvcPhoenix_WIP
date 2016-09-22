using MvcPhoenix.EF;
using MvcPhoenix.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcPhoenix.Services
{
    public class InventoryService
    {
        public static Inventory fnFillInventoryVM(int id)
        {
            using (var db = new CMCSQL03Entities())
            {
                Inventory vm = new Inventory();
                ProductProfile PP = new ProductProfile();

                PP.productdetailid = id;
                vm.PP = ProductsService.FillFromPD(PP);
                PP = ProductsService.FillFromPM(PP);
                PP = ProductsService.fnFillOtherPMProps(PP);

                //vm.vmMasterNotesAlert = PP.masternotesalert;    // cannot get View to properly handle this when buried in PP
                vm.ClientCode = (from t in db.tblClient
                                 where t.ClientID == PP.clientid
                                 select t.ClientCode).FirstOrDefault();

                vm.ClientUM = (from t in db.tblClient
                               where t.ClientID == id
                               select t.ClientUM).FirstOrDefault();

                vm.Division = (from t in db.tblDivision
                               where t.DivisionID == PP.divisionid
                               select t.Division).FirstOrDefault();

                var q = (from t in db.tblBulkOrderItem
                         where t.ProductMasterID == id && t.Status == "OP"
                         select t).FirstOrDefault();

                vm.BackOrderPending = q == null ? false : true;

                var q1 = (from t in db.tblBulkOrderItem
                          where t.ProductMasterID == id && t.Status == "OP"
                          select new { tot = t.Qty * t.Weight }).ToList();

                var q2 = (from x in q1 select x.tot).Sum();

                vm.BulkWeightCurrentlyOnOrder = Convert.ToDecimal(q2);

                vm.ShelfLevelTotal = StatusLevelShelf(id, "TOTAL");
                vm.ShelfLevelAvail = StatusLevelShelf(id, "AVAIL");
                vm.ShelfLevelTest = StatusLevelShelf(id, "TEST");
                vm.ShelfLevelHold = StatusLevelShelf(id, "HOLD");
                vm.ShelfLevelQC = StatusLevelShelf(id, "QC");
                vm.ShelfLevelReturn = StatusLevelShelf(id, "RETURN");
                vm.ShelfLevelRecd = StatusLevelShelf(id, "RECD");
                vm.ShelfLevelOther = StatusLevelShelf(id, "OTHER");
                vm.BulkLevelTotal = StatusLevelBulk(id, "TOTAL");
                vm.BulkLevelAvail = StatusLevelBulk(id, "AVAIL");
                vm.BulkLevelTest = StatusLevelBulk(id, "TEST");
                vm.BulkLevelHold = StatusLevelBulk(id, "HOLD");
                vm.BulkLevelQC = StatusLevelBulk(id, "QC");
                vm.BulkLevelReturn = StatusLevelBulk(id, "RETURN");
                vm.BulkLevelRecd = StatusLevelBulk(id, "RECD");
                vm.BulkLevelOther = StatusLevelBulk(id, "OTHER");
                vm.TotalLevelTotal = StatusLevelTotal(id, "TOTAL");
                vm.TotalLevelAvail = StatusLevelTotal(id, "AVAIL");
                vm.TotalLevelTest = StatusLevelTotal(id, "TEST");
                vm.TotalLevelHold = StatusLevelTotal(id, "HOLD");
                vm.TotalLevelQC = StatusLevelTotal(id, "QC");
                vm.TotalLevelReturn = StatusLevelTotal(id, "RETURN");
                vm.TotalLevelReturn = StatusLevelTotal(id, "RECD");
                vm.TotalLevelOther = StatusLevelTotal(id, "OTHER");

                return vm;
            }
        }

        public static void fnSaveInventory(Inventory vm)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                var pd = db.tblProductDetail.Find(vm.PP.productdetailid);
				var pm = db.tblProductMaster.Find(pd.ProductMasterID);
                pm.AlertNotesReceiving = vm.PP.alertnotesreceiving;
                
                db.SaveChanges();
            }
        }

        public static decimal StatusLevelShelf(int? id, string status)
        {
            // add up weight for a status and productdetailid
            using (var db = new EF.CMCSQL03Entities())
            {
                var mylist = (from t in db.tblStock
                              join sm in db.tblShelfMaster on t.ShelfID equals sm.ShelfID
                              where sm.ProductDetailID == id && t.StockID != null
                              select new { stat = t.ShelfStatus, tot = (t.QtyOnHand == null ? 0 : t.QtyOnHand) * (sm.UnitWeight == null ? 0 : sm.UnitWeight) }).ToList();

                decimal retval;

                switch (status)
                {
                    case "TOTAL":
                        var q1 = (from x in mylist select x.tot).Sum();
                        retval = Convert.ToDecimal(q1);
                        break;

                    case "OTHER":
                        var ListOfStatus = (from t in db.tblStock
                                            select t.ShelfStatus).Distinct().ToList();

                        var q2 = (from x in mylist
                                  where !ListOfStatus.Contains(x.stat)
                                  select x.tot).Sum();

                        retval = Convert.ToDecimal(q2);
                        break;

                    default:
                        var q3 = (from x in mylist where x.stat == status select x.tot).Sum();
                        retval = Convert.ToDecimal(q3);
                        break;
                }

                return System.Math.Round(retval, 0);
            }
        }

        public static decimal StatusLevelBulk(int id, string status)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                var mylist = (from t in db.vwBulkLevel
                              where t.ProductDetailID == id
                              select new
                              {
                                  stat = t.BulkStatus,
                                  tot = (t.Qty == null ? 1 : t.Qty) * (t.CurrentWeight == null ? 0 : t.CurrentWeight)
                              });

                decimal retval;

                switch (status)
                {
                    case "TOTAL":
                        var q1 = (from x in mylist select x.tot).Sum();
                        retval = Convert.ToDecimal(q1);
                        break;

                    case "OTHER":
                        // todo this needs to be not in('AVAIL','etc')
                        var ListOfStatus = (from t in db.tblBulk select t.BulkStatus).Distinct().ToList();
                        var q2 = (from x in mylist where !ListOfStatus.Contains(x.stat) select x.tot).Sum();
                        retval = Convert.ToDecimal(q2);
                        break;

                    default:
                        var q3 = (from x in mylist where x.stat == status select x.tot).Sum();
                        retval = Convert.ToDecimal(q3);
                        break;
                }

                return System.Math.Round(retval, 0);
            }
        }

        public static decimal StatusLevelTotal(int id, string status)
        {
            decimal s = StatusLevelShelf(id, status);
            decimal b = StatusLevelBulk(id, status);

            return System.Math.Round(s + b, 1);
        }

        public static StockViewModel fnFillStockViewModel(int id)
        {
            using (var db = new CMCSQL03Entities())
            {
                var vm = (from t in db.tblStock
                          join sm in db.tblShelfMaster on t.ShelfID equals sm.ShelfID
                          join pd in db.tblProductDetail on sm.ProductDetailID equals pd.ProductDetailID
                          join bk in db.tblBulk on t.BulkID equals bk.BulkID
                          where t.StockID == id
                          select new StockViewModel
                          {
                              StockID = t.StockID,
                              ShelfID = t.ShelfID,
                              BulkID = t.BulkID,
                              Warehouse = t.Warehouse,
                              QtyOnHand = t.QtyOnHand,
                              QtyAllocated = t.QtyAllocated,
                              Bin = t.Bin,
                              ShelfStatus = t.ShelfStatus,
                              WasteAccumStartDate = t.WasteAccumStartDate,
                              CreateDate = t.CreateDate,
                              CreateUser = t.CreateUser,
                              ProductDetailID = pd.ProductDetailID,
                              ProductCode = pd.ProductCode,
                              ProductName = pd.ProductName,
                              LotNumber = bk.LotNumber,
                              ExpirationDate = bk.ExpirationDate,
                              MfgDate = bk.MfgDate,
                              QCDate = bk.QCDate,
                              Size = sm.Size,
                              UnitWeight = sm.UnitWeight,
                              UpdateDate = t.UpdateDate,
                              UpdateUser = t.UpdateUser
                          }).FirstOrDefault();

                vm.ListOfShelfStatusIDs = Services.InventoryService.fnListOfShelfStatusIDs();
                vm.ListOfShelfMasterIDs = Services.InventoryService.fnListOfShelfMasterIDs(vm.ProductDetailID);
                vm.ListOfBulkIDs = Services.InventoryService.fnListOfBulkIDs(vm.ShelfID);
                vm.ListOfWareHouseIDs = Services.InventoryService.fnListOfWarehouseIDs();

                return vm;
            }
        }

        public static List<SelectListItem> fnListOfShelfStatusIDs()
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> mylist = new List<SelectListItem>();
                mylist.Add(new SelectListItem { Text = "AVAIL", Value = "AVAIL" });
                mylist.Add(new SelectListItem { Text = "TEST", Value = "TEST" });
                mylist.Add(new SelectListItem { Text = "HOLD", Value = "HOLD" });
                mylist.Add(new SelectListItem { Text = "QC", Value = "QC" });
                mylist.Add(new SelectListItem { Text = "RETURN", Value = "RETURN" });
                mylist.Insert(0, new SelectListItem { Value = "0", Text = "" });

                return mylist;
            }
        }

        public static List<SelectListItem> fnListOfShelfMasterIDs(int? id)
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> mylist = new List<SelectListItem>();
                mylist = (from t in db.tblShelfMaster
                          join pd in db.tblProductDetail on t.ProductDetailID equals pd.ProductDetailID
                          where t.ProductDetailID == id
                          select new SelectListItem { Value = t.ShelfID.ToString(), Text = t.Size }).ToList();
                mylist.Insert(0, new SelectListItem { Value = "", Text = "Select Size" });

                return mylist;
            }
        }

        public static List<SelectListItem> fnListOfBulkIDs(int? id)     // id=productdetailid
        {
            using (MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities())
            {
                List<SelectListItem> mylist = new List<SelectListItem>();
                mylist = (from t in db.tblBulk
                          join pm in db.tblProductMaster on t.ProductMasterID equals pm.ProductMasterID
                          join pd in db.tblProductDetail on pm.ProductMasterID equals pd.ProductMasterID
                          join sm in db.tblShelfMaster on pd.ProductDetailID equals sm.ProductDetailID
                          where sm.ShelfID == id
                          select new SelectListItem { Value = t.BulkID.ToString(), Text = t.LotNumber }).ToList();
                mylist.Insert(0, new SelectListItem { Value = "", Text = "Select Bulk Container" });

                return mylist;
            }
        }

        public static void fnSaveStock(StockViewModel vm)
        {
            using (var db = new CMCSQL03Entities())
            {
                var q = (from t in db.tblStock
                         where t.StockID == vm.StockID
                         select t).FirstOrDefault();

                q.Warehouse = vm.Warehouse;
                q.QtyOnHand = vm.QtyOnHand;
                q.QtyAllocated = vm.QtyAllocated;
                q.Bin = vm.Bin;
                q.ShelfStatus = vm.ShelfStatus;
                q.WasteAccumStartDate = vm.WasteAccumStartDate;
                q.UpdateDate = System.DateTime.Now;
                q.UpdateUser = HttpContext.Current.User.Identity.Name;

                db.SaveChanges();
            }
        }

        public static void fnSavePrePackStock(PrePackStock vm, FormCollection fc)
        {
            using (var db = new CMCSQL03Entities())
            {
                // for each sm, insert tblStock
                vm.BulkContainer.bulkid = fnNewBulkID();

                var dbBulk = db.tblBulk.Find(vm.BulkContainer.bulkid);
                var dbPD = db.tblProductDetail.Find(vm.ProductDetailID);

                dbBulk.ProductMasterID = dbPD.ProductMasterID;
                dbBulk.Qty = 1;
                dbBulk.ReceiveDate = vm.BulkContainer.receivedate;
                dbBulk.LotNumber = vm.BulkContainer.lotnumber;
                dbBulk.MfgDate = vm.BulkContainer.mfgdate;
                dbBulk.ExpirationDate = vm.BulkContainer.expirationdate;
                dbBulk.CeaseShipDate = vm.BulkContainer.ceaseshipdate;
                dbBulk.QCDate = vm.BulkContainer.qcdate;
                dbBulk.BulkStatus = vm.BulkContainer.bulkstatus;
                dbBulk.Bin = vm.BulkContainer.bin;
                dbBulk.Warehouse = vm.BulkContainer.warehouse;
                dbBulk.CreateDate = System.DateTime.Now;
                dbBulk.CreateUser = System.Web.HttpContext.Current.User.Identity.Name;
                dbBulk.ReceiveWeight = 0;
                dbBulk.CurrentWeight = 0;

                for (int i = 1; i <= vm.ShelfMasterCount; i++)
                {
                    // this will always have a value
                    string sThisShelfID = fc["Key" + i.ToString()];
                    Int32 ThisShelfID = Convert.ToInt32(sThisShelfID);

                    string sThisQty = fc["Value" + i.ToString()];
                    if (!String.IsNullOrEmpty(sThisQty))
                    {
                        Int32 ThisQty = Convert.ToInt32(sThisQty);
                        var newstock = new EF.tblStock();
                        newstock.ShelfID = ThisShelfID;
                        newstock.BulkID = vm.BulkContainer.bulkid;
                        newstock.Warehouse = vm.BulkContainer.warehouse;
                        newstock.QtyOnHand = ThisQty;

                        var sm = (from t in db.tblShelfMaster
                                  where t.ShelfID == ThisShelfID
                                  select t).FirstOrDefault();   // needed for default bin

                        dbBulk.ReceiveWeight = dbBulk.ReceiveWeight + (ThisQty * sm.UnitWeight);
                        newstock.Bin = sm.Bin;
                        newstock.ShelfStatus = "AVAIL";
                        newstock.CreateDate = DateTime.Now;
                        newstock.CreateUser = HttpContext.Current.User.Identity.Name;
                        db.tblStock.Add(newstock);

                        db.SaveChanges();
                    }
                }
            }
        }

        public static int fnNewBulkID()
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                EF.tblBulk newrec = new EF.tblBulk();
                db.tblBulk.Add(newrec);
                db.SaveChanges();

                return newrec.BulkID;
            }
        }

        public static List<SelectListItem> fnListOfWarehouseIDs()
        {
            List<SelectListItem> mylist = new List<SelectListItem>();
            mylist.Add(new SelectListItem { Value = null, Text = "" });
            mylist.Add(new SelectListItem { Value = "AP", Text = "AP" });
            mylist.Add(new SelectListItem { Value = "CT", Text = "CT" });
            mylist.Add(new SelectListItem { Value = "CO", Text = "CO" });
            mylist.Add(new SelectListItem { Value = "EU", Text = "EU" });

            return mylist;
        }
    }
}