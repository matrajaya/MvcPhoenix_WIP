using MvcPhoenix.EF;
using MvcPhoenix.Extensions;
using MvcPhoenix.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcPhoenix.Services
{
    public class ShelfMasterService
    {
        #region Shelf Master

        /// <summary>
        /// Get list of shelf masters
        /// </summary>
        /// <param name="productdetailid"></param>
        /// <returns>list of shelf masters</returns>
        public static List<ShelfMasterViewModel> GetShelfs(int productdetailid)
        {
            using (var db = new CMCSQL03Entities())
            {
                var shelfMasters = (from shelfmaster in db.tblShelfMaster
                                    join productdetail in db.tblProductDetail on shelfmaster.ProductDetailID equals productdetail.ProductDetailID
                                    join productmaster in db.tblProductMaster on productdetail.ProductMasterID equals productmaster.ProductMasterID
                                    join client in db.tblClient on productmaster.ClientID equals client.ClientID
                                    where shelfmaster.ProductDetailID == productdetailid
                                    let package = db.tblPackage.Where(x => x.PackageID == shelfmaster.PackageID)
                                                               .Select(x => new { x.PartNumber, x.Description })
                                                               .FirstOrDefault()
                                    select new ShelfMasterViewModel
                                    {
                                        clientum = client.ClientUM,
                                        shelfid = shelfmaster.ShelfID,
                                        productdetailid = shelfmaster.ProductDetailID,
                                        clientid = client.ClientID,
                                        clientname = client.ClientName,
                                        productcode = productdetail.ProductCode,
                                        productname = productdetail.ProductName,
                                        size = shelfmaster.Size,
                                        packageid = shelfmaster.PackageID,
                                        warehouse = shelfmaster.Warehouse,
                                        bin = shelfmaster.Bin,
                                        reordermin = shelfmaster.ReorderMin,
                                        reordermax = shelfmaster.ReorderMax,
                                        reorderqty = shelfmaster.ReorderQty,
                                        notes = shelfmaster.Notes,
                                        packagepartnumber = package.PartNumber,
                                        pkgmaterial = package.Description,
                                        hazardsurcharge = shelfmaster.HazardSurcharge,
                                        flammablesurcharge = shelfmaster.FlammableSurcharge,
                                        heatsurcharge = shelfmaster.HeatSurcharge,
                                        refrigsurcharge = shelfmaster.RefrigSurcharge,
                                        freezersurcharge = shelfmaster.FreezerSurcharge,
                                        cleansurcharge = shelfmaster.CleanSurcharge,
                                        blendsurcharge = shelfmaster.BlendSurcharge,
                                        nalgenesurcharge = shelfmaster.NalgeneSurcharge,
                                        nitrogensurcharge = shelfmaster.NitrogenSurcharge,
                                        biocidesurcharge = shelfmaster.BiocideSurcharge,
                                        koshersurcharge = shelfmaster.KosherSurcharge,
                                        labelsurcharge = shelfmaster.LabelSurcharge,
                                        othersurcharge = shelfmaster.OtherSurcharge,
                                        inactivesize = shelfmaster.InactiveSize
                                    }).ToList();

                return shelfMasters;
            }
        }

        /// <summary>
        /// Get shelf master details
        /// </summary>
        /// <param name="shelfid"></param>
        /// <returns>Shelf model</returns>
        public static ShelfMasterViewModel GetShelf(int shelfid)
        {
            var shelf = new ShelfMasterViewModel();

            using (var db = new CMCSQL03Entities())
            {
                var shelfmaster = db.tblShelfMaster.Find(shelfid);
                var productdetail = db.tblProductDetail.Find(shelfmaster.ProductDetailID);
                var productmaster = db.tblProductMaster.Find(productdetail.ProductMasterID);

                shelf.shelfid = shelfmaster.ShelfID;
                shelf.clientid = productmaster.ClientID ?? 0;
                shelf.productdetailid = shelfmaster.ProductDetailID;
                shelf.warehouse = shelfmaster.Warehouse;
                shelf.size = shelfmaster.Size;
                shelf.unitweight = shelfmaster.UnitWeight;
                shelf.reordermin = shelfmaster.ReorderMin;
                shelf.reordermax = shelfmaster.ReorderMax;
                shelf.reorderqty = shelfmaster.ReorderQty;
                shelf.bin = shelfmaster.Bin;
                shelf.hazardsurcharge = shelfmaster.HazardSurcharge;
                shelf.flammablesurcharge = shelfmaster.FlammableSurcharge;
                shelf.heatsurcharge = shelfmaster.HeatSurcharge;
                shelf.refrigsurcharge = shelfmaster.RefrigSurcharge;
                shelf.freezersurcharge = shelfmaster.FreezerSurcharge;
                shelf.cleansurcharge = shelfmaster.CleanSurcharge;
                shelf.blendsurcharge = shelfmaster.BlendSurcharge;
                shelf.nalgenesurcharge = shelfmaster.NalgeneSurcharge;
                shelf.nitrogensurcharge = shelfmaster.NitrogenSurcharge;
                shelf.biocidesurcharge = shelfmaster.BiocideSurcharge;
                shelf.koshersurcharge = shelfmaster.KosherSurcharge;
                shelf.labelsurcharge = shelfmaster.LabelSurcharge;
                shelf.othersurcharge = shelfmaster.OtherSurcharge;
                shelf.othersurchargeamt = shelfmaster.OtherSurchargeAmt;
                shelf.othersurchargedescription = shelfmaster.OtherSurchargeDescription;
                shelf.newitem = shelfmaster.NewItem;
                shelf.inactivesize = shelfmaster.InactiveSize;
                shelf.weboeinclude = shelfmaster.WebOEInclude;
                shelf.sortorder = shelfmaster.SortOrder;
                shelf.packageid = shelfmaster.PackageID;
                shelf.notes = shelfmaster.Notes;
                shelf.discontinued = shelfmaster.Discontinued;
                shelf.alert = shelfmaster.Alert;
                shelf.custcode = shelfmaster.CustCode;
            }

            return shelf;
        }

        /// <summary>
        /// Create new shelf
        /// </summary>
        /// <returns>shelfId</returns>
        public static int NewShelfId()
        {
            using (var db = new CMCSQL03Entities())
            {
                var shelf = new tblShelfMaster { };
                db.tblShelfMaster.Add(shelf);
                db.SaveChanges();

                int shelfId = shelf.ShelfID;

                return shelfId;
            }
        }

        /// <summary>
        /// Create empty new shelf
        /// </summary>
        /// <param name="productdetailid"></param>
        /// <returns>Shelf model</returns>
        public static ShelfMasterViewModel CreateShelf(int productdetailid)
        {
            using (var db = new CMCSQL03Entities())
            {
                var productdetail = db.tblProductDetail.Find(productdetailid);
                var productmaster = db.tblProductMaster.Find(productdetail.ProductMasterID);

                var shelf = new ShelfMasterViewModel();

                shelf.shelfid = -1;
                shelf.productdetailid = productdetailid;
                shelf.clientid = productmaster.ClientID ?? 0;

                return shelf;
            }
        }

        /// <summary>
        /// Clone shelf master
        /// </summary>
        /// <param name="shelfid"></param>
        /// <returns>productDetailId</returns>
        public static int CloneShelf(int shelfid)
        {
            int productDetailId;

            using (var db = new CMCSQL03Entities())
            {
                var shelf = db.tblShelfMaster.Find(shelfid);
                var newShelf = shelf.Clone();
                newShelf.Notes = "New shelf size cloned on " + DateTime.UtcNow.ToString("R") + " by " + HttpContext.Current.User.Identity.Name + ".\n";

                db.tblShelfMaster.Add(newShelf);
                db.SaveChanges();

                productDetailId = newShelf.ProductDetailID ?? 0;
            }

            return productDetailId;
        }

        /// <summary>
        /// Save shelf master entries
        /// </summary>
        /// <param name="shelf"></param>
        public static void SaveShelf(ShelfMasterViewModel shelf)
        {
            if (shelf.shelfid == -1)
            {
                shelf.shelfid = NewShelfId();
            }

            using (var db = new CMCSQL03Entities())
            {
                var shelfmaster = db.tblShelfMaster.Find(shelf.shelfid);

                shelfmaster.ProductDetailID = shelf.productdetailid;
                shelfmaster.Warehouse = shelf.warehouse;
                shelfmaster.Size = shelf.size;
                shelfmaster.UnitWeight = shelf.unitweight;
                shelfmaster.ReorderMin = shelf.reordermin;
                shelfmaster.ReorderMax = shelf.reordermax;
                shelfmaster.ReorderQty = shelf.reorderqty;
                shelfmaster.Bin = shelf.bin;
                shelfmaster.HazardSurcharge = shelf.hazardsurcharge;
                shelfmaster.FlammableSurcharge = shelf.flammablesurcharge;
                shelfmaster.HeatSurcharge = shelf.heatsurcharge;
                shelfmaster.RefrigSurcharge = shelf.refrigsurcharge;
                shelfmaster.FreezerSurcharge = shelf.freezersurcharge;
                shelfmaster.CleanSurcharge = shelf.cleansurcharge;
                shelfmaster.BlendSurcharge = shelf.blendsurcharge;
                shelfmaster.NalgeneSurcharge = shelf.nalgenesurcharge;
                shelfmaster.NitrogenSurcharge = shelf.nitrogensurcharge;
                shelfmaster.BiocideSurcharge = shelf.biocidesurcharge;
                shelfmaster.KosherSurcharge = shelf.koshersurcharge;
                shelfmaster.LabelSurcharge = shelf.labelsurcharge;
                shelfmaster.OtherSurcharge = shelf.othersurcharge;
                shelfmaster.OtherSurchargeAmt = shelf.othersurchargeamt;
                shelfmaster.OtherSurchargeDescription = shelf.othersurchargedescription;
                shelfmaster.NewItem = shelf.newitem;
                shelfmaster.InactiveSize = shelf.inactivesize;
                shelfmaster.WebOEInclude = shelf.weboeinclude;
                shelfmaster.SortOrder = shelf.sortorder;
                shelfmaster.PackageID = shelf.packageid;
                shelfmaster.Notes = shelf.notes;
                shelfmaster.Discontinued = shelf.discontinued;
                shelfmaster.Alert = shelf.alert;
                shelfmaster.CustCode = shelf.custcode;

                db.SaveChanges();
            }
        }

        /// <summary>
        /// Delete shelf master
        /// </summary>
        /// <param name="shelfid"></param>
        public static void DeleteShelf(int shelfid)
        {
            using (var db = new CMCSQL03Entities())
            {
                string deleteQuery = "DELETE FROM tblShelfMaster WHERE ShelfID=" + shelfid;
                db.Database.ExecuteSqlCommand(deleteQuery);
            }
        }

        #endregion Shelf Master

        /// <summary>
        /// Get shelf id using product detail id. If none is found; create a new shelf id
        /// </summary>
        /// <param name="productdetailid"></param>
        /// <param name="size"></param>
        /// <returns>shelfId</returns>
        public static int GetShelfIdProductDetail(int? productdetailid, string size)
        {
            int shelfId;

            using (var db = new CMCSQL03Entities())
            {
                int getShelfId = (from t in db.tblShelfMaster
                                  where t.ProductDetailID == productdetailid
                                  && t.Size == size
                                  select t.ShelfID).FirstOrDefault();

                if (getShelfId != 0)
                {
                    shelfId = getShelfId;
                }
                else
                {
                    var newShelfMaster = new tblShelfMaster
                    {
                        ProductDetailID = productdetailid,
                        Size = size
                    };

                    db.tblShelfMaster.Add(newShelfMaster);
                    db.SaveChanges();

                    shelfId = newShelfMaster.ShelfID;
                }
            }

            return shelfId;
        }

        /// <summary>
        /// Get shelf id using product master id
        /// </summary>
        /// <param name="productmasterid"></param>
        /// <param name="um"></param>
        /// <returns>shelfId</returns>
        public static int GetShelfIdProductMaster(int? productmasterid, string um)
        {
            int productDetailId = ProductsService.GetProductDetailId(productmasterid);
            int shelfId = GetShelfIdProductDetail(productDetailId, um);

            return shelfId;
        }

        /// <summary>
        /// Gets unit weight from shelfmaster
        /// </summary>
        /// <param name="shelfid"></param>
        /// <returns>unitWeight</returns>
        public static decimal GetUnitWeight(int? shelfid)
        {
            decimal unitWeight;

            using (var db = new CMCSQL03Entities())
            {
                unitWeight = db.tblShelfMaster
                               .Where(x => x.ShelfID == shelfid)
                               .Select(x => x.UnitWeight)
                               .FirstOrDefault() ?? 0;
            }

            return unitWeight;
        }
    }
}