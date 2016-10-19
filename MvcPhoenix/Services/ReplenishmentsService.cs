using MvcPhoenix.EF;
using MvcPhoenix.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
                              }).ToList();

                if (mode == "User")
                {
                    int myordercount = Convert.ToInt32(fc["ordercount"]);
                    int myclientid = Convert.ToInt32(fc["clientid"]);
                    if (!String.IsNullOrEmpty(fc["ordercount"]))
                    {
                        mylist = (from t in mylist where t.clientid == myclientid select t).ToList();
                    }
                    if (!String.IsNullOrEmpty(fc["clientid"]))
                    {
                        mylist = (from t in mylist select t).Take(myordercount).ToList();
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
                        mylist = (from t in mylist where t.emailsent == null select t).ToList();
                        sResultsMessage = "Un-confirmed";
                        break;

                    case "OpenOnly":
                        mylist = (from t in mylist where t.opencount > 0 select t).ToList();
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
                var q = (from t in db.tblBulkSupplier where t.ClientID == clientid && t.SupplyID == supplyid select t).FirstOrDefault();
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
                           logofilename = c.LogoFileName,
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
            // build obj and return it
            BulkOrderEmailViewModel message = new BulkOrderEmailViewModel();

            message.bulkorderid = vm.bulkorderid;
            message.clientname = vm.clientname;
            message.logofilename = vm.logofilename;
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
                dbBulkOrder.EmailSent = String.Format("{0:dd MMM yyyy HH:mm:ss}", DateTime.Now.ToString());
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
                
                //var dbrow = db.tblBulkOrderItem.Find(obj.bulkorderitemid);
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
                var qry = (from t in db.tblDivision where t.ClientID == id orderby t.Division, t.BusinessUnit select t);
                //string s = "<option value='0' selected=true>Select Division</option>";
                string s = "<option value='0'>All Divisions</option>";
                if (qry.Count() > 0)
                {
                    foreach (var item in qry)
                    {
                        s = s + "<option value=" + item.DivisionID.ToString() + ">" + item.Division + " - " + item.BusinessUnit + "</option>";
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
                s = "Delete from " + fnTempTable + " where UserName='" + username + "'";
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
                              pm.CreateDate,
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
                    newrec.CreateDate = row.CreateDate;
                    newrec.DivisionID = row.DivisionID;

                    db.tblSuggestedBulk.Add(newrec);
                    db.SaveChanges();
                }

                s = String.Format("Update {0} Set BulkOnOrder=0 where Username='{1}'", fnTempTable, username);
                System.Diagnostics.Debug.WriteLine(s);
                db.Database.ExecuteSqlCommand(s);

                s = string.Format("Update {0} Set ProductMasterAge=DateDiff(day,createdate,getdate()) where Username='{1}'", fnTempTable, username);
                System.Diagnostics.Debug.WriteLine(s);
                db.Database.ExecuteSqlCommand(s);

                s = String.Format("Update {0} Set BulkCurrentavailable=(Select isnull(Sum( isnull(qty,1) * CurrentWeight),0) from tblBulk where ProductMasterID={0}.ProductMasterID and Username='{1}' and BulkStatus not in('QC','TEST','WASTE'))", fnTempTable, username);
                System.Diagnostics.Debug.WriteLine(s);
                db.Database.ExecuteSqlCommand(s);

                //vwShelfWeightForReplenishment
                s = String.Format("Update {0} Set ShelfCurrentAvailable=(Select isnull(Sum(QtyOnHand*UnitWeight),0) from vwShelfStockForReplenishment where ProductMasterID={0}.ProductMasterID) where Username='{1}'", fnTempTable, username);
                System.Diagnostics.Debug.WriteLine(s);
                db.Database.ExecuteSqlCommand(s);

                s = String.Format("Update {0} Set CurrentAvailable=IsNull(BulkCurrentAvailable,0)+IsNull(ShelfCurrentAvailable,0) Where Username='{1}'", fnTempTable, username);
                System.Diagnostics.Debug.WriteLine(s);
                db.Database.ExecuteSqlCommand(s);

                s = String.Format("Update {0} Set BulkShippedPastYear=(Select sum(LogQty*LogAmount) from vwBulkTransForReplenishment where logdate>DateAdd(day,-365,getdate()) and ProductMasterID={0}.ProductMasterID) where Username='{1}'", fnTempTable, username);
                System.Diagnostics.Debug.WriteLine(s);
                db.Database.ExecuteSqlCommand(s);

                s = String.Format("Update {0} Set BulkShippedPastYear=0 where BulkShippedPastYear is Null and Username='{1}'", fnTempTable, username);
                System.Diagnostics.Debug.WriteLine(s);
                db.Database.ExecuteSqlCommand(s);

                s = String.Format("Update {0} Set BulkShippedPerDay=bulkShippedPastYear/365 where BulkShippedPastYear>0 and ProductMasterAge>365 and Username='{1}'", fnTempTable, username);
                System.Diagnostics.Debug.WriteLine(s);
                db.Database.ExecuteSqlCommand(s);

                s = String.Format("Update {0} Set BulkShippedPerDay=bulkShippedPastYear/ProductMasterAge where BulkShippedpastYear>0 and ProductMasterAge<=365 and Username='{1}'", fnTempTable, username);
                System.Diagnostics.Debug.WriteLine(s);
                db.Database.ExecuteSqlCommand(s);

                s = String.Format("Update {0} Set BulkShippedPerDay=0 Where BulkShippedPerDay is null And Username='{1}'", fnTempTable, username);
                System.Diagnostics.Debug.WriteLine(s);
                db.Database.ExecuteSqlCommand(s);

                s = String.Format("Update {0} Set ShelfShippedPastYear=(Select isnull(Sum(LogQty*LogAmount),0) from vwShelfTransForReplenishment Where logdate>DateAdd(day,-365,getdate()) And ProductMasterID={0}.ProductMasterID) where UserName='{1}'", fnTempTable, username);
                System.Diagnostics.Debug.WriteLine(s);
                db.Database.ExecuteSqlCommand(s);

                s = String.Format("Update {0} Set ShelfShippedPastYear=0 where ShelfShippedPastYear is null and Username='{1}'", fnTempTable, username);
                System.Diagnostics.Debug.WriteLine(s);
                db.Database.ExecuteSqlCommand(s);

                s = String.Format("Update {0} Set ShelfShippedPerDay=ShelfShippedPastYear/365 Where ShelfShippedPastYear>0 and ProductMasterAge>365 And Username='{1}'", fnTempTable, username);
                System.Diagnostics.Debug.WriteLine(s);
                db.Database.ExecuteSqlCommand(s);

                s = String.Format("Update {0} Set ShelfShippedPerDay=ShelfShippedPastYear/ProductMasterAge Where ShelfShippedPastYear>0 and ProductMasterAge<=365 And Username='{1}'", fnTempTable, username);
                System.Diagnostics.Debug.WriteLine(s);
                db.Database.ExecuteSqlCommand(s);

                s = String.Format("Update {0} Set ShelfShippedPerDay=0 Where ShelfShippedPerDay Is Null and Username='{1}'", fnTempTable, username);
                System.Diagnostics.Debug.WriteLine(s);
                db.Database.ExecuteSqlCommand(s);

                s = String.Format("Update {0} Set BulkLatestExpirationDate=(Select top 1 ExpirationDate from tblBulk where ProductMasterID={0}.ProductMasterID order by ExpirationDate Desc) Where Username='{1}'", fnTempTable, username);
                System.Diagnostics.Debug.WriteLine(s);
                db.Database.ExecuteSqlCommand(s);

                s = String.Format("Update {0} Set BulkDaysTilExpiration=DateDiff(day, Getdate(), BulkLatestExpirationDate) where Username='{1}'", fnTempTable, username);
                System.Diagnostics.Debug.WriteLine(s);
                db.Database.ExecuteSqlCommand(s);

                s = String.Format("Update {0} Set ShelfLatestExpirationDate=(Select top 1 ExpirationDate from tblBulk where ProductMasterID={0}.ProductMasterID order by ExpirationDate Desc) Where Username='{1}'", fnTempTable, username);
                System.Diagnostics.Debug.WriteLine(s);
                db.Database.ExecuteSqlCommand(s);

                s = String.Format("Update {0} Set ShelfDaysTilExpiration=DateDiff(day, Getdate(), ShelfLatestExpirationDate) Where Username='{1}'", fnTempTable, username);
                System.Diagnostics.Debug.WriteLine(s);
                db.Database.ExecuteSqlCommand(s);

                s = String.Format("Update {0} Set UseThisExpirationDate=IIF(BulkLatestExpirationDate>ShelfLatestExpirationDate,BulkLatestExpirationDate,ShelfLatestExpirationDate) Where Username='{1}'", fnTempTable, username);
                System.Diagnostics.Debug.WriteLine(s);
                db.Database.ExecuteSqlCommand(s);

                s = String.Format("Update {0} Set UseThisExpirationDate=BulkLatestExpirationDate where UseThisExpirationDate is null and ShelfLatestExpirationDate is null And Username='{1}'", fnTempTable, username);
                System.Diagnostics.Debug.WriteLine(s);
                db.Database.ExecuteSqlCommand(s);

                s = String.Format("Update {0} Set UseThisExpirationDate=ShelfLatestExpirationDate where UseThisExpirationDate is null and BulkLatestExpirationDate is null And Username='{1}'", fnTempTable, username);
                System.Diagnostics.Debug.WriteLine(s);
                db.Database.ExecuteSqlCommand(s);

                s = String.Format("Update {0} Set UseThisExpirationDate='2099-01-01' where UseThisExpirationDate is null and Username='{1}'", fnTempTable, username);
                System.Diagnostics.Debug.WriteLine(s);
                db.Database.ExecuteSqlCommand(s);

                s = String.Format("Update {0} Set UseThisDaysTilExpiration=datediff(day,getdate(),UseThisExpirationDate) where Username='{1}'", fnTempTable, username);
                System.Diagnostics.Debug.WriteLine(s);
                db.Database.ExecuteSqlCommand(s);

                s = String.Format("Update {0} Set UseThisDaysTilExpiration=999 where UseThisDaysTilExpiration>998 and Username='{1}'", fnTempTable, username);
                System.Diagnostics.Debug.WriteLine(s);
                db.Database.ExecuteSqlCommand(s);

                s = String.Format("Update {0} Set DaysSupplyLeft= CurrentAvailable / (ShelfShippedPerDay+BulkShippedPerDay) Where (ShelfShippedPerDay+BulkShippedPerDay>0) And Username='{1}'", fnTempTable, username);
                System.Diagnostics.Debug.WriteLine(s);
                db.Database.ExecuteSqlCommand(s);

                s = String.Format("Update {0} Set DaysSupplyLeft=0 Where DaysSupplyLeft Is Null and Username='{1}'", fnTempTable, username);
                System.Diagnostics.Debug.WriteLine(s);
                db.Database.ExecuteSqlCommand(s);

                s = String.Format("Update {0} Set AverageLeadTime=0 where Username='{1}'", fnTempTable, username);
                System.Diagnostics.Debug.WriteLine(s);
                db.Database.ExecuteSqlCommand(s);

                s = String.Format("Update {0} Set ReorderThis=0,ReorderWeight=0 where Username='{1}'", fnTempTable, username);
                System.Diagnostics.Debug.WriteLine(s);
                db.Database.ExecuteSqlCommand(s);

                s = String.Format("Update {0} Set BulkOnOrder=(Select Case When (Select Count(*) from tblBulkOrderItem Where ProductMasterID={0}.ProductMasterID and Status in('OP','OPEN'))>0 then 1 Else 0 End) where username='{1}'", fnTempTable, username);
                System.Diagnostics.Debug.WriteLine(s);
                db.Database.ExecuteSqlCommand(s);

                s = String.Format("Update {0} Set ReorderThis=1 Where DaysSupplyLeft<65 and Username='{1}'", fnTempTable, username);
                System.Diagnostics.Debug.WriteLine(s);
                db.Database.ExecuteSqlCommand(s);

                s = String.Format("Update {0} Set ReorderThis=1 Where UseThisDaysTilExpiration<65 and Username='{1}'", fnTempTable, username);
                System.Diagnostics.Debug.WriteLine(s);
                db.Database.ExecuteSqlCommand(s);

                s = String.Format("Update {0} Set ReorderThis=0 Where (ShelfShippedPerDay)+(BulkShippedPerDay)=0 and Username='{1}'", fnTempTable, username);
                System.Diagnostics.Debug.WriteLine(s);
                db.Database.ExecuteSqlCommand(s);

                s = String.Format("Update {0} Set ReorderThis=0 Where BulkOnOrder=1 and Username='{1}'", fnTempTable, username);
                System.Diagnostics.Debug.WriteLine(s);
                db.Database.ExecuteSqlCommand(s);

                s = String.Format("Delete from {0} where ReorderThis=0 and Username='{1}'", fnTempTable, username);
                System.Diagnostics.Debug.WriteLine(s);
                db.Database.ExecuteSqlCommand(s);

                s = String.Format("Update {0} Set ReorderWeight=Round((ShelfShippedPerDay+BulkShippedPerDay) * 120,0) Where ShelfLife<13 and ReorderThis=1 and Username='{1}'", fnTempTable, username);
                System.Diagnostics.Debug.WriteLine(s);
                db.Database.ExecuteSqlCommand(s);

                s = String.Format("Update {0} Set ReorderWeight=Round((ShelfShippedPerDay+BulkShippedPerDay) * 180,0) Where (ShelfLife>=13 or ShelfLife is null) and (ReorderThis=1) and Username='{1}'", fnTempTable, username);
                System.Diagnostics.Debug.WriteLine(s);
                db.Database.ExecuteSqlCommand(s);

                s = String.Format("Update {0} Set ReorderWeight=1 Where (ReorderWeight<1)  and (ReorderThis=1) and Username='{1}'", fnTempTable, username);
                System.Diagnostics.Debug.WriteLine(s);
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
                                  logofilename = c.LogoFileName,
                                  mastercode = pm.MasterCode,
                                  mastername = pm.MasterName,
                                  division = x.Division
                              }).ToList();

                mylist = (from t in mylist orderby t.mastercode, t.supplyid select t).ToList();
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
                       //join t2 in db.tblProductMaster on t.ProductMasterID equals t2.ProductMasterID
                       //join t3 in db.tblDivision on t2.MasterDivisionID equals t3.DivisionID
                       where t.id == id
                       orderby t.ProductMasterID
                       select new SuggestedBulkOrderItem
                       {
                           id = t.id,
                           clientid = t.ClientID,
                           productmasterid = t.ProductMasterID,
                           supplyid = t.SupplyID,
                           reorderweight = t.ReorderWeight,
                           //username=t.UserName,
                           //mastercode = t2.MasterCode,
                           //division = t3.Division,
                           //mastername = t2.MasterName,
                           //bulkshippedperday = t.BulkShippedPerDay,
                           //shelfshippedperday = t.ShelfShippedPerDay,
                           //usethisdaystilexpiration = t.UseThisDaysTilExpiration,
                           //averageleadtime = t.AverageLeadTime,
                           reordernotes = t.ReorderNotes
                       }).FirstOrDefault();
                obj.ListOfProductMasters = fnProductMasterIDs(Convert.ToInt32(obj.clientid));
                //obj.ListOfSupplyIDs = fnListOfSupplyIDs(obj.clientid);
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
            //System.Threading.Thread.Sleep(1500);
            using (var db = new MvcPhoenix.EF.CMCSQL03Entities())
            {
                if (obj.id == -1)
                {
                    obj.id = fnNewSuggestedOrderItemID();
                }
                var dbrow = db.tblSuggestedBulk.Find(obj.id);

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
                DateTime myOrderDate = DateTime.Now;
                string BatchNumber = myOrderDate.ToString();
                //string sSessionID = HttpContext.Current.Session.SessionID;
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
                          select new SelectListItem { Value = t.ClientID.ToString(), Text = t.ClientName }).ToList();
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
                          new SelectListItem { Value = t.SUPPLYID, Text = t.SUPPLYID }).Distinct().ToList();
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
                          select
                              new SelectListItem { Value = t.ProductMasterID.ToString(), Text = t.MasterCode + " - " + t.MasterName.Substring(0, 25) }).ToList();

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
                mylist = (from c in db.tblDivision where c.ClientID == clientid select new SelectListItem { Value = c.DivisionID.ToString(), Text = c.Division }).ToList();
                mylist.Insert(0, new SelectListItem { Value = "0", Text = "Division" });
                return mylist;
            }
        }
    }
}