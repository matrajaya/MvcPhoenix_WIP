using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using MvcPhoenix.Models;

namespace MvcPhoenix.Services
{
    public class OrderService
    {
        public static OrderTrans fnFillOrderTrans(int pk)
        {
            OrderTrans obj = new OrderTrans();
            using (var db = new EF.CMCSQL03Entities())
            {
                var qry = (from t in db.tblOrderTrans
                           where t.OrderTransID == pk
                           select t).FirstOrDefault();
                obj.pagemode = "";
                obj.ordertransid = qry.OrderTransID;
                obj.clientid = qry.ClientID;
                obj.orderid = qry.OrderID;
                obj.orderitemid = qry.OrderItemID;
                obj.userid = qry.UserID;
                obj.transtype = qry.TransType;
                obj.transdate = qry.TransDate;
                obj.transqty = qry.TransQty;
                obj.transamount = qry.TransAmount;
                obj.comments = qry.Comments;
            }

            return obj;
        }

        public static int fnAddOrderTrans(OrderTrans o)
        {
            // take a populated obj and insert into table
            using (var db = new EF.CMCSQL03Entities())
            {
                // instance of the EF object NOT the business/Model View object
                var newtrans = new EF.tblOrderTrans
                {
                    OrderTransID = o.ordertransid,
                    ClientID = o.clientid,
                    OrderID = o.orderid,
                    OrderItemID = o.orderitemid,
                    UserID = o.userid,
                    TransType = o.transtype,
                    TransDate = o.transdate,
                    TransQty = o.transqty,
                    TransAmount = o.transamount,
                    Comments = o.comments
                };
                db.tblOrderTrans.Add(newtrans);
                db.SaveChanges();
                // Stash the new PK
                int newpk = newtrans.OrderTransID;
                return newpk;
            }
        }

        public static void fnUpdateOrderTrans(OrderTrans o)
        {
            // take a populated obj and insert into table
            using (var db = new EF.CMCSQL03Entities())
            {
                var qry = (from t in db.tblOrderTrans
                           where t.OrderTransID == o.ordertransid
                           select t).FirstOrDefault();
                //OrderTransID = o.OrderTransID,
                qry.ClientID = o.clientid;
                qry.OrderID = o.orderid;
                qry.OrderItemID = o.orderitemid;
                qry.UserID = o.userid;
                qry.TransType = o.transtype;
                qry.TransDate = o.transdate;
                qry.TransQty = o.transqty;
                qry.TransAmount = o.transamount;
                qry.Comments = o.comments;
                db.SaveChanges();
            }
        }

