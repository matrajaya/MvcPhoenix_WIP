﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcPhoenix.Models
{
    public class InvoiceService
    {
        public static InvoiceViewModel FillInvoice(InvoiceViewModel model)
        {
            model.invoiceid = 1000556;
            model.billinggroup = "ALL";
            model.warehouselocation = "CT";
            model.clientid = 123456;
            model.clientname = "Ashland";
            model.createdby = "You";
            model.createddate = System.DateTime.Now;
            model.verifieddate = System.DateTime.Now;
            model.status = "Pending Review";
            model.invoicedate = System.DateTime.Now;
            model.invoiceperiod = "July, 2016"; //extract month and year from invoice date conver tto string
            // model.periodmonth = "July";
            // model.periodyear = "2016";
            model.ponumber = 14562654;
            model.netterm = 60;
            model.billto = "Ashland, Inc<br>Suite 400<br>Roseland, New Jersey 07068<br>Attn: M. Feeney - Manager, Sales Service";
            model.remitto = "Chemical Marketing Concepts, LLC <br>c/o Odyssey Logistics & Technology Corp <br>39 Old Ridgebury Road, N-1 <br>Danbury, CT 06810";
            model.currency = "EUR";        // USD / EUR / CNY
            model.tier = 1;
            model.ordertype = "Samples"; // Sample/International/Revenue does not apply to all clients
           
            // Shipping Performance
            model.sampleshipsameday = 50;
            model.sampleshipnextday = 30;
            model.sampleshipsecondday = 20;
            model.sampleshipother = 10;
            
            // Invoice Summary
            model.totalsamples = 100;
            model.totalcostsamples = 100;
            model.totalfreight = 100;
            model.totalfrtHzdSchg = 100;
            model.totaladmincharge = 100;
            model.totaldue = model.totalcostsamples + model.totalfreight + model.totalfrtHzdSchg + model.totaladmincharge;
             
            // Billing Worksheet
            model.grandtotal = 0;

            model.samplesqty = 1;
            model.samplesrate = 1;
            model.samplescharge = model.samplesqty * model.samplesrate;

            model.revenueqty = 1;
            model.revenuerate = 1;
            model.revenuecharge = model.revenueqty * model.revenueqty;

            model.nonrevenueqty = 1;
            model.nonrevenuerate = 1;
            model.nonrevenuecharge = model.nonrevenueqty * model.nonrevenueqty;

            model.manualentryqty = 10;
            model.manualentryrate = 5;
            model.manualentrycharge = model.manualentryqty * model.manualentryrate;

            model.followupqty = 10;
            model.followuprate = 5;
            model.followupcharge = model.followupqty * model.followuprate;

            model.labelprtqty = 10;
            model.labelprtrate = 10;
            model.labelprtcharge = model.labelprtqty * model.labelprtrate;

            model.labelstock = 100;

            model.relabelprtqty = 10;
            model.relabelprtrate = 10;
            model.relabelprtcharge = model.relabelprtqty * model.relabelprtrate;

            model.relabelfeeqty = 10;
            model.relabelfeerate = 5;
            model.relabelfeecharge = model.relabelfeeqty * model.relabelfeerate;

            model.productsetupqty = 10;
            model.productsetuprate = 5;
            model.productsetupcharge = model.productsetupqty * model.productsetuprate;

            model.ccprocessqty = 10;
            model.ccprocessrate = 5;
            model.ccprocesscharge = model.ccprocessqty * model.ccprocessrate;

            model.cccredit = -100;
            model.globalprocesscharge = 100;            
            model.misccreditcharge = 10;
            model.itsupportcharge = 100;
            model.emptydrumcharge = 100;

            model.rushshipqty = 100;
            model.rushshiprate = 5;
            model.rushshipcharge = model.rushshipqty * model.rushshiprate;

            model.emptypailsqty = 10;
            model.emptypailsrate = 5;
            model.emptypailscharge = model.emptypailsqty * model.emptypailsrate;
            
            model.emptypailsfgtcharge = 100;
            
            model.inactivestockqty = 10;
            model.inactivestockrate = 20;
            model.inactivestockcharge = model.inactivestockqty * model.inactivestockrate;

            model.hm181pkgcharge = 100;
            model.dochandlingcharge = 100;
            model.minimalsamplecharge = 100.00;

            return model;
        }
    }
}