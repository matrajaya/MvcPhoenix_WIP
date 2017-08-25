using MvcPhoenix.EF;
using MvcPhoenix.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace MvcPhoenix.Services
{
    public class ReplenishmentsService
    {
        public static List<BulkOrder> fnSearchResults(FormCollection form, string mode)
        {
            using (var db = new CMCSQL03Entities())
            {
                var mylist = (from t in db.tblBulkOrder
                              join t2 in db.tblClient on t.ClientID equals t2.ClientID
                              let itemscount = (from items in db.tblBulkOrderItem
                                                where (items.BulkOrderID == t.BulkOrderID)
                                                select items).Count()
                              let opencount = (from items in db.tblBulkOrderItem
                                               where (items.BulkOrderID == t.BulkOrderID)
                                               && (items.Status == "OP")
                                               select items).Count()
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
                    int myordercount = Convert.ToInt32(form["ordercount"]);
                    int myclientid = Convert.ToInt32(form["clientid"]);

                    if (!String.IsNullOrEmpty(form["ordercount"]))
                    {
                        mylist = (from t in mylist
                                  where t.clientid == myclientid
                                  select t).ToList();
                    }

                    if (!String.IsNullOrEmpty(form["clientid"]))
                    {
                        mylist = (from t in mylist
                                  select t).Take(myordercount).ToList();
                    }
                }

                // preset request
                string message = null;
                switch (mode)
                {
                    case "Initial":
                        mylist = mylist.Take(10).ToList();
                        message = "Initial";
                        break;

                    case "LastTen":
                        mylist = mylist.Take(10).ToList();
                        message = "Last Ten";
                        break;

                    case "UnConfirmed":
                        mylist = (from t in mylist
                                  where t.emailsent == null
                                  select t).ToList();

                        message = "Un-confirmed";
                        break;

                    case "OpenOnly":
                        mylist = (from t in mylist
                                  where t.opencount > 0
                                  select t).ToList();

                        message = "Open Items";
                        break;

                    default:
                        break;
                }

                foreach (var item in mylist)
                {
                    item.ResultsMessage = message;
                }

                return mylist;
            }
        }

        public static string fnGetSupplyIDEmail(int clientid, string supplyid)
        {
            using (var db = new CMCSQL03Entities())
            {
                string retval = "no email on file";
                var q = (from t in db.tblBulkSupplier
                         where t.ClientID == clientid
                         && t.SupplyID == supplyid
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
            using (var db = new CMCSQL03Entities())
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
            using (var db = new CMCSQL03Entities())
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
                                               itemnotes = oi.ItemNotes,
                                               PrepackedBulk = pm.PrePacked
                                           }).ToList();

                return obj;
            }
        }

        public static int fnSaveBulkOrder(BulkOrder obj)
        {
            System.Threading.Thread.Sleep(1500);

            using (var db = new CMCSQL03Entities())
            {
                if (obj.bulkorderid == -1)
                {
                    obj.bulkorderid = fnNewBulkOrderID();
                }

                var bulkOrder = db.tblBulkOrder.Find(obj.bulkorderid);

                bulkOrder.ClientID = obj.clientid;
                bulkOrder.OrderDate = obj.orderdate;
                bulkOrder.Comment = obj.ordercomment;
                bulkOrder.SupplyID = obj.supplyid;
                bulkOrder.BulkSupplierEmail = obj.bulksupplieremail;
                bulkOrder.EmailSent = obj.emailsent;

                db.SaveChanges();

                return obj.bulkorderid;
            }
        }

        public static BulkOrderEmailViewModel fnCreateEmail(BulkOrder vm)
        {
            var bulkOrder = fnFillBulkOrderFromDB(vm.bulkorderid);

            BulkOrderEmailViewModel message = new BulkOrderEmailViewModel();
            message.bulkorderid = vm.bulkorderid;
            message.clientname = vm.clientname;
            message.ToAddress = vm.bulksupplieremail;
            message.FromAddress = HttpContext.Current.User.Identity.Name;
            message.Subject = "CMC Replenishment Order: " + vm.bulkorderid;

            StringBuilder builder = new StringBuilder();
            builder.Append("<div>");
            builder.Append("<p><b>Order Note: </b>" + vm.ordercomment + "</p>");
            builder.Append("</div>");

            builder.Append("<div class='table-responsive'>");
            builder.Append("<table class='table table-hover table-striped'>");

            builder.Append("<thead>");
            builder.Append("<tr>");
            builder.Append("<th>Mastercode</th>");
            builder.Append("<th>Master Name</th>");
            builder.Append("<th>Weight</th>");
            builder.Append("<th>Prepacked Bulk?</th>");
            builder.Append("<th align='right'>Notes</th>");
            builder.Append("</tr>");
            builder.Append("</thead>");

            foreach (var item in bulkOrder.ListOfBulkOrderItem)
            {
                builder.Append("<tr>");
                builder.Append("<td>item.mastercode</td>");
                builder.Append("<td>item.mastername</td>");
                builder.Append("<td>item.weight</td>");
                builder.Append("<td>item.PrepackedBulk</td>");
                builder.Append("<td>item.itemnotes</td>");
                builder.Append("</tr>");
            }

            builder.Append("</table>");
            builder.Append("</div>");

            message.MessageBody = builder.ToString();

            return message;
        }

        public static void fnSendEmail(BulkOrderEmailViewModel obj)
        {
            // build table and send email
            var bulkOrder = fnFillBulkOrderFromDB(obj.bulkorderid);
            var shipTo = @"Chemical Marketing Concepts Europe<br/>
                           Industrieweg 73<br/>
                           5145 PD Waalwijk<br/>
                           The Netherlands<br/>
                           +31 (0)416-651977";

            StringBuilder builder = new StringBuilder();
            builder.Append("<p><em>The following message is sent on behalf of " + obj.FromAddress + "</em></p>");

            builder.Append("<p>Hello,</p>");
            builder.Append("<p>Please find below our new replenishment order.</p>");
            builder.Append("<p>Thank you for your feedback.</p>");
            builder.Append("<p>Kind regards,<br/>Inventory Team<br/>Chemical Marketing Concepts</p>");
            builder.Append("<hr style='border: 1px solid black'/>");

            builder.Append("<p><b>Order Number: </b>" + obj.bulkorderid + "<br/>");
            builder.Append("<b>Order Date: </b>" + DateTime.UtcNow.ToShortDateString() + "<br/></p>");
            builder.Append("<p><b>Ship To:</b><br/>" + shipTo + "</p>");

            builder.Append("<p><b>Order Note: </p>");
            builder.Append("<p>" + bulkOrder.ordercomment + "</p>");
            builder.Append("<p>Please send the following items:</p>");

            builder.Append("<table width='70%' style='border: 1px solid black'>");
            builder.Append("<tr align='left' bgcolor='#428BCA' style='color:white;'>");
            builder.Append("<th align='left'>Product Code</th>");
            builder.Append("<th align='left'>Product Name</th>");
            builder.Append("<th align='center'>Weight (KG)</th>");
            builder.Append("<th>Prepacked Bulk?</th>");
            builder.Append("<th align='left'>Notes</th></tr>");

            foreach (var item in bulkOrder.ListOfBulkOrderItem)
            {
                builder.Append("<tr>");
                builder.Append("<td>" + item.mastercode + "</td>");
                builder.Append("<td>" + item.mastername + "</td>");
                builder.Append("<td align='center'>" + item.weight + "</td>");
                builder.Append("<td>" + item.PrepackedBulk + "</td>");
                builder.Append("<td>" + item.itemnotes + "</td>");
                builder.Append("</tr>");
            }

            builder.Append("</table>");
            builder.Append("<br/>");
            builder.Append("<hr style='border: 1px solid black'/>");

            obj.MessageBody = builder.ToString();

            Thread.Sleep(500);
            EmailServices.EmailSmtpSend(obj.FromAddress, obj.ToAddress, obj.Subject, obj.MessageBody);

            // update db with email timestamp
            using (var db = new CMCSQL03Entities())
            {
                var dbBulkOrder = db.tblBulkOrder.Find(obj.bulkorderid);
                dbBulkOrder.EmailSent = DateTime.UtcNow.ToString("R");
                db.SaveChanges();
            }
        }

        private static int fnNewBulkOrderID()
        {
            using (var db = new CMCSQL03Entities())
            {
                var newrec = new tblBulkOrder();
                db.tblBulkOrder.Add(newrec);
                db.SaveChanges();

                return newrec.BulkOrderID;
            }
        }

        public static BulkOrderItem fnCreateItem(int id)
        {
            using (var db = new CMCSQL03Entities())
            {
                BulkOrderItem bulkOrderItem = new BulkOrderItem();

                var dbBulkOrder = db.tblBulkOrder.Find(id);

                bulkOrderItem.bulkorderitemid = -1;
                bulkOrderItem.bulkorderid = id;
                bulkOrderItem.productmasterid = null;
                bulkOrderItem.weight = null;
                bulkOrderItem.itemstatus = "OP";
                bulkOrderItem.eta = null;
                bulkOrderItem.datereceived = null;
                bulkOrderItem.itemnotes = null;
                bulkOrderItem.ListOfProductMasters = ApplicationService.ddlProductMasterIDs(Convert.ToInt32(dbBulkOrder.ClientID));

                return bulkOrderItem;
            }
        }

        public static BulkOrderItem fnFillItemFromDB(int id)
        {
            // build object and return
            using (var db = new CMCSQL03Entities())
            {
                BulkOrderItem bulkItem = new BulkOrderItem();
                bulkItem = (from t in db.tblBulkOrderItem
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

                var dbBulkOrder = db.tblBulkOrder.Find(bulkItem.bulkorderid);
                bulkItem.ListOfProductMasters = ApplicationService.ddlProductMasterIDs(Convert.ToInt32(dbBulkOrder.ClientID));

                return bulkItem;
            }
        }

        public static int fnSaveItem(BulkOrderItem obj)
        {
            System.Threading.Thread.Sleep(1500);
            using (var db = new CMCSQL03Entities())
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

        public static int fnDeleteItem(int bulkOrderItemId)
        {
            using (var db = new CMCSQL03Entities())
            {
                var dbBulkOrder = db.tblBulkOrderItem.Find(bulkOrderItemId);
                int bulkOrderId = Convert.ToInt32(dbBulkOrder.BulkOrderID);

                string deleteQuery = "DELETE FROM tblBulkOrderItem WHERE BulkOrderItemID=" + bulkOrderItemId;
                db.Database.ExecuteSqlCommand(deleteQuery);

                return bulkOrderId;
            }
        }

        private static int fnNewBulkOrderItemID()
        {
            using (var db = new CMCSQL03Entities())
            {
                var bulkItem = new tblBulkOrderItem();
                db.tblBulkOrderItem.Add(bulkItem);
                db.SaveChanges();

                return bulkItem.BulkOrderItemID;
            }
        }

        #region SuggestedBulkOrder ---------------------------------------------------------------------------------

        public static int fnGenerateSuggestedOrder(int clientid, int divisionid)
        {
            // return the number of items created
            int itemsCount = 0;
            string tempTable = "tblSuggestedBulk";
            string userName = HttpContext.Current.User.Identity.Name;
            string deleteQuery = "DELETE FROM " + tempTable + " WHERE UserName=" + userName;

            using (var db = new CMCSQL03Entities())
            {
                // Clear the work table of my records
                db.Database.ExecuteSqlCommand(deleteQuery);

                // Build a list of ProductMasters to analyze
                var products = (from pd in db.tblProductDetail
                                join pm in db.tblProductMaster on pd.ProductMasterID equals pm.ProductMasterID
                                where pm.ClientID == clientid
                                select new
                                {
                                    pm.ClientID,
                                    pm.ProductMasterID,
                                    pm.SUPPLYID,
                                    pm.ShelfLife,
                                    pm.ProductSetupDate,
                                    pd.DivisionID,
                                }).ToList();

                // Restrict list to a PD.DivisionID if user requested
                if (divisionid > 0)
                {
                    products = (from t in products
                                where t.DivisionID == divisionid
                                select t).ToList();
                }

                // Insert new records into the work table
                foreach (var item in products)
                {
                    tblSuggestedBulk suggestedBulk = new tblSuggestedBulk();
                    suggestedBulk.ClientID = item.ClientID;
                    suggestedBulk.UserName = userName;
                    suggestedBulk.ProductMasterID = item.ProductMasterID;
                    suggestedBulk.SupplyID = item.SUPPLYID;
                    suggestedBulk.ShelfLife = item.ShelfLife;
                    suggestedBulk.ProductSetupDate = item.ProductSetupDate;
                    suggestedBulk.DivisionID = item.DivisionID;
                    suggestedBulk.BulkCurrentAvailable = 0;
                    suggestedBulk.ShelfCurrentAvailable = 0;
                    suggestedBulk.BulkShippedPastYear = 0;
                    suggestedBulk.BulkShippedPerDay = 0;
                    suggestedBulk.ShelfShippedPastYear = 0;
                    suggestedBulk.ShelfShippedPerDay = 0;
                    suggestedBulk.UseThisExpirationDate = null;
                    suggestedBulk.AverageLeadTime = 0;
                    suggestedBulk.ReorderThis = false;
                    suggestedBulk.ReorderWeight = 0;
                    suggestedBulk.BulkOnOrder = false;

                    if (!item.ProductSetupDate.HasValue)
                    {
                        suggestedBulk.ProductMasterAge = 7;
                    }
                    else
                    {
                        suggestedBulk.ProductMasterAge = (DateTime.UtcNow.Date - item.ProductSetupDate.Value).Days;
                    }

                    // get the bulk containers and setup critieria
                    string[] bulkStatus = { "QC", "TEST", "WASTE" };

                    var bulks = (from t in db.tblBulk
                                 where t.ProductMasterID == item.ProductMasterID
                                 select t).ToList();

                    suggestedBulk.BulkCurrentAvailable = (from t in bulks
                                                          where !bulkStatus.Contains(t.BulkStatus)
                                                          select ((t.Qty == null ? 1 : t.Qty) * t.CurrentWeight)).Sum();

                    string[] stockStatus = { "QC", "TEST", "WASTE" };

                    var stock = (from t in db.tblStock
                                  join sm in db.tblShelfMaster on t.ShelfID equals sm.ShelfID
                                  join pd in db.tblProductDetail on sm.ProductDetailID equals pd.ProductDetailID
                                  join pm in db.tblProductMaster on pd.ProductMasterID equals pm.ProductMasterID
                                  where pm.ProductMasterID == item.ProductMasterID
                                  select new
                                  {
                                      t.ShelfStatus,
                                      t.QtyOnHand,
                                      sm.UnitWeight
                                  }).ToList();

                    suggestedBulk.ShelfCurrentAvailable = (from t in stock
                                                           where !stockStatus.Contains(t.ShelfStatus)
                                                           select (t.QtyOnHand * t.UnitWeight)).Sum();

                    suggestedBulk.CurrentAvailable = suggestedBulk.BulkCurrentAvailable + suggestedBulk.ShelfCurrentAvailable;

                    var qBulkLog = (from t in db.tblInvLog
                                    join bl in db.tblBulk on t.BulkID equals bl.BulkID
                                    where (bl.ProductMasterID == item.ProductMasterID)
                                    && (t.LogType == "BS-SHP")
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
                                where !bulkStatus.Contains(t.BulkStatus)
                                select t).ToList();

                    qBulkLog = (from t in qBulkLog
                                where t.LogDate > DateTime.UtcNow.AddDays(-365)
                                select t).ToList();

                    suggestedBulk.BulkShippedPastYear = (from t in qBulkLog
                                                         select (t.LogQty * t.LogAmount)).Sum();

                    if (suggestedBulk.BulkShippedPastYear > 0 && suggestedBulk.ProductMasterAge > 365)
                    {
                        suggestedBulk.BulkShippedPerDay = suggestedBulk.BulkShippedPastYear / 365;
                    }

                    if (suggestedBulk.BulkShippedPastYear > 0 && suggestedBulk.ProductMasterAge <= 365)
                    {
                        suggestedBulk.BulkShippedPerDay = suggestedBulk.BulkShippedPastYear / suggestedBulk.ProductMasterAge;
                    }

                    var qShelfLog = (from t in db.tblInvLog
                                     join st in db.tblStock on t.StockID equals st.StockID
                                     join sm in db.tblShelfMaster on st.ShelfID equals sm.ShelfID
                                     join pd in db.tblProductDetail on sm.ProductDetailID equals pd.ProductDetailID
                                     where (pd.ProductMasterID == item.ProductMasterID)
                                     && (t.LogType == "SS-SHP")
                                     select new
                                     {
                                         t.LogType,
                                         t.LogDate,
                                         t.LogQty,
                                         t.LogAmount,
                                         t.Status
                                     }).ToList();

                    qShelfLog = (from t in qShelfLog
                                 where !stockStatus.Contains(t.Status)
                                 select t).ToList();

                    qShelfLog = (from t in qShelfLog
                                 where t.LogDate > DateTime.UtcNow.AddDays(-365)
                                 select t).ToList();

                    suggestedBulk.ShelfShippedPastYear = (from t in qShelfLog
                                                          select (t.LogQty * t.LogAmount)).Sum();

                    if (suggestedBulk.ShelfShippedPastYear > 0 && suggestedBulk.ProductMasterAge > 365)
                    {
                        suggestedBulk.ShelfShippedPerDay = suggestedBulk.ShelfShippedPastYear / 365;
                    }

                    if (suggestedBulk.ShelfShippedPastYear > 0 && suggestedBulk.ProductMasterAge <= 365)
                    {
                        suggestedBulk.ShelfShippedPerDay = suggestedBulk.ProductMasterAge / 365;
                    }

                    var qBulkExpiration = (from t in db.tblBulk
                                           where t.ProductMasterID == item.ProductMasterID
                                           orderby t.CeaseShipDate descending
                                           select t).Take(1).FirstOrDefault();

                    // set initial value to null, then try to update it, if cannot update set to a future date (below)
                    if (qBulkExpiration != null && qBulkExpiration.CeaseShipDate != null)
                    {
                        suggestedBulk.BulkLatestExpirationDate = qBulkExpiration.CeaseShipDate;
                        suggestedBulk.BulkDaysTilExpiration = (suggestedBulk.BulkLatestExpirationDate.Value - DateTime.UtcNow.Date).Days;
                        suggestedBulk.ShelfLatestExpirationDate = qBulkExpiration.CeaseShipDate;
                        suggestedBulk.ShelfDaysTilExpiration = (suggestedBulk.ShelfLatestExpirationDate.Value - DateTime.UtcNow.Date).Days;
                        suggestedBulk.UseThisExpirationDate = suggestedBulk.BulkLatestExpirationDate;
                    }

                    if (suggestedBulk.UseThisExpirationDate == null)
                    {
                        suggestedBulk.UseThisExpirationDate = new DateTime(2099, 01, 01, 0, 0, 0);
                    }

                    suggestedBulk.UseThisDaysTilExpiration = (suggestedBulk.UseThisExpirationDate.Value - DateTime.UtcNow.Date).Days;

                    if (suggestedBulk.UseThisDaysTilExpiration > 998)
                    {
                        suggestedBulk.UseThisDaysTilExpiration = 999;
                    }

                    if ((suggestedBulk.ShelfShippedPerDay + suggestedBulk.BulkShippedPerDay) > 0)
                    {
                        suggestedBulk.DaysSupplyLeft = Convert.ToInt32(suggestedBulk.CurrentAvailable / (suggestedBulk.ShelfShippedPerDay + suggestedBulk.BulkShippedPerDay));
                    }

                    if (suggestedBulk.DaysSupplyLeft == null)
                    {
                        suggestedBulk.DaysSupplyLeft = 0;
                    }

                    var qBulkOrders = (from t in db.tblBulkOrderItem
                                       where t.ProductMasterID == item.ProductMasterID
                                       && (t.Status == "OP" || t.Status == "OPEN")
                                       select t.BulkOrderItemID).ToList();

                    if (qBulkOrders.Count() > 0)
                    {
                        suggestedBulk.BulkOnOrder = true;
                    }
                    else
                    {
                        suggestedBulk.BulkOnOrder = false;
                    }

                    if (suggestedBulk.DaysSupplyLeft < 65)
                    {
                        suggestedBulk.ReorderThis = true;
                    }

                    if (suggestedBulk.UseThisDaysTilExpiration < 65)
                    {
                        suggestedBulk.ReorderThis = true;
                    }

                    if (suggestedBulk.ShelfShippedPerDay + suggestedBulk.BulkShippedPerDay == 0)
                    {
                        suggestedBulk.ReorderThis = false;
                    }

                    if (suggestedBulk.BulkOnOrder == true)
                    {
                        suggestedBulk.ReorderThis = false;
                    }

                    if (suggestedBulk.ShelfLife < 13 && suggestedBulk.ReorderThis == true)
                    {
                        suggestedBulk.ReorderWeight = Convert.ToInt32((suggestedBulk.ShelfShippedPerDay + suggestedBulk.BulkShippedPerDay) * 120);
                    }

                    if (suggestedBulk.ShelfLife >= 13 || suggestedBulk.ShelfLife == null)
                    {
                        suggestedBulk.ReorderWeight = Convert.ToInt32((suggestedBulk.ShelfShippedPerDay + suggestedBulk.BulkShippedPerDay) * 180);
                    }

                    if (suggestedBulk.ReorderWeight < 1 && suggestedBulk.ReorderThis == true)
                    {
                        suggestedBulk.ReorderWeight = 1;
                    }

                    db.tblSuggestedBulk.Add(suggestedBulk);
                    db.SaveChanges();
                }

                deleteQuery = deleteQuery + " AND ReorderThis=0";
                db.Database.ExecuteSqlCommand(deleteQuery);

                itemsCount = (from t in db.tblSuggestedBulk
                              where t.UserName == userName
                              select t).Count();

                return itemsCount;
            }
        }

        public static List<SuggestedBulkOrderItem> fnSuggestedItemsList()
        {
            using (var db = new CMCSQL03Entities())
            {
                string userName = HttpContext.Current.User.Identity.Name;
                var suggestedItems = (from t in db.tblSuggestedBulk
                                      join division in db.tblDivision on t.DivisionID equals division.DivisionID into m
                                      from x in m.DefaultIfEmpty()
                                      join client in db.tblClient on t.ClientID equals client.ClientID
                                      join product in db.tblProductMaster on t.ProductMasterID equals product.ProductMasterID
                                      where t.UserName == userName
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
                                          clientname = client.ClientName,
                                          mastercode = product.MasterCode,
                                          mastername = product.MasterName,
                                          division = x.DivisionName
                                      }).ToList();

                return suggestedItems;
            }
        }

        public static void fnDeleteSuggestedItem(int suggestedBulkId)
        {
            using (var db = new CMCSQL03Entities())
            {
                string deleteQuery = "DELETE FROM tblSuggestedBulk WHERE ID=" + suggestedBulkId;
                db.Database.ExecuteSqlCommand(deleteQuery);
            }
        }

        public static SuggestedBulkOrderItem fnFillSuggestedItemfromDB(int suggestBulkId)
        {
            using (var db = new CMCSQL03Entities())
            {
                SuggestedBulkOrderItem suggestedBulk = new SuggestedBulkOrderItem();

                suggestedBulk = (from t in db.tblSuggestedBulk
                       where t.id == suggestBulkId
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

                return suggestedBulk;
            }
        }

        public static SuggestedBulkOrderItem fnCreateSuggestedBulkOrderItem(int clientId)
        {
            // a new suggested item needs the client id to get started
            SuggestedBulkOrderItem obj = new SuggestedBulkOrderItem();
            obj.id = -1;
            obj.clientid = clientId;

            return obj;
        }

        public static int fnSaveSuggestedItem(SuggestedBulkOrderItem obj)
        {
            using (var db = new CMCSQL03Entities())
            {
                if (obj.id == -1)
                {
                    obj.id = fnNewSuggestedOrderItemID();
                }

                var suggesteditem = db.tblSuggestedBulk.Find(obj.id);
                var productmaster = db.tblProductMaster.Find(obj.productmasterid);

                suggesteditem.ClientID = obj.clientid;
                suggesteditem.ProductMasterID = obj.productmasterid;
                suggesteditem.ReorderWeight = obj.reorderweight ?? 0;
                suggesteditem.ReorderNotes = obj.reordernotes;
                suggesteditem.UserName = HttpContext.Current.User.Identity.Name;

                if (productmaster != null)
                {
                    suggesteditem.SupplyID = productmaster.SUPPLYID;
                }

                db.SaveChanges();

                return obj.id;
            }
        }

        private static int fnNewSuggestedOrderItemID()
        {
            using (var db = new CMCSQL03Entities())
            {
                var suggestedItem = new tblSuggestedBulk();
                db.tblSuggestedBulk.Add(suggestedItem);
                db.SaveChanges();

                return suggestedItem.id;
            }
        }

        public static int fnCreateBulkOrders()
        {
            using (var db = new CMCSQL03Entities())
            {
                string userName = HttpContext.Current.User.Identity.Name;
                string fnTempTable = "tblSuggestedBulk";

                string updateQuery = String.Format(@"UPDATE {0} SET SupplyID='N/A'
                                                     WHERE Supplyid IS NULL
                                                     AND UserName='{1}'", fnTempTable, userName);

                db.Database.ExecuteSqlCommand(updateQuery);

                var suggestedBulkItems = (from t in db.tblSuggestedBulk
                                          where t.UserName == userName
                                          select new
                                          {
                                              t.ClientID,
                                              t.SupplyID
                                          }).Distinct();

                int SupplyIDCount = suggestedBulkItems.Count();
                string BatchNumber = DateTime.UtcNow.ToString();

                foreach (var item in suggestedBulkItems)
                {
                    using (var db1 = new CMCSQL03Entities())
                    {
                        tblBulkOrder bulkOrder = new tblBulkOrder
                        {
                            ClientID = item.ClientID,
                            OrderDate = DateTime.UtcNow,
                            Status = "OP",
                            SupplyID = item.SupplyID
                        };

                        var bulkSupplier = (from t in db.tblBulkSupplier
                                            where t.ClientID == item.ClientID
                                            && t.SupplyID == item.SupplyID
                                            select t).FirstOrDefault();

                        if (bulkSupplier != null)
                        {
                            bulkOrder.BulkSupplierEmail = bulkSupplier.Email;
                        }

                        db1.tblBulkOrder.Add(bulkOrder);
                        db1.SaveChanges();

                        int bulkOrderId = bulkOrder.BulkOrderID;

                        string insertQuery = String.Format(@"INSERT INTO tblBulkOrderItem
                                                             (BulkOrderID,ProductMasterID,Qty,Weight,Status,SupplyID,ItemNotes)
                                                             Select {0},ProductMasterID,1,ReorderWeight,'OP',SupplyID,ReorderNotes
                                                             FROM {1} WHERE SupplyID='{2}'", bulkOrderId, fnTempTable, item.SupplyID);

                        db1.Database.ExecuteSqlCommand(insertQuery);
                    }
                }

                int OrdersCount = suggestedBulkItems.Count();

                string deleteQuery = "DELETE FROM " + fnTempTable + " WHERE UserName=" + userName;
                db.Database.ExecuteSqlCommand(deleteQuery);

                return OrdersCount;
            }
        }

        #endregion SuggestedBulkOrder ---------------------------------------------------------------------------------
    }
}