        public static void fnCreateItemCharges(int pk)
        {
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            string s = "";
            var qryItem = (from t in db.tblOrderItem where t.ItemID == pk select t).FirstOrDefault();
            var qryOrder = (from t in db.tblOrderMaster where t.OrderID == qryItem.OrderID select t).FirstOrDefault();
            var qryTier = (from t in db.tblTier where t.ClientID == qryOrder.ClientID && t.Size == qryItem.Size && t.Tier == "1" select t).FirstOrDefault();

            OrderTrans obj = new OrderTrans();
            // set common values
            obj.clientid = qryOrder.ClientID;
            obj.orderid = qryOrder.OrderID;
            obj.orderitemid = qryItem.ItemID;
            obj.userid = null;
            obj.transdate = DateTime.Now;
            obj.comments = null;

            // Sample charge
            s = "Delete from tblOrderTrans where OrderItemID=" + pk + " and Transtype IN('SAMP')";
            ExecuteADOSQL(s);
            obj.transtype = "SAMP";
            obj.transqty = qryItem.Qty;

            try
            {
                obj.transamount = qryTier.Price;
            }
            catch
            {
                obj.transamount = 0;
                obj.comments = "Missing Tier Record";
            }

            fnAddOrderTrans(obj);

            // surcharges
            var qrySurcharges = (from t in db.tblSurcharge where t.ClientID == qryOrder.ClientID select t).FirstOrDefault();
            var qrySampSize = (from t in db.tblSampSize where t.ProfileID == qryItem.ProfileID && t.Size == qryItem.Size select t).FirstOrDefault();

            if (qrySampSize.HazardSurcharge == true)
            {
                ExecuteADOSQL("Delete from tblOrderTrans where OrderItemID=" + pk + " and Transtype='HAZD'");
                obj.transtype = "HAZD";
                obj.transqty = qryItem.Qty;
                obj.transamount = qrySurcharges.Haz ?? 0;
                fnAddOrderTrans(obj);
            }

            if (qrySampSize.FlammableSurcharge == true)
            {
                ExecuteADOSQL("Delete from tblOrderTrans where OrderItemID=" + pk + " and Transtype='FLAM'");
                obj.transtype = "FLAM";
                obj.transqty = qryItem.Qty;
                obj.transamount = qrySurcharges.Flam;
                fnAddOrderTrans(obj);
            }

            if (qrySampSize.CleanSurcharge == true)
            {
                ExecuteADOSQL("Delete from tblOrderTrans where OrderItemID=" + pk + " and Transtype='CLEN'");
                obj.transtype = "CLEN";
                obj.transqty = qryItem.Qty;
                obj.transamount = qrySurcharges.Clean ?? 0;
                fnAddOrderTrans(obj);
            }

            if (qrySampSize.HeatSurcharge == true)
            {
                ExecuteADOSQL("Delete from tblOrderTrans where OrderItemID=" + pk + " and Transtype='HEAT'");
                obj.transtype = "HEAT";
                obj.transqty = qryItem.Qty;
                obj.transamount = qrySurcharges.Heat ?? 0;
                fnAddOrderTrans(obj);
            }

            if (qrySampSize.RefrigeratorSurcharge == true)
            {
                ExecuteADOSQL("Delete from tblOrderTrans where OrderItemID=" + pk + " and Transtype='REFR'");
                obj.transtype = "REFR";
                obj.transqty = qryItem.Qty;
                obj.transamount = qrySurcharges.Refrig ?? 0;
                fnAddOrderTrans(obj);
            }
            if (qrySampSize.FreezerSurcharge == true)
            {
                ExecuteADOSQL("Delete from tblOrderTrans where OrderItemID=" + pk + " and Transtype='FREZ'");
                obj.transtype = "FREZ";
                obj.transqty = qryItem.Qty;
                obj.transamount = qrySurcharges.Freezer ?? 0;
                fnAddOrderTrans(obj);
            }

            if (qrySampSize.NalgeneSurcharge == true)
            {
                ExecuteADOSQL("Delete from tblOrderTrans where OrderItemID=" + pk + " and Transtype='NALG'");
                obj.transtype = "NALG";
                obj.transqty = qryItem.Qty;
                obj.transamount = qrySurcharges.Nalgene ?? 0;
                fnAddOrderTrans(obj);
            }

            if (qrySampSize.LabelSurcharge == true)
            {
                ExecuteADOSQL("Delete from tblOrderTrans where OrderItemID=" + pk + " and Transtype='LABL'");
                obj.transtype = "LABL";
                obj.transqty = qryItem.Qty;
                obj.transamount = qrySurcharges.LabelFee ?? 0;
                fnAddOrderTrans(obj);
            }

            if (qrySampSize.OtherSurcharge == true)
            {
                ExecuteADOSQL("Delete from tblOrderTrans where OrderItemID=" + pk + " and Transtype='MISC'");
                obj.transtype = "MISC";
                obj.transqty = qryItem.Qty;
                obj.transamount = qrySampSize.OtherSurchargeAmt ?? 0;
                fnAddOrderTrans(obj);
            }

            db.Dispose();
        }

        public static void fnDeleteOrderItem(int ItemID)
        {
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            // archive the record, then delete it
            string s = @"Delete from tblOrderItem where ItemID=" + ItemID.ToString();
            db.Database.ExecuteSqlCommand(s);
            db.Dispose();
        }

