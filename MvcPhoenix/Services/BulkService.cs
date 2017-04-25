using MvcPhoenix.EF;
using MvcPhoenix.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcPhoenix.Services
{
    public class BulkService
    {
        public static string fnBuildProductMasterDropDown(int clientid)
        {
            // This returns ONLY the <option> portion of the <select> tag, thus allowing the <select> tag to
            // be propering decorated with onchange= etc..
            using (var db = new CMCSQL03Entities())
            {
                var products = (from t in db.tblProductMaster
                                where t.ClientID == clientid
                                orderby t.MasterCode, t.MasterName
                                select t);

                string s = "<option value='0' selected=true>Select Master Code</option>";
                if (products.Count() > 0)
                {
                    foreach (var item in products)
                    { s = s + "<option value=" + item.ProductMasterID.ToString() + ">" + item.MasterCode + " - " + item.MasterName + "</option>"; }
                }
                else
                { s = s + "<option value=0>No Products Found</option>"; }
                s = s + "</select>";
                return s;
            }
        }

        public static BulkContainerViewModel fnFillBulkContainerFromDB(int id)
        {
            // id=bulkid
            using (var db = new CMCSQL03Entities())
            {
                var obj = (from t in db.tblBulk
                           where t.BulkID == id
                           select new BulkContainerViewModel
                           {
                               isknownmaterial = true,
                               bulkid = t.BulkID,
                               productmasterid = t.ProductMasterID,
                               warehouse = t.Warehouse,
                               receivedate = t.ReceiveDate,
                               carrier = t.Carrier,
                               receivedby = t.ReceivedBy,
                               enteredby = t.EnteredBy,
                               receiveweight = t.ReceiveWeight,
                               lotnumber = t.LotNumber,
                               mfgdate = t.MfgDate,
                               expirationdate = t.ExpirationDate,
                               ceaseshipdate = t.CeaseShipDate,
                               bulkstatus = t.BulkStatus,
                               qty = "1",
                               um = t.UM,
                               containercolor = t.ContainerColor,
                               bin = t.Bin,
                               containertype = t.ContainerType,
                               coaincluded = t.COAIncluded,
                               msdsincluded = t.MSDSIncluded,
                               currentweight = t.CurrentWeight,
                               qcdate = t.QCDate,
                               noticedate = t.NoticeDate,
                               bulklabelnote = t.BulkLabelNote,
                               receivedascode = t.ReceivedAsCode,
                               receivedasname = t.ReceivedAsName,
                               containernotes = t.ContainerNotes,
                               CreateUser = t.CreateUser,
                               CreateDate = t.CreateDate,
                               UpdateUser = t.UpdateUser,
                               UpdateDate = t.UpdateDate
                           }).FirstOrDefault();

                var qPM = (from t in db.tblProductMaster
                           where t.ProductMasterID == obj.productmasterid
                           select t).FirstOrDefault();

                var qCL = (from t in db.tblClient
                           where t.ClientID == qPM.ClientID
                           select t).FirstOrDefault();

                obj.clientid = qPM.ClientID;
                obj.clientname = qCL.ClientName;
                obj.MasterCode = qPM.MasterCode;
                obj.MasterName = qPM.MasterName;
                obj.pm_ceaseshipdifferential = qPM.CeaseShipDifferential;

                return obj;
            }
        }

        public static bool fnSaveBulk(BulkContainerViewModel incoming)
        {
            bool retval = true;
            try
            {
                using (var db = new CMCSQL03Entities())
                {
                    int pk = incoming.bulkid;
                    if (incoming.bulkid == -1)
                    {
                        var newrec = new tblBulk { ProductMasterID = incoming.productmasterid };
                        db.tblBulk.Add(newrec);
                        newrec.CreateDate = DateTime.UtcNow;
                        newrec.CreateUser = HttpContext.Current.User.Identity.Name;
                        newrec.UpdateDate = DateTime.UtcNow;
                        newrec.UpdateUser = HttpContext.Current.User.Identity.Name;
                        db.SaveChanges();
                        pk = newrec.BulkID;
                    }

                    var bulk = (from t in db.tblBulk
                                where t.BulkID == pk
                                select t).FirstOrDefault();

                    bulk.Warehouse = incoming.warehouse;
                    bulk.ReceiveDate = incoming.receivedate;
                    bulk.Carrier = incoming.carrier;
                    bulk.ReceivedBy = incoming.receivedby;
                    bulk.EnteredBy = incoming.enteredby;
                    bulk.ProductMasterID = incoming.productmasterid;
                    bulk.ReceiveWeight = incoming.receiveweight;
                    bulk.LotNumber = incoming.lotnumber;
                    bulk.MfgDate = incoming.mfgdate;
                    bulk.ExpirationDate = incoming.expirationdate;
                    bulk.CeaseShipDate = incoming.ceaseshipdate;
                    bulk.BulkStatus = incoming.bulkstatus;
                    bulk.UM = incoming.um;
                    bulk.ContainerColor = incoming.containercolor;
                    bulk.Bin = incoming.bin;
                    bulk.ContainerType = incoming.containertype;
                    bulk.COAIncluded = incoming.coaincluded;
                    bulk.MSDSIncluded = incoming.msdsincluded;
                    bulk.ContainerNotes = incoming.containernotes;
                    bulk.CurrentWeight = incoming.currentweight;
                    bulk.QCDate = incoming.qcdate;
                    bulk.NoticeDate = incoming.noticedate;
                    bulk.BulkLabelNote = incoming.bulklabelnote;
                    bulk.ReceivedAsCode = incoming.receivedascode;
                    bulk.ReceivedAsName = incoming.receivedasname;
                    bulk.UpdateDate = DateTime.UtcNow;
                    bulk.UpdateUser = HttpContext.Current.User.Identity.Name;
                    db.SaveChanges();
                    retval = true;
                }
            }
            catch
            {
                retval = false;
                throw new Exception("Error occurred saving Bulk Container");
            }
            return retval;
        }

        #region SupportMethods

        public static List<BulkContainerViewModel> fnBulkContainerList()
        {
            using (var db = new CMCSQL03Entities())
            {
                var mylist = (from t in db.tblBulk
                              join pm in db.tblProductMaster on t.ProductMasterID equals pm.ProductMasterID
                              join cl in db.tblClient on pm.ClientID equals cl.ClientID
                              orderby cl.ClientName, pm.MasterCode
                              select new BulkContainerViewModel
                              {
                                  bulkid = t.BulkID,
                                  warehouse = t.Warehouse,
                                  bin = t.Bin,
                                  clientname = cl.ClientName,
                                  MasterCode = pm.MasterCode,
                                  MasterName = pm.MasterName,
                                  bulkstatus = t.BulkStatus,
                                  productmasterid = t.ProductMasterID,
                                  clientid = cl.ClientID
                              }).ToList();
                return mylist;
            }
        }

        #endregion SupportMethods
    }
}