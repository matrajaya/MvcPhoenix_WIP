using MvcPhoenix.EF;
using MvcPhoenix.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MvcPhoenix.Services
{
    public class OrderService
    {
        #region Order Master Methods

        public static OrderMasterFull CreateOrder(int clientid)
        {
            using (var db = new CMCSQL03Entities())
            {
                var client = db.tblClient.Find(clientid);

                var order = new OrderMasterFull();

                order.OrderID = -1;
                order.ClientId = client.ClientID;
                order.ClientName = client.ClientName;
                order.OrderStatus = "New";
                order.IsSDN = false;
                order.OrderDate = DateTime.UtcNow;
                order.CreateUser = HttpContext.Current.User.Identity.Name;
                order.CreateDate = DateTime.UtcNow;

                return order;
            }
        }

        public static OrderMasterFull FillOrder(int orderid)
        {
            using (var db = new CMCSQL03Entities())
            {
                var order = new OrderMasterFull();
                var orderDetails = db.tblOrderMaster.Find(orderid);
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

                return order;
            }
        }

        public static int NewOrderId()
        {
            using (var db = new CMCSQL03Entities())
            {
                var newOrder = new tblOrderMaster();
                newOrder.CreateDate = DateTime.UtcNow;
                newOrder.CreateUser = HttpContext.Current.User.Identity.Name;

                db.tblOrderMaster.Add(newOrder);
                db.SaveChanges();

                return newOrder.OrderID;
            }
        }

        public static int SaveOrder(OrderMasterFull order)
        {
            using (var db = new CMCSQL03Entities())
            {
                if (order.OrderID == -1)
                {
                    order.OrderID = NewOrderId();
                }

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

                return order.OrderID;
            }
        }

        public static void SaveOrderPostUpdate(OrderMasterFull order)
        {
            if (order.IsSDNOverride != true)
            {
                using (var db = new CMCSQL03Entities())
                {
                    var getOrder = db.tblOrderMaster.Find(order.OrderID);

                    var country = (from t in db.tblCountry
                                   where t.Country == order.Country
                                   && t.DoNotShip == true
                                   select t).FirstOrDefault();

                    var orderItems = (from t in db.tblOrderItem
                                      where t.OrderID == order.OrderID
                                      && t.AllocateStatus == null
                                      select t).ToList();

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
                        countryId = (from t in db.tblCountry
                                     where t.Country.Contains(order.Country)
                                     select t.CountryID).FirstOrDefault();

                        var ceaseShipOffset = (from t in db.tblCeaseShipOffSet
                                               where t.ClientID == order.ClientId
                                               && t.CountryID == countryId
                                               select t).FirstOrDefault();

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

        #endregion Order Master Methods

        #region Order Item Methods

        public static List<OrderItem> OrderItems(int orderid)
        {
            using (var db = new CMCSQL03Entities())
            {
                var orderItems = (from t in db.tblOrderItem
                                  join pd in db.tblProductDetail on t.ProductDetailID equals pd.ProductDetailID into productDetailOrderItem
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
                                      Mnemonic = t.Mnemonic,
                                      ProductName = t.ProductName,
                                      Size = t.Size,
                                      SRSize = t.SRSize,
                                      LotNumber = t.LotNumber,
                                      Qty = t.Qty,
                                      ShipDate = t.ShipDate,
                                      Via = t.Via,
                                      BackOrdered = t.BackOrdered,
                                      AllocateStatus = t.AllocateStatus,
                                      AllocatedDate = t.AllocatedDate,
                                      CSAllocate = t.CSAllocate,
                                      NonCMCDelay = t.NonCMCDelay,
                                      RDTransfer = t.RDTransfer,
                                      GrnUnNumber = productdetail.GRNUNNUMBER,
                                      GrnPkGroup = productdetail.GRNPKGRP,
                                      AirUnNumber = productdetail.AIRUNNUMBER,
                                      AirPkGroup = productdetail.AIRPKGRP,
                                      QtyAvailable = (from x in db.tblStock
                                                      where (x.ShelfID != null)
                                                      && (x.ShelfID == t.ShelfID)
                                                      && (x.ShelfStatus == "AVAIL")
                                                      select (x.QtyOnHand - x.QtyAllocated)).Sum(),
                                      CreateDate = t.CreateDate,
                                      CreateUser = t.CreateUser,
                                      UpdateDate = t.UpdateDate,
                                      UpdateUser = t.UpdateUser
                                  }).ToList();

                return orderItems;
            }
        }

        public static OrderItem CreateOrderItem(int orderid)
        {
            using (var db = new CMCSQL03Entities())
            {
                var order = db.tblOrderMaster.Find(orderid);

                var orderItem = new OrderItem();

                orderItem.CrudMode = "RW";
                orderItem.ItemID = -1;
                orderItem.OrderID = order.OrderID;
                orderItem.ClientID = order.ClientID;
                orderItem.CreateDate = DateTime.UtcNow;
                orderItem.CreateUser = HttpContext.Current.User.Identity.Name;
                orderItem.ProductDetailID = -1;
                orderItem.ProductCode = null;
                orderItem.ProductName = null;
                orderItem.Size = null;
                orderItem.SRSize = null;
                orderItem.Qty = 1;
                orderItem.LotNumber = null;
                orderItem.ShipDate = null;
                orderItem.ItemShipVia = "";
                orderItem.CSAllocate = true;
                orderItem.AllocateStatus = null;
                orderItem.NonCMCDelay = false;
                orderItem.RDTransfer = false;
                orderItem.CarrierInvoiceRcvd = false;
                orderItem.DelayReason = null;
                orderItem.StatusID = -1;
                orderItem.AlertNotesShipping = null;
                orderItem.AlertNotesPackOut = null;
                orderItem.AlertNotesOrderEntry = null;
                orderItem.AlertNotesOther = null;

                return orderItem;
            }
        }

        public static int NewOrderItemId()
        {
            using (var db = new CMCSQL03Entities())
            {
                tblOrderItem newOrderItem = new tblOrderItem();
                db.tblOrderItem.Add(newOrderItem);
                db.SaveChanges();

                return newOrderItem.ItemID;
            }
        }

        public static OrderItem FillOrderItemDetails(int orderitemid)
        {
            using (var db = new CMCSQL03Entities())
            {
                var getOrderItem = db.tblOrderItem.Find(orderitemid);
                var clientId = db.tblOrderMaster
                                 .Where(x => x.OrderID == getOrderItem.OrderID)
                                 .Select(x => x.ClientID).FirstOrDefault();

                OrderItem orderItem = new OrderItem();

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
                orderItem.WasteOrderTotalWeight = getOrderItem.WasteOrderTotalWeight;
                orderItem.AlertNotesShipping = getOrderItem.AlertNotesShipping;
                orderItem.AlertNotesPackOut = getOrderItem.AlertNotesPackout;
                orderItem.AlertNotesOrderEntry = getOrderItem.AlertNotesOrderEntry;
                orderItem.AlertNotesOther = getOrderItem.AlertNotesOther;

                if (getOrderItem.ShipDate != null || getOrderItem.AllocateStatus == "A")
                {
                    orderItem.CrudMode = "RO";
                }

                return orderItem;
            }
        }

        public static int SaveOrderItem(OrderItem orderitem)
        {
            using (var db = new CMCSQL03Entities())
            {
                bool isNewItem = false;

                if (orderitem.ItemID == -1)
                {
                    orderitem.ItemID = NewOrderItemId();
                    orderitem.CreateDate = DateTime.UtcNow;
                    orderitem.CreateUser = HttpContext.Current.User.Identity.Name;
                    isNewItem = true;
                }

                var orderMaster = db.tblOrderMaster.Find(orderitem.OrderID);
                var orderItem = db.tblOrderItem.Find(orderitem.ItemID);
                var productDetail = db.tblProductDetail.Find(orderitem.ProductDetailID);
                var productMaster = db.tblProductMaster.Find(productDetail.ProductMasterID);

                // Handle special request size
                if (orderItem.Size == "1SR")
                {
                    orderItem.SRSize = (decimal)(orderitem.SRSize);
                    orderitem.ShelfID = ShelfMasterService.GetShelfIdProductDetail(orderitem.ProductDetailID, "1SR");
                }

                if (orderitem.ShelfID == 0 || orderitem.SRSize != null)
                {
                    orderItem.SRSize = orderitem.SRSize;
                    orderItem.Size = "1SR";
                    orderitem.ShelfID = ShelfMasterService.GetShelfIdProductDetail(orderitem.ProductDetailID, "1SR");
                }
                else
                {
                    var shelfMaster = db.tblShelfMaster.Find(orderitem.ShelfID);

                    if (shelfMaster != null)
                    {
                        orderItem.SRSize = orderitem.SRSize ?? 0;
                        orderItem.Size = shelfMaster.Size;
                        shelfMaster.UnitWeight = shelfMaster.UnitWeight ?? orderItem.Weight;
                        orderItem.Weight = shelfMaster.UnitWeight * orderitem.Qty;
                    }
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
                orderItem.WasteOrderTotalWeight = orderitem.WasteOrderTotalWeight;
                orderItem.AlertNotesShipping = productDetail.AlertNotesShipping;
                orderItem.AlertNotesOrderEntry = productDetail.AlertNotesOrderEntry;
                orderItem.AlertNotesPackout = productMaster.AlertNotesPackout;

                if (productDetail.AIRUNNUMBER == "UN3082" || productDetail.AIRUNNUMBER == "UN3077" || productDetail.GRNUNNUMBER == "UN3082" || productDetail.GRNUNNUMBER == "UN3077")
                {
                    orderItem.AlertNotesOther = "Products with UN3082 and UN3077 may be shipped as non hazardous if under 5 kg";
                }

                orderMaster.UpdateDate = DateTime.UtcNow;
                orderMaster.UpdateUser = HttpContext.Current.User.Identity.Name;

                // Update order division
                orderMaster.DivisionID = productDetail.DivisionID;

                db.SaveChanges();

                if (isNewItem)
                {
                    GenerateOrderTransaction(orderitem.ItemID);
                }

                return orderitem.ItemID;
            }
        }

        public static void DeleteOrderItem(int orderitemid)
        {
            using (var db = new CMCSQL03Entities())
            {
                string deleteQuery;
                int orderItemId = db.tblOrderItem
                                    .Where(x => x.ItemID == orderitemid)
                                    .Select(x => x.ItemID).FirstOrDefault();

                if (orderItemId > 0)
                {
                    deleteQuery = "DELETE FROM tblOrderItem WHERE ItemID=" + orderItemId;
                    db.Database.ExecuteSqlCommand(deleteQuery);

                    deleteQuery = "DELETE FROM tblOrderTrans WHERE OrderItemID=" + orderItemId;
                    db.Database.ExecuteSqlCommand(deleteQuery);
                }
            }
        }

        #endregion Order Item Methods

        #region Order Transaction Methods

        public static OrderTrans CreateOrderTransaction(int orderid)
        {
            using (var db = new CMCSQL03Entities())
            {
                var order = db.tblOrderMaster.Find(orderid);
                OrderTrans orderTransaction = new OrderTrans();

                orderTransaction.OrderTransID = -1;
                orderTransaction.OrderId = orderid;
                orderTransaction.TransType = null;
                orderTransaction.CreateDate = DateTime.UtcNow;
                orderTransaction.TransDate = DateTime.UtcNow.Date;
                orderTransaction.CreateUser = HttpContext.Current.User.Identity.Name;
                orderTransaction.ClientId = order.ClientID;
                orderTransaction.DivisionId = order.DivisionID;

                return orderTransaction;
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

                return ordertransaction.OrderTransID;
            }
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

        public static OrderTrans FillOrderTransaction(int ordertransid)
        {
            OrderTrans orderTransaction = new OrderTrans();

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

        public static void DeleteTransaction(int ordertransid)
        {
            using (var db = new CMCSQL03Entities())
            {
                string deleteQuery = "DELETE FROM tblOrderTrans WHERE OrderTransID=" + ordertransid;
                db.Database.ExecuteSqlCommand(deleteQuery);
            }
        }

        public static void GenerateOrderTransaction(int orderitemid)
        {
            using (var db = new CMCSQL03Entities())
            {
                var orderItem = db.tblOrderItem.Find(orderitemid);
                var order = db.tblOrderMaster.Find(orderItem.OrderID);

                // Tier 1 sample charge
                string deleteQuery = "DELETE FROM tblOrderTrans WHERE OrderItemID=" + orderItem.ItemID + " AND Transtype = 'SAMP' AND CreateUser='System'";
                db.Database.ExecuteSqlCommand(deleteQuery);

                var tierSize = (from t in db.tblTier
                                where t.ClientID == order.ClientID
                                && t.Size == orderItem.Size
                                && t.TierLevel == 1
                                select t).FirstOrDefault();

                if (tierSize != null)
                {
                    tblOrderTrans orderTransaction = new tblOrderTrans();

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
                    var tierSpecialRequest = (from t in db.tblTier
                                              where t.ClientID == order.ClientID
                                              && t.Size == "1SR"
                                              && t.TierLevel == 1
                                              select t).FirstOrDefault();

                    if (tierSpecialRequest != null)
                    {
                        tblOrderTrans orderTransaction = new tblOrderTrans();

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
                string deleteQuery = String.Format("DELETE FROM tblOrderTrans WHERE OrderItemID={0} AND Transtype = '{1}' AND CreateUser='System'", ItemID, TransType);
                db.Database.ExecuteSqlCommand(deleteQuery);

                var orderItem = db.tblOrderItem.Find(ItemID);
                var order = db.tblOrderMaster.Find(orderItem.OrderID);

                tblOrderTrans newOrderTransaction = new tblOrderTrans();

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

        #endregion Order Transaction Methods

        #region Allocate Methods

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

                        stock = (from t in stock
                                 where t.CeaseShipDate > ceaseShipBufferDate
                                 select t).ToList();
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
                        var log = new OrderInventoryLog();

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
                                decimal? unitWeight = db.tblShelfMaster
                                                        .Where(x => x.ShelfID == item.orderitem.ShelfID)
                                                        .Select(x => x.UnitWeight)
                                                        .FirstOrDefault() ?? 0;

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

                                InventoryLog(log);
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
                var log = new OrderInventoryLog();

                var orderItem = (from t in db.tblOrderItem
                                 where t.ItemID == orderitemid
                                 && t.AllocateStatus == "A"
                                 && t.ShipDate == null
                                 select t).FirstOrDefault();

                var order = db.tblOrderMaster
                              .Where(x => x.OrderID == orderItem.OrderID)
                              .Select(x => new { x.ClientID, x.OrderID }).FirstOrDefault();

                var bulk = db.tblBulk.Find(orderItem.BulkID);

                var product = db.tblProductMaster
                                .Where(x => x.ProductMasterID == bulk.ProductMasterID)
                                .Select(x => new { x.MasterCode, x.MasterName }).FirstOrDefault();

                decimal? unitWeight = db.tblShelfMaster
                                        .Where(x => x.ShelfID == orderItem.ShelfID)
                                        .Select(x => x.UnitWeight).FirstOrDefault() ?? 0;

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

                    InventoryLog(log);
                }
            }
        }

        #endregion Allocate Methods

        #region Return Order Methods

        public static async Task<int> AddBulkItemToReturnOrder(int orderid, int bulkid)
        {
            var log = new OrderInventoryLog();

            using (var db = new CMCSQL03Entities())
            {
                var bulk = db.tblBulk.Find(bulkid);

                var productMaster = db.tblProductMaster
                                      .Where(x => x.ProductMasterID == bulk.ProductMasterID)
                                      .Select(x => new { x.ProductMasterID, x.ClientID, x.MasterCode, x.MasterName })
                                      .FirstOrDefault();

                var productDetailId = ProductsService.GetProductDetailId(productMaster.ProductMasterID);
                var productDetail = db.tblProductDetail.Find(productDetailId);

                int newItemId = OrderService.NewOrderItemId();
                var newItem = db.tblOrderItem.Find(newItemId);

                newItem.ShelfID = ShelfMasterService.GetShelfIdProductMaster(productMaster.ProductMasterID, bulk.UM);
                newItem.OrderID = orderid;
                newItem.AllocatedBulkID = newItem.BulkID = bulk.BulkID;
                newItem.ProductDetailID = productDetailId;
                newItem.ProductCode = productMaster.MasterCode;
                newItem.ProductName = productMaster.MasterName;
                newItem.LotNumber = bulk.LotNumber;
                newItem.Size = bulk.UM;
                newItem.Qty = 1;
                newItem.Weight = bulk.CurrentWeight;
                newItem.Bin = bulk.Bin;
                newItem.Warehouse = bulk.Warehouse;
                newItem.CSAllocate = true;
                newItem.AllocateStatus = "A";
                newItem.AllocatedDate = DateTime.UtcNow;
                newItem.GrnUnNumber = productDetail.GRNUNNUMBER;
                newItem.GrnPkGroup = productDetail.GRNPKGRP;
                newItem.AirUnNumber = productDetail.AIRUNNUMBER;
                newItem.AirPkGroup = productDetail.AIRPKGRP;
                newItem.AlertNotesOrderEntry = productDetail.AlertNotesOrderEntry;
                newItem.AlertNotesShipping = productDetail.AlertNotesShipping;
                newItem.ItemNotes = "Return bulk order item: bulk container for return is " + bulk.UM + " with a current weight of " + bulk.CurrentWeight;
                newItem.CreateDate = DateTime.UtcNow;
                newItem.CreateUser = HttpContext.Current.User.Identity.Name;
                newItem.UpdateDate = DateTime.UtcNow;
                newItem.UpdateUser = HttpContext.Current.User.Identity.Name;

                bulk.CurrentWeight = 0;
                bulk.BulkStatus = "RETURN";
                bulk.MarkedForReturn = true;
                bulk.UpdateDate = DateTime.UtcNow;
                bulk.UpdateUser = HttpContext.Current.User.Identity.Name;

                db.SaveChanges();

                // Insert inventory log record
                log.LogType = "BS-RTN";
                log.ClientId = productMaster.ClientID;
                log.ProductMasterId = productMaster.ProductMasterID;
                log.MasterCode = productMaster.MasterCode;
                log.MasterName = productMaster.MasterName;
                log.OrderNumber = newItem.OrderID;
                log.BulkId = newItem.AllocatedBulkID;
                log.ProductDetailId = newItem.ProductDetailID;
                log.ProductCode = newItem.ProductCode;
                log.ProductName = newItem.ProductName;
                log.LotNumber = newItem.LotNumber;
                log.Size = newItem.Size;
                log.LogQty = newItem.Qty;
                log.LogAmount = newItem.Weight;
                log.BulkBin = newItem.Bin;
                log.Warehouse = newItem.Warehouse;
                log.LogNotes = newItem.ItemNotes;

                InventoryLog(log);

                OrderService.GenerateOrderTransaction(newItemId);
            }

            return orderid;
        }

        public static async Task<int> AddStockItemToReturnOrder(int orderid, int stockid, int clientid)
        {
            var log = new OrderInventoryLog();

            using (var db = new CMCSQL03Entities())
            {
                int newOrderItemId = OrderService.NewOrderItemId();

                var stockItem = db.tblStock.Find(stockid);
                var newOrderItem = db.tblOrderItem.Find(newOrderItemId);

                var shelf = db.tblShelfMaster.Find(stockItem.ShelfID);
                var bulk = db.tblBulk
                             .Where(x => x.BulkID == stockItem.BulkID)
                             .Select(x => new { x.ProductMasterID, x.LotNumber })
                             .FirstOrDefault();

                var productDetail = db.tblProductDetail.Find(shelf.ProductDetailID);
                var productMaster = db.tblProductMaster
                                      .Where(x => x.ProductMasterID == bulk.ProductMasterID)
                                      .Select(x => new { x.ProductMasterID, x.MasterCode, x.MasterName })
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
                log.ProductMasterId = productMaster.ProductMasterID;
                log.MasterCode = productMaster.MasterCode;
                log.MasterName = productMaster.MasterName;
                log.OrderNumber = newOrderItem.OrderID = orderid;
                log.ProductDetailId = newOrderItem.ProductDetailID;
                log.ProductCode = newOrderItem.ProductCode;
                log.ProductName = newOrderItem.ProductName;
                log.LotNumber = newOrderItem.LotNumber;
                log.LogQty = newOrderItem.Qty;
                log.Size = newOrderItem.Size;
                log.LogAmount = newOrderItem.Weight;
                log.ShelfBin = newOrderItem.Bin;
                log.BulkId = newOrderItem.AllocatedBulkID;
                log.StockId = newOrderItem.AllocatedStockID;
                log.Warehouse = newOrderItem.Warehouse;
                log.LogNotes = newOrderItem.ItemNotes;

                InventoryLog(log);

                OrderService.GenerateOrderTransaction(newOrderItemId);
            }

            return orderid;
        }

        #endregion Return Order Methods

        #region SPS Billing Methods

        public static OrderSPSBilling SPSBilling(int orderid)
        {
            using (var db = new CMCSQL03Entities())
            {
                var orderSPSBilling = new OrderSPSBilling();
                var getSPSBilling = db.tblOrderSPSBilling.FirstOrDefault(i => i.OrderID == orderid);

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

                return orderSPSBilling;
            }
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
                if (orderspsbilling.SPSBillingID == -1)
                {
                    var newOrderSPSBilling = new tblOrderSPSBilling();

                    newOrderSPSBilling.OrderID = orderspsbilling.OrderId;
                    db.tblOrderSPSBilling.Add(newOrderSPSBilling);

                    db.SaveChanges();
                }

                var orderSPSBilling = db.tblOrderSPSBilling
                                        .SingleOrDefault(i => i.OrderID == orderspsbilling.OrderId);

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

        #endregion SPS Billing Methods

        #region List Methods

        public static List<OrderMasterFull> OpenOrdersAssigned()
        {
            using (var db = new CMCSQL03Entities())
            {
                var orders = new List<OrderMasterFull>();
                orders = OrderService.SearchOrders();

                // Get list of order ids for orders not shipped.
                var unshippedOrders = (from i in db.tblOrderItem
                                       where i.ShipDate == null
                                       && i.Qty > 0
                                       select i).ToList();

                unshippedOrders = unshippedOrders.GroupBy(x => x.OrderID)
                                                 .Select(g => g.First())
                                                 .ToList();

                // Get list of open orders.
                orders = (from t in orders
                          join u in unshippedOrders on t.OrderID equals u.OrderID
                          join c in db.tblClientAccountRep on t.ClientId equals c.ClientID
                          where c.AccountRepEmail == HttpContext.Current.User.Identity.Name
                          orderby t.OrderID descending, t.OrderDate descending
                          select t).ToList();

                // Display all clients in EU if user has no client assignments.
                // Since CMCEU does not have specific csr assignments for clients.
                if (orders.Count() == 0)
                {
                    orders = OrderService.SearchOrders();
                    orders = (from t in orders
                              join u in unshippedOrders on t.OrderID equals u.OrderID
                              join c in db.tblClient on t.ClientId equals c.ClientID
                              where c.CMCLocation == "EU"
                              orderby t.OrderID descending, t.OrderDate descending
                              select t).ToList();
                }

                return orders;
            }
        }

        public static List<OrderMasterFull> SearchOrders()
        {
            using (var db = new CMCSQL03Entities())
            {
                var orders = new List<OrderMasterFull>();

                orders = (from t in db.tblOrderMaster
                          join client in db.tblClient on t.ClientID equals client.ClientID
                          let count = (from items in db.tblOrderItem
                                       where items.OrderID == t.OrderID
                                       select items).Count()
                          let allocationcount = (from i in db.tblOrderItem
                                                 where i.OrderID == t.OrderID
                                                 && i.ShipDate == null
                                                 && i.Qty > 0
                                                 && String.IsNullOrEmpty(i.AllocateStatus)
                                                 && i.ProductDetailID != null
                                                 && i.ShelfID == null
                                                 select i).Count()
                          select new OrderMasterFull
                          {
                              ClientId = t.ClientID,
                              OrderID = t.OrderID,
                              Customer = t.Customer,
                              ClientName = client.ClientName,
                              OrderType = t.OrderType,
                              OrderDate = t.OrderDate,
                              Company = t.Company,
                              CreateUser = t.CreateUser,
                              ItemsCount = count,
                              Zip = t.Zip,
                              SalesRepName = t.SalesRep,
                              NeedAllocationCount = allocationcount
                          }).ToList();

                return orders;
            }
        }

        public static Contact GetClientContact(int clientcontactid)
        {
            using (var db = new CMCSQL03Entities())
            {
                var contact = new Contact();
                var clientContact = db.tblClientContact.Find(clientcontactid);

                contact.FullName = clientContact.FullName;
                contact.Email = clientContact.Email;
                contact.Phone = clientContact.Phone;
                contact.Phone = clientContact.Fax;
                contact.Account = clientContact.Account;

                return contact;
            }
        }

        #endregion List Methods

        #region Order Import Methods

        public static void PrepareForImport()
        {
            using (var db = new CMCSQL03Entities())
            {
                db.Database.ExecuteSqlCommand("DELETE FROM tblOrderImport WHERE Status NOT IN('0')");

                var orderImports = (from t in db.tblOrderImport
                                    where t.ImportStatus != "IMPORTED"
                                    && t.Location_MDB == "EU"
                                    select t).ToList();

                foreach (var item in orderImports)
                {
                    item.ImportStatus = "FAIL";

                    int? clientId = (from t in db.tblClient
                                     where t.CMCLongCustomer == item.Company_MDB
                                     && t.CMCLocation == item.Location_MDB
                                     select t.ClientID).FirstOrDefault();

                    if ((clientId ?? 0) == 0)
                    {
                        item.ImportError += " Client does not exist.";
                    }
                    else
                    {
                        item.ClientID = clientId;

                        int? divisionId = (from t in db.tblDivision
                                           where t.ClientID == item.ClientID
                                           && t.DivisionName == item.Division_MDB
                                           select t.DivisionID).FirstOrDefault();

                        if ((divisionId ?? 0) != 0)
                        {
                            item.DivisionID = divisionId;
                        }
                    }

                    // Make sure the product exists
                    int? productDetailId = (from pd in db.tblProductDetail
                                            join pm in db.tblProductMaster on pd.ProductMasterID equals pm.ProductMasterID
                                            where pd.ProductCode == item.ProductCode
                                            && pm.ClientID == item.ClientID
                                            select pd.ProductDetailID).FirstOrDefault();

                    if ((productDetailId ?? 0) == 0)
                    {
                        item.ImportError += " Requested product not found.";
                    }
                    else
                    {
                        item.ProductDetailID = productDetailId;

                        int? shelfMasterId = (from t in db.tblShelfMaster
                                              where t.ProductDetailID == item.ProductDetailID
                                              && t.Size == item.Size
                                              select t.ShelfID).FirstOrDefault();

                        // Create new shelfmaster if none found
                        if ((shelfMasterId ?? 0) == 0)
                        {
                            item.ShelfID = ShelfMasterService.GetShelfIdProductDetail(item.ProductDetailID, item.Size);
                            item.ImportError = String.Format(" Missing shelf profile; shelf id: {0} created.", item.ShelfID);
                        }
                        else
                        {
                            item.ShelfID = shelfMasterId;
                        }
                    }

                    // Make sure clientid, productdetailid, shelfid exists for a successful import
                    if (((item.ClientID ?? 0) != 0) && ((item.ProductDetailID ?? 0) != 0) && ((item.ShelfID ?? 0) != 0))
                    {
                        item.ImportStatus = "PASS";
                    }

                    db.SaveChanges();
                }

                // Cancel order import if a related item fails
                var failedOrderImport = (from t in db.tblOrderImport
                                         where t.ImportStatus == "FAIL"
                                         select new { guid = t.GUID }).ToList().Distinct();

                foreach (var r in failedOrderImport)
                {
                    string updateQuery = String.Format("UPDATE tblOrderImport SET ImportStatus='FAIL' WHERE GUID='{0}'", r.guid);
                    db.Database.ExecuteSqlCommand(updateQuery);
                }

                return;
            }
        }

        public static int ImportOrders()
        {
            PrepareForImport();

            string location = "EU";
            int OrdersImportedCount = 0;

            using (var db = new CMCSQL03Entities())
            {
                // Get list of unique rows that passed precheck
                var orderImportGUIDs = (from t in db.tblOrderImport
                                        where t.ImportStatus == "PASS"
                                        && t.Location_MDB == location
                                        select new { t.GUID }).ToList().Distinct();

                // Create orders and insert items
                foreach (var row in orderImportGUIDs)
                {
                    var importOrder = (from t in db.tblOrderImport
                                       where t.GUID == row.GUID
                                       select t).FirstOrDefault();

                    OrderMasterFull newOrder = new OrderMasterFull();

                    newOrder.OrderID = -1;
                    newOrder.OrderDate = Convert.ToDateTime(importOrder.OrderDate);
                    newOrder.ClientId = importOrder.ClientID;
                    newOrder.DivisionId = importOrder.DivisionID;
                    newOrder.IsSDN = importOrder.IsSDN;
                    newOrder.Customer = importOrder.Customer;
                    newOrder.CMCOrder = Convert.ToInt32(importOrder.CMCOrder);
                    newOrder.WebOrderId = Convert.ToInt32(importOrder.WebOrderID);
                    newOrder.CMCLegacyNumber = importOrder.CMCLegacyNum;
                    newOrder.ClientOrderNumber = importOrder.CustOrdNum;
                    newOrder.ClientSAPNumber = importOrder.CustSapNum;
                    newOrder.ClientRefNumber = importOrder.CustRefNum;
                    if (importOrder.OrderType == "w")
                    {
                        newOrder.OrderType = "S";
                    }
                    else
                    {
                        newOrder.OrderType = importOrder.OrderType;
                    }
                    if (importOrder.Source == null)
                    {
                        newOrder.Source = "Web";
                    }
                    else
                    {
                        newOrder.Source = importOrder.Source;
                    }
                    newOrder.Company = importOrder.Company;
                    newOrder.Street = importOrder.Street;
                    newOrder.Street2 = importOrder.Street2;
                    newOrder.Street3 = importOrder.Street3;
                    newOrder.City = importOrder.City;
                    newOrder.State = importOrder.State;
                    newOrder.Zip = importOrder.Zip;
                    newOrder.Country = importOrder.Country;
                    newOrder.Attention = importOrder.Attention;
                    newOrder.Email = importOrder.Email;
                    newOrder.SalesRepName = importOrder.SalesRep;
                    newOrder.SalesRepEmail = importOrder.SalesEmail;
                    newOrder.RequestorName = importOrder.Req;
                    newOrder.RequestorPhone = importOrder.ReqPhone;
                    newOrder.RequestorFax = importOrder.ReqFax;
                    newOrder.RequestorEmail = importOrder.ReqEmail;
                    newOrder.EndUse = importOrder.EndUse;
                    newOrder.ShipVia = importOrder.ShipVia;
                    newOrder.ShipAcct = importOrder.ShipAcct;
                    newOrder.Phone = importOrder.Phone;
                    newOrder.Fax = importOrder.Fax;
                    newOrder.Tracking = importOrder.Tracking;
                    newOrder.Special = importOrder.Special;
                    newOrder.SpecialInternal = importOrder.SpecialInternal;
                    newOrder.IsLiterature = Convert.ToBoolean(importOrder.Lit);
                    newOrder.Region = importOrder.Region;
                    newOrder.COA = Convert.ToBoolean(importOrder.COA);
                    newOrder.TDS = Convert.ToBoolean(importOrder.TDS);
                    newOrder.CID = importOrder.CID;
                    newOrder.ClientAcct = importOrder.CustAcct;
                    newOrder.ACode = importOrder.ACode;
                    newOrder.ImportFile = importOrder.ImportFile;
                    newOrder.ImportDateLine = importOrder.ImportDateLine;
                    newOrder.Timing = importOrder.Timing;
                    newOrder.Volume = importOrder.Volume;
                    newOrder.SampleRack = Convert.ToBoolean(importOrder.SampleRack);
                    newOrder.CMCUser = importOrder.CMCUser;
                    newOrder.ClientReference = importOrder.CustomerReference;
                    newOrder.TotalOrderWeight = importOrder.TotalOrderWeight;
                    newOrder.ClientOrderType = importOrder.CustOrderType;
                    newOrder.ClientRequestDate = importOrder.CustRequestDate;
                    newOrder.ApprovalDate = importOrder.ApprovalDate;
                    newOrder.RequestedDeliveryDate = importOrder.RequestedDeliveryDate;
                    newOrder.ClientTotalItems = Convert.ToInt32(importOrder.CustTotalItems);
                    newOrder.ClientRequestedCarrier = importOrder.CustRequestedCarrier;
                    newOrder.LegacyId = Convert.ToInt32(importOrder.LegacyID);
                    newOrder.SalesRepPhone = importOrder.SalesRepPhone;
                    newOrder.SalesRepTerritory = importOrder.SalesRepTerritory;
                    newOrder.MarketingRep = importOrder.MarketingRep;
                    newOrder.MarketingRepEmail = importOrder.MarketingRepEmail;
                    newOrder.Distributor = importOrder.Distributor;
                    newOrder.PreferredCarrier = importOrder.PreferredCarrier;
                    newOrder.ApprovalNeeded = Convert.ToBoolean(importOrder.ApprovalNeeded);
                    newOrder.CreateUser = importOrder.CreateUser;
                    newOrder.CreateDate = importOrder.CreateDate;
                    newOrder.UpdateUser = importOrder.UpdateUser;
                    newOrder.UpdateDate = importOrder.UpdateDate;

                    int newOrderId = OrderService.SaveOrder(newOrder);

                    var importOrderItems = (from t in db.tblOrderImport
                                            where t.GUID == row.GUID
                                            select t).ToList();

                    foreach (var item in importOrderItems)
                    {
                        OrderItem newOrderItem = new OrderItem();

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
                        newOrderItem.CSAllocate = item.CSAllocate;
                        newOrderItem.AllocateStatus = item.AllocateStatus;
                        newOrderItem.NonCMCDelay = item.NonCMCDelay;
                        newOrderItem.CarrierInvoiceRcvd = item.CarrierInvoiceRcvd;
                        newOrderItem.Status = item.Status;
                        newOrderItem.DelayReason = item.DelayReason;
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
                                newOrderItem.ItemNotes += String.Format(" {0} was imported as a special request size but it's not recognized as a decimal.", item.SRSize);
                            }

                            newOrderItem.SRSize = srsize;
                        }

                        int newOrderItemId = OrderService.SaveOrderItem(newOrderItem);
                    }

                    OrdersImportedCount = OrdersImportedCount + 1;

                    // Update order import status for successful imports
                    string updateQuery = String.Format("UPDATE tblOrderImport SET ImportStatus='IMPORTED', CMCLocation='{0}', OrderID='{1}' WHERE GUID='{2}'", location, newOrderId, row.GUID);
                    db.Database.ExecuteSqlCommand(updateQuery);
                }

                return OrdersImportedCount;
            }
        }

        #endregion Order Import Methods

        public static void InventoryLog(OrderInventoryLog log)
        {
            using (var db = new CMCSQL03Entities())
            {
                var client = db.tblClient
                               .Where(x => x.ClientID == log.ClientId)
                               .Select(x => new { x.ClientName, x.ClientUM })
                               .FirstOrDefault();

                tblInvLog InventoryLog = new tblInvLog();

                InventoryLog.LogType = log.LogType;
                InventoryLog.LogDate = DateTime.UtcNow;
                InventoryLog.BulkID = log.BulkId;
                InventoryLog.StockID = log.StockId;
                InventoryLog.ProductMasterID = log.ProductMasterId;
                InventoryLog.ProductDetailID = log.ProductDetailId;
                InventoryLog.ProductCode = log.ProductCode;
                InventoryLog.ProductName = log.ProductName;
                InventoryLog.MasterCode = log.MasterCode;
                InventoryLog.MasterName = log.MasterName;
                InventoryLog.Size = log.Size;
                InventoryLog.LogQty = log.LogQty;
                InventoryLog.LogAmount = log.LogAmount;
                InventoryLog.ClientID = log.ClientId;
                InventoryLog.ClientName = client.ClientName;
                InventoryLog.UM = client.ClientUM;
                InventoryLog.LogNotes = log.LogNotes;
                InventoryLog.Status = log.Status;
                InventoryLog.Warehouse = log.Warehouse;
                InventoryLog.Lotnumber = log.LotNumber;
                InventoryLog.BulkBin = log.BulkBin;
                InventoryLog.ShelfBin = log.ShelfBin;
                InventoryLog.ShipDate = log.ShipDate;
                InventoryLog.OrderNumber = log.OrderNumber;
                InventoryLog.CurrentQtyAvailable = log.CurrentQtyAvailable;
                InventoryLog.CurrentWeightAvailable = log.CurrentWeightAvailable;
                InventoryLog.ExpirationDate = log.ExpirationDate;
                InventoryLog.CeaseShipDate = log.CeaseShipDate;
                InventoryLog.DateReceived = log.DateReceived;
                InventoryLog.QCDate = log.QCDate;
                InventoryLog.PackOutID = log.PackOutId;
                InventoryLog.CreateDate = DateTime.UtcNow;
                InventoryLog.CreateUser = HttpContext.Current.User.Identity.Name;
                InventoryLog.UpdateDate = DateTime.UtcNow;
                InventoryLog.UpdateUser = HttpContext.Current.User.Identity.Name;

                db.tblInvLog.Add(InventoryLog);
                db.SaveChanges();
            }
        }
    }
}