        public static Boolean IsValidOrderItem(OrderItem incoming)
        {
            bool retval = true;
            if (incoming.ProfileID == 0) { retval = false; }
            if (incoming.Size == "Size") { retval = false; }
            if (incoming.Size == "0") { retval = false; }
            if (incoming.Qty == 0) { retval = false; }
            return retval;
        }

        public static int fnInsertOrderItem(OrderItem incoming)
        {
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            int pk = incoming.ItemID;
            int retval = 0;

            var qryProfile = (from t in db.tblProfile
                              where t.ProfileID == incoming.ProfileID
                              select t).FirstOrDefault();

            var newitem = new EF.tblOrderItem
            {
                ProductCode = qryProfile.ProductCode,
                ProductName = qryProfile.ProductName,
                OrderID = incoming.OrderID,
                ProfileID = incoming.ProfileID,
                Size = incoming.Size,
                SRSize = incoming.SRSize,
                Qty = incoming.Qty,
                LotNumber = incoming.LotNumber,
                NonCMCDelay = incoming.NonCMCDelay,
                CarrierInvoiceRcvd = incoming.CarrierInvoiceRcvd,
                Status = incoming.Status
            };
            db.tblOrderItem.Add(newitem);
            db.SaveChanges();
            retval = newitem.ItemID;
            db.Dispose();
            fnAddToItemStatus(retval, Convert.ToInt32(incoming.StatusID));
            MvcPhoenix.Services.OrderService.fnCreateItemCharges(retval);
            return retval;
        }

        public static int fnUpdateOrderItem(OrderItem incoming)
        {
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            int pk = incoming.ItemID;

            var qryProfile = (from t in db.tblProfile
                              where t.ProfileID == incoming.ProfileID
                              select t).FirstOrDefault();

            var qry = (from t in db.tblOrderItem where t.ItemID == pk select t).FirstOrDefault();
            qry.ProductCode = qryProfile.ProductCode; qry.ProductName = qryProfile.ProductName;
            qry.OrderID = incoming.OrderID; qry.ProfileID = incoming.ProfileID; qry.Size = incoming.Size;
            qry.SRSize = incoming.SRSize; qry.Qty = incoming.Qty; qry.LotNumber = incoming.LotNumber;
            qry.NonCMCDelay = incoming.NonCMCDelay; qry.CarrierInvoiceRcvd = incoming.CarrierInvoiceRcvd;
            qry.Status = incoming.Status;
            db.SaveChanges();
            fnAddToItemStatus(pk, Convert.ToInt32(incoming.StatusID));
            MvcPhoenix.Services.OrderService.fnCreateItemCharges(pk);
            return pk;
        }

        public static void fnAddToItemStatus(int pk, int StatusID)
        {
            // Take a value from tblStatusNotes and append to tblOrderItem record
            if (StatusID > 0)
            {
                MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
                var qryLookup = (from t in db.tblStatusNotes
                                 where t.StatusNotesID == StatusID
                                 select t).FirstOrDefault();
                var qry = (from t in db.tblOrderItem
                           where t.ItemID == pk
                           select t).FirstOrDefault();
                string s = "";
                s = "Update tblOrderItem set Status = Cast( ISNULL(Status,'') as nvarchar(1000))";
                s = s + " + '\r' + ";
                s = s + " cast('" + qryLookup.Note + "'" + " as varchar(1000))";
                s = s + " Where ItemID=" + pk;
                System.Diagnostics.Debug.WriteLine(s);
                ExecuteADOSQL(s);
            }
        }

