using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

//pc add
using System.Web.Mvc;
using MvcPhoenix.EF;

// ************** ApplicationService.cs *****************
// This class contains Application level methods
// ******************************************************

namespace MvcPhoenix.Models
{
        
    public class ApplicationService
    {
        static string SmtpHostName = "192.168.0.27";
        static string SmtpUserName = "??";
        static string SmtpPassword = "??";
        static int SmtpPort = 25;
        //static EF.CMCSQL03Entities thisdb = new EF.CMCSQL03Entities();

     public static void fnInsertLog(string UserName,int ClientID,string CalledFrom,string Notes)
        {
            // to be developed - takes parameters and inserts a record into new tblLog
        }


     public static void fnSimpleSendSmtp(string FromAddress,string ToAddress,string Subject,string Body)
     {
         var msg = new System.Net.Mail.MailMessage();
         msg.To.Add(new System.Net.Mail.MailAddress(ToAddress));
         msg.From = new System.Net.Mail.MailAddress(FromAddress);
         msg.Subject = Subject;
         msg.Body = Body;
         msg.IsBodyHtml = true;
         using (var smtp = new System.Net.Mail.SmtpClient())
         {
             var credential = new System.Net.NetworkCredential
             {
                 UserName = SmtpUserName,
                 Password = SmtpPassword
             };
             smtp.Credentials = credential;
             smtp.Host = SmtpHostName;
             smtp.Port = SmtpPort;
             //smtp.EnableSsl = true;
             smtp.Send(msg);
         }

     }

     public static string GetTemplateFromFile(string FileName)
         {
         // Easy way to use a .HTML file as an email template
         string sFileName = HttpContext.Current.Server.MapPath("/Templates/" + FileName);
         System.IO.StreamReader obj = new System.IO.StreamReader(sFileName);
         obj = System.IO.File.OpenText(sFileName);
         string FileContents = obj.ReadToEnd();
         obj.Close();
         return FileContents;
     }






        #region generic select lists

