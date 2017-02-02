using MvcPhoenix.EF;
using MvcPhoenix.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace MvcPhoenix.Services
{
    public class ReplenishmentsService
    {
        public static List<BulkOrder> fnSearchResults(FormCollection fc, string mode)
        {
            using (var db = new MvcPhoenix.EF.CMCSQL03Entities())
            {
                var mylist = (from t in db.tblBulkOrder
                              join t2 in db.tblClient on t.ClientID equals t2.ClientID
                              let itemscount = (from items in db.tblBulkOrderItem where (items.BulkOrderID == t.BulkOrderID) select items).Count()
                              let opencount = (from items in db.tblBulkOrderItem where (items.BulkOrderID == t.BulkOrderID) && (items.Status == "OP") select items).Count()
                              orderby t.BulkOrderID descending
                              select new MvcPhoenix.Models.BulkOrder
                              {
                                  bulkorderid = t.BulkOrderID,
                                  clientid = t.ClientID,
                                  supplyid = t.SupplyID,
                                  orderdate = t.OrderDate,
                                  ordercomment = t.Comment,
                                  emailsent = t.EmailSent,
                                  clientname = t2.ClientName,
                                  itemcount = itemscount,
								  opencount = opencount
                              }).ToList();

                if (mode == "User")
                {
                    int myordercount = Convert.ToInt32(fc["ordercount"]);
                    int myclientid = Convert.ToInt32(fc["clientid"]);

                    if (!String.IsNullOrEmpty(fc["ordercount"]))
                    {
                        mylist = (from t in mylist
                                  where t.clientid == myclientid
                                  select t).ToList();
                    }

                    if (!String.IsNullOrEmpty(fc["clientid"]))
                    {
                        mylist = (from t in mylist
                                  select t).Take(myordercount).ToList();
                    }
                }

                // preset request
                string sResultsMessage = null;
                switch (mode)
                {
                    case "Initial":
                        mylist = mylist.Take(10).ToList();
                        sResultsMessage = "Initial";
                        break;

                    case "LastTen":
                        mylist = mylist.Take(10).ToList();
                        sResultsMessage = "Last Ten";
                        break;

                    case "UnConfirmed":
                        mylist = (from t in mylist
                                  where t.emailsent == null
                                  select t).ToList();

                        sResultsMessage = "Un-confirmed";
                        break;

                    case "OpenOnly":
                        mylist = (from t in mylist
                                  where t.opencount > 0
                                  select t).ToList();

                        sResultsMessage = "Open Items";
                        break;

                    default:
                        break;
                }

                foreach (var item in mylist)
                {
                    item.ResultsMessage = sResultsMessage;
                }

                return mylist;
            }
        }

        public static string fnGetSupplyIDEmail(int clientid, string supplyid)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                string retval = "no email on file";
                var q = (from t in db.tblBulkSupplier
                         where t.ClientID == clientid && t.SupplyID == supplyid
                         select t).FirstOrDefault();

                if (q != null)
                {
                    retval = q.Email;
                }

                return retval;
            }
        }

        public static List<BulkOrderItem> fnItemsList(int id)
        {
            using (var db = new MvcPhoenix.EF.CMCSQL03Entities())
            {
                var mylist = (from t in db.tblBulkOrderItem
                              join pm in db.tblProductMaster on t.ProductMasterID equals pm.ProductMasterID
                              where t.BulkOrderID == id
                              select new BulkOrderItem
                              {
                                  mastercode = pm.MasterCode,
                                  mastername = pm.MasterName,
                                  bulkorderitemid = t.BulkOrderItemID,
                                  bulkorderid = t.BulkOrderID,
                                  productmasterid = t.ProductMasterID,
                                  weight = t.Weight,
                                  itemstatus = t.Status,
                                  eta = t.ETA,
                                  datereceived = t.DateReceived,
                                  itemnotes = t.ItemNotes
                              }).ToList();

                return mylist;
            }
        }

        public static BulkOrder fnFillBulkOrderFromDB(int id)
        {
            using (var db = new MvcPhoenix.EF.CMCSQL03Entities())
            {
                BulkOrder obj = new BulkOrder();
                obj = (from t in db.tblBulkOrder
                       join c in db.tblClient on t.ClientID equals c.ClientID
                       where t.BulkOrderID == id
                       select new BulkOrder
                       {
                           bulkorderid = t.BulkOrderID,
                           clientid = t.ClientID,
                           clientname = c.ClientName,
                           orderdate = t.OrderDate,
                           ordercomment = t.Comment,
                           supplyid = t.SupplyID,

                           bulksupplieremail = t.BulkSupplierEmail,
                           emailsent = t.EmailSent,
                       }).FirstOrDefault();

                obj.ListOfSupplyIDs = fnListOfSupplyIDs(obj.clientid);

                obj.ListOfBulkOrderItem = (from oi in db.tblBulkOrderItem
                                           join pm in db.tblProductMaster on oi.ProductMasterID equals pm.ProductMasterID
                                           where oi.BulkOrderID == id
                                           select new BulkOrderItem
                                           {
                                               bulkorderitemid = oi.BulkOrderItemID,
                                               bulkorderid = oi.BulkOrderID,
                                               productmasterid = oi.ProductMasterID,
                                               mastercode = pm.MasterCode,
                                               mastername = pm.MasterName,
                                               weight = oi.Weight,
                                               itemstatus = oi.Status,
                                               eta = oi.ETA,
                                               datereceived = oi.DateReceived,
                                               itemnotes = oi.ItemNotes
                                           }).ToList();

                return obj;
            }
        }

        public static int fnSaveBulkOrder(BulkOrder obj)
        {
            System.Threading.Thread.Sleep(1500);
            using (var db = new MvcPhoenix.EF.CMCSQL03Entities())
            {
                if (obj.bulkorderid == -1)
                {
                    obj.bulkorderid = fnNewBulkOrderID();
                }

                var dbrow = db.tblBulkOrder.Find(obj.bulkorderid);

                dbrow.ClientID = obj.clientid;
                dbrow.OrderDate = obj.orderdate;
                dbrow.Comment = obj.ordercomment;
                dbrow.SupplyID = obj.supplyid;
                dbrow.BulkSupplierEmail = obj.bulksupplieremail;
                dbrow.EmailSent = obj.emailsent;
                db.SaveChanges();

                return obj.bulkorderid;
            }
        }

        public static BulkOrderEmailViewModel fnCreateEmail(BulkOrder vm)
        {
            BulkOrderEmailViewModel message = new BulkOrderEmailViewModel();

            message.bulkorderid = vm.bulkorderid;
            message.clientname = vm.clientname;
            message.ToAddress = vm.bulksupplieremail;
            message.FromAddress = HttpContext.Current.User.Identity.Name;
            message.Subject = "CMC Replenishment Order: " + vm.bulkorderid;

            var q = fnFillBulkOrderFromDB(vm.bulkorderid);
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            s.Append("<div class='table-responsive'><table class='table table-hover table-striped'><thead><tr><th>Mastercode</th><th>Master Name</th><th align='right'>Weight</th></tr></thead>");

            foreach (var item in q.ListOfBulkOrderItem)
            {
                s.Append(String.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>", item.mastercode, item.mastername, item.weight));
            }

            s.Append("</table></div>");

            message.MessageBody = s.ToString();

            return message;
        }

        public static void fnSendEmail(BulkOrderEmailViewModel obj)
        {
            // build table and send email
            var q = fnFillBulkOrderFromDB(obj.bulkorderid);
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            s.Append(String.Format("<p><em>The following message is sent on behalf of {0}</em></p>", obj.FromAddress));
            s.Append("<p>Please send the following items:</p>");
            s.Append("<table width='70%'><tr align='left'><th align='left'>Mastercode</th><th align='left'>Master Name</th><th align='left'>Weight</th></tr>");

            foreach (var item in q.ListOfBulkOrderItem)
            {
                s.Append(String.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>", item.mastercode, item.mastername, item.weight));
            }

            s.Append("</table>");

            obj.MessageBody = s.ToString();

            Thread.Sleep(500);
            ApplicationService.EmailSmtpSend(obj.FromAddress, obj.ToAddress, obj.Subject, obj.MessageBody);

            // update db with email timestamp
            using (var db = new EF.CMCSQL03Entities())
            {
                var dbBulkOrder = db.tblBulkOrder.Find(obj.bulkorderid);
                dbBulkOrder.EmailSent = DateTime.UtcNow.ToString("R");
                db.SaveChanges();
            }
        }

        private static int fnNewBulkOrderID()
        {
            using (var db = new MvcPhoenix.EF.CMCSQL03Entities())
            {
                var newrec = new EF.tblBulkOrder();
                db.tblBulkOrder.Add(newrec);
                db.SaveChanges();

                return newrec.BulkOrderID;
            }
        }

        public static BulkOrderItem fnCreateItem(int id)
        {
            using (var db = new MvcPhoenix.EF.CMCSQL03Entities())
            {
                BulkOrderItem obj = new BulkOrderItem();

                var dbBulkOrder = db.tblBulkOrder.Find(id);

                obj.bulkorderitemid = -1;
                obj.bulkorderid = id;
                obj.productmasterid = null;
                obj.weight = null;
                obj.itemstatus = "OP";
                obj.eta = null;
                obj.datereceived = null;
                obj.itemnotes = null;
                obj.ListOfProductMasters = fnProductMasterIDs(Convert.ToInt32(dbBulkOrder.ClientID));
                obj.ListOfItemStatusIDs = fnOrderItemStatusIDs();

                return obj;
            }
        }

        public static BulkOrderItem fnFillItemFromDB(int id)
        {
            // build object and return
            using (var db = new MvcPhoenix.EF.CMCSQL03Entities())
            {
                BulkOrderItem obj = new BulkOrderItem();
                obj = (from t in db.tblBulkOrderItem
                       where t.BulkOrderItemID == id
                       select new BulkOrderItem
                       {
                           bulkorderitemid = t.BulkOrderItemID,
                           bulkorderid = t.BulkOrderID,
                           productmasterid = t.ProductMasterID,
                           weight = t.Weight,
                           itemstatus = t.Status,
                           eta = t.ETA,
                           datereceived = t.DateReceived,
                           itemnotes = t.ItemNotes,
                       }).FirstOrDefault();

                var dbBulkOrder = db.tblBulkOrder.Find(obj.bulkorderid);
                obj.ListOfProductMasters = fnProductMasterIDs(Convert.ToInt32(dbBulkOrder.ClientID));
                obj.ListOfItemStatusIDs = fnOrderItemStatusIDs();

                return obj;
            }
        }

        public static int fnSaveItem(BulkOrderItem obj)
        {
            System.Threading.Thread.Sleep(1500);
            using (var db = new MvcPhoenix.EF.CMCSQL03Entities())
            {
                if (obj.bulkorderitemid == -1)
                {
                    obj.bulkorderitemid = fnNewBulkOrderItemID();
                }

                var dbrow = (from t in db.tblBulkOrderItem
                             where t.BulkOrderItemID == obj.bulkorderitemid
                             select t).FirstOrDefault();

                dbrow.BulkOrderID = obj.bulkorderid;
                dbrow.ProductMasterID = obj.productmasterid;
                dbrow.Weight = obj.weight;
                dbrow.Status = obj.itemstatus;
                dbrow.ETA = obj.eta;
                dbrow.DateReceived = obj.datereceived;
                dbrow.ItemNotes = obj.itemnotes;
                db.SaveChanges();

                return obj.bulkorderitemid;
            }
        }

        public static int fnDeleteItem(int id)
        {
            using (var db = new MvcPhoenix.EF.CMCSQL03Entities())
            {
                var dbBulkOrder = db.tblBulkOrderItem.Find(id);
                int pk = Convert.ToInt32(dbBulkOrder.BulkOrderID);
                string s = "Delete from tblBulkOrderItem where BulkOrderItemID=" + id;

                db.Database.ExecuteSqlCommand(s);
                db.Dispose();

                return pk;
            }
        }

        private static int fnNewBulkOrderItemID()
        {
            using (var db = new MvcPhoenix.EF.CMCSQL03Entities())
            {
                var newrec = new EF.tblBulkOrderItem();
                db.tblBulkOrderItem.Add(newrec);
                db.SaveChanges();

                return newrec.BulkOrderItemID;
            }
        }

        #region SuggestedBulkOrder ---------------------------------------------------------------------------------

        public static string fnBuildDivisionDropDown(int id)
        {
            using (var db = new CMCSQL03Entities())
            {
                var qry = (from t in db.tblDivision
                           where t.ClientID == id
                           orderby t.DivisionName, t.BusinessUnit
                           select t);

                string s = "<option value='0'>All Divisions</option>";

                if (qry.Count() > 0)
                {
                    foreach (var item in qry)
                    {
                        s = s + "<option value=" + item.DivisionID.ToString() + ">" + item.DivisionName + " - " + item.BusinessUnit + "</option>";
                    }
                }
                else
                {
                    s = s + "<option value=0>No Divisions Found</option>";
                }

                s = s + "</select>";

                return s;
            }
        }

        public static int fnGenerateSuggestedOrder(int clientid, int divisionid)
        {
            // return the number of items created
            int itemscount = 0;

            string username = HttpContext.Current.User.Identity.Name;

            using (var db = new CMCSQL03Entities())
            {
                string s;
                string fnTempTable = "tblSuggestedBulk";

                // Clear the work table of my records
                s = String.Format("Delete from {0} where UserName='{1}'", fnTempTable, username);
                db.Database.ExecuteSqlCommand(s);

                // Build a list of ProductMasters to analyze
                var q1 = (from pd in db.tblProductDetail
                          join pm in db.tblProductMaster on pd.ProductMasterID equals pm.ProductMasterID
                          where pm.ClientID == clientid
                          select new
                          {
                              pm.ClientID,
                              pm.ProductMasterID,
                              pm.SUPPLYID,
                              pm.ShlfLife,
                              pm.ProductSetupDate,
                              pd.DivisionID,
                          }).ToList();

                // Restrict list to a PD.DivisionID if user requested
                if (divisionid > 0)
                {
                    q1 = (from t in q1
                          where t.DivisionID == divisionid
                          select t).ToList();
                }

                // Insert new records into the work table
                foreach (var row in q1)
                {
                    var newrec = new EF.tblSuggestedBulk();
                    newrec.ClientID = row.ClientID;
                    newrec.UserName = username;
                    newrec.ProductMasterID = row.ProductMasterID;
                    newrec.SupplyID = row.SUPPLYID;
                    newrec.ShelfLife = row.ShlfLife;
                    newrec.ProductSetupDate = row.ProductSetupDate;
                    newrec.DivisionID = row.DivisionID;
                    newrec.BulkCurrentAvailable = 0;
                    newrec.ShelfCurrentAvailable = 0;
                    newrec.BulkShippedPastYear = 0;
                    newrec.BulkShippedPerDay = 0;
                    newrec.ShelfShippedPastYear = 0;
                    newrec.ShelfShippedPerDay = 0;
                    newrec.UseThisExpirationDate = null;
                    newrec.AverageLeadTime = 0;
                    newrec.ReorderThis = false;
                    newrec.ReorderWeight = 0;
                    newrec.BulkOnOrder = false;

                    newrec.ProductMasterAge = (DateTime.UtcNow.Date - row.ProductSetupDate.Value).Days;

                    // get the bulk containers and setup critieria
                    string[] sBulkStatus = { "QC", "TEST", "WASTE" };

                    var qBulk = (from t in db.tblBulk
                                 where t.ProductMasterID == row.ProductMasterID
                                 select t).ToList();

                    newrec.BulkCurrentAvailable = (from t in qBulk
                                                   where !sBulkStatus.Contains(t.BulkStatus)
                                                   select ((t.Qty == null ? 1 : t.Qty) * t.CurrentWeight)).Sum();

                    string[] sStockStatus = { "QC", "TEST", "WASTE" };

                    var qStock = (from t in db.tblStock
                                  join sm in db.tblShelfMaster on t.ShelfID equals sm.ShelfID
                                  join pd in db.tblProductDetail on sm.ProductDetailID equals pd.ProductDetailID
                                  join pm in db.tblProductMaster on pd.ProductMasterID equals pm.ProductMasterID
                                  where pm.ProductMasterID == row.ProductMasterID
                                  select new
                                  {
                                      t.ShelfStatus,
                                      t.QtyOnHand,
                                      sm.UnitWeight
                                  }).ToList();

                    newrec.ShelfCurrentAvailable = (from t in qStock
                                                    where !sStockStatus.Contains(t.ShelfStatus)
                                                    select (t.QtyOnHand * t.UnitWeight)).Sum();

                    newrec.CurrentAvailable = newrec.BulkCurrentAvailable + newrec.ShelfCurrentAvailable;

                    // Set BulkShippedPastYear=(Select sum(LogQty*LogAmount) from vwBulkTransForReplenishment where logdate>DateAdd(day,-365,getdate()) and ProductMasterID={0}.ProductMasterID)
                    // may not be working because missing BulkIDs in log
                    var qBulkLog = (from t in db.tblInvLog
                                    join bl in db.tblBulk on t.BulkID equals bl.BulkID
                                    where (bl.ProductMasterID == row.ProductMasterID) && (t.LogType == "BS-SHP")
                                    select new
                                    {
                                        t.LogType,
                                        t.LogDate,
                                        t.LogQty,
                                        t.LogAmount,
                                        bl.BulkID,
                                        bl.BulkStatus
                                    }).ToList();

                    qBulkLog = (from t in qBulkLog
                                where !sBulkStatus.Contains(t.BulkStatus)
                                select t).ToList();

                    qBulkLog = (from t in qBulkLog
                                where t.LogDate > DateTime.UtcNow.AddDays(-365)
                                select t).ToList();

                    newrec.BulkShippedPastYear = (from t in qBulkLog
                                                  select (t.LogQty * t.LogAmount)).Sum();

                    if (newrec.BulkShippedPastYear > 0 && newrec.ProductMasterAge > 365)
                    {
                        newrec.BulkShippedPerDay = newrec.BulkShippedPastYear / 365;
                    }

                    if (newrec.BulkShippedPastYear > 0 && newrec.ProductMasterAge <= 365)
                    {
                        newrec.BulkShippedPerDay = newrec.BulkShippedPastYear / newrec.ProductMasterAge;
                    }

                    var qShelfLog = (from t in db.tblInvLog
                                     join st in db.tblStock on t.StockID equals st.StockID
                                     join sm in db.tblShelfMaster on st.ShelfID equals sm.ShelfID
                                     join pd in db.tblProductDetail on sm.ProductDetailID equals pd.ProductDetailID
                                     where (pd.ProductMasterID == row.ProductMasterID) && (t.LogType == "SS-SHP")
                                     select new
                                     {
                                         t.LogType,
                                         t.LogDate,
                                         t.LogQty,
                                         t.LogAmount,
                                         t.Status
                                     }).ToList();

                    qShelfLog = (from t in qShelfLog
                                 where !sStockStatus.Contains(t.Status)
                                 select t).ToList();

                    qShelfLog = (from t in qShelfLog
                                 where t.LogDate > DateTime.UtcNow.AddDays(-365)
                                 select t).ToList();

                    newrec.ShelfShippedPastYear = (from t in qShelfLog
                                                   select (t.LogQty * t.LogAmount)).Sum();

                    if (newrec.ShelfShippedPastYear > 0 && newrec.ProductMasterAge > 365)
                    {
                        newrec.ShelfShippedPerDay = newrec.ShelfShippedPastYear / 365;
                    }

                    if (newrec.ShelfShippedPastYear > 0 && newrec.ProductMasterAge <= 365)
                    {
                        newrec.ShelfShippedPerDay = newrec.ProductMasterAge / 365;
                    }

                    // Set BulkLatestExpirationDate=(Select top 1 ExpirationDate from tblBulk where ProductMasterID={0}.ProductMasterID order by ExpirationDate Desc)
                    // assumes always at least one bulk container
                    var qBulkExpiration = (from t in db.tblBulk
                                           where t.ProductMasterID == row.ProductMasterID
                                           orderby t.CeaseShipDate descending
                                           select t).Take(1).FirstOrDefault();

                    // set initial value to null, then try to update it, if cannot update set to a future date (below)
                    if (qBulkExpiration != null && qBulkExpiration.CeaseShipDate != null)
                    {
                        newrec.BulkLatestExpirationDate = qBulkExpiration.CeaseShipDate;
                        newrec.BulkDaysTilExpiration = (newrec.BulkLatestExpirationDate.Value - DateTime.UtcNow.Date).Days;

                        newrec.ShelfLatestExpirationDate = qBulkExpiration.CeaseShipDate;
                        newrec.ShelfDaysTilExpiration = (newrec.ShelfLatestExpirationDate.Value - DateTime.UtcNow.Date).Days;

                        newrec.UseThisExpirationDate = newrec.BulkLatestExpirationDate;
                    }

                    if (newrec.UseThisExpirationDate == null)
                    {
                        newrec.UseThisExpirationDate = new DateTime(2099, 01, 01, 0, 0, 0);
                    }

                    newrec.UseThisDaysTilExpiration = (newrec.UseThisExpirationDate.Value - DateTime.UtcNow.Date).Days;

                    if (newrec.UseThisDaysTilExpiration > 998)
                    {
                        newrec.UseThisDaysTilExpiration = 999;
                    }

                    // Set DaysSupplyLeft= CurrentAvailable / (ShelfShippedPerDay+BulkShippedPerDay) Where (ShelfShippedPerDay+BulkShippedPerDay>0)
                    if ((newrec.ShelfShippedPerDay + newrec.BulkShippedPerDay) > 0)
                    {
                        newrec.DaysSupplyLeft = Convert.ToInt32(newrec.CurrentAvailable / (newrec.ShelfShippedPerDay + newrec.BulkShippedPerDay));
                    }

                    // Set DaysSupplyLeft=0 Where DaysSupplyLeft Is Null
                    if (newrec.DaysSupplyLeft == null)
                    {
                        newrec.DaysSupplyLeft = 0;
                    }

                    var qBulkOrders = (from t in db.tblBulkOrderItem
                                       where t.ProductMasterID == row.ProductMasterID && (t.Status == "OP" || t.Status == "OPEN")
                                       select t.BulkOrderItemID).ToList();

                    if (qBulkOrders.Count() > 0)
                    {
                        newrec.BulkOnOrder = true;
                    }
                    else
                    {
                        newrec.BulkOnOrder = false;
                    }

                    if (newrec.DaysSupplyLeft < 65)
                    {
                        newrec.ReorderThis = true;
                    }

                    if (newrec.UseThisDaysTilExpiration < 65)
                    {
                        newrec.ReorderThis = true;
                    }

                    if (newrec.ShelfShippedPerDay + newrec.BulkShippedPerDay == 0)
                    {
                        newrec.ReorderThis = false;
                    }

                    if (newrec.BulkOnOrder == true)
                    {
                        newrec.ReorderThis = false;
                    }

                    // Set ReorderWeight=Round((ShelfShippedPerDay+BulkShippedPerDay) * 120,0) Where ShelfLife<13 and ReorderThis=1
                    if (newrec.ShelfLife < 13 && newrec.ReorderThis == true)
                    {
                        newrec.ReorderWeight = Convert.ToInt32((newrec.ShelfShippedPerDay + newrec.BulkShippedPerDay) * 120);
                    }

                    // Set ReorderWeight=Round((ShelfShippedPerDay+BulkShippedPerDay) * 180,0) Where (ShelfLife>=13 or ShelfLife is null) and (ReorderThis=1)
                    if (newrec.ShelfLife >= 13 || newrec.ShelfLife == null)
                    {
                        newrec.ReorderWeight = Convert.ToInt32((newrec.ShelfShippedPerDay + newrec.BulkShippedPerDay)) * 180;
                    }

                    // Set ReorderWeight=1 Where (ReorderWeight<1) and (ReorderThis=1)
                    if (newrec.ReorderWeight < 1 && newrec.ReorderThis == true)
                    {
                        newrec.ReorderWeight = 1;
                    }

                    db.tblSuggestedBulk.Add(newrec);
                    db.SaveChanges();
                }

                s = String.Format("Delete from {0} where ReorderThis=0 and Username='{1}'", fnTempTable, username);

                db.Database.ExecuteSqlCommand(s);

                itemscount = (from t in db.tblSuggestedBulk
                              where t.UserName == username
                              select t).Count();

                return itemscount;
            }
        }

        public static List<SuggestedBulkOrderItem> fnSuggestedItemsList()
        {
            using (var db = new MvcPhoenix.EF.CMCSQL03Entities())
            {
                string username = HttpContext.Current.User.Identity.Name;
                var mylist = (from t in db.tblSuggestedBulk
                              join dv in db.tblDivision on t.DivisionID equals dv.DivisionID into m
                              from x in m.DefaultIfEmpty()
                              join c in db.tblClient on t.ClientID equals c.ClientID
                              join pm in db.tblProductMaster on t.ProductMasterID equals pm.ProductMasterID
                              where t.UserName == username
                              select new SuggestedBulkOrderItem
                              {
                                  id = t.id,
                                  clientid = t.ClientID,
                                  productmasterid = t.ProductMasterID,
                                  divisionid = t.DivisionID,
                                  supplyid = t.SupplyID,
                                  reorderweight = t.ReorderWeight,
                                  reordernotes = t.ReorderNotes,
                                  bulkshippedperday = t.BulkShippedPerDay,
                                  shelfshippedperday = t.ShelfShippedPerDay,
                                  usethisdaystilexpiration = t.UseThisDaysTilExpiration,
                                  averageleadtime = t.AverageLeadTime,
                                  username = t.UserName,
                                  clientname = c.ClientName,
                                  mastercode = pm.MasterCode,
                                  mastername = pm.MasterName,
                                  division = x.DivisionName
                              }).ToList();

                mylist = (from t in mylist
                          orderby t.mastercode, t.supplyid
                          select t).ToList();

                return mylist;
            }
        }

        public static void fnDeleteSuggestedItem(int id)
        {
            using (var db = new MvcPhoenix.EF.CMCSQL03Entities())
            {
                db.Database.ExecuteSqlCommand("Delete From tblSuggestedBulk Where ID=" + id);
            }
        }

        public static SuggestedBulkOrderItem fnFillSuggestedItemfromDB(int id)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                SuggestedBulkOrderItem obj = new SuggestedBulkOrderItem();

                obj = (from t in db.tblSuggestedBulk
                       where t.id == id
                       orderby t.ProductMasterID
                       select new SuggestedBulkOrderItem
                       {
                           id = t.id,
                           clientid = t.ClientID,
                           productmasterid = t.ProductMasterID,
                           supplyid = t.SupplyID,
                           reorderweight = t.ReorderWeight,
                           reordernotes = t.ReorderNotes
                       }).FirstOrDefault();

                obj.ListOfProductMasters = fnProductMasterIDs(Convert.ToInt32(obj.clientid));

                return obj;
            }
        }

        public static SuggestedBulkOrderItem fnCreateSuggestedBulkOrderItem(int ClientID)
        {
            // a new suggested item needs the client id to get started
            SuggestedBulkOrderItem obj = new SuggestedBulkOrderItem();
            obj.id = -1;    // important
            obj.clientid = ClientID;
            obj.ListOfProductMasters = ReplenishmentsService.fnProductMasterIDs(obj.clientid);

            return obj;
        }

        public static int fnSaveSuggestedItem(SuggestedBulkOrderItem obj)
        {
            using (var db = new MvcPhoenix.EF.CMCSQL03Entities())
            {
                if (obj.id == -1)
                {
                    obj.id = fnNewSuggestedOrderItemID();
                }

                var dbrow = db.tblSuggestedBulk.Find(obj.id);

                if (obj.reorderweight == null)
                {
                    obj.reorderweight = 0;
                }

                dbrow.ClientID = obj.clientid;
                dbrow.ProductMasterID = obj.productmasterid;
                dbrow.ReorderWeight = obj.reorderweight;
                dbrow.ReorderNotes = obj.reordernotes;

                var dbMaster = db.tblProductMaster.Find(obj.productmasterid);
                dbrow.SupplyID = dbMaster.SUPPLYID;

                dbrow.UserName = HttpContext.Current.User.Identity.Name;

                db.SaveChanges();

                return obj.id;
            }
        }

        private static int fnNewSuggestedOrderItemID()
        {
            using (var db = new MvcPhoenix.EF.CMCSQL03Entities())
            {
                var newrec = new EF.tblSuggestedBulk();
                db.tblSuggestedBulk.Add(newrec);
                db.SaveChanges();

                return newrec.id;
            }
        }

        public static int fnCreateBulkOrders()
        {
            using (var db = new MvcPhoenix.EF.CMCSQL03Entities())
            {
                string username = HttpContext.Current.User.Identity.Name;

                string fnTempTable = "tblSuggestedBulk";
                string s = String.Format("Update {0} set SupplyID='n/a' where Supplyid is null and UserName='{1}'", fnTempTable, username);
                db.Database.ExecuteSqlCommand(s);

                var qry = (from t in db.tblSuggestedBulk
                           where t.UserName == username
                           select new { t.ClientID, t.SupplyID }).Distinct();

                int SupplyIDCount = qry.Count();
                DateTime myOrderDate = DateTime.UtcNow;
                string BatchNumber = myOrderDate.ToString();
                foreach (var item in qry)
                {
                    using (var db1 = new MvcPhoenix.EF.CMCSQL03Entities())
                    {
                        var newitem = new EF.tblBulkOrder
                        {
                            ClientID = item.ClientID,
                            OrderDate = myOrderDate,
                            Status = "OP",
                            SupplyID = item.SupplyID
                        };

						var bs = (from x in db.tblBulkSupplier 
                                  where x.ClientID == item.ClientID && x.SupplyID == item.SupplyID 
                                  select x).FirstOrDefault();
                        
                        if(bs != null)
                        {
                            newitem.BulkSupplierEmail = bs.Email;
                        }

                        db1.tblBulkOrder.Add(newitem);

                        db1.SaveChanges();

                        int newpk = newitem.BulkOrderID;
                        //// now create order items records
                        s = String.Format("Insert into tblBulkOrderItem (BulkOrderID,ProductMasterID,Qty,Weight,Status,SupplyID,ItemNotes) Select {0},ProductMasterID,1,ReorderWeight,'OP',SupplyID,ReorderNotes from {1} where SupplyID='{2}'", newpk, fnTempTable, item.SupplyID);
                        db1.Database.ExecuteSqlCommand(s);
                    }
                }

                int OrdersCount = qry.Count();
                s = String.Format("Delete From {0} where UserName='{1}'", fnTempTable, username);
                db.Database.ExecuteSqlCommand(s);

                return OrdersCount;
            }
        }

        #endregion SuggestedBulkOrder ---------------------------------------------------------------------------------

        // ********************************* Support methods

        //TODO fix private vs public

        public static List<SelectListItem> fnClientIDs()
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> mylist = new List<SelectListItem>();

                mylist = (from t in db.tblClient
                          orderby t.ClientName
                          select new SelectListItem
                          {
                              Value = t.ClientID.ToString(),
                              Text = t.ClientName
                          }).ToList();

                mylist.Insert(0, new SelectListItem { Value = "0", Text = "Select Client" });

                return mylist;
            }
        }

        private static List<SelectListItem> fnListOfSupplyIDs(int? clientid)
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> mylist = new List<SelectListItem>();

                mylist = (from t in db.tblProductMaster
                          where t.ClientID == clientid
                          orderby t.SUPPLYID
                          select
                          new SelectListItem
                          {
                              Value = t.SUPPLYID,
                              Text = t.SUPPLYID
                          }).Distinct().ToList();

                mylist.Insert(0, new SelectListItem { Value = "0", Text = "" });

                return mylist;
            }
        }

        public static List<SelectListItem> fnProductMasterIDs(int? clientid)
        {
            // 06/13/2016 This now is a list of PD-PN records
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> mylist = new List<SelectListItem>();

                mylist = (from t in db.tblProductMaster
                          where t.ClientID == clientid
                          orderby t.MasterCode
                          select new SelectListItem
                          {
                              Value = t.ProductMasterID.ToString(),
                              Text = t.MasterCode + " - " + t.MasterName.Substring(0, 25)
                          }).ToList();

                mylist.Insert(0, new SelectListItem { Value = "0", Text = "Master Code" });

                return mylist;
            }
        }

        private static List<SelectListItem> fnOrderItemStatusIDs()
        {
            List<SelectListItem> mylist = new List<SelectListItem>();

            mylist.Insert(0, new SelectListItem { Value = "0", Text = "Please Select" });
            mylist.Insert(1, new SelectListItem { Value = "CL", Text = "Closed" });
            mylist.Insert(2, new SelectListItem { Value = "OP", Text = "Open" });
            mylist.Insert(3, new SelectListItem { Value = "CN", Text = "Cancelled" });

            return mylist;
        }

        public static List<SelectListItem> fnDivisionIDs(int clientid)
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> mylist = new List<SelectListItem>();

                mylist = (from c in db.tblDivision
                          where c.ClientID == clientid
                          select new SelectListItem
                          {
                              Value = c.DivisionID.ToString(),
                              Text = c.DivisionName
                          }).ToList();

                mylist.Insert(0, new SelectListItem { Value = "0", Text = "Division" });

                return mylist;
            }
        }
    }
}