        public static OrderItem fnFillOrderItem(int id)
        {
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            OrderItem o = new OrderItem();
            var q = (from t in db.tblOrderItem
                     where t.ItemID == id
                     select t).FirstOrDefault();

            // Primary fields ***********************
            o.OrderID = q.OrderID;
            o.ItemID = q.ItemID;
            o.OrderID = q.OrderID;
            o.ProfileID = q.ProfileID;
            o.Size = q.Size;
            o.SRSize = q.SRSize;
            o.Qty = q.Qty;
            o.LotNumber = q.LotNumber;
            o.NonCMCDelay = Convert.ToBoolean(q.NonCMCDelay);
            o.CarrierInvoiceRcvd = Convert.ToBoolean(q.CarrierInvoiceRcvd);
            o.Status = q.Status;

            return o;
        }

        public static void fnArchiveOrderMaster(int pk)
        {
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            string s = "Insert Into tblOrderMasterArchive(";
            s = s + "[ClientID],[OrderID],[Customer],[CMCORDER],[WebOrderID],[CMCLEGACYNUM],[CUSTORDNUM],[CUSTSAPNUM],[CUSTREFNUM],[ShipRef],[ORDERTYPE],[ORDERDATE],[COMPANY]";
            s = s + ",[STREET],[STREET2],[STREET3],[CITY],[STATE],[ZIP],[country],[ATTENTION],[EMAIL],[SALESREP],[SALESEmail],[REQ],[REQPHONE],[REQfax]";
            s = s + ",[REQemail],[EndUse],[ShipVia],[ShipAcct],[Phone],[Source],[Fax],[Tracking],[Special],[SpecialInternal],[Lit],[Region],[COA],[TDS]";
            s = s + ",[CID],[CustACCT],[ACode],[ImportFile],[Importdateline],[Timing],[Volume],[SampleRack],[CMCUser],[CustomerReference],[DIVISION]";
            s = s + ",[BusARea],[TotalOrderWeight],[SPSTaxID],[SPSCurrency],[SPSShippedWt],[SPSFreightCost],[InvoiceCompany],[InvoiceTitle]";
            s = s + ",[InvoiceFirstName],[InvoiceLastName],[InvoiceAddress1],[InvoiceAddress2],[InvoiceAddress3],[InvoiceCity],[InvoiceStateProv]";
            s = s + ",[InvoicePostalCode],[InvoiceCountry],[InvoicePhone],[CustOrderType],[LegacyID]";
            s = s + ",[CustRequestDate],[ApprovalDate],[RequestedDeliveryDate],[CustTotalItems],[CustRequestedCarrier])";
            s = s + "Select ";
            s = s + "[ClientID],[OrderID],[Customer],[CMCORDER],[WebOrderID],[CMCLEGACYNUM],[CUSTORDNUM],[CUSTSAPNUM],[CUSTREFNUM],[ShipRef],[ORDERTYPE],[ORDERDATE],[COMPANY]";
            s = s + ",[STREET],[STREET2],[STREET3],[CITY],[STATE],[ZIP],[country],[ATTENTION],[EMAIL],[SALESREP],[SALESEmail],[REQ],[REQPHONE],[REQfax]";
            s = s + ",[REQemail],[EndUse],[ShipVia],[ShipAcct],[Phone],[Source],[Fax],[Tracking],[Special],[SpecialInternal],[Lit],[Region],[COA],[TDS]";
            s = s + ",[CID],[CustACCT],[ACode],[ImportFile],[Importdateline],[Timing],[Volume],[SampleRack],[CMCUser],[CustomerReference],[DIVISION]";
            s = s + ",[BusARea],[TotalOrderWeight],[SPSTaxID],[SPSCurrency],[SPSShippedWt],[SPSFreightCost],[InvoiceCompany],[InvoiceTitle]";
            s = s + ",[InvoiceFirstName],[InvoiceLastName],[InvoiceAddress1],[InvoiceAddress2],[InvoiceAddress3],[InvoiceCity],[InvoiceStateProv]";
            s = s + ",[InvoicePostalCode],[InvoiceCountry],[InvoicePhone],[CustOrderType],[LegacyID]";
            s = s + ",[CustRequestDate],[ApprovalDate],[RequestedDeliveryDate],[CustTotalItems],[CustRequestedCarrier]";
            s = s + " From tblOrderMaster Where OrderID=" + pk;

            try
            {
                db.Database.ExecuteSqlCommand(s);
            }
            catch
            {

            }
            finally
            {
                db.Dispose();
            }
        }

