﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using MvcPhoenix.Models;
using System.Web.Mvc;

namespace MvcPhoenix.Services
{
    public class ShelfMasterService
    {
        private static string PathToLogos = "http://www.mysamplecenter.com/Logos/";


        public static List<ShelfMasterViewModel> fnListOfShelfMasters(int id)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                var mylist = (from t in db.tblShelfMaster
                              join pd in db.tblProductDetail on t.ProductDetailID equals pd.ProductDetailID
                              join pm in db.tblProductMaster on pd.ProductMasterID equals pm.ProductMasterID
                              join cl in db.tblClient on pm.ClientID equals cl.ClientID
                              where t.ProductDetailID == id
                              let PPN = (from pk in db.tblPackage where pk.PackageID == t.PackageID select pk.PartNumber).FirstOrDefault()
                              select new ShelfMasterViewModel
                              {
                                  shelfid = t.ShelfID,
                                  productdetailid = t.ProductDetailID,
                                  clientid = cl.ClientID,
                                  clientname = cl.ClientName,
                                  logofilename = PathToLogos + cl.LogoFileName,
                                  productcode = pd.ProductCode,
                                  productname = pd.ProductName,
                                  size = t.Size,
                                  packageid = t.PackageID,
                                  busarea = t.BusArea,
                                  warehouse=t.Warehouse,
                                  bin = t.Bin,
                                  reordermin = t.ReorderMin,
                                  reordermax = t.ReorderMax,
                                  reorderqty = t.ReorderQty,
                                  notes = t.Notes,
                                  packagepartnumber = PPN,

                                  hazardsurcharge = t.HazardSurcharge,
                                  flammablesurcharge = t.FlammableSurcharge,
                                  heatsurcharge = t.HeatSurcharge,
                                  refrigsurcharge = t.RefrigSurcharge,
                                  freezersurcharge = t.FreezerSurcharge,
                                  cleansurcharge = t.CleanSurcharge,
                                  blendsurcharge = t.BlendSurcharge,
                                  nalgenesurcharge = t.NalgeneSurcharge,
                                  nitrogensurcharge = t.NitrogenSurcharge,
                                  biocidesurcharge = t.BiocideSurcharge,
                                  koshersurcharge = t.KosherSurcharge,
                                  labelsurcharge = t.LabelSurcharge,
                                  othersurcharge = t.OtherSurcharge
                              }).ToList();
                foreach(var item in mylist)
                {
                    //if(String.IsNullOrEmpty(item.size))
                    //{
                    //    item.IsValidItem = false;
                    //}
                }
                return mylist;
            }
        }


        public static int fnCloneShelfMaster(int id)
        {
            // id = shelfid
            using (var db = new EF.CMCSQL03Entities())
            {
                // copy a record, return the productdetailid
                var dbrow = db.tblShelfMaster.Find(id);
                int pdid = Convert.ToInt32(dbrow.ProductDetailID);
                string s = "Insert into tblShelfMaster (ProductDetailID,Size,Warehouse,Bin) Select ProductDetailID,null,Warehouse,Bin from tblShelfMaster where shelfid=" + id.ToString();
                db.Database.ExecuteSqlCommand(s);
                return pdid;
            }
        }

        public static ShelfMasterViewModel fnCreateNewShelfMaster(int id)
        {
            ShelfMasterViewModel SM = new ShelfMasterViewModel();
            SM.shelfid = -1;
            SM.productdetailid = id;
            using (var db = new EF.CMCSQL03Entities())
            {
                // get a pointer to the ProductDetail and ProductMaster parent records
                var PD = db.tblProductDetail.Find(id);
                var PM = db.tblProductMaster.Find(PD.ProductMasterID);
                SM.ListOfTierSizes = fnListOfTierSizes(PM.ClientID);
                SM.ListOfPackages = fnListOfPackageIDs(null);
                SM.ListOfWareHouses = fnWarehouseIDs();
                return SM;
            }
        }

        public static void fnDeleteShelfMaster(int id)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                //var dbrow = db.tblShelfMaster.Find(id);
                //int? PDid = dbrow.ProductDetailID;
                db.Database.ExecuteSqlCommand("Delete from tblShelfMaster where ShelfID=" + id.ToString());
            }

        }

        private static int fnNewShelfID()
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                var newrecord = new EF.tblShelfMaster
                {
                    // dont need to insert any fields, just need the new PK
                };
                db.tblShelfMaster.Add(newrecord); db.SaveChanges();
                int newpk = newrecord.ShelfID;
                return newpk;
            }
        }
        
        public static ShelfMasterViewModel fnFillShelfMasterFromDB(int id)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                ShelfMasterViewModel SM = new ShelfMasterViewModel();
                var dbSM = db.tblShelfMaster.Find(id);

                // get a pointer to the ProductDetail and ProductMaster parent records
                var PD = db.tblProductDetail.Find(dbSM.ProductDetailID);
                var PM = db.tblProductMaster.Find(PD.ProductMasterID);

                SM.shelfid = dbSM.ShelfID;
                SM.productdetailid = dbSM.ProductDetailID;
                SM.warehouse = dbSM.Warehouse;
                SM.size = dbSM.Size;
                SM.unitweight = dbSM.UnitWeight;
                SM.reordermin = dbSM.ReorderMin;
                SM.reordermax = dbSM.ReorderMax;
                SM.reorderqty = dbSM.ReorderQty;
                SM.bin = dbSM.Bin;   //??????
                SM.hazardsurcharge = dbSM.HazardSurcharge;
                SM.flammablesurcharge = dbSM.FlammableSurcharge;
                SM.heatsurcharge = dbSM.HeatSurcharge;
                SM.refrigsurcharge = dbSM.RefrigSurcharge;
                SM.freezersurcharge = dbSM.FreezerSurcharge;
                SM.cleansurcharge = dbSM.CleanSurcharge;
                SM.blendsurcharge = dbSM.BlendSurcharge;
                SM.nalgenesurcharge = dbSM.NalgeneSurcharge;
                SM.nitrogensurcharge = dbSM.NitrogenSurcharge;
                SM.biocidesurcharge = dbSM.BiocideSurcharge;
                SM.koshersurcharge = dbSM.KosherSurcharge;
                SM.labelsurcharge = dbSM.LabelSurcharge;
                SM.othersurcharge = dbSM.OtherSurcharge;
                SM.othersurchargeamt = dbSM.OtherSurchargeAmt;
                SM.newitem = dbSM.NewItem;
                SM.inactivesize = dbSM.InactiveSize;
                SM.weboeinclude = dbSM.WebOEInclude;
                SM.sortorder = dbSM.SortOrder;
                SM.packageid = dbSM.PackageID;
                SM.busarea = dbSM.BusArea;
                //SM.mnemonic = dbSM.Mnemonic;
                //SM.groundhazard = dbSM.GroundHazard;
                //SM.airhazard = dbSM.AirHazard;
                SM.notes = dbSM.Notes;
                SM.discontinued = dbSM.Discontinued;
                SM.alert = dbSM.Alert;
                SM.custcode = dbSM.CustCode;

                SM.ListOfTierSizes = fnListOfTierSizes(PM.ClientID);
                SM.ListOfPackages = fnListOfPackageIDs(dbSM.Size);
                SM.ListOfWareHouses = fnWarehouseIDs();
                return SM;
            }
        }

        public static void fnSaveShelfMaster(ShelfMasterViewModel obj)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                if (obj.shelfid == -1)
                { obj.shelfid = fnNewShelfID(); }

                var dbSM = db.tblShelfMaster.Find(obj.shelfid);
                //SM.shelfid = dbSM.ShelfID;
                dbSM.ProductDetailID = obj.productdetailid;
                dbSM.Warehouse = obj.warehouse;
                dbSM.Size = obj.size;
                dbSM.UnitWeight = obj.unitweight;
                dbSM.ReorderMin = obj.reordermin;
                dbSM.ReorderMax = obj.reordermax;
                dbSM.ReorderQty = obj.reorderqty;
                dbSM.Bin = obj.bin;   //??????
                dbSM.HazardSurcharge = obj.hazardsurcharge;
                dbSM.FlammableSurcharge = obj.flammablesurcharge;
                dbSM.HeatSurcharge = obj.heatsurcharge;
                dbSM.RefrigSurcharge = obj.refrigsurcharge;
                dbSM.FreezerSurcharge = obj.freezersurcharge;
                dbSM.CleanSurcharge = obj.cleansurcharge;
                dbSM.BlendSurcharge = obj.blendsurcharge;
                dbSM.NalgeneSurcharge = obj.nalgenesurcharge;
                dbSM.NitrogenSurcharge = obj.nitrogensurcharge;
                dbSM.BiocideSurcharge = obj.biocidesurcharge;
                dbSM.KosherSurcharge = obj.koshersurcharge;
                dbSM.LabelSurcharge = obj.labelsurcharge;
                dbSM.OtherSurcharge = obj.othersurcharge;
                dbSM.OtherSurchargeAmt = obj.othersurchargeamt;
                dbSM.NewItem = obj.newitem;
                dbSM.InactiveSize = obj.inactivesize;
                dbSM.WebOEInclude = obj.weboeinclude;
                dbSM.SortOrder = obj.sortorder;
                dbSM.PackageID = obj.packageid;
                dbSM.BusArea = obj.busarea;
                //dbSM.Mnemonic = obj.mnemonic;
                //dbSM.GroundHazard = obj.groundhazard;
                //dbSM.AirHazard = obj.airhazard;
                dbSM.Notes = obj.notes;
                dbSM.Discontinued = obj.discontinued;
                dbSM.Alert = obj.alert;
                dbSM.CustCode = obj.custcode;
                db.SaveChanges();

            }
        }

        private static List<SelectListItem> fnWarehouseIDs()
        {
            List<SelectListItem> mylist = new List<SelectListItem>();
            mylist.Add(new SelectListItem { Value = null, Text = "" });
            mylist.Add(new SelectListItem { Value = "AP", Text = "AP" });
            mylist.Add(new SelectListItem { Value = "CT", Text = "CT" });
            mylist.Add(new SelectListItem { Value = "CO", Text = "CO" });
            mylist.Add(new SelectListItem { Value = "EU", Text = "EU" });
            return mylist;
        }


        private static List<SelectListItem> fnListOfTierSizes(int? ClientID)
        {
            List<SelectListItem> mylist = new List<SelectListItem>();
            using (var db = new EF.CMCSQL03Entities())
            {
                mylist = (from t in db.tblTier
                          where t.ClientID == ClientID
                          orderby t.Size
                          select new SelectListItem { Value = t.Size, Text = t.Size }).Distinct().ToList();
                mylist.Add(new SelectListItem { Value = "", Text = "" });
                return mylist;
            }

        }


        public static string fnBuildShelfMasterPackagesDropDown(string size)
        {
            // This returns ONLY the <option> portion of the <select> tag
            using (var db = new EF.CMCSQL03Entities())
            {
                var qry = (from t in db.tblPackage where t.Size == size orderby t.Size select t);
                string s = "<option value='0' selected=true>Select Package</option>";
                if (qry.Count() > 0)
                {
                    foreach (var item in qry)
                    { s = s + "<option value=" + item.PackageID.ToString() + ">" + item.PartNumber + " - " + item.Description + "</option>"; }
                }
                else
                { s = s + "<option value=0>No Packages Found</option>"; }
                s = s + "</select>";
                return s;
            }

        }

        //public static string? isValidShelfMaster(ShelfMasterViewModel obj)
        //{
        //    // Validate the viewmodel and return a t/f
        //    //bool retval = false;
        //    // look for a reason to fail
        //    string retval = "Errors";
                    
        //    return retval;
        //}


        private static List<SelectListItem> fnListOfPackageIDs(string size)
        {
            List<SelectListItem> mylist = new List<SelectListItem>();
            using (var db = new EF.CMCSQL03Entities())
            {
                mylist = (from t in db.tblPackage
                          where t.Size == size
                          orderby t.Size
                          select new SelectListItem { Value = t.PackageID.ToString(), Text = t.PartNumber + "-" + t.Description }).ToList();
                mylist.Add(new SelectListItem { Value = "", Text = "" });
                return mylist;
            }

        }



    }


}