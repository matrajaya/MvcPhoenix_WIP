using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcPhoenix.Models
{
    public class InvoiceService
    {
        public static InvoiceViewModel FillInvoice(InvoiceViewModel model)
        {
            model.createddate = System.DateTime.UtcNow;
            model.createdby = "You";
            model.verifieddate = System.DateTime.UtcNow;
            model.status = "Pending Review";
            model.clientname = "Ashland";
            model.billinggroup = "ALL";
            model.periodmonth = "July";
            model.periodyear = "2016";

            model.invoiceid = 1000556;
            model.invoicedate = System.DateTime.UtcNow;
            model.ponumber = 14562654;
            model.netterm = 60;

            model.billto = "Ashland, Inc<br>Suite 400<br>Roseland, New Jersey 07068<br>Attn: M. Feeney - Manager, Sales Service";
            
            //model.billtoaddress = "Suite 400 <br>Roseland, New Jersey 07068";
            //model.billtoattention = "Attn: M. Feeney - Manager, Sales Service";

            model.remitto = "Chemical Marketing Concepts, LLC <br>c/o Odyssey Logistics & Technology Corp <br>39 Old Ridgebury Road, N-1 <br>Danbury, CT 06810";

            model.totalsamples = 100;
            model.totalcostsampleprocessing = 100;
            model.totalfreight = 100;
            model.totalfreightHzdSchg = 100;
            model.totaladmincharge = 100;
            model.totaldue = model.totalcostsampleprocessing + model.totalfreight + model.totalfreightHzdSchg + model.totaladmincharge;
            
            model.sampleshipsameday = 50;
            model.sampleshipnextday = 30;
            model.sampleshipsecondday = 20;
            model.sampleshipother = 10;
            
            model.grandtotal = model.totaldue;

            model.manualentryqty = 10;
            model.manualentryrate = 5;
            model.manualentrytotal = model.manualentryqty * model.manualentryrate;

            model.followuporderqty = 10;
            model.followuporderrate = 5;
            model.followupordertotal = model.followuporderqty * model.followuporderrate;

            model.relabelfeeqty = 10;
            model.relabelfeerate = 5;
            model.relabelfeetotal = model.relabelfeeqty * model.relabelfeerate;

            model.productsetupqty = 10;
            model.productsetuprate = 5;
            model.productsetuptotal = model.productsetupqty * model.productsetuprate;

            model.ccorderprocessqty = 10;
            model.ccorderprocessrate = 5;
            model.ccorderprocesstotal = model.ccorderprocessqty * model.ccorderprocessrate;

            model.cccredittotal = 100;
            model.globalprocessingfeetotal = 100;
            
            model.misccreditqty = 10;
            model.misccreditrate = 5;
            model.misccredittotal = model.misccreditqty * model.misccreditrate;

            model.samedayrushqty = 100;
            model.samedayrushrate = 5;
            model.samedayrushtotal = model.samedayrushqty * model.samedayrushrate;

            model.emptydrumdisposaltotal = 100;
            
            model.emptypailsqty = 10;
            model.emptypailsrate = 5;
            model.emptypailstotal = model.emptypailsqty * model.emptypailsrate;
            
            model.emptypailsfgttotal = 100;
            
            model.inactivestksurchqty = 10;
            model.inactivestksurchrate = 20;
            model.inactivestksurchtotal = model.inactivestksurchqty * model.inactivestksurchrate;

            return model;
        }
    }
}