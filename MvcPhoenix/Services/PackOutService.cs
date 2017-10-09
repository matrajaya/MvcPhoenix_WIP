using MvcPhoenix.EF;
using System;
using System.Linq;
using System.Web;

namespace MvcPhoenix.Services
{
    public class PackoutService
    {
        public static int CreatePackOutOrder(int bulkid, int priority)
        {
            int packoutId = 0;

            using (var db = new CMCSQL03Entities())
            {
                var bulk = db.tblBulk.Find(bulkid);
                var productMaster = db.tblProductMaster.Find(bulk.ProductMasterID);
                var client = db.tblClient.Find(productMaster.ClientID);

                // check to see if there is an open packout already
                var packout = (from t in db.tblProductionMaster
                               where t.Company == client.CMCLongCustomer
                               && t.MasterCode == productMaster.MasterCode
                               && (t.ProductionStage == 10 | t.ProductionStage == 20)
                               select t).FirstOrDefault();

                if (packout != null)
                {
                    packoutId = -1;
                    return packoutId;
                }

                // Get a list of what we need to inform packout
                var packoutBulk = (from bulks in db.tblBulk
                                   join productmaster in db.tblProductMaster on bulks.ProductMasterID equals productmaster.ProductMasterID
                                   join productdetail in db.tblProductDetail on productmaster.ProductMasterID equals productdetail.ProductMasterID
                                   join shelfmaster in db.tblShelfMaster on productdetail.ProductDetailID equals shelfmaster.ProductDetailID
                                   where bulks.BulkID == bulkid
                                   select new { bulks, productdetail, productmaster, shelfmaster }).ToList();

                if (priority == 0)
                {
                    priority = ColorPriorityToday();
                }

                // Create new production master
                var newProductionMaster = new tblProductionMaster();
                db.tblProductionMaster.Add(newProductionMaster);
                db.SaveChanges();

                int newPackOutID = newProductionMaster.ID;

                // Insert production master
                var productionMaster = db.tblProductionMaster.Find(newPackOutID);

                productionMaster.BulkID = bulk.BulkID;
                productionMaster.ClientID = client.ClientID;
                productionMaster.CreateDate = DateTime.UtcNow;
                productionMaster.ProdmastCreateDate = DateTime.UtcNow;
                productionMaster.Company = client.CMCLongCustomer;
                productionMaster.Division = "N/A";
                productionMaster.MasterCode = productMaster.MasterCode;
                productionMaster.ProdName = productMaster.MasterName;
                productionMaster.Lot_Number = bulk.LotNumber;
                productionMaster.Bulk_Location = bulk.Bin;
                productionMaster.Contents_Weight = bulk.CurrentWeight;
                productionMaster.Shelf__Life = productMaster.ShelfLife;
                productionMaster.ExpDt = bulk.ExpirationDate;
                productionMaster.CeaseShipDate = bulk.CeaseShipDate;
                productionMaster.RecDate = bulk.ReceiveDate;
                productionMaster.Packout = true;
                productionMaster.Priority = priority;
                productionMaster.ProductionStage = 10;
                productionMaster.AirUN = "N/A";
                productionMaster.Status = bulk.BulkStatus;
                productionMaster.CMCUser = HttpContext.Current.User.Identity.Name;
                productionMaster.Heat_Prior_To_Filling = productMaster.HeatPriorToFilling;
                productionMaster.Moisture = productMaster.MoistureSensitive;
                productionMaster.CleanRoom = productMaster.CleanRoomEquipment;

                db.SaveChanges();

                packoutId = newPackOutID;

                // Insert production details
                foreach (var row in packoutBulk)
                {
                    var newProductionDetail = new tblProductionDetail();

                    newProductionDetail.MasterID = productionMaster.ID;
                    newProductionDetail.ShelfID = row.shelfmaster.ShelfID;
                    newProductionDetail.InvRequestedQty = 0;
                    newProductionDetail.ProdActualQty = 0;
                    newProductionDetail.LabelQty = 0;
                    newProductionDetail.UM = row.shelfmaster.Size;
                    newProductionDetail.Unit_Weight = row.shelfmaster.UnitWeight;

                    DateTime? oneYearAgo = DateTime.UtcNow.AddDays(-365);
                    DateTime? fourMonthsAgo = DateTime.UtcNow.AddDays(-120);

                    var log = (from t in db.tblInvLog
                               where t.LogType == "SS-SHP"
                               && t.ProductDetailID == row.productdetail.ProductDetailID
                               select t).ToList();

                    var shippedPastYear = log.Where(x => x.ShipDate >= oneYearAgo);
                    var shippedPastFourMonths = log.Where(x => x.ShipDate >= fourMonthsAgo);

                    int? totalShippedPastYear = shippedPastYear.Sum(x => x.LogQty);
                    int? totalShippedPastFourMonths = shippedPastFourMonths.Sum(x => x.LogQty);

                    decimal? reOrderMin = 0;

                    if (row.productmaster.ProductSetupDate <= DateTime.UtcNow.AddDays(-365))
                    {
                        reOrderMin = ((totalShippedPastYear / 12) * 2);
                    }
                    else
                    {
                        reOrderMin = (totalShippedPastFourMonths / 2);
                    }

                    newProductionDetail.SS_REORDMIN = reOrderMin;
                    newProductionDetail.SS_REORDMAX = (reOrderMin * 2);

                    var stock = db.tblStock.Where(x => x.ShelfID == row.shelfmaster.ShelfID
                                                    && x.ShelfStatus == "AVAIL");

                    newProductionDetail.OnHand = stock.Sum(x => x.QtyOnHand);
                    newProductionDetail.RecQty = newProductionDetail.SS_REORDMAX - newProductionDetail.OnHand;
                    newProductionDetail.Status = row.bulks.BulkStatus;
                    newProductionDetail.ProdCode = row.productdetail.ProductCode;
                    newProductionDetail.ProductName = row.productdetail.ProductName;
                    newProductionDetail.ShelfStockLocation = row.shelfmaster.Bin;

                    var package = db.tblPackage.Find(row.shelfmaster.PackageID);
                    newProductionDetail.PackagePartNumber = package.PartNumber;

                    db.tblProductionDetail.Add(newProductionDetail);
                    db.SaveChanges();
                }
            }

            return packoutId;
        }

        private static int ColorPriorityToday()
        {
            switch (System.DateTime.Today.DayOfWeek.ToString())
            {
                case "Monday":
                    return 2;
                    break;

                case "Tuesday":
                    return 3;
                    break;

                case "Wednesday":
                    return 3;
                    break;

                case "Thursday":
                    return 4;
                    break;

                case "Friday":
                    return 6;
                    break;

                default:
                    return 1;
            }
        }
    }
}