        public static int SaveOrderMaster(OrderMasterFull o)
        {
            // DB Update success returns PK, Fail returns 0
            // Calling controller will deal with it
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            int pk = 0;

            if (o.orderid == -1)
            {
                string s = @"Insert into tblOrderMaster (ClientID,OrderDate) VALUES (@clientid,@orderdate)";
                db.Database.ExecuteSqlCommand(s,
                new SqlParameter("clientid", o.clientid),
                new SqlParameter("orderdate", o.orderdate)
                );
                int newpk = (from t in db.tblOrderMaster
                             select t.OrderID).Max();
                pk = newpk;
            }
            else
            {
                pk = o.orderid;
            }

            int retvalue;

            try
            {
                var q = (from t in db.tblOrderMaster
                         where t.OrderID == pk
                         select t).FirstOrDefault();

                q.OrderDate = q.OrderDate;
                q.ClientID = o.clientid; q.Customer = o.customer; q.CMCOrder = o.cmcorder; q.WebOrderID = o.weborderid;
                q.CMCLegacyNum = o.cmclegacynumber; q.CustOrdNum = o.custordnum; q.CustSapNum = o.custsapnum; q.CustRefNum = o.custrefnum;
                q.OrderType = o.ordertype; q.Company = o.company; q.Street = o.street; q.Street2 = o.street2;
                q.Street3 = o.street3; q.City = o.city; q.State = o.state; q.Zip = o.Zip; q.Country = o.country; q.Attention = o.attention;
                q.Email = o.email; q.SalesRep = o.salesrep; q.SalesEmail = o.sales_email; q.Req = o.req; q.ReqPhone = o.reqphone; q.ReqFax = o.reqfax;
                q.ReqEmail = o.reqemail; q.EndUse = o.enduse; q.ShipVia = o.shipvia; q.ShipAcct = o.shipacct; q.Phone = o.phone; q.Source = o.source;
                q.Fax = o.fax; q.Tracking = o.tracking; q.Special = o.special; q.SpecialInternal = o.specialinternal; q.Lit = Convert.ToBoolean(o.lit);
                q.Region = o.region; q.COA = o.coa; q.TDS = o.tds; q.CID = o.cid; q.CustAcct = o.custacct; q.ACode = o.acode; q.ImportFile = o.importfile;
                q.ImportDateLine = o.importdateline; q.Timing = o.timing; q.Volume = o.volume; q.SampleRack = Convert.ToBoolean(o.samplerack); q.CMCUser = o.cmcuser;
                q.CustomerReference = o.customerreference; q.Division = o.division; q.BusArea = o.busarea; q.TotalOrderWeight = (o.totalorderweight); q.SPSTaxID = o.spstaxid;
                q.SPSCurrency = o.spscurrency; q.SPSShippedWt = o.spsshippedwt; q.SPSFreightCost = o.spsfreightcost; q.InvoiceCompany = o.invoicecompany;
                q.InvoiceTitle = o.invoicetitle; q.InvoiceFirstName = o.invoicefirstname; q.InvoiceLastName = o.invoicelastname; q.InvoiceAddress1 = o.invoiceaddress1;
                q.InvoiceAddress2 = o.invoiceaddress2; q.InvoiceAddress3 = o.invoiceaddress3; q.InvoiceCity = o.invoicecity; q.InvoiceStateProv = o.invoicestateprov;
                q.InvoicePostalCode = o.invoicepostalcode; q.InvoiceCountry = o.invoicecountry; q.InvoicePhone = o.invoicephone; q.CustOrderType = o.custordertype;
                q.CustRequestDate = o.custrequestdate; q.ApprovalDate = o.approvaldate; q.RequestedDeliveryDate = o.requesteddeliverydate; q.CustTotalItems = o.custtotalitems;
                q.CustRequestedCarrier = o.custrequestedcarrier; q.LegacyID = (o.legacyid);
                q.SalesRepPhone = o.salesrepphone; q.SalesRepTerritory = o.salesrepterritory; q.MarketingRep = o.marketingrep; q.MarketingRepEmail = o.marketingrepemail;
                q.Distributor = o.distributor; q.PreferredCarrier = o.preferredcarrier; q.ApprovalNeeded = o.approvalneeded;

                db.SaveChanges();
                db.Dispose();
                retvalue = pk;
            }
            catch
            {
                retvalue = 0;
            }
            finally
            {
                db.Dispose();
            }

            return retvalue;
        }

