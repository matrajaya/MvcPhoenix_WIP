using MvcPhoenix.EF;
using MvcPhoenix.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcPhoenix.Services
{
    public class OrderService
    {
        #region Order Master Methods

        public static OrderMasterFull fnCreateOrder(int id)
        {
            // id=clientid
            using (var db = new CMCSQL03Entities())
            {
                OrderMasterFull vm = new OrderMasterFull();

                vm.orderid = -1;
                vm.clientid = id;
                var cl = db.tblClient.Find(vm.clientid);
                vm.clientname = cl.ClientName;
                vm.orderstatus = "z";
                vm.IsSDN = false;
                vm.orderdate = DateTime.UtcNow;
                vm.CreateUser = HttpContext.Current.User.Identity.Name;
                vm.CreateDate = DateTime.UtcNow;

                return vm;
            }
        }

        public static OrderMasterFull fnFillOrder(int id)
        {
            using (var db = new CMCSQL03Entities())
            {
                OrderMasterFull o = new OrderMasterFull();
                var q = (from t in db.tblOrderMaster
                         where t.OrderID == id
                         select t).FirstOrDefault();

                o.itemscount = (from t in db.tblOrderItem
                                where t.OrderID == id
                                select t).Count();

                o.transcount = (from t in db.tblOrderTrans
                                where t.OrderID == id
                                select t).Count();

                var cl = db.tblClient.Find(q.ClientID);

                o.clientname = cl.ClientName;
                o.clientcode = cl.ClientCode;
                o.orderid = q.OrderID;
                o.clientid = q.ClientID;
                o.divisionid = q.DivisionID;
                o.billinggroup = q.BillingGroup;
                o.orderstatus = q.OrderStatus;
                o.customer = q.Customer;
                o.cmcorder = Convert.ToInt32(q.CMCOrder);
                o.weborderid = Convert.ToInt32(q.WebOrderID);
                o.cmclegacynumber = q.CMCLegacyNum;
                o.custordnum = q.CustOrdNum;
                o.custsapnum = q.CustSapNum;
                o.custrefnum = q.CustRefNum;
                o.ordertype = q.OrderType;
                o.orderdate = q.OrderDate;
                o.company = q.Company;
                o.street = q.Street;
                o.street2 = q.Street2;
                o.street3 = q.Street3;
                o.city = q.City;
                o.state = q.State;
                o.Zip = q.Zip;
                o.country = q.Country;
                o.CustomsRefNum = q.CustomsRefNum;
                o.attention = q.Attention;
                o.email = q.Email;
                o.salesrep = q.SalesRep;
                o.sales_email = q.SalesEmail;
                o.req = q.Req;
                o.reqphone = q.ReqPhone;
                o.reqfax = q.ReqFax;
                o.reqemail = q.ReqEmail;
                o.enduse = q.EndUse;
                o.shipvia = q.ShipVia;
                o.shipacct = q.ShipAcct;
                o.phone = q.Phone;
                o.source = q.Source;
                o.fax = q.Fax;
                o.tracking = q.Tracking;
                o.special = q.Special;
                o.specialinternal = q.SpecialInternal;
                o.lit = Convert.ToBoolean(q.Lit);
                o.region = q.Region;
                o.coa = Convert.ToBoolean(q.COA);
                o.tds = Convert.ToBoolean(q.TDS);
                o.cid = q.CID;
                o.custacct = q.CustAcct;
                o.acode = q.ACode;
                o.importfile = q.ImportFile;

                if (q.ImportDateLine.HasValue)
                {
                    o.importdateline = Convert.ToDateTime(q.ImportDateLine);
                }
                else
                {
                    o.importdateline = null;
                }

                o.timing = q.Timing;
                o.volume = q.Volume;
                o.samplerack = Convert.ToBoolean(q.SampleRack);
                o.cmcuser = q.CMCUser;
                o.customerreference = q.CustomerReference;
                o.totalorderweight = q.TotalOrderWeight;
                o.custordertype = q.CustOrderType;

                o.custrequestdate = null;
                if (q.CustRequestDate.HasValue)
                {
                    o.custrequestdate = q.CustRequestDate;
                }

                o.approvaldate = null;
                if (q.ApprovalDate.HasValue)
                {
                    o.approvaldate = q.ApprovalDate;
                }

                o.requesteddeliverydate = null;
                if (q.RequestedDeliveryDate.HasValue)
                {
                    o.requesteddeliverydate = q.RequestedDeliveryDate;
                }

                o.custtotalitems = Convert.ToInt32(q.CustTotalItems);
                o.custrequestedcarrier = q.CustRequestedCarrier;
                o.legacyid = Convert.ToInt32(q.LegacyID);
                o.salesrepphone = q.SalesRepPhone;
                o.salesrepterritory = q.SalesRepTerritory;
                o.marketingrep = q.MarketingRep;
                o.marketingrepemail = q.MarketingRepEmail;
                o.distributor = q.Distributor;
                o.preferredcarrier = q.PreferredCarrier;
                o.approvalneeded = Convert.ToBoolean(q.ApprovalNeeded);
                o.CreateUser = q.CreateUser;
                o.CreateDate = q.CreateDate;
                o.UpdateUser = q.UpdateUser;
                o.UpdateDate = q.UpdateDate;
                o.IsSDN = q.IsSDN;

                return o;
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
                if (vm.orderid == -1)
                {
                    vm.orderid = fnNewOrderID();
                }

                // update time stamps
                vm.UpdateUser = HttpContext.Current.User.Identity.Name;
                vm.UpdateDate = DateTime.UtcNow;

                var q = (from t in db.tblOrderMaster
                         where t.OrderID == vm.orderid
                         select t).FirstOrDefault();

                q.OrderDate = vm.orderdate;
                q.ClientID = vm.clientid;
                q.DivisionID = vm.divisionid;
                q.BillingGroup = vm.billinggroup;               // divisionname + businessunit. Rethink changing to divisionid since it is more unique now.
                q.Customer = vm.customer;
                q.CMCOrder = vm.cmcorder;
                q.WebOrderID = vm.weborderid;
                q.CMCLegacyNum = vm.cmclegacynumber;
                q.CustOrdNum = vm.custordnum;
                q.CustSapNum = vm.custsapnum;
                q.CustRefNum = vm.custrefnum;
                q.OrderType = vm.ordertype;
                q.OrderDate = vm.orderdate;
                q.Company = vm.company;
                q.Street = vm.street;
                q.Street2 = vm.street2;
                q.Street3 = vm.street3;
                q.City = vm.city;
                q.State = vm.state;
                q.Zip = vm.Zip;
                q.Country = vm.country;
                q.Attention = vm.attention;
                q.CustomsRefNum = vm.CustomsRefNum;
                q.Email = vm.email;
                q.SalesRep = vm.salesrep;
                q.SalesEmail = vm.sales_email;
                q.Req = vm.req;
                q.ReqPhone = vm.reqphone;
                q.ReqFax = vm.reqfax;
                q.ReqEmail = vm.reqemail;
                q.EndUse = vm.enduse;
                q.ShipVia = vm.shipvia;
                q.ShipAcct = vm.shipacct;
                q.Phone = vm.phone;
                q.Source = vm.source;
                q.Fax = vm.fax;
                q.Tracking = vm.tracking;
                q.Special = vm.special;
                q.SpecialInternal = vm.specialinternal;
                q.Lit = Convert.ToBoolean(vm.lit);
                q.Region = vm.region;
                q.COA = vm.coa;
                q.TDS = vm.tds;
                q.CID = vm.cid;
                q.CustAcct = vm.custacct;
                q.ACode = vm.acode;
                q.ImportFile = vm.importfile;
                q.ImportDateLine = vm.importdateline;
                q.Timing = vm.timing;
                q.Volume = vm.volume;
                q.SampleRack = Convert.ToBoolean(vm.samplerack);
                q.CMCUser = vm.cmcuser;
                q.CustomerReference = vm.customerreference;
                q.TotalOrderWeight = (vm.totalorderweight);
                q.CustOrderType = vm.custordertype;
                q.CustRequestDate = vm.custrequestdate;
                q.ApprovalDate = vm.approvaldate;
                q.RequestedDeliveryDate = vm.requesteddeliverydate;
                q.CustTotalItems = vm.custtotalitems;
                q.CustRequestedCarrier = vm.custrequestedcarrier;
                q.LegacyID = (vm.legacyid);
                q.SalesRepPhone = vm.salesrepphone;
                q.SalesRepTerritory = vm.salesrepterritory;
                q.MarketingRep = vm.marketingrep;
                q.MarketingRepEmail = vm.marketingrepemail;
                q.Distributor = vm.distributor;
                q.PreferredCarrier = vm.preferredcarrier;
                q.ApprovalNeeded = vm.approvalneeded;
                q.UpdateUser = vm.UpdateUser;
                q.UpdateDate = vm.UpdateDate;
                q.CreateUser = vm.CreateUser;
                q.CreateDate = vm.CreateDate;
                q.IsSDN = false;

                db.SaveChanges();

                fnSaveOrderPostUpdate(vm);

                return vm.orderid;
            }
        }

        public static void fnSaveOrderPostUpdate(OrderMasterFull vm)
        {
            // changes to order record after it is saved
            using (var db = new CMCSQL03Entities())
            {
                bool ShowAlert = false;

                var q = (from t in db.tblOrderMaster
                         where t.OrderID == vm.orderid
                         select t).FirstOrDefault();

                var qCountry = (from t in db.tblCountry
                                where t.Country == vm.country
                                && t.DoNotShip == true
                                select t).FirstOrDefault();

                if (qCountry != null)
                {
                    // flag the order record and the item records that are yet to be allocated
                    q.IsSDN = true;
                    var orderitems = (from t in db.tblOrderItem
                                      where t.OrderID == vm.orderid
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
                                      where t.OrderID == vm.orderid
                                      && t.AllocateStatus == null
                                      select t).ToList();

                    foreach (var item in orderitems)
                    {
                        item.CSAllocate = false;
                        db.SaveChanges();
                    }

                    ShowAlert = true;
                }

                if (vm.country != "0")
                {
                    var qCountry2 = (from t in db.tblCountry
                                     where t.Country == vm.country
                                     select t).FirstOrDefault(); // need the ID

                    var qCeaseShipOffset = (from t in db.tblCeaseShipOffSet
                                            where t.ClientID == vm.clientid
                                            && t.CountryID == qCountry2.CountryID
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

        public static bool fnIsSDN(OrderMasterFull vm)
        {
            var filecontent = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Content/sdnlist.txt"));

            if (filecontent.Contains(vm.company))
            {
                return true;
            }

            if (!String.IsNullOrEmpty(vm.street) && filecontent.Contains(vm.street))
            {
                return true;
            }

            if (!String.IsNullOrEmpty(vm.attention) && filecontent.Contains(vm.attention))
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
                                  select new MvcPhoenix.Models.OrderItem
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

        public static OrderItem fnCreateItem(int id)
        {
            // id=OrderID
            using (var db = new CMCSQL03Entities())
            {
                var dbOrder = db.tblOrderMaster.Find(id);

                OrderItem vm = new OrderItem();

                vm.CrudMode = "RW";
                vm.ItemID = -1;
                vm.OrderID = id;
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
                vm.CarrierInvoiceRcvd = false;
                vm.DelayReason = null;
                vm.StatusID = -1;
                vm.AlertNotesShipping = "<< AlertNotesShipping >>";
                vm.AlertNotesPackOut = "<< AlertNotesPackOut >>";
                vm.AlertNotesOrderEntry = "<< AlertNotesOrderEntry >>";
                vm.AlertNotesOther = "<< AlertNotesOther >>";

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

        public static OrderItem fnFillOrderItem(int id)
        {
            // id=itemid
            // Fill all the fields from the tblOrderItem record
            using (var db = new CMCSQL03Entities())
            {
                OrderItem vm = new OrderItem();
                var q = (from t in db.tblOrderItem
                         where t.ItemID == id
                         select t).FirstOrDefault();

                var cl = db.tblOrderMaster.Find(q.OrderID);

                vm.CrudMode = "RW";                                         // default value

                // Hidden fields to persist for postback
                vm.ItemID = q.ItemID;
                vm.OrderID = q.OrderID;
                vm.ClientID = Convert.ToInt32(cl.ClientID);
                vm.CreateDate = q.CreateDate;
                vm.CreateUser = q.CreateUser;
                vm.UpdateDate = q.UpdateDate;
                vm.UpdateUser = q.UpdateUser;
                vm.ProductDetailID = q.ProductDetailID;
                vm.ProductCode = q.ProductCode;
                vm.ProductName = q.ProductName;
                vm.ShelfID = q.ShelfID;
                vm.Size = q.Size;
                vm.SRSize = q.SRSize;
                vm.Qty = q.Qty;
                vm.LotNumber = q.LotNumber;
                vm.ItemShipVia = q.Via;
                vm.ShipDate = q.ShipDate;
                vm.CSAllocate = q.CSAllocate;
                vm.AllocateStatus = q.AllocateStatus;
                vm.NonCMCDelay = q.NonCMCDelay;
                vm.BackOrdered = q.BackOrdered;
                vm.CarrierInvoiceRcvd = q.CarrierInvoiceRcvd;
                vm.DelayReason = q.DelayReason;
                vm.SPSCharge = q.SPSCharge;
                vm.Status = q.Status;
                vm.StatusID = null;
                vm.ItemNotes = q.ItemNotes;
                vm.WasteOrderTotalWeight = q.WasteOrderTotalWeight;
                vm.AlertNotesShipping = q.AlertNotesShipping;
                vm.AlertNotesPackOut = q.AlertNotesPackout;
                vm.AlertNotesOrderEntry = q.AlertNotesOrderEntry;
                vm.AlertNotesOther = q.AlertNotesOther;

                // Look for a reason to set the item R/O
                if (q.ShipDate != null || q.AllocateStatus == "A")
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
                if (vm.ItemID == -1)
                {
                    vm.ItemID = fnNewItemID();
                    vm.CreateDate = DateTime.UtcNow;
                    vm.CreateUser = HttpContext.Current.User.Identity.Name;
                }

                var q = (from t in db.tblOrderItem
                         where t.ItemID == vm.ItemID
                         select t).FirstOrDefault();

                var dbPD = (from t in db.tblProductDetail
                            where t.ProductDetailID == vm.ProductDetailID
                            select t).FirstOrDefault();

                var dbPM = db.tblProductMaster.Find(dbPD.ProductMasterID);

                if (vm.ShelfID == 0 || vm.SRSize != null)
                {
                    q.SRSize = vm.SRSize;
                    q.Size = "1SR";
                    vm.ShelfID = GetShelfIdfor1SR(vm.ProductDetailID, "1SR");
                }
                else
                {
                    var dbSM = (from t in db.tblShelfMaster
                                where t.ShelfID == vm.ShelfID
                                select t).FirstOrDefault();

                    if (dbSM != null)
                    {
                        q.Size = dbSM.Size;
                        q.Weight = dbSM.UnitWeight * vm.Qty;
                    }
                }

                q.OrderID = vm.OrderID;
                q.CreateDate = vm.CreateDate;
                q.CreateUser = vm.CreateUser;
                q.UpdateDate = DateTime.UtcNow;
                q.UpdateUser = HttpContext.Current.User.Identity.Name;
                q.ProductDetailID = vm.ProductDetailID;
                q.ProductCode = dbPD.ProductCode;
                q.ProductName = dbPD.ProductName;
                q.ShelfID = vm.ShelfID;
                q.Qty = vm.Qty;
                q.LotNumber = vm.LotNumber;
                q.Via = vm.ItemShipVia;
                q.ShipDate = vm.ShipDate;
                q.CSAllocate = vm.CSAllocate;
                q.AllocateStatus = vm.AllocateStatus;
                q.NonCMCDelay = vm.NonCMCDelay;
                q.BackOrdered = vm.BackOrdered;
                q.CarrierInvoiceRcvd = vm.CarrierInvoiceRcvd;
                q.Status = vm.Status;
                q.DelayReason = vm.DelayReason;
                q.SPSCharge = vm.SPSCharge;
                q.ItemNotes = vm.ItemNotes;
                q.WasteOrderTotalWeight = vm.WasteOrderTotalWeight;
                q.AlertNotesShipping = dbPD.AlertNotesShipping;
                q.AlertNotesOrderEntry = dbPD.AlertNotesOrderEntry;
                q.AlertNotesPackout = dbPM.AlertNotesPackout;

                if (dbPD.AIRUNNUMBER == "UN3082" | dbPD.AIRUNNUMBER == "UN3077" | dbPD.GRNUNNUMBER == "UN3082" | dbPD.GRNUNNUMBER == "UN3077")
                {
                    q.AlertNotesOther = "Products with UN3082 and UN3077 may be shipped as non hazardous if under 5 kg";
                }

                db.SaveChanges();

                fnGenerateOrderTransactions(vm.ItemID);         // Go do the Order Trans work....

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
        //  use productdetailid and um(size) to check shelfmaster
        //  if true: get shelfid ?: insert new record in shelfmaster using productdetailid and size = um
        /// </summary>
        public static int GetShelfId(int? productmasterid, string um)
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

        public static int GetShelfIdfor1SR(int? productdetailid, string size)
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

        public static OrderTrans fnCreateTrans(int id)
        {
            // id=orderid
            using (var db = new CMCSQL03Entities())
            {
                var vm = new OrderTrans();
                var cl = (from t in db.tblOrderMaster
                          where t.OrderID == id
                          select t).FirstOrDefault();

                vm.ordertransid = -1;
                vm.orderid = id;
                vm.transtype = "MEMO";
                vm.createdate = DateTime.UtcNow;
                vm.transdate = DateTime.UtcNow.Date;
                vm.createuser = HttpContext.Current.User.Identity.Name;
                vm.clientid = cl.ClientID;

                return vm;
            }
        }

        public static int fnSaveTrans(OrderTrans vm)
        {
            using (var db = new CMCSQL03Entities())
            {
                if (vm.ordertransid == -1)
                {
                    tblOrderTrans newrec = new tblOrderTrans();
                    db.tblOrderTrans.Add(newrec);
                    vm.createdate = DateTime.UtcNow;
                    vm.createuser = HttpContext.Current.User.Identity.Name;

                    db.SaveChanges();
                    vm.ordertransid = newrec.OrderTransID;
                }

                vm.updatedate = DateTime.UtcNow;
                vm.updateuser = HttpContext.Current.User.Identity.Name;

                var d = (from t in db.tblOrderTrans
                         where t.OrderTransID == vm.ordertransid
                         select t).FirstOrDefault();

                d.OrderID = vm.orderid;
                d.OrderItemID = vm.orderitemid;
                d.ClientID = vm.clientid;
                d.TransDate = vm.transdate;
                d.TransType = vm.transtype;
                d.TransQty = vm.transqty;
                d.TransRate = vm.transrate;
                d.Comments = vm.comments;
                d.CreateDate = vm.createdate;
                d.CreateUser = vm.createuser;
                d.UpdateDate = vm.updatedate;
                d.UpdateUser = vm.updateuser;

                db.SaveChanges();

                return vm.ordertransid;
            }
        }

        public static OrderTrans fnFillTrans(int id)
        {
            OrderTrans orderitemtransaction = new OrderTrans();

            using (var db = new CMCSQL03Entities())
            {
                var result = (from t in db.tblOrderTrans
                              where t.OrderTransID == id
                              select t).FirstOrDefault();

                orderitemtransaction.ordertransid = result.OrderTransID;
                orderitemtransaction.orderid = result.OrderID;
                orderitemtransaction.orderitemid = result.OrderItemID;
                orderitemtransaction.clientid = result.ClientID;
                orderitemtransaction.transdate = result.TransDate;
                orderitemtransaction.transtype = result.TransType;
                orderitemtransaction.transqty = result.TransQty;
                orderitemtransaction.transrate = result.TransRate;
                orderitemtransaction.comments = result.Comments;
                orderitemtransaction.createdate = result.CreateDate;
                orderitemtransaction.createuser = result.CreateUser;
                orderitemtransaction.updatedate = result.UpdateDate;
                orderitemtransaction.updateuser = result.UpdateUser;

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
                var oi = (from t in db.tblOrderItem
                          where t.ItemID == id
                          select t).FirstOrDefault();

                var o = (from t in db.tblOrderMaster
                         where t.OrderID == oi.OrderID
                         select t).FirstOrDefault();

                var clt = (from t in db.tblClient
                           where t.ClientID == o.ClientID
                           select t).FirstOrDefault();
                string s = "";

                // Tier 1 sample charge
                s = "DELETE FROM tblOrderTrans WHERE OrderItemID=" + oi.ItemID + " AND Transtype = 'SAMP' AND CreateUser='System'";
                db.Database.ExecuteSqlCommand(s);

                var tierSize = (from t in db.tblTier
                                where t.ClientID == o.ClientID
                                && t.Size == oi.Size
                                && t.TierLevel == 1
                                select t).FirstOrDefault();

                if (tierSize != null)
                {
                    tblOrderTrans newrec = new tblOrderTrans();
                    newrec.TransDate = DateTime.UtcNow;
                    newrec.OrderItemID = oi.ItemID;
                    newrec.OrderID = oi.OrderID;
                    newrec.ClientID = o.ClientID;
                    newrec.BillingGroup = o.BillingGroup;
                    newrec.TransType = "SAMP";
                    newrec.TransQty = oi.Qty;
                    newrec.TransRate = tierSize.Price;
                    newrec.CreateDate = DateTime.UtcNow;
                    newrec.CreateUser = "System";
                    newrec.UpdateDate = DateTime.UtcNow;
                    newrec.UpdateUser = "System";

                    db.tblOrderTrans.Add(newrec);
                    db.SaveChanges();
                }
                else
                {
                    // Assume this is a SR size? - pc
                    var tierSpecialRequest = (from t in db.tblTier
                                              where t.ClientID == o.ClientID
                                              && t.Size == "1SR"
                                              && t.TierLevel == 1
                                              select t).FirstOrDefault();

                    if (tierSpecialRequest != null)
                    {
                        tblOrderTrans newrec = new tblOrderTrans();
                        newrec.TransDate = DateTime.UtcNow;
                        newrec.OrderItemID = oi.ItemID;
                        newrec.OrderID = oi.OrderID;
                        newrec.ClientID = o.ClientID;
                        newrec.BillingGroup = o.BillingGroup;
                        newrec.TransType = "SAMP";
                        newrec.TransQty = oi.Qty;
                        newrec.TransRate = tierSpecialRequest.Price;
                        newrec.Comments = "Special Request";
                        newrec.CreateDate = DateTime.UtcNow;
                        newrec.CreateUser = "System";
                        newrec.UpdateDate = DateTime.UtcNow;
                        newrec.UpdateUser = "System";

                        db.tblOrderTrans.Add(newrec);
                        db.SaveChanges();
                    }
                }

                // Other charges from shelfmaster

                // pc 10/27/16 Need to know if we are trying to price a Bulk or Stock item
                // assume a shelfmaster, then change to bulk if necessary
                var sm = (from t in db.tblShelfMaster
                          where t.ShelfID == oi.ShelfID
                          select t).FirstOrDefault();

                if (oi.BulkID != null)
                {
                    // find a SM record to use to surcharge this order item
                    var bl = (from t in db.tblBulk
                              where t.BulkID == oi.BulkID
                              select t).FirstOrDefault();

                    var pm = (from t in db.tblProductMaster
                              where t.ProductMasterID == bl.ProductMasterID
                              select t).FirstOrDefault();

                    var pd = (from t in db.tblProductDetail
                              where t.ProductMasterID == bl.ProductMasterID
                              && t.ProductCode == pm.MasterCode
                              select t).FirstOrDefault();

                    sm = (from t in db.tblShelfMaster
                          where t.ProductDetailID == pd.ProductDetailID
                          && t.Size == oi.Size
                          select t).FirstOrDefault();

                    if (sm == null)
                    {
                        // go no further
                        return;
                    }
                    // this should rest me on a shelfmaster that is really the bulk size sample
                }

                var surcharge = (from t in db.tblSurcharge
                                 where t.ClientID == o.ClientID
                                 select t).FirstOrDefault();

                if (surcharge != null)
                {
                    if (sm.HazardSurcharge == true)
                    {
                        fnInsertOrderTrans(oi.ItemID, "HAZD", surcharge.Haz);
                    }

                    if (sm.FlammableSurcharge == true)
                    {
                        fnInsertOrderTrans(oi.ItemID, "FLAM", surcharge.Flam);
                    }

                    if (sm.HeatSurcharge == true)
                    {
                        fnInsertOrderTrans(oi.ItemID, "HEAT", surcharge.Heat);
                    }

                    if (sm.RefrigSurcharge == true)
                    {
                        fnInsertOrderTrans(oi.ItemID, "REFR", surcharge.Refrig);
                    }

                    if (sm.FreezerSurcharge == true)
                    {
                        fnInsertOrderTrans(oi.ItemID, "FREZ", surcharge.Freezer);
                    }

                    if (sm.CleanSurcharge == true)
                    {
                        fnInsertOrderTrans(oi.ItemID, "CLEN", surcharge.Clean);
                    }

                    if (sm.BlendSurcharge == true)
                    {
                        fnInsertOrderTrans(oi.ItemID, "BLND", 0);
                    }

                    if (sm.NalgeneSurcharge == true)
                    {
                        fnInsertOrderTrans(oi.ItemID, "NALG", surcharge.Nalgene);
                    }

                    if (sm.NitrogenSurcharge == true)
                    {
                        fnInsertOrderTrans(oi.ItemID, "NITR", 0);
                    }

                    if (sm.BiocideSurcharge == true)
                    {
                        fnInsertOrderTrans(oi.ItemID, "BIOC", 0);
                    }

                    if (sm.KosherSurcharge == true)
                    {
                        fnInsertOrderTrans(oi.ItemID, "KOSH", 0);
                    }

                    if (sm.LabelSurcharge == true)
                    {
                        fnInsertOrderTrans(oi.ItemID, "LABL", surcharge.LabelFee);
                    }

                    if (sm.OtherSurcharge == true)
                    {
                        fnInsertOrderTrans(oi.ItemID, "OTHR", sm.OtherSurchargeAmt);
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

                var oi = (from t in db.tblOrderItem
                          where t.ItemID == ItemID
                          select t).FirstOrDefault();

                var o = (from t in db.tblOrderMaster
                         where t.OrderID == oi.OrderID
                         select t).FirstOrDefault();

                tblOrderTrans newrec = new tblOrderTrans();
                newrec.TransDate = DateTime.UtcNow;
                newrec.OrderItemID = ItemID;
                newrec.OrderID = oi.OrderID;
                newrec.ClientID = o.ClientID;
                newrec.BillingGroup = o.BillingGroup;
                newrec.TransDate = DateTime.UtcNow;
                newrec.TransType = TransType;
                newrec.TransQty = oi.Qty;
                newrec.TransRate = TransRate;
                newrec.CreateDate = DateTime.UtcNow;
                newrec.CreateUser = "System";
                newrec.UpdateDate = DateTime.UtcNow;
                newrec.UpdateUser = "System";

                db.tblOrderTrans.Add(newrec);
                db.SaveChanges();
            }
        }

        #endregion Order Transaction Methods

        #region Allocate Methods

        #region Bulk Allocate not being implemented currently

        //public static int fnAllocateBulk(int OrderID, bool IncludeQCStock)
        //{
        //    using (var db = new CMCSQL03Entities())
        //    {
        //        int AllocationCount = 0;
        //        var orderitems = (from t in db.tblOrderItem
        //                          where t.OrderID == OrderID
        //                          && t.ShipDate == null
        //                          && t.AllocateStatus == null
        //                          && t.CSAllocate == true
        //                          select t).ToList();

        //        if (orderitems == null)
        //        {
        //            return AllocationCount;
        //        }

        //        foreach (var item in orderitems)
        //        {
        //            var tblbulk = (from t in db.tblBulk
        //                           join pm in db.tblProductMaster on t.ProductMasterID equals pm.ProductMasterID
        //                           join pd in db.tblProductDetail on pm.ProductMasterID equals pd.ProductMasterID
        //                           where (pd.ProductDetailID == item.ProductDetailID)
        //                           select new
        //                           {
        //                               BulkID = t.BulkID,
        //                               Warehouse = t.Warehouse,
        //                               Qty = t.Qty,
        //                               LotNumber = t.LotNumber,
        //                               CurrentWeight = t.CurrentWeight,
        //                               CeaseShipDate = t.CeaseShipDate,
        //                               BulkStatus = t.BulkStatus,
        //                               Bin = t.Bin
        //                           }).ToList();

        //            var qOffset = db.tblOrderMaster.Find(OrderID);
        //            int iOffset = Convert.ToInt32(qOffset.CeaseShipOffset);

        //            if (iOffset != 0)
        //            {
        //                // if there's an offset on the order record, add that to today to build in extra time
        //                DateTime dCease = DateTime.UtcNow.AddDays(iOffset);

        //                tblbulk = (from t in tblbulk
        //                           where t.CeaseShipDate > dCease
        //                           select t).ToList();
        //            }

        //            if (IncludeQCStock == true)
        //            {
        //                tblbulk = (from t in tblbulk
        //                           where t.BulkStatus == "AVAIL" || t.BulkStatus == "QC"
        //                           select t).ToList();
        //            }
        //            else
        //            {
        //                tblbulk = (from t in tblbulk
        //                           where t.BulkStatus == "AVAIL"
        //                           select t).ToList();
        //            }

        //            tblbulk = (from t in tblbulk
        //                       where t.CurrentWeight >= (item.Qty * item.Weight)
        //                       select t).ToList();

        //            tblbulk = (from t in tblbulk
        //                       orderby t.CeaseShipDate ascending
        //                       select t).ToList();

        //            foreach (var row in tblbulk)
        //            {
        //                // update tblstock record (need to use separate qry)
        //                if (row.CurrentWeight >= item.Qty * item.Weight)
        //                {
        //                    AllocationCount = AllocationCount + 1;
        //                    var q = db.tblBulk.Find(row.BulkID);

        //                    q.CurrentWeight = q.CurrentWeight - (item.Qty * item.Weight);
        //                    item.AllocatedBulkID = row.BulkID;
        //                    item.Warehouse = row.Warehouse;
        //                    item.LotNumber = row.LotNumber;
        //                    item.AllocateStatus = "A";
        //                    item.Bin = row.Bin;
        //                    item.ExpirationDate = q.ExpirationDate;
        //                    item.CeaseShipDate = q.CeaseShipDate;

        //                    fnInsertLogRecord("BS-ALC", DateTime.UtcNow, null, row.BulkID, item.Qty, item.Weight, DateTime.UtcNow, HttpContext.Current.User.Identity.Name, null, null);

        //                    db.SaveChanges();

        //                    break;
        //                }
        //            }
        //        }

        //        return AllocationCount;
        //    }
        //}

        #endregion Bulk Allocate not being implemented currently

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

                    // check for qcstock if requested
                    if (IncludeQCStock == true)
                    {
                        stock = stock.Where(s => s.ShelfStatus == "AVAIL" || s.ShelfStatus == "QC").ToList();
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
                    if (updatestock.QtyAllocated < 0)
                    {
                        // zero out QtyAllocated if negative
                        updatestock.QtyAllocated = 0;
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

        public static void CreateReturnOrderBulkItems(int orderid)
        {
            using (var db = new CMCSQL03Entities())
            {
                var bulk = (from t in db.tblBulk
                            where t.MarkedForReturn == true
                            select t).ToList();

                foreach (var row in bulk)
                {
                    var pm = (from t in db.tblProductMaster
                              where t.ProductMasterID == row.ProductMasterID
                              select t).FirstOrDefault();

                    var productdetailid = OrderService.GetProductDetailId(pm.ProductMasterID);
                    var pd = db.tblProductDetail.Find(productdetailid);

                    int newItemId = OrderService.fnNewItemID();

                    var newitem = (from t in db.tblOrderItem
                                   where t.ItemID == newItemId
                                   select t).FirstOrDefault();

                    newitem.OrderID = orderid;
                    newitem.BulkID = row.BulkID;
                    newitem.ShelfID = OrderService.GetShelfId(pm.ProductMasterID, row.UM);
                    newitem.ProductDetailID = productdetailid;
                    newitem.ProductCode = pm.MasterCode;
                    newitem.ProductName = pm.MasterName;
                    newitem.LotNumber = row.LotNumber;
                    newitem.Qty = 1;
                    newitem.Size = row.UM;
                    newitem.Weight = row.CurrentWeight;
                    newitem.Bin = row.Bin;
                    newitem.CSAllocate = true;
                    newitem.AllocatedBulkID = row.BulkID;
                    newitem.AllocateStatus = "A";
                    newitem.AllocatedDate = DateTime.UtcNow;
                    newitem.Warehouse = row.Warehouse;
                    newitem.GrnUnNumber = pd.GRNUNNUMBER;
                    newitem.GrnPkGroup = pd.GRNPKGRP;
                    newitem.AirUnNumber = pd.AIRUNNUMBER;
                    newitem.AirPkGroup = pd.AIRPKGRP;
                    newitem.AlertNotesOrderEntry = pd.AlertNotesOrderEntry;
                    newitem.AlertNotesShipping = pd.AlertNotesShipping;
                    newitem.ItemNotes = "Return Bulk Order Item \nBulk container for return is " + row.UM + " with a current weight of " + row.CurrentWeight;
                    newitem.CreateDate = DateTime.UtcNow;
                    newitem.CreateUser = HttpContext.Current.User.Identity.Name;
                    newitem.UpdateDate = DateTime.UtcNow;
                    newitem.UpdateUser = HttpContext.Current.User.Identity.Name;

                    // Insert log record
                    OrderService.fnInsertLogRecord("BS-RTN", DateTime.UtcNow, null, row.BulkID, 1, row.CurrentWeight, DateTime.UtcNow, HttpContext.Current.User.Identity.Name, null, null);

                    //update bulk record
                    row.CurrentWeight = 0;
                    row.BulkStatus = "RETURN";
                    row.MarkedForReturn = true;
                    row.UpdateDate = DateTime.UtcNow;
                    row.UpdateUser = HttpContext.Current.User.Identity.Name;

                    db.SaveChanges();

                    OrderService.fnGenerateOrderTransactions(newItemId);
                }
            }
        }

        public static void CreateReturnOrderStockItems(int orderid)
        {
            using (var db = new CMCSQL03Entities())
            {
                // This assumes that all marked items are acoped for a particular client
                // what happens if multiple users execute at the same time, or if there are
                // pending marked items not intended for the intended client?
                // There is a potential for leaky abstractions here -- Iffy
                var stock = (from t in db.tblStock
                             where t.MarkedForReturn == true
                             select t).ToList();

                foreach (var row in stock)
                {
                    var bl = db.tblBulk.Find(row.BulkID);
                    var sm = db.tblShelfMaster.Find(row.ShelfID);
                    var pd = db.tblProductDetail.Find(sm.ProductDetailID);
                    var pm = db.tblProductMaster.Find(bl.ProductMasterID);

                    int newItemId = OrderService.fnNewItemID();
                    var newitem = (from t in db.tblOrderItem
                                   where t.ItemID == newItemId
                                   select t).FirstOrDefault();

                    newitem.OrderID = orderid;
                    newitem.ShelfID = sm.ShelfID;
                    newitem.ProductDetailID = pd.ProductDetailID;
                    newitem.ProductCode = pd.ProductCode;
                    newitem.ProductName = pd.ProductName;
                    newitem.LotNumber = bl.LotNumber;
                    newitem.Qty = row.QtyOnHand;
                    newitem.Size = sm.Size;
                    newitem.Weight = Convert.ToDecimal(sm.UnitWeight * row.QtyOnHand);
                    newitem.Bin = row.Bin;
                    newitem.CSAllocate = true;
                    newitem.AllocatedBulkID = row.BulkID;
                    newitem.AllocatedStockID = row.StockID;
                    newitem.AllocateStatus = "A";
                    newitem.AllocatedDate = DateTime.UtcNow;
                    newitem.Warehouse = row.Warehouse;
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
                    OrderService.fnInsertLogRecord("SS-RTN", DateTime.UtcNow, row.StockID, null, 1, sm.UnitWeight, DateTime.UtcNow, HttpContext.Current.User.Identity.Name, null, null);

                    //update tblstock record
                    row.QtyAllocated = row.QtyAvailable;
                    row.QtyAvailable = 0;                                               // Clear out available stock
                    row.ShelfStatus = "RETURN";
                    row.MarkedForReturn = true;
                    row.UpdateDate = DateTime.UtcNow;
                    row.UpdateUser = HttpContext.Current.User.Identity.Name;

                    db.SaveChanges();

                    OrderService.fnGenerateOrderTransactions(newItemId);
                }
            }
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
                                  clientid = t.ClientID,
                                  orderid = t.OrderID,
                                  customer = t.Customer,
                                  clientname = clt.ClientName,
                                  ordertype = t.OrderType,
                                  orderdate = t.OrderDate,
                                  company = t.Company,
                                  CreateUser = t.CreateUser,
                                  itemscount = count,
                                  Zip = t.Zip,
                                  salesrep = t.SalesRep,
                                  needallocationcount = allocationcount
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
            // this method tries to put the external record into Phoenix normalized fashion and tag for actual import
            using (var db = new CMCSQL03Entities())
            {
                db.Database.ExecuteSqlCommand("DELETE FROM tblOrderImport WHERE Status NOT IN('0')");

                var q = (from t in db.tblOrderImport
                         where t.ImportStatus == null && t.Location_MDB == "EU"
                         select t).ToList();

                foreach (var r in q)
                {
                    r.ImportStatus = "FAIL";

                    var qclt = (from t in db.tblClient
                                where t.CMCLongCustomer == r.Company_MDB
                                && t.CMCLocation == r.Location_MDB
                                select t).FirstOrDefault();

                    if (qclt == null)
                    {
                        r.ImportError += " [ClientID]";
                    }

                    if (qclt != null)
                    {
                        r.ClientID = qclt.ClientID;
                        var qdiv = (from t in db.tblDivision
                                    where t.ClientID == r.ClientID
                                    && t.DivisionName == r.Division_MDB
                                    select t).FirstOrDefault();

                        if (qdiv != null)
                        {
                            r.DivisionID = qdiv.DivisionID;
                        }
                    }

                    // will this always return one row ?????
                    var qPD = (from pd in db.tblProductDetail
                               join pm in db.tblProductMaster on pd.ProductMasterID equals pm.ProductMasterID
                               where pd.ProductCode == r.ProductCode
                               && pm.ClientID == r.ClientID
                               select new { pd, pm }).FirstOrDefault();

                    if (qPD == null)
                    {
                        r.ImportError += " [ProductDetailID]";
                    }

                    if (qPD != null)
                    {
                        r.ProductDetailID = qPD.pd.ProductDetailID;
                        // note - there needs to be a SM record for size 1SR for each productdetailid that can be sampled in SR
                        var qSM = (from t in db.tblShelfMaster
                                   where t.ProductDetailID == r.ProductDetailID
                                   && t.Size == r.Size
                                   select t).FirstOrDefault();

                        if (qSM == null)
                        {
                            r.ImportError += " [ShelfID]";
                        }

                        if (qSM != null)
                        {
                            r.ShelfID = qSM.ShelfID;
                        }
                        else
                        {
                            var qSM_SR = (from t in db.tblShelfMaster
                                          where t.ProductDetailID == r.ProductDetailID
                                          && t.Size == "1SR"
                                          select t).FirstOrDefault();

                            if (qSM_SR != null)
                            {
                                r.ShelfID = qSM_SR.ShelfID;
                            }
                        }
                    }

                    // min requirements are clientid, productdetailid, shelfid
                    // ******* THIS WILL CAUSE ISSUES WITh PHOENIX ORDER EDIT IF SHELFID=null ********************************************
                    if (r.ClientID != null && r.ProductDetailID != null && r.ShelfID != null)
                    {
                        r.ImportStatus = "PASS";
                    }

                    db.SaveChanges();
                }

                // post adjust - if one item failed, the whole order fails
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

                // if ImportStatus=GOOD we can create VM objects as if it were data entry

                var qGUIDs = (from t in db.tblOrderImport
                              where t.ImportStatus == "PASS"
                              && t.Location_MDB == sLocation
                              select new { t.GUID }).ToList().Distinct();

                foreach (var r in qGUIDs)
                {
                    // just set a pointer to one of the rows so we can read the data
                    var qOM = (from t in db.tblOrderImport
                               where t.GUID == r.GUID
                               select t).FirstOrDefault();

                    Models.OrderMasterFull NewOrder = new Models.OrderMasterFull();

                    // fill OM fields
                    NewOrder.CreateUser = "Import";
                    NewOrder.CreateDate = DateTime.UtcNow;
                    NewOrder.orderdate = Convert.ToDateTime(qOM.OrderDate);
                    NewOrder.clientid = qOM.ClientID;
                    NewOrder.divisionid = qOM.DivisionID;
                    NewOrder.customer = qOM.Customer;
                    NewOrder.cmcorder = Convert.ToInt32(qOM.CMCOrder);
                    NewOrder.weborderid = Convert.ToInt32(qOM.WebOrderID);
                    NewOrder.cmclegacynumber = qOM.CMCLegacyNum;
                    NewOrder.custordnum = qOM.CustOrdNum;
                    NewOrder.custsapnum = qOM.CustSapNum;
                    NewOrder.custrefnum = qOM.CustRefNum;
                    if (qOM.OrderType == "w")
                    {
                        NewOrder.ordertype = "S";
                    }
                    else
                    {
                        NewOrder.ordertype = qOM.OrderType;
                    }
                    NewOrder.company = qOM.Company;
                    NewOrder.street = qOM.Street;
                    NewOrder.street2 = qOM.Street2;
                    NewOrder.street3 = qOM.Street3;
                    NewOrder.city = qOM.City;
                    NewOrder.state = qOM.State;
                    NewOrder.Zip = qOM.Zip;
                    NewOrder.country = qOM.Country;
                    NewOrder.attention = qOM.Attention;
                    NewOrder.email = qOM.Email;
                    NewOrder.salesrep = qOM.SalesRep;
                    NewOrder.sales_email = qOM.SalesEmail;
                    NewOrder.req = qOM.Req;
                    NewOrder.reqphone = qOM.ReqPhone;
                    NewOrder.reqfax = qOM.ReqFax;
                    NewOrder.reqemail = qOM.ReqEmail;
                    NewOrder.enduse = qOM.EndUse;
                    NewOrder.shipvia = qOM.ShipVia;
                    NewOrder.shipacct = qOM.ShipAcct;
                    NewOrder.phone = qOM.Phone;
                    if (qOM.Source == null)
                    {
                        NewOrder.source = "Web";
                    }
                    else
                    {
                        NewOrder.source = qOM.Source;
                    }
                    NewOrder.fax = qOM.Fax;
                    NewOrder.tracking = qOM.Tracking;
                    NewOrder.special = qOM.Special;
                    NewOrder.specialinternal = qOM.SpecialInternal;
                    NewOrder.lit = Convert.ToBoolean(qOM.Lit);
                    NewOrder.region = qOM.Region;
                    NewOrder.coa = Convert.ToBoolean(qOM.COA);
                    NewOrder.tds = Convert.ToBoolean(qOM.TDS);
                    NewOrder.cid = qOM.CID;
                    NewOrder.custacct = qOM.CustAcct;
                    NewOrder.acode = qOM.ACode;
                    NewOrder.importfile = qOM.ImportFile;
                    NewOrder.importdateline = qOM.ImportDateLine ?? null;
                    NewOrder.timing = qOM.Timing;
                    NewOrder.volume = qOM.Volume;
                    NewOrder.samplerack = Convert.ToBoolean(qOM.SampleRack);
                    NewOrder.cmcuser = qOM.CMCUser;
                    NewOrder.customerreference = qOM.CustomerReference;
                    NewOrder.totalorderweight = qOM.TotalOrderWeight;
                    NewOrder.custordertype = qOM.CustOrderType;
                    NewOrder.custrequestdate = qOM.CustRequestDate ?? null;
                    NewOrder.approvaldate = qOM.ApprovalDate ?? null;
                    NewOrder.requesteddeliverydate = qOM.RequestedDeliveryDate ?? null;
                    NewOrder.custtotalitems = Convert.ToInt32(qOM.CustTotalItems);
                    NewOrder.custrequestedcarrier = qOM.CustRequestedCarrier;
                    NewOrder.legacyid = Convert.ToInt32(qOM.LegacyID);
                    NewOrder.salesrepphone = qOM.SalesRepPhone;
                    NewOrder.salesrepterritory = qOM.SalesRepTerritory;
                    NewOrder.marketingrep = qOM.MarketingRep;
                    NewOrder.marketingrepemail = qOM.MarketingRepEmail;
                    NewOrder.distributor = qOM.Distributor;
                    NewOrder.preferredcarrier = qOM.PreferredCarrier;
                    NewOrder.approvalneeded = Convert.ToBoolean(qOM.ApprovalNeeded);
                    NewOrder.CreateUser = qOM.CreateUser;
                    NewOrder.CreateDate = qOM.CreateDate;
                    NewOrder.UpdateUser = qOM.UpdateUser;
                    NewOrder.UpdateDate = qOM.UpdateDate;
                    NewOrder.billinggroup = qOM.BillingGroup;
                    NewOrder.IsSDN = qOM.IsSDN;

                    // save tblOrderMaster to DB
                    NewOrder.orderid = -1;                                              // needed to do insert
                    int NewOrderID = Services.OrderService.fnSaveOrder(NewOrder);

                    // fill OM fields
                    var qOI = (from t in db.tblOrderImport
                               where t.GUID == r.GUID
                               select t).ToList();

                    foreach (var i in qOI)
                    {
                        Models.OrderItem NewItem = new Models.OrderItem();
                        NewItem.ItemID = -1;
                        NewItem.OrderID = NewOrderID;
                        NewItem.CreateDate = DateTime.UtcNow;
                        NewItem.CreateUser = "System";
                        NewItem.UpdateUser = HttpContext.Current.User.Identity.Name;
                        NewItem.ProductDetailID = i.ProductDetailID;
                        NewItem.ShelfID = i.ShelfID;
                        if (i.ShelfID == null)                                          // TODO: If shelf id is null; GetShelfid()
                        {
                            i.ShelfID = 0;                                              // needed for post save to pull SR

                            decimal srsize;                                             // get rid of this when tblOrderImport.SRSize is changed to decimal
                            try
                            {
                                srsize = decimal.Parse(i.SRSize);
                            }
                            catch (Exception)
                            {
                                srsize = 0.00M;
                            }

                            NewItem.SRSize = srsize;
                        }
                        NewItem.Qty = i.Qty;
                        NewItem.LotNumber = i.LotNumber;
                        NewItem.ShipDate = i.ShipDate;
                        NewItem.CSAllocate = i.CSAllocate;
                        NewItem.AllocateStatus = i.AllocateStatus;
                        NewItem.NonCMCDelay = i.NonCMCDelay;
                        NewItem.CarrierInvoiceRcvd = i.CarrierInvoiceRcvd;
                        NewItem.Status = i.Status;
                        NewItem.DelayReason = i.DelayReason;
                        NewItem.ItemNotes = i.ItemNotes;

                        int NewItemID = Services.OrderService.fnSaveItem(NewItem);      // save tblOrderItem to DB
                    }

                    OrdersImportedCount = OrdersImportedCount + 1;
                    string s = String.Format("UPDATE tblOrderImport SET ImportStatus='IMPORTED' WHERE GUID='{0}'", r.GUID);         // at this point we have an order

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
                tblInvLog newrec = new tblInvLog();

                newrec.LogType = vLogType;
                newrec.LogDate = vLogDate;
                newrec.StockID = vStockID;
                newrec.BulkID = vBulkID;
                newrec.LogQty = vLogQty;
                newrec.LogAmount = vLogAmount;
                newrec.CreateDate = vCreateDate;
                newrec.CreateUser = vCreateUser;
                newrec.UpdateDate = vUpdateDate;
                newrec.UpdateUser = vUpdateUser;

                db.tblInvLog.Add(newrec);

                db.SaveChanges();
            }
        }

        public static int ExecuteADOSQL(string sql)
        {
            using (var db = new CMCSQL03Entities())
            {
                db.Database.ExecuteSqlCommand(sql);
                return 1;
            }
        }
    }
}