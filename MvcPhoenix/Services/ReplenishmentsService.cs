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
        #region Bulk Order

        /// <summary>
        /// Get bulk orders.
        /// </summary>
        /// <param name="form"></param>
        /// <param name="searchmode"></param>
        /// <returns>list of bulkOrders</returns>
        public static List<BulkOrder> GetBulkOrders(FormCollection form, string searchmode)
        {
            var bulkOrders = new List<BulkOrder>();

            using (var db = new CMCSQL03Entities())
            {
                bulkOrders = (from bulkorder in db.tblBulkOrder
                              join client in db.tblClient on bulkorder.ClientID equals client.ClientID
                              let itemscount = db.tblBulkOrderItem
                                                 .Where(t => t.BulkOrderID == bulkorder.BulkOrderID)
                                                 .Count()
                              let opencount = db.tblBulkOrderItem
                                                .Where(t => t.BulkOrderID == bulkorder.BulkOrderID &&
                                                       t.Status == "OP").Count()
                              orderby bulkorder.BulkOrderID descending
                              select new BulkOrder
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

                if (searchmode == "User")
                {
                    int orderCount = Convert.ToInt32(form["ordercount"]);
                    int clientId = Convert.ToInt32(form["clientid"]);

                    if (orderCount > 0)
                    {
                        bulkOrders = bulkOrders.Where(t => t.clientid == clientId).ToList();
                    }

                    if (clientId > 0)
                    {
                        bulkOrders = bulkOrders.Take(orderCount).ToList();
                    }
                }

                // preset request
                string message = null;
                switch (searchmode)
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
                        bulkOrders = bulkOrders.Where(t => t.emailsent == null).ToList();
                        message = "Un-confirmed";
                        break;

                    case "OpenOnly":
                        bulkOrders = bulkOrders.Where(t => t.opencount > 0).ToList();
                        message = "Open Items";
                        break;

                    default:
                        break;
                }

                foreach (var item in bulkOrders)
                {
                    item.ResultsMessage = message;
                }
            }

            return bulkOrders;
        }

        /// <summary>
        /// Get bulk order details
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

                bulkOrder.ListOfBulkOrderItem = ReplenishmentsService.BulkOrderItems(bulkOrder.bulkorderid);
            }

            return bulkOrder;
        }

        /// <summary>
        /// Create/Edit bulk order data
        /// </summary>
        /// <param name="bulkOrderData"></param>
        /// <returns>bulkOrderId</returns>
        public static int SaveBulkOrder(BulkOrder bulkOrderData)
        {
            System.Threading.Thread.Sleep(500);

            int bulkOrderId;

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

                bulkOrderId = bulkOrderData.bulkorderid;
            }

            return bulkOrderId;
        }

        #endregion Bulk Order

        #region Bulk Order Item

        /// <summary>
        /// Get list of bulk order items
        /// </summary>
        /// <param name="bulkOrderId"></param>
        /// <returns>bulkOrderItems</returns>
        public static List<BulkOrderItem> BulkOrderItems(int bulkOrderId)
        {
            var bulkOrderItems = new List<BulkOrderItem>();

            using (var db = new CMCSQL03Entities())
            {
                bulkOrderItems = (from bulkitem in db.tblBulkOrderItem
                                  join productmaster in db.tblProductMaster on bulkitem.ProductMasterID equals productmaster.ProductMasterID
                                  where bulkitem.BulkOrderID == bulkOrderId
                                  select new BulkOrderItem
                                  {
                                      mastercode = productmaster.MasterCode,
                                      mastername = productmaster.MasterName,
                                      bulkorderitemid = bulkitem.BulkOrderItemID,
                                      bulkorderid = bulkitem.BulkOrderID,
                                      productmasterid = bulkitem.ProductMasterID,
                                      weight = bulkitem.Weight,
                                      itemstatus = bulkitem.Status,
                                      eta = bulkitem.ETA,
                                      datereceived = bulkitem.DateReceived,
                                      itemnotes = bulkitem.ItemNotes,
                                      PrepackedBulk = productmaster.PrePacked
                                  }).ToList();
            }

            return bulkOrderItems;
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
                                     itemnotes = t.ItemNotes
                                 }).FirstOrDefault();

                var bulkOrder = db.tblBulkOrder.Find(bulkOrderItem.bulkorderid);
                bulkOrderItem.ListOfProductMasters = ApplicationService.ddlProductMasterIDs(Convert.ToInt32(bulkOrder.ClientID));
            }

            return bulkOrderItem;
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
            }

            return bulkOrderItem;
        }

        /// <summary>
        /// Create/Edit bulk order item data.
        /// </summary>
        /// <param name="bulkIOrderItemData"></param>
        /// <returns>bulkOrderItemId</returns>
        public static int SaveBulkOrderItem(BulkOrderItem bulkIOrderItemData)
        {
            System.Threading.Thread.Sleep(500);

            int bulkOrderItemId;

            using (var db = new CMCSQL03Entities())
            {
                if (bulkIOrderItemData.bulkorderitemid == -1)
                {
                    var bulkItem = new tblBulkOrderItem();
                    db.tblBulkOrderItem.Add(bulkItem);
                    db.SaveChanges();

                    bulkIOrderItemData.bulkorderitemid = bulkItem.BulkOrderItemID;
                }

                var bulkOrderItem = db.tblBulkOrderItem.Find(bulkIOrderItemData.bulkorderitemid);

                bulkOrderItem.BulkOrderID = bulkIOrderItemData.bulkorderid;
                bulkOrderItem.ProductMasterID = bulkIOrderItemData.productmasterid;
                bulkOrderItem.Weight = bulkIOrderItemData.weight;
                bulkOrderItem.Status = bulkIOrderItemData.itemstatus;
                bulkOrderItem.ETA = bulkIOrderItemData.eta;
                bulkOrderItem.DateReceived = bulkIOrderItemData.datereceived;
                bulkOrderItem.ItemNotes = bulkIOrderItemData.itemnotes;

                db.SaveChanges();

                bulkOrderItemId = bulkIOrderItemData.bulkorderitemid;
            }

            return bulkOrderItemId;
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

        #endregion Bulk Order Item

        #region Suggested Bulk Order

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
                var products = (from productdetail in db.tblProductDetail
                                join productmaster in db.tblProductMaster on productdetail.ProductMasterID equals productmaster.ProductMasterID
                                where productmaster.ClientID == clientId
                                select new
                                {
                                    productmaster.ClientID,
                                    productmaster.ProductMasterID,
                                    productmaster.SUPPLYID,
                                    productmaster.ShelfLife,
                                    productmaster.ProductSetupDate,
                                    productdetail.DivisionID,
                                }).ToList();

                // Restrict list to a PD.DivisionID if user requested
                if (divisionId > 0)
                {
                    products = products.Where(t => t.DivisionID == divisionId).ToList();
                }

                // Insert new records into the work table
                foreach (var item in products)
                {
                    var suggestedBulk = new tblSuggestedBulk();
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
                    var bulks = db.tblBulk.Where(t => t.ProductMasterID == item.ProductMasterID).ToList();

                    suggestedBulk.BulkCurrentAvailable = bulks.Where(t => t.BulkStatus == "AVAIL")
                                                              .Select(t => (t.Qty == null ? 1 : t.Qty) * t.CurrentWeight)
                                                              .Sum();

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

                    suggestedBulk.ShelfCurrentAvailable = stock.Where(t => t.ShelfStatus == "AVAIL")
                                                               .Select(t => t.QtyOnHand * t.UnitWeight)
                                                               .Sum();

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

                    inventoryBulkLog = inventoryBulkLog.Where(t => t.BulkStatus == "AVAIL").ToList();
                    inventoryBulkLog = inventoryBulkLog.Where(t => t.LogDate > DateTime.UtcNow.AddDays(-365)).ToList();
                    suggestedBulk.BulkShippedPastYear = inventoryBulkLog.Select(t => t.LogQty * t.LogAmount).Sum();

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

                    inventoryShelfLog = inventoryShelfLog.Where(t => t.Status == "AVAIL").ToList();
                    inventoryShelfLog = inventoryShelfLog.Where(t => t.LogDate > DateTime.UtcNow.AddDays(-365)).ToList();
                    suggestedBulk.ShelfShippedPastYear = inventoryShelfLog.Select(t => t.LogQty * t.LogAmount).Sum();

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

                itemsCount = db.tblSuggestedBulk.Where(t => t.UserName == userName).Count();
            }

            return itemsCount;
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
                var suggestedBulkItems = db.tblSuggestedBulk.Where(t => t.UserName == userName).ToList();

                // Filter for unique products
                var uniqueBulkItems = (from x in suggestedBulkItems
                                       group x by new { x.ProductMasterID }
                                           into uniqueProducts
                                           select uniqueProducts.FirstOrDefault()).ToList();

                bulkItemsCount = suggestedBulkItems.Count();
                bulkItemsCount = uniqueBulkItems.Count();
                clientId = suggestedBulkItems.Select(x => x.ClientID).FirstOrDefault();
                supplyId = suggestedBulkItems.Select(x => x.SupplyID).FirstOrDefault();

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
                    var product = db.tblProductMaster.Find(item.ProductMasterID);

                    tblBulkOrderItem bulkOrderItem = new tblBulkOrderItem
                    {
                        BulkOrderID = bulkOrder.BulkOrderID,
                        ProductMasterID = item.ProductMasterID,
                        ProductCode = product.MasterCode,
                        ProductName = product.MasterName,
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

        #endregion Suggested Bulk Order

        #region Suggested Bulk Item

        /// <summary>
        /// Fetch a list of bulk items generated by user.
        /// </summary>
        /// <returns>suggestedItems</returns>
        public static List<SuggestedBulkOrderItem> GetSuggestedBulkItems()
        {
            string userName = HttpContext.Current.User.Identity.Name;

            using (var db = new CMCSQL03Entities())
            {
                var suggestedBulkItems = (from t in db.tblSuggestedBulk
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

                return suggestedBulkItems;
            }
        }

        /// <summary>
        /// Fetch details for bulk item from temporary table.
        /// </summary>
        /// <param name="bulkItemId"></param>
        /// <returns>suggestedBulkItem</returns>
        public static SuggestedBulkOrderItem GetSuggestedBulkItem(int bulkItemId)
        {
            var suggestedBulkOrderItem = new SuggestedBulkOrderItem();

            using (var db = new CMCSQL03Entities())
            {
                suggestedBulkOrderItem = (from t in db.tblSuggestedBulk
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
            }

            return suggestedBulkOrderItem;
        }

        /// <summary>
        /// Create/Edit a suggested bulk order item.
        /// Write to the temp table tblSuggestedBulk.
        /// </summary>
        /// <param name="bulkOrderItem"></param>
        /// <returns>bulkOrderItemId</returns>
        public static int SaveSuggestedBulkItem(SuggestedBulkOrderItem bulkOrderItem)
        {
            int bulkOrderItemId;

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

                bulkOrderItemId = bulkOrderItem.id;
            }

            return bulkOrderItemId;
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

        #endregion Suggested Bulk Item

        #region Bulk Order Email

        /// <summary>
        /// Fetch supplier email
        /// </summary>
        /// <param name="clientid"></param>
        /// <param name="supplyid"></param>
        /// <returns>email</returns>
        public static string GetSupplierEmail(int? clientid, string supplyid)
        {
            string email = "No email found";

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
            EmailService.EmailSmtpSend(bulkOrderEmail.FromAddress,
                                        bulkOrderEmail.ToAddress,
                                        bulkOrderEmail.Subject,
                                        bulkOrderEmail.MessageBody);

            using (var db = new CMCSQL03Entities())
            {
                var updateBulkOrder = db.tblBulkOrder.Find(bulkOrderEmail.bulkorderid);
                updateBulkOrder.EmailSent = DateTime.UtcNow.ToString("R");

                db.SaveChanges();
            }
        }

        #endregion Bulk Order Email
    }
}