        private void fnClientID1() { }
        private void fnClientID2() { }
        private void fnClientID3() { }

        private void fnAddOrderTaskToQueue()
        {
            // parameters to be determined...
            // Call from a SaveOrder method
        }

        public static OrderMasterFull fillOrderMasterObject(int id)
        {
            // Take an int and return an OrderMaster object for your pleasure
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();

            OrderMasterFull o = new OrderMasterFull();
            var q = (from t in db.tblOrderMaster
                     where t.OrderID == id
                     select t).FirstOrDefault();

            // Fill lists buried in object
            o.ListOfDivisions = fnListOfDivisions(q.ClientID);

            o.orderid = q.OrderID;
            o.clientid = q.ClientID;
            o.customer = q.Customer;
            o.cmcorder = Convert.ToInt32(q.CMCOrder);
            o.weborderid = Convert.ToInt32(q.WebOrderID);
            o.cmclegacynumber = q.CMCLegacyNum;
            o.custordnum = q.CustOrdNum;
            o.custsapnum = q.CustSapNum;
            o.custrefnum = q.CustRefNum;
            o.ordertype = q.OrderType;

            if (q.OrderDate.HasValue)
            { o.orderdate = Convert.ToDateTime(q.OrderDate); }
            else
            { o.orderdate = null; }

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
            o.customerreference = q.CustomerReference; o.division = q.Division; o.busarea = q.BusArea; o.totalorderweight = q.TotalOrderWeight;
            o.spstaxid = q.SPSTaxID; o.spscurrency = q.SPSCurrency; o.spsshippedwt = q.SPSShippedWt; o.spsfreightcost = q.SPSFreightCost;
            o.invoicecompany = q.InvoiceCompany; o.invoicetitle = q.InvoiceTitle; o.invoicefirstname = q.InvoiceFirstName; o.invoicelastname = q.InvoiceLastName;
            o.invoiceaddress1 = q.InvoiceAddress1; o.invoiceaddress2 = q.InvoiceAddress2; o.invoiceaddress3 = q.InvoiceAddress3; o.invoicecity = q.InvoiceCity;
            o.invoicestateprov = q.InvoiceStateProv; o.invoicepostalcode = q.InvoicePostalCode; o.invoicecountry = q.InvoiceCountry; o.invoicephone = q.InvoicePhone;
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

            return o;
        }

        private static MvcPhoenix.EF.CMCSQL03Entities dbnew = new MvcPhoenix.EF.CMCSQL03Entities();

        private static string connstring()
        {
            string s = System.Configuration.ConfigurationManager.ConnectionStrings["ADOConnectionString"].ConnectionString;
            return s;
        }

        public static int NewPK(string sql)
        {
            int recs = 0;
            sql = sql + ";SELECT SCOPE_IDENTITY();";
            System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection();
            conn.ConnectionString = connstring();
            System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(sql, conn);
            conn.Open();
            recs = Convert.ToInt32(cmd.ExecuteScalar());
            conn.Close();
            return recs;
        }

        public static int ExecuteADOSQL(string sql)
        {
            dbnew.Database.ExecuteSqlCommand(sql);
            return 1;
        }

