using MvcPhoenix.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcPhoenix.Services
{
    public class OrderService
    {
        public static OrderMasterFull fnCreateOrder(int id)
        {
            // id=clientid
            using (var db = new EF.CMCSQL03Entities())
            {
                OrderMasterFull vm = new OrderMasterFull();
                vm.orderid = -1;
                vm.clientid = id;
                vm.ListOfDivisions = fnListOfDivisions(id);
                var cl = db.tblClient.Find(vm.clientid);
                vm.clientname = cl.ClientName;
                vm.orderstatus = "z";
                vm.orderdate = DateTime.UtcNow;
                vm.ListOfSalesReps = fnListOfSalesReps(id);
                vm.ListOfRequestors = fnListOfRequestors(id);
                vm.ListOfCountries = fnListOfCountries();
                vm.ListOfEndUses = fnListOfEndUses(id);
                vm.ListOfShipVias = fnListOfShipVias();
                vm.CreateUser = HttpContext.Current.User.Identity.Name;
                vm.CreateDate = DateTime.UtcNow;
                vm.IsSDN = false;

                return vm;
            }
        }

        public static OrderMasterFull fnFillOrder(int id)
        {
            using (var db = new EF.CMCSQL03Entities())
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

                o.ListOfSalesReps = fnListOfSalesReps(cl.ClientID);
                o.ListOfRequestors = fnListOfRequestors(cl.ClientID);
                o.ListOfCountries = fnListOfCountries();
                o.ListOfEndUses = fnListOfEndUses(cl.ClientID);
                o.ListOfShipVias = fnListOfShipVias();
                o.ListOfDivisions = fnListOfDivisions(cl.ClientID);

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
                //o.spstaxid = q.SPSTaxID;
                //o.spscurrency = q.SPSCurrency;
                //o.spsshippedwt = q.SPSShippedWt;
                //o.spsfreightcost = q.SPSFreightCost;
                //o.invoicecompany = q.InvoiceCompany;
                //o.invoicetitle = q.InvoiceTitle;
                //o.invoicefirstname = q.InvoiceFirstName;
                //o.invoicelastname = q.InvoiceLastName;
                //o.invoiceaddress1 = q.InvoiceAddress1;
                //o.invoiceaddress2 = q.InvoiceAddress2;
                //o.invoiceaddress3 = q.InvoiceAddress3;
                //o.invoicecity = q.InvoiceCity;
                //o.invoicestateprov = q.InvoiceStateProv;
                //o.invoicepostalcode = q.InvoicePostalCode;
                //o.invoicecountry = q.InvoiceCountry;
                //o.invoicephone = q.InvoicePhone;
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
            using (var db = new MvcPhoenix.EF.CMCSQL03Entities())
            {
                MvcPhoenix.EF.tblOrderMaster newrec = new MvcPhoenix.EF.tblOrderMaster();
                db.tblOrderMaster.Add(newrec);
                db.SaveChanges();

                return newrec.OrderID;
            }
        }

        public static int fnSaveOrder(OrderMasterFull vm)
        {
            fnArchiveOrderMaster(vm.orderid);

            // Take a ViewModel and Insert/Update DB / Return the PK
            using (var db = new MvcPhoenix.EF.CMCSQL03Entities())
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
                //q.SPSTaxID = vm.spstaxid;
                //q.SPSCurrency = vm.spscurrency;
                //q.SPSShippedWt = vm.spsshippedwt;
                //q.SPSFreightCost = vm.spsfreightcost;
                //q.InvoiceCompany = vm.invoicecompany;
                //q.InvoiceTitle = vm.invoicetitle;
                //q.InvoiceFirstName = vm.invoicefirstname;
                //q.InvoiceLastName = vm.invoicelastname;
                //q.InvoiceAddress1 = vm.invoiceaddress1;
                //q.InvoiceAddress2 = vm.invoiceaddress2;
                //q.InvoiceAddress3 = vm.invoiceaddress3;
                //q.InvoiceCity = vm.invoicecity;
                //q.InvoiceStateProv = vm.invoicestateprov;
                //q.InvoicePostalCode = vm.invoicepostalcode;
                //q.InvoiceCountry = vm.invoicecountry;
                //q.InvoicePhone = vm.invoicephone;
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
            using (var db = new EF.CMCSQL03Entities())
            {
                bool ShowAlert = false;

                var q = (from t in db.tblOrderMaster
                         where t.OrderID == vm.orderid
                         select t).FirstOrDefault();

                var qCountry = (from t in db.tblCountry
                                where t.Country == vm.country && t.DoNotShip == true
                                select t).FirstOrDefault();

                if (qCountry != null)
                {
                    // flag the order record and the item records that are yet to be allocated
                    q.IsSDN = true;
                    var orderitems = (from t in db.tblOrderItem
                                      where t.OrderID == vm.orderid && t.AllocateStatus == null
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
                                      where t.OrderID == vm.orderid && t.AllocateStatus == null
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
                                            where t.ClientID == vm.clientid && t.CountryID == qCountry2.CountryID
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

        public static OrderSPSBilling fnSPSBilling(int id)
        {
            // id=OrderID
            using (var db = new EF.CMCSQL03Entities())
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
            using (var db = new EF.CMCSQL03Entities())
            {
                var SumSPSCharge = db.tblOrderItem
                    .Where(t => t.OrderID == OrderId)
                    .Sum(t => t.SPSCharge);

                return SumSPSCharge;
            }
        }

        public static void fnSaveSPSBillingDetails(OrderSPSBilling vm)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                if (vm.SPSBillingID == -1)
                {
                    EF.tblOrderSPSBilling newrec = new EF.tblOrderSPSBilling();
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

        public static OrderItem fnCreateItem(int id)
        {
            // id=OrderID
            using (var db = new EF.CMCSQL03Entities())
            {
                OrderItem vm = new OrderItem();
                vm.CrudMode = "RW";
                vm.ItemID = -1;
                vm.OrderID = id;
                var dbOrder = db.tblOrderMaster.Find(id);
                vm.ClientID = dbOrder.ClientID;
                vm.CreateDate = DateTime.UtcNow;
                vm.CreateUser = HttpContext.Current.User.Identity.Name;
                vm.ProductDetailID = -1;
                vm.ListOfProductDetailIDs = fnListOfProductCodes(vm.ClientID);
                vm.ProductCode = null;
                vm.ProductName = null;
                vm.ShelfID = -1;
                vm.Size = null;
                vm.SRSize = null;
                vm.Qty = 1;
                vm.LotNumber = null;
                vm.ShipDate = null;
                vm.ItemShipVia = "";
                vm.ListOfShipVias = fnListOfShipVias();
                vm.CSAllocate = true;
                vm.AllocateStatus = null;
                vm.NonCMCDelay = false;
                vm.CarrierInvoiceRcvd = false;
                vm.DelayReason = null;
                vm.StatusID = -1;
                vm.ListOfStatusNotesIDs = fnListOfStatusNotesIDs();
                vm.AlertNotesShipping = "<< AlertNotesShipping >>";
                vm.AlertNotesPackOut = "<< AlertNotesPackOut >>";
                vm.AlertNotesOrderEntry = "<< AlertNotesOrderEntry >>";
                vm.AlertNotesOther = "<< AlertNotesOther >>";

                return vm;
            }
        }

        public static int fnNewItemID()
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                EF.tblOrderItem newrec = new EF.tblOrderItem();
                db.tblOrderItem.Add(newrec);
                db.SaveChanges();

                return newrec.ItemID;
            }
        }

        public static OrderItem fnFillOrderItem(int id)
        {
            // id=itemid
            // Fill all the fields from the tblOrderItem record
            using (var db = new EF.CMCSQL03Entities())
            {
                OrderItem vm = new OrderItem();
                var q = (from t in db.tblOrderItem
                         where t.ItemID == id
                         select t).FirstOrDefault();

                var cl = db.tblOrderMaster.Find(q.OrderID);

                vm.CrudMode = "RW";    // default value

                // Hidden fields to persist for postback
                vm.ItemID = q.ItemID;
                vm.OrderID = q.OrderID;
                vm.ClientID = Convert.ToInt32(cl.ClientID);
                vm.CreateDate = q.CreateDate;
                vm.CreateUser = q.CreateUser;
                vm.UpdateDate = q.UpdateDate;
                vm.UpdateUser = q.UpdateUser;
                vm.ProductDetailID = q.ProductDetailID;
                vm.ListOfProductDetailIDs = fnListOfProductCodes(vm.ClientID);
                vm.ProductCode = q.ProductCode;
                vm.ProductName = q.ProductName;
                vm.ShelfID = q.ShelfID;
                vm.ListOfShelfIDs = fnProductCodeSizes(Convert.ToInt32(vm.ProductDetailID));
                vm.Size = q.Size;
                vm.SRSize = q.SRSize;
                vm.Qty = q.Qty;
                vm.LotNumber = q.LotNumber;
                vm.ItemShipVia = q.Via;
                vm.ListOfShipVias = fnListOfShipVias();
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
                vm.ListOfStatusNotesIDs = fnListOfStatusNotesIDs();
                vm.ItemNotes = q.ItemNotes;
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
            fnArchiveOrderItem(vm.ItemID);

            using (var db = new EF.CMCSQL03Entities())
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

                q.OrderID = vm.OrderID;
                q.CreateDate = vm.CreateDate;
                q.CreateUser = vm.CreateUser;
                q.UpdateDate = DateTime.UtcNow;
                q.UpdateUser = HttpContext.Current.User.Identity.Name;
                q.ProductDetailID = vm.ProductDetailID;
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

                db.SaveChanges();

                fnSaveItemPostUpdate(vm);                       // update the row from other tables
                fnGenerateOrderTransactions(vm.ItemID);         // Go do the Order Trans work....

                return vm.ItemID;
            }
        }

        public static void fnSaveItemPostUpdate(OrderItem vm)
        {
            // pull values for ProductCode, ProductName, Size
            using (var db = new EF.CMCSQL03Entities())
            {
                var q = (from t in db.tblOrderItem
                         where t.ItemID == vm.ItemID
                         select t).FirstOrDefault();

                var dbPD = (from t in db.tblProductDetail
                            where t.ProductDetailID == vm.ProductDetailID
                            select t).FirstOrDefault();

                q.ProductCode = dbPD.ProductCode;
                q.ProductName = dbPD.ProductName;

                if (vm.ShelfID == 9999 || vm.SRSize != null || vm.SRSize != 0)
                {
                    q.Size = "1SR";
                }
                else
                {
                    var dbSM = (from t in db.tblShelfMaster
                                where t.ShelfID == vm.ShelfID
                                select t).FirstOrDefault();

                    if (dbSM != null)
                    {
                        q.Size = dbSM.Size;
                        q.Weight = dbSM.UnitWeight;
                    }
                }

                q.AlertNotesShipping = dbPD.AlertNotesShipping;

                var dbPM = db.tblProductMaster.Find(dbPD.ProductMasterID);

                q.AlertNotesPackout = dbPM.AlertNotesPackout;

                q.AlertNotesOrderEntry = dbPD.AlertNotesOrderEntry;  // comes from profiles

                if (dbPD.AIRUNNUMBER == "UN3082" | dbPD.AIRUNNUMBER == "UN3077" | dbPD.GRNUNNUMBER == "UN3082" | dbPD.GRNUNNUMBER == "UN3077")
                {
                    q.AlertNotesOther = "Products with UN3082 and UN3077 may be shipped as non hazardous if under 5 kg";
                }

                db.SaveChanges();
            }
        }

        public static void fnDeleteOrderItem(int id)
        {
            System.Threading.Thread.Sleep(1000);
            using (var db = new EF.CMCSQL03Entities())
            {
                var qry = (from t in db.tblOrderItem
                           where t.ItemID == id
                           select t).FirstOrDefault();

                if (qry != null)
                {
                    string s = @"DELETE FROM tblOrderItem WHERE ItemID=" + id.ToString();
                    db.Database.ExecuteSqlCommand(s);

                    // TODO do we really want to delete all the transactons?
                    s = @"DELETE FROM tblOrderTrans WHERE OrderItemID=" + id.ToString();
                    db.Database.ExecuteSqlCommand(s);
                }
            }
        }

        public static int fnAllocateBulk(int OrderID, bool IncludeQCStock)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                int AllocationCount = 0;
                var orderitems = (from t in db.tblOrderItem
                                  where t.OrderID == OrderID && t.ShipDate == null && t.AllocateStatus == null && t.CSAllocate == true
                                  select t).ToList();

                if (orderitems == null)
                {
                    return AllocationCount;
                }

                foreach (var item in orderitems)
                {
                    var tblbulk = (from t in db.tblBulk
                                   join pm in db.tblProductMaster on t.ProductMasterID equals pm.ProductMasterID
                                   join pd in db.tblProductDetail on pm.ProductMasterID equals pd.ProductMasterID
                                   where (pd.ProductDetailID == item.ProductDetailID)
                                   select new
                                   {
                                       BulkID = t.BulkID,
                                       Warehouse = t.Warehouse,
                                       Qty = t.Qty,
                                       LotNumber = t.LotNumber,
                                       CurrentWeight = t.CurrentWeight,
                                       CeaseShipDate = t.CeaseShipDate,
                                       BulkStatus = t.BulkStatus,
                                       Bin = t.Bin
                                   }).ToList();

                    var qOffset = db.tblOrderMaster.Find(OrderID);
                    int iOffset = Convert.ToInt32(qOffset.CeaseShipOffset);
                    if (iOffset != 0)
                    {
                        // if there's an offset on the order record, add that to today to build in extra time
                        DateTime dCease = DateTime.UtcNow.AddDays(iOffset);

                        tblbulk = (from t in tblbulk
                                   where t.CeaseShipDate > dCease
                                   select t).ToList();
                    }

                    if (IncludeQCStock == true)
                    {
                        tblbulk = (from t in tblbulk
                                   where t.BulkStatus == "AVAIL" || t.BulkStatus == "QC"
                                   select t).ToList();
                    }
                    else
                    {
                        tblbulk = (from t in tblbulk
                                   where t.BulkStatus == "AVAIL"
                                   select t).ToList();
                    }

                    tblbulk = (from t in tblbulk
                               where t.CurrentWeight >= (item.Qty * item.Weight)
                               select t).ToList();

                    tblbulk = (from t in tblbulk
                               orderby t.CeaseShipDate ascending
                               select t).ToList();

                    foreach (var row in tblbulk)
                    {
                        // update tblstock record (need to use separate qry)
                        if (row.CurrentWeight >= item.Qty * item.Weight)
                        {
                            AllocationCount = AllocationCount + 1;
                            var q = db.tblBulk.Find(row.BulkID);

                            q.CurrentWeight = q.CurrentWeight - (item.Qty * item.Weight);
                            item.AllocatedBulkID = row.BulkID;
                            item.Warehouse = row.Warehouse;
                            item.LotNumber = row.LotNumber;
                            item.AllocateStatus = "A";
                            item.Bin = row.Bin;
                            item.ExpirationDate = q.ExpirationDate;
                            item.CeaseShipDate = q.CeaseShipDate;

                            fnInsertLogRecord("BS-ALC", DateTime.UtcNow, null, row.BulkID, item.Qty, item.Weight, DateTime.UtcNow, HttpContext.Current.User.Identity.Name, null, null);

                            db.SaveChanges();

                            break;
                        }
                    }
                }

                return AllocationCount;
            }
        }

        public static int fnAllocateShelf(int OrderID, bool IncludeQCStock)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                int AllocationCount = 0;

                var items = (from t in db.tblOrderItem
                             join om in db.tblOrderMaster on t.OrderID equals om.OrderID
                             where t.OrderID == OrderID && t.ShipDate == null && t.AllocateStatus == null && t.CSAllocate == true
                             select t).ToList();

                if (items == null)
                {
                    return AllocationCount;
                }

                foreach (var item in items)
                {
                    var tblstock = (from t in db.tblStock
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
                                        }
                                    ).ToList();

                    var qOffset = db.tblOrderMaster.Find(items[0].OrderID);
                    int iOffset = Convert.ToInt32(qOffset.CeaseShipOffset);

                    // change to take negative numbers
                    if (iOffset != 0)
                    {
                        // if there's an offset on the order record, add that to today to build in extra time
                        DateTime dCease = DateTime.UtcNow.AddDays(iOffset);
                        tblstock = (from t in tblstock
                                    where t.CeaseShipDate > dCease
                                    select t).ToList();
                    }

                    if (item.LotNumber != null) // specific lot number requested
                    {
                        tblstock = (from t in tblstock
                                    where t.LotNumber == item.LotNumber
                                    select t).ToList();
                    }

                    if (IncludeQCStock == true)
                    {
                        tblstock = (from t in tblstock
                                    where t.ShelfStatus == "AVAIL" || t.ShelfStatus == "QC"
                                    select t).ToList();
                    }
                    else
                    {
                        tblstock = (from t in tblstock
                                    where t.ShelfStatus == "AVAIL"
                                    select t).ToList();
                    }

                    // Do something with the Profiles alert message (add to tblOrderItem on creation?
                    // Do something with the Special Transport Provision Alert - add to tblOrderitem on creation?

                    tblstock = (from t in tblstock
                                orderby t.CeaseShipDate ascending
                                select t).ToList();

                    // Page thru tblstock rows looking for the first record that has enough qty then bailout
                    foreach (var row in tblstock)
                    {
                        if (row.QtyOnHand - row.QtyAllocated >= item.Qty)
                        {
                            AllocationCount = AllocationCount + 1;
                            // update tblstock record (need to use separate qry)
                            var q = db.tblStock.Find(row.StockID);
                            q.QtyAllocated = (q.QtyAllocated ?? 0) + item.Qty;

                            // update tblorderitem record
                            item.AllocatedStockID = row.StockID;
                            item.Warehouse = row.Warehouse;
                            item.LotNumber = row.LotNumber;
                            item.AllocateStatus = "A";
                            item.Bin = row.Bin;
                            item.ExpirationDate = row.ExpirationDate;
                            item.CeaseShipDate = row.CeaseShipDate;
                            item.AllocatedDate = DateTime.UtcNow;

                            db.SaveChanges();

                            break;

                            fnInsertLogRecord("SS-ALC", DateTime.UtcNow, row.StockID, null, item.Qty, null, DateTime.UtcNow, HttpContext.Current.User.Identity.Name, null, null);
                        }
                    }
                }

                return AllocationCount;
            }
        }

        public static void fnInsertLogRecord(string vLogType, DateTime? vLogDate, int? vStockID, int? vBulkID, int? vLogQty, decimal? vLogAmount, DateTime? vCreateDate, string vCreateUser, DateTime? vUpdateDate, string vUpdateUser)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                EF.tblInvLog newrec = new EF.tblInvLog();
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

        public static OrderTrans fnCreateTrans(int id)
        {
            // id=orderid
            using (var db = new EF.CMCSQL03Entities())
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
            using (var db = new EF.CMCSQL03Entities())
            {
                if (vm.ordertransid == -1)
                {
                    EF.tblOrderTrans newrec = new EF.tblOrderTrans();
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
                d.TransAmount = vm.transamount;
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
            OrderTrans vm = new OrderTrans();

            using (var db = new EF.CMCSQL03Entities())
            {
                var qry = (from t in db.tblOrderTrans
                           where t.OrderTransID == id
                           select t).FirstOrDefault();

                vm.ordertransid = qry.OrderTransID;
                vm.orderid = qry.OrderID;
                vm.orderitemid = qry.OrderItemID;
                vm.clientid = qry.ClientID;
                vm.transdate = qry.TransDate;
                vm.transtype = qry.TransType;
                vm.transqty = qry.TransQty;
                vm.transamount = qry.TransAmount;
                vm.comments = qry.Comments;
                vm.createdate = qry.CreateDate;
                vm.createuser = qry.CreateUser;
                vm.updatedate = qry.UpdateDate;
                vm.updateuser = qry.UpdateUser;

                return vm;
            }
        }

        public static void fnDeleteTrans(int id)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                string s = @"DELETE FROM tblOrderTrans WHERE OrderTransID=" + id.ToString();
                db.Database.ExecuteSqlCommand(s);
            }
        }

        public static void fnGenerateOrderTransactions(int id)
        {
            using (var db = new EF.CMCSQL03Entities())
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
                                where t.ClientID == o.ClientID && t.Size == oi.Size && t.TierLevel == 1
                                select t).FirstOrDefault();

                if (tierSize != null)
                {
                    EF.tblOrderTrans newrec = new EF.tblOrderTrans();
                    newrec.TransDate = DateTime.UtcNow;
                    newrec.OrderItemID = oi.ItemID;
                    newrec.OrderID = oi.OrderID;
                    newrec.ClientID = o.ClientID;
                    newrec.BillingGroup = o.BillingGroup;
                    newrec.TransType = "SAMP";
                    newrec.TransQty = oi.Qty;
                    newrec.TransAmount = tierSize.Price;
                    newrec.CreateDate = DateTime.UtcNow;
                    newrec.CreateUser = "System";
                    newrec.UpdateDate = DateTime.UtcNow;
                    newrec.UpdateUser = "System";

                    db.tblOrderTrans.Add(newrec);
                    db.SaveChanges();
                }
                else
                {
                    // Assume this is a SR size ??????????????????????
                    var tierSpecialRequest = (from t in db.tblTier
                                              where t.ClientID == o.ClientID && t.Size == "1SR" && t.TierLevel == 1
                                              select t).FirstOrDefault();

                    if (tierSpecialRequest != null)
                    {
                        EF.tblOrderTrans newrec = new EF.tblOrderTrans();
                        newrec.TransDate = DateTime.UtcNow;
                        newrec.OrderItemID = oi.ItemID;
                        newrec.OrderID = oi.OrderID;
                        newrec.ClientID = o.ClientID;
                        newrec.BillingGroup = o.BillingGroup;
                        newrec.TransType = "SAMP";
                        newrec.TransQty = oi.Qty;
                        newrec.TransAmount = tierSpecialRequest.Price;
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
                              where t.ProductMasterID == bl.ProductMasterID && t.ProductCode == pm.MasterCode
                              select t).FirstOrDefault();

                    sm = (from t in db.tblShelfMaster
                          where t.ProductDetailID == pd.ProductDetailID && t.Size == oi.Size
                          select t).FirstOrDefault();

                    if (sm == null)
                    {
                        // go no further
                        return;
                    }
                    // this should rest me on a shelfmaster that is really the bulk size sample
                }

                var qrySurcharges = (from t in db.tblSurcharge
                                     where t.ClientID == o.ClientID
                                     select t).FirstOrDefault();

                if (qrySurcharges != null)
                {
                    if (sm.HazardSurcharge == true)
                    {
                        fnInsertOrderTrans(oi.ItemID, "HAZD", qrySurcharges.Haz);
                    }

                    if (sm.FlammableSurcharge == true)
                    {
                        fnInsertOrderTrans(oi.ItemID, "FLAM", qrySurcharges.Flam);
                    }

                    if (sm.HeatSurcharge == true)
                    {
                        fnInsertOrderTrans(oi.ItemID, "HEAT", qrySurcharges.Heat);
                    }

                    if (sm.RefrigSurcharge == true)
                    {
                        fnInsertOrderTrans(oi.ItemID, "REFR", qrySurcharges.Refrig);
                    }

                    if (sm.FreezerSurcharge == true)
                    {
                        fnInsertOrderTrans(oi.ItemID, "FREZ", qrySurcharges.Freezer);
                    }

                    if (sm.CleanSurcharge == true)
                    {
                        fnInsertOrderTrans(oi.ItemID, "CLEN", qrySurcharges.Clean);
                    }

                    if (sm.BlendSurcharge == true)
                    {
                        fnInsertOrderTrans(oi.ItemID, "BLND", 0);
                    }

                    if (sm.NalgeneSurcharge == true)
                    {
                        fnInsertOrderTrans(oi.ItemID, "NALG", qrySurcharges.Nalgene);
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
                        fnInsertOrderTrans(oi.ItemID, "LABL", qrySurcharges.LabelFee);
                    }

                    if (sm.OtherSurcharge == true)
                    {
                        fnInsertOrderTrans(oi.ItemID, "OTHR", sm.OtherSurchargeAmt);
                    }
                }
            }
        }

        public static void fnInsertOrderTrans(int? ItemID, string TransType, decimal? TransAmount)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                string s = String.Format("DELETE FROM tblOrderTrans WHERE OrderItemID={0} AND Transtype = '{1}' AND CreateUser='System'", ItemID, TransType);

                db.Database.ExecuteSqlCommand(s);

                var oi = (from t in db.tblOrderItem
                          where t.ItemID == ItemID
                          select t).FirstOrDefault();

                var o = (from t in db.tblOrderMaster
                         where t.OrderID == oi.OrderID
                         select t).FirstOrDefault();

                EF.tblOrderTrans newrec = new EF.tblOrderTrans();
                newrec.TransDate = DateTime.UtcNow;
                newrec.OrderItemID = ItemID;
                newrec.OrderID = oi.OrderID;
                newrec.ClientID = o.ClientID;
                newrec.BillingGroup = o.BillingGroup;
                newrec.TransDate = DateTime.UtcNow;
                newrec.TransType = TransType;
                newrec.TransQty = oi.Qty;
                newrec.TransAmount = TransAmount;
                newrec.CreateDate = DateTime.UtcNow;
                newrec.CreateUser = "System";
                newrec.UpdateDate = DateTime.UtcNow;
                newrec.UpdateUser = "System";

                db.tblOrderTrans.Add(newrec);
                db.SaveChanges();
            }
        }

        public static void fnArchiveOrderMaster(int id)
        {
            // stash a copy before update
        }

        public static void fnArchiveOrderItem(int id)
        {
            // stash a copy before update
        }

        // ****************************************************
        // Service utility methods
        // ****************************************************

        public static int ExecuteADOSQL(string sql)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                db.Database.ExecuteSqlCommand(sql);
                return 1;
            }
        }

        #region Selection List Methods

        public static List<OrderMasterFull> fnOrdersSearchResults()
        {
            // default query join for the index_partial ORDERS search results, also used by all the search requests as the starting point
            using (var db = new EF.CMCSQL03Entities())
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                List<OrderMasterFull> orderslist = new List<OrderMasterFull>();
                orderslist = (from t in db.tblOrderMaster
                              join clt in db.tblClient on t.ClientID equals clt.ClientID
                              let count = (from items in db.tblOrderItem
                                           where items.OrderID == t.OrderID
                                           select items).Count()

                              let allocationcount = (from i in db.tblOrderItem
                                                     where i.OrderID == t.OrderID && i.ShipDate == null && i.Qty > 0 && String.IsNullOrEmpty(i.AllocateStatus) && i.ProductDetailID != null && i.ShelfID == null
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

        public static string fnBuildSizeDropDown(int id)
        {
            // id=productdetailid
            using (var db = new EF.CMCSQL03Entities())
            {
                var qry = (from t in db.tblShelfMaster
                           where t.ProductDetailID == id
                           orderby t.Size
                           select t);

                string s = "<option value='0' selected=true>Select Size</option>";

                if (qry.Count() > 0)
                {
                    foreach (var item in qry)
                    {
                        s = s + "<option value=" + item.ShelfID.ToString() + ">" + item.Size + " - " + item.UnitWeight + "</option>";
                    }
                }
                else
                {
                    s = s + "<option value='0'>No Sizes Found</option>";
                }

                s = s + "<option value='9999'>1SR</option>";

                return s;
            }
        }

        public static List<SelectListItem> fnListOfOrderItemIDs(int? id)
        {
            using (var db = new MvcPhoenix.EF.CMCSQL03Entities())
            {
                List<SelectListItem> mylist = new List<SelectListItem>();
                mylist = (from t in db.tblOrderItem
                          where t.OrderID == id
                          orderby t.ProductCode
                          select new SelectListItem { Value = t.ItemID.ToString(), Text = t.ProductCode }).ToList();
                mylist.Insert(0, new SelectListItem { Value = "0", Text = "None" });
                return mylist;
            }
        }

        public static List<SelectListItem> fnListOfDelayReasons()
        {
            List<SelectListItem> mylist = new List<SelectListItem>();

            mylist.Insert(0, new SelectListItem { Value = "", Text = "Select Delay Reason" });
            mylist.Insert(1, new SelectListItem { Value = "Backorder", Text = "Backorder" });
            mylist.Insert(2, new SelectListItem { Value = "Oven Product", Text = "Oven Product" });
            mylist.Insert(3, new SelectListItem { Value = "Humidity", Text = "Humidity" });
            mylist.Insert(4, new SelectListItem { Value = "Very Large Order", Text = "Very Large Order" });
            mylist.Insert(5, new SelectListItem { Value = "Questions/Info Needed", Text = "Questions/Info Needed" });
            mylist.Insert(6, new SelectListItem { Value = "Special Doc Required", Text = "Special Doc Required" });
            mylist.Insert(7, new SelectListItem { Value = "Special Request Size", Text = "Special Request Size" });
            mylist.Insert(8, new SelectListItem { Value = "Return Order", Text = "Return Order" });
            mylist.Insert(9, new SelectListItem { Value = "Waste", Text = "Waste" });
            mylist.Insert(10, new SelectListItem { Value = "No partial delivery", Text = "No partial delivery" });
            mylist.Insert(11, new SelectListItem { Value = "Special delivery date", Text = "Special delivery date" });
            mylist.Insert(12, new SelectListItem { Value = "Public holiday", Text = "Public holiday" });
            mylist.Insert(13, new SelectListItem { Value = "Freezable Procedure", Text = "Freezable Procedure" });
            mylist.Insert(14, new SelectListItem { Value = "CMC delay, customer informed", Text = "CMC delay, customer informed" });
            mylist.Insert(15, new SelectListItem { Value = "R&D flow", Text = "R&D flow" });
            mylist.Insert(16, new SelectListItem { Value = "Special procedure", Text = "Special procedure" });
            mylist.Insert(17, new SelectListItem { Value = "Misc. Charges", Text = "Misc. Charges" });
            mylist.Insert(18, new SelectListItem { Value = "Transfer order", Text = "Transfer order" });
            mylist.Insert(19, new SelectListItem { Value = "Consolidated Order", Text = "Consolidated Order" });
            mylist.Insert(20, new SelectListItem { Value = "Approval Required", Text = "Approval Required" });
            mylist.Insert(21, new SelectListItem { Value = "Conditioned packaging", Text = "Conditioned packaging" });
            mylist.Insert(22, new SelectListItem { Value = "Product Not Setup", Text = "Product Not Setup" });
            mylist.Insert(23, new SelectListItem { Value = "Frt Fwd-Pending Arrangement", Text = "Frt Fwd-Pending Arrangement" });
            mylist.Insert(24, new SelectListItem { Value = "Labels, SDS", Text = "Labels, SDS" });

            return mylist;
        }

        public static List<SelectListItem> fnListOfClientIDs()
        {
            using (var db = new EF.CMCSQL03Entities())
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

        public static List<SelectListItem> fnListOfDivisions(int? id)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                List<SelectListItem> mylist = new List<SelectListItem>();

                mylist = (from t in db.tblDivision
                          where t.ClientID == id
                          orderby t.DivisionName
                          select new SelectListItem
                          {
                              Value = t.DivisionID.ToString(),
                              Text = t.DivisionName + " / " + t.BusinessUnit
                          }).Distinct().ToList();

                mylist.Insert(0, new SelectListItem { Value = "", Text = "" });

                return mylist;
            }
        }

        public static List<OrderItem> fnListOfOrderItemsForOrderID(int OrderID)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                List<OrderItem> mylist = new List<OrderItem>();

                var qry = (from t in db.tblOrderItem
                           where t.OrderID == OrderID
                           select t).ToList();

                return mylist;
            }
        }

        public static List<SelectListItem> fnProductCodeSizes(int id)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                List<SelectListItem> mylist = new List<SelectListItem>();

                mylist = (from t in db.tblShelfMaster
                          where t.ProductDetailID == id
                          select new SelectListItem
                          {
                              Value = t.ShelfID.ToString(),
                              Text = t.Size + " - " + t.UnitWeight
                          }).ToList();

                mylist.Insert(0, new SelectListItem { Value = "0", Text = " -- Select Size --" });
                mylist.Add(new SelectListItem { Value = "9999", Text = "1SR" });

                return mylist;
            }
        }

        public static List<SelectListItem> fnProductCodeXref(int? id)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                List<SelectListItem> mylist = new List<SelectListItem>();

                mylist = (from t in db.tblProductXRef
                          where t.ClientID == id
                          orderby t.CustProductCode
                          select new SelectListItem
                          {
                              Value = t.CustProductCode,
                              Text = t.CustProductCode + " : " + t.CMCProductCode
                          }).ToList();

                mylist.Insert(0, new SelectListItem { Value = "0", Text = "Client Product Code" });

                return mylist;
            }
        }

        public static List<SelectListItem> fnListOfOrderIDs()
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                List<SelectListItem> mylist = new List<SelectListItem>();

                mylist = (from t in db.tblOrderMaster
                          orderby t.OrderID descending
                          select new SelectListItem
                          {
                              Value = t.OrderID.ToString(),
                              Text = t.OrderID.ToString()
                          }).ToList();

                mylist.Insert(0, new SelectListItem { Value = "0", Text = "Please Select" });

                return mylist;
            }
        }

        public static List<SelectListItem> fnListOfCountries()
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                List<SelectListItem> mylist = new List<SelectListItem>();

                mylist = (from t in db.tblCountry
                          orderby t.Country
                          select new SelectListItem
                          {
                              Value = t.Country,
                              Text = t.Country
                          }).ToList();

                mylist.Insert(0, new SelectListItem { Value = "0", Text = "Please Select" });

                return mylist;
            }
        }

        public static List<SelectListItem> fnListOfSalesReps(int? id)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                List<SelectListItem> mylist = new List<SelectListItem>();

                mylist = (from t in db.tblClientContact
                          where t.ClientID == id
                          where t.ContactType == "SalesRep"
                          orderby t.FullName
                          select new SelectListItem
                          {
                              Value = t.ClientContactID.ToString(),
                              Text = t.FullName
                          }).ToList();

                mylist.Insert(0, new SelectListItem { Value = "0", Text = "Please Select" });

                return mylist;
            }
        }

        public static List<SelectListItem> fnListOfRequestors(int id)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                List<SelectListItem> mylist = new List<SelectListItem>();

                mylist = (from t in db.tblClientContact
                          where t.ClientID == id
                          where t.ContactType == "Requestor"
                          orderby t.FullName
                          select new SelectListItem
                          {
                              Value = t.ClientContactID.ToString(),
                              Text = t.FullName
                          }).ToList();

                mylist.Insert(0, new SelectListItem { Value = "0", Text = "Please Select" });

                return mylist;
            }
        }

        public static Contact fnGetClientContacts(int id)
        {
            // fill a json object for View use
            using (var db = new EF.CMCSQL03Entities())
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

        public static List<SelectListItem> fnListOfEndUses(int? id)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                List<SelectListItem> mylist = new List<SelectListItem>();

                mylist = (from t in db.tblEndUse
                          where t.ClientID == id
                          orderby t.EndUse
                          select new SelectListItem
                          {
                              Value = t.EndUse,
                              Text = t.EndUse
                          }).ToList();

                mylist.Insert(0, new SelectListItem { Value = "0", Text = "Please Select" });

                return mylist;
            }
        }

        public static List<SelectListItem> fnListOfShipVias()
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                List<SelectListItem> mylist = new List<SelectListItem>();

                mylist = (from t in db.tblCarrier
                          orderby t.CarrierName
                          select new SelectListItem
                          {
                              Value = t.CarrierName,
                              Text = t.CarrierName
                          }).ToList();

                mylist.Insert(0, new SelectListItem { Value = "0", Text = "Please Select" });

                return mylist;
            }
        }

        public static List<SelectListItem> fnListOfOrderSources()
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                List<SelectListItem> mylist = new List<SelectListItem>();

                mylist = (from t in db.tblOrderSource
                          orderby t.Source
                          select new SelectListItem
                          {
                              Value = t.Source,
                              Text = t.Source
                          }).ToList();

                mylist.Insert(0, new SelectListItem { Value = "0", Text = "Please Select" });

                return mylist;
            }
        }

        public static List<SelectListItem> fnListOfProductCodes(int? id)
        {
            List<SelectListItem> mylist = new List<SelectListItem>();

            using (var db = new EF.CMCSQL03Entities())
            {
                mylist = (from t in db.tblProductDetail
                          join m in db.tblProductMaster on t.ProductMasterID equals m.ProductMasterID
                          where m.ClientID == id
                          orderby t.ProductCode
                          select new SelectListItem
                          {
                              Value = t.ProductDetailID.ToString(),
                              Text = t.ProductCode + " " + t.ProductName
                          }).ToList();

                mylist.Insert(0, new SelectListItem { Value = "0", Text = "Please Select" });

                return mylist;
            }
        }

        public static List<SelectListItem> fnListOfShipViasItemLevel()
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                List<SelectListItem> mylist = new List<SelectListItem>();

                mylist = (from t in db.tblCarrier
                          orderby t.CarrierName
                          select new SelectListItem
                          {
                              Value = t.CarrierName,
                              Text = t.CarrierName
                          }).ToList();

                mylist.Insert(0, new SelectListItem { Value = "0", Text = "Please Select" });

                return mylist;
            }
        }

        public static List<SelectListItem> fnListOfStatuses()
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                List<SelectListItem> mylist = new List<SelectListItem>();

                mylist.Add(new SelectListItem { Value = "S1", Text = "S1" });
                mylist.Add(new SelectListItem { Value = "S2", Text = "S2" });
                mylist.Add(new SelectListItem { Value = "S3", Text = "S3" });

                mylist.Insert(0, new SelectListItem { Value = "0", Text = "Please Select" });

                return mylist;
            }
        }

        public static List<SelectListItem> fnListOfStatusNotesIDs()
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                List<SelectListItem> mylist = new List<SelectListItem>();

                mylist = (from t in db.tblStatusNotes
                          orderby t.Note
                          select new SelectListItem
                          {
                              Value = t.Note,
                              Text = t.Note
                          }).ToList();

                mylist.Insert(0, new SelectListItem { Value = "0", Text = "Please Select" });

                return mylist;
            }
        }

        #endregion Selection List Methods
    }
}