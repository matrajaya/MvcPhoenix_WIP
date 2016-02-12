using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Mvc;
using MvcPhoenix.EF;

using MvcPhoenix.Models;

namespace MvcPhoenix.Services
{
    public class BulkService
    {
        private static string PathToLogos = "http://www.mysamplecenter.com/Logos/";

        public static string fnBuildProductMasterDropDown(int clientid)
        {
            // This returns ONLY the <option> portion of the <select> tag, thus allowing the <select> tag to
            // be propering decorated with onchange= etc..
            using (var db = new CMCSQL03Entities())
            {
                var qry = (from t in db.tblProductMaster where t.ClientID == clientid orderby t.MasterCode, t.MasterName select t);
                string s = "<option value='0' selected=true>Select Master Code</option>";
                if (qry.Count() > 0)
                {
                    foreach (var item in qry)
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
                               returnlocation = t.ReturnLocation,
                               noticedate = t.NoticeDate,
                               bulklabelnote = t.BulkLabelNote,
                               receivedascode = t.ReceivedAsCode,
                               receivedasname = t.ReceivedAsName,
                               containernotes = t.ContainerNotes,
                               otherstorage = t.OtherStorage
                           }).FirstOrDefault();

                var qPM = (from t in db.tblProductMaster where t.ProductMasterID == obj.productmasterid select t).FirstOrDefault();
                var qCL = (from t in db.tblClient where t.ClientID == qPM.ClientID select t).FirstOrDefault();
                obj.clientid = qPM.ClientID;
                obj.clientname = qCL.ClientName;

                obj.logofilename = PathToLogos + qCL.LogoFileName;
                obj.ListOfWareHouses = fnWarehouseIDs();
                //obj.ListOfProductMasters = ReceivingService.fnProductMasterIDs(qPM.ClientID, qPM.ProductMasterID);
                obj.ListOfBulkStatusIDs = fnBulkStatusIDs();
                obj.ListOfContainerTypeIDs = ReceivingService.fnContainerTypeIDs();
                obj.ListOfCarriers = ReceivingService.fnCarriers();

                obj.MasterCode = qPM.MasterCode;
                obj.MasterName = qPM.MasterName;
                //obj.flammable = qPM.Flammable;
                //obj.freezer = qPM.FREEZERSTORAGE;
                //obj.refrigerated = qPM.Refrigerate;
                //obj.packout = qPM.PackOutOnReceipt;

                return obj;

            }
        }


        public static bool fnSaveBulk(BulkContainerViewModel incoming)
        {
            bool retval = true;
            try
            {
                using (var db = new EF.CMCSQL03Entities())
                {
                    int pk = incoming.bulkid;
                    if (incoming.bulkid == -1)
                    {
                        var newrec = new EF.tblBulk { ProductMasterID = incoming.productmasterid };
                        db.tblBulk.Add(newrec);
                        db.SaveChanges();
                        pk = newrec.BulkID;
                    }

                    var qry = (from t in db.tblBulk where t.BulkID == pk select t).FirstOrDefault();
                    qry.Warehouse = incoming.warehouse;
                    qry.ReceiveDate = incoming.receivedate;
                    qry.Carrier = incoming.carrier;
                    qry.ReceivedBy = incoming.receivedby;
                    qry.EnteredBy = incoming.enteredby;
                    qry.ProductMasterID = incoming.productmasterid;
                    qry.ReceiveWeight = incoming.receiveweight;
                    qry.LotNumber = incoming.lotnumber;
                    qry.MfgDate = incoming.mfgdate;
                    qry.ExpirationDate = incoming.expirationdate;
                    qry.CeaseShipDate = incoming.ceaseshipdate;
                    qry.BulkStatus = incoming.bulkstatus;
                    qry.UM = incoming.um;
                    qry.ContainerColor = incoming.containercolor;
                    qry.Bin = incoming.bin;
                    qry.ContainerType = incoming.containertype;
                    qry.COAIncluded = incoming.coaincluded;
                    qry.MSDSIncluded = incoming.msdsincluded;
                    qry.ContainerNotes = incoming.containernotes;
                    qry.CurrentWeight = incoming.currentweight;
                    qry.QCDate = incoming.qcdate;
                    qry.ReturnLocation = incoming.returnlocation;
                    qry.NoticeDate = incoming.noticedate;
                    qry.BulkLabelNote = incoming.bulklabelnote;
                    qry.ReceivedAsCode = incoming.receivedascode;
                    qry.ReceivedAsName = incoming.receivedasname;
                    qry.OtherStorage = incoming.otherstorage;
                    db.SaveChanges();
                    retval = true;
                }
            }
            catch
            {
                retval = false;
                throw new Exception("Error occurred saving Bulk Container");
                //System.Diagnostics.Debug.WriteLine(ex.Message);
                //retval = false;
            }
            return retval;
        }




        #region SupportMethods
        public static List<BulkContainerViewModel> fnBulkContainerList()
        {
            // full list
            using (var db = new CMCSQL03Entities())
            {
                var mylist = (from t in db.tblBulk
                              join pm in db.tblProductMaster on t.ProductMasterID equals pm.ProductMasterID
                              join cl in db.tblClient on pm.ClientID equals cl.ClientID
                              orderby cl.ClientName,pm.MasterCode
                              select new BulkContainerViewModel
                              {
                                  bulkid = t.BulkID,
                                  warehouse = t.Warehouse,
                                  bin=t.Bin,
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


        public static List<SelectListItem> fnClientIDs()
        {
            using (var db = new MvcPhoenix.EF.CMCSQL03Entities())
            {
                List<SelectListItem> mylist = new List<SelectListItem>();
                mylist = (from t in db.tblClient
                          orderby t.ClientName
                          select new SelectListItem { Value = t.ClientID.ToString(), Text = t.ClientName }).ToList();
                mylist.Insert(0, new SelectListItem { Value = "0", Text = "Select Client" });
                return mylist;
            }
        }

        public static List<SelectListItem> fnWarehouseIDs()
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> mylist = new List<SelectListItem>();
                mylist.Add(new SelectListItem { Value = "CO", Text = "CO" });
                mylist.Add(new SelectListItem { Value = "CT", Text = "CT" });
                mylist.Add(new SelectListItem { Value = "EU", Text = "EU" });
                mylist.Add(new SelectListItem { Value = "AP", Text = "AP" });
                mylist.Insert(0, new SelectListItem { Value = "", Text = "All" });
                return mylist;
            }
        }

        public static List<SelectListItem> fnBulkStatusIDs()
        {
            using (var db = new MvcPhoenix.EF.CMCSQL03Entities())
            {
                List<SelectListItem> mylist = new List<SelectListItem>();
                mylist = (from t in db.tblBulk
                          orderby t.BulkStatus
                          select new SelectListItem { Value = t.BulkStatus, Text = t.BulkStatus }).Distinct().ToList();
                mylist.Insert(0, new SelectListItem { Value = "", Text = "All" });
                return mylist;
            }
        }
        #endregion



    }
}