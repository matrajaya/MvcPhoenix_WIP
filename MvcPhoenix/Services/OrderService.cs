using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using MvcPhoenix.Models;

//using Microsoft.AspNet.Identity;

namespace MvcPhoenix.Services
{
    public class OrderService
    {
        private static string PathToLogos = "http://www.mysamplecenter.com/Logos/";
        //private static string connstring = System.Configuration.ConfigurationManager.ConnectionStrings["ADOConnectionString"].ConnectionString;


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
                vm.logofilename = PathToLogos + cl.LogoFileName;
                vm.orderstatus = "z";
                vm.orderdate = System.DateTime.Now;
                //vm.ListOfDivisions = fnListOfDivisions(id);
                vm.ListOfOrderTypes = fnListOfOrderTypes();
                vm.ListOfSalesReps = fnListOfSalesReps(id);
                vm.ListOfOrderSources = fnListOfOrderSources();
                vm.ListOfCountries = fnListOfCountries();
                vm.ListOfEndUses = fnListOfEndUses(id);
                vm.ListOfShipVias = fnListOfShipVias();
                vm.ListOfBillingGroups = fnListOfBillingGroups(id);
                vm.CreateUser = HttpContext.Current.User.Identity.Name;
                vm.CreateDate = System.DateTime.Now;
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
                o.logofilename = PathToLogos + cl.LogoFileName;

                // Fill lists buried in object
                //o.ListOfDivisions = fnListOfDivisions(q.ClientID);
                o.ListOfOrderTypes = fnListOfOrderTypes();
                o.ListOfSalesReps = fnListOfSalesReps(o.clientid);
                o.ListOfOrderSources = fnListOfOrderSources();
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
                    vm.CreateDate = System.DateTime.Now;
                    vm.CreateUser = HttpContext.Current.User.Identity.Name;
                }
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

                q.UpdateUser = HttpContext.Current.User.Identity.Name;
                q.UpdateDate = System.DateTime.Now;

                q.BillingGroup = vm.billinggroup;

                db.SaveChanges();
                return vm.orderid;
            }
        }


        public static OrderItem fnCreateItem(int id)
        {
            // id=OrderID
            // populate the minimum fields needed for the modal View
            using(var db = new EF.CMCSQL03Entities())
            {
            OrderItem obj = new OrderItem();
            obj.CrudMode = "RW";
            obj.ItemID = -1;
            obj.OrderID = id;
            var dbOrder = db.tblOrderMaster.Find(id);
            obj.ClientID = dbOrder.ClientID;
            obj.ProductDetailID = -1;
            obj.ListOfProductDetailIDs = fnListOfProductCodes(obj.ClientID);
            obj.ShelfID = -1;

                // on edit, the shelf ids DD needs to be filled based on the current produ
            //obj.ListOfShelfIDs = filled by a ajax call to return a <select> tag
            obj.LotNumber = null;
            obj.Qty = 1;
            obj.SRSize = null;
            obj.NonCMCDelay = false;
            obj.CarrierInvoiceRcvd = false;
            obj.StatusID = -1;
            obj.ListOfStatusNotesIDs = fnListOfStatusNotesIDs();
            obj.CreateUser = HttpContext.Current.User.Identity.Name;
            obj.CreateDate = System.DateTime.Now;
            return obj;
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
                        s = s + "<option value=" + item.ShelfID.ToString() + ">" + item.Size +  "</option>";
                    }
                }
                else
                {
                    s = s + "<option value='0'>No Sizes Found</option>";
                }
                s = s + "<option value='SR'>Special Request</option>";
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

                // update fields
                // q.ItemID = oi.ItemID;
                // no clientid in table, only in vm
                q.OrderID = vm.OrderID;
                q.ShelfID = vm.ShelfID;
                q.ProductDetailID = vm.ProductDetailID;
                //q.AllocatedBulkID = null;
                //q.AllocatedStockID = null;
                //q.ImportItemID = vm.ImportItemID;

                // Lookups
                var dbPD = (from t in db.tblProductDetail where t.ProductDetailID == vm.ProductDetailID select t).FirstOrDefault();
                var dbSM = (from t in db.tblShelfMaster where t.ShelfID == vm.ShelfID select t).FirstOrDefault();
                q.ProductCode = dbPD.ProductCode;
                q.ProductName = dbPD.ProductName;
                //
                q.LotNumber = vm.LotNumber;
                q.Qty = vm.Qty;
                if (vm.Size=="SR")
                {
                    vm.Size = vm.SRSize;
                }
                else
                {
                    q.Size = dbSM.Size;
                }
                q.ShipDate = vm.ShipDate;
                q.NonCMCDelay = vm.NonCMCDelay;
                //q.DelayReason = vm.DelayReason;
                //q.BackOrdered = vm.BackOrdered;
                q.Status = vm.Status;
                q.AllocateStatus = vm.AllocateStatus;
                //q.CSAllocate = vm.CSAllocate;
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
                q.CarrierInvoiceRcvd = vm.CarrierInvoiceRcvd;
                //q.GrnUnNumber = vm.GrnUnNumber;
                //q.GrnPkGroup = vm.GrnPkGroup;
                //q.AirUnNumber = vm.AirUnNumber;
                //q.AirPkGroup = vm.AirPkGroup;
                //q.Via = vm.Via;
                //q.ShipWt = vm.ShipWt;
                //q.ShipDimWt = vm.ShipDimWt;
                q.CreateDate = vm.CreateDate;
                q.CreateUser = vm.CreateUser;
                q.UpdateDate = System.DateTime.Now;
                q.UpdateUser = HttpContext.Current.User.Identity.Name;
               
                db.SaveChanges();
                
                // Go do the Order Trans work....
                fnGenerateOrderTransactions(vm.ItemID);

                return  vm.ItemID;
            }
        }



        public static OrderItem fnFillOrderItem(int id)
        {
            // id=itemid
            // Fill all the fields from the tblOrderItem record
            using (var db = new EF.CMCSQL03Entities())
            {
                OrderItem obj = new OrderItem();
                var q = (from t in db.tblOrderItem
                         where t.ItemID == id
                         select t).FirstOrDefault();
                var cl = db.tblOrderMaster.Find(q.OrderID);

                // Hidden fields to persist for postback
                obj.ItemID = q.ItemID;
                obj.OrderID = q.OrderID;
                obj.ClientID = Convert.ToInt32(cl.ClientID);
                obj.CrudMode = "RW";    // default value

                obj.ShelfID = q.ShelfID;
                obj.ProductDetailID = q.ProductDetailID;
                obj.ListOfProductDetailIDs = fnListOfProductCodes(obj.ClientID);
                obj.ListOfShelfIDs = fnProductCodeSizes(Convert.ToInt32(obj.ProductDetailID));
                //obj.AllocatedBulkID = q.AllocatedBulkID;
                //obj.AllocatedStockID = q.AllocatedStockID;
                //obj.ImportItemID = q.ImportItemID;
                // read ProductCode and Size from the tblOrderItem record for display purposes if R/O (handled by view)
                obj.ProductCode = q.ProductCode;
                obj.ProductName = q.ProductName;
                // retired field ???? obj.Mnemonic = q.Mnemonic;
                obj.LotNumber = q.LotNumber;
                obj.Qty = q.Qty;
                obj.Size = q.Size;
                //obj.SRSize = q.SRSize;
                obj.ShipDate = q.ShipDate;
                obj.NonCMCDelay = q.NonCMCDelay;
                //obj.DelayReason = q.DelayReason;
                //obj.BackOrdered = q.BackOrdered;
                obj.Status = q.Status;
                obj.AllocateStatus = q.AllocateStatus;
                obj.CSAllocate = q.CSAllocate;
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
                obj.CarrierInvoiceRcvd = q.CarrierInvoiceRcvd;
                //obj.GrnUnNumber = q.GrnUnNumber;
                //obj.GrnPkGroup = q.GrnPkGroup;
                //obj.AirUnNumber = q.AirUnNumber;
                //obj.AirPkGroup = q.AirPkGroup;
                //obj.Via = q.Via;
                //obj.ShipWt = q.ShipWt;
                //obj.ShipDimWt = q.ShipDimWt;
                obj.CreateDate = q.CreateDate;
                obj.CreateUser = q.CreateUser;
                obj.UpdateDate = q.UpdateDate;
                obj.UpdateUser = q.UpdateUser;
                                
                obj.StatusID = null;
                obj.ListOfStatusNotesIDs = fnListOfStatusNotesIDs();

                // Look for a reason to set the item R/O
                if (q.ShipDate != null)
                {
                    obj.CrudMode = "RO";
                }
                
                return obj;
            }
        }

        public static void fnDeleteOrderItem(int id)
        {
            System.Threading.Thread.Sleep(1000);
            using (var db = new EF.CMCSQL03Entities())
            {
                var qry = (from t in db.tblOrderItem where t.ItemID == id select t).FirstOrDefault();
                
                if(qry != null)
                {
                    string s = @"Delete from tblOrderItem where ItemID=" + id.ToString();
                    db.Database.ExecuteSqlCommand(s);
                }
            }
        }

        public static int fnAllocateItem(int id)
        {
            // id= ItemId, but return the orderid
            // TOSO refine the logic
            using (var db = new EF.CMCSQL03Entities())
            {
                var dbitem = (from t in db.tblOrderItem where t.ItemID == id select t).FirstOrDefault();
                // if not already allocated
                if (!String.IsNullOrEmpty(dbitem.AllocateStatus))
                {
                    var dbstock = (from t in db.tblStock where t.ShelfID == dbitem.ShelfID && (t.QtyOnHand - t.QtyAllocated >= dbitem.Qty) && t.ShelfStatus == "AVAIL" select t).FirstOrDefault();
                    if (dbstock != null)
                    {
                        dbstock.QtyAllocated = dbstock.QtyAllocated + dbitem.Qty;
                        dbitem.AllocatedStockID = dbstock.StockID;
                        dbitem.AllocateStatus = "A";
                        db.SaveChanges();
                    }
                }
                return Convert.ToInt32(dbitem.OrderID);
            }
        }


        public static OrderTrans fnCreateTrans(int id)
        {
            // id=orderid
            using (var db = new EF.CMCSQL03Entities())
            {
                var vm = new OrderTrans();
                vm.ordertransid = -1;
                vm.orderid = id;
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
                vm.createdate=qry.CreateDate;
                vm.createuser=qry.CreateUser;
                vm.updatedate=qry.UpdateDate;
                vm.updateuser=qry.UpdateUser;

                vm.ListOfOrderTransTypes = fnListOfOrderTransTypes();
                return vm;
            }

        }

        //public static void fnSaveSystemTransaction(OrderTrans vm)
        //{
        //    using (var db = new EF.CMCSQL03Entities())
        //    {
        //        EF.tblOrderTrans newrec = new EF.tblOrderTrans();
        //        newrec.OrderItemID = vm.orderitemid;
        //        newrec.OrderID = vm.orderid;
        //        newrec.ClientID = vm.clientid;
        //        newrec.CreateUser = "System";
        //        newrec.CreateDate = System.DateTime.Now;
        //        newrec.TransType = vm.transtype;
        //        newrec.TransQty = vm.transqty;
        //        newrec.TransAmount = vm.transamount;
        //        db.tblOrderTrans.Add(newrec);
        //        db.SaveChanges();
        //    }

        
        //}


        public static void fnGenerateOrderTransactions(int id)
        {
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
                var tier = (from t in db.tblTier where t.ClientID == o.ClientID && t.Size==oi.Size && t.Tier=="1" select t).FirstOrDefault();
                if (tier != null)
                {
                    EF.tblOrderTrans newrec = new EF.tblOrderTrans();
                    newrec.TransDate = System.DateTime.Now;
                    newrec.OrderItemID = oi.ItemID;
                    newrec.OrderID = oi.OrderID;
                    newrec.ClientID = o.ClientID;
                    newrec.TransType = "SAMP";
                    newrec.TransQty = oi.Qty;
                    newrec.TransAmount = tier.Price;
                    newrec.CreateDate = System.DateTime.Now;
                    newrec.CreateUser = "System";
                    db.tblOrderTrans.Add(newrec);
                    db.SaveChanges();
                }
                else
                {
                    EF.tblOrderTrans newrec = new EF.tblOrderTrans();
                    newrec.TransDate = System.DateTime.Now;
                    newrec.OrderItemID = oi.ItemID;
                    newrec.OrderID = oi.OrderID;
                    newrec.ClientID = o.ClientID;
                    newrec.TransType = "SAMP";
                    newrec.TransQty = oi.Qty;
                    newrec.TransAmount = tier.Price;
                    newrec.Comments = "No Tier 1 price found";
                    newrec.CreateDate = System.DateTime.Now;
                    newrec.CreateUser = "System";
                    db.tblOrderTrans.Add(newrec);
                    db.SaveChanges();
                }

                // Other charges from shelfmaster
                var sm = (from t in db.tblShelfMaster where t.ShelfID == oi.ShelfID select t).FirstOrDefault();
                var qrySurcharges = (from t in db.tblSurcharge where t.ClientID == o.ClientID select t).FirstOrDefault();

                if(sm.HazardSurcharge==true)
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

                if(sm.FlammableSurcharge==true)
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

                if(sm.HeatSurcharge==true)
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
                
                if(sm.RefrigSurcharge==true)
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
      
                if(sm.FreezerSurcharge==true)
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

                if(sm.CleanSurcharge==true)
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

                if(sm.BlendSurcharge==true)
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

                if(sm.NalgeneSurcharge==true)
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

                if(sm.NitrogenSurcharge==true)
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
                
                if(sm.BiocideSurcharge==true)
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

                if(sm.KosherSurcharge==true)
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

                if(sm.LabelSurcharge==true)
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

                if(sm.OtherSurcharge==true)
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


        //public static int NewPK(string sql)
        //{
        //    int recs = 0;
        //    sql = sql + ";SELECT SCOPE_IDENTITY();";
        //    System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection();
        //    conn.ConnectionString = connstring();
        //    System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(sql, conn);
        //    conn.Open();
        //    recs = Convert.ToInt32(cmd.ExecuteScalar());
        //    conn.Close();
        //    return recs;
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
                           select new SelectListItem{Value=t.Division,Text=t.Division}).Distinct().ToList();
                           mylist.Insert(0, new SelectListItem { Value = "", Text = "" });
                return mylist;
            }
        }

        

        //public static string ClientNameForDisplay(int? id)
        //{
        //    using (var db = new EF.CMCSQL03Entities())
        //    {
        //        string s = (from t in db.tblClient
        //                    where t.ClientID == id
        //                    select t.ClientName).FirstOrDefault();
        //        return s;
        //    }
        //}

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
                mylist.Insert(0, new SelectListItem { Value = "0", Text = " -- Size --" });
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
                db.Dispose();
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
                          where m.ClientID==id 
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