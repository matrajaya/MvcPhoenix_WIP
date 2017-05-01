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

                //vm.vmMasterNotesAlert = PP.masternotesalert;
                // cannot get View to properly handle this when buried in PP
                vm.ClientCode = (from t in db.tblClient
                                 where t.ClientID == PP.clientid
                                 select t.ClientCode).FirstOrDefault();

                vm.ClientUM = (from t in db.tblClient
                               where t.ClientID == id
                               select t.ClientUM).FirstOrDefault();

                vm.Division = (from t in db.tblDivision
                               where t.DivisionID == PP.divisionid
                               select t.DivisionName).FirstOrDefault();

                var q = (from t in db.tblBulkOrderItem
                         where t.ProductMasterID == id
                         && t.Status == "OP"
                         select t).FirstOrDefault();

                vm.BackOrderPending = q == null ? false : true;

                var q1 = (from t in db.tblBulkOrderItem
                          where t.ProductMasterID == id
                          && t.Status == "OP"
                          select new
                          {
                              tot = t.Qty * t.Weight
                          }).ToList();

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
                vm.TotalLevelRecd = StatusLevelTotal(id, "RECD");
                vm.TotalLevelOther = StatusLevelTotal(id, "OTHER");

                return vm;
            }
        }

        public static void fnSaveInventory(Inventory vm)
        {
            using (var db = new CMCSQL03Entities())
            {
                var pd = db.tblProductDetail.Find(vm.PP.productdetailid);
                var pm = db.tblProductMaster.Find(pd.ProductMasterID);

                pm.AlertNotesPackout = vm.PP.alertnotespackout;
                pm.AlertNotesReceiving = vm.PP.alertnotesreceiving;
                pd.AlertNotesOrderEntry = vm.PP.alertnotesorderentry;
                pd.AlertNotesShipping = vm.PP.alertnotesshipping;

                db.SaveChanges();
            }
        }

        public static decimal StatusLevelShelf(int? id, string status)
        {
            // add up weight for a status and productdetailid
            using (var db = new CMCSQL03Entities())
            {
                var mylist = (from t in db.tblStock
                              join sm in db.tblShelfMaster on t.ShelfID equals sm.ShelfID
                              where sm.ProductDetailID == id && t.StockID != null
                              select new { stat = t.ShelfStatus, tot = (t.QtyOnHand == null ? 0 : t.QtyOnHand) * (sm.UnitWeight == null ? 0 : sm.UnitWeight) }).ToList();

                decimal retval;

                switch (status)
                {
                    case "TOTAL":
                        var q1 = (from x in mylist 
                                  select x.tot).Sum();

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
                        var q3 = (from x in mylist 
                                  where x.stat == status 
                                  select x.tot).Sum();

                        retval = Convert.ToDecimal(q3);
                        break;
                }

                return System.Math.Round(retval, 0);
            }
        }

        public static decimal StatusLevelBulk(int id, string status)
        {
            using (var db = new CMCSQL03Entities())
            {
                var mylist = (from pd in db.tblProductDetail
                              join pm in db.tblProductMaster on pd.ProductMasterID equals pm.ProductMasterID
                              join b in db.tblBulk on pm.ProductMasterID equals b.ProductMasterID
                              where pd.ProductDetailID == id
                              select new
                              {
                                  stat = b.BulkStatus,
                                  tot = (b.Qty == null ? 1 : b.Qty) * (b.CurrentWeight == null ? 0 : b.CurrentWeight)
                              });

                decimal retval;

                switch (status)
                {
                    case "TOTAL":
                        var q1 = (from x in mylist
                                  select x.tot).Sum();

                        retval = Convert.ToDecimal(q1);
                        break;

                    case "OTHER":
                        var q2 = (from x in mylist
                                  where !(x.stat == "AVAIL" || x.stat == "TEST" || x.stat == "HOLD" || x.stat == "QC" || x.stat == "RETURN" || x.stat == "RECD")
                                  select x.tot).Sum();

                        retval = Convert.ToDecimal(q2);
                        break;

                    default:
                        var q3 = (from x in mylist
                                  where x.stat == status
                                  select x.tot).Sum();

                        retval = Convert.ToDecimal(q3);
                        break;
                }

                return Math.Round(retval, 0);
            }
        }

        public static decimal StatusLevelTotal(int id, string status)
        {
            decimal s = StatusLevelShelf(id, status);
            decimal b = StatusLevelBulk(id, status);

            return Math.Round(s + b, 1);
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
                              QtyAvailable = t.QtyAvailable,
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
                              CeaseShipDate = bk.CeaseShipDate,
                              Size = sm.Size,
                              UnitWeight = sm.UnitWeight,
                              UpdateDate = t.UpdateDate,
                              UpdateUser = t.UpdateUser
                          }).FirstOrDefault();

                vm.ListOfShelfMasterIDs = ApplicationService.ddlShelfMasterIDs(vm.ProductDetailID);

                return vm;
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
                q.Bin = vm.Bin;
                q.ShelfStatus = vm.ShelfStatus;
                q.WasteAccumStartDate = vm.WasteAccumStartDate;
                q.UpdateDate = DateTime.UtcNow;
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
                dbBulk.CreateDate = DateTime.UtcNow;
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
                        var newstock = new tblStock();
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
                        newstock.CreateDate = DateTime.UtcNow;
                        newstock.CreateUser = HttpContext.Current.User.Identity.Name;
                        db.tblStock.Add(newstock);

                        db.SaveChanges();
                    }
                }
            }
        }

        public static int fnNewBulkID()
        {
            using (var db = new CMCSQL03Entities())
            {
                EF.tblBulk newrec = new tblBulk();
                db.tblBulk.Add(newrec);
                db.SaveChanges();

                return newrec.BulkID;
            }
        }

        #region Inventory Product Master Log Notes

        public static List<InventoryLogNote> ListInvPMLogNotes(int? masterid)
        {
            using (var db = new CMCSQL03Entities())
            {
                var obj = (from t in db.tblInvPMLogNote
                           where t.ProductMasterID == masterid
                           orderby t.InvPMLogNoteIDID descending
                           select new InventoryLogNote
                           {
                               productnoteid = t.InvPMLogNoteIDID,
                               productmasterid = t.ProductMasterID,
                               notedate = t.NoteDate,
                               notes = t.Notes,
                               reasoncode = t.Comment,                                      // TBD: modify table field later to become reasoncode - Iffy
                               UpdateDate = t.UpdateDate,
                               UpdateUser = t.UpdateUser,
                               CreateDate = t.CreateDate,
                               CreateUser = t.CreateUser
                           }).ToList();

                return obj;
            }
        }

        public static InventoryLogNote fnGetInventoryNote(int id)
        {
            using (var db = new CMCSQL03Entities())
            {
                InventoryLogNote obj = new InventoryLogNote();

                var q = (from t in db.tblInvPMLogNote
                         where t.InvPMLogNoteIDID == id
                         select t).FirstOrDefault();

                obj.productnoteid = q.InvPMLogNoteIDID;
                obj.productmasterid = q.ProductMasterID;
                obj.reasoncode = q.Comment;
                obj.notedate = q.NoteDate;
                obj.notes = q.Notes;
                obj.UpdateDate = q.UpdateDate;
                obj.UpdateUser = q.UpdateUser;
                obj.CreateDate = q.CreateDate;
                obj.CreateUser = q.CreateUser;

                return obj;
            }
        }

        public static InventoryLogNote fnCreateInventoryLogNote(int id)
        {
            InventoryLogNote obj = new InventoryLogNote();
            using (var db = new CMCSQL03Entities())
            {
                obj.productnoteid = -1;
                obj.productmasterid = id;  // important
                obj.reasoncode = null;
                obj.notedate = DateTime.UtcNow;
                obj.notes = null;
                obj.UpdateDate = DateTime.UtcNow;
                obj.UpdateUser = HttpContext.Current.User.Identity.Name;
                obj.CreateDate = DateTime.UtcNow;
                obj.CreateUser = HttpContext.Current.User.Identity.Name;
            }

            return obj;
        }

        public static int fnSaveInventoryLogNote(InventoryLogNote obj)
        {
            using (var db = new CMCSQL03Entities())
            {
                if (obj.productnoteid == -1)
                {
                    var newrec = new tblInvPMLogNote();
                    newrec.CreateDate = DateTime.UtcNow;
                    newrec.CreateUser = HttpContext.Current.User.Identity.Name;

                    db.tblInvPMLogNote.Add(newrec);
                    db.SaveChanges();

                    obj.productnoteid = newrec.InvPMLogNoteIDID;
                }

                var q = (from t in db.tblInvPMLogNote
                         where t.InvPMLogNoteIDID == obj.productnoteid
                         select t).FirstOrDefault();

                q.ProductMasterID = obj.productmasterid;
                q.NoteDate = DateTime.UtcNow;
                q.Notes = obj.notes;
                q.Comment = obj.reasoncode;
                q.UpdateDate = DateTime.UtcNow;
                q.UpdateUser = HttpContext.Current.User.Identity.Name;

                db.SaveChanges();

                return q.InvPMLogNoteIDID;
            }
        }

        public static int fnDeleteProductNote(int id)
        {
            using (var db = new CMCSQL03Entities())
            {
                db.Database.ExecuteSqlCommand("Delete from tblInvPMLogNote Where InvPMLogNoteIDID=" + id);
            }

            return id;
        }

        #endregion Inventory Product Master Log Notes
    }
}