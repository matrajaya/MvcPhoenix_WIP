using System;
using System.Linq;
using System.Web;

namespace MvcPhoenix.Services
{
    public class Packout
    {
        public static int fnCreatePackOutOrder(int id, int Priority)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                int retval = 0;
                string s = "";
                var bulk = db.tblBulk.Find(id);
                var pmaster = db.tblProductMaster.Find(bulk.ProductMasterID);
                var client = db.tblClient.Find(pmaster.ClientID);
                
				// check to see if there is an open packout already 
                var qPackout = (from t in db.tblProductionMaster
                                where t.Company == client.CMCLongCustomer && t.MasterCode == pmaster.MasterCode && (t.ProductionStage == 10 | t.ProductionStage == 20)
                                select t).FirstOrDefault();
                if (qPackout != null)
                {
                    retval = -1;
                    return retval;
                }

                // gather a list of what we need to packout
                var q = (from t in db.tblBulk
                         join pm in db.tblProductMaster on t.ProductMasterID equals pm.ProductMasterID
                         join pd in db.tblProductDetail on pm.ProductMasterID equals pd.ProductMasterID
                         join sm in db.tblShelfMaster on pd.ProductDetailID equals sm.ProductDetailID
                         where t.BulkID == id
                         select new { t, pd, pm, sm }).ToList();

                if (Priority == 0)
                { Priority = FnTodayColorPriority(); }

                // insert master
                MvcPhoenix.EF.tblProductionMaster newrec = new MvcPhoenix.EF.tblProductionMaster();

                db.tblProductionMaster.Add(newrec);
                db.SaveChanges();

                int newPackOutID = newrec.ID;
                retval = newPackOutID;

                var newMaster = (from t in db.tblProductionMaster
                                 where t.ID == newPackOutID
                                 select t).FirstOrDefault();

                // fill master record values from bulk-pm-pd-dv
                newMaster.BulkID = bulk.BulkID;
				newMaster.ClientID = client.ClientID;
                newMaster.CreateDate = DateTime.UtcNow;
                newMaster.ProdmastCreateDate = DateTime.UtcNow;
                newMaster.Company = client.CMCLongCustomer;
                newMaster.Division = "n/a";
                newMaster.MasterCode = pmaster.MasterCode;
                newMaster.ProdName = pmaster.MasterName;
                newMaster.Lot_Number = bulk.LotNumber;
                newMaster.Bulk_Location = bulk.Bin;
                newMaster.Contents_Weight = bulk.CurrentWeight;
                newMaster.Shelf__Life = pmaster.ShlfLife;
                newMaster.ExpDt = bulk.ExpirationDate;
                newMaster.CeaseShipDate = bulk.CeaseShipDate;
                newMaster.RecDate = bulk.ReceiveDate;
                newMaster.Packout = true;
                newMaster.Priority = Priority;
                newMaster.ProductionStage = 10;
                newMaster.AirUN = "n/a";
                newMaster.Status = bulk.BulkStatus;
                newMaster.CMCUser = HttpContext.Current.User.Identity.Name;
                newMaster.Heat_Prior_To_Filling = pmaster.HeatPriorToFilling;
                newMaster.Moisture = pmaster.MoistureSensitive;
                newMaster.CleanRoom = pmaster.CleanRoomEquipment;

                db.SaveChanges();

                foreach (var row in q)
                {
                    // insert detail records from q
                    MvcPhoenix.EF.tblProductionDetail newdetail = new MvcPhoenix.EF.tblProductionDetail();
                    newdetail.MasterID = newMaster.ID;
                    newdetail.ShelfID = row.sm.ShelfID;

					newdetail.InvRequestedQty = 0;
                    newdetail.ProdActualQty = 0;
                    newdetail.LabelQty = 0;
					
                    var qLog = (from t in db.tblInvLog
                                where t.LogType == "SS-SHP" && t.ProductDetailID == row.pd.ProductDetailID
                                select t);

                    DateTime? d1 = DateTime.UtcNow.AddDays(-365);

                    var qSSPY = (from t in qLog
                                 where t.LogType == "SS-SHP" && t.ProductDetailID == row.pd.ProductDetailID && t.ShipDate >= d1
                                 select t);

                    int qShelfShippedPastYear = Convert.ToInt32((from t in qSSPY
                                                                 select t.LogQty).Sum());

                    DateTime? d2 = DateTime.UtcNow.AddDays(-120);

                    var qSSP4M = (from t in qLog
                                  where t.LogType == "SS-SHP" && t.ProductDetailID == row.pd.ProductDetailID && t.ShipDate >= d2
                                  select t);

                    int qShelfShippedPastFourMonths = Convert.ToInt32((from t in qSSP4M
                                                                       select t.LogQty).Sum());

                    decimal? dReorderMin = 0;

                    if (row.pm.ProductSetupDate <= DateTime.UtcNow.AddDays(-365))
                    {
                        dReorderMin = ((qShelfShippedPastYear / 12) * 2);
                    }
                    else
                    {
                        dReorderMin = (qShelfShippedPastFourMonths / 2);
                    }

                    //int dPK = newPackOutID;
                    newdetail.UM = row.sm.Size; // string dUM = row.sm.Size;
                    newdetail.Unit_Weight = row.sm.UnitWeight; //decimal? dUnitWeight = row.sm.UnitWeight;
                    newdetail.SS_REORDMIN = dReorderMin;
                    newdetail.SS_REORDMAX = (dReorderMin * 2); // decimal? dReorderMax = (dReorderMin * 2);

                    var qStock = (from t in db.tblStock
                                  where t.ShelfID == row.sm.ShelfID && t.ShelfStatus == "AVAIL"
                                  select t);

                    newdetail.OnHand = (from t in qStock
                                        select t.QtyOnHand).Sum(); //decimal? dOnHand = (from t in qStock select t.QtyOnHand).Sum();

                    newdetail.RecQty = newdetail.SS_REORDMAX - newdetail.OnHand; //decimal? dRecQty = dReorderMax - dOnHand;
                    newdetail.Status = row.t.BulkStatus; // string dStatus = row.t.BulkStatus;
                    newdetail.ProdCode = row.pd.ProductCode; // string dProductCode = row.pd.ProductCode;
                    newdetail.ProductName = row.pd.ProductName; //string dProductName = row.pd.ProductName;
                    newdetail.ShelfStockLocation = row.sm.Bin; //string dShelfStockLocation = row.sm.Bin;

                    var dPackage = (from t in db.tblPackage
                                    where t.PackageID == row.sm.PackageID
                                    select t).FirstOrDefault();

                    newdetail.PackagePartNumber = dPackage.PartNumber;

                    db.tblProductionDetail.Add(newdetail);
                    db.SaveChanges();
                }
                return retval;
            }
        }

        private static int FnTodayColorPriority()
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