        public static string ClientNameForDisplay(int? id)
        {
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            string s = (from t in db.tblClient
                        where t.ClientID == id
                        select t.ClientName).FirstOrDefault();
            db.Dispose();
            return s;
        }

        public static List<OrderItem> fnListOfOrderItemsForOrderID(long OrderID)
        {
            List<OrderItem> mylist = new List<OrderItem>();
            var qry = (from t in dbnew.tblOrderItem
                       where t.OrderID == OrderID
                       select t).ToList();
            dbnew.Dispose();
            return mylist;
        }

        public static List<SelectListItem> fnProductCodeSizes(int myprofileid)
        {
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            List<SelectListItem> mylist = new List<SelectListItem>();

            mylist = (from t in db.tblSampSize
                      where t.ProfileID == myprofileid
                      orderby t.ProductCode
                      select
                        new SelectListItem { Value = t.Size, Text = t.Size }).ToList();
            mylist.Insert(0, new SelectListItem { Value = "0", Text = " -- Size --" });
            db.Dispose();
            return mylist;

        }

        public static List<SelectListItem> fnProductCodeXref(int? clientid)
        {
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            List<SelectListItem> mylist = new List<SelectListItem>();

            mylist = (from t in db.tblProductXRef
                      where t.ClientID == clientid
                      orderby t.CustProductCode
                      select
                      new SelectListItem { Value = t.CustProductCode, Text = t.CustProductCode + " : " + t.CMCProductCode }).ToList();
            mylist.Insert(0, new SelectListItem { Value = "0", Text = " -- Cust Product Code -- " });
            db.Dispose();
            return mylist;

        }

        public static List<SelectListItem> fnListOfOrderIDs()
        {
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            // How do you do a top 30 select ?????
            List<SelectListItem> mylist = new List<SelectListItem>();

            mylist = (from t in db.tblOrderMaster
                      orderby t.OrderID descending
                      select
                          new SelectListItem { Value = t.OrderID.ToString(), Text = t.OrderID.ToString() }).ToList();
            mylist.Insert(0, new SelectListItem { Value = "0", Text = "Please Select" });
            db.Dispose();
            return mylist;
        }

        public static List<SelectListItem> fnListOfClientIDs()
        {
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            List<SelectListItem> mylist = new List<SelectListItem>();
            mylist = (from t in db.tblClient
                      orderby t.ClientName
                      select
                          new SelectListItem { Value = t.ClientID.ToString(), Text = t.ClientName }).ToList();
            mylist.Insert(0, new SelectListItem { Value = "0", Text = "Select Client" });
            db.Dispose();
            return mylist;
        }

        public static List<SelectListItem> fnListOfOrderTypes()
        {
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            List<SelectListItem> mylist = new List<SelectListItem>();
            mylist = (from t in db.tblOrderType
                      orderby t.OrderType
                      select
                          new SelectListItem { Value = t.OrderType, Text = t.OrderType }).ToList();
            mylist.Insert(0, new SelectListItem { Value = "0", Text = "Please Select" });
            db.Dispose();
            return mylist;
        }

        public static List<SelectListItem> fnListOfCountries()
        {
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            List<SelectListItem> mylist = new List<SelectListItem>();
            mylist = (from t in db.tblCountry
                      orderby t.Country
                      select
                          new SelectListItem { Value = t.Country, Text = t.Country }).ToList();
            mylist.Insert(0, new SelectListItem { Value = "0", Text = "Please Select" });
            db.Dispose();
            return mylist;
        }

        public static List<SelectListItem> fnListOfSalesReps(int myclientid)
        {
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            List<SelectListItem> mylist = new List<SelectListItem>();
            mylist = (from t in db.tblClientContact
                      where t.ContactType == "SalesRep"
                      where t.ClientID.Equals(myclientid)
                      orderby t.FullName
                      select
                          new SelectListItem { Value = t.FullName, Text = t.FullName }).ToList();
            mylist.Insert(0, new SelectListItem { Value = "0", Text = "Please Select" });
            db.Dispose();
            return mylist;
        }

