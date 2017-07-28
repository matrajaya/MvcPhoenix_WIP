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

        public static OrderMasterFull fnCreateOrder(int clientid)
        {
            using (var db = new CMCSQL03Entities())
            {
                OrderMasterFull ordermodel = new OrderMasterFull();
                var client = db.tblClient.Find(clientid);

                ordermodel.OrderID = -1;
                ordermodel.ClientId = client.ClientID;
                ordermodel.ClientName = client.ClientName;
                ordermodel.OrderStatus = "New";
                ordermodel.IsSDN = false;
                ordermodel.OrderDate = DateTime.UtcNow;
                ordermodel.CreateUser = HttpContext.Current.User.Identity.Name;
                ordermodel.CreateDate = DateTime.UtcNow;

                return ordermodel;
            }
        }

        public static OrderMasterFull fnFillOrder(int orderid)
        {
            using (var db = new CMCSQL03Entities())
            {
                OrderMasterFull ordermodel = new OrderMasterFull();
                var getorder = (from t in db.tblOrderMaster
                                where t.OrderID == orderid
                                select t).FirstOrDefault();

                ordermodel.ItemsCount = (from t in db.tblOrderItem
                                         where t.OrderID == orderid
                                         select t).Count();

                ordermodel.TransCount = (from t in db.tblOrderTrans
                                         where t.OrderID == orderid
                                         select t).Count();

                var client = db.tblClient.Find(getorder.ClientID);

                ordermodel.ClientName = client.ClientName;
                ordermodel.ClientCode = client.ClientCode;
                ordermodel.OrderID = getorder.OrderID;
                ordermodel.ClientId = getorder.ClientID;
                ordermodel.DivisionId = getorder.DivisionID;
                ordermodel.OrderStatus = getorder.OrderStatus;
                ordermodel.Customer = getorder.Customer;
                ordermodel.CMCOrder = Convert.ToInt32(getorder.CMCOrder);
                ordermodel.WebOrderId = Convert.ToInt32(getorder.WebOrderID);
                ordermodel.CMCLegacyNumber = getorder.CMCLegacyNum;
                ordermodel.ClientOrderNumber = getorder.CustOrdNum;
                ordermodel.ClientSAPNumber = getorder.CustSapNum;
                ordermodel.ClientRefNumber = getorder.CustRefNum;
                ordermodel.OrderType = getorder.OrderType;
                ordermodel.OrderDate = getorder.OrderDate;
                ordermodel.Company = getorder.Company;
                ordermodel.Street = getorder.Street;
                ordermodel.Street2 = getorder.Street2;
                ordermodel.Street3 = getorder.Street3;
                ordermodel.City = getorder.City;
                ordermodel.State = getorder.State;
                ordermodel.Zip = getorder.Zip;
                ordermodel.Country = getorder.Country;
                ordermodel.CustomsRefNumber = getorder.CustomsRefNum;
                ordermodel.Attention = getorder.Attention;
                ordermodel.Email = getorder.Email;
                ordermodel.SalesRepName = getorder.SalesRep;
                ordermodel.SalesRepEmail = getorder.SalesEmail;
                ordermodel.RequestorName = getorder.Req;
                ordermodel.RequestorPhone = getorder.ReqPhone;
                ordermodel.RequestorFax = getorder.ReqFax;
                ordermodel.RequestorEmail = getorder.ReqEmail;
                ordermodel.EndUse = getorder.EndUse;
                ordermodel.ShipVia = getorder.ShipVia;
                ordermodel.ShipAcct = getorder.ShipAcct;
                ordermodel.Phone = getorder.Phone;
                ordermodel.Source = getorder.Source;
                ordermodel.Fax = getorder.Fax;
                ordermodel.Tracking = getorder.Tracking;
                ordermodel.Special = getorder.Special;
                ordermodel.SpecialInternal = getorder.SpecialInternal;
                ordermodel.IsLiterature = Convert.ToBoolean(getorder.Lit);
                ordermodel.Region = getorder.Region;
                ordermodel.COA = Convert.ToBoolean(getorder.COA);
                ordermodel.TDS = Convert.ToBoolean(getorder.TDS);
                ordermodel.CID = getorder.CID;
                ordermodel.ClientAcct = getorder.CustAcct;
                ordermodel.ACode = getorder.ACode;
                ordermodel.ImportFile = getorder.ImportFile;

                if (getorder.ImportDateLine.HasValue)
                {
                    ordermodel.ImportDateLine = Convert.ToDateTime(getorder.ImportDateLine);
                }
                else
                {
                    ordermodel.ImportDateLine = null;
                }

                ordermodel.Timing = getorder.Timing;
                ordermodel.Volume = getorder.Volume;
                ordermodel.SampleRack = Convert.ToBoolean(getorder.SampleRack);
                ordermodel.CMCUser = getorder.CMCUser;
                ordermodel.ClientReference = getorder.CustomerReference;
                ordermodel.TotalOrderWeight = getorder.TotalOrderWeight;
                ordermodel.ClientOrderType = getorder.CustOrderType;

                ordermodel.ClientRequestDate = null;
                if (getorder.CustRequestDate.HasValue)
                {
                    ordermodel.ClientRequestDate = getorder.CustRequestDate;
                }

                ordermodel.ApprovalDate = null;
                if (getorder.ApprovalDate.HasValue)
                {
                    ordermodel.ApprovalDate = getorder.ApprovalDate;
                }

                ordermodel.RequestedDeliveryDate = null;
                if (getorder.RequestedDeliveryDate.HasValue)
                {
                    ordermodel.RequestedDeliveryDate = getorder.RequestedDeliveryDate;
                }

                ordermodel.ClientTotalItems = Convert.ToInt32(getorder.CustTotalItems);
                ordermodel.ClientRequestedCarrier = getorder.CustRequestedCarrier;
                ordermodel.LegacyId = Convert.ToInt32(getorder.LegacyID);
                ordermodel.SalesRepPhone = getorder.SalesRepPhone;
                ordermodel.SalesRepTerritory = getorder.SalesRepTerritory;
                ordermodel.MarketingRep = getorder.MarketingRep;
                ordermodel.MarketingRepEmail = getorder.MarketingRepEmail;
                ordermodel.Distributor = getorder.Distributor;
                ordermodel.PreferredCarrier = getorder.PreferredCarrier;
                ordermodel.ApprovalNeeded = Convert.ToBoolean(getorder.ApprovalNeeded);
                ordermodel.CreateUser = getorder.CreateUser;
                ordermodel.CreateDate = getorder.CreateDate;
                ordermodel.UpdateUser = getorder.UpdateUser;
                ordermodel.UpdateDate = getorder.UpdateDate;
                ordermodel.IsSDN = getorder.IsSDN;
                ordermodel.IsSDNOverride = getorder.IsSDNOverride;

                return ordermodel;
            }
        }

        public static int fnNewOrderID()
        {
            using (var db = new CMCSQL03Entities())
            {
                tblOrderMaster newrec = new tblOrderMaster();
                db.tblOrderMaster.Add(newrec);
                db.SaveChanges();

                return newrec.OrderID;
            }
        }

        public static int fnSaveOrder(OrderMasterFull vm)
        {
            // Take a ViewModel and Insert/Update DB / Return the PK
            using (var db = new CMCSQL03Entities())
            {
                if (vm.OrderID == -1)
                {
                    vm.OrderID = fnNewOrderID();
                }

                // update time stamps
                vm.UpdateUser = HttpContext.Current.User.Identity.Name;
                vm.UpdateDate = DateTime.UtcNow;

                var q = (from t in db.tblOrderMaster
                         where t.OrderID == vm.OrderID
                         select t).FirstOrDefault();

                q.OrderDate = vm.OrderDate;
                q.ClientID = vm.ClientId;
                q.DivisionID = vm.DivisionId;
                q.Customer = vm.Customer;
                q.CMCOrder = vm.CMCOrder;
                q.WebOrderID = vm.WebOrderId;
                q.CMCLegacyNum = vm.CMCLegacyNumber;
                q.CustOrdNum = vm.ClientOrderNumber;
                q.CustSapNum = vm.ClientSAPNumber;
                q.CustRefNum = vm.ClientRefNumber;
                q.OrderType = vm.OrderType;
                q.OrderDate = vm.OrderDate;
                q.Company = vm.Company;
                q.Street = vm.Street;
                q.Street2 = vm.Street2;
                q.Street3 = vm.Street3;
                q.City = vm.City;
                q.State = vm.State;
                q.Zip = vm.Zip;
                q.Country = vm.Country;
                q.Attention = vm.Attention;
                q.CustomsRefNum = vm.CustomsRefNumber;
                q.Email = vm.Email;
                q.SalesRep = vm.SalesRepName;
                q.SalesEmail = vm.SalesRepEmail;
                q.Req = vm.RequestorName;
                q.ReqPhone = vm.RequestorPhone;
                q.ReqFax = vm.RequestorFax;
                q.ReqEmail = vm.RequestorEmail;
                q.EndUse = vm.EndUse;
                q.ShipVia = vm.ShipVia;
                q.ShipAcct = vm.ShipAcct;
                q.Phone = vm.Phone;
                q.Source = vm.Source;
                q.Fax = vm.Fax;
                q.Tracking = vm.Tracking;
                q.Special = vm.Special;
                q.SpecialInternal = vm.SpecialInternal;
                q.Lit = Convert.ToBoolean(vm.IsLiterature);
                q.Region = vm.Region;
                q.COA = vm.COA;
                q.TDS = vm.TDS;
                q.CID = vm.CID;
                q.CustAcct = vm.ClientAcct;
                q.ACode = vm.ACode;
                q.ImportFile = vm.ImportFile;
                q.ImportDateLine = vm.ImportDateLine;
                q.Timing = vm.Timing;
                q.Volume = vm.Volume;
                q.SampleRack = Convert.ToBoolean(vm.SampleRack);
                q.CMCUser = vm.CMCUser;
                q.CustomerReference = vm.ClientReference;
                q.TotalOrderWeight = (vm.TotalOrderWeight);
                q.CustOrderType = vm.ClientOrderType;
                q.CustRequestDate = vm.ClientRequestDate;
                q.ApprovalDate = vm.ApprovalDate;
                q.RequestedDeliveryDate = vm.RequestedDeliveryDate;
                q.CustTotalItems = vm.ClientTotalItems;
                q.CustRequestedCarrier = vm.ClientRequestedCarrier;
                q.LegacyID = (vm.LegacyId);
                q.SalesRepPhone = vm.SalesRepPhone;
                q.SalesRepTerritory = vm.SalesRepTerritory;
                q.MarketingRep = vm.MarketingRep;
                q.MarketingRepEmail = vm.MarketingRepEmail;
                q.Distributor = vm.Distributor;
                q.PreferredCarrier = vm.PreferredCarrier;
                q.ApprovalNeeded = vm.ApprovalNeeded;
                q.UpdateUser = vm.UpdateUser;
                q.UpdateDate = vm.UpdateDate;
                q.CreateUser = vm.CreateUser;
                q.CreateDate = vm.CreateDate;
                q.IsSDNOverride = vm.IsSDNOverride;
                q.IsSDN = false;

                db.SaveChanges();

                fnSaveOrderPostUpdate(vm);

                return vm.OrderID;
            }
        }

        public static void fnSaveOrderPostUpdate(OrderMasterFull vm)
        {
            if (vm.IsSDNOverride != true)
            {
                // changes to order record after it is saved
                using (var db = new CMCSQL03Entities())
                {
                    bool ShowAlert = false;

                    var q = (from t in db.tblOrderMaster
                             where t.OrderID == vm.OrderID
                             select t).FirstOrDefault();

                    var qCountry = (from t in db.tblCountry
                                    where t.Country == vm.Country
                                    && t.DoNotShip == true
                                    select t).FirstOrDefault();

                    if (qCountry != null)
                    {
                        // flag the order record and the item records that are yet to be allocated
                        q.IsSDN = true;
                        var orderitems = (from t in db.tblOrderItem
                                          where t.OrderID == vm.OrderID
                                          && t.AllocateStatus == null
                                          select t).ToList();

                        foreach (var item in orderitems)
                        {
                            item.CSAllocate = false;
                            db.SaveChanges();
                        }

                        ShowAlert = true;
                    }

                    if (fnIsSDN(vm) == true)
                    {
                        // flag the order record and the item records that are yet to be allocated (again maybe)
                        q.IsSDN = true;
                        var orderitems = (from t in db.tblOrderItem
                                          where t.OrderID == vm.OrderID
                                          && t.AllocateStatus == null
                                          select t).ToList();

                        foreach (var item in orderitems)
                        {
                            item.CSAllocate = false;
                            db.SaveChanges();
                        }

                        ShowAlert = true;
                    }

                    if (vm.Country != "0")
                    {
                        int countryid = 0;
                        countryid = (from t in db.tblCountry
                                     where t.Country.Contains(vm.Country)
                                     select t.CountryID).FirstOrDefault(); // need the ID

                        var qCeaseShipOffset = (from t in db.tblCeaseShipOffSet
                                                where t.ClientID == vm.ClientId
                                                && t.CountryID == countryid
                                                select t).FirstOrDefault();

                        if (qCeaseShipOffset != null)
                        {
                            q.CeaseShipOffset = qCeaseShipOffset.OffsetDays;
                        }
                    }

                    db.SaveChanges();

                    if (ShowAlert == true)
                    {
                        // do something , return js alert ???
                    }
                }
            }
        }

        // Search through SDN List and return true if a name match occurs
        public static bool fnIsSDN(OrderMasterFull vm)
        {
            var filecontent = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Content/sdnlist.txt"));

            if (filecontent.Contains(vm.Company))
            {
                return true;
            }

            if (!String.IsNullOrEmpty(vm.Street) && filecontent.Contains(vm.Street))
            {
                return true;
            }

            if (!String.IsNullOrEmpty(vm.Attention) && filecontent.Contains(vm.Attention))
            {
                return true;
            }

            return false;
        }

        #endregion Order Master Methods

        #region Order Item Methods

        public static List<OrderItem> ListOrderItems(int id)
        {
            using (var db = new CMCSQL03Entities())
            {
                var orderitems = (from t in db.tblOrderItem
                                  join pd in db.tblProductDetail on t.ProductDetailID equals pd.ProductDetailID into productdetailleftjoin
                                  from productdetail in productdetailleftjoin.DefaultIfEmpty()
                                  where t.OrderID == id
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

                return orderitems;
            }
        }

        public static OrderItem fnCreateItem(int orderid)
        {
            using (var db = new CMCSQL03Entities())
            {
                var dbOrder = db.tblOrderMaster.Find(orderid);

                OrderItem vm = new OrderItem();

                vm.CrudMode = "RW";
                vm.ItemID = -1;
                vm.OrderID = orderid;
                vm.ClientID = dbOrder.ClientID;
                vm.CreateDate = DateTime.UtcNow;
                vm.CreateUser = HttpContext.Current.User.Identity.Name;
                vm.ProductDetailID = -1;
                vm.ProductCode = null;
                vm.ProductName = null;
                vm.Size = null;
                vm.SRSize = null;
                vm.Qty = 1;
                vm.LotNumber = null;
                vm.ShipDate = null;
                vm.ItemShipVia = "";
                vm.CSAllocate = true;
                vm.AllocateStatus = null;
                vm.NonCMCDelay = false;
                vm.RDTransfer = false;
                vm.CarrierInvoiceRcvd = false;
                vm.DelayReason = null;
                vm.StatusID = -1;
                vm.AlertNotesShipping = null;
                vm.AlertNotesPackOut = null;
                vm.AlertNotesOrderEntry = null;
                vm.AlertNotesOther = null;

                return vm;
            }
        }

        public static int fnNewItemID()
        {
            using (var db = new CMCSQL03Entities())
            {
                tblOrderItem newrec = new tblOrderItem();
                db.tblOrderItem.Add(newrec);
                db.SaveChanges();

                return newrec.ItemID;
            }
        }

        public static int GetProductDetailId(int? productmasterid)
        {
            using (var db = new CMCSQL03Entities())
            {
                int getproductdetailid = (from pd in db.tblProductDetail
                                          join pm in db.tblProductMaster on pd.ProductMasterID equals pm.ProductMasterID
                                          where pm.ProductMasterID == productmasterid
                                          && pd.ProductCode == pm.MasterCode
                                          select pd.ProductDetailID).FirstOrDefault();

                return getproductdetailid;
            }
        }

        public static OrderItem fnFillOrderItem(int orderitemid)
        {
            using (var db = new CMCSQL03Entities())
            {
                OrderItem vm = new OrderItem();
                var orderitem = (from t in db.tblOrderItem
                         where t.ItemID == orderitemid
                         select t).FirstOrDefault();

                var client = db.tblOrderMaster.Find(orderitem.OrderID);

                vm.CrudMode = "RW";
                vm.ItemID = orderitem.ItemID;
                vm.OrderID = orderitem.OrderID;
                vm.ClientID = Convert.ToInt32(client.ClientID);
                vm.CreateDate = orderitem.CreateDate;
                vm.CreateUser = orderitem.CreateUser;
                vm.UpdateDate = orderitem.UpdateDate;
                vm.UpdateUser = orderitem.UpdateUser;
                vm.ProductDetailID = orderitem.ProductDetailID;
                vm.ProductCode = orderitem.ProductCode;
                vm.ProductName = orderitem.ProductName;
                vm.ShelfID = orderitem.ShelfID;
                vm.Size = orderitem.Size;
                vm.SRSize = orderitem.SRSize;
                vm.Qty = orderitem.Qty;
                vm.LotNumber = orderitem.LotNumber;
                vm.ItemShipVia = orderitem.Via;
                vm.ShipDate = orderitem.ShipDate;
                vm.CSAllocate = orderitem.CSAllocate;
                vm.AllocateStatus = orderitem.AllocateStatus;
                vm.NonCMCDelay = orderitem.NonCMCDelay;
                vm.RDTransfer = orderitem.RDTransfer;
                vm.BackOrdered = orderitem.BackOrdered;
                vm.CarrierInvoiceRcvd = orderitem.CarrierInvoiceRcvd;
                vm.DelayReason = orderitem.DelayReason;
                vm.SPSCharge = orderitem.SPSCharge;
                vm.Status = orderitem.Status;
                vm.StatusID = null;
                vm.ItemNotes = orderitem.ItemNotes;
                vm.WasteOrderTotalWeight = orderitem.WasteOrderTotalWeight;
                vm.AlertNotesShipping = orderitem.AlertNotesShipping;
                vm.AlertNotesPackOut = orderitem.AlertNotesPackout;
                vm.AlertNotesOrderEntry = orderitem.AlertNotesOrderEntry;
                vm.AlertNotesOther = orderitem.AlertNotesOther;

                if (orderitem.ShipDate != null || orderitem.AllocateStatus == "A")
                {
                    vm.CrudMode = "RO";
                }

                return vm;
            }
        }

        public static int fnSaveItem(OrderItem vm)
        {
            using (var db = new CMCSQL03Entities())
            {
                bool isNewItem = false;

                // get new order item id if submission is a new entry
                if (vm.ItemID == -1)
                {
                    vm.ItemID = fnNewItemID();
                    vm.CreateDate = DateTime.UtcNow;
                    vm.CreateUser = HttpContext.Current.User.Identity.Name;
                    isNewItem = true;
                }

                var ordermaster = db.tblOrderMaster.Find(vm.OrderID);

                var orderitem = (from t in db.tblOrderItem
                                 where t.ItemID == vm.ItemID
                                 select t).FirstOrDefault();

                var productdetail = (from t in db.tblProductDetail
                                     where t.ProductDetailID == vm.ProductDetailID
                                     select t).FirstOrDefault();

                var productmaster = db.tblProductMaster.Find(productdetail.ProductMasterID);

                // handle special request sizes
                if (orderitem.Size == "1SR")
                {
                    orderitem.SRSize = (decimal)(vm.SRSize);
                    vm.ShelfID = GetShelfIdProductDetail(vm.ProductDetailID, "1SR");
                }

                if (vm.ShelfID == 0 || vm.SRSize != null)
                {
                    orderitem.SRSize = vm.SRSize;
                    orderitem.Size = "1SR";
                    vm.ShelfID = GetShelfIdProductDetail(vm.ProductDetailID, "1SR");
                }
                else
                {
                    var shelfmaster = (from t in db.tblShelfMaster
                                       where t.ShelfID == vm.ShelfID
                                       select t).FirstOrDefault();

                    if (shelfmaster != null)
                    {
                        orderitem.SRSize = vm.SRSize ?? 0;
                        orderitem.Size = shelfmaster.Size;
                        shelfmaster.UnitWeight = shelfmaster.UnitWeight ?? orderitem.Weight;    // if unit weight is null, set current item weight; -> bulk order item edits
                        orderitem.Weight = shelfmaster.UnitWeight * vm.Qty;
                    }
                }

                orderitem.OrderID = vm.OrderID;
                orderitem.CreateDate = vm.CreateDate;
                orderitem.CreateUser = vm.CreateUser;
                orderitem.UpdateDate = DateTime.UtcNow;
                orderitem.UpdateUser = HttpContext.Current.User.Identity.Name;
                orderitem.ProductDetailID = vm.ProductDetailID;
                orderitem.ProductCode = productdetail.ProductCode;
                orderitem.ProductName = productdetail.ProductName;
                orderitem.ShelfID = vm.ShelfID;
                orderitem.Qty = vm.Qty;
                orderitem.LotNumber = vm.LotNumber;
                orderitem.Via = vm.ItemShipVia;
                orderitem.ShipDate = vm.ShipDate;
                orderitem.CSAllocate = vm.CSAllocate;
                orderitem.AllocateStatus = vm.AllocateStatus;
                orderitem.NonCMCDelay = vm.NonCMCDelay;
                orderitem.RDTransfer = vm.RDTransfer;
                orderitem.BackOrdered = vm.BackOrdered;
                orderitem.CarrierInvoiceRcvd = vm.CarrierInvoiceRcvd;
                orderitem.Status = vm.Status;
                orderitem.DelayReason = vm.DelayReason;
                orderitem.SPSCharge = vm.SPSCharge;
                orderitem.ItemNotes = vm.ItemNotes;
                orderitem.WasteOrderTotalWeight = vm.WasteOrderTotalWeight;
                orderitem.AlertNotesShipping = productdetail.AlertNotesShipping;
                orderitem.AlertNotesOrderEntry = productdetail.AlertNotesOrderEntry;
                orderitem.AlertNotesPackout = productmaster.AlertNotesPackout;

                // Update ordermaster using new divisionid from product profile
                ordermaster.DivisionID = productdetail.DivisionID;
                ordermaster.UpdateDate = DateTime.UtcNow;
                ordermaster.UpdateUser = HttpContext.Current.User.Identity.Name;

                if (productdetail.AIRUNNUMBER == "UN3082" | productdetail.AIRUNNUMBER == "UN3077" | productdetail.GRNUNNUMBER == "UN3082" | productdetail.GRNUNNUMBER == "UN3077")
                {
                    orderitem.AlertNotesOther = "Products with UN3082 and UN3077 may be shipped as non hazardous if under 5 kg";
                }

                db.SaveChanges();

                if (isNewItem)
                {
                    fnGenerateOrderTransactions(vm.ItemID);
                }

                return vm.ItemID;
            }
        }

        public static void fnDeleteOrderItem(int id)
        {
            System.Threading.Thread.Sleep(1000);
            using (var db = new CMCSQL03Entities())
            {
                var result = (from t in db.tblOrderItem
                              where t.ItemID == id
                              select t).FirstOrDefault();

                if (result != null)
                {
                    // Delete orderitem
                    string s = @"DELETE FROM tblOrderItem WHERE ItemID=" + id.ToString();
                    db.Database.ExecuteSqlCommand(s);

                    // DElete transactions associated with the related order items
                    s = @"DELETE FROM tblOrderTrans WHERE OrderItemID=" + id.ToString();
                    db.Database.ExecuteSqlCommand(s);
                }
            }
        }

        /// <summary>
        /// find original productdetailid, ignore equivalent products
        //  if true: get shelfid ?: insert new record in shelfmaster using productdetailid and size = um
        /// </summary>
        public static int GetShelfIdProductMaster(int? productmasterid, string um)
        {
            int getproductdetailid = GetProductDetailId(productmasterid);

            int xshelfid = 0;
            try
            {
                using (var db = new CMCSQL03Entities())
                {
                    // check table to see if the size has been set up on the shelf already
                    int qshelfid = (from t in db.tblShelfMaster
                                    where t.ProductDetailID == getproductdetailid
                                    && t.Size == um
                                    select t.ShelfID).FirstOrDefault();

                    // if it exists then assign the shelfid to xshelfid
                    if (qshelfid != 0)
                    {
                        xshelfid = qshelfid;
                    }
                    else
                    {
                        var newrecord = new tblShelfMaster
                        {
                            ProductDetailID = getproductdetailid,
                            Size = um
                        };
                        db.tblShelfMaster.Add(newrecord);
                        db.SaveChanges();

                        xshelfid = newrecord.ShelfID;
                    }
                }
            }
            catch (Exception)
            {
                xshelfid = 0;
            }

            return xshelfid;
        }

        public static int GetShelfIdProductDetail(int? productdetailid, string size)
        {
            int xshelfid = 0;
            try
            {
                using (var db = new CMCSQL03Entities())
                {
                    int qshelfid = (from t in db.tblShelfMaster
                                    where t.ProductDetailID == productdetailid
                                    && t.Size == size
                                    select t.ShelfID).FirstOrDefault();

                    if (qshelfid != 0)
                    {
                        xshelfid = qshelfid;
                    }
                    else
                    {
                        var newrecord = new tblShelfMaster
                        {
                            ProductDetailID = productdetailid,
                            Size = size
                        };
                        db.tblShelfMaster.Add(newrecord);
                        db.SaveChanges();

                        xshelfid = newrecord.ShelfID;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return xshelfid;
        }

        #endregion Order Item Methods

        #region Order Transaction Methods

        public static OrderTrans fnCreateTrans(int orderid)
        {
            using (var db = new CMCSQL03Entities())
            {
                OrderTrans ordertrans = new OrderTrans();
                var order = (from t in db.tblOrderMaster
                             where t.OrderID == orderid
                             select t).FirstOrDefault();

                ordertrans.OrderTransID = -1;
                ordertrans.OrderId = orderid;
                ordertrans.TransType = null;
                ordertrans.CreateDate = DateTime.UtcNow;
                ordertrans.TransDate = DateTime.UtcNow.Date;
                ordertrans.CreateUser = HttpContext.Current.User.Identity.Name;
                ordertrans.ClientId = order.ClientID;
                ordertrans.DivisionId = order.DivisionID;

                return ordertrans;
            }
        }

        public static int fnSaveTrans(OrderTrans model)
        {
            using (var db = new CMCSQL03Entities())
            {
                if (model.OrderTransID == -1)
                {
                    tblOrderTrans newrec = new tblOrderTrans();
                    db.tblOrderTrans.Add(newrec);
                    model.CreateDate = DateTime.UtcNow;
                    model.CreateUser = HttpContext.Current.User.Identity.Name;

                    db.SaveChanges();
                    model.OrderTransID = newrec.OrderTransID;
                }

                model.UpdateDate = DateTime.UtcNow;
                model.UpdateUser = HttpContext.Current.User.Identity.Name;

                var ordertrans = (from t in db.tblOrderTrans
                                  where t.OrderTransID == model.OrderTransID
                                  select t).FirstOrDefault();

                ordertrans.OrderID = model.OrderId;
                ordertrans.OrderItemID = model.OrderItemId;
                ordertrans.ClientID = model.ClientId;
                ordertrans.DivisionID = model.DivisionId;
                ordertrans.TransDate = model.TransDate;
                ordertrans.TransType = model.TransType;
                ordertrans.TransQty = model.TransQty ?? 1;
                ordertrans.TransRate = GetTransactionRate(model.ClientId, model.TransType, model.TransRate);
                ordertrans.TransAmount = ordertrans.TransQty * ordertrans.TransRate;
                ordertrans.Comments = model.Comments;
                ordertrans.CreateDate = model.CreateDate;
                ordertrans.CreateUser = model.CreateUser;
                ordertrans.UpdateDate = model.UpdateDate;
                ordertrans.UpdateUser = model.UpdateUser;

                db.SaveChanges();

                return model.OrderTransID;
            }
        }

        public static decimal? GetTransactionRate(int? clientid, string transtype, decimal? transrate)
        {
            decimal? rate = 0;

            using (var db = new CMCSQL03Entities())
            {
                var rates = (from t in db.tblRates
                             where t.ClientID == clientid
                             select t).FirstOrDefault();

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

        public static OrderTrans fnFillTrans(int id)
        {
            OrderTrans orderitemtransaction = new OrderTrans();

            using (var db = new CMCSQL03Entities())
            {
                var result = (from t in db.tblOrderTrans
                              where t.OrderTransID == id
                              select t).FirstOrDefault();

                orderitemtransaction.OrderTransID = result.OrderTransID;
                orderitemtransaction.OrderId = result.OrderID;
                orderitemtransaction.OrderItemId = result.OrderItemID;
                orderitemtransaction.ClientId = result.ClientID;
                orderitemtransaction.DivisionId = result.DivisionID;
                orderitemtransaction.TransDate = result.TransDate;
                orderitemtransaction.TransType = result.TransType;
                orderitemtransaction.TransQty = result.TransQty;
                orderitemtransaction.TransRate = result.TransRate;
                orderitemtransaction.TransAmount = result.TransAmount;
                orderitemtransaction.BillingTier = result.BillingTier;
                orderitemtransaction.BillingRate = result.BillingRate;
                orderitemtransaction.BillingCharge = result.BillingCharge;
                orderitemtransaction.Comments = result.Comments;
                orderitemtransaction.CreateDate = result.CreateDate;
                orderitemtransaction.CreateUser = result.CreateUser;
                orderitemtransaction.UpdateDate = result.UpdateDate;
                orderitemtransaction.UpdateUser = result.UpdateUser;

                return orderitemtransaction;
            }
        }

        public static void fnDeleteTrans(int id)
        {
            using (var db = new CMCSQL03Entities())
            {
                string s = @"DELETE FROM tblOrderTrans WHERE OrderTransID=" + id.ToString();
                db.Database.ExecuteSqlCommand(s);
            }
        }

        public static void fnGenerateOrderTransactions(int id)
        {
            using (var db = new CMCSQL03Entities())
            {
                var orderitem = (from t in db.tblOrderItem
                                 where t.ItemID == id
                                 select t).FirstOrDefault();

                var order = (from t in db.tblOrderMaster
                             where t.OrderID == orderitem.OrderID
                             select t).FirstOrDefault();

                // Tier 1 sample charge
                string sqlquery = "DELETE FROM tblOrderTrans WHERE OrderItemID=" + orderitem.ItemID + " AND Transtype = 'SAMP' AND CreateUser='System'";
                db.Database.ExecuteSqlCommand(sqlquery);

                var tierSize = (from t in db.tblTier
                                where t.ClientID == order.ClientID
                                && t.Size == orderitem.Size
                                && t.TierLevel == 1
                                select t).FirstOrDefault();

                if (tierSize != null)
                {
                    tblOrderTrans newrec = new tblOrderTrans();
                    newrec.TransDate = DateTime.UtcNow;
                    newrec.OrderItemID = orderitem.ItemID;
                    newrec.OrderID = orderitem.OrderID;
                    newrec.ClientID = order.ClientID;
                    newrec.DivisionID = order.DivisionID;
                    newrec.TransType = "SAMP";
                    newrec.TransQty = orderitem.Qty;
                    newrec.TransRate = tierSize.Price;
                    newrec.TransAmount = newrec.TransQty * newrec.TransRate;
                    newrec.BillingTier = 1;
                    newrec.BillingRate = tierSize.Price;
                    newrec.BillingCharge = newrec.TransQty * newrec.TransRate;
                    newrec.CreateDate = DateTime.UtcNow;
                    newrec.CreateUser = HttpContext.Current.User.Identity.Name;
                    newrec.UpdateDate = DateTime.UtcNow;
                    newrec.UpdateUser = HttpContext.Current.User.Identity.Name;

                    db.tblOrderTrans.Add(newrec);
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
                        tblOrderTrans newrec = new tblOrderTrans();
                        newrec.TransDate = DateTime.UtcNow;
                        newrec.OrderItemID = orderitem.ItemID;
                        newrec.OrderID = orderitem.OrderID;
                        newrec.ClientID = order.ClientID;
                        newrec.DivisionID = order.DivisionID;
                        newrec.TransType = "SAMP";
                        newrec.TransQty = orderitem.Qty;
                        newrec.TransRate = tierSpecialRequest.Price;
                        newrec.TransAmount = newrec.TransQty * newrec.TransRate;
                        newrec.BillingTier = 1;
                        newrec.BillingRate = tierSize.Price;
                        newrec.BillingCharge = newrec.TransQty * newrec.TransRate;
                        newrec.Comments = "Special Request";
                        newrec.CreateDate = DateTime.UtcNow;
                        newrec.CreateUser = HttpContext.Current.User.Identity.Name;
                        newrec.UpdateDate = DateTime.UtcNow;
                        newrec.UpdateUser = HttpContext.Current.User.Identity.Name;

                        db.tblOrderTrans.Add(newrec);
                        db.SaveChanges();
                    }
                }

                // Other charges from shelfmaster
                var shelf = (from t in db.tblShelfMaster
                             where t.ShelfID == orderitem.ShelfID
                             select t).FirstOrDefault();

                if (orderitem.BulkID != null)
                {
                    // find a SM record to use to surcharge this order item
                    var bulk = (from t in db.tblBulk
                                where t.BulkID == orderitem.BulkID
                                select t).FirstOrDefault();

                    var productmaster = (from t in db.tblProductMaster
                                         where t.ProductMasterID == bulk.ProductMasterID
                                         select t).FirstOrDefault();

                    var productdetail = (from t in db.tblProductDetail
                                         where t.ProductMasterID == bulk.ProductMasterID
                                         && t.ProductCode == productmaster.MasterCode
                                         select t).FirstOrDefault();

                    shelf = (from t in db.tblShelfMaster
                             where t.ProductDetailID == productdetail.ProductDetailID
                             && t.Size == orderitem.Size
                             select t).FirstOrDefault();

                    if (shelf == null)
                    {
                        return;
                    }
                }

                var surcharge = (from t in db.tblSurcharge
                                 where t.ClientID == order.ClientID
                                 select t).FirstOrDefault();

                if (surcharge != null)
                {
                    if (shelf.HazardSurcharge == true)
                    {
                        fnInsertOrderTrans(orderitem.ItemID, "HAZD", surcharge.Haz);
                    }

                    if (shelf.FlammableSurcharge == true)
                    {
                        fnInsertOrderTrans(orderitem.ItemID, "FLAM", surcharge.Flam);
                    }

                    if (shelf.HeatSurcharge == true)
                    {
                        fnInsertOrderTrans(orderitem.ItemID, "HEAT", surcharge.Heat);
                    }

                    if (shelf.RefrigSurcharge == true)
                    {
                        fnInsertOrderTrans(orderitem.ItemID, "REFR", surcharge.Refrig);
                    }

                    if (shelf.FreezerSurcharge == true)
                    {
                        fnInsertOrderTrans(orderitem.ItemID, "FREZ", surcharge.Freezer);
                    }

                    if (shelf.CleanSurcharge == true)
                    {
                        fnInsertOrderTrans(orderitem.ItemID, "CLEN", surcharge.Clean);
                    }

                    if (shelf.BlendSurcharge == true)
                    {
                        fnInsertOrderTrans(orderitem.ItemID, "BLND", 0);
                    }

                    if (shelf.NalgeneSurcharge == true)
                    {
                        fnInsertOrderTrans(orderitem.ItemID, "NALG", surcharge.Nalgene);
                    }

                    if (shelf.NitrogenSurcharge == true)
                    {
                        fnInsertOrderTrans(orderitem.ItemID, "NITR", 0);
                    }

                    if (shelf.BiocideSurcharge == true)
                    {
                        fnInsertOrderTrans(orderitem.ItemID, "BIOC", 0);
                    }

                    if (shelf.KosherSurcharge == true)
                    {
                        fnInsertOrderTrans(orderitem.ItemID, "KOSH", 0);
                    }

                    if (shelf.LabelSurcharge == true)
                    {
                        fnInsertOrderTrans(orderitem.ItemID, "LABL", surcharge.LabelFee);
                    }

                    if (shelf.OtherSurcharge == true)
                    {
                        fnInsertOrderTrans(orderitem.ItemID, "OTHR", shelf.OtherSurchargeAmt);
                    }
                }
            }
        }

        public static void fnInsertOrderTrans(int? ItemID, string TransType, decimal? TransRate)
        {
            using (var db = new CMCSQL03Entities())
            {
                string s = String.Format("DELETE FROM tblOrderTrans WHERE OrderItemID={0} AND Transtype = '{1}' AND CreateUser='System'", ItemID, TransType);

                db.Database.ExecuteSqlCommand(s);

                var orderitem = (from t in db.tblOrderItem
                                 where t.ItemID == ItemID
                                 select t).FirstOrDefault();

                var order = (from t in db.tblOrderMaster
                             where t.OrderID == orderitem.OrderID
                             select t).FirstOrDefault();

                tblOrderTrans newrec = new tblOrderTrans();
                newrec.TransDate = DateTime.UtcNow;
                newrec.OrderItemID = ItemID;
                newrec.OrderID = orderitem.OrderID;
                newrec.ClientID = order.ClientID;
                newrec.DivisionID = order.DivisionID;
                newrec.TransDate = DateTime.UtcNow;
                newrec.TransType = TransType;
                newrec.TransQty = orderitem.Qty;
                newrec.TransRate = TransRate;
                newrec.TransAmount = newrec.TransQty * newrec.TransRate;
                newrec.CreateDate = DateTime.UtcNow;
                newrec.CreateUser = HttpContext.Current.User.Identity.Name;
                newrec.UpdateDate = DateTime.UtcNow;
                newrec.UpdateUser = HttpContext.Current.User.Identity.Name;

                db.tblOrderTrans.Add(newrec);
                db.SaveChanges();
            }
        }

        public int? GetDivisionId()
        {
            return 1;
        }

        #endregion Order Transaction Methods

        #region Allocate Methods

        public static int fnAllocateShelf(int OrderID, bool IncludeQCStock)
        {
            using (var db = new CMCSQL03Entities())
            {
                int AllocationCount = 0;

                // check order for items
                var orderitems = (from t in db.tblOrderItem
                                  join om in db.tblOrderMaster on t.OrderID equals om.OrderID
                                  where t.OrderID == OrderID
                                  && t.ShipDate == null
                                  && t.AllocateStatus == null
                                  && t.CSAllocate == true
                                  select t).ToList();

                // escape if nothing matches the criteria for allocation
                if (orderitems == null)
                {
                    return AllocationCount;
                }

                // check stock for each item in order
                foreach (var item in orderitems)
                {
                    var stock = (from t in db.tblStock
                                 join b in db.tblBulk on t.BulkID equals b.BulkID
                                 orderby b.CeaseShipDate
                                 where t.ShelfID == item.ShelfID
                                 select new
                                 {
                                     StockID = t.StockID,
                                     Warehouse = t.Warehouse,
                                     QtyOnHand = t.QtyOnHand,
                                     QtyAllocated = t.QtyAllocated ?? 0,
                                     Bin = t.Bin,
                                     ShelfStatus = t.ShelfStatus,
                                     ExpirationDate = b.ExpirationDate,
                                     CeaseShipDate = b.CeaseShipDate,
                                     LotNumber = b.LotNumber
                                 }).ToList();

                    // get ceaseshipdateoffset to consider if eligible for allocation on an order.
                    // consider ignoring this when handling returns, even though returns sidesteps this - Iffy
                    var qOffset = db.tblOrderMaster.Find(orderitems[0].OrderID);
                    int iOffset = Convert.ToInt32(qOffset.CeaseShipOffset);

                    // change to take negative numbers
                    if (iOffset != 0)
                    {
                        // if there's an offset on the order record, add that to today to build in extra time
                        DateTime dCease = DateTime.UtcNow.AddDays(iOffset);
                        stock = (from t in stock
                                 where t.CeaseShipDate > dCease
                                 select t).ToList();
                    }

                    // specific lot number requested
                    if (item.LotNumber != null)
                    {
                        stock = stock.Where(s => s.LotNumber == item.LotNumber).ToList();
                    }

                    // check for qcstock if requested,
                    // available and qc stock are exclusive to themselves as discussed with Chris - Iffy
                    if (IncludeQCStock == true)
                    {
                        stock = stock.Where(s => s.ShelfStatus == "QC").ToList();
                    }
                    else
                    {
                        stock = stock.Where(s => s.ShelfStatus == "AVAIL").ToList();
                    }

                    // Distilled stock list
                    stock = stock.OrderBy(s => s.CeaseShipDate).ToList();

                    // Page thru tblstock rows looking for the first record that has enough qty then bailout
                    foreach (var row in stock)
                    {
                        if (row.QtyOnHand - row.QtyAllocated >= item.Qty)
                        {
                            AllocationCount = AllocationCount + 1;

                            // update tblstock record (need to use separate qry)
                            var q = db.tblStock.Find(row.StockID);

                            q.QtyAllocated = (q.QtyAllocated ?? 0) + item.Qty;
                            q.QtyAvailable = q.QtyOnHand - item.Qty;

                            // update tblorderitem record
                            item.AllocatedStockID = row.StockID;
                            item.BulkID = q.BulkID;
                            item.Warehouse = row.Warehouse;
                            item.LotNumber = row.LotNumber;
                            item.AllocateStatus = "A";
                            item.Bin = row.Bin;
                            item.ExpirationDate = row.ExpirationDate;
                            item.CeaseShipDate = row.CeaseShipDate;
                            item.AllocatedDate = DateTime.UtcNow;
                            item.UpdateDate = DateTime.UtcNow;
                            item.UpdateUser = HttpContext.Current.User.Identity.Name;

                            db.SaveChanges();

                            break;

                            fnInsertLogRecord("SS-ALC", DateTime.UtcNow, row.StockID, null, item.Qty, null, DateTime.UtcNow, HttpContext.Current.User.Identity.Name, null, null);
                        }
                    }
                }

                return AllocationCount;
            }
        }

        public static void fnReverseAllocatedItem(int orderitemid)
        {
            using (var db = new CMCSQL03Entities())
            {
                var success = false;

                // Check if order item meets criteria for reversal
                var orderitem = (from t in db.tblOrderItem
                                 where t.ItemID == orderitemid
                                 && t.AllocateStatus == "A"
                                 && t.ShipDate == null
                                 select t).FirstOrDefault();

                // Check if bulk allocated order item and update bulk record
                if (orderitem.AllocatedStockID == null && orderitem.AllocatedBulkID != null)
                {
                    var updatebulk = (from t in db.tblBulk
                                      where t.BulkID == orderitem.AllocatedBulkID
                                      select t).FirstOrDefault();

                    // Note: User will need to delete order item to re-activate bulk status via Returns again
                    // unless we create bulk allocate method - Iffy
                    updatebulk.CurrentWeight = orderitem.Weight;
                    updatebulk.BulkStatus = "AVAIL";
                    updatebulk.MarkedForReturn = false;
                    updatebulk.UpdateDate = DateTime.UtcNow;
                    updatebulk.UpdateUser = HttpContext.Current.User.Identity.Name;

                    success = true;
                }

                // Check if stock allocated order item and update stock record
                if (orderitem.AllocatedStockID != null)
                {
                    var updatestock = (from t in db.tblStock
                                       where t.ShelfID == orderitem.ShelfID
                                       && t.Bin == orderitem.Bin
                                       select t).FirstOrDefault();

                    updatestock.QtyAvailable = updatestock.QtyAvailable + orderitem.Qty;
                    updatestock.QtyAllocated -= orderitem.Qty;
                    if (updatestock.ShelfStatus == "RETURN")
                    {
                        updatestock.ShelfStatus = "AVAIL";
                        updatestock.MarkedForReturn = false;
                    }
                    if (updatestock.QtyAllocated < 0)
                    {
                        updatestock.QtyAllocated = 0;                                           // zero out QtyAllocated if negative
                    }
                    updatestock.UpdateDate = DateTime.UtcNow;
                    updatestock.UpdateUser = HttpContext.Current.User.Identity.Name;

                    success = true;
                }

                // Make sure there is something to reverse before wiping out fields
                if (success == true)
                {
                    try
                    {
                        // Clear allocation related fields for order item
                        orderitem.AllocatedStockID = null;
                        orderitem.AllocatedBulkID = null;
                        orderitem.BulkID = null;
                        orderitem.AllocateStatus = null;
                        orderitem.AllocatedDate = null;
                        orderitem.CSAllocate = null;
                        orderitem.Bin = null;
                        orderitem.LotNumber = null;
                        orderitem.Warehouse = null;
                        orderitem.ExpirationDate = null;
                        orderitem.CeaseShipDate = null;
                        orderitem.UpdateDate = DateTime.UtcNow;
                        orderitem.UpdateUser = HttpContext.Current.User.Identity.Name;

                        db.SaveChanges();
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
        }

        #endregion Allocate Methods

        #region Return Order Methods

        // Add bulk item to return order
        public static async Task<int> AddBulkItemToReturnOrder(int orderid, int bulkid)
        {
            using (var db = new CMCSQL03Entities())
            {
                var bulkitem = (from t in db.tblBulk
                                where t.BulkID == bulkid
                                select t).FirstOrDefault();

                var productmaster = (from t in db.tblProductMaster
                                     where t.ProductMasterID == bulkitem.ProductMasterID
                                     select t).FirstOrDefault();

                var productdetailid = OrderService.GetProductDetailId(productmaster.ProductMasterID);
                var productdetail = db.tblProductDetail.Find(productdetailid);

                int newitemid = OrderService.fnNewItemID();
                var newitem = (from t in db.tblOrderItem
                               where t.ItemID == newitemid
                               select t).FirstOrDefault();

                newitem.OrderID = orderid;
                newitem.BulkID = bulkitem.BulkID;
                newitem.ShelfID = OrderService.GetShelfIdProductMaster(productmaster.ProductMasterID, bulkitem.UM);
                newitem.ProductDetailID = productdetailid;
                newitem.ProductCode = productmaster.MasterCode;
                newitem.ProductName = productmaster.MasterName;
                newitem.LotNumber = bulkitem.LotNumber;
                newitem.Qty = 1;
                newitem.Size = bulkitem.UM;
                newitem.Weight = bulkitem.CurrentWeight;
                newitem.Bin = bulkitem.Bin;
                newitem.CSAllocate = true;
                newitem.AllocatedBulkID = bulkitem.BulkID;
                newitem.AllocateStatus = "A";
                newitem.AllocatedDate = DateTime.UtcNow;
                newitem.Warehouse = bulkitem.Warehouse;
                newitem.GrnUnNumber = productdetail.GRNUNNUMBER;
                newitem.GrnPkGroup = productdetail.GRNPKGRP;
                newitem.AirUnNumber = productdetail.AIRUNNUMBER;
                newitem.AirPkGroup = productdetail.AIRPKGRP;
                newitem.AlertNotesOrderEntry = productdetail.AlertNotesOrderEntry;
                newitem.AlertNotesShipping = productdetail.AlertNotesShipping;
                newitem.ItemNotes = "Return Bulk Order Item \nBulk container for return is " + bulkitem.UM + " with a current weight of " + bulkitem.CurrentWeight;
                newitem.CreateDate = DateTime.UtcNow;
                newitem.CreateUser = HttpContext.Current.User.Identity.Name;
                newitem.UpdateDate = DateTime.UtcNow;
                newitem.UpdateUser = HttpContext.Current.User.Identity.Name;

                // Insert log record
                OrderService.fnInsertLogRecord("BS-RTN", DateTime.UtcNow, null, bulkitem.BulkID, 1, bulkitem.CurrentWeight, DateTime.UtcNow, HttpContext.Current.User.Identity.Name, null, null);

                //update bulk record
                bulkitem.CurrentWeight = 0;
                bulkitem.BulkStatus = "RETURN";
                bulkitem.MarkedForReturn = true;
                bulkitem.UpdateDate = DateTime.UtcNow;
                bulkitem.UpdateUser = HttpContext.Current.User.Identity.Name;

                db.SaveChanges();

                OrderService.fnGenerateOrderTransactions(newitemid);
            }

            return orderid;
        }

        // Add stock item to new return order
        public static async Task<int> AddStockItemToReturnOrder(int orderid, int stockid)
        {
            using (var db = new CMCSQL03Entities())
            {
                var stockitem = (from t in db.tblStock
                                 where t.StockID == stockid
                                 select t).FirstOrDefault();

                var bl = db.tblBulk.Find(stockitem.BulkID);
                var sm = db.tblShelfMaster.Find(stockitem.ShelfID);
                var pd = db.tblProductDetail.Find(sm.ProductDetailID);
                var pm = db.tblProductMaster.Find(bl.ProductMasterID);

                int newitemid = OrderService.fnNewItemID();
                var newitem = (from t in db.tblOrderItem
                               where t.ItemID == newitemid
                               select t).FirstOrDefault();

                newitem.OrderID = orderid;
                newitem.ShelfID = sm.ShelfID;
                newitem.ProductDetailID = pd.ProductDetailID;
                newitem.ProductCode = pd.ProductCode;
                newitem.ProductName = pd.ProductName;
                newitem.LotNumber = bl.LotNumber;
                newitem.Qty = stockitem.QtyOnHand;
                newitem.Size = sm.Size;
                newitem.Weight = Convert.ToDecimal(sm.UnitWeight * stockitem.QtyOnHand);
                newitem.Bin = stockitem.Bin;
                newitem.CSAllocate = true;
                newitem.AllocatedBulkID = stockitem.BulkID;
                newitem.AllocatedStockID = stockitem.StockID;
                newitem.AllocateStatus = "A";
                newitem.AllocatedDate = DateTime.UtcNow;
                newitem.Warehouse = stockitem.Warehouse;
                newitem.GrnUnNumber = pd.GRNUNNUMBER;
                newitem.GrnPkGroup = pd.GRNPKGRP;
                newitem.AirUnNumber = pd.AIRUNNUMBER;
                newitem.AirPkGroup = pd.AIRPKGRP;
                newitem.AlertNotesOrderEntry = pd.AlertNotesOrderEntry;
                newitem.AlertNotesShipping = pd.AlertNotesShipping;
                newitem.ItemNotes = "Return Shelf Order Item";
                newitem.CreateDate = DateTime.UtcNow;
                newitem.CreateUser = HttpContext.Current.User.Identity.Name;
                newitem.UpdateDate = DateTime.UtcNow;
                newitem.UpdateUser = HttpContext.Current.User.Identity.Name;

                // Insert log record
                OrderService.fnInsertLogRecord("SS-RTN", DateTime.UtcNow, stockitem.StockID, null, 1, sm.UnitWeight, DateTime.UtcNow, HttpContext.Current.User.Identity.Name, null, null);

                //update tblstock record
                stockitem.QtyAllocated = stockitem.QtyAvailable;
                stockitem.QtyAvailable = 0;                                               // Clear out available stock
                stockitem.ShelfStatus = "RETURN";
                stockitem.MarkedForReturn = true;
                stockitem.UpdateDate = DateTime.UtcNow;
                stockitem.UpdateUser = HttpContext.Current.User.Identity.Name;

                db.SaveChanges();

                OrderService.fnGenerateOrderTransactions(newitemid);
            }

            return orderid;
        }

        #endregion Return Order Methods

        #region SPS Billing Methods

        public static OrderSPSBilling fnSPSBilling(int id)
        {
            // id=OrderID
            using (var db = new CMCSQL03Entities())
            {
                OrderSPSBilling vm = new OrderSPSBilling();
                var q = db.tblOrderSPSBilling.SingleOrDefault(i => i.OrderID == id);

                if (q == null)
                {
                    vm.SPSBillingID = -1;
                    vm.OrderId = id;
                    vm.PriceCost = CalcSPSSumCharge(id);
                    return vm;
                }

                vm.SPSBillingID = q.SPSBillingID;
                vm.OrderId = q.OrderID;
                vm.Type = q.Type;
                vm.TaxId = q.TaxID;
                vm.Currency = q.Currency;
                vm.PriceCost = CalcSPSSumCharge(id);
                vm.FreightCost = q.FreightCost;
                vm.ShippedWeight = q.ShippedWeight;
                vm.InvoiceTitle = q.InvoiceTitle;
                vm.InvoiceFirstName = q.InvoiceFirstName;
                vm.InvoiceLastName = q.InvoiceLastName;
                vm.InvoiceCompany = q.InvoiceCompany;
                vm.InvoiceAddress1 = q.InvoiceAddress1;
                vm.InvoiceAddress2 = q.InvoiceAddress2;
                vm.InvoiceAddress3 = q.InvoiceAddress3;
                vm.InvoiceCity = q.InvoiceCity;
                vm.InvoiceState = q.InvoiceState;
                vm.InvoicePostalCode = q.InvoicePostalCode;
                vm.InvoiceCountry = q.InvoiceCountry;
                vm.InvoicePhone = q.InvoicePhone;
                vm.InvoiceEmail = q.InvoiceEmail;
                vm.UpdateDate = DateTime.UtcNow;
                vm.UpdateUser = HttpContext.Current.User.Identity.Name;

                return vm;
            }
        }

        private static decimal? CalcSPSSumCharge(int OrderId)
        {
            using (var db = new CMCSQL03Entities())
            {
                var SumSPSCharge = db.tblOrderItem
                    .Where(t => t.OrderID == OrderId)
                    .Sum(t => t.SPSCharge);

                return SumSPSCharge;
            }
        }

        public static void fnSaveSPSBillingDetails(OrderSPSBilling vm)
        {
            using (var db = new CMCSQL03Entities())
            {
                if (vm.SPSBillingID == -1)
                {
                    tblOrderSPSBilling newrec = new tblOrderSPSBilling();
                    newrec.OrderID = vm.OrderId;
                    db.tblOrderSPSBilling.Add(newrec);
                    db.SaveChanges();
                }

                var q = db.tblOrderSPSBilling.SingleOrDefault(i => i.OrderID == vm.OrderId);

                q.Type = "Invoice";
                q.TaxID = vm.TaxId;
                q.Currency = "EUR";
                q.PriceCost = CalcSPSSumCharge(vm.OrderId);
                q.FreightCost = vm.FreightCost;
                q.ShippedWeight = vm.ShippedWeight;
                q.InvoiceTitle = vm.InvoiceTitle;
                q.InvoiceFirstName = vm.InvoiceFirstName;
                q.InvoiceLastName = vm.InvoiceLastName;
                q.InvoiceCompany = vm.InvoiceCompany;
                q.InvoiceAddress1 = vm.InvoiceAddress1;
                q.InvoiceAddress2 = vm.InvoiceAddress2;
                q.InvoiceAddress3 = vm.InvoiceAddress3;
                q.InvoiceCity = vm.InvoiceCity;
                q.InvoiceState = vm.InvoiceState;
                q.InvoicePostalCode = vm.InvoicePostalCode;
                q.InvoiceCountry = vm.InvoiceCountry;
                q.InvoicePhone = vm.InvoicePhone;
                q.InvoiceEmail = vm.InvoiceEmail;
                q.UpdateDate = DateTime.UtcNow;
                q.UpdateUser = HttpContext.Current.User.Identity.Name;

                db.SaveChanges();
            }
        }

        #endregion SPS Billing Methods

        #region List Methods

        public static List<OrderMasterFull> OpenOrdersAssigned()
        {
            using (var db = new CMCSQL03Entities())
            {
                List<OrderMasterFull> orderslist = new List<OrderMasterFull>();
                orderslist = OrderService.fnOrdersSearchResults();

                // Get list of order ids for orders not shipped.
                var unshippedorders = (from i in db.tblOrderItem
                                       where i.ShipDate == null
                                       && i.Qty > 0
                                       select i).ToList();

                unshippedorders = unshippedorders.GroupBy(x => x.OrderID)
                                                 .Select(g => g.First()).ToList();

                // Get list of open orders.
                orderslist = (from t in orderslist
                              join u in unshippedorders on t.OrderID equals u.OrderID
                              join c in db.tblClientAccountRep on t.ClientId equals c.ClientID
                              where c.AccountRepEmail == HttpContext.Current.User.Identity.Name
                              orderby t.OrderID descending, t.OrderDate descending
                              select t).ToList();

                // Display all clients in EU if user has no client assignments.
                // Since CMCEU does not have specific csr assignments for clients.
                if (orderslist.Count() == 0)
                {
                    orderslist = OrderService.fnOrdersSearchResults();
                    orderslist = (from t in orderslist
                                  join u in unshippedorders on t.OrderID equals u.OrderID
                                  join c in db.tblClient on t.ClientId equals c.ClientID
                                  where c.CMCLocation == "EU"
                                  orderby t.OrderID descending, t.OrderDate descending
                                  select t).ToList();
                }

                return orderslist;
            }
        }

        public static List<OrderMasterFull> fnOrdersSearchResults()
        {
            // default query join for the index_partial ORDERS search results,
            // also used by all the search requests as the starting point
            using (var db = new CMCSQL03Entities())
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                List<OrderMasterFull> orderslist = new List<OrderMasterFull>();
                orderslist = (from t in db.tblOrderMaster
                              join clt in db.tblClient on t.ClientID equals clt.ClientID
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
                                  ClientName = clt.ClientName,
                                  OrderType = t.OrderType,
                                  OrderDate = t.OrderDate,
                                  Company = t.Company,
                                  CreateUser = t.CreateUser,
                                  ItemsCount = count,
                                  Zip = t.Zip,
                                  SalesRepName = t.SalesRep,
                                  NeedAllocationCount = allocationcount
                              }).ToList();

                db.Configuration.AutoDetectChangesEnabled = true;

                return orderslist;
            }
        }

        public static Contact fnGetClientContacts(int id)
        {
            // fill a json object for View use
            using (var db = new CMCSQL03Entities())
            {
                Contact obj = new Contact();

                var q = (from t in db.tblClientContact
                         where t.ClientContactID == id
                         select t).FirstOrDefault();

                obj.FullName = q.FullName;
                obj.Email = q.Email;
                obj.Phone = q.Phone;
                obj.Phone = q.Fax;
                obj.Account = q.Account;

                return obj;
            }
        }

        #endregion List Methods

        #region Order Import Methods

        public static void PrepareForImport()
        {
            using (var db = new CMCSQL03Entities())
            {
                db.Database.ExecuteSqlCommand("DELETE FROM tblOrderImport WHERE Status NOT IN('0')");

                var orderimports = (from t in db.tblOrderImport
                                    where t.ImportStatus != "IMPORTED"
                                    && t.Location_MDB == "EU"
                                    select t).ToList();

                foreach (var item in orderimports)
                {
                    item.ImportStatus = "FAIL";

                    int? clientid = (from t in db.tblClient
                                     where t.CMCLongCustomer == item.Company_MDB
                                     && t.CMCLocation == item.Location_MDB
                                     select t.ClientID).FirstOrDefault();

                    if ((clientid ?? 0) == 0)
                    {
                        item.ImportError += " Client does not exist.";
                    }
                    else
                    {
                        item.ClientID = clientid;

                        int? divisionid = (from t in db.tblDivision
                                           where t.ClientID == item.ClientID
                                           && t.DivisionName == item.Division_MDB
                                           select t.DivisionID).FirstOrDefault();

                        if ((divisionid ?? 0) != 0)
                        {
                            item.DivisionID = divisionid;
                        }
                    }

                    // Make sure the product exists
                    int? productdetailid = (from pd in db.tblProductDetail
                                            join pm in db.tblProductMaster on pd.ProductMasterID equals pm.ProductMasterID
                                            where pd.ProductCode == item.ProductCode
                                            && pm.ClientID == item.ClientID
                                            select pd.ProductDetailID).FirstOrDefault();

                    if ((productdetailid ?? 0) == 0)
                    {
                        item.ImportError += " Requested product not found.";
                    }
                    else
                    {
                        item.ProductDetailID = productdetailid;

                        int? shelfmasterid = (from t in db.tblShelfMaster
                                              where t.ProductDetailID == item.ProductDetailID
                                              && t.Size == item.Size
                                              select t.ShelfID).FirstOrDefault();

                        // Create new shelfmaster if none found
                        if ((shelfmasterid ?? 0) == 0)
                        {
                            item.ShelfID = GetShelfIdProductDetail(item.ProductDetailID, item.Size);
                            item.ImportError = String.Format(" Missing shelf profile; shelf id: {0} created.", item.ShelfID);
                        }
                        else
                        {
                            item.ShelfID = shelfmasterid;
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
                var qFix = (from t in db.tblOrderImport
                            where t.ImportStatus == "FAIL"
                            select new { guid = t.GUID }).ToList().Distinct();

                foreach (var r in qFix)
                {
                    string s = String.Format("UPDATE tblOrderImport SET ImportStatus='FAIL' WHERE GUID='{0}'", r.guid);
                    db.Database.ExecuteSqlCommand(s);
                }

                return;
            }
        }

        public static int ImportOrders()
        {
            PrepareForImport();

            string sLocation = "EU"; // later move to parameter for this method

            using (var db = new CMCSQL03Entities())
            {
                int OrdersImportedCount = 0;

                // Get list of unique rows that passed precheck
                var OrderImportGUIDs = (from t in db.tblOrderImport
                                        where t.ImportStatus == "PASS"
                                        && t.Location_MDB == sLocation
                                        select new { t.GUID }).ToList().Distinct();

                // Create orders and insert items
                foreach (var row in OrderImportGUIDs)
                {
                    var orderimport = (from t in db.tblOrderImport
                                       where t.GUID == row.GUID
                                       select t).FirstOrDefault();

                    OrderMasterFull newOrder = new OrderMasterFull();

                    newOrder.OrderID = -1;
                    newOrder.OrderDate = Convert.ToDateTime(orderimport.OrderDate);
                    newOrder.ClientId = orderimport.ClientID;
                    newOrder.DivisionId = orderimport.DivisionID;
                    newOrder.IsSDN = orderimport.IsSDN;
                    newOrder.Customer = orderimport.Customer;
                    newOrder.CMCOrder = Convert.ToInt32(orderimport.CMCOrder);
                    newOrder.WebOrderId = Convert.ToInt32(orderimport.WebOrderID);
                    newOrder.CMCLegacyNumber = orderimport.CMCLegacyNum;
                    newOrder.ClientOrderNumber = orderimport.CustOrdNum;
                    newOrder.ClientSAPNumber = orderimport.CustSapNum;
                    newOrder.ClientRefNumber = orderimport.CustRefNum;
                    if (orderimport.OrderType == "w")
                    {
                        newOrder.OrderType = "S";
                    }
                    else
                    {
                        newOrder.OrderType = orderimport.OrderType;
                    }
                    if (orderimport.Source == null)
                    {
                        newOrder.Source = "Web";
                    }
                    else
                    {
                        newOrder.Source = orderimport.Source;
                    }
                    newOrder.Company = orderimport.Company;
                    newOrder.Street = orderimport.Street;
                    newOrder.Street2 = orderimport.Street2;
                    newOrder.Street3 = orderimport.Street3;
                    newOrder.City = orderimport.City;
                    newOrder.State = orderimport.State;
                    newOrder.Zip = orderimport.Zip;
                    newOrder.Country = orderimport.Country;
                    newOrder.Attention = orderimport.Attention;
                    newOrder.Email = orderimport.Email;
                    newOrder.SalesRepName = orderimport.SalesRep;
                    newOrder.SalesRepEmail = orderimport.SalesEmail;
                    newOrder.RequestorName = orderimport.Req;
                    newOrder.RequestorPhone = orderimport.ReqPhone;
                    newOrder.RequestorFax = orderimport.ReqFax;
                    newOrder.RequestorEmail = orderimport.ReqEmail;
                    newOrder.EndUse = orderimport.EndUse;
                    newOrder.ShipVia = orderimport.ShipVia;
                    newOrder.ShipAcct = orderimport.ShipAcct;
                    newOrder.Phone = orderimport.Phone;
                    newOrder.Fax = orderimport.Fax;
                    newOrder.Tracking = orderimport.Tracking;
                    newOrder.Special = orderimport.Special;
                    newOrder.SpecialInternal = orderimport.SpecialInternal;
                    newOrder.IsLiterature = Convert.ToBoolean(orderimport.Lit);
                    newOrder.Region = orderimport.Region;
                    newOrder.COA = Convert.ToBoolean(orderimport.COA);
                    newOrder.TDS = Convert.ToBoolean(orderimport.TDS);
                    newOrder.CID = orderimport.CID;
                    newOrder.ClientAcct = orderimport.CustAcct;
                    newOrder.ACode = orderimport.ACode;
                    newOrder.ImportFile = orderimport.ImportFile;
                    newOrder.ImportDateLine = orderimport.ImportDateLine;
                    newOrder.Timing = orderimport.Timing;
                    newOrder.Volume = orderimport.Volume;
                    newOrder.SampleRack = Convert.ToBoolean(orderimport.SampleRack);
                    newOrder.CMCUser = orderimport.CMCUser;
                    newOrder.ClientReference = orderimport.CustomerReference;
                    newOrder.TotalOrderWeight = orderimport.TotalOrderWeight;
                    newOrder.ClientOrderType = orderimport.CustOrderType;
                    newOrder.ClientRequestDate = orderimport.CustRequestDate;
                    newOrder.ApprovalDate = orderimport.ApprovalDate;
                    newOrder.RequestedDeliveryDate = orderimport.RequestedDeliveryDate;
                    newOrder.ClientTotalItems = Convert.ToInt32(orderimport.CustTotalItems);
                    newOrder.ClientRequestedCarrier = orderimport.CustRequestedCarrier;
                    newOrder.LegacyId = Convert.ToInt32(orderimport.LegacyID);
                    newOrder.SalesRepPhone = orderimport.SalesRepPhone;
                    newOrder.SalesRepTerritory = orderimport.SalesRepTerritory;
                    newOrder.MarketingRep = orderimport.MarketingRep;
                    newOrder.MarketingRepEmail = orderimport.MarketingRepEmail;
                    newOrder.Distributor = orderimport.Distributor;
                    newOrder.PreferredCarrier = orderimport.PreferredCarrier;
                    newOrder.ApprovalNeeded = Convert.ToBoolean(orderimport.ApprovalNeeded);
                    newOrder.CreateUser = orderimport.CreateUser;
                    newOrder.CreateDate = orderimport.CreateDate;
                    newOrder.UpdateUser = orderimport.UpdateUser;
                    newOrder.UpdateDate = orderimport.UpdateDate;

                    // Save order and get id
                    int newOrderId = Services.OrderService.fnSaveOrder(newOrder);

                    // Create order items
                    var orderimports = (from t in db.tblOrderImport
                                        where t.GUID == row.GUID
                                        select t).ToList();

                    foreach (var item in orderimports)
                    {
                        OrderItem newItem = new OrderItem();

                        newItem.ItemID = -1;
                        newItem.OrderID = newOrderId;
                        newItem.CreateDate = DateTime.UtcNow;
                        newItem.CreateUser = "System [Import]";
                        newItem.UpdateUser = HttpContext.Current.User.Identity.Name;
                        newItem.ProductDetailID = item.ProductDetailID;
                        newItem.ShelfID = item.ShelfID;
                        newItem.Qty = item.Qty;
                        newItem.LotNumber = item.LotNumber;
                        newItem.ShipDate = item.ShipDate;
                        newItem.CSAllocate = item.CSAllocate;
                        newItem.AllocateStatus = item.AllocateStatus;
                        newItem.NonCMCDelay = item.NonCMCDelay;
                        newItem.CarrierInvoiceRcvd = item.CarrierInvoiceRcvd;
                        newItem.Status = item.Status;
                        newItem.DelayReason = item.DelayReason;
                        newItem.ItemNotes = item.ItemNotes;

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
                                newItem.ItemNotes += String.Format(" {0} was imported as a special request size but it's not recognized as a decimal.", item.SRSize);
                            }

                            newItem.SRSize = srsize;
                        }

                        // Save order item and get id
                        int newItemId = Services.OrderService.fnSaveItem(newItem);
                    }

                    OrdersImportedCount = OrdersImportedCount + 1;

                    // Update order import status for successful imports
                    string s = String.Format("UPDATE tblOrderImport SET ImportStatus='IMPORTED', CMCLocation='{0}', OrderID='{1}' WHERE GUID='{2}'", sLocation, newOrderId, row.GUID);
                    db.Database.ExecuteSqlCommand(s);
                }

                return OrdersImportedCount;
            }
        }

        #endregion Order Import Methods

        public static void fnInsertLogRecord(string vLogType, DateTime? vLogDate, int? vStockID, int? vBulkID, int? vLogQty, decimal? vLogAmount, DateTime? vCreateDate, string vCreateUser, DateTime? vUpdateDate, string vUpdateUser)
        {
            using (var db = new CMCSQL03Entities())
            {
                tblInvLog InventoryLog = new tblInvLog();

                InventoryLog.LogType = vLogType;
                InventoryLog.LogDate = vLogDate;
                InventoryLog.StockID = vStockID;
                InventoryLog.BulkID = vBulkID;
                InventoryLog.LogQty = vLogQty;
                InventoryLog.LogAmount = vLogAmount;
                InventoryLog.CreateDate = vCreateDate;
                InventoryLog.CreateUser = vCreateUser;
                InventoryLog.UpdateDate = vUpdateDate;
                InventoryLog.UpdateUser = vUpdateUser;

                db.tblInvLog.Add(InventoryLog);
                db.SaveChanges();
            }
        }
    }
}