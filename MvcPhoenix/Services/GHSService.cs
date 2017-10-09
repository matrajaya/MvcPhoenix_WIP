using MvcPhoenix.EF;
using MvcPhoenix.Models;
using MvcPhoenix.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcPhoenix.Services
{
    public class GHSService
    {
        public static void SaveGHS(GHSViewModel ghs)
        {
            using (var db = new CMCSQL03Entities())
            {
                var getGHS = db.tblGHS
                               .Where(x => x.ProductDetailID == ghs.ProductDetailID)
                               .FirstOrDefault();

                if (getGHS != null)
                {
                    getGHS.SignalWord = ghs.GHSSignalWord;
                    getGHS.Symbol1 = ghs.GHSSymbol1;
                    getGHS.Symbol2 = ghs.GHSSymbol2;
                    getGHS.Symbol3 = ghs.GHSSymbol3;
                    getGHS.Symbol4 = ghs.GHSSymbol4;
                    getGHS.Symbol5 = ghs.GHSSymbol5;
                    getGHS.OtherLabelInfo = ghs.OtherLabelInfo;

                    db.SaveChanges();
                }
                else
                {
                    var newGHS = new tblGHS
                    {
                        ProductDetailID = ghs.ProductDetailID,
                        SignalWord = ghs.GHSSignalWord,
                        Symbol1 = ghs.GHSSymbol1,
                        Symbol2 = ghs.GHSSymbol2,
                        Symbol3 = ghs.GHSSymbol3,
                        Symbol4 = ghs.GHSSymbol4,
                        Symbol5 = ghs.GHSSymbol5,
                        OtherLabelInfo = ghs.OtherLabelInfo
                    };

                    db.tblGHS.Add(newGHS);
                    db.SaveChanges();
                }
            }
        }

        public static List<GHSDetail> GetGHSDetails(int productDetailId)
        {
            var ghsDetails = new List<GHSDetail>();

            using (var db = new CMCSQL03Entities())
            {
                var phDetail = (from phdetail in db.tblGHSPHDetail
                                join phsource in db.tblGHSPHSource on phdetail.PHNumber equals phsource.PHNumber
                                where phdetail.ProductDetailID == productDetailId
                                orderby phdetail.PHNumber ascending
                                select new
                                {
                                    phdetail.PHDetailID,
                                    phdetail.ProductDetailID,
                                    phdetail.ExcludeFromLabel,
                                    phdetail.PHNumber,
                                    phsource.Language,
                                    phsource.PHStatement,
                                    phsource.CreateUser
                                }).ToList();

                foreach (var item in phDetail)
                {
                    ghsDetails.Add(new GHSDetail()
                    {
                        PHDetailID = item.PHDetailID,
                        ProductDetailID = Convert.ToInt32(item.ProductDetailID),
                        ExcludeFromLabel = item.ExcludeFromLabel,
                        PHNumber = item.PHNumber,
                        Language = item.Language,
                        PHStatement = item.PHStatement,
                        CreateUser = item.CreateUser
                    });
                }
            }

            return ghsDetails;
        }

        public static GHSViewModel GetGHS(int productDetailId)
        {
            var ghs = new GHSViewModel();

            using (var db = new CMCSQL03Entities())
            {
                var getGHS = db.tblGHS
                               .Where(x => x.ProductDetailID == productDetailId)
                               .FirstOrDefault();

                ghs.GHSID = getGHS.GHSID;
                ghs.ProductDetailID = getGHS.ProductDetailID;
                ghs.GHSSignalWord = getGHS.SignalWord;
                ghs.GHSSymbol1 = getGHS.Symbol1;
                ghs.GHSSymbol2 = getGHS.Symbol2;
                ghs.GHSSymbol3 = getGHS.Symbol3;
                ghs.GHSSymbol4 = getGHS.Symbol4;
                ghs.GHSSymbol5 = getGHS.Symbol5;
                ghs.OtherLabelInfo = getGHS.OtherLabelInfo;
            }

            return ghs;
        }

        public static void CreatePHDetail(int phsourceid, int productdetailid)
        {
            using (var db = new CMCSQL03Entities())
            {
                var phSource = db.tblGHSPHSource.Find(phsourceid);

                var newPhDetail = new tblGHSPHDetail
                {
                    PHNumber = phSource.PHNumber,
                    ProductDetailID = productdetailid,
                    CreateDate = DateTime.UtcNow,
                    CreateUser = HttpContext.Current.User.Identity.Name,
                    UpdateDate = DateTime.UtcNow,
                    UpdateUser = HttpContext.Current.User.Identity.Name,
                    ExcludeFromLabel = false
                };

                db.tblGHSPHDetail.Add(newPhDetail);
                db.SaveChanges();
            }
        }

        public static GHSPHSource EditPHSource(string phnumber, string language)
        {
            var phSource = new GHSPHSource();

            using (var db = new CMCSQL03Entities())
            {
                var getPHSource = db.tblGHSPHSource
                                    .Where(t => t.PHNumber == phnumber
                                             && t.Language == language)
                                    .FirstOrDefault();

                if (getPHSource != null)
                {
                    phSource.PHSourceID = getPHSource.PHsourceID;
                    phSource.PHNumber = getPHSource.PHNumber;
                    phSource.Language = getPHSource.Language;
                    phSource.PHStatement = getPHSource.PHStatement;
                }
            }

            return phSource;
        }

        public static Tuple<GHSPHSource, string> CloneAltPHSource(int phSourceId)
        {
            var PHSource = new GHSPHSource();
            string originalPHNumber;

            //string[] suffix = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
            char[] suffix = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

            using (var db = new CMCSQL03Entities())
            {
                var phSource = db.tblGHSPHSource.Find(phSourceId);
                originalPHNumber = phSource.PHNumber;

                int i = 0;
                PHSource.PHNumber = phSource.PHNumber + "-" + suffix[i];
                var isExists = db.tblGHSPHSource.Any(r => r.PHNumber.Equals(PHSource.PHNumber));

                while (isExists)
                {
                    ++i;
                    PHSource.PHNumber = phSource.PHNumber + "-" + suffix[i];
                    isExists = db.tblGHSPHSource.Any(r => r.PHNumber.Equals(PHSource.PHNumber));
                }

                PHSource.PHSourceID = phSource.PHsourceID;
                PHSource.Language = phSource.Language;
                PHSource.PHStatement = phSource.PHStatement;
            }

            return Tuple.Create(PHSource, originalPHNumber);
        }

        public static int SavePHSourceClone(GHSPHSource phsource, string originalPHNumber)
        {
            int phSourceId = phsource.PHSourceID;

            using (var db = new CMCSQL03Entities())
            {
                var phSource = db.tblGHSPHSource
                                 .Where(x => x.PHNumber == originalPHNumber)
                                 .ToList();

                for (int i = 0; i < phSource.Count; i++)
                {
                    var newPHSource = phSource[i].Clone();

                    newPHSource.PHNumber = phsource.PHNumber;
                    newPHSource.Language = phSource[i].Language;
                    newPHSource.PHStatement = phSource[i].PHStatement;
                    newPHSource.CreateDate = DateTime.UtcNow;
                    newPHSource.CreateUser = HttpContext.Current.User.Identity.Name;

                    if (newPHSource.Language == phsource.Language)
                    {
                        newPHSource.PHStatement = phsource.PHStatement;
                    }

                    db.tblGHSPHSource.Add(newPHSource);
                    db.SaveChanges();

                    phSourceId = newPHSource.PHsourceID;
                }
            }

            return phSourceId;
        }

        public static void UpdatePHSource(GHSPHSource phsource)
        {
            using (var db = new CMCSQL03Entities())
            {
                var phSource = db.tblGHSPHSource.Find(phsource.PHSourceID);

                phSource.PHStatement = phsource.PHStatement;
                phSource.UpdateDate = DateTime.UtcNow;
                phSource.UpdateUser = HttpContext.Current.User.Identity.Name;

                db.SaveChanges();
            }
        }

        public static void UpdatePHDetailExclude(int phDetailId, bool isChecked)
        {
            using (var db = new CMCSQL03Entities())
            {
                var phDetail = db.tblGHSPHDetail.Find(phDetailId);

                phDetail.ExcludeFromLabel = isChecked;

                db.SaveChanges();
            }
        }

        public static void DeletePH(int phdetailid)
        {
            using (var db = new CMCSQL03Entities())
            {
                string deleteQuery = @"DELETE FROM tblGHSPHDetail WHERE PHDetailID=" + phdetailid;
                db.Database.ExecuteSqlCommand(deleteQuery);
            }
        }
    }
}