        public static List<SelectListItem> fnListOfRequestors(int myclientid)
        {
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            List<SelectListItem> mylist = new List<SelectListItem>();
            mylist = (from t in db.tblClientContact
                      where t.ContactType == "Requestor"
                      where t.ClientID == myclientid
                      orderby t.FullName
                      select
                          new SelectListItem { Value = t.FullName, Text = t.FullName }).ToList();
            mylist.Insert(0, new SelectListItem { Value = "0", Text = "Please Select" });
            db.Dispose();
            return mylist;
        }

        public static List<SelectListItem> fnListOfEndUses(int? myclientid)
        {
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            List<SelectListItem> mylist = new List<SelectListItem>();
            mylist = (from t in db.tblEndUse
                      where t.ClientID == myclientid
                      orderby t.EndUse
                      select
                          new SelectListItem { Value = t.EndUse, Text = t.EndUse }).ToList();
            mylist.Insert(0, new SelectListItem { Value = "0", Text = "Please Select" });
            db.Dispose();
            return mylist;
        }

        public static List<SelectListItem> fnListOfShipVias()
        {
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            List<SelectListItem> mylist = new List<SelectListItem>();
            mylist = (from t in db.tblCarrier
                      orderby t.CarrierName
                      select
                          new SelectListItem { Value = t.CarrierName, Text = t.CarrierName }).ToList();
            mylist.Insert(0, new SelectListItem { Value = "0", Text = "Please Select" });
            db.Dispose();
            return mylist;
        }

        public static List<SelectListItem> fnListOfOrderSources()
        {
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            List<SelectListItem> mylist = new List<SelectListItem>();
            mylist = (from t in db.tblOrderSource
                      orderby t.Source
                      select
                          new SelectListItem { Value = t.Source, Text = t.Source }).ToList();
            mylist.Insert(0, new SelectListItem { Value = "0", Text = "Please Select" });
            db.Dispose();
            return mylist;
        }

        public static List<SelectListItem> fnListOfDivisions(int? myclientid)
        {
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            List<SelectListItem> mylist = new List<SelectListItem>();
            mylist = (from t in db.tblDivision
                      where t.ClientID == myclientid
                      orderby t.Division
                      select
                          new SelectListItem { Value = t.Division, Text = t.Division }).ToList();
            mylist.Insert(0, new SelectListItem { Value = "0", Text = "Please Select" });
            db.Dispose();
            return mylist;
        }

        public static List<SelectListItem> fnListOfProductCodes(int myclientid)
        {
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            List<SelectListItem> mylist = new List<SelectListItem>();
            mylist = (from t in db.tblProfile
                      where t.ClientID == myclientid
                      orderby t.ProductCode
                      select
                          new SelectListItem { Value = t.ProductCode, Text = t.ProductCode + " " + t.ProductName }).ToList();
            mylist.Insert(0, new SelectListItem { Value = "0", Text = "Please Select" });
            db.Dispose();
            return mylist;
        }

        public static List<SelectListItem> fnListOfShipViasItemLevel()
        {
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            List<SelectListItem> mylist = new List<SelectListItem>();
            mylist = (from t in db.tblCarrier
                      orderby t.CarrierName
                      select
                      new SelectListItem { Value = t.CarrierName, Text = t.CarrierName }).ToList();
            mylist.Insert(0, new SelectListItem { Value = "0", Text = "Please Select" });
            db.Dispose();
            return mylist;
        }

        public static List<SelectListItem> fnListOfStatuses()
        {
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            List<SelectListItem> mylist = new List<SelectListItem>();
            mylist.Add(new SelectListItem { Value = "S1", Text = "S1" });
            mylist.Add(new SelectListItem { Value = "S2", Text = "S2" });
            mylist.Add(new SelectListItem { Value = "S3", Text = "S3" });
            mylist.Insert(0, new SelectListItem { Value = "0", Text = "Please Select" });
            return mylist;
        }
    }
}