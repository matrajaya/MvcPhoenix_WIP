using MvcPhoenix.EF;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;

// ************** ApplicationService.cs *****************
// This class contains Application level methods
// ******************************************************

namespace MvcPhoenix.Models
{
    public class ApplicationService
    {
        public static void EmailSmtpSend(string from, string to, string subject, string body)
        {
            const string hostName = "secure.emailsrvr.com";
            const string userName = "mailman@chemicalmarketing.com";
            const string password = "SamPles!23";
            const int port = 587;

            var msg = new System.Net.Mail.MailMessage();

            msg.From = new MailAddress(from);
            msg.To.Add(new MailAddress(to));
            msg.Bcc.Add(new MailAddress(from)); //send email copy to self
            msg.Subject = subject;
            msg.Body = body;
            msg.IsBodyHtml = true;

            using (var smtp = new System.Net.Mail.SmtpClient())
            {
                var credential = new System.Net.NetworkCredential
                {
                    UserName = userName,
                    Password = password,
                };

                smtp.Credentials = credential;
                smtp.Host = hostName;
                smtp.Port = port;
                smtp.EnableSsl = true;

                smtp.Send(msg);
            }
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

        #endregion generic select lists

        #region specific select lists

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

        #endregion specific select lists
    }
}