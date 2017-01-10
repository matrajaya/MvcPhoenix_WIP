using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

//pc add
using System.Data.OleDb;
using System.Web.Mvc;

namespace MvcPhoenix.Services
{

    public class Packout
    {

        //return "H:\\Inv\\CMCBE.MDB";
        //return "\\\\SX\\Clients\\CMC\\MDBs\\EU\\Inv\\CMCBE.MDB";
        //return @"\\SX\\Clients\\CMC\\MDBs\\EU\\Inv\\CMCBE.MDB";

        //https://www.connectionstrings.com/access/


        public static List<SelectListItem> fnMDBTest()
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                List<SelectListItem> mylist = new List<SelectListItem>();
                mylist.Add(new SelectListItem { Value = "", Text = "" });
                mylist = (from t in db.tblClient
                          orderby t.MDB_CMCBE
                          select new SelectListItem { Value = t.MDB_CMCBE, Text = t.MDB_CMCBE }).Distinct().ToList();
                //mylist.Insert(0, new SelectListItem { Value = "0", Text = "Select Test MDB" });
                mylist.Add(new SelectListItem { Value = "APP_DATA\\CMCBE.MDB", Text = "APP_DATA\\CMCBE.MDB" });
                
                return mylist;
            }
        }



        public static int fnCreatePackOutOrder(int id, int Priority, string cmcmdb)
        {
            // id= bulkid
            // return values
            // 0 = error
            //-1 = packout exists
            // xx = new tblProductmasterID

            

            using (var db = new EF.CMCSQL03Entities())
            {
                int retval = 0;
                string s = "";
                var b = db.tblBulk.Find(id);
                var pmm = db.tblProductMaster.Find(b.ProductMasterID);
                var client = db.tblClient.Find(pmm.ClientID);

                //TODO
                // - add bulkid to tblProductionMaster

                using (System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection())
                {
                    var q = (from t in db.tblBulk
                             join pm in db.tblProductMaster on t.ProductMasterID equals pm.ProductMasterID
                             join pd in db.tblProductDetail on pm.ProductMasterID equals pd.ProductMasterID
                             join dv in db.tblDivision on pd.DivisionID equals dv.DivisionID
                             join sm in db.tblShelfMaster on pd.ProductDetailID equals sm.ProductDetailID
                             where t.BulkID == id
                             select new { t, pd, dv, pm, sm }).ToList();

                    //if posted priority=0, then change to color of the day
                    if (Priority == 0)
                    {
                        Priority = FnTodayColorPriority();
                    }

                    // build and open connection to MDB
                    //string MDBpath = @client.MDB_CMCBE;
                    //string MDBpath = HttpContext.Current.Server.MapPath("~/App_data/CMCBE.MDB");
                    //string MDBpath = cmcmdb;    //dev and testing, pull from View
                    string MDBpath = "";

                    if(cmcmdb=="APP_DATA\\CMCBE.MDB")
                    {
                        MDBpath = HttpContext.Current.Server.MapPath("~/App_data/CMCBE.mdb");
                    }
                    else
                    {
                        MDBpath = cmcmdb;
                    }

                    string MDBconnstring = String.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Persist Security Info=False;", MDBpath);
                    conn.ConnectionString = MDBconnstring;
                    conn.Open();

                    // check for existing packout and bail out if one there
                    s = String.Format("Select Count(*) from tblProductionMaster where Company='{0}' and Division='{1}' and MasterCode='{2}' and (ProductionStage=10 or ProductionStage=20)", client.CMCLongCustomer, q[0].dv.DivisionName, q[0].pm.MasterCode);
                    OleDbCommand cmdExists = new OleDbCommand();
                    cmdExists.Connection = conn;
                    cmdExists.CommandText = s;
                    int ExistingCount = Convert.ToInt32(cmdExists.ExecuteScalar());
                    if (ExistingCount > 0)
                    {
                        retval= -1;
                        return retval;
                    }


                    // build a set of command objects for each hit
                    s = String.Format("Insert Into tblProductionMaster ([CreateDate],[ProdMastCreateDate],[Company],[Division],[MasterCode],[ProdName],[Lot Number],[Bulk Location],[Contents Weight],[Shelf  Life],[Expdt],[CeaseShipDate],[recdate],[Packout],[Priority],[ProductionStage],[AirUN],[Status],[CMCUser],[Heat Prior to Filling],[Moisture],[CleanRoom]) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}',{8},{9},'{10}','{11}','{12}',{13},{14},{15},'{16}','{17}','{18}',{19},{20},{21})", System.DateTime.Now, System.DateTime.Now, client.CMCLongCustomer, q[0].dv.DivisionName, q[0].pm.MasterCode, q[0].pm.MasterName, q[0].t.LotNumber, q[0].t.Bin, q[0].t.CurrentWeight, q[0].pm.ShlfLife, q[0].t.ExpirationDate, q[0].t.CeaseShipDate, q[0].t.ReceiveDate, -1, Priority, 10, q[0].pd.AIRUNNUMBER, q[0].t.BulkStatus, HttpContext.Current.User.Identity.Name, pmm.HeatPriorToFilling, pmm.MoistureSensitive, pmm.CleanRoomEquipment);
                    OleDbCommand cmdInsert = new OleDbCommand();
                    cmdInsert.Connection = conn;
                    //conn.Open();
                    cmdInsert.CommandText = s;
                    cmdInsert.ExecuteNonQuery();

                    // get the new Access PK here using a scalar 
                    OleDbCommand cmd2 = new OleDbCommand();
                    cmd2.Connection = conn;
                    cmd2.CommandText = "Select top 1 id from tblProductionMaster order by id desc";
                    int newPackOutID = Convert.ToInt32(cmd2.ExecuteScalar());

                    foreach (var row in q)
                    {
                        // create tblProductionDetail MDB record
                        // put all calcd values into variables first

                        var qLog = (from t in db.tblInvLog where t.LogType == "SS-SHP" && t.ProductDetailID == row.pd.ProductDetailID select t);

                        DateTime? d1 = DateTime.Now.AddDays(-365);
                        var qSSPY = (from t in qLog where t.LogType == "SS-SHP" && t.ProductDetailID == row.pd.ProductDetailID && t.ShipDate >= d1 select t);
                        int qShelfShippedPastYear = Convert.ToInt32((from t in qSSPY select t.LogQty).Sum());


                        DateTime? d2 = DateTime.Now.AddDays(-120);
                        var qSSP4M = (from t in qLog where t.LogType == "SS-SHP" && t.ProductDetailID == row.pd.ProductDetailID && t.ShipDate >= d2 select t);
                        int qShelfShippedPastFourMonths = Convert.ToInt32((from t in qSSP4M select t.LogQty).Sum());

                        decimal? dReorderMin = 0;
                        if (row.pm.ProductSetupDate <= DateTime.Now.AddDays(-365))
                        {
                            dReorderMin = ((qShelfShippedPastYear / 12) * 2);
                        }
                        else
                        {
                            dReorderMin = (qShelfShippedPastFourMonths / 2);
                        }

                        int dPK = newPackOutID;
                        string dUM = row.sm.Size;
                        decimal? dUnitWeight = row.sm.UnitWeight;
                        decimal? dReorderMax = (dReorderMin * 2);
                        var qStock = (from t in db.tblStock where t.ShelfID == row.sm.ShelfID && t.ShelfStatus == "AVAIL" select t);
                        decimal? dOnHand = (from t in qStock select t.QtyOnHand).Sum();
                        decimal? dRecQty = dReorderMax - dOnHand;
                        string dStatus = row.t.BulkStatus;
                        string dProductCode = row.pd.ProductCode;
                        string dProductName = row.pd.ProductName;
                        string dShelfStockLocation = row.sm.Bin;
                        var dPackagePartNumber = (from t in db.tblPackage where t.PackageID == row.sm.PackageID select t.PartNumber).FirstOrDefault();

                        // build detail insert
                        s = String.Format(@"Insert into tblProductionDetail ([MasterID],[UM],[Unit Weight],[SS_REORDMIN],[SS_REORDMAX],[OnHand],[RecQty],[status],
                                [prodcode],[ProductName],[ShelfStockLocation],[PackagePartNumber]) 
                                Values ({0},'{1}',{2},{3},{4},{5},{6},'{7}','{8}','{9}','{10}','{11}')", dPK, dUM, dUnitWeight, dReorderMin, dReorderMax, dOnHand, dRecQty, dStatus, dProductCode, dProductName, dShelfStockLocation, dPackagePartNumber);

                        OleDbCommand cmdInsertDetail = new OleDbCommand();
                        cmdInsertDetail.Connection = conn;
                        cmdInsertDetail.CommandText = s;
                        cmdInsertDetail.ExecuteNonQuery();
                        retval = dPK;
                    }
                    //return retval;
                }

                return retval;
            }
            //return retval;
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

        public static void stash()
        {
            // ----------------------------------- DEVELOPMENT
            // delete last packout order to make dev easier
            //if (HttpContext.Current.Request.IsLocal)
            //{
            //    s = String.Format("Select top 1 Id from tblProductionMaster order by id desc");
            //    OleDbCommand cmd01 = new OleDbCommand();
            //    cmd01.Connection = conn;
            //    cmd01.CommandText = s;
            //    int PKDelete = Convert.ToInt32(cmd01.ExecuteScalar());
            //    s = String.Format("Delete * from tblProductionMaster where id={0}", PKDelete);
            //    OleDbCommand cmd02 = new OleDbCommand();
            //    cmd02.Connection = conn;
            //    cmd02.CommandText = s;
            //    cmd02.ExecuteNonQuery();
            //    s = String.Format("Delete * from tblProductionDetail where Masterid={0}", PKDelete);
            //    OleDbCommand cmd03 = new OleDbCommand();
            //    cmd03.Connection = conn;
            //    cmd03.CommandText = s;
            //    cmd03.ExecuteNonQuery();
            //    // ----------------------------------- DEVELOPMENT
            //}



            // check status of bulk before beginning
            //using (var db = new EF.CMCSQL03Entities())
            //{
            //    string s = "";
            //    var qBulk = (from t in db.tblBulk where t.BulkID == id select t).FirstOrDefault();
            //    switch (qBulk.BulkStatus)
            //    {
            //        case "QC":
            //            s = String.Format("Update tblBulk set Packout=0 where Bulkid={0}", id);
            //            db.Database.ExecuteSqlCommand(s);
            //            return "BulkIsInQCStatus";
            //            break;
            //        case "HOLD":
            //        case "TEST":
            //        case "RETURN":
            //            s = String.Format("Update tblBulk set Packout=0 where Bulkid={0}", id);
            //            db.Database.ExecuteSqlCommand(s);
            //            return "BulkIsInOtherStatus";
            //            break;
            //        default:
            //            break;
            //    }
            //}
        
        }



        public static void ConnectToAccess()
        {
            System.Data.OleDb.OleDbConnection conn = new
                System.Data.OleDb.OleDbConnection();
            // TODO: Modify the connection string and include any
            // additional required properties for your database.
            conn.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;" +
                @"Data source= C:\Clients\CMC\MDBS\EU\Inv\CMCBE.MDB";
            try
            {
                conn.Open();
                // Insert code to process data.
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Failed to connect to data source");
            }
            finally
            {
                conn.Close();
            }
        }

    }
}