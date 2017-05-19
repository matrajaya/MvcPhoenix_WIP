using MvcPhoenix.EF;
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
                var listofshelfmasters = (from t in db.tblShelfMaster
                                          join pd in db.tblProductDetail on t.ProductDetailID equals pd.ProductDetailID
                                          join pm in db.tblProductMaster on pd.ProductMasterID equals pm.ProductMasterID
                                          join cl in db.tblClient on pm.ClientID equals cl.ClientID
                                          where t.ProductDetailID == id
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

                return listofshelfmasters;
            }
        }

        public static int fnCloneShelfMaster(int shelfid)
        {
            using (var db = new CMCSQL03Entities())
            {
                // clone shelf -> return the productdetailid
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
            using (var db = new CMCSQL03Entities())
            {
                ShelfMasterViewModel shelfmaster = new ShelfMasterViewModel();

                var productdetail = db.tblProductDetail.Find(id);
                var productmaster = db.tblProductMaster.Find(productdetail.ProductMasterID);

                shelfmaster.shelfid = -1;
                shelfmaster.productdetailid = id;
                shelfmaster.clientid = productmaster.ClientID ?? 0;

                return shelfmaster;
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
                ShelfMasterViewModel model = new ShelfMasterViewModel();
                var shelfmaster = db.tblShelfMaster.Find(id);

                var productdetail = db.tblProductDetail.Find(shelfmaster.ProductDetailID);
                var productmaster = db.tblProductMaster.Find(productdetail.ProductMasterID);

                model.shelfid = shelfmaster.ShelfID;
                model.clientid = productmaster.ClientID ?? 0;
                model.productdetailid = shelfmaster.ProductDetailID;
                model.warehouse = shelfmaster.Warehouse;
                model.size = shelfmaster.Size;
                model.unitweight = shelfmaster.UnitWeight;
                model.reordermin = shelfmaster.ReorderMin;
                model.reordermax = shelfmaster.ReorderMax;
                model.reorderqty = shelfmaster.ReorderQty;
                model.bin = shelfmaster.Bin;
                model.hazardsurcharge = shelfmaster.HazardSurcharge;
                model.flammablesurcharge = shelfmaster.FlammableSurcharge;
                model.heatsurcharge = shelfmaster.HeatSurcharge;
                model.refrigsurcharge = shelfmaster.RefrigSurcharge;
                model.freezersurcharge = shelfmaster.FreezerSurcharge;
                model.cleansurcharge = shelfmaster.CleanSurcharge;
                model.blendsurcharge = shelfmaster.BlendSurcharge;
                model.nalgenesurcharge = shelfmaster.NalgeneSurcharge;
                model.nitrogensurcharge = shelfmaster.NitrogenSurcharge;
                model.biocidesurcharge = shelfmaster.BiocideSurcharge;
                model.koshersurcharge = shelfmaster.KosherSurcharge;
                model.labelsurcharge = shelfmaster.LabelSurcharge;
                model.othersurcharge = shelfmaster.OtherSurcharge;
                model.othersurchargeamt = shelfmaster.OtherSurchargeAmt;
                model.othersurchargedescription = shelfmaster.OtherSurchargeDescription;
                model.newitem = shelfmaster.NewItem;
                model.inactivesize = shelfmaster.InactiveSize;
                model.weboeinclude = shelfmaster.WebOEInclude;
                model.sortorder = shelfmaster.SortOrder;
                model.packageid = shelfmaster.PackageID;
                model.notes = shelfmaster.Notes;
                model.discontinued = shelfmaster.Discontinued;
                model.alert = shelfmaster.Alert;
                model.custcode = shelfmaster.CustCode;

                return model;
            }
        }

        public static void fnSaveShelfMaster(ShelfMasterViewModel obj)
        {
            using (var db = new CMCSQL03Entities())
            {
                if (obj.shelfid == -1)
                {
                    obj.shelfid = fnNewShelfID();
                }

                var shelfmaster = db.tblShelfMaster.Find(obj.shelfid);

                shelfmaster.ProductDetailID = obj.productdetailid;
                shelfmaster.Warehouse = obj.warehouse;
                shelfmaster.Size = obj.size;
                shelfmaster.UnitWeight = obj.unitweight;
                shelfmaster.ReorderMin = obj.reordermin;
                shelfmaster.ReorderMax = obj.reordermax;
                shelfmaster.ReorderQty = obj.reorderqty;
                shelfmaster.Bin = obj.bin;
                shelfmaster.HazardSurcharge = obj.hazardsurcharge;
                shelfmaster.FlammableSurcharge = obj.flammablesurcharge;
                shelfmaster.HeatSurcharge = obj.heatsurcharge;
                shelfmaster.RefrigSurcharge = obj.refrigsurcharge;
                shelfmaster.FreezerSurcharge = obj.freezersurcharge;
                shelfmaster.CleanSurcharge = obj.cleansurcharge;
                shelfmaster.BlendSurcharge = obj.blendsurcharge;
                shelfmaster.NalgeneSurcharge = obj.nalgenesurcharge;
                shelfmaster.NitrogenSurcharge = obj.nitrogensurcharge;
                shelfmaster.BiocideSurcharge = obj.biocidesurcharge;
                shelfmaster.KosherSurcharge = obj.koshersurcharge;
                shelfmaster.LabelSurcharge = obj.labelsurcharge;
                shelfmaster.OtherSurcharge = obj.othersurcharge;
                shelfmaster.OtherSurchargeAmt = obj.othersurchargeamt;
                shelfmaster.OtherSurchargeDescription = obj.othersurchargedescription;
                shelfmaster.NewItem = obj.newitem;
                shelfmaster.InactiveSize = obj.inactivesize;
                shelfmaster.WebOEInclude = obj.weboeinclude;
                shelfmaster.SortOrder = obj.sortorder;
                shelfmaster.PackageID = obj.packageid;
                shelfmaster.Notes = obj.notes;
                shelfmaster.Discontinued = obj.discontinued;
                shelfmaster.Alert = obj.alert;
                shelfmaster.CustCode = obj.custcode;

                db.SaveChanges();
            }
        }
    }
}