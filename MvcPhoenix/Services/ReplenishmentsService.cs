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
        /// <summary>
        /// Search funtionality for bulk orders.
        /// </summary>
        /// <param name="form"></param>
        /// <param name="mode"></param>
        /// <returns>bulkOrders</returns>
        public static List<BulkOrder> SearchBulkOrders(FormCollection form, string mode)
        {
            using (var db = new CMCSQL03Entities())
            {
                var bulkOrders = (from bulkorder in db.tblBulkOrder
                                  join client in db.tblClient on bulkorder.ClientID equals client.ClientID
                                  let itemscount = (from items in db.tblBulkOrderItem
                                                    where (items.BulkOrderID == bulkorder.BulkOrderID)
                                                    select items).Count()
                                  let opencount = (from items in db.tblBulkOrderItem
                                                   where (items.BulkOrderID == bulkorder.BulkOrderID)
                                                   && (items.Status == "OP")
                                                   select items).Count()
                                  orderby bulkorder.BulkOrderID descending
                                  select new MvcPhoenix.Models.BulkOrder
                                  {
                                      bulkorderid = bulkorder.BulkOrderID,
                                      clientid = bulkorder.ClientID,
                                      supplyid = bulkorder.SupplyID,
                                      orderdate = bulkorder.OrderDate,
                                      ordercomment = bulkorder.Comment,
                                      emailsent = bulkorder.EmailSent,
                                      clientname = client.ClientName,
                                      itemcount = itemscount,
                                      opencount = opencount
                                  }).ToList();

                if (mode == "User")
                {
                    int orderCount = Convert.ToInt32(form["ordercount"]);
                    int clientId = Convert.ToInt32(form["clientid"]);

                    if (orderCount > 0)
                    {
                        bulkOrders = (from t in bulkOrders
                                      where t.clientid == clientId
                                      select t).ToList();
                    }

                    if (clientId > 0)
                    {
                        bulkOrders = (from t in bulkOrders
                                      select t).Take(orderCount).ToList();
                    }
                }

                // preset request
                string message = null;
                switch (mode)
                {
                    case "Initial":
                        bulkOrders = bulkOrders.Take(10).ToList();
                        message = "Initial";
                        break;

                    case "LastTen":
                        bulkOrders = bulkOrders.Take(10).ToList();
                        message = "Last Ten";
                        break;

                    case "UnConfirmed":
                        bulkOrders = (from t in bulkOrders
                                      where t.emailsent == null
                                      select t).ToList();

                        message = "Un-confirmed";
                        break;

                    case "OpenOnly":
                        bulkOrders = (from t in bulkOrders
                                      where t.opencount > 0
                                      select t).ToList();

                        message = "Open Items";
                        break;

                    default:
                        break;
                }

                foreach (var item in bulkOrders)
                {
                    item.ResultsMessage = message;
                }

                return bulkOrders;
            }
        }

        /// <summary>
        /// Fetch supplier email
        /// </summary>
        /// <param name="clientid"></param>
        /// <param name="supplyid"></param>
        /// <returns>email</returns>
        public static string GetSupplierEmail(int? clientid, string supplyid)
        {
            string email = "no email on file";
            
            using (var db = new CMCSQL03Entities())
            {
                var supplier = (from t in db.tblBulkSupplier
                                where t.ClientID == clientid
                                && t.SupplyID == supplyid
                                select t).FirstOrDefault();

                if (supplier != null)
                {
                    email = supplier.Email;
                }
            }
            
            return email;
        }

        /// <summary>
        /// Fetch a list of bulk order items.
        /// </summary>
        /// <param name="bulkOrderId"></param>
        /// <returns>bulkOrderItems</returns>
        public static List<BulkOrderItem> BulkOrderItems(int bulkOrderId)
        {
            using (var db = new CMCSQL03Entities())
            {
                var bulkOrderItems = (from t in db.tblBulkOrderItem
                                      join pm in db.tblProductMaster on t.ProductMasterID equals pm.ProductMasterID
                                      where t.BulkOrderID == bulkOrderId
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

                return bulkOrderItems;
            }
        }

        /// <summary>
        /// Fetch details for bulk order and items in it.
        /// </summary>
        /// <param name="bulkOrderId"></param>
        /// <returns>bulkOrder</returns>
        public static BulkOrder GetBulkOrder(int bulkOrderId)
        {
            BulkOrder bulkOrder = new BulkOrder();

            using (var db = new CMCSQL03Entities())
            {
                bulkOrder = (from t in db.tblBulkOrder
                             join c in db.tblClient on t.ClientID equals c.ClientID
                             where t.BulkOrderID == bulkOrderId
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

                bulkOrder.ListOfBulkOrderItem = (from oi in db.tblBulkOrderItem
                                                 join pm in db.tblProductMaster on oi.ProductMasterID equals pm.ProductMasterID
                                                 where oi.BulkOrderID == bulkOrderId
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

                return bulkOrder;
            }
        }

        /// <summary>
        /// Create/Edit bulk order data
        /// </summary>
        /// <param name="bulkOrderData"></param>
        /// <returns></returns>
        public static int SaveBulkOrder(BulkOrder bulkOrderData)
        {
            System.Threading.Thread.Sleep(1500);
            
            using (var db = new CMCSQL03Entities())
            {
                if (bulkOrderData.bulkorderid == -1)
                {
                    var newBulkOrder = new tblBulkOrder();
                    db.tblBulkOrder.Add(newBulkOrder);
                    db.SaveChanges();

                    bulkOrderData.bulkorderid = newBulkOrder.BulkOrderID;
                }

                var bulkOrder = db.tblBulkOrder.Find(bulkOrderData.bulkorderid);

                bulkOrder.ClientID = bulkOrderData.clientid;
                bulkOrder.OrderDate = bulkOrderData.orderdate;
                bulkOrder.Comment = bulkOrderData.ordercomment;
                bulkOrder.SupplyID = bulkOrderData.supplyid;
                bulkOrder.BulkSupplierEmail = bulkOrderData.bulksupplieremail;
                bulkOrder.EmailSent = bulkOrderData.emailsent;

                db.SaveChanges();

                return bulkOrderData.bulkorderid;
            }
        }
        
        /// <summary>
        /// Build template for bulk order email
        /// </summary>
        /// <param name="bulkOrder"></param>
        /// <returns>message</returns>
        public static BulkOrderEmailViewModel BuildBulkOrderEmail(BulkOrder bulkOrder)
        {
            var getBulkOrder = GetBulkOrder(bulkOrder.bulkorderid);

            BulkOrderEmailViewModel message = new BulkOrderEmailViewModel();
            message.bulkorderid = bulkOrder.bulkorderid;
            message.clientname = bulkOrder.clientname;
            message.ToAddress = bulkOrder.bulksupplieremail;
            message.FromAddress = HttpContext.Current.User.Identity.Name;
            message.Subject = "CMC Replenishment Order: " + bulkOrder.bulkorderid;

            StringBuilder builder = new StringBuilder();
            builder.Append("<div>");
            builder.Append("<p><b>Order Note: </b>" + bulkOrder.ordercomment + "</p>");
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

            foreach (var item in getBulkOrder.ListOfBulkOrderItem)
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

        /// <summary>
        /// Build email template for replenishment order.
        /// Send email to supplier or provided email address.
        /// </summary>
        /// <param name="bulkOrderEmail"></param>
        public static void BuildSendEmail(BulkOrderEmailViewModel bulkOrderEmail)
        {
            // build table and send email
            var bulkOrder = GetBulkOrder(bulkOrderEmail.bulkorderid);
            var shipTo = @"Chemical Marketing Concepts Europe<br/>
                           Industrieweg 73<br/>
                           5145 PD Waalwijk<br/>
                           The Netherlands<br/>
                           +31 (0)416-651977";

            StringBuilder builder = new StringBuilder();
            builder.Append("<p><em>The following message is sent on behalf of " + bulkOrderEmail.FromAddress + "</em></p>");

            builder.Append("<p>Hello,</p>");
            builder.Append("<p>Please find below our new replenishment order.</p>");
            builder.Append("<p>Thank you for your feedback.</p>");
            builder.Append("<p>Kind regards,<br/>Inventory Team<br/>Chemical Marketing Concepts</p>");
            builder.Append("<hr style='border: 1px solid black'/>");

            builder.Append("<p><b>Order Number: </b>" + bulkOrderEmail.bulkorderid + "<br/>");
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

            bulkOrderEmail.MessageBody = builder.ToString();

            Thread.Sleep(500);
            EmailServices.EmailSmtpSend(bulkOrderEmail.FromAddress, bulkOrderEmail.ToAddress, bulkOrderEmail.Subject, bulkOrderEmail.MessageBody);

            using (var db = new CMCSQL03Entities())
            {
                var updateBulkOrder = db.tblBulkOrder.Find(bulkOrderEmail.bulkorderid);
                updateBulkOrder.EmailSent = DateTime.UtcNow.ToString("R");

                db.SaveChanges();
            }
        }

        /// <summary>
        /// Get empty model for new bulk order item entry.
        /// </summary>
        /// <param name="bulkOrderId"></param>
        /// <returns>bulkOrderItem</returns>
        public static BulkOrderItem CreateBulkOrderItem(int bulkOrderId)
        {
            BulkOrderItem bulkOrderItem = new BulkOrderItem();

            using (var db = new CMCSQL03Entities())
            {
                var bulkOrder = db.tblBulkOrder.Find(bulkOrderId);
                bulkOrderItem.bulkorderitemid = -1;
                bulkOrderItem.bulkorderid = bulkOrderId;
                bulkOrderItem.productmasterid = null;
                bulkOrderItem.weight = 0;
                bulkOrderItem.itemstatus = "OP";
                bulkOrderItem.eta = null;
                bulkOrderItem.datereceived = null;
                bulkOrderItem.itemnotes = "";
                bulkOrderItem.ListOfProductMasters = ApplicationService.ddlProductMasterIDs(Convert.ToInt32(bulkOrder.ClientID));

                return bulkOrderItem;
            }
        }

        /// <summary>
        /// Fetch details for bulk order item.
        /// </summary>
        /// <param name="bulkOrderItemId"></param>
        /// <returns>bulkOrderItem</returns>
        public static BulkOrderItem GetBulkOrderItem(int bulkOrderItemId)
        {
            BulkOrderItem bulkOrderItem = new BulkOrderItem();

            using (var db = new CMCSQL03Entities())
            {
                bulkOrderItem = (from t in db.tblBulkOrderItem
                                 where t.BulkOrderItemID == bulkOrderItemId
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

                var bulkOrder = db.tblBulkOrder.Find(bulkOrderItem.bulkorderid);
                bulkOrderItem.ListOfProductMasters = ApplicationService.ddlProductMasterIDs(Convert.ToInt32(bulkOrder.ClientID));

                return bulkOrderItem;
            }
        }
        
        /// <summary>
        /// Create/Edit bulk order item data.
        /// </summary>
        /// <param name="bulkIOrderItemData"></param>
        /// <returns>bulkIOrderItemData.bulkorderitemid</returns>
        public static int SaveBulkOrderItem(BulkOrderItem bulkIOrderItemData)
        {
            System.Threading.Thread.Sleep(500);
            
            using (var db = new CMCSQL03Entities())
            {
                if (bulkIOrderItemData.bulkorderitemid == -1)
                {
                    var bulkItem = new tblBulkOrderItem();
                    db.tblBulkOrderItem.Add(bulkItem);
                    db.SaveChanges();
                    
                    bulkIOrderItemData.bulkorderitemid = bulkItem.BulkOrderItemID;
                }

                var bulkOrderItem = (from t in db.tblBulkOrderItem
                                     where t.BulkOrderItemID == bulkIOrderItemData.bulkorderitemid
                                     select t).FirstOrDefault();

                bulkOrderItem.BulkOrderID = bulkIOrderItemData.bulkorderid;
                bulkOrderItem.ProductMasterID = bulkIOrderItemData.productmasterid;
                bulkOrderItem.Weight = bulkIOrderItemData.weight;
                bulkOrderItem.Status = bulkIOrderItemData.itemstatus;
                bulkOrderItem.ETA = bulkIOrderItemData.eta;
                bulkOrderItem.DateReceived = bulkIOrderItemData.datereceived;
                bulkOrderItem.ItemNotes = bulkIOrderItemData.itemnotes;

                db.SaveChanges();

                return bulkIOrderItemData.bulkorderitemid;
            }
        }

        /// <summary>
        /// Delete bulk product item from replenishment order.
        /// </summary>
        /// <param name="bulkOrderItemId"></param>
        /// <returns>bulkOrderId</returns>
        public static int DeleteBulkOrderItem(int bulkOrderItemId)
        {
            using (var db = new CMCSQL03Entities())
            {
                var bulkOrderItem = db.tblBulkOrderItem.Find(bulkOrderItemId);
                int bulkOrderId = Convert.ToInt32(bulkOrderItem.BulkOrderID);

                string deleteQuery = "DELETE FROM tblBulkOrderItem WHERE BulkOrderItemID=" + bulkOrderItemId;
                db.Database.ExecuteSqlCommand(deleteQuery);

                return bulkOrderId;
            }
        }

        /// <summary>
        /// Scan bulk and shelf products using business rules.
        /// Copy products that match to temporary working table.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="divisionId"></param>
        /// <returns>itemsCount</returns>
        public static int GenerateSuggestedBulkOrder(int clientId, int divisionId)
        {
            int itemsCount = 0;
            string userName = HttpContext.Current.User.Identity.Name;
            string deleteQuery = "DELETE FROM tblSuggestedBulk WHERE UserName='" + userName + "'";

            using (var db = new CMCSQL03Entities())
            {
                // Clear the table for current user rows
                db.Database.ExecuteSqlCommand(deleteQuery);

                // Build a list of ProductMasters to analyze
                var products = (from pd in db.tblProductDetail
                                join pm in db.tblProductMaster on pd.ProductMasterID equals pm.ProductMasterID
                                where pm.ClientID == clientId
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
                if (divisionId > 0)
                {
                    products = (from t in products
                                where t.DivisionID == divisionId
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
                    // TODO: Flip logic to check only avail instead of exceptions in string array
                    //string[] bulkStatus = { "QC", "TEST", "WASTE", "RETURN" };

                    var bulks = (from t in db.tblBulk
                                 where t.ProductMasterID == item.ProductMasterID
                                 select t).ToList();

                    suggestedBulk.BulkCurrentAvailable = (from t in bulks
                                                          //where !bulkStatus.Contains(t.BulkStatus)
                                                          where t.BulkStatus == "AVAIL"
                                                          select ((t.Qty == null ? 1 : t.Qty) * t.CurrentWeight)).Sum();

                    //string[] stockStatus = { "QC", "TEST", "WASTE", "RETURN" };

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
                                                           //where !stockStatus.Contains(t.ShelfStatus)
                                                           where t.ShelfStatus == "AVAIL"
                                                           select (t.QtyOnHand * t.UnitWeight)).Sum();

                    suggestedBulk.CurrentAvailable = suggestedBulk.BulkCurrentAvailable + suggestedBulk.ShelfCurrentAvailable;

                    var inventoryBulkLog = (from t in db.tblInvLog
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

                    inventoryBulkLog = (from t in inventoryBulkLog
                                        //where !bulkStatus.Contains(t.BulkStatus)
                                        where t.BulkStatus == "AVAIL"
                                        select t).ToList();

                    inventoryBulkLog = (from t in inventoryBulkLog
                                        where t.LogDate > DateTime.UtcNow.AddDays(-365)
                                        select t).ToList();

                    suggestedBulk.BulkShippedPastYear = (from t in inventoryBulkLog
                                                         select (t.LogQty * t.LogAmount)).Sum();

                    if (suggestedBulk.BulkShippedPastYear > 0 && suggestedBulk.ProductMasterAge > 365)
                    {
                        suggestedBulk.BulkShippedPerDay = suggestedBulk.BulkShippedPastYear / 365;
                    }

                    if (suggestedBulk.BulkShippedPastYear > 0 && suggestedBulk.ProductMasterAge <= 365)
                    {
                        suggestedBulk.BulkShippedPerDay = suggestedBulk.BulkShippedPastYear / suggestedBulk.ProductMasterAge;
                    }

                    var inventoryShelfLog = (from t in db.tblInvLog
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

                    inventoryShelfLog = (from t in inventoryShelfLog
                                         //where !stockStatus.Contains(t.Status)
                                         where t.Status == "AVAIL"
                                         select t).ToList();

                    inventoryShelfLog = (from t in inventoryShelfLog
                                         where t.LogDate > DateTime.UtcNow.AddDays(-365)
                                         select t).ToList();

                    suggestedBulk.ShelfShippedPastYear = (from t in inventoryShelfLog
                                                          select (t.LogQty * t.LogAmount)).Sum();

                    if (suggestedBulk.ShelfShippedPastYear > 0 && suggestedBulk.ProductMasterAge > 365)
                    {
                        suggestedBulk.ShelfShippedPerDay = suggestedBulk.ShelfShippedPastYear / 365;
                    }

                    if (suggestedBulk.ShelfShippedPastYear > 0 && suggestedBulk.ProductMasterAge <= 365)
                    {
                        suggestedBulk.ShelfShippedPerDay = suggestedBulk.ProductMasterAge / 365;
                    }

                    var bulkProduct = (from t in db.tblBulk
                                       where t.ProductMasterID == item.ProductMasterID
                                       orderby t.CeaseShipDate descending
                                       select t).Take(1).FirstOrDefault();

                    // set initial value to null, then try to update it, if cannot update set to a future date (below)
                    if (bulkProduct != null && bulkProduct.CeaseShipDate != null)
                    {
                        suggestedBulk.BulkLatestExpirationDate = bulkProduct.CeaseShipDate;
                        suggestedBulk.BulkDaysTilExpiration = (suggestedBulk.BulkLatestExpirationDate.Value - DateTime.UtcNow.Date).Days;
                        suggestedBulk.ShelfLatestExpirationDate = bulkProduct.CeaseShipDate;
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

                    var bulkOrderItems = (from t in db.tblBulkOrderItem
                                          where t.ProductMasterID == item.ProductMasterID
                                          && (t.Status == "OP" || t.Status == "OPEN")
                                          select t.BulkOrderItemID).ToList();

                    if (bulkOrderItems.Count() > 0)
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

        /// <summary>
        /// Fetch a list of bulk items generated by user.
        /// </summary>
        /// <returns>suggestedItems</returns>
        public static List<SuggestedBulkOrderItem> SuggestedItems()
        {
            string userName = HttpContext.Current.User.Identity.Name;

            using (var db = new CMCSQL03Entities())
            {                
                var suggestedItems = (from t in db.tblSuggestedBulk
                                      join division in db.tblDivision on t.DivisionID equals division.DivisionID into divisionBulk
                                      from x in divisionBulk.DefaultIfEmpty()
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

        /// <summary>
        /// Detail bulk item from temporary table.
        /// </summary>
        /// <param name="bulkItemId"></param>
        public static void DeleteSuggestedBulkItem(int bulkItemId)
        {
            using (var db = new CMCSQL03Entities())
            {
                string deleteQuery = "DELETE FROM tblSuggestedBulk WHERE ID=" + bulkItemId;
                db.Database.ExecuteSqlCommand(deleteQuery);
            }
        }

        /// <summary>
        /// Fetch details for bulk item from temporary table.
        /// </summary>
        /// <param name="bulkItemId"></param>
        /// <returns>suggestedBulkItem</returns>
        public static SuggestedBulkOrderItem GetSuggestedBulkItem(int bulkItemId)
        {
            SuggestedBulkOrderItem suggestedBulkItem = new SuggestedBulkOrderItem();

            using (var db = new CMCSQL03Entities())
            {
                suggestedBulkItem = (from t in db.tblSuggestedBulk
                                     where t.id == bulkItemId
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

                return suggestedBulkItem;
            }
        }

        /// <summary>
        /// Create/Edit a suggested bulk order item.
        /// Write to the temp table tblSuggestedBulk.
        /// </summary>
        /// <param name="bulkOrderItem"></param>
        /// <returns>bulkOrderItem.id</returns>
        public static int SaveSuggestedBulkItem(SuggestedBulkOrderItem bulkOrderItem)
        {
            using (var db = new CMCSQL03Entities())
            {
                if (bulkOrderItem.id == -1)
                {
                    var newSuggestedItem = new tblSuggestedBulk();
                    db.tblSuggestedBulk.Add(newSuggestedItem);
                    db.SaveChanges();

                    bulkOrderItem.id = newSuggestedItem.id;
                }

                var suggestedItem = db.tblSuggestedBulk.Find(bulkOrderItem.id);
                var productMaster = db.tblProductMaster.Find(bulkOrderItem.productmasterid);

                suggestedItem.ClientID = bulkOrderItem.clientid;
                suggestedItem.ProductMasterID = bulkOrderItem.productmasterid;
                suggestedItem.ReorderWeight = bulkOrderItem.reorderweight ?? 0;
                suggestedItem.ReorderNotes = bulkOrderItem.reordernotes;
                suggestedItem.UserName = HttpContext.Current.User.Identity.Name;

                if (productMaster != null)
                {
                    suggestedItem.SupplyID = productMaster.SUPPLYID;
                }

                db.SaveChanges();

                return bulkOrderItem.id;
            }
        }

        /// <summary>
        /// Gather list of bulk items generated by current user.
        /// Create bulk order and item details with list.
        /// </summary>
        /// <returns>bulkItemsCount</returns>
        public static int CreateSuggestedBulkOrder()
        {
            string userName = HttpContext.Current.User.Identity.Name;
            string batchNumber = DateTime.UtcNow.ToString();
            DateTime clockWatch = DateTime.UtcNow;
            int bulkItemsCount;
            int supplyIdCount;
            int? clientId = 0;
            string supplyId;

            using (var db = new CMCSQL03Entities())
            {
                // Get all suggested items held by current user
                var suggestedBulkItems = (from t in db.tblSuggestedBulk
                                          where t.UserName == userName
                                          select t).ToList();

                // Filter for unique products
                var uniqueBulkItems = (from x in suggestedBulkItems
                                       group x by new { x.ProductMasterID }
                                           into uniqueProducts
                                           select uniqueProducts.FirstOrDefault()).ToList();

                bulkItemsCount = suggestedBulkItems.Count();
                bulkItemsCount = uniqueBulkItems.Count();

                clientId = suggestedBulkItems.Select(x => x.ClientID).FirstOrDefault();
                supplyId = suggestedBulkItems.Select(x => x.SupplyID).FirstOrDefault();

                // Get bulk supplier email
                //var bulkSupplierEmail = (from t in db.tblBulkSupplier
                //                         where t.ClientID == clientId
                //                         && t.SupplyID.Contains(supplyId)
                //                         select t.Email).FirstOrDefault();

                // Create bulk order
                tblBulkOrder bulkOrder = new tblBulkOrder
                {
                    ClientID = clientId,
                    OrderDate = clockWatch,
                    Status = "OP",
                    SupplyID = supplyId,
                    BulkSupplierEmail = GetSupplierEmail(clientId, supplyId),
                    CreateDate = clockWatch,
                    CreateUser = userName,
                    UpdateDate = clockWatch,
                    UpdateUser = userName
                };

                db.tblBulkOrder.Add(bulkOrder);
                db.SaveChanges();

                // Create bulk items for the order
                foreach (var item in uniqueBulkItems)
                {
                    var productCheck = (from t in db.tblProductMaster
                                        where t.ProductMasterID == item.ProductMasterID
                                        select t).FirstOrDefault();

                    tblBulkOrderItem bulkOrderItem = new tblBulkOrderItem
                    {
                        BulkOrderID = bulkOrder.BulkOrderID,
                        ProductMasterID = item.ProductMasterID,
                        ProductCode = productCheck.MasterCode,
                        ProductName = productCheck.MasterName,
                        Qty = 1,
                        Weight = item.ReorderWeight,
                        Status = "OP",
                        SupplyID = item.SupplyID,
                        ItemNotes = item.ReorderNotes,
                        ShelfLife = item.ShelfLife
                    };

                    db.tblBulkOrderItem.Add(bulkOrderItem);
                    db.SaveChanges();
                }

                string deleteQuery = "DELETE FROM tblSuggestedBulk WHERE ClientID=" + clientId + " AND UserName='" + userName + "'";
                db.Database.ExecuteSqlCommand(deleteQuery);

                return bulkItemsCount;
            }
        }
    }
}