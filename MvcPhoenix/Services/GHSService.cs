using MvcPhoenix.Models;
using System;
using System.Linq;
using System.Web;

namespace MvcPhoenix.Services
{
    public class GHSService
    {
        // full view of data
        public GHSViewModel fnFillGHS(int id)   // id=productdetailid
        {
            using (var db = new MvcPhoenix.EF.CMCSQL03Entities())
            {
                var vm = (from t in db.tblGHSPHDetail
                          join pd in db.tblProductDetail on t.ProductDetailID equals pd.ProductDetailID
                          //join pm in db.tblProductMaster on pd.ProductMasterID equals pm.ProductMasterID
                          join s in db.tblGHSPHSource on t.PHNumber equals s.PHNumber
                          where t.PHDetailID == id
                          select new GHSViewModel
                          {
                              PHDetailID = t.PHDetailID,
                              ProductDetailID = t.ProductDetailID,
                              PHNumber = t.PHNumber,
                              PHSourceID = s.PHsourceID,
                              Language = s.Language,
                              PHStatement = s.PHStatement,
                              ProductCode = pd.ProductCode,
                              ProductName = pd.ProductName
                          }).FirstOrDefault();

                return vm;
            }
        }

        public GHSPHDetail fnFillGHSPHDetail(int id)    //crud
        {
            using (var db = new MvcPhoenix.EF.CMCSQL03Entities())
            {
                var vm = (from t in db.tblGHSPHDetail
                          where t.PHDetailID == id
                          select new GHSPHDetail
                          {
                              PHDetailID = t.PHDetailID,
                              ProductDetailID = t.ProductDetailID,
                              PHNumber = t.PHNumber,
                              CreateDate = t.CreateDate,
                              CreateUser = t.CreateUser,
                              UpdateDate = t.UpdateDate,
                              UpdateUser = t.UpdateUser,
                          }).FirstOrDefault();

                return vm;
            }
        }

        public GHSPHSource fnFillGHSPHSource(int id)    //crud
        {
            using (var db = new MvcPhoenix.EF.CMCSQL03Entities())
            {
                var vm = (from t in db.tblGHSPHSource
                          where t.PHsourceID == id
                          select new GHSPHSource
                          {
                              PHSourceID = t.PHsourceID,
                              PHNumber = t.PHNumber,
                              Language = t.Language,
                              PHStatement = t.PHStatement,
                              CreateDate = t.CreateDate,
                              CreateUser = t.CreateUser,
                              UpdateDate = t.UpdateDate,
                              UpdateUser = t.UpdateUser
                          }).FirstOrDefault();

                return vm;
            }
        }

        public static void fnSaveGHSPHDetail(GHSPHDetail vm)
        {
            using (var db = new MvcPhoenix.EF.CMCSQL03Entities())
            {
                if (vm.PHDetailID == -1)
                {
                    vm.PHDetailID = fnNewGHSPHDetailID();
                }

                var dbrow = (from t in db.tblGHSPHDetail
                             where t.PHDetailID == vm.PHDetailID
                             select t).FirstOrDefault();

                {
                    dbrow.ProductDetailID = vm.ProductDetailID;
                    dbrow.PHNumber = vm.PHNumber;
                    dbrow.UpdateDate = System.DateTime.Now;
                    dbrow.UpdateUser = HttpContext.Current.User.Identity.Name;

                    db.SaveChanges();
                }
            }
        }

        public static int fnNewGHSPHDetailID()
        {
            using (var db = new MvcPhoenix.EF.CMCSQL03Entities())
            {
                var newrecord = new EF.tblGHSPHDetail { };
                db.tblGHSPHDetail.Add(newrecord);
                db.SaveChanges();
                int newpk = newrecord.PHDetailID;

                return newpk;
            }
        }

        public static void fnSaveGHSPHSource(GHSPHSource vm)
        {
            using (var db = new MvcPhoenix.EF.CMCSQL03Entities())
            {
                if (vm.PHSourceID == -1)
                {
                    vm.PHSourceID = fnNewGHSPHSourceID();
                }

                var dbrow = (from t in db.tblGHSPHSource
                             where t.PHsourceID == vm.PHSourceID
                             select t).FirstOrDefault();

                dbrow.PHNumber = vm.PHNumber;
                dbrow.Language = vm.Language;
                dbrow.PHStatement = vm.PHStatement;
                dbrow.UpdateDate = System.DateTime.Now;
                dbrow.UpdateUser = HttpContext.Current.User.Identity.Name;

                db.SaveChanges();
            }
        }

        public static int fnNewGHSPHSourceID()
        {
            using (var db = new MvcPhoenix.EF.CMCSQL03Entities())
            {
                var newrecord = new EF.tblGHSPHSource { };
                db.tblGHSPHSource.Add(newrecord);
                db.SaveChanges();
                int newpk = newrecord.PHsourceID;

                return newpk;
            }
        }

        public static void fnClonePHNumber(string vPHNumber)
        {
            // TODO: finish this
            using (var db = new MvcPhoenix.EF.CMCSQL03Entities())
            {
                // fix to modify the PhNumber accordingly
                string s = String.Format("Insert into tblGHSPHSource select from tblGHSPHSource where PHNumber = '{0}'", vPHNumber);
                db.Database.ExecuteSqlCommand(s);
            }
        }

        public static string fnNextPH(string vPHNumber)
        {
            // calc the suffix to apply to a PHNumber
            string s = "XXXX";
            // TODO: finish this
            return s;
        }
    }
}