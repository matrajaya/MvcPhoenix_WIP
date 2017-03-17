using System;
using System.Linq;
using System.Web;

namespace MvcPhoenix.Services
{
    public class OrderImportService
    {
        public static MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();

        public static void PrepareForImport()
        {
            // this method tries to put the external record into Phoenix normalized fashion and tag for actual import

            using (var db = new EF.CMCSQL03Entities())
            {
                db.Database.ExecuteSqlCommand("DELETE FROM tblOrderImport WHERE Status NOT IN('0')");

                var q = (from t in db.tblOrderImport 
                         where t.ImportStatus == null && t.Location_MDB == "EU"
                         select t).ToList();

                foreach (var r in q)
                {
                    r.ImportStatus = "FAIL";

                    var qclt = (from t in db.tblClient 
                                where t.CMCLongCustomer == r.Company_MDB && t.CMCLocation == r.Location_MDB 
                                select t).FirstOrDefault();

                    if (qclt == null)
                    {
                        r.ImportError += " [ClientID]";
                    }

                    if (qclt != null)
                    {
                        r.ClientID = qclt.ClientID;
                        var qdiv = (from t in db.tblDivision 
                                    where t.ClientID == r.ClientID && t.DivisionName == r.Division_MDB 
                                    select t).FirstOrDefault();

                        if (qdiv != null)
                        {
                            r.DivisionID = qdiv.DivisionID;
                        }
                    }

                    // will this always return one row ?????
                    var qPD = (from pd in db.tblProductDetail
                               join pm in db.tblProductMaster on pd.ProductMasterID equals pm.ProductMasterID
                               where pd.ProductCode == r.ProductCode && pm.ClientID == r.ClientID
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
                                   where t.ProductDetailID == r.ProductDetailID && t.Size == r.Size 
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
                                          where t.ProductDetailID == r.ProductDetailID && t.Size == "1SR" 
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

            using (var db = new EF.CMCSQL03Entities())
            {
                int OrdersImportedCount = 0;

                // if ImportStatus=GOOD we can create VM objects as if it were data entry

                var qGUIDs = (from t in db.tblOrderImport
                              where t.ImportStatus == "PASS" && t.Location_MDB == sLocation
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
                    //NewOrder.spstaxid = qOM.SPSTaxID;
                    //NewOrder.spscurrency = qOM.SPSCurrency;
                    //NewOrder.spsshippedwt = qOM.SPSShippedWt;
                    //NewOrder.spsfreightcost = qOM.SPSFreightCost;
                    //NewOrder.invoicecompany = qOM.InvoiceCompany;
                    //NewOrder.invoicetitle = qOM.InvoiceTitle;
                    //NewOrder.invoicefirstname = qOM.InvoiceFirstName;
                    //NewOrder.invoicelastname = qOM.InvoiceLastName;
                    //NewOrder.invoiceaddress1 = qOM.InvoiceAddress1;
                    //NewOrder.invoiceaddress2 = qOM.InvoiceAddress2;
                    //NewOrder.invoiceaddress3 = qOM.InvoiceAddress3;
                    //NewOrder.invoicecity = qOM.InvoiceCity;
                    //NewOrder.invoicestateprov = qOM.InvoiceStateProv;
                    //NewOrder.invoicepostalcode = qOM.InvoicePostalCode;
                    //NewOrder.invoicecountry = qOM.InvoiceCountry;
                    //NewOrder.invoicephone = qOM.InvoicePhone;
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
                    NewOrder.orderid = -1; // needed to do insert
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
                        if (i.ShelfID == null)
                        {
                            i.ShelfID = 99;                     // needed for post save to pull SR
                            NewItem.SRSize = i.SRSize;
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

                        // save tblOrderItem to DB
                        int NewItemID = Services.OrderService.fnSaveItem(NewItem);
                    }
                    OrdersImportedCount = OrdersImportedCount + 1;
                    
                    // at this point we have an order
                    string s = String.Format("UPDATE tblOrderImport SET ImportStatus='IMPORTED' WHERE GUID='{0}'", r.GUID);
                    db.Database.ExecuteSqlCommand(s);
                }

                return OrdersImportedCount;
            }
        }
    }
}