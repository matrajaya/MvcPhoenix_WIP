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
        public static List<OrderMasterFull> fnOrdersSearchResults()
        {
            // default query join for the index_partial ORDERS search results, also used by all the search requests as the starting point
            using (var db = new EF.CMCSQL03Entities())
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                List<OrderMasterFull> orderslist = new List<OrderMasterFull>();
                orderslist = (from t in db.tblOrderMaster
                              join clt in db.tblClient on t.ClientID equals clt.ClientID
                              let count = (from items in db.tblOrderItem where items.OrderID == t.OrderID select items).Count()
                              let allocationcount = (from i in db.tblOrderItem where i.OrderID == t.OrderID && i.ShipDate == null && i.Qty > 0 && String.IsNullOrEmpty(i.AllocateStatus) && i.ProductDetailID != null && i.ShelfID == null select i).Count()
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

        public static OrderMasterFull fnCreateOrder(int id)
        {
            // id=clientid
            using (var db = new EF.CMCSQL03Entities())
            {
                // populate the minimum fields needed for the View
                OrderMasterFull vm = new OrderMasterFull();
                vm.orderid = -1;
                vm.clientid = id;
                var cl = db.tblClient.Find(vm.clientid);
                vm.clientname = cl.ClientName;
                vm.logofilename = cl.LogoFileName;
                vm.orderstatus = "z";
                vm.orderdate = System.DateTime.Now;
                //vm.ListOfDivisions = fnListOfDivisions(id);
                //vm.ListOfOrderTypes = fnListOfOrderTypes();
                vm.ListOfSalesReps = fnListOfSalesReps(id);
                //vm.ListOfOrderSources = fnListOfOrderSources();
                vm.ListOfCountries = fnListOfCountries();
                vm.ListOfEndUses = fnListOfEndUses(id);
                vm.ListOfShipVias = fnListOfShipVias();
                vm.ListOfBillingGroups = fnListOfBillingGroups(id);
                vm.CreateUser = HttpContext.Current.User.Identity.Name;
                vm.CreateDate = System.DateTime.Now;

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

                o.itemscount = (from t in db.tblOrderItem where t.OrderID == id select t).Count();
                o.transcount = (from t in db.tblOrderTrans where t.OrderID == id select t).Count();

                var cl = db.tblClient.Find(q.ClientID);
                o.clientname = cl.ClientName;
                o.logofilename = cl.LogoFileName;

                // Fill lists buried in object
                //o.ListOfDivisions = fnListOfDivisions(q.ClientID);
                //o.ListOfOrderTypes = fnListOfOrderTypes();
                o.ListOfSalesReps = fnListOfSalesReps(o.clientid);
                //o.ListOfOrderSources = fnListOfOrderSources();
                o.ListOfCountries = fnListOfCountries();
                o.ListOfEndUses = fnListOfEndUses(o.clientid);
                o.ListOfShipVias = fnListOfShipVias();
                o.ListOfBillingGroups = fnListOfBillingGroups(cl.ClientID);

                o.orderid = q.OrderID;
                o.clientid = q.ClientID;
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
                //if (q.OrderDate.HasValue)
                //{ o.orderdate = Convert.ToDateTime(q.OrderDate); }
                //else
                //{ o.orderdate = null; }

                o.company = q.Company;
                o.street = q.Street; o.street2 = q.Street2; o.street3 = q.Street3; o.city = q.City; o.state = q.State; o.Zip = q.Zip;
                o.country = q.Country; o.attention = q.Attention; o.email = q.Email; o.salesrep = q.SalesRep; o.sales_email = q.SalesEmail;
                o.req = q.Req; o.reqphone = q.ReqPhone; o.reqfax = q.ReqFax; o.reqemail = q.ReqEmail; o.enduse = q.EndUse; o.shipvia = q.ShipVia;
                o.shipacct = q.ShipAcct; o.phone = q.Phone; o.source = q.Source; o.fax = q.Fax; o.tracking = q.Tracking; o.special = q.Special;
                o.specialinternal = q.SpecialInternal; o.lit = Convert.ToBoolean(q.Lit); o.region = q.Region; o.coa = Convert.ToBoolean(q.COA);
                o.tds = Convert.ToBoolean(q.TDS); o.cid = q.CID; o.custacct = q.CustAcct; o.acode = q.ACode;
                o.importfile = q.ImportFile;

                if (q.ImportDateLine.HasValue)
                {
                    o.importdateline = Convert.ToDateTime(q.ImportDateLine);
                }
                else
                {
                    o.importdateline = null;
                }

                o.timing = q.Timing; o.volume = q.Volume; o.samplerack = Convert.ToBoolean(q.SampleRack); o.cmcuser = q.CMCUser;
                o.customerreference = q.CustomerReference;
                //o.division = q.Division;
                //o.busarea = q.BusArea;
                o.totalorderweight = q.TotalOrderWeight;
                o.spstaxid = q.SPSTaxID; o.spscurrency = q.SPSCurrency; o.spsshippedwt = q.SPSShippedWt; o.spsfreightcost = q.SPSFreightCost;
                o.invoicecompany = q.InvoiceCompany; o.invoicetitle = q.InvoiceTitle; o.invoicefirstname = q.InvoiceFirstName; o.invoicelastname = q.InvoiceLastName;
                o.invoiceaddress1 = q.InvoiceAddress1; o.invoiceaddress2 = q.InvoiceAddress2; o.invoiceaddress3 = q.InvoiceAddress3; o.invoicecity = q.InvoiceCity;
                o.invoicestateprov = q.InvoiceStateProv; o.invoicepostalcode = q.InvoicePostalCode; o.invoicecountry = q.InvoiceCountry; o.invoicephone = q.InvoicePhone;
                o.custordertype = q.CustOrderType;

                o.custrequestdate = null;
                if (q.CustRequestDate.HasValue)
                { o.custrequestdate = q.CustRequestDate; }

                o.approvaldate = null;
                if (q.ApprovalDate.HasValue)
                { o.approvaldate = q.ApprovalDate; }

                o.requesteddeliverydate = null;
                if (q.RequestedDeliveryDate.HasValue)
                { o.requesteddeliverydate = q.RequestedDeliveryDate; }

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

                // pc 04/28/2016 add per cd, ii
                o.billinggroup = q.BillingGroup;

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
                //    string sSQL = @"Insert into tblOrderMaster (ClientID,OrderDate) VALUES (@clientid,@orderdate)";
                //    db.Database.ExecuteSqlCommand(sSQL,
                //    new SqlParameter("clientid", o.clientid),
                //    new SqlParameter("orderdate", o.orderdate)
                //    );
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
                    //vm.CreateDate = System.DateTime.Now;
                    //vm.CreateUser = HttpContext.Current.User.Identity.Name;
                }

                // update time stamps
                vm.UpdateUser = HttpContext.Current.User.Identity.Name;
                vm.UpdateDate = System.DateTime.Now;

                var q = (from t in db.tblOrderMaster where t.OrderID == vm.orderid select t).FirstOrDefault();

                q.OrderDate = vm.orderdate;
                q.ClientID = vm.clientid;
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
                //q.Division = vm.division;
                //q.BusArea = vm.busarea;
                q.TotalOrderWeight = (vm.totalorderweight);
                q.SPSTaxID = vm.spstaxid;
                q.SPSCurrency = vm.spscurrency;
                q.SPSShippedWt = vm.spsshippedwt;
                q.SPSFreightCost = vm.spsfreightcost;
                q.InvoiceCompany = vm.invoicecompany;
                q.InvoiceTitle = vm.invoicetitle;
                q.InvoiceFirstName = vm.invoicefirstname;
                q.InvoiceLastName = vm.invoicelastname;
                q.InvoiceAddress1 = vm.invoiceaddress1;
                q.InvoiceAddress2 = vm.invoiceaddress2;
                q.InvoiceAddress3 = vm.invoiceaddress3;
                q.InvoiceCity = vm.invoicecity;
                q.InvoiceStateProv = vm.invoicestateprov;
                q.InvoicePostalCode = vm.invoicepostalcode;
                q.InvoiceCountry = vm.invoicecountry;
                q.InvoicePhone = vm.invoicephone;
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
                q.BillingGroup = vm.billinggroup;

                // reset the value
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
                // string sCommand = "";
                var q = (from t in db.tblOrderMaster where t.OrderID == vm.orderid select t).FirstOrDefault();

                var qCountry = (from t in db.tblCountry where t.Country == vm.country && t.DoNotShip == true select t).FirstOrDefault();
                if (qCountry != null)
                {
                    // flag the order record and the item records that are yet to be allocated
                    q.IsSDN = true;
                    var orderitems = (from t in db.tblOrderItem where t.OrderID == vm.orderid && t.AllocateStatus == null select t).ToList();
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
                    var orderitems = (from t in db.tblOrderItem where t.OrderID == vm.orderid && t.AllocateStatus == null select t).ToList();
                    foreach (var item in orderitems)
                    {
                        item.CSAllocate = false;
                        db.SaveChanges();
                    }
                    ShowAlert = true;
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
            // TODO finish the lookup and return T/F
            var filecontent = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Content/sdnlist.txt"));
            if (filecontent.Contains(vm.company))
            {
                return true;
            }
            //orderslist = (from t in orderslist where t.company != null && t.company.Contains(mycompany) select t).ToList();
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
                vm.CreateDate = System.DateTime.Now;
                vm.CreateUser = HttpContext.Current.User.Identity.Name;

                vm.ProductDetailID = -1;
                vm.ListOfProductDetailIDs = fnListOfProductCodes(vm.ClientID);
                vm.ProductCode = null;
                vm.ProductName = null;

                vm.ShelfID = -1;
                //vm.ListOfShelfIDs = filled by a ajax call to return a <select> tag
                vm.Size = null;
                vm.SRSize = null;
                vm.Qty = 1;
                vm.LotNumber = null;
                vm.ShipDate = null;
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

        public static string fnBuildSizeDropDown(int id)
        {
            // id=productdetailid
            using (var db = new EF.CMCSQL03Entities())
            {
                var qry = (from t in db.tblShelfMaster where t.ProductDetailID == id orderby t.Size select t);
                string s = "<option value='0' selected=true>Select Size</option>";
                if (qry.Count() > 0)
                {
                    foreach (var item in qry)
                    {
                        s = s + "<option value=" + item.ShelfID.ToString() + ">" + item.Size + "</option>";
                    }
                }
                else
                {
                    s = s + "<option value='0'>No Sizes Found</option>";
                }
                s = s + "<option value='99'>Special Request</option>";
                //s = s + "</select>";
                return s;
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
                vm.Qty = q.Qty;
                vm.LotNumber = q.LotNumber;
                vm.ShipDate = q.ShipDate;
                vm.CSAllocate = q.CSAllocate;
                vm.AllocateStatus = q.AllocateStatus;
                vm.NonCMCDelay = q.NonCMCDelay;
                vm.CarrierInvoiceRcvd = q.CarrierInvoiceRcvd;
                vm.DelayReason = q.DelayReason;
                vm.Status = q.Status;
                vm.StatusID = null;
                vm.ListOfStatusNotesIDs = fnListOfStatusNotesIDs();

                vm.ItemNotes = q.ItemNotes;
                vm.AlertNotesShipping = q.AlertNotesShipping;
                vm.AlertNotesPackOut = q.AlertNotesPackout;
                vm.AlertNotesOrderEntry = q.AlertNotesOrderEntry;
                vm.AlertNotesOther = q.AlertNotesOther;

                //obj.AllocatedBulkID = q.AllocatedBulkID;
                //obj.AllocatedStockID = q.AllocatedStockID;
                //obj.ImportItemID = q.ImportItemID;
                // retired field ???? obj.Mnemonic = q.Mnemonic;
                //obj.SRSize = q.SRSize;
                //obj.BackOrdered = q.BackOrdered;
                //obj.Bin = q.Bin;
                //obj.CustProdCode = q.CustProdCode;
                //obj.CustProdName = q.CustProdName;
                //obj.CustSize = q.CustSize;
                //obj.EmailSent = q.EmailSent;
                //obj.BackorderEmailSent = q.BackorderEmailSent;
                //obj.Weight = q.Weight;
                //obj.Warehouse = q.Warehouse;
                //obj.LineItem = q.LineItem;
                //obj.PackID = q.PackID;
                //obj.SPSCharge = q.SPSCharge;
                //obj.GrnUnNumber = q.GrnUnNumber;
                //obj.GrnPkGroup = q.GrnPkGroup;
                //obj.AirUnNumber = q.AirUnNumber;
                //obj.AirPkGroup = q.AirPkGroup;
                //obj.Via = q.Via;
                //obj.ShipWt = q.ShipWt;
                //obj.ShipDimWt = q.ShipDimWt;

                // Look for a reason to set the item R/O
                if (q.ShipDate != null)
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
                    vm.CreateDate = System.DateTime.Now;
                    vm.CreateUser = HttpContext.Current.User.Identity.Name;
                }
                var q = (from t in db.tblOrderItem where t.ItemID == vm.ItemID select t).FirstOrDefault();

                // q.ItemID = oi.ItemID;
                q.OrderID = vm.OrderID;
                // no clientid in table, only in vm
                q.CreateDate = vm.CreateDate;
                q.CreateUser = vm.CreateUser;
                q.UpdateDate = System.DateTime.Now;
                q.UpdateUser = HttpContext.Current.User.Identity.Name;

                q.ProductDetailID = vm.ProductDetailID;
                q.ShelfID = vm.ShelfID;
                //q.Size = vm.Size;   // update in post method later
                //q.SRSize = vm.SRSize;
                q.Qty = vm.Qty;
                q.LotNumber = vm.LotNumber;
                q.ShipDate = vm.ShipDate;
                q.CSAllocate = vm.CSAllocate;
                q.AllocateStatus = vm.AllocateStatus;
                q.NonCMCDelay = vm.NonCMCDelay;
                q.CarrierInvoiceRcvd = vm.CarrierInvoiceRcvd;
                q.Status = vm.Status;
                q.DelayReason = vm.DelayReason;
                q.ItemNotes = vm.ItemNotes;

                //q.AllocatedBulkID = null;
                //q.AllocatedStockID = null;
                //q.ImportItemID = vm.ImportItemID;
                //q.BackOrdered = vm.BackOrdered;
                //q.Bin = vm.Bin;
                //q.CustProdCode = vm.CustProdCode;
                //q.CustProdName = vm.CustProdName;
                //q.CustSize = vm.CustSize;
                //q.EmailSent = vm.EmailSent;
                //q.BackorderEmailSent = vm.BackorderEmailSent;
                //q.Weight = vm.Weight;
                //q.Warehouse = vm.Warehouse;
                //q.LineItem = vm.LineItem;
                //q.PackID = vm.PackID;
                //q.SPSCharge = vm.SPSCharge;
                //q.GrnUnNumber = vm.GrnUnNumber;
                //q.GrnPkGroup = vm.GrnPkGroup;
                //q.AirUnNumber = vm.AirUnNumber;
                //q.AirPkGroup = vm.AirPkGroup;
                //q.Via = vm.Via;
                //q.ShipWt = vm.ShipWt;
                //q.ShipDimWt = vm.ShipDimWt;
                //q.CreateDate = vm.CreateDate;
                //q.CreateUser = vm.CreateUser;
                //q.UpdateDate = System.DateTime.Now;
                //q.UpdateUser = HttpContext.Current.User.Identity.Name;

                // to force an ajax fail q.AllocateStatus = "DDDDDDDDDDDDDDDDDDDDD";

                db.SaveChanges();

                // update the row from other tables
                fnSaveItemPostUpdate(vm);

                // Go do the Order Trans work....
                fnGenerateOrderTransactions(vm.ItemID);

                return vm.ItemID;
            }
        }

        public static void fnSaveItemPostUpdate(OrderItem vm)
        {
            // pull values for ProductCode, ProductName, Size
            using (var db = new EF.CMCSQL03Entities())
            {
                var q = (from t in db.tblOrderItem where t.ItemID == vm.ItemID select t).FirstOrDefault();

                var dbPD = (from t in db.tblProductDetail where t.ProductDetailID == vm.ProductDetailID select t).FirstOrDefault();
                q.ProductCode = dbPD.ProductCode;
                q.ProductName = dbPD.ProductName;

                //var dbSM = (from t in db.tblShelfMaster where t.ShelfID == vm.ShelfID select t).FirstOrDefault();

                if (vm.ShelfID == 99 && !String.IsNullOrEmpty(vm.SRSize))
                {
                    q.Size = vm.SRSize;
                }
                else
                {
                    var dbSM = (from t in db.tblShelfMaster where t.ShelfID == vm.ShelfID select t).FirstOrDefault();
                    if (dbSM != null)
                    {
                        q.Size = dbSM.Size;
                        q.Weight = dbSM.UnitWeight;
                    }
                }
                q.AlertNotesShipping = dbPD.AlertNotesShipping;
                q.AlertNotesPackout = dbPD.AlertNotesPackout;
                q.AlertNotesOrderEntry = dbPD.AlertNotesOrderEntry;  // comes from profiles

                if (dbPD.AIRUNNUMBER == "UN3082" | dbPD.AIRUNNUMBER == "UN3077" | dbPD.GRNUNNUMBER == "UN3082" | dbPD.GRNUNNUMBER == "UN3077")
                    q.AlertNotesOther = "Products with UN3082 and UN3077 may be shipped as non hazardous if under 5 kg";

                db.SaveChanges();
            }
        }

        public static void fnDeleteOrderItem(int id)
        {
            System.Threading.Thread.Sleep(1000);
            using (var db = new EF.CMCSQL03Entities())
            {
                var qry = (from t in db.tblOrderItem where t.ItemID == id select t).FirstOrDefault();

                if (qry != null)
                {
                    string s = @"Delete from tblOrderItem where ItemID=" + id.ToString();
                    db.Database.ExecuteSqlCommand(s);

                    // TODO do we really want to delete all the transactons?
                    s = @"Delete from tblOrderTrans where OrderItemID=" + id.ToString();
                    db.Database.ExecuteSqlCommand(s);
                }
            }
        }

        public static int fnAllocateBulk(int OrderID, bool IncludeExpiredStock)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                int AllocationCount = 0;
                var orderitems = (from t in db.tblOrderItem where t.OrderID == OrderID && t.ShipDate == null && t.AllocateStatus == null && t.CSAllocate == true select t).ToList();
                if (orderitems == null)
                {
                    return AllocationCount;
                }
                foreach (var item in orderitems)
                {
                    var tblbulk = (from t in db.tblBulk
                                   join pm in db.tblProductMaster on t.ProductMasterID equals pm.ProductMasterID
                                   join pd in db.tblProductDetail on pm.ProductMasterID equals pd.ProductMasterID
                                   where (pd.ProductDetailID == item.ProductDetailID) && (t.LotNumber == item.LotNumber)
                                   select new
                                   {
                                       BulkID = t.BulkID,
                                       Warehouse = t.Warehouse,
                                       Qty = t.Qty,
                                       LotNumber = t.LotNumber,
                                       CurrentWeight = t.CurrentWeight,
                                       ExpirationDate = t.ExpirationDate,
                                       BulkStatus = t.BulkStatus,
                                       Bin = t.Bin
                                   }).ToList();
                    if (IncludeExpiredStock == false)
                    { tblbulk = (from t in tblbulk where t.BulkStatus == "AVAIL" select t).ToList(); }
                    else
                    { tblbulk = (from t in tblbulk where t.BulkStatus == "QC" select t).ToList(); }
                    // shouldnt this be where >= ??
                    tblbulk = (from t in tblbulk where t.CurrentWeight >= (item.Qty * item.Weight) select t).ToList();
                    tblbulk = (from t in tblbulk orderby t.ExpirationDate ascending select t).ToList();
                    foreach (var row in tblbulk)
                    {
                        // update tblstock record (need to use separate qry)
                        AllocationCount = AllocationCount + 1;
                        var q = db.tblBulk.Find(row.BulkID);
                        q.CurrentWeight = q.CurrentWeight - (item.Qty * item.Weight);

                        item.AllocatedBulkID = row.BulkID;
                        item.Warehouse = row.Warehouse;
                        item.LotNumber = row.LotNumber;
                        item.AllocateStatus = "A";
                        item.Bin = row.Bin;
                        item.ExpirationDate = q.ExpirationDate;
                        // insert log
                        fnInsertInvTrans("B07", System.DateTime.Now, null, row.BulkID, item.Qty, item.Weight, System.DateTime.Now, HttpContext.Current.User.Identity.Name, null, null);
                        db.SaveChanges();

                        break;
                    }
                }
                return AllocationCount;
            }
        }

        public static int fnAllocateShelf(int OrderID, bool IncludeExpiredStock)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                // build list of orderitems
                int AllocationCount = 0;
                var items = (from t in db.tblOrderItem where t.OrderID == OrderID && t.ShipDate == null && t.AllocateStatus == null && t.CSAllocate == true select t).ToList();
                if (items == null)
                {
                    //return OrderID;
                    return AllocationCount;
                }

                foreach (var item in items)
                {
                    // build a list of potential tblStock records to look thru
                    var tblstock = (from t in db.tblStock
                                    join b in db.tblBulk on t.BulkID equals b.BulkID
                                    where t.ShelfID == item.ShelfID
                                    select
                                        new
                                        {
                                            StockID = t.StockID,
                                            Warehouse = t.Warehouse,
                                            QtyOnHand = t.QtyOnHand,
                                            QtyAllocated = t.QtyAllocated,
                                            Bin = t.Bin,
                                            ShelfStatus = t.ShelfStatus,
                                            ExpirationDate = b.ExpirationDate,
                                            LotNumber = b.LotNumber
                                        }
                                    ).ToList();

                    // further limit the tblStock records
                    if (item.LotNumber != null)
                    {
                        tblstock = (from t in tblstock where t.LotNumber == item.LotNumber select t).ToList();
                    }
                    if (IncludeExpiredStock == false)
                    {
                        tblstock = (from t in tblstock where t.ShelfStatus == "AVAIL" select t).ToList();
                    }
                    else
                    {
                        // ??????????
                        tblstock = (from t in tblstock where t.ShelfStatus == "QC" select t).ToList();
                    }
                    tblstock = (from t in tblstock orderby t.ExpirationDate ascending select t).ToList();

                    // Do something with the Profiles alert message (add to tblOrderItem on creation?
                    // Do something with the Special Transport Provision Alert - add to tblOrderitem on creation?

                    // Page thru tblstock rows looking for the first record that has enough qty then bail
                    foreach (var row in tblstock)
                    {
                        if (row.QtyOnHand - row.QtyAllocated >= item.Qty)
                        {
                            AllocationCount = AllocationCount + 1;
                            // update tblstock record (need to use separate qry)
                            var q = db.tblStock.Find(row.StockID);
                            q.QtyAllocated = q.QtyAllocated + item.Qty;

                            // update tblorderitem record               FIX THIS
                            item.AllocatedStockID = row.StockID;
                            item.Warehouse = row.Warehouse;
                            item.LotNumber = row.LotNumber;
                            item.AllocateStatus = "A";
                            item.Bin = row.Bin;

                            // insert log
                            fnInsertInvTrans("S05", System.DateTime.Now, row.StockID, null, item.Qty, null, System.DateTime.Now, HttpContext.Current.User.Identity.Name, null, null);
                        }
                        db.SaveChanges();

                        // Insert client specific stuff
                        // Elementis - Print sample letter???

                        break;
                    }
                }
                //return OrderID;
                return AllocationCount;
            }
        }

        public static void fnInsertInvTrans(string vTransType, DateTime? vTransDate, int? vStockID, int? vBulkID, int? vTransQty, decimal? vTransAmount, DateTime? vCreateDate, string vCreateUser, DateTime? vUpdateDate, string vUpdateUser)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                EF.tblInvTrans newrec = new EF.tblInvTrans();
                newrec.TransType = vTransType;
                newrec.TransDate = vTransDate;
                newrec.StockID = vStockID;
                newrec.BulkID = vBulkID;
                newrec.TransQty = vTransQty;
                newrec.TransAmount = vTransAmount;
                newrec.CreateDate = vCreateDate;
                newrec.CreateUser = vCreateUser;
                newrec.UpdateDate = vUpdateDate;
                newrec.UpdateUser = vUpdateUser;
                db.tblInvTrans.Add(newrec);
                db.SaveChanges();
            }
        }

        public static void fnProcessInvTrans(int id)
        {
            // read the tblInvTrans record and do something with it
            // Pull fields to de-normalize it
        }

        public static OrderTrans fnCreateTrans(int id)
        {
            // id=orderid
            using (var db = new EF.CMCSQL03Entities())
            {
                var vm = new OrderTrans();
                vm.ordertransid = -1;
                vm.orderid = id;
                vm.createdate = System.DateTime.Now;
                vm.transdate = System.DateTime.Now.Date;
                vm.createuser = HttpContext.Current.User.Identity.Name;
                var cl = (from t in db.tblOrderMaster where t.OrderID == id select t).FirstOrDefault();
                vm.clientid = cl.ClientID;
                vm.ListOfOrderTransTypes = fnListOfOrderTransTypes();
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
                    vm.createdate = System.DateTime.Now;
                    vm.createuser = HttpContext.Current.User.Identity.Name;
                    db.SaveChanges();
                    vm.ordertransid = newrec.OrderTransID;
                }
                vm.updatedate = System.DateTime.Now;
                vm.updateuser = HttpContext.Current.User.Identity.Name;

                var d = (from t in db.tblOrderTrans where t.OrderTransID == vm.ordertransid select t).FirstOrDefault();
                //d.ordertransid = qry.OrderTransID;
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

                vm.ListOfOrderTransTypes = fnListOfOrderTransTypes();
                return vm;
            }
        }

        public static void fnDeleteTrans(int id)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                string s = @"Delete from tblOrderTrans where OrderTransID=" + id.ToString();
                db.Database.ExecuteSqlCommand(s);
            }
        }

        public static void fnGenerateOrderTransactions(int id)
        {
            // TODO How to create charges when there is no tblShelfMaster????

            //id=tblOrderItem.Itemid
            using (var db = new EF.CMCSQL03Entities())
            {
                var oi = (from t in db.tblOrderItem where t.ItemID == id select t).FirstOrDefault();
                var o = (from t in db.tblOrderMaster where t.OrderID == oi.OrderID select t).FirstOrDefault();
                var clt = (from t in db.tblClient where t.ClientID == o.ClientID select t).FirstOrDefault();
                string s = "";

                // Tier 1 sample charge
                s = "Delete from tblOrderTrans where OrderItemID=" + oi.ItemID + " and Transtype = 'SAMP' And CreateUser='System'";
                db.Database.ExecuteSqlCommand(s);

                var tierSize = (from t in db.tblTier where t.ClientID == o.ClientID && t.Size == oi.Size && t.Tier == "1" select t).FirstOrDefault();
                if (tierSize != null)
                {
                    EF.tblOrderTrans newrec = new EF.tblOrderTrans();
                    newrec.TransDate = System.DateTime.Now;
                    newrec.OrderItemID = oi.ItemID;
                    newrec.OrderID = oi.OrderID;
                    newrec.ClientID = o.ClientID;
                    newrec.TransType = "SAMP";
                    newrec.TransQty = oi.Qty;
                    newrec.TransAmount = tierSize.Price;
                    newrec.CreateDate = System.DateTime.Now;
                    //newrec.CreateDate = System.DateTimeOffset.UtcNow;
                    newrec.CreateUser = "System";
                    db.tblOrderTrans.Add(newrec);
                    db.SaveChanges();
                }
                else
                {
                    // Assume this is a SR size ??????????????????????
                    var tierSpecialRequest = (from t in db.tblTier where t.ClientID == o.ClientID && t.Size == "1SR" && t.Tier == "1" select t).FirstOrDefault();
                    if (tierSpecialRequest != null)
                    {
                        EF.tblOrderTrans newrec = new EF.tblOrderTrans();
                        newrec.TransDate = System.DateTime.Now;
                        newrec.OrderItemID = oi.ItemID;
                        newrec.OrderID = oi.OrderID;
                        newrec.ClientID = o.ClientID;
                        newrec.TransType = "SAMP";
                        newrec.TransQty = oi.Qty;
                        newrec.TransAmount = tierSpecialRequest.Price;
                        newrec.Comments = "Special Request";
                        newrec.CreateDate = System.DateTime.Now;
                        newrec.CreateUser = "System";
                        db.tblOrderTrans.Add(newrec);
                        db.SaveChanges();
                    }
                }

                // Other charges from shelfmaster
                var sm = (from t in db.tblShelfMaster where t.ShelfID == oi.ShelfID select t).FirstOrDefault();
                var qrySurcharges = (from t in db.tblSurcharge where t.ClientID == o.ClientID select t).FirstOrDefault();

                if (sm.HazardSurcharge == true)
                {
                    s = "Delete from tblOrderTrans where OrderItemID=" + oi.ItemID + " and Transtype = 'HAZD' And CreateUser='System'";
                    db.Database.ExecuteSqlCommand(s);
                    EF.tblOrderTrans newrec = new EF.tblOrderTrans();
                    newrec.TransDate = System.DateTime.Now;
                    newrec.OrderItemID = oi.ItemID;
                    newrec.OrderID = oi.OrderID;
                    newrec.ClientID = o.ClientID;
                    newrec.TransDate = System.DateTime.Now;
                    newrec.TransType = "HAZD";
                    newrec.TransQty = oi.Qty;
                    newrec.TransAmount = qrySurcharges.Haz;
                    newrec.CreateDate = System.DateTime.Now;
                    newrec.CreateUser = "System";
                    db.tblOrderTrans.Add(newrec);
                    db.SaveChanges();
                }

                if (sm.FlammableSurcharge == true)
                {
                    s = "Delete from tblOrderTrans where OrderItemID=" + oi.ItemID + " and Transtype = 'FLAM' And CreateUser='System'";
                    db.Database.ExecuteSqlCommand(s);
                    EF.tblOrderTrans newrec = new EF.tblOrderTrans();
                    newrec.TransDate = System.DateTime.Now;
                    newrec.OrderItemID = oi.ItemID;
                    newrec.OrderID = oi.OrderID;
                    newrec.ClientID = o.ClientID;
                    newrec.TransDate = System.DateTime.Now;
                    newrec.TransType = "FLAM";
                    newrec.TransQty = oi.Qty;
                    newrec.TransAmount = qrySurcharges.Flam;
                    newrec.CreateDate = System.DateTime.Now;
                    newrec.CreateUser = "System";
                    db.tblOrderTrans.Add(newrec);
                    db.SaveChanges();
                }

                if (sm.HeatSurcharge == true)
                {
                    s = "Delete from tblOrderTrans where OrderItemID=" + oi.ItemID + " and Transtype = 'HEAT' And CreateUser='System'";
                    db.Database.ExecuteSqlCommand(s);
                    EF.tblOrderTrans newrec = new EF.tblOrderTrans();
                    newrec.TransDate = System.DateTime.Now;
                    newrec.OrderItemID = oi.ItemID;
                    newrec.OrderID = oi.OrderID;
                    newrec.ClientID = o.ClientID;
                    newrec.TransDate = System.DateTime.Now;
                    newrec.TransType = "HEAT";
                    newrec.TransQty = oi.Qty;
                    newrec.TransAmount = qrySurcharges.Heat;
                    newrec.CreateDate = System.DateTime.Now;
                    newrec.CreateUser = "System";
                    db.tblOrderTrans.Add(newrec);
                    db.SaveChanges();
                }

                if (sm.RefrigSurcharge == true)
                {
                    s = "Delete from tblOrderTrans where OrderItemID=" + oi.ItemID + " and Transtype = 'REFR' And CreateUser='System'";
                    db.Database.ExecuteSqlCommand(s);
                    EF.tblOrderTrans newrec = new EF.tblOrderTrans();
                    newrec.TransDate = System.DateTime.Now;
                    newrec.OrderItemID = oi.ItemID;
                    newrec.OrderID = oi.OrderID;
                    newrec.ClientID = o.ClientID;
                    newrec.TransDate = System.DateTime.Now;
                    newrec.TransType = "REFR";
                    newrec.TransQty = oi.Qty;
                    newrec.TransAmount = qrySurcharges.Refrig;
                    newrec.CreateDate = System.DateTime.Now;
                    newrec.CreateUser = "System";
                    db.tblOrderTrans.Add(newrec);
                    db.SaveChanges();
                }

                if (sm.FreezerSurcharge == true)
                {
                    s = "Delete from tblOrderTrans where OrderItemID=" + oi.ItemID + " and Transtype = 'FREZ' And CreateUser='System'";
                    db.Database.ExecuteSqlCommand(s);
                    EF.tblOrderTrans newrec = new EF.tblOrderTrans();
                    newrec.TransDate = System.DateTime.Now;
                    newrec.OrderItemID = oi.ItemID;
                    newrec.OrderID = oi.OrderID;
                    newrec.ClientID = o.ClientID;
                    newrec.TransDate = System.DateTime.Now;
                    newrec.TransType = "FREZ";
                    newrec.TransQty = oi.Qty;
                    newrec.TransAmount = qrySurcharges.Freezer;
                    newrec.CreateDate = System.DateTime.Now;
                    newrec.CreateUser = "System";
                    db.tblOrderTrans.Add(newrec);
                    db.SaveChanges();
                }

                if (sm.CleanSurcharge == true)
                {
                    s = "Delete from tblOrderTrans where OrderItemID=" + oi.ItemID + " and Transtype = 'CLEN' And CreateUser='System'";
                    db.Database.ExecuteSqlCommand(s);
                    EF.tblOrderTrans newrec = new EF.tblOrderTrans();
                    newrec.TransDate = System.DateTime.Now;
                    newrec.OrderItemID = oi.ItemID;
                    newrec.OrderID = oi.OrderID;
                    newrec.ClientID = o.ClientID;
                    newrec.TransDate = System.DateTime.Now;
                    newrec.TransType = "CLEN";
                    newrec.TransQty = oi.Qty;
                    newrec.TransAmount = qrySurcharges.Clean;
                    newrec.CreateDate = System.DateTime.Now;
                    newrec.CreateUser = "System";
                    db.tblOrderTrans.Add(newrec);
                    db.SaveChanges();
                }

                if (sm.BlendSurcharge == true)
                {
                    s = "Delete from tblOrderTrans where OrderItemID=" + oi.ItemID + " and Transtype = 'BLND' And CreateUser='System'";
                    db.Database.ExecuteSqlCommand(s);
                    EF.tblOrderTrans newrec = new EF.tblOrderTrans();
                    newrec.TransDate = System.DateTime.Now;
                    newrec.OrderItemID = oi.ItemID;
                    newrec.OrderID = oi.OrderID;
                    newrec.ClientID = o.ClientID;
                    newrec.TransDate = System.DateTime.Now;
                    newrec.TransType = "BLND";
                    newrec.TransQty = oi.Qty;
                    newrec.TransAmount = 0;
                    newrec.Comments = "Missing tblSurcharge record for BLND";
                    newrec.CreateDate = System.DateTime.Now;
                    newrec.CreateUser = "System";
                    db.tblOrderTrans.Add(newrec);
                    db.SaveChanges();
                }

                if (sm.NalgeneSurcharge == true)
                {
                    s = "Delete from tblOrderTrans where OrderItemID=" + oi.ItemID + " and Transtype = 'NALG' And CreateUser='System'";
                    db.Database.ExecuteSqlCommand(s);
                    EF.tblOrderTrans newrec = new EF.tblOrderTrans();
                    newrec.TransDate = System.DateTime.Now;
                    newrec.OrderItemID = oi.ItemID;
                    newrec.OrderID = oi.OrderID;
                    newrec.ClientID = o.ClientID;
                    newrec.TransDate = System.DateTime.Now;
                    newrec.TransType = "NALG";
                    newrec.TransQty = oi.Qty;
                    newrec.TransAmount = qrySurcharges.Nalgene;
                    newrec.CreateDate = System.DateTime.Now;
                    newrec.CreateUser = "System";
                    db.tblOrderTrans.Add(newrec);
                    db.SaveChanges();
                }

                if (sm.NitrogenSurcharge == true)
                {
                    s = "Delete from tblOrderTrans where OrderItemID=" + oi.ItemID + " and Transtype = 'NITR' And CreateUser='System'";
                    db.Database.ExecuteSqlCommand(s);
                    EF.tblOrderTrans newrec = new EF.tblOrderTrans();
                    newrec.TransDate = System.DateTime.Now;
                    newrec.OrderItemID = oi.ItemID;
                    newrec.OrderID = oi.OrderID;
                    newrec.ClientID = o.ClientID;
                    newrec.TransDate = System.DateTime.Now;
                    newrec.TransType = "NITR";
                    newrec.TransQty = oi.Qty;
                    newrec.TransAmount = 0;
                    newrec.Comments = "Missing tblSurcharge record for NITR";
                    newrec.CreateDate = System.DateTime.Now;
                    newrec.CreateUser = "System";
                    db.tblOrderTrans.Add(newrec);
                    db.SaveChanges();
                }

                if (sm.BiocideSurcharge == true)
                {
                    s = "Delete from tblOrderTrans where OrderItemID=" + oi.ItemID + " and Transtype = 'BIOC' And CreateUser='System'";
                    db.Database.ExecuteSqlCommand(s);
                    EF.tblOrderTrans newrec = new EF.tblOrderTrans();
                    newrec.TransDate = System.DateTime.Now;
                    newrec.OrderItemID = oi.ItemID;
                    newrec.OrderID = oi.OrderID;
                    newrec.ClientID = o.ClientID;
                    newrec.TransDate = System.DateTime.Now;
                    newrec.TransType = "BIOC";
                    newrec.TransQty = oi.Qty;
                    newrec.TransAmount = 0;
                    newrec.Comments = "Missing tblSurcharge record for BIOC";
                    newrec.CreateDate = System.DateTime.Now;
                    newrec.CreateUser = "System";
                    db.tblOrderTrans.Add(newrec);
                    db.SaveChanges();
                }

                if (sm.KosherSurcharge == true)
                {
                    s = "Delete from tblOrderTrans where OrderItemID=" + oi.ItemID + " and Transtype = 'KOSH' And CreateUser='System'";
                    db.Database.ExecuteSqlCommand(s);
                    EF.tblOrderTrans newrec = new EF.tblOrderTrans();
                    newrec.TransDate = System.DateTime.Now;
                    newrec.OrderItemID = oi.ItemID;
                    newrec.OrderID = oi.OrderID;
                    newrec.ClientID = o.ClientID;
                    newrec.TransDate = System.DateTime.Now;
                    newrec.TransType = "KOSH";
                    newrec.TransQty = oi.Qty;
                    newrec.TransAmount = 0;
                    newrec.Comments = "Missing tblSurcharge record for KOSH";
                    newrec.CreateDate = System.DateTime.Now;
                    newrec.CreateUser = "System";
                    db.tblOrderTrans.Add(newrec);
                    db.SaveChanges();
                }

                if (sm.LabelSurcharge == true)
                {
                    s = "Delete from tblOrderTrans where OrderItemID=" + oi.ItemID + " and Transtype = 'LABL' And CreateUser='System'";
                    db.Database.ExecuteSqlCommand(s);
                    EF.tblOrderTrans newrec = new EF.tblOrderTrans();
                    newrec.TransDate = System.DateTime.Now;
                    newrec.OrderItemID = oi.ItemID;
                    newrec.OrderID = oi.OrderID;
                    newrec.ClientID = o.ClientID;
                    newrec.TransDate = System.DateTime.Now;
                    newrec.TransType = "LABL";
                    newrec.TransQty = oi.Qty;
                    newrec.TransAmount = qrySurcharges.LabelFee;
                    newrec.CreateDate = System.DateTime.Now;
                    newrec.CreateUser = "System";
                    db.tblOrderTrans.Add(newrec);
                    db.SaveChanges();
                }

                if (sm.OtherSurcharge == true)
                {
                    s = "Delete from tblOrderTrans where OrderItemID=" + oi.ItemID + " and Transtype = 'OTHR' And CreateUser='System'";
                    db.Database.ExecuteSqlCommand(s);
                    EF.tblOrderTrans newrec = new EF.tblOrderTrans();
                    newrec.TransDate = System.DateTime.Now;
                    newrec.OrderItemID = oi.ItemID;
                    newrec.OrderID = oi.OrderID;
                    newrec.ClientID = o.ClientID;
                    newrec.TransDate = System.DateTime.Now;
                    newrec.TransType = "OTHR";
                    newrec.TransQty = oi.Qty;
                    newrec.TransAmount = sm.OtherSurchargeAmt;
                    newrec.CreateDate = System.DateTime.Now;
                    newrec.CreateUser = "System";
                    db.tblOrderTrans.Add(newrec);
                    db.SaveChanges();
                }
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

        public static List<SelectListItem> fnListOfClientIDs()
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                List<SelectListItem> mylist = new List<SelectListItem>();
                mylist = (from t in db.tblClient
                          orderby t.ClientName
                          select new SelectListItem { Value = t.ClientID.ToString(), Text = t.ClientName }).ToList();
                mylist.Insert(0, new SelectListItem { Value = "0", Text = "Select Client" });
                return mylist;
            }
        }

        //private static string AppKey(string sAppKey)
        //{
        //    string s = "System.Configuration.ConfigurationManager.AppSettings['" + sAppKey + "'].ConnectionString";
        //    return s;
        //}

        public static int ExecuteADOSQL(string sql)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                db.Database.ExecuteSqlCommand(sql);
                return 1;
            }
        }

        private static List<SelectListItem> fnListOfOrderTransTypes()
        {
            List<SelectListItem> mylist = new List<SelectListItem>();
            mylist.Add(new SelectListItem { Text = "BIOC", Value = "BIOC" });
            mylist.Add(new SelectListItem { Text = "BLND", Value = "BLND" });
            mylist.Add(new SelectListItem { Text = "CLEN", Value = "CLEN" });
            mylist.Add(new SelectListItem { Text = "FLAM", Value = "FLAM" });
            mylist.Add(new SelectListItem { Text = "FREZ", Value = "FREZ" });
            mylist.Add(new SelectListItem { Text = "HAZD", Value = "HAZD" });
            mylist.Add(new SelectListItem { Text = "HEAT", Value = "HEAT" });
            mylist.Add(new SelectListItem { Text = "KOSH", Value = "KOSH" });
            mylist.Add(new SelectListItem { Text = "LABL", Value = "LABL" });
            mylist.Add(new SelectListItem { Text = "MISC", Value = "MISC" });
            mylist.Add(new SelectListItem { Text = "MEMO", Value = "MEMO" });
            mylist.Add(new SelectListItem { Text = "NALG", Value = "NALG" });
            mylist.Add(new SelectListItem { Text = "NITR", Value = "NITR" });
            mylist.Add(new SelectListItem { Text = "REFR", Value = "REFR" });
            mylist.Add(new SelectListItem { Text = "SAMP", Value = "SAMP" });
            mylist.Add(new SelectListItem { Text = "OTHR", Value = "OTHR" });
            mylist.Insert(0, new SelectListItem { Value = "0", Text = "Select Type" });
            return mylist;
        }

        //fnListOfBillingGroups
        public static List<SelectListItem> fnListOfBillingGroups(int? id)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                List<SelectListItem> mylist = new List<SelectListItem>();
                mylist = (from t in db.tblDivision
                          where t.ClientID == id
                          orderby t.Division
                          select new SelectListItem { Value = t.Division, Text = t.Division }).Distinct().ToList();
                mylist.Insert(0, new SelectListItem { Value = "", Text = "" });
                return mylist;
            }
        }

        public static List<OrderItem> fnListOfOrderItemsForOrderID(long OrderID)
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
                          select new SelectListItem { Value = t.ShelfID.ToString(), Text = t.Size }).ToList();
                mylist.Insert(0, new SelectListItem { Value = "0", Text = " -- Select Size --" });
                mylist.Add(new SelectListItem { Value = "99", Text = "SR" });
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
                          select new SelectListItem { Value = t.CustProductCode, Text = t.CustProductCode + " : " + t.CMCProductCode }).ToList();
                mylist.Insert(0, new SelectListItem { Value = "0", Text = " -- Cust Product Code -- " });
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
                          select new SelectListItem { Value = t.OrderID.ToString(), Text = t.OrderID.ToString() }).ToList();
                mylist.Insert(0, new SelectListItem { Value = "0", Text = "Please Select" });
                return mylist;
            }
        }

        public static List<SelectListItem> fnListOfOrderTypes()
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                List<SelectListItem> mylist = new List<SelectListItem>();
                mylist = (from t in db.tblOrderType
                          orderby t.OrderType
                          select new SelectListItem { Value = t.OrderType, Text = t.OrderType }).ToList();
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
                          select new SelectListItem { Value = t.Country, Text = t.Country }).ToList();
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
                          where t.ContactType == "SalesRep"
                          where t.ClientID == id
                          orderby t.FullName
                          select new SelectListItem { Value = t.FullName, Text = t.FullName }).ToList();
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
                          where t.ContactType == "Requestor"
                          where t.ClientID == id
                          orderby t.FullName
                          select new SelectListItem { Value = t.FullName, Text = t.FullName }).ToList();
                mylist.Insert(0, new SelectListItem { Value = "0", Text = "Please Select" });
                return mylist;
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
                          select new SelectListItem { Value = t.EndUse, Text = t.EndUse }).ToList();
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
                          select new SelectListItem { Value = t.CarrierName, Text = t.CarrierName }).ToList();
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
                          select new SelectListItem { Value = t.Source, Text = t.Source }).ToList();
                mylist.Insert(0, new SelectListItem { Value = "0", Text = "Please Select" });
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
                          orderby t.Division
                          select new SelectListItem { Value = t.Division, Text = t.Division }).ToList();
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
                          select new SelectListItem { Value = t.ProductDetailID.ToString(), Text = t.ProductCode + " " + t.ProductName }).ToList();
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
                          select new SelectListItem { Value = t.CarrierName, Text = t.CarrierName }).ToList();
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
                //mylist = (from t in db.tblStatusNotes orderby t.Note select new SelectListItem { Value = t.StatusNotesID.ToString(), Text = t.Note }).ToList();
                mylist = (from t in db.tblStatusNotes orderby t.Note select new SelectListItem { Value = t.Note, Text = t.Note }).ToList();
                mylist.Insert(0, new SelectListItem { Value = "0", Text = "Please Select" });
                return mylist;
            }
        }
    }
}