using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

//pc add
using System.Web.Mvc;
using MvcPhoenix.EF;
using MvcPhoenix.Models;


namespace MvcPhoenix.Services
{
    public class ReplenishmentsService
    {
        private static string PathToLogos="http://www.mysamplecenter.com/Logos/";

        public static List<BulkOrderSearchResults> fnSearchResults(FormCollection fc,string mode)
        {

            using (var db = new MvcPhoenix.EF.CMCSQL03Entities())
            {
                var mylist = (from t in db.tblBulkOrder
                              join t2 in db.tblClient on t.ClientID equals t2.ClientID
                              let itemscount = (from items in db.tblBulkOrderItem where items.BulkOrderID == t.BulkOrderID select items).Count()
                              let opencount = (from items in db.tblBulkOrderItem where (items.BulkOrderID == t.BulkOrderID) && (items.Status == "OP") select items).Count()
                              orderby t.BulkOrderID descending
                              select new MvcPhoenix.Models.BulkOrderSearchResults
                              {
                                  bulkorderid = t.BulkOrderID,
                                  clientid = t.ClientID,
                                  supplyid = t.SupplyID,
                                  orderdate = t.OrderDate,
                                  comment = t.Comment,
                                  emailsent = t.EmailSent,
                                  clientname = t2.ClientName,
                                  itemcount = itemscount,
                                  ResultsMessage="Results"
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
                    return mylist;
                }

                // preset request
                string sResultsMessage=null;
                switch (mode)
                {
                    case "Initial":
                        mylist = mylist.Take(8).ToList();
                        sResultsMessage = "Initial";
                        //mylist = mylist.Select(e => { e.ResultsMessage = "Initial"; return e; });
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
                var dbrow = (from t in db.tblBulkSupplier where t.ClientID == clientid && t.SupplyID == supplyid select new { t.Email }).FirstOrDefault();
                if (!string.IsNullOrEmpty(dbrow.Email))
                {
                    retval = dbrow.Email;
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
                                  mastercode=pm.MasterCode,
                                  mastername=pm.MasterName,
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
            using(var db = new MvcPhoenix.EF.CMCSQL03Entities())
            {
                BulkOrder obj = new BulkOrder();
                obj = (from t in db.tblBulkOrder
                           join c in db.tblClient on t.ClientID equals c.ClientID
                           where t.BulkOrderID == id
                           select new BulkOrder
                           {
                               bulkorderid = t.BulkOrderID,
                               clientid = t.ClientID,
                               clientname =c.ClientName,
                               logofilename = PathToLogos + c.LogoFileName,
                               orderdate = t.OrderDate,
                               ordercomment = t.Comment,
                               supplyid = t.SupplyID,
                
                               bulksupplieremail = t.BulkSupplierEmail,
                               emailsent = t.EmailSent,
                           }).FirstOrDefault();

                obj.ListOfSupplyIDs = fnListOfSupplyIDs(obj.clientid);
                
                obj.ListOfBulkOrderItem = (from oi in db.tblBulkOrderItem
                                           where oi.BulkOrderID == id
                                           select new BulkOrderItem
                                           {
                                               bulkorderitemid = oi.BulkOrderItemID,
                                               bulkorderid = oi.BulkOrderID,
                                               productmasterid = oi.ProductMasterID,
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
                if(obj.bulkorderid==-1)
                {
                    obj.bulkorderid = fnNewBulkOrderID();
                }
                var dbrow= db.tblBulkOrder.Find(obj.bulkorderid);
                dbrow.ClientID = obj.clientid;
                dbrow.OrderDate=obj.orderdate;
                dbrow.Comment=obj.ordercomment;
                dbrow.SupplyID = obj.supplyid;
                dbrow.BulkSupplierEmail=obj.bulksupplieremail;
                dbrow.EmailSent=obj.emailsent;
                db.SaveChanges();
                return obj.bulkorderid;
            }
        }

        public static BulkOrderEmailViewModel fnCreateEmail(BulkOrder obj)
        {
            // build obj and return it
            BulkOrderEmailViewModel message = new BulkOrderEmailViewModel();

            message.bulkorderid = obj.bulkorderid;
            message.clientname = obj.clientname;
            message.logofilename = obj.logofilename;
            message.FromAddress = "philc@usdevelopers.com";
            message.ToAddress = obj.bulksupplieremail;
            message.Subject = "Replenishment Order";
            //message.MessageBody = EmailBody(obj.bulkorderid).ToHtmlString();
            //message.MessageBody = "<table><tr><td>Mastercode</td><td>Master Name</td><td>Weight</td></tr>";
            message.MessageBody = "When completed this will contain a greeting and list of order items";
            return message;
        }


         public static void fnSendEmail(BulkOrderEmailViewModel obj)
        {
             // update and SMTP
            using (var db = new EF.CMCSQL03Entities())
            { 
                var dbBulkOrder = db.tblBulkOrder.Find(obj.bulkorderid);
                dbBulkOrder.EmailSent = String.Format("{0:dd MMM yyyy HH:mm:ss}", DateTime.Now.ToString());
                db.SaveChanges();
                //ApplicationService.fnSimpleSendSmtp(obj.FromAddress, obj.ToAddress, obj.Subject, obj.MessageBody);
            }
        }



         private static MvcHtmlString EmailBody(int id)
         {
             using (var db = new EF.CMCSQL03Entities())
             {
                 //var dbBulkOrder = db.tblBulkOrder.Find(id);
                 List<BulkOrderItem> mylist = ReplenishmentsService.fnItemsList(id);
                 //var myitems=(from t in db.tblBulkOrderItem where t.BulkOrderID==obj.bulkorderid select t).ToList();
                 System.Text.StringBuilder sb = new System.Text.StringBuilder();
                 sb.Append("<table><tr><td>Mastercode</td><td>Master Name</td><td>Weight</td></tr>");
                 //string sb = "";
                 //sb = sb + string.Format("{0} {1} {2}", "MasterCode", "Name", "Weight") + "<br>";
                 foreach (var item in mylist)
                 {
                     sb.Append("<tr><td>" + item.mastername + "</td><td>" + item.mastername + "</td><td>" + item.weight.ToString() + "</td></tr>");
                     //    sb = sb + string.Format("{0} {1} {2}", item.mastercode, item.mastername, item.weight) + "<br>";
                 }
                 sb.Append("</table>");
                 var str = MvcHtmlString.Create(sb.ToString());
                 //return str.ToHtmlString();            
                 return str;
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
                var dbrow = (from t in db.tblBulkOrderItem where t.BulkOrderItemID == obj.bulkorderitemid select t).FirstOrDefault();
                dbrow.BulkOrderID = obj.bulkorderid;
                dbrow.ProductMasterID =obj.productmasterid;
                dbrow.Weight=obj.weight;
                dbrow.Status=obj.itemstatus;
                dbrow.ETA=obj.eta;
                dbrow.DateReceived=obj.datereceived;
                dbrow.ItemNotes=obj.itemnotes;
                db.SaveChanges();
                return obj.bulkorderitemid;
            }
        }

        public static int fnDeleteItem(int id)
        {
            using (var db = new MvcPhoenix.EF.CMCSQL03Entities())
            {
                var dbBulkOrder=db.tblBulkOrderItem.Find(id);
                int pk = Convert.ToInt32(dbBulkOrder.BulkOrderID);
                string s = "Delete from tblBulkOrderItem where BulkOrderItemID=" + id;
                db.Database.ExecuteSqlCommand(s); db.Dispose();
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
                var qry = (from t in db.tblDivision where t.ClientID == id  orderby t.Division, t.BusinessUnit select t);
                string s = "<option value='0' selected=true>Select Division</option>";
                s = s + "<option value='0'>All Divisions</option>";
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

        public static int fnGenerateSuggestedOrder(SuggestedBulkOrder obj)
        {
            // return the number of items created
            //System.Diagnostics.Debug.WriteLine(s);
            //MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            int itemscount = 0;

            // change this later to user the identity prop
            string username = "philc";

            using (var db = new CMCSQL03Entities())
            {
                string s;
                string fnTempTable = "tblSuggestedBulk";
                //string sSessionID = HttpContext.Current.Session["MySessionID"].ToString();
                //string sSessionID = HttpContext.Current.Session.SessionID.ToString();
                //string sSessionID = "hello";

                //s = "Delete from " + fnTempTable;
                s = "Delete from " + fnTempTable + " where UserName='" + username + "'";
                db.Database.ExecuteSqlCommand(s);

                s = "Insert into " + fnTempTable + "(ClientID, ProductMasterID,SUPPLYID, ShelfLife, CreateDate,MasterDivisionID)";
                s = s + " SELECT ClientID, ProductMasterID,SUPPLYID,ShlfLife,CreateDate,MasterDivisionID from tblProductMaster";
                s = s + " Where ClientID=" + obj.clientid;

                if (obj.divisionid > 0)
                { s = s + " and MasterDivisionID='" + obj.divisionid + "'"; }
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set UserName='" + username + "'";
                db.Database.ExecuteSqlCommand(s);

                //s = "Update " + fnTempTable + " Set SessionID='" + sSessionID + "'";
                //db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set BulkOnOrder=0";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set ProductMasterAge=DateDiff(day,createdate,getdate())";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set BulkCurrentavailable=(Select isnull(Sum(  isnull(qty,1) *CurrentWeight),0) from tblBulk where ProductMasterID=" + fnTempTable + ".ProductMasterID and BulkStatus not in('QC','TEST','WASTE'))";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set ShelfCurrentAvailable=(Select isnull(Sum(TotalShelfWeight),0) from vwStockWeightsForReOrder where ProductMasterID=" + fnTempTable + ".ProductMasterID and ShelfStatus not in('QC','TEST','WASTE'))";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set CurrentAvailable=IsNull(BulkCurrentAvailable,0)+IsNull(ShelfCurrentAvailable,0)";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set BulkShippedPastYear=(Select sum(TransQty*TransAmount) from vwInvTransBulk";
                s = s + " Where TransType='B02' and Status not in('QC','TEST','WASTE') and transdate>DateAdd(day,-365,getdate())";
                s = s + " And ProductMasterID=" + fnTempTable + ".ProductMasterID)";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set BulkShippedPastYear=0 where BulkShippedPastYear is Null";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set BulkShippedPerDay=bulkShippedPastYear/365 where BulkShippedPastYear>0 and ProductMasterAge>365";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set BulkShippedPerDay=bulkShippedPastYear/ProductMasterAge where BulkShippedpastYear>0 and ProductMasterAge<=365";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set BulkShippedPerDay=0 Where BulkShippedPerDay is null";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set ShelfShippedPastYear=(Select isnull(Sum(TransQty*TransAmount),0) from vwInvTransShelf";
                s = s + " Where Status not in('QC','TEST','WASTE') and TransType IN('S04') and transdate>DateAdd(day,-365,getdate()) And ProductMasterID=" + fnTempTable + ".ProductMasterID)";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set ShelfShippedPastYear=0 where ShelfShippedPastYear is null";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set ShelfShippedPerDay=ShelfShippedPastYear/365 Where ShelfShippedPastYear>0 and ProductMasterAge>365";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set ShelfShippedPerDay=ShelfShippedPastYear/ProductMasterAge Where ShelfShippedPastYear>0 and ProductMasterAge<=365";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set ShelfShippedPerDay=0 Where ShelfShippedPerDay Is Null";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set BulkLatestExpirationDate=(Select top 1 ExpirationDate from tblBulk where ProductMasterID=" + fnTempTable + ".ProductMasterID order by ExpirationDate Desc)";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set BulkDaysTilExpiration=DateDiff(day, Getdate(), BulkLatestExpirationDate)";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set ShelfLatestExpirationDate=(Select Max(expirationdate) from vwExpirationForReorder where ProductMasterid=" + fnTempTable + ".ProductMasterID)";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set ShelfDaysTilExpiration=DateDiff(day, Getdate(), ShelfLatestExpirationDate)";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set UseThisExpirationDate=IIF(BulkLatestExpirationDate>ShelfLatestExpirationDate,BulkLatestExpirationDate,ShelfLatestExpirationDate)";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set UseThisExpirationDate=BulkLatestExpirationDate where UseThisExpirationDate is null and ShelfLatestExpirationDate is null";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set UseThisExpirationDate=ShelfLatestExpirationDate where UseThisExpirationDate is null and BulkLatestExpirationDate is null";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set UseThisExpirationDate='2099-01-01' where UseThisExpirationDate is null";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set UseThisDaysTilExpiration=datediff(day,getdate(),UseThisExpirationDate)";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set UseThisDaysTilExpiration=999 where UseThisDaysTilExpiration>998";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set DaysSupplyLeft= CurrentAvailable / (ShelfShippedPerDay+BulkShippedPerDay) Where (ShelfShippedPerDay+BulkShippedPerDay>0)";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set DaysSupplyLeft=0 Where DaysSupplyLeft Is Null";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set AverageLeadTime=0";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set ReorderThis=0,ReorderWeight=0";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set BulkOnOrder=(Select Case When (Select Count(*) from tblBulkOrderItem Where ProductMasterID=" + fnTempTable + ".ProductMasterID and Status in('OP','OPEN'))>0 then 1 Else 0 End)";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set ReorderThis=1 Where DaysSupplyLeft<65";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set ReorderThis=1 Where UseThisDaysTilExpiration<65";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set ReorderThis=0 Where (ShelfShippedPerDay)+(BulkShippedPerDay)=0";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set ReorderThis=0 Where BulkOnOrder=1";
                db.Database.ExecuteSqlCommand(s);

                s = "Delete from " + fnTempTable + " where ReorderThis=0";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set ReorderWeight=Round((ShelfShippedPerDay+BulkShippedPerDay) * 120,0) Where ShelfLife<13 and ReorderThis=1";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set ReorderWeight=Round((ShelfShippedPerDay+BulkShippedPerDay) * 180,0) Where (ShelfLife>=13 or ShelfLife is null) and (ReorderThis=1)";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set ReorderWeight=1 Where (ReorderWeight<1)  and (ReorderThis=1)";
                db.Database.ExecuteSqlCommand(s);

                itemscount = db.tblSuggestedBulk.Count();
                return itemscount;
            }
            
            
        }
        
        public static List<SuggestedBulkOrderItem> Org_fnSuggestedItemsList()
        {
            using (var db = new MvcPhoenix.EF.CMCSQL03Entities())
            {
                // *****************************************************
                // NOTE
                string username = "philc"; // replace later with identity

                var mylist = (from t in db.tblSuggestedBulk
                              where t.UserName==username
                              join t2 in db.tblProductMaster on t.ProductMasterID equals t2.ProductMasterID
                              join t3 in db.tblDivision on t2.MasterDivisionID equals t3.DivisionID
                              join c in db.tblClient on t.ClientID equals c.ClientID
                              orderby t2.MasterCode, t2.SUPPLYID
                              select new SuggestedBulkOrderItem
                              {
                                  id = t.id,
                                  username=t.UserName,
                                  clientid = t.ClientID,
                                  clientname = c.ClientName,
                                  logofilename = c.LogoFileName,
                                  productmasterid = t.ProductMasterID,
                                  mastercode = t2.MasterCode,
                                  division = t3.Division,
                                  mastername = t2.MasterName,
                                  supplyid = t.SupplyID,
                                  reorderweight = t.ReorderWeight,
                                  bulkshippedperday = t.BulkShippedPerDay,
                                  shelfshippedperday = t.ShelfShippedPerDay,
                                  usethisdaystilexpiration = t.UseThisDaysTilExpiration,
                                  averageleadtime = t.AverageLeadTime,
                                  reordernotes = t.ReorderNotes
                              }).ToList();
                return mylist;
            }
        }


        public static List<SuggestedBulkOrderItem> fnSuggestedItemsList()
        {
            using (var db = new MvcPhoenix.EF.CMCSQL03Entities())
            {
                // *****************************************************
                // NOTE
                string username = "philc"; // replace later with identity

                var mylist = (from t in db.tblSuggestedBulk
                              join c in db.tblClient on t.ClientID equals c.ClientID
                              join pm in db.tblProductMaster on t.ProductMasterID equals pm.ProductMasterID
                              where t.UserName == username
                              select new SuggestedBulkOrderItem
                              {
                                  id = t.id,
                                  clientid = t.ClientID,
                                  productmasterid = t.ProductMasterID,
                                  masterdivisionid=t.MasterDivisionID,
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
                                  division = "No Division"
                              }).ToList();
                foreach(var item in mylist)
                {
                    var dbDivision = (from t in db.tblDivision where t.DivisionID == item.masterdivisionid select t).FirstOrDefault();
                    if (dbDivision == null)
                    {

                    }
                    else
                    {
                        item.division = dbDivision.Division;
                    }
                }
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
            using(var db = new EF.CMCSQL03Entities())
            {
                SuggestedBulkOrderItem obj = new SuggestedBulkOrderItem();
                obj = (from t in db.tblSuggestedBulk
                              //join t2 in db.tblProductMaster on t.ProductMasterID equals t2.ProductMasterID
                              //join t3 in db.tblDivision on t2.MasterDivisionID equals t3.DivisionID
                     where t.id == id
                     orderby t.ProductMasterID
                     select new SuggestedBulkOrderItem {
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
            //obj.username = "philc";    // replace later with identity string 
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

                var dbDivision = db.tblDivision.Find(dbMaster.MasterDivisionID);
                if (dbDivision == null)
                { }
                else
                { dbrow.MasterDivisionID = dbDivision.DivisionID; }

                //TODO change later
                // always overwrite
                dbrow.UserName = "philc";

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
             
             //TODO using
             //TODO add username filter
             
             // convert to identity
             string username = "philc";

             string s;
             string fnTempTable = "tblSuggestedBulk";
             MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
             s = "Update " + fnTempTable + " set SupplyID='n/a' where Supplyid is null";
             s = s + " and UserName='" + username + "'";
             db.Database.ExecuteSqlCommand(s);

             var qry = (from t in db.tblSuggestedBulk
                        where t.UserName==username
                        select new { t.ClientID, t.SupplyID }).Distinct();
             int SupplyIDCount = qry.Count();
             DateTime myOrderDate = DateTime.Now;
             string BatchNumber = myOrderDate.ToString();
             string sSessionID = HttpContext.Current.Session.SessionID;

             MvcPhoenix.EF.CMCSQL03Entities db1 = new MvcPhoenix.EF.CMCSQL03Entities();

             // For each distinct supplyid
             foreach (var item in qry)
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
                 //System.Diagnostics.Debug.WriteLine("New OrderID= " + newpk.ToString());
                 //// now create order items records
                 s = "Insert into tblBulkOrderItem (BulkOrderID,ProductMasterID,Qty,Weight,Status,SupplyID,ItemNotes)";
                 s = s + " Select " + newpk + ",ProductMasterID,1,ReorderWeight,'OP',SupplyID,ReorderNotes from " + fnTempTable;
                 s = s + " Where Supplyid='" + item.SupplyID + "'";
                 //System.Diagnostics.Debug.WriteLine(s);
                 db1.Database.ExecuteSqlCommand(s);
             }
             int OrdersCount = qry.Count();
             db1.Database.ExecuteSqlCommand("Delete From tblSuggestedBulk where UserName='" + username + "'");
             db1.Dispose();
             db.Dispose();
             return OrdersCount;
         }


        #endregion --------------------------------------------------------------------------------------



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