        public static List<SelectListItem> fnClientIDs()
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> mylist = new List<SelectListItem>();
                mylist = (from t in db.tblClient
                          orderby t.ClientName
                          select new SelectListItem { Value = t.ClientID.ToString(), Text = t.ClientName }).ToList();
                mylist.Insert(0, new SelectListItem { Value = "0", Text = "Select Client" });
                return mylist;
            }
        }


        public static List<SelectListItem> fnStates()
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> mylist = new List<SelectListItem>();
                mylist = (from t in db.tblState
                          orderby t.StateName
                          select new SelectListItem { Value = t.StateAbbr, Text = t.StateName }).ToList();
                mylist.Insert(0, new SelectListItem { Value = "0", Text = "Select State" });
                return mylist;
            }
        }


        public static List<SelectListItem> fnCountries()
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> mylist = new List<SelectListItem>();
                mylist = (from t in db.tblCountry
                          orderby t.Country
                          select new SelectListItem { Value = t.CountryID.ToString(), Text = t.Country }).ToList();
                mylist.Insert(0, new SelectListItem { Value = "0", Text = "Select Country" });
                return mylist;
            }
        }


        public static List<SelectListItem> fnCarriers()
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> mylist = new List<SelectListItem>();
                mylist = (from t in db.tblCarrier
                          orderby t.CarrierName
                          select new SelectListItem { Value = t.CarrierName, Text = t.CarrierName }).ToList();
                mylist.Insert(0, new SelectListItem { Value = "0", Text = "Select Carrier" });
                return mylist;
            }
        }


        public static List<SelectListItem> fnHSCodes()
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> mylist = new List<SelectListItem>();
                mylist = (from t in db.tblHSCode
                          orderby t.HarmonizedCode
                          select new SelectListItem { Value = t.HarmonizedCode, Text = t.HarmonizedCode }).ToList();
                mylist.Insert(0, new SelectListItem { Value = "0", Text = "Select Harmonized Code" });
                return mylist;
            }
        }


        public static List<SelectListItem> fnOrderSources()
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> mylist = new List<SelectListItem>();
                mylist = (from t in db.tblOrderSource
                          orderby t.Source
                          select new SelectListItem { Value = t.Source, Text = t.Source }).ToList();
                mylist.Insert(0, new SelectListItem { Value = "0", Text = "Select Order Source" });
                return mylist;
            }
        }

        public static List<SelectListItem> fnOrderTypes()
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> mylist = new List<SelectListItem>();
                mylist = (from t in db.tblOrderType
                          orderby t.Description
                          select new SelectListItem { Value = t.OrderType, Text = t.Description }).ToList();
                mylist.Insert(0, new SelectListItem { Value = "0", Text = "Select Order Type" });
                return mylist;
            }
        }


        public static List<SelectListItem> fnReportCriterias()
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> mylist = new List<SelectListItem>();
                mylist = (from t in db.tblReportCriteria
                          orderby t.Display
                          select new SelectListItem { Value = t.Display, Text = t.ReportName }).ToList();
                mylist.Insert(0, new SelectListItem { Value = "0", Text = "Select Report Criteria" });
                return mylist;
            }
        }

        public static List<SelectListItem> fnStatusNotes()
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> mylist = new List<SelectListItem>();
                mylist = (from t in db.tblStatusNotes
                          orderby t.Note
                          select new SelectListItem { Value = t.StatusNotesID.ToString(), Text = t.Note }).ToList();
                mylist.Insert(0, new SelectListItem { Value = "0", Text = "Select Status Note" });
                return mylist;
            }
        }

        public static List<SelectListItem> fnUsers()
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> mylist = new List<SelectListItem>();
                mylist = (from t in db.tblUser
                          orderby t.UserName
                          select new SelectListItem { Value = t.UserID.ToString(), Text = t.UserName }).ToList();
                mylist.Insert(0, new SelectListItem { Value = "0", Text = "Select User" });
                return mylist;
            }
        }

        #endregion

        #region specific select lists
                
        //public static List<SelectListItem> fnBolComments(int id)
        //{
        //    using (var db = new CMCSQL03Entities())
        //    {
        //        List<SelectListItem> mylist = new List<SelectListItem>();
        //        mylist = (from t in db.tblBOLComment
        //                  where t.ClientID == id
        //                  orderby t.BOLComment
        //                  select new SelectListItem { Value = t.BOLCommentID.ToString(), Text = t.BOLComment }).ToList();
        //        mylist.Insert(0, new SelectListItem { Value = "0", Text = "Select BOL Comment" });
        //        return mylist;
        //    }
        //}

        public static List<SelectListItem> fnBulkSuppliers(int id)
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> mylist = new List<SelectListItem>();
                mylist = (from t in db.tblBulkSupplier
                          where t.ClientID == id
                          orderby t.SupplyID
                          select new SelectListItem { Value = t.BulkSupplierID.ToString(), Text = t.SupplyID }).ToList();
                mylist.Insert(0, new SelectListItem { Value = "0", Text = "Select Supplier" });
                return mylist;
            }
        }
                
        public static List<SelectListItem> fnClientsContacts(int id, string sContactType)
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> mylist = new List<SelectListItem>();

                mylist = (from t in db.tblClientContact
                          where t.ClientID == id && t.ContactType == sContactType
                          orderby t.FullName
                          select new SelectListItem { Value = t.ClientContactID.ToString(), Text = t.FullName }).ToList();
                mylist.Insert(0, new SelectListItem { Value = "0", Text = "Select Supplier" });
                return mylist;
            }
        }

        public static List<SelectListItem> fnDivisions(int id)
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> mylist = new List<SelectListItem>();
                mylist = (from t in db.tblDivision
                          where t.ClientID == id
                          orderby t.Division
                          select new SelectListItem { Value = t.DivisionID.ToString(), Text = t.Division }).ToList();
                mylist.Insert(0, new SelectListItem { Value = "0", Text = "Select Division" });
                return mylist;
            }
        }

        public static List<SelectListItem> fnEndUses(int id)
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> mylist = new List<SelectListItem>();
                mylist = (from t in db.tblEndUse
                          where t.ClientID == id
                          orderby t.EndUse
                          select new SelectListItem { Value = t.EndUseID.ToString(), Text = t.EndUse }).ToList();
                mylist.Insert(0, new SelectListItem { Value = "0", Text = "Select End Use" });
                return mylist;
            }
        }

        //public partial class DTO_Application
        //{

        //}

        //Insert here
        //public ActionResult LookupUN(string UN)
        //{
        //    //string test="Group1";
        //    DTO_UN obj = new DTO_UN();
        //    using (var db = new EF.CMCSQL03Entities())
        //     {
        //         var q = (from t in db.tblUN where t.UNNumber == UN select t).FirstOrDefault();
        //         if (q != null)
        //         {
        //             obj.hazardclass = q.HazardClass;
        //             obj.propershippingname = q.ProperShippingName;
        //             obj.nosname = q.NOSName;
        //             obj.labelreq = q.LabelReq;
        //             obj.subclass = q.SubClass;
        //             obj.subsidlabelreq = q.SubSidLabelReq;
        //             obj.packinggroup = q.PackingGroup;
        //         }
        //     }
        //    return Json(obj,JsonRequestBehavior.AllowGet);

        //}

        //Insert here


        //Insert here


        //Insert here


        //Insert here


        //Insert here


        //Insert here

        #endregion


    }
}