﻿using MvcPhoenix.EF;
using MvcPhoenix.Extensions;
using MvcPhoenix.Models;
using System.Collections.Generic;
using System.Linq;

namespace MvcPhoenix.Services
{
    public class ShelfMasterService
    {
        public static List<ShelfMasterViewModel> fnListOfShelfMasters(int id)
        {
            using (var db = new CMCSQL03Entities())
            {
                var mylist = (from t in db.tblShelfMaster
                              join pd in db.tblProductDetail on t.ProductDetailID equals pd.ProductDetailID
                              join pm in db.tblProductMaster on pd.ProductMasterID equals pm.ProductMasterID
                              join cl in db.tblClient on pm.ClientID equals cl.ClientID
                              where t.ProductDetailID == id //&& t.InactiveSize == false
                              let PPN = (from pk in db.tblPackage
                                         where pk.PackageID == t.PackageID
                                         select pk.PartNumber).FirstOrDefault()
                              let PPMaterial = (from pk1 in db.tblPackage
                                                where pk1.PackageID == t.PackageID
                                                select pk1.Description).FirstOrDefault()
                              select new ShelfMasterViewModel
                              {
                                  clientum = cl.ClientUM,
                                  shelfid = t.ShelfID,
                                  productdetailid = t.ProductDetailID,
                                  clientid = cl.ClientID,
                                  clientname = cl.ClientName,
                                  productcode = pd.ProductCode,
                                  productname = pd.ProductName,
                                  size = t.Size,
                                  packageid = t.PackageID,
                                  warehouse = t.Warehouse,
                                  bin = t.Bin,
                                  reordermin = t.ReorderMin,
                                  reordermax = t.ReorderMax,
                                  reorderqty = t.ReorderQty,
                                  notes = t.Notes,
                                  packagepartnumber = PPN,
                                  pkgmaterial = PPMaterial,
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
                                  othersurcharge = t.OtherSurcharge,
                                  inactivesize = t.InactiveSize
                              }).ToList();
                foreach (var item in mylist)
                {
                }
                return mylist;
            }
        }

        public static int fnCloneShelfMaster(int shelfid)
        {
            using (var db = new CMCSQL03Entities())
            {
                // clone shelf, return the productdetailid
                var shelf = (from s in db.tblShelfMaster
                             where s.ShelfID == shelfid
                             select s).FirstOrDefault();

                var newShelf = shelf.Clone();
                int productdetailid = newShelf.ProductDetailID ?? 0;
                db.tblShelfMaster.Add(newShelf);

                db.SaveChanges();

                return productdetailid;
            }
        }

        public static ShelfMasterViewModel fnCreateNewShelfMaster(int id)
        {
            ShelfMasterViewModel SM = new ShelfMasterViewModel();
            SM.shelfid = -1;
            SM.productdetailid = id;
            using (var db = new CMCSQL03Entities())
            {
                // get a pointer to the ProductDetail and ProductMaster parent records
                var PD = db.tblProductDetail.Find(id);
                var PM = db.tblProductMaster.Find(PD.ProductMasterID);

                return SM;
            }
        }

        public static void fnDeleteShelfMaster(int id)
        {
            using (var db = new CMCSQL03Entities())
            {
                db.Database.ExecuteSqlCommand("Delete from tblShelfMaster where ShelfID=" + id.ToString());
            }
        }

        public static int fnNewShelfID()
        {
            using (var db = new CMCSQL03Entities())
            {
                var newrecord = new tblShelfMaster
                {
                    // dont need to insert any fields, just need the new PK
                };
                db.tblShelfMaster.Add(newrecord);
                db.SaveChanges();
                int newpk = newrecord.ShelfID;
                return newpk;
            }
        }

        public static ShelfMasterViewModel fnFillShelfMasterFromDB(int id)
        {
            using (var db = new CMCSQL03Entities())
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
                SM.othersurchargedescription = dbSM.OtherSurchargeDescription;
                SM.newitem = dbSM.NewItem;
                SM.inactivesize = dbSM.InactiveSize;
                SM.weboeinclude = dbSM.WebOEInclude;
                SM.sortorder = dbSM.SortOrder;
                SM.packageid = dbSM.PackageID;
                SM.notes = dbSM.Notes;
                SM.discontinued = dbSM.Discontinued;
                SM.alert = dbSM.Alert;
                SM.custcode = dbSM.CustCode;
                return SM;
            }
        }

        public static void fnSaveShelfMaster(ShelfMasterViewModel obj)
        {
            using (var db = new CMCSQL03Entities())
            {
                if (obj.shelfid == -1)
                { obj.shelfid = fnNewShelfID(); }

                var dbSM = db.tblShelfMaster.Find(obj.shelfid);
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
                dbSM.OtherSurchargeDescription = obj.othersurchargedescription;
                dbSM.NewItem = obj.newitem;
                dbSM.InactiveSize = obj.inactivesize;
                dbSM.WebOEInclude = obj.weboeinclude;
                dbSM.SortOrder = obj.sortorder;
                dbSM.PackageID = obj.packageid;
                dbSM.Notes = obj.notes;
                dbSM.Discontinued = obj.discontinued;
                dbSM.Alert = obj.alert;
                dbSM.CustCode = obj.custcode;
                db.SaveChanges();
            }
        }

        public static string fnBuildShelfMasterPackagesDropDown(string size)
        {
            // This returns ONLY the <option> portion of the <select> tag
            using (var db = new CMCSQL03Entities())
            {
                var packages = (from t in db.tblPackage
                                //where t.Size == size
                                orderby t.Size
                                select t);

                string s = "<option value='0' selected=true>Select Package</option>";
                if (packages.Count() > 0)
                {
                    foreach (var item in packages)
                    { s = s + "<option value=" + item.PackageID.ToString() + ">" + item.PartNumber + " - " + item.Description + "</option>"; }
                }
                else
                { s = s + "<option value=0>No Packages Found</option>"; }
                s = s + "</select>";
                return s;
            }
        }
    }
}