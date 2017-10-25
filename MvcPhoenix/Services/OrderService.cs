using MvcPhoenix.EF;
using MvcPhoenix.Extensions;
using MvcPhoenix.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MvcPhoenix.Services
{
    public class OrderService
    {
        #region Order

        public static List<OrderMasterFull> GetOrders()
        {
            var orders = new List<OrderMasterFull>();

            using (var db = new CMCSQL03Entities())
            {
                orders = (from t in db.tblOrderMaster
                          join client in db.tblClient
                          on t.ClientID equals client.ClientID
                          let count = db.tblOrderItem
                                        .Where(x => x.OrderID == t.OrderID)
                                        .Count()
                          let allocationcount = db.tblOrderItem
                                                  .Where(i => i.OrderID == t.OrderID &&
                                                              i.ShipDate == null &&
                                                              i.Qty > 0 &&
                                                              String.IsNullOrEmpty(i.AllocateStatus) &&
                                                              i.ProductDetailID != null &&
                                                              i.ShelfID == null).Count()
                          select new OrderMasterFull
                          {
                              OrderID = t.OrderID,
                              OrderType = t.OrderType,
                              OrderDate = t.OrderDate,
                              ClientId = t.ClientID,
                              ClientName = client.ClientName,
                              Customer = t.Customer,                // client name on order master
                              Company = t.Company,                  // ship to
                              Attention = t.Attention,
                              Zip = t.Zip,
                              SalesRepName = t.SalesRep,
                              Tracking = t.Tracking,
                              WebOrderId = t.WebOrderID ?? 0,
                              ClientOrderNumber = t.CustOrdNum,
                              ClientRefNumber = t.RefNum,
                              SpecialInternal = t.SpecialInternal,
                              Special = t.Special,
                              ItemsCount = count,
                              NeedAllocationCount = allocationcount,
                              CreateDate = t.CreateDate,
                              CreateUser = t.CreateUser,
                              UpdateDate = t.UpdateDate,
                              UpdateUser = t.UpdateUser
                          }).ToList();
            }

            return orders;
        }

        public static List<OrderMasterFull> GetAssignedOpenOrders()
        {
            var orders = new List<OrderMasterFull>();

            using (var db = new CMCSQL03Entities())
            {
                orders = OrderService.GetOrders();

                // Get list of order ids for orders not shipped.
                var unshippedOrders = db.tblOrderItem
                                        .Where(i => i.ShipDate == null &&
                                                    i.Qty > 0)
                                        .ToList();

                unshippedOrders = unshippedOrders.GroupBy(x => x.OrderID)
                                                 .Select(g => g.First())
                                                 .ToList();

                // Get list of open orders.
                orders = (from t in orders
                          join u in unshippedOrders
                          on t.OrderID equals u.OrderID
                          join c in db.tblClientAccountRep
                          on t.ClientId equals c.ClientID
                          where c.AccountRepEmail == HttpContext.Current.User.Identity.Name
                          orderby t.OrderID descending, t.OrderDate descending
                          select t).ToList();

                // Display all clients in EU if user has no client assignments.
                // Since CMCEU does not have specific csr assignments for clients.
                if (orders.Count() < 1)
                {
                    orders = OrderService.GetOrders();
                    orders = (from t in orders
                              join u in unshippedOrders
                              on t.OrderID equals u.OrderID
                              join c in db.tblClient
                              on t.ClientId equals c.ClientID
                              where c.CMCLocation == "EU"
                              orderby t.OrderID descending, t.OrderDate descending
                              select t).ToList();
                }
            }

            return orders;
        }

        public static OrderMasterFull CreateOrder(int clientid)
        {
            var order = new OrderMasterFull();

            using (var db = new CMCSQL03Entities())
            {
                var client = db.tblClient.Find(clientid);

                order.OrderID = -1;
                order.ClientId = client.ClientID;
                order.ClientName = client.ClientName;
                order.OrderStatus = "New";
                order.IsSDN = false;
                order.OrderDate = DateTime.UtcNow;
                order.CreateUser = HttpContext.Current.User.Identity.Name;
                order.CreateDate = DateTime.UtcNow;
            }

            return order;
        }

        public static OrderMasterFull FillOrder(int orderid)
        {
            var order = new OrderMasterFull();

            using (var db = new CMCSQL03Entities())
            {
                var orderDetails = db.tblOrderMaster.Find(orderid);

                if (orderDetails == null)
                {
                    return null;
                }

                var client = db.tblClient.Find(orderDetails.ClientID);
                order.ItemsCount = db.tblOrderItem.Where(x => x.OrderID == orderid).Count();
                order.TransCount = db.tblOrderTrans.Where(x => x.OrderID == orderid).Count();

                order.ClientName = client.ClientName;
                order.ClientCode = client.ClientCode;
                order.OrderID = orderDetails.OrderID;
                order.ClientId = orderDetails.ClientID;
                order.DivisionId = orderDetails.DivisionID;
                order.OrderStatus = orderDetails.OrderStatus;
                order.Customer = orderDetails.Customer;
                order.CMCOrder = Convert.ToInt32(orderDetails.CMCOrder);
                order.WebOrderId = Convert.ToInt32(orderDetails.WebOrderID);
                order.CMCLegacyNumber = orderDetails.CMCLegacyNum;
                order.ClientOrderNumber = orderDetails.CustOrdNum;
                order.ClientSAPNumber = orderDetails.CustSapNum;
                order.ClientRefNumber = orderDetails.CustRefNum;
                order.OrderType = orderDetails.OrderType;
                order.OrderDate = orderDetails.OrderDate;
                order.Company = orderDetails.Company;
                order.Street = orderDetails.Street;
                order.Street2 = orderDetails.Street2;
                order.Street3 = orderDetails.Street3;
                order.City = orderDetails.City;
                order.State = orderDetails.State;
                order.Zip = orderDetails.Zip;
                order.Country = orderDetails.Country;
                order.CustomsRefNumber = orderDetails.CustomsRefNum;
                order.Attention = orderDetails.Attention;
                order.Email = orderDetails.Email;
                order.SalesRepName = orderDetails.SalesRep;
                order.SalesRepEmail = orderDetails.SalesEmail;
                order.RequestorName = orderDetails.Req;
                order.RequestorPhone = orderDetails.ReqPhone;
                order.RequestorFax = orderDetails.ReqFax;
                order.RequestorEmail = orderDetails.ReqEmail;
                order.EndUse = orderDetails.EndUse;
                order.ShipVia = orderDetails.ShipVia;
                order.ShipAcct = orderDetails.ShipAcct;
                order.Phone = orderDetails.Phone;
                order.Source = orderDetails.Source;
                order.Fax = orderDetails.Fax;
                order.Tracking = orderDetails.Tracking;
                order.Special = orderDetails.Special;
                order.SpecialInternal = orderDetails.SpecialInternal;
                order.IsLiterature = Convert.ToBoolean(orderDetails.Lit);
                order.Region = orderDetails.Region;
                order.COA = Convert.ToBoolean(orderDetails.COA);
                order.TDS = Convert.ToBoolean(orderDetails.TDS);
                order.CID = orderDetails.CID;
                order.ClientAcct = orderDetails.CustAcct;
                order.ACode = orderDetails.ACode;
                order.ImportFile = orderDetails.ImportFile;

                if (orderDetails.ImportDateLine.HasValue)
                {
                    order.ImportDateLine = Convert.ToDateTime(orderDetails.ImportDateLine);
                }
                else
                {
                    order.ImportDateLine = null;
                }

                order.Timing = orderDetails.Timing;
                order.Volume = orderDetails.Volume;
                order.SampleRack = Convert.ToBoolean(orderDetails.SampleRack);
                order.CMCUser = orderDetails.CMCUser;
                order.ClientReference = orderDetails.CustomerReference;
                order.TotalOrderWeight = orderDetails.TotalOrderWeight;
                order.ClientOrderType = orderDetails.CustOrderType;

                order.ClientRequestDate = null;
                if (orderDetails.CustRequestDate.HasValue)
                {
                    order.ClientRequestDate = orderDetails.CustRequestDate;
                }

                order.ApprovalDate = null;
                if (orderDetails.ApprovalDate.HasValue)
                {
                    order.ApprovalDate = orderDetails.ApprovalDate;
                }

                order.RequestedDeliveryDate = null;
                if (orderDetails.RequestedDeliveryDate.HasValue)
                {
                    order.RequestedDeliveryDate = orderDetails.RequestedDeliveryDate;
                }

                order.ClientTotalItems = Convert.ToInt32(orderDetails.CustTotalItems);
                order.ClientRequestedCarrier = orderDetails.CustRequestedCarrier;
                order.LegacyId = Convert.ToInt32(orderDetails.LegacyID);
                order.SalesRepPhone = orderDetails.SalesRepPhone;
                order.SalesRepTerritory = orderDetails.SalesRepTerritory;
                order.MarketingRep = orderDetails.MarketingRep;
                order.MarketingRepEmail = orderDetails.MarketingRepEmail;
                order.Distributor = orderDetails.Distributor;
                order.PreferredCarrier = orderDetails.PreferredCarrier;
                order.ApprovalNeeded = Convert.ToBoolean(orderDetails.ApprovalNeeded);
                order.CreateUser = orderDetails.CreateUser;
                order.CreateDate = orderDetails.CreateDate;
                order.UpdateUser = orderDetails.UpdateUser;
                order.UpdateDate = orderDetails.UpdateDate;
                order.IsSDN = orderDetails.IsSDN;
                order.IsSDNOverride = orderDetails.IsSDNOverride;
            }

            return order;
        }

        public static int NewOrderId()
        {
            var newOrder = new tblOrderMaster();

            using (var db = new CMCSQL03Entities())
            {
                newOrder.CreateDate = DateTime.UtcNow;
                newOrder.CreateUser = HttpContext.Current.User.Identity.Name;

                db.tblOrderMaster.Add(newOrder);
                db.SaveChanges();
            }

            return newOrder.OrderID;
        }

        public static int SaveOrder(OrderMasterFull order)
        {
            if (order.OrderID == -1)
            {
                order.OrderID = NewOrderId();
            }

            using (var db = new CMCSQL03Entities())
            {
                var orderMaster = db.tblOrderMaster.Find(order.OrderID);

                orderMaster.OrderDate = order.OrderDate;
                orderMaster.ClientID = order.ClientId;
                orderMaster.DivisionID = order.DivisionId;
                orderMaster.Customer = order.Customer;
                orderMaster.CMCOrder = order.CMCOrder;
                orderMaster.WebOrderID = order.WebOrderId;
                orderMaster.CMCLegacyNum = order.CMCLegacyNumber;
                orderMaster.CustOrdNum = order.ClientOrderNumber;
                orderMaster.CustSapNum = order.ClientSAPNumber;
                orderMaster.CustRefNum = order.ClientRefNumber;
                orderMaster.OrderType = order.OrderType;
                orderMaster.OrderDate = order.OrderDate;
                orderMaster.Company = order.Company;
                orderMaster.Street = order.Street;
                orderMaster.Street2 = order.Street2;
                orderMaster.Street3 = order.Street3;
                orderMaster.City = order.City;
                orderMaster.State = order.State;
                orderMaster.Zip = order.Zip;
                orderMaster.Country = order.Country;
                orderMaster.Attention = order.Attention;
                orderMaster.CustomsRefNum = order.CustomsRefNumber;
                orderMaster.Email = order.Email;
                orderMaster.SalesRep = order.SalesRepName;
                orderMaster.SalesEmail = order.SalesRepEmail;
                orderMaster.Req = order.RequestorName;
                orderMaster.ReqPhone = order.RequestorPhone;
                orderMaster.ReqFax = order.RequestorFax;
                orderMaster.ReqEmail = order.RequestorEmail;
                orderMaster.EndUse = order.EndUse;
                orderMaster.ShipVia = order.ShipVia;
                orderMaster.ShipAcct = order.ShipAcct;
                orderMaster.Phone = order.Phone;
                orderMaster.Source = order.Source;
                orderMaster.Fax = order.Fax;
                orderMaster.Tracking = order.Tracking;
                orderMaster.Special = order.Special;
                orderMaster.SpecialInternal = order.SpecialInternal;
                orderMaster.Lit = Convert.ToBoolean(order.IsLiterature);
                orderMaster.Region = order.Region;
                orderMaster.COA = order.COA;
                orderMaster.TDS = order.TDS;
                orderMaster.CID = order.CID;
                orderMaster.CustAcct = order.ClientAcct;
                orderMaster.ACode = order.ACode;
                orderMaster.ImportFile = order.ImportFile;
                orderMaster.ImportDateLine = order.ImportDateLine;
                orderMaster.Timing = order.Timing;
                orderMaster.Volume = order.Volume;
                orderMaster.SampleRack = Convert.ToBoolean(order.SampleRack);
                orderMaster.CMCUser = order.CMCUser;
                orderMaster.CustomerReference = order.ClientReference;
                orderMaster.TotalOrderWeight = (order.TotalOrderWeight);
                orderMaster.CustOrderType = order.ClientOrderType;
                orderMaster.CustRequestDate = order.ClientRequestDate;
                orderMaster.ApprovalDate = order.ApprovalDate;
                orderMaster.RequestedDeliveryDate = order.RequestedDeliveryDate;
                orderMaster.CustTotalItems = order.ClientTotalItems;
                orderMaster.CustRequestedCarrier = order.ClientRequestedCarrier;
                orderMaster.LegacyID = (order.LegacyId);
                orderMaster.SalesRepPhone = order.SalesRepPhone;
                orderMaster.SalesRepTerritory = order.SalesRepTerritory;
                orderMaster.MarketingRep = order.MarketingRep;
                orderMaster.MarketingRepEmail = order.MarketingRepEmail;
                orderMaster.Distributor = order.Distributor;
                orderMaster.PreferredCarrier = order.PreferredCarrier;
                orderMaster.ApprovalNeeded = order.ApprovalNeeded;
                orderMaster.UpdateUser = HttpContext.Current.User.Identity.Name;
                orderMaster.UpdateDate = DateTime.UtcNow;
                orderMaster.IsSDNOverride = order.IsSDNOverride;
                orderMaster.IsSDN = false;

                db.SaveChanges();

                SaveOrderPostUpdate(order);
            }

            return order.OrderID;
        }

        public static void SaveOrderPostUpdate(OrderMasterFull order)
        {
            if (order.IsSDNOverride != true)
            {
                using (var db = new CMCSQL03Entities())
                {
                    var getOrder = db.tblOrderMaster.Find(order.OrderID);

                    var country = db.tblCountry
                                    .Where(x => x.Country == order.Country &&
                                                x.DoNotShip == true)
                                    .FirstOrDefault();

                    var orderItems = db.tblOrderItem
                                       .Where(x => x.OrderID == order.OrderID &&
                                                   x.AllocateStatus == null)
                                       .ToList();

                    if (country != null)
                    {
                        getOrder.IsSDN = true;

                        foreach (var item in orderItems)
                        {
                            item.CSAllocate = false;
                            db.SaveChanges();
                        }
                    }

                    if (IsSDN(order) == true)
                    {
                        getOrder.IsSDN = true;

                        foreach (var item in orderItems)
                        {
                            item.CSAllocate = false;
                            db.SaveChanges();
                        }
                    }

                    if (order.Country != "0")
                    {
                        int countryId = 0;
                        countryId = db.tblCountry
                                      .Where(x => x.Country.Contains(order.Country))
                                      .Select(x => x.CountryID)
                                      .FirstOrDefault();

                        var ceaseShipOffset = db.tblCeaseShipOffSet
                                                .Where(x => x.ClientID == order.ClientId &&
                                                            x.CountryID == countryId)
                                                .FirstOrDefault();

                        if (ceaseShipOffset != null)
                        {
                            getOrder.CeaseShipOffset = ceaseShipOffset.OffsetDays;
                        }
                    }

                    db.SaveChanges();
                }
            }
        }

        public static bool IsSDN(OrderMasterFull order)
        {
            var file = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Content/sdnlist.txt"));

            if (file.Contains(order.Company))
            {
                return true;
            }

            if (!String.IsNullOrEmpty(order.Street) && file.Contains(order.Street))
            {
                return true;
            }

            if (!String.IsNullOrEmpty(order.Attention) && file.Contains(order.Attention))
            {
                return true;
            }

            return false;
        }

        public static PreferredCarrierViewModel GetPreferredCarrier(string country)
        {
            var preferredCarrier = new PreferredCarrierViewModel();

            if (!String.IsNullOrWhiteSpace(country))
            {
                using (var db = new CMCSQL03Entities())
                {
                    var suggestedCarrier = db.tblPreferredCarrierList
                                             .Where(x => x.CountryName.Contains(country))
                                             .FirstOrDefault();

                    if (suggestedCarrier == null)
                    {
                        return null;
                    }

                    preferredCarrier.CountryCode = suggestedCarrier.CountryCode;
                    preferredCarrier.CountryName = suggestedCarrier.CountryName;
                    preferredCarrier.CommInvoiceReq = suggestedCarrier.CommInvoiceReq;
                    preferredCarrier.NonHazSm = suggestedCarrier.NonHaz_Sm;
                    preferredCarrier.NonHazLg = suggestedCarrier.NonHaz_Lg;
                    preferredCarrier.NonHazIncoTerms = suggestedCarrier.NonHaz_IncoTerms;
                    preferredCarrier.HazIATASm = suggestedCarrier.HazIATA_Sm;
                    preferredCarrier.HazIATALg = suggestedCarrier.HazIATA_Lg;
                    preferredCarrier.HazGroundLQ = suggestedCarrier.HazGround_LQ;
                    preferredCarrier.HazGround = suggestedCarrier.HazGround;
                    preferredCarrier.HazIncoterms = suggestedCarrier.Haz_Incoterms;
                    preferredCarrier.IncotermsAlt = suggestedCarrier.Incoterms_Alt;
                    preferredCarrier.NotesGeneral = suggestedCarrier.Notes_General;
                    preferredCarrier.NotesIATAADR = suggestedCarrier.Notes_IATA_ADR;
                    preferredCarrier.NonHazIncotermsAlt = suggestedCarrier.NonHazIncoterms_Alt;
                    preferredCarrier.HazIncotermsAlt = suggestedCarrier.HazIncoterms_Alt;
                }
            }
            else
            {
                return null;
            }

            return preferredCarrier;
        }

        #endregion Order

        #region Order Item

        public static List<OrderItem> OrderItems(int orderid)
        {
            using (var db = new CMCSQL03Entities())
            {
                var orderItems = (from t in db.tblOrderItem
                                  join pd in db.tblProductDetail
                                  on t.ProductDetailID equals pd.ProductDetailID
                                  into productDetailOrderItem
                                  from productdetail in productDetailOrderItem.DefaultIfEmpty()
                                  where t.OrderID == orderid
                                  orderby t.ItemID
                                  select new OrderItem
                                  {
                                      OrderID = t.OrderID,
                                      ItemID = t.ItemID,
                                      ShelfID = t.ShelfID,
                                      BulkID = t.BulkID,
                                      ProductDetailID = t.ProductDetailID,
                                      ProductCode = t.ProductCode,
                                      ProductName = t.ProductName,
                                      Size = t.Size,
                                      SRSize = t.SRSize,
                                      Bin = t.Bin,
                                      LotNumber = t.LotNumber,
                                      Qty = t.Qty,
                                      CeaseShipDate = t.CeaseShipDate,
                                      ShipDate = t.ShipDate,
                                      Via = t.Via,
                                      BackOrdered = t.BackOrdered,
                                      AllocateStatus = t.AllocateStatus,
                                      AllocatedDate = t.AllocatedDate,
                                      CSAllocate = t.CSAllocate,
                                      NonCMCDelay = t.NonCMCDelay,
                                      RDTransfer = t.RDTransfer,
                                      ItemNotes = t.ItemNotes,
                                      AirUnNumber = productdetail.AIRUNNUMBER,
                                      AirPkGroup = productdetail.AIRPKGRP,
                                      AirHzdClass = productdetail.AIRHAZCL,
                                      GrnUnNumber = productdetail.GRNUNNUMBER,
                                      GrnPkGroup = productdetail.GRNPKGRP,
                                      GrnHzdClass = productdetail.GRNHAZCL,
                                      SeaUnNumber = productdetail.SEAUNNUM,
                                      SeaPkGroup = productdetail.SEAPKGRP,
                                      SeaHzdClass = productdetail.SEAHAZCL,
                                      HarmonizedCode = productdetail.HarmonizedCode,
                                      AlertNotesOrderEntry = productdetail.AlertNotesOrderEntry,
                                      AlertNotesShipping = productdetail.AlertNotesShipping,
                                      QtyAvailable = db.tblStock.Where(x => x.ShelfID != null &&
                                                                            x.ShelfID == t.ShelfID &&
                                                                            x.ShelfStatus == "AVAIL")
                                                                .Select(x => (x.QtyOnHand - x.QtyAllocated)).Sum(),
                                      FreezableList = db.tblProductMaster
                                                        .Where(x => x.ProductMasterID == productdetail.ProductMasterID)
                                                        .Select(x => x.FreezableList).FirstOrDefault(),
                                      CreateDate = t.CreateDate,
                                      CreateUser = t.CreateUser,
                                      UpdateDate = t.UpdateDate,
                                      UpdateUser = t.UpdateUser,
                                      DelayReason = t.DelayReason
                                  }).ToList();

                return orderItems;
            }
        }

        public static OrderItem GetOrderItem(int orderitemid)
        {
            var orderItem = new OrderItem();

            using (var db = new CMCSQL03Entities())
            {
                var getOrderItem = db.tblOrderItem.Find(orderitemid);
                int? clientId = db.tblOrderMaster.Find(getOrderItem.OrderID).ClientID;

                orderItem.CrudMode = "RW";
                orderItem.ClientID = clientId;
                orderItem.ItemID = getOrderItem.ItemID;
                orderItem.OrderID = getOrderItem.OrderID;
                orderItem.CreateDate = getOrderItem.CreateDate;
                orderItem.CreateUser = getOrderItem.CreateUser;
                orderItem.UpdateDate = getOrderItem.UpdateDate;
                orderItem.UpdateUser = getOrderItem.UpdateUser;
                orderItem.ProductDetailID = getOrderItem.ProductDetailID;
                orderItem.ProductCode = getOrderItem.ProductCode;
                orderItem.ProductName = getOrderItem.ProductName;
                orderItem.ShelfID = getOrderItem.ShelfID;
                orderItem.Size = getOrderItem.Size;
                orderItem.SRSize = getOrderItem.SRSize;
                orderItem.Qty = getOrderItem.Qty;
                orderItem.LotNumber = getOrderItem.LotNumber;
                orderItem.ItemShipVia = getOrderItem.Via;
                orderItem.ShipDate = getOrderItem.ShipDate;
                orderItem.CSAllocate = getOrderItem.CSAllocate;
                orderItem.AllocateStatus = getOrderItem.AllocateStatus;
                orderItem.NonCMCDelay = getOrderItem.NonCMCDelay;
                orderItem.RDTransfer = getOrderItem.RDTransfer;
                orderItem.BackOrdered = getOrderItem.BackOrdered;
                orderItem.CarrierInvoiceRcvd = getOrderItem.CarrierInvoiceRcvd;
                orderItem.DelayReason = getOrderItem.DelayReason;
                orderItem.SPSCharge = getOrderItem.SPSCharge;
                orderItem.Status = getOrderItem.Status;
                orderItem.StatusID = null;
                orderItem.ItemNotes = getOrderItem.ItemNotes;
                orderItem.ImportItemID = getOrderItem.ImportItemID;
                orderItem.WasteOrderTotalWeight = getOrderItem.WasteOrderTotalWeight;
                orderItem.AlertNotesShipping = getOrderItem.AlertNotesShipping;
                orderItem.AlertNotesPackOut = getOrderItem.AlertNotesPackout;
                orderItem.AlertNotesOrderEntry = getOrderItem.AlertNotesOrderEntry;
                orderItem.AlertNotesOther = getOrderItem.AlertNotesOther;

                if (getOrderItem.ShipDate != null || 
                    getOrderItem.AllocateStatus == "A")
                {
                    orderItem.CrudMode = "RO";
                }
            }

            return orderItem;
        }

        public static OrderItem CreateOrderItem(int orderid)
        {
            var orderItem = new OrderItem();
            var order = new tblOrderMaster();

            using (var db = new CMCSQL03Entities())
            {
                order = db.tblOrderMaster.Find(orderid);
            }

            orderItem.CrudMode = "RW";
            orderItem.ItemID = -1;
            orderItem.OrderID = order.OrderID;
            orderItem.ClientID = order.ClientID;
            orderItem.ProductDetailID = -1;
            orderItem.Qty = 1;
            orderItem.ItemShipVia = "";
            orderItem.CSAllocate = true;
            orderItem.NonCMCDelay = false;
            orderItem.RDTransfer = false;
            orderItem.CarrierInvoiceRcvd = false;
            orderItem.StatusID = -1;

            return orderItem;
        }

        public static int NewOrderItemId()
        {
            using (var db = new CMCSQL03Entities())
            {
                var newOrderItem = new tblOrderItem();
                db.tblOrderItem.Add(newOrderItem);
                db.SaveChanges();

                return newOrderItem.ItemID;
            }
        }

        public static int SaveOrderItem(OrderItem orderitem)
        {
            using (var db = new CMCSQL03Entities())
            {
                if (orderitem.ItemID < 1)
                {
                    orderitem.ItemID = OrderService.NewOrderItemId();
                    orderitem.CreateDate = DateTime.UtcNow;
                    orderitem.CreateUser = HttpContext.Current.User.Identity.Name;
                }

                var orderMaster = db.tblOrderMaster.Find(orderitem.OrderID);
                var orderItem = db.tblOrderItem.Find(orderitem.ItemID);
                var productDetail = db.tblProductDetail.Find(orderitem.ProductDetailID);
                var productMaster = db.tblProductMaster.Find(productDetail.ProductMasterID);
                var shelfMaster = db.tblShelfMaster.Find(orderitem.ShelfID);

                // Set shelf details for special request
                orderItem.SRSize = 0;
                if (orderitem.SRSize > 0 && shelfMaster.Size == "1SR")
                {
                    orderItem.SRSize = orderitem.SRSize;
                    orderItem.Size = "1SR";
                    orderitem.ShelfID = ShelfMasterService.GetShelfIdProductDetail(orderitem.ProductDetailID, "1SR");
                    orderitem.CSAllocate = false;
                }

                // Get item size details from shelf
                if (shelfMaster != null)
                {
                    orderItem.Size = shelfMaster.Size;
                    shelfMaster.UnitWeight = shelfMaster.UnitWeight ?? orderItem.Weight;
                    orderItem.Weight = shelfMaster.UnitWeight * orderitem.Qty;
                }

                orderItem.OrderID = orderitem.OrderID;
                orderItem.CreateDate = orderitem.CreateDate;
                orderItem.CreateUser = orderitem.CreateUser;
                orderItem.UpdateDate = DateTime.UtcNow;
                orderItem.UpdateUser = HttpContext.Current.User.Identity.Name;
                orderItem.ProductDetailID = orderitem.ProductDetailID;
                orderItem.ProductCode = productDetail.ProductCode;
                orderItem.ProductName = productDetail.ProductName;
                orderItem.ShelfID = orderitem.ShelfID;
                orderItem.Qty = orderitem.Qty;
                orderItem.LotNumber = orderitem.LotNumber;
                orderItem.Via = orderitem.ItemShipVia;
                orderItem.ShipDate = orderitem.ShipDate;
                orderItem.CSAllocate = orderitem.CSAllocate;
                orderItem.AllocateStatus = orderitem.AllocateStatus;
                orderItem.NonCMCDelay = orderitem.NonCMCDelay;
                orderItem.RDTransfer = orderitem.RDTransfer;
                orderItem.BackOrdered = orderitem.BackOrdered;
                orderItem.CarrierInvoiceRcvd = orderitem.CarrierInvoiceRcvd;
                orderItem.Status = orderitem.Status;
                orderItem.DelayReason = orderitem.DelayReason;
                orderItem.SPSCharge = orderitem.SPSCharge;
                orderItem.ItemNotes = orderitem.ItemNotes;
                orderItem.ImportItemID = orderitem.ImportItemID;
                orderItem.WasteOrderTotalWeight = orderitem.WasteOrderTotalWeight;
                orderItem.AlertNotesShipping = productDetail.AlertNotesShipping;
                orderItem.AlertNotesOrderEntry = productDetail.AlertNotesOrderEntry;
                orderItem.AlertNotesPackout = productMaster.AlertNotesPackout;

                // Specific alert for order item
                if (productDetail.AIRUNNUMBER == "UN3082" ||
                    productDetail.AIRUNNUMBER == "UN3077" ||
                    productDetail.GRNUNNUMBER == "UN3082" ||
                    productDetail.GRNUNNUMBER == "UN3077")
                {
                    orderItem.AlertNotesOther = "Products with UN3082 and UN3077 may be shipped as non hazardous if under 5 kg";
                }

                orderMaster.UpdateDate = DateTime.UtcNow;
                orderMaster.UpdateUser = HttpContext.Current.User.Identity.Name;

                // Update order division
                orderMaster.DivisionID = productDetail.DivisionID;

                db.SaveChanges();
            }

            // Update transaction charges
            GenerateOrderTransaction(orderitem.ItemID);

            return orderitem.ItemID;
        }

        public static void DeleteOrderItem(int orderitemid)
        {
            using (var db = new CMCSQL03Entities())
            {
                string deleteQuery;
                int orderItemId = db.tblOrderItem.Find(orderitemid).ItemID;

                if (orderItemId > 0)
                {
                    deleteQuery = "DELETE FROM tblOrderItem WHERE ItemID=" + orderItemId;
                    db.Database.ExecuteSqlCommand(deleteQuery);

                    deleteQuery = "DELETE FROM tblOrderTrans WHERE OrderItemID=" + orderItemId;
                    db.Database.ExecuteSqlCommand(deleteQuery);
                }
            }
        }

        #endregion Order Item

        #region Order Transaction

        public static OrderTrans GetOrderTransaction(int ordertransid)
        {
            var orderTransaction = new OrderTrans();

            using (var db = new CMCSQL03Entities())
            {
                var getOrderTransaction = db.tblOrderTrans.Find(ordertransid);

                orderTransaction.OrderTransID = getOrderTransaction.OrderTransID;
                orderTransaction.OrderId = getOrderTransaction.OrderID;
                orderTransaction.OrderItemId = getOrderTransaction.OrderItemID;
                orderTransaction.ClientId = getOrderTransaction.ClientID;
                orderTransaction.DivisionId = getOrderTransaction.DivisionID;
                orderTransaction.TransDate = getOrderTransaction.TransDate;
                orderTransaction.TransType = getOrderTransaction.TransType;
                orderTransaction.TransQty = getOrderTransaction.TransQty;
                orderTransaction.TransRate = getOrderTransaction.TransRate;
                orderTransaction.TransAmount = getOrderTransaction.TransAmount;
                orderTransaction.BillingTier = getOrderTransaction.BillingTier;
                orderTransaction.BillingRate = getOrderTransaction.BillingRate;
                orderTransaction.BillingCharge = getOrderTransaction.BillingCharge;
                orderTransaction.Comments = getOrderTransaction.Comments;
                orderTransaction.CreateDate = getOrderTransaction.CreateDate;
                orderTransaction.CreateUser = getOrderTransaction.CreateUser;
                orderTransaction.UpdateDate = getOrderTransaction.UpdateDate;
                orderTransaction.UpdateUser = getOrderTransaction.UpdateUser;
            }

            return orderTransaction;
        }

        public static OrderTrans CreateOrderTransaction(int orderid)
        {
            var orderTransaction = new OrderTrans();

            using (var db = new CMCSQL03Entities())
            {
                var order = db.tblOrderMaster.Find(orderid);

                orderTransaction.OrderTransID = -1;
                orderTransaction.OrderId = orderid;
                orderTransaction.TransType = null;
                orderTransaction.CreateDate = DateTime.UtcNow;
                orderTransaction.TransDate = DateTime.UtcNow;
                orderTransaction.CreateUser = HttpContext.Current.User.Identity.Name;
                orderTransaction.ClientId = order.ClientID;
                orderTransaction.DivisionId = order.DivisionID;
            }

            return orderTransaction;
        }

        public static void GenerateOrderTransaction(int orderitemid)
        {
            using (var db = new CMCSQL03Entities())
            {
                var orderItem = db.tblOrderItem.Find(orderitemid);
                var order = db.tblOrderMaster.Find(orderItem.OrderID);

                // Tier 1 sample charge
                string deleteQuery = "DELETE FROM tblOrderTrans WHERE OrderItemID=" + orderItem.ItemID;
                db.Database.ExecuteSqlCommand(deleteQuery);

                var tierSize = db.tblTier
                                 .Where(t => t.ClientID == order.ClientID
                                         && t.Size == orderItem.Size
                                         && t.TierLevel == 1)
                                 .FirstOrDefault();

                if (tierSize != null)
                {
                    var orderTransaction = new tblOrderTrans();

                    orderTransaction.TransDate = DateTime.UtcNow;
                    orderTransaction.OrderItemID = orderItem.ItemID;
                    orderTransaction.OrderID = orderItem.OrderID;
                    orderTransaction.ClientID = order.ClientID;
                    orderTransaction.DivisionID = order.DivisionID;
                    orderTransaction.TransType = "SAMP";
                    orderTransaction.TransQty = orderItem.Qty;
                    orderTransaction.TransRate = tierSize.Price;
                    orderTransaction.TransAmount = orderTransaction.TransQty * orderTransaction.TransRate;
                    orderTransaction.BillingTier = 1;
                    orderTransaction.BillingRate = tierSize.Price;
                    orderTransaction.BillingCharge = orderTransaction.TransQty * orderTransaction.TransRate;
                    orderTransaction.CreateDate = DateTime.UtcNow;
                    orderTransaction.CreateUser = HttpContext.Current.User.Identity.Name;
                    orderTransaction.UpdateDate = DateTime.UtcNow;
                    orderTransaction.UpdateUser = HttpContext.Current.User.Identity.Name;

                    db.tblOrderTrans.Add(orderTransaction);
                    db.SaveChanges();
                }
                else
                {
                    var tierSpecialRequest = db.tblTier
                                               .Where(t => t.ClientID == order.ClientID
                                                       && t.Size == "1SR"
                                                       && t.TierLevel == 1)
                                               .FirstOrDefault();

                    if (tierSpecialRequest != null)
                    {
                        var orderTransaction = new tblOrderTrans();

                        orderTransaction.TransDate = DateTime.UtcNow;
                        orderTransaction.OrderItemID = orderItem.ItemID;
                        orderTransaction.OrderID = orderItem.OrderID;
                        orderTransaction.ClientID = order.ClientID;
                        orderTransaction.DivisionID = order.DivisionID;
                        orderTransaction.TransType = "SAMP";
                        orderTransaction.TransQty = orderItem.Qty;
                        orderTransaction.TransRate = tierSpecialRequest.Price;
                        orderTransaction.TransAmount = orderTransaction.TransQty * orderTransaction.TransRate;
                        orderTransaction.BillingTier = 1;
                        orderTransaction.BillingRate = tierSize.Price;
                        orderTransaction.BillingCharge = orderTransaction.TransQty * orderTransaction.TransRate;
                        orderTransaction.Comments = "Special Request";
                        orderTransaction.CreateDate = DateTime.UtcNow;
                        orderTransaction.CreateUser = HttpContext.Current.User.Identity.Name;
                        orderTransaction.UpdateDate = DateTime.UtcNow;
                        orderTransaction.UpdateUser = HttpContext.Current.User.Identity.Name;

                        db.tblOrderTrans.Add(orderTransaction);
                        db.SaveChanges();
                    }
                }

                // Other charges from shelfmaster
                var shelf = db.tblShelfMaster.Find(orderItem.ShelfID);
                var surcharge = db.tblSurcharge.FirstOrDefault(t => t.ClientID == order.ClientID);

                if (surcharge != null)
                {
                    if (shelf.HazardSurcharge == true)
                    {
                        InsertOrderTransaction(orderItem.ItemID, "HAZD", surcharge.Haz);
                    }

                    if (shelf.FlammableSurcharge == true)
                    {
                        InsertOrderTransaction(orderItem.ItemID, "FLAM", surcharge.Flam);
                    }

                    if (shelf.HeatSurcharge == true)
                    {
                        InsertOrderTransaction(orderItem.ItemID, "HEAT", surcharge.Heat);
                    }

                    if (shelf.RefrigSurcharge == true)
                    {
                        InsertOrderTransaction(orderItem.ItemID, "REFR", surcharge.Refrig);
                    }

                    if (shelf.FreezerSurcharge == true)
                    {
                        InsertOrderTransaction(orderItem.ItemID, "FREZ", surcharge.Freezer);
                    }

                    if (shelf.CleanSurcharge == true)
                    {
                        InsertOrderTransaction(orderItem.ItemID, "CLEN", surcharge.Clean);
                    }

                    if (shelf.BlendSurcharge == true)
                    {
                        InsertOrderTransaction(orderItem.ItemID, "BLND", 0);
                    }

                    if (shelf.NalgeneSurcharge == true)
                    {
                        InsertOrderTransaction(orderItem.ItemID, "NALG", surcharge.Nalgene);
                    }

                    if (shelf.NitrogenSurcharge == true)
                    {
                        InsertOrderTransaction(orderItem.ItemID, "NITR", 0);
                    }

                    if (shelf.BiocideSurcharge == true)
                    {
                        InsertOrderTransaction(orderItem.ItemID, "BIOC", 0);
                    }

                    if (shelf.KosherSurcharge == true)
                    {
                        InsertOrderTransaction(orderItem.ItemID, "KOSH", 0);
                    }

                    if (shelf.LabelSurcharge == true)
                    {
                        InsertOrderTransaction(orderItem.ItemID, "LABL", surcharge.LabelFee);
                    }

                    if (shelf.OtherSurcharge == true)
                    {
                        InsertOrderTransaction(orderItem.ItemID, "OTHR", shelf.OtherSurchargeAmt);
                    }
                }
            }
        }

        public static void InsertOrderTransaction(int? ItemID, string TransType, decimal? TransRate)
        {
            using (var db = new CMCSQL03Entities())
            {
                string deleteQuery = "DELETE FROM tblOrderTrans WHERE OrderItemID=" + ItemID + " AND Transtype = '" + TransType + "'";
                db.Database.ExecuteSqlCommand(deleteQuery);

                var orderItem = db.tblOrderItem.Find(ItemID);
                var order = db.tblOrderMaster.Find(orderItem.OrderID);

                var newOrderTransaction = new tblOrderTrans();

                newOrderTransaction.TransDate = DateTime.UtcNow;
                newOrderTransaction.OrderItemID = ItemID;
                newOrderTransaction.OrderID = orderItem.OrderID;
                newOrderTransaction.ClientID = order.ClientID;
                newOrderTransaction.DivisionID = order.DivisionID;
                newOrderTransaction.TransDate = DateTime.UtcNow;
                newOrderTransaction.TransType = TransType;
                newOrderTransaction.TransQty = orderItem.Qty;
                newOrderTransaction.TransRate = TransRate;
                newOrderTransaction.TransAmount = newOrderTransaction.TransQty * newOrderTransaction.TransRate;
                newOrderTransaction.CreateDate = DateTime.UtcNow;
                newOrderTransaction.CreateUser = HttpContext.Current.User.Identity.Name;
                newOrderTransaction.UpdateDate = DateTime.UtcNow;
                newOrderTransaction.UpdateUser = HttpContext.Current.User.Identity.Name;

                db.tblOrderTrans.Add(newOrderTransaction);
                db.SaveChanges();
            }
        }

        public static int SaveOrderTransaction(OrderTrans ordertransaction)
        {
            using (var db = new CMCSQL03Entities())
            {
                if (ordertransaction.OrderTransID == -1)
                {
                    var newOrderTransaction = new tblOrderTrans();
                    newOrderTransaction.CreateDate = DateTime.UtcNow;
                    newOrderTransaction.CreateUser = HttpContext.Current.User.Identity.Name;

                    db.tblOrderTrans.Add(newOrderTransaction);
                    db.SaveChanges();

                    ordertransaction.OrderTransID = newOrderTransaction.OrderTransID;
                }

                var orderTransaction = db.tblOrderTrans.Find(ordertransaction.OrderTransID);

                orderTransaction.OrderID = ordertransaction.OrderId;
                orderTransaction.OrderItemID = ordertransaction.OrderItemId;
                orderTransaction.ClientID = ordertransaction.ClientId;
                orderTransaction.DivisionID = ordertransaction.DivisionId;
                orderTransaction.TransDate = ordertransaction.TransDate;
                orderTransaction.TransType = ordertransaction.TransType;
                orderTransaction.TransQty = ordertransaction.TransQty ?? 1;
                orderTransaction.TransRate = GetTransactionRate(ordertransaction.ClientId, ordertransaction.TransType, ordertransaction.TransRate);
                orderTransaction.TransAmount = orderTransaction.TransQty * orderTransaction.TransRate;
                orderTransaction.Comments = ordertransaction.Comments;
                orderTransaction.CreateDate = ordertransaction.CreateDate;
                orderTransaction.CreateUser = ordertransaction.CreateUser;
                orderTransaction.UpdateDate = DateTime.UtcNow;
                orderTransaction.UpdateUser = HttpContext.Current.User.Identity.Name;

                db.SaveChanges();
            }

            return ordertransaction.OrderTransID;
        }

        public static decimal? GetTransactionRate(int? clientid, string transtype, decimal? transrate)
        {
            decimal? rate = 0;

            using (var db = new CMCSQL03Entities())
            {
                var rates = db.tblRates.FirstOrDefault(t => t.ClientID == clientid);

                switch (transtype)
                {
                    case "Air Hazard Only":
                        rate = rates.AirHazardOnly;
                        break;

                    case "Certificate Of Origin":
                        rate = rates.CertificateOfOrigin;
                        break;

                    case "CMC Pack":
                        rate = rates.CMCPack;
                        break;

                    case "Cool Pack":
                        rate = rates.CoolPack;
                        break;

                    case "Credit Card Fee":
                        rate = rates.CreditCardFee;
                        break;

                    case "Credit Card Order":
                        rate = rates.CreditCardOrder;
                        break;

                    case "Document Handling":
                        rate = rates.DocumentHandling;
                        break;

                    case "Empty Packaging":
                        rate = rates.EmptyPackaging;
                        break;

                    case "External System":
                        rate = rates.ExternalSystem;
                        break;

                    case "Follow Up Order":
                        rate = rates.FollowUpOrder;
                        break;

                    case "Freezer Pack":
                        rate = rates.FreezerPack;
                        break;

                    case "GHS Labels":
                        rate = rates.GHSLabels;
                        break;

                    case "Inactive Products":
                        rate = rates.InactiveProducts;
                        break;

                    case "Isolation":
                        rate = rates.Isolation;
                        break;

                    case "Isolation Box":
                        rate = rates.IsolationBox;
                        break;

                    case "IT Fee":
                        rate = rates.ITFee;
                        break;

                    case "Label Maintainance":
                        rate = rates.LabelMaintainance;
                        break;

                    case "Label Stock":
                        rate = rates.LabelStock;
                        break;

                    case "Labels Printed":
                        rate = rates.LabelsPrinted;
                        break;

                    case "Labor Relabel":
                        rate = rates.LaborRelabel;
                        break;

                    case "Literature Fee":
                        rate = rates.LiteratureFee;
                        break;

                    case "Limited Quantity":
                        rate = rates.LimitedQuantity;
                        break;

                    case "Manual Handling":
                        rate = rates.ManualHandling;
                        break;

                    case "MSDS Prints":
                        rate = rates.MSDSPrints;
                        break;

                    case "New Label Setup":
                        rate = rates.NewLabelSetup;
                        break;

                    case "New Product Setup":
                        rate = rates.NewProductSetup;
                        break;

                    case "Oberk Pack":
                        rate = rates.OberkPack;
                        break;

                    case "Order Entry":
                        rate = rates.OrderEntry;
                        break;

                    case "Over Pack":
                        rate = rates.OverPack;
                        break;

                    case "Pallet Return":
                        rate = rates.PalletReturn;
                        break;

                    case "Poison Pack":
                        rate = rates.PoisonPack;
                        break;

                    case "Product Setup Changes":
                        rate = rates.ProductSetupChanges;
                        break;

                    case "QC Storage":
                        rate = rates.QCStorage;
                        break;

                    case "RD Handling ADR":
                        rate = rates.RDHandlingADR;
                        break;

                    case "RD Handling IATA":
                        rate = rates.RDHandlingIATA;
                        break;

                    case "RD Handling LQ":
                        rate = rates.RDHandlingLQ;
                        break;

                    case "RD Handling Non Hazard":
                        rate = rates.RDHandlingNonHazard;
                        break;

                    case "Refrigerator Storage":
                        rate = rates.RefrigeratorStorage;
                        break;

                    case "Relabels":
                        rate = rates.Relabels;
                        break;

                    case "Rush Shipment":
                        rate = rates.RushShipment;
                        break;

                    case "SPA 197 Applied":
                        rate = rates.SPA197Applied;
                        break;

                    case "SPS Paid Order":
                        rate = rates.SPSPaidOrder;
                        break;

                    case "UN Box":
                        rate = rates.UNBox;
                        break;

                    case "Warehouse Storage":
                        rate = rates.WarehouseStorage;
                        break;

                    case "WHMIS Labels":
                        rate = rates.WHMISLabels;
                        break;

                    default:
                        rate = transrate;
                        break;
                }
            }

            return rate;
        }

        public static void DeleteTransaction(int ordertransid)
        {
            using (var db = new CMCSQL03Entities())
            {
                string deleteQuery = "DELETE FROM tblOrderTrans WHERE OrderTransID=" + ordertransid;
                db.Database.ExecuteSqlCommand(deleteQuery);
            }
        }

        #endregion Order Transaction

        #region Allocate Order Item

        public static int AllocateShelf(int OrderID, bool IncludeQCStock)
        {
            int allocatedItemCount = 0;
            bool isAllocated = false;

            using (var db = new CMCSQL03Entities())
            {
                var orderItems = (from orderitem in db.tblOrderItem
                                  join ordermaster in db.tblOrderMaster on orderitem.OrderID equals ordermaster.OrderID
                                  where orderitem.OrderID == OrderID
                                  && orderitem.ShipDate == null
                                  && orderitem.AllocateStatus == null
                                  && orderitem.CSAllocate == true
                                  select new { orderitem, ordermaster }).ToList();

                if (orderItems == null)
                {
                    return allocatedItemCount;
                }

                // Assign stock to order items
                foreach (var item in orderItems)
                {
                    var stock = (from itemstock in db.tblStock
                                 join itembulk in db.tblBulk on itemstock.BulkID equals itembulk.BulkID
                                 orderby itembulk.CeaseShipDate
                                 where itemstock.ShelfID == item.orderitem.ShelfID
                                 select new
                                 {
                                     StockID = itemstock.StockID,
                                     ProductMasterId = itembulk.ProductMasterID,
                                     Warehouse = itemstock.Warehouse,
                                     QtyOnHand = itemstock.QtyOnHand,
                                     QtyAllocated = itemstock.QtyAllocated ?? 0,
                                     ShelfBin = itemstock.Bin,
                                     BulkBin = itembulk.Bin,
                                     ShelfStatus = itemstock.ShelfStatus,
                                     ExpirationDate = itembulk.ExpirationDate,
                                     CeaseShipDate = itembulk.CeaseShipDate,
                                     LotNumber = itembulk.LotNumber
                                 }).ToList();

                    int ceaseShipOffSet = Convert.ToInt32(item.ordermaster.CeaseShipOffset);

                    // Filter list to shippable stock
                    if (ceaseShipOffSet != 0)
                    {
                        var ceaseShipBufferDate = DateTime.UtcNow.AddDays(ceaseShipOffSet);

                        stock = stock.Where(s => s.CeaseShipDate > ceaseShipBufferDate).ToList();
                    }

                    // Filter list to requested lot
                    if (item.orderitem.LotNumber != null)
                    {
                        stock = stock.Where(s => s.LotNumber == item.orderitem.LotNumber).ToList();
                    }

                    // Filter list to status
                    if (IncludeQCStock == true)
                    {
                        stock = stock.Where(s => s.ShelfStatus != "AVAIL").ToList();
                        stock = stock.Where(s => s.ShelfStatus != "HOLD").ToList();
                    }
                    else
                    {
                        stock = stock.Where(s => s.ShelfStatus == "AVAIL").ToList();
                    }

                    // Order filtered list
                    stock = stock.OrderBy(s => s.CeaseShipDate).ToList();

                    // Fill order item with stock that satisfies quantity
                    foreach (var row in stock)
                    {
                        var log = new InventoryLog();

                        if (isAllocated == false)
                        {
                            if (row.QtyOnHand - row.QtyAllocated >= item.orderitem.Qty)
                            {
                                allocatedItemCount = allocatedItemCount + 1;

                                var updateStock = db.tblStock.Find(row.StockID);

                                updateStock.QtyAllocated = (updateStock.QtyAllocated ?? 0) + item.orderitem.Qty;
                                updateStock.QtyAvailable = updateStock.QtyOnHand - item.orderitem.Qty;
                                log.StockId = item.orderitem.AllocatedStockID = row.StockID;
                                log.BulkId = item.orderitem.BulkID = updateStock.BulkID;
                                log.Warehouse = item.orderitem.Warehouse = row.Warehouse;
                                log.LotNumber = item.orderitem.LotNumber = row.LotNumber;
                                log.ShelfBin = item.orderitem.Bin = row.ShelfBin;
                                log.ExpirationDate = item.orderitem.ExpirationDate = row.ExpirationDate;
                                log.CeaseShipDate = item.orderitem.CeaseShipDate = row.CeaseShipDate;
                                item.orderitem.AllocateStatus = "A";
                                item.orderitem.AllocatedDate = DateTime.UtcNow;
                                item.orderitem.UpdateDate = DateTime.UtcNow;
                                item.orderitem.UpdateUser = HttpContext.Current.User.Identity.Name;

                                db.SaveChanges();

                                isAllocated = true;

                                // Insert inventory log record
                                decimal? unitWeight = ShelfMasterService.GetUnitWeight(item.orderitem.ShelfID);

                                var productMaster = db.tblProductMaster
                                                      .Where(x => x.ProductMasterID == row.ProductMasterId)
                                                      .Select(x => new { x.MasterCode, x.MasterName })
                                                      .FirstOrDefault();

                                log.LogType = "SS-ALC";
                                log.OrderNumber = item.orderitem.OrderID;
                                log.LogQty = item.orderitem.Qty;
                                log.ClientId = item.ordermaster.ClientID;
                                log.ProductMasterId = row.ProductMasterId;
                                log.ProductDetailId = item.orderitem.ProductDetailID;
                                log.MasterCode = productMaster.MasterCode;
                                log.MasterName = productMaster.MasterName;
                                log.ProductCode = item.orderitem.ProductCode;
                                log.ProductName = item.orderitem.ProductName;
                                log.LogAmount = unitWeight * item.orderitem.Qty;
                                log.CurrentQtyAvailable = updateStock.QtyAvailable;
                                log.CurrentWeightAvailable = updateStock.QtyAvailable * unitWeight;
                                log.Status = row.ShelfStatus;
                                log.BulkBin = row.BulkBin;
                                log.Size = item.orderitem.Size;
                                log.CeaseShipDate = row.CeaseShipDate;
                                log.LogNotes = "Shelf stock allocated";

                                InventoryService.LogInventoryUpdates(log);
                            }
                        }
                    }

                    isAllocated = false;
                }
            }

            return allocatedItemCount;
        }

        public static void ReverseAllocatedItem(int orderitemid)
        {
            using (var db = new CMCSQL03Entities())
            {
                var isReversed = false;
                var log = new InventoryLog();

                var orderItem = db.tblOrderItem
                                  .Where(x => x.ItemID == orderitemid &&
                                              x.AllocateStatus == "A" &&
                                              x.ShipDate == null)
                                  .FirstOrDefault();

                var order = db.tblOrderMaster
                              .Where(x => x.OrderID == orderItem.OrderID)
                              .Select(x => new { x.ClientID, x.OrderID })
                              .FirstOrDefault();

                var bulk = db.tblBulk.Find(orderItem.BulkID);

                var product = db.tblProductMaster
                                .Where(x => x.ProductMasterID == bulk.ProductMasterID)
                                .Select(x => new { x.MasterCode, x.MasterName })
                                .FirstOrDefault();

                decimal? unitWeight = ShelfMasterService.GetUnitWeight(orderItem.ShelfID);

                log.LogType = "RV-ALC";
                log.ClientId = order.ClientID;
                log.OrderNumber = order.OrderID;
                log.ProductDetailId = orderItem.ProductDetailID;
                log.ProductMasterId = bulk.ProductMasterID;
                log.ProductCode = orderItem.ProductCode;
                log.ProductName = orderItem.ProductName;
                log.MasterCode = product.MasterCode;
                log.MasterName = product.MasterName;
                log.Size = orderItem.Size;
                log.LogQty = orderItem.Qty;
                log.BulkId = bulk.BulkID;
                log.BulkBin = bulk.Bin;
                log.Warehouse = orderItem.Warehouse;
                log.LotNumber = bulk.LotNumber;
                log.ExpirationDate = bulk.ExpirationDate;
                log.CeaseShipDate = bulk.CeaseShipDate;
                log.DateReceived = bulk.ReceiveDate;
                log.LogAmount = orderItem.Qty * unitWeight;

                // Bulk item
                if (orderItem.AllocatedStockID == null && orderItem.AllocatedBulkID != null)
                {
                    bulk.Qty += orderItem.Qty;
                    bulk.CurrentWeight += orderItem.Weight;
                    bulk.BulkStatus = "AVAIL";
                    bulk.MarkedForReturn = false;
                    bulk.UpdateDate = DateTime.UtcNow;
                    bulk.UpdateUser = HttpContext.Current.User.Identity.Name;

                    log.CurrentQtyAvailable = bulk.Qty;
                    log.CurrentWeightAvailable = bulk.CurrentWeight;
                    log.Status = bulk.BulkStatus;
                    log.LogNotes = "Reverse bulk stock allocation.";

                    isReversed = true;
                }

                // Shelf stock item
                if (orderItem.AllocatedStockID != null)
                {
                    var stock = db.tblStock.Find(orderItem.AllocatedStockID);

                    stock.QtyAvailable += orderItem.Qty;
                    stock.QtyAllocated -= orderItem.Qty;
                    stock.MarkedForReturn = false;
                    stock.QtyAllocated = (stock.QtyAllocated < 0) ? 0 : stock.QtyAllocated;
                    stock.ShelfStatus = (stock.ShelfStatus == "RETURN") ? "AVAIL" : stock.ShelfStatus;
                    stock.UpdateDate = DateTime.UtcNow;
                    stock.UpdateUser = HttpContext.Current.User.Identity.Name;

                    log.StockId = stock.StockID;
                    log.ShelfBin = stock.Bin;
                    log.CurrentQtyAvailable = stock.QtyAvailable;
                    log.CurrentWeightAvailable = stock.QtyAvailable * unitWeight;
                    log.Status = stock.ShelfStatus;
                    log.LogNotes = "Reverse shelf stock allocation.";

                    isReversed = true;
                }

                // Clear order item allocation fields
                if (isReversed == true)
                {
                    orderItem.AllocatedStockID = null;
                    orderItem.AllocatedBulkID = null;
                    orderItem.AllocateStatus = null;
                    orderItem.AllocatedDate = null;
                    orderItem.Bin = null;
                    orderItem.LotNumber = null;
                    orderItem.Warehouse = null;
                    orderItem.ExpirationDate = null;
                    orderItem.CeaseShipDate = null;
                    orderItem.ShipDate = null;
                    orderItem.UpdateDate = DateTime.UtcNow;
                    orderItem.UpdateUser = HttpContext.Current.User.Identity.Name;

                    db.SaveChanges();

                    InventoryService.LogInventoryUpdates(log);
                }
            }
        }

        #endregion Allocate Order Item

        #region Return Order

        public static async Task<int> AddBulkItemToReturnOrder(int orderid, int bulkid)
        {
            var log = new InventoryLog();

            using (var db = new CMCSQL03Entities())
            {
                int newOrderItemId = OrderService.NewOrderItemId();
                var newOrderItem = db.tblOrderItem.Find(newOrderItemId);

                var bulk = db.tblBulk.Find(bulkid);
                var productMaster = db.tblProductMaster
                                      .Where(x => x.ProductMasterID == bulk.ProductMasterID)
                                      .Select(x => new
                                      {
                                          x.ProductMasterID,
                                          x.ClientID,
                                          x.MasterCode,
                                          x.MasterName
                                      }).FirstOrDefault();

                var productDetailId = ProductService.GetProductDetailId(productMaster.ProductMasterID);
                var productDetail = db.tblProductDetail.Find(productDetailId);

                newOrderItem.ShelfID = ShelfMasterService.GetShelfIdProductMaster(productMaster.ProductMasterID, bulk.UM);
                newOrderItem.OrderID = orderid;
                newOrderItem.AllocatedBulkID = newOrderItem.BulkID = bulk.BulkID;
                newOrderItem.ProductDetailID = productDetailId;
                newOrderItem.ProductCode = productMaster.MasterCode;
                newOrderItem.ProductName = productMaster.MasterName;
                newOrderItem.LotNumber = bulk.LotNumber;
                newOrderItem.Size = bulk.UM;
                newOrderItem.Qty = 1;
                newOrderItem.Weight = bulk.CurrentWeight;
                newOrderItem.Bin = bulk.Bin;
                newOrderItem.Warehouse = bulk.Warehouse;
                newOrderItem.CSAllocate = true;
                newOrderItem.AllocateStatus = "A";
                newOrderItem.AllocatedDate = DateTime.UtcNow;
                newOrderItem.GrnUnNumber = productDetail.GRNUNNUMBER;
                newOrderItem.GrnPkGroup = productDetail.GRNPKGRP;
                newOrderItem.AirUnNumber = productDetail.AIRUNNUMBER;
                newOrderItem.AirPkGroup = productDetail.AIRPKGRP;
                newOrderItem.AlertNotesOrderEntry = productDetail.AlertNotesOrderEntry;
                newOrderItem.AlertNotesShipping = productDetail.AlertNotesShipping;
                newOrderItem.ItemNotes = "Return bulk order item: bulk container for return is " + bulk.UM + " with a current weight of " + bulk.CurrentWeight;
                newOrderItem.CreateDate = DateTime.UtcNow;
                newOrderItem.CreateUser = HttpContext.Current.User.Identity.Name;
                newOrderItem.UpdateDate = DateTime.UtcNow;
                newOrderItem.UpdateUser = HttpContext.Current.User.Identity.Name;

                bulk.CurrentWeight = 0;
                bulk.BulkStatus = "RETURN";
                bulk.MarkedForReturn = true;
                bulk.UpdateDate = DateTime.UtcNow;
                bulk.UpdateUser = HttpContext.Current.User.Identity.Name;

                db.SaveChanges();

                // Insert inventory log record
                log.LogType = "BS-RTN";
                log.ClientId = productMaster.ClientID;
                log.OrderNumber = newOrderItem.OrderID;
                log.BulkId = newOrderItem.AllocatedBulkID;
                log.ProductDetailId = newOrderItem.ProductDetailID;
                log.ProductCode = newOrderItem.ProductCode;
                log.ProductName = newOrderItem.ProductName;
                log.ProductMasterId = productMaster.ProductMasterID;
                log.MasterCode = productMaster.MasterCode;
                log.MasterName = productMaster.MasterName;
                log.LotNumber = newOrderItem.LotNumber;
                log.Size = newOrderItem.Size;
                log.LogQty = newOrderItem.Qty;
                log.LogAmount = newOrderItem.Weight;
                log.BulkBin = newOrderItem.Bin;
                log.Warehouse = newOrderItem.Warehouse;
                log.LogNotes = newOrderItem.ItemNotes;

                InventoryService.LogInventoryUpdates(log);

                OrderService.GenerateOrderTransaction(newOrderItemId);
            }

            return orderid;
        }

        public static async Task<int> AddStockItemToReturnOrder(int orderid, int stockid, int clientid)
        {
            var log = new InventoryLog();

            using (var db = new CMCSQL03Entities())
            {
                int newOrderItemId = OrderService.NewOrderItemId();
                var newOrderItem = db.tblOrderItem.Find(newOrderItemId);

                var stockItem = db.tblStock.Find(stockid);
                var shelf = db.tblShelfMaster
                              .Where(x => x.ShelfID == stockItem.ShelfID)
                              .Select(x => new
                              {
                                  x.ProductDetailID,
                                  x.ShelfID,
                                  x.Size,
                                  x.UnitWeight
                              })
                              .FirstOrDefault();

                var bulk = db.tblBulk
                             .Where(x => x.BulkID == stockItem.BulkID)
                             .Select(x => new
                             {
                                 x.ProductMasterID,
                                 x.LotNumber
                             })
                             .FirstOrDefault();

                var productDetail = db.tblProductDetail
                                      .Where(x => x.ProductDetailID == shelf.ProductDetailID)
                                      .Select(x => new
                                      {
                                          x.ProductDetailID,
                                          x.ProductCode,
                                          x.ProductName,
                                          x.GRNUNNUMBER,
                                          x.GRNPKGRP,
                                          x.AIRUNNUMBER,
                                          x.AIRPKGRP,
                                          x.AlertNotesOrderEntry,
                                          x.AlertNotesShipping
                                      })
                                      .FirstOrDefault();

                newOrderItem.OrderID = orderid;
                newOrderItem.ShelfID = shelf.ShelfID;
                newOrderItem.ProductDetailID = productDetail.ProductDetailID;
                newOrderItem.ProductCode = productDetail.ProductCode;
                newOrderItem.ProductName = productDetail.ProductName;
                newOrderItem.LotNumber = bulk.LotNumber;
                newOrderItem.Qty = stockItem.QtyOnHand;
                newOrderItem.Size = shelf.Size;
                newOrderItem.Weight = shelf.UnitWeight * stockItem.QtyOnHand;
                newOrderItem.Bin = stockItem.Bin;
                newOrderItem.CSAllocate = true;
                newOrderItem.AllocatedBulkID = stockItem.BulkID;
                newOrderItem.AllocatedStockID = stockItem.StockID;
                newOrderItem.AllocateStatus = "A";
                newOrderItem.AllocatedDate = DateTime.UtcNow;
                newOrderItem.Warehouse = stockItem.Warehouse;
                newOrderItem.GrnUnNumber = productDetail.GRNUNNUMBER;
                newOrderItem.GrnPkGroup = productDetail.GRNPKGRP;
                newOrderItem.AirUnNumber = productDetail.AIRUNNUMBER;
                newOrderItem.AirPkGroup = productDetail.AIRPKGRP;
                newOrderItem.AlertNotesOrderEntry = productDetail.AlertNotesOrderEntry;
                newOrderItem.AlertNotesShipping = productDetail.AlertNotesShipping;
                newOrderItem.ItemNotes = "Return Shelf Order Item";
                newOrderItem.CreateDate = DateTime.UtcNow;
                newOrderItem.CreateUser = HttpContext.Current.User.Identity.Name;
                newOrderItem.UpdateDate = DateTime.UtcNow;
                newOrderItem.UpdateUser = HttpContext.Current.User.Identity.Name;

                // Adjust stock
                stockItem.QtyAllocated = stockItem.QtyAvailable;
                stockItem.QtyAvailable = 0;
                stockItem.ShelfStatus = "RETURN";
                stockItem.MarkedForReturn = true;
                stockItem.UpdateDate = DateTime.UtcNow;
                stockItem.UpdateUser = HttpContext.Current.User.Identity.Name;

                db.SaveChanges();

                // Insert inventory log record
                log.LogType = "SS-RTN";
                log.ClientId = clientid;
                log.OrderNumber = newOrderItem.OrderID = orderid;
                log.ProductDetailId = newOrderItem.ProductDetailID;
                log.ProductMasterId = bulk.ProductMasterID;
                log.LotNumber = newOrderItem.LotNumber;
                log.LogQty = newOrderItem.Qty;
                log.Size = newOrderItem.Size;
                log.LogAmount = newOrderItem.Weight;
                log.ShelfBin = newOrderItem.Bin;
                log.BulkId = newOrderItem.AllocatedBulkID;
                log.StockId = newOrderItem.AllocatedStockID;
                log.Warehouse = newOrderItem.Warehouse;
                log.LogNotes = newOrderItem.ItemNotes;

                InventoryService.LogInventoryUpdates(log);

                OrderService.GenerateOrderTransaction(newOrderItemId);
            }

            return orderid;
        }

        #endregion Return Order

        #region SPS Billing

        public static OrderSPSBilling SPSBilling(int orderid)
        {
            var orderSPSBilling = new OrderSPSBilling();

            using (var db = new CMCSQL03Entities())
            {
                var getSPSBilling = db.tblOrderSPSBilling
                                      .FirstOrDefault(i => i.OrderID == orderid);

                if (getSPSBilling == null)
                {
                    orderSPSBilling.SPSBillingID = -1;
                    orderSPSBilling.OrderId = orderid;
                    orderSPSBilling.PriceCost = CalculateSPSSumCharge(orderid);

                    return orderSPSBilling;
                }

                orderSPSBilling.SPSBillingID = getSPSBilling.SPSBillingID;
                orderSPSBilling.OrderId = getSPSBilling.OrderID;
                orderSPSBilling.Type = getSPSBilling.Type;
                orderSPSBilling.TaxId = getSPSBilling.TaxID;
                orderSPSBilling.Currency = getSPSBilling.Currency;
                orderSPSBilling.PriceCost = CalculateSPSSumCharge(orderid);
                orderSPSBilling.FreightCost = getSPSBilling.FreightCost;
                orderSPSBilling.ShippedWeight = getSPSBilling.ShippedWeight;
                orderSPSBilling.InvoiceTitle = getSPSBilling.InvoiceTitle;
                orderSPSBilling.InvoiceFirstName = getSPSBilling.InvoiceFirstName;
                orderSPSBilling.InvoiceLastName = getSPSBilling.InvoiceLastName;
                orderSPSBilling.InvoiceCompany = getSPSBilling.InvoiceCompany;
                orderSPSBilling.InvoiceAddress1 = getSPSBilling.InvoiceAddress1;
                orderSPSBilling.InvoiceAddress2 = getSPSBilling.InvoiceAddress2;
                orderSPSBilling.InvoiceAddress3 = getSPSBilling.InvoiceAddress3;
                orderSPSBilling.InvoiceCity = getSPSBilling.InvoiceCity;
                orderSPSBilling.InvoiceState = getSPSBilling.InvoiceState;
                orderSPSBilling.InvoicePostalCode = getSPSBilling.InvoicePostalCode;
                orderSPSBilling.InvoiceCountry = getSPSBilling.InvoiceCountry;
                orderSPSBilling.InvoicePhone = getSPSBilling.InvoicePhone;
                orderSPSBilling.InvoiceEmail = getSPSBilling.InvoiceEmail;
                orderSPSBilling.UpdateDate = DateTime.UtcNow;
                orderSPSBilling.UpdateUser = HttpContext.Current.User.Identity.Name;
            }

            return orderSPSBilling;
        }

        private static decimal? CalculateSPSSumCharge(int orderid)
        {
            using (var db = new CMCSQL03Entities())
            {
                var sumSPSCharge = db.tblOrderItem
                                     .Where(t => t.OrderID == orderid)
                                     .Sum(t => t.SPSCharge);

                return sumSPSCharge;
            }
        }

        public static void SaveSPSBillingDetails(OrderSPSBilling orderspsbilling)
        {
            using (var db = new CMCSQL03Entities())
            {
                if (orderspsbilling.SPSBillingID < -1)
                {
                    var newOrderSPSBilling = new tblOrderSPSBilling();
                    newOrderSPSBilling.OrderID = orderspsbilling.OrderId;
                    db.tblOrderSPSBilling.Add(newOrderSPSBilling);

                    db.SaveChanges();

                    orderspsbilling.SPSBillingID = newOrderSPSBilling.SPSBillingID;
                }

                var orderSPSBilling = db.tblOrderSPSBilling.Find(orderspsbilling.SPSBillingID);

                orderSPSBilling.Type = "Invoice";
                orderSPSBilling.Currency = "EUR";
                orderSPSBilling.TaxID = orderspsbilling.TaxId;
                orderSPSBilling.PriceCost = CalculateSPSSumCharge(orderspsbilling.OrderId);
                orderSPSBilling.FreightCost = orderspsbilling.FreightCost;
                orderSPSBilling.ShippedWeight = orderspsbilling.ShippedWeight;
                orderSPSBilling.InvoiceTitle = orderspsbilling.InvoiceTitle;
                orderSPSBilling.InvoiceFirstName = orderspsbilling.InvoiceFirstName;
                orderSPSBilling.InvoiceLastName = orderspsbilling.InvoiceLastName;
                orderSPSBilling.InvoiceCompany = orderspsbilling.InvoiceCompany;
                orderSPSBilling.InvoiceAddress1 = orderspsbilling.InvoiceAddress1;
                orderSPSBilling.InvoiceAddress2 = orderspsbilling.InvoiceAddress2;
                orderSPSBilling.InvoiceAddress3 = orderspsbilling.InvoiceAddress3;
                orderSPSBilling.InvoiceCity = orderspsbilling.InvoiceCity;
                orderSPSBilling.InvoiceState = orderspsbilling.InvoiceState;
                orderSPSBilling.InvoicePostalCode = orderspsbilling.InvoicePostalCode;
                orderSPSBilling.InvoiceCountry = orderspsbilling.InvoiceCountry;
                orderSPSBilling.InvoicePhone = orderspsbilling.InvoicePhone;
                orderSPSBilling.InvoiceEmail = orderspsbilling.InvoiceEmail;
                orderSPSBilling.UpdateDate = DateTime.UtcNow;
                orderSPSBilling.UpdateUser = HttpContext.Current.User.Identity.Name;

                db.SaveChanges();
            }
        }

        #endregion SPS Billing

        #region Order Import Methods

        public static void ImportOrders(out int OrdersImportedCount, out TimeSpan runTime)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            string location = "EU";
            OrdersImportedCount = 0;

            // Prepare items for import by assigning client, products and shelf ids
            PrepareImport();

            using (var db = new CMCSQL03Entities())
            {
                // Get list of unique rows that have not been imported
                var orders = db.tblOrderImport
                               .Where(t => t.Location_MDB == location)
                               .Where(t => t.ImportStatus != "IMPORTED")
                               .Where(t => t.ClientID > 0)
                               .ToList();

                if (orders.Count < 1)
                {
                    stopWatch.Stop();
                    runTime = stopWatch.Elapsed;

                    return;
                }

                // Create orders
                var uniqueOrderGuids = orders.DistinctBy(t => t.GUID).ToList();

                foreach (var row in uniqueOrderGuids)
                {
                    int newOrderId = ImportOrderHeader(row);

                    // Insert order items
                    var importOrderItems = orders.Where(t => t.GUID == row.GUID).ToList();

                    foreach (var item in importOrderItems)
                    {
                        if (item.ProductDetailID > 0 && item.ShelfID > 0)
                        {
                            try
                            {
                                ImportOrderItem(newOrderId, item);
                            }
                            catch (Exception)
                            {
                                SaveUnknownOrderItem(newOrderId, item);
                            }
                        }
                        else
                        {
                            SaveUnknownOrderItem(newOrderId, item);
                        }

                        // Update order import table
                        item.ImportStatus = "IMPORTED";
                        item.CMCLocation = location;
                        item.OrderID = newOrderId;

                        db.SaveChanges();
                    }

                    OrdersImportedCount = OrdersImportedCount + 1;
                }
            }

            stopWatch.Stop();
            runTime = stopWatch.Elapsed;

            return;
        }

        private static void PrepareImport()
        {
            string location = "EU";

            using (var db = new CMCSQL03Entities())
            {
                var orderImports = db.tblOrderImport
                                     .Where(t => t.Location_MDB == location)
                                     .Where(t => t.Status == "0")
                                     .Where(t => t.ImportStatus == null ||
                                                 t.ImportStatus == "FAIL")
                                     .ToList();

                if (orderImports.Count < 1)
                {
                    return;
                }

                var clients = db.tblClient.AsQueryable();
                var divisions = db.tblDivision.AsQueryable();
                var shelfItems = db.tblShelfMaster.AsQueryable();
                var products = (from productdetail in db.tblProductDetail
                                join productmaster in db.tblProductMaster
                                on productdetail.ProductMasterID equals productmaster.ProductMasterID
                                select new { productdetail, productmaster }).AsQueryable();

                foreach (var item in orderImports)
                {
                    item.ImportStatus = "FAIL";
                    item.ImportError = null;

                    // Get client id
                    int? clientId = clients.Where(t => t.CMCLongCustomer == item.Company_MDB &&
                                                       t.CMCLocation == item.Location_MDB)
                                           .Select(t => t.ClientID)
                                           .FirstOrDefault();

                    if ((clientId ?? 0) == 0)
                    {
                        item.ImportError = "Client not found.\n";
                    }
                    else
                    {
                        item.ClientID = clientId;

                        // Get division id
                        int? divisionId = divisions.Where(x => x.ClientID == item.ClientID &&
                                                               x.DivisionName == item.Division_MDB)
                                                   .Select(x => x.DivisionID)
                                                   .FirstOrDefault();

                        if ((divisionId ?? 0) != 0)
                        {
                            item.DivisionID = divisionId;
                        }
                    }

                    // Get product detail id
                    int? productDetailId = products.Where(x => x.productdetail.ProductCode == item.ProductCode &&
                                                               x.productmaster.ClientID == item.ClientID)
                                                   .Select(x => x.productdetail.ProductDetailID)
                                                   .FirstOrDefault();

                    if ((productDetailId ?? 0) == 0)
                    {
                        item.ProductDetailID = 0;
                        item.ImportError += "Requested product not found.\n";
                    }
                    else
                    {
                        item.ProductDetailID = productDetailId;

                        // Get shelf id
                        int? shelfMasterId = shelfItems.Where(t => t.ProductDetailID == item.ProductDetailID &&
                                                                   t.Size == item.Size)
                                                       .Select(t => t.ShelfID)
                                                       .FirstOrDefault();

                        if ((shelfMasterId ?? 0) == 0)
                        {
                            item.ShelfID = 0;
                            item.ImportError += "Shelf profile not found";
                        }
                        else
                        {
                            item.ShelfID = shelfMasterId;
                        }
                    }

                    // Make sure clientid exists for a successful import
                    if ((item.ClientID ?? 0) != 0)
                    {
                        item.ImportStatus = "PASS";
                    }
                }

                db.SaveChanges();
            }
        }

        private static int ImportOrderHeader(tblOrderImport orderImport)
        {
            var order = new OrderMasterFull();

            order.OrderID = -1;
            order.OrderDate = Convert.ToDateTime(orderImport.OrderDate);
            order.ClientId = orderImport.ClientID;
            order.DivisionId = orderImport.DivisionID;
            order.IsSDN = orderImport.IsSDN;
            order.Customer = orderImport.Customer;
            order.CMCOrder = Convert.ToInt32(orderImport.CMCOrder);
            order.WebOrderId = Convert.ToInt32(orderImport.WebOrderID);
            order.CMCLegacyNumber = orderImport.CMCLegacyNum;
            order.ClientOrderNumber = orderImport.CustOrdNum;
            order.ClientSAPNumber = orderImport.CustSapNum;
            order.ClientRefNumber = orderImport.CustRefNum;
            order.OrderType = (orderImport.OrderType == "w") ? "S" : orderImport.OrderType;
            order.Source = (orderImport.Source == null) ? "Web" : orderImport.Source;
            order.Company = orderImport.Company;
            order.Street = orderImport.Street;
            order.Street2 = orderImport.Street2;
            order.Street3 = orderImport.Street3;
            order.City = orderImport.City;
            order.State = orderImport.State;
            order.Zip = orderImport.Zip;
            order.Country = orderImport.Country;
            order.Attention = orderImport.Attention;
            order.Email = orderImport.Email;
            order.SalesRepName = orderImport.SalesRep;
            order.SalesRepEmail = orderImport.SalesEmail;
            order.RequestorName = orderImport.Req;
            order.RequestorPhone = orderImport.ReqPhone;
            order.RequestorFax = orderImport.ReqFax;
            order.RequestorEmail = orderImport.ReqEmail;
            order.EndUse = orderImport.EndUse;
            order.ShipVia = orderImport.ShipVia;
            order.ShipAcct = orderImport.ShipAcct;
            order.Phone = orderImport.Phone;
            order.Fax = orderImport.Fax;
            order.Tracking = orderImport.Tracking;
            order.Special = orderImport.Special;
            order.SpecialInternal = orderImport.SpecialInternal;
            order.IsLiterature = Convert.ToBoolean(orderImport.Lit);
            order.Region = orderImport.Region;
            order.COA = Convert.ToBoolean(orderImport.COA);
            order.TDS = Convert.ToBoolean(orderImport.TDS);
            order.CID = orderImport.CID;
            order.ClientAcct = orderImport.CustAcct;
            order.ACode = orderImport.ACode;
            order.ImportFile = orderImport.ImportFile;
            order.ImportDateLine = orderImport.ImportDateLine;
            order.Timing = orderImport.Timing;
            order.Volume = orderImport.Volume;
            order.SampleRack = Convert.ToBoolean(orderImport.SampleRack);
            order.CMCUser = orderImport.CMCUser;
            order.ClientReference = orderImport.CustomerReference;
            order.TotalOrderWeight = orderImport.TotalOrderWeight;
            order.ClientOrderType = orderImport.CustOrderType;
            order.ClientRequestDate = orderImport.CustRequestDate;
            order.ApprovalDate = orderImport.ApprovalDate;
            order.RequestedDeliveryDate = orderImport.RequestedDeliveryDate;
            order.ClientTotalItems = Convert.ToInt32(orderImport.CustTotalItems);
            order.ClientRequestedCarrier = orderImport.CustRequestedCarrier;
            order.LegacyId = Convert.ToInt32(orderImport.LegacyID);
            order.SalesRepPhone = orderImport.SalesRepPhone;
            order.SalesRepTerritory = orderImport.SalesRepTerritory;
            order.MarketingRep = orderImport.MarketingRep;
            order.MarketingRepEmail = orderImport.MarketingRepEmail;
            order.Distributor = orderImport.Distributor;
            order.PreferredCarrier = orderImport.PreferredCarrier;
            order.ApprovalNeeded = Convert.ToBoolean(orderImport.ApprovalNeeded);
            order.CreateUser = orderImport.CreateUser;
            order.CreateDate = orderImport.CreateDate;
            order.UpdateUser = orderImport.UpdateUser;
            order.UpdateDate = orderImport.UpdateDate;

            int newOrderId = OrderService.SaveOrder(order);

            return newOrderId;
        }

        private static void ImportOrderItem(int newOrderId, tblOrderImport item)
        {
            var newOrderItem = new OrderItem();

            newOrderItem.ItemID = -1;
            newOrderItem.OrderID = newOrderId;
            newOrderItem.CreateDate = DateTime.UtcNow;
            newOrderItem.CreateUser = "System [Import]";
            newOrderItem.UpdateUser = HttpContext.Current.User.Identity.Name;
            newOrderItem.ProductDetailID = item.ProductDetailID;
            newOrderItem.ShelfID = item.ShelfID;
            newOrderItem.Qty = item.Qty;
            newOrderItem.LotNumber = item.LotNumber;
            newOrderItem.ShipDate = item.ShipDate;
            newOrderItem.CSAllocate = true;
            newOrderItem.AllocateStatus = item.AllocateStatus;
            newOrderItem.NonCMCDelay = item.NonCMCDelay;
            newOrderItem.CarrierInvoiceRcvd = item.CarrierInvoiceRcvd;
            newOrderItem.Status = item.Status;
            newOrderItem.DelayReason = item.DelayReason;
            newOrderItem.ImportItemID = item.ImportItemID;
            newOrderItem.ItemNotes = item.ItemNotes;

            // Check special request size and zero out if not decimal.
            if (item.Size == "1SR")
            {
                decimal srsize;
                try
                {
                    srsize = decimal.Parse(item.SRSize);
                }
                catch (Exception)
                {
                    srsize = 0.00M;
                    newOrderItem.ItemNotes += "Special request size of " + item.SRSize + " was specified; adjust SR size field.";
                }

                newOrderItem.SRSize = srsize;
            }

            int newOrderItemId = OrderService.SaveOrderItem(newOrderItem);
        }

        private static void SaveUnknownOrderItem(int orderid, tblOrderImport importItem)
        {
            importItem.ItemNotes = String.Format(
                @"Delete this item when done.
                PRODUCT CODE: {0}
                PRODUCT NAME: {1} {2}
                QTY: {3} | SIZE: {4} | SRSIZE: {5}
                LOTNUMBER: {6}
                EXTERNAL ITEM ID: {7}
                ERROR: {8}",
                           importItem.ProductCode, importItem.ProductName, importItem.Mnemonic,
                           importItem.Qty, importItem.Size, importItem.SRSize,
                           importItem.LotNumber,
                           importItem.ImportItemID,
                           importItem.ImportError);

            var orderItem = new tblOrderItem();

            orderItem.OrderID = orderid;
            orderItem.ProductDetailID = 0;
            orderItem.ProductCode = "XXXXX - " + importItem.ProductCode;
            orderItem.ProductName = "UNKNOWN PRODUCT - " + importItem.ProductName + " " + importItem.Mnemonic;
            orderItem.Qty = importItem.Qty;
            orderItem.Size = importItem.Size;
            orderItem.LotNumber = importItem.LotNumber;
            orderItem.ImportItemID = importItem.ImportItemID;
            orderItem.ItemNotes = importItem.ItemNotes;
            orderItem.AlertNotesOrderEntry = importItem.ItemNotes;
            orderItem.CreateDate = DateTime.UtcNow;
            orderItem.CreateUser = "System [Import]";
            orderItem.UpdateDate = DateTime.UtcNow;
            orderItem.UpdateUser = HttpContext.Current.User.Identity.Name;

            using (var db = new CMCSQL03Entities())
            {
                db.tblOrderItem.Add(orderItem);
                db.SaveChanges();
            }
        }

        #endregion Order Import Methods
    }
}