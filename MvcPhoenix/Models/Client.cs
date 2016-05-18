using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

// no user inteface to tblClient


namespace MvcPhoenix.Models
{
    public class Client
    {
        public int ClientID { get; set; }
        public string LogoFileName { get; set; }
        public Nullable<int> LegacyID { get; set; }
        public Nullable<int> GlobalClientID { get; set; }
        public string ClientCode { get; set; }
        public string ClientName { get; set; }
        public string CMCLocation { get; set; }
        public string CustomerReference { get; set; }
        public string CMCLongCustomer { get; set; }
        public Nullable<bool> MSDS { get; set; }
        public Nullable<bool> TDS { get; set; }
        public Nullable<bool> COA { get; set; }
        public string DocumentDirectory { get; set; }
        public string MSDS_Folder { get; set; }
        public string TDS_Folder { get; set; }
        public string COA_Folder { get; set; }
        public string SP_ServiceSummary { get; set; }
        public string SP_Detail1 { get; set; }
        public string SP_Detail2 { get; set; }
        public string SP_Detail3 { get; set; }
        public string SP_Detail4 { get; set; }
        public string SP_Detail5 { get; set; }
        public Nullable<System.DateTime> SP_RevDate { get; set; }
        public string SP_CommValue { get; set; }
        public string SP_COARequired { get; set; }
        public string SP_TDSRequired { get; set; }
        public string SP_MSDSLanguage { get; set; }
        public string SP_Freezable { get; set; }
        public string SP_LabelLanguage { get; set; }
        public string SP_Expedite { get; set; }
        public string BOLComment { get; set; }


        // constructor
        public Client()
        {
            
        }

        public static Client fnFillClient(int id)
        {
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            Client obj = new Client();
            var qry = (from t in db.tblClient
                       where t.ClientID == id
                       select t).FirstOrDefault();
            obj.ClientID = qry.ClientID;
            // Cheat this value until SQL changes flush
            obj.LogoFileName = "http://www.mysamplecenter.com/Logos/" +  qry.CustomerReference.Trim() + ".gif";
            //obj.LogoFileName = "acme80.jpg"; // until all data changes are flushed thru
            obj.LegacyID = qry.LegacyID; obj.GlobalClientID = qry.GlobalClientID;  obj.ClientCode = qry.ClientCode;  obj.ClientName = qry.ClientName;
            obj.CMCLocation = qry.CMCLocation;   obj.CustomerReference = qry.CustomerReference;  obj.CMCLongCustomer = qry.CMCLongCustomer;
            obj.MSDS = qry.MSDS;  obj.TDS = qry.TDS;   obj.COA = qry.COA;   obj.DocumentDirectory = qry.DocumentDirectory;
            obj.MSDS_Folder = qry.MSDS_Folder;  obj.TDS_Folder = qry.TDS_Folder;  obj.COA_Folder = qry.COA_Folder;  obj.SP_ServiceSummary = qry.SP_ServiceSummary;
            obj.SP_Detail1 = qry.SP_Detail1; obj.SP_Detail2 = qry.SP_Detail2;  obj.SP_Detail3 = qry.SP_Detail3;  obj.SP_Detail4 = qry.SP_Detail4; obj.SP_Detail5 = qry.SP_Detail5;
            obj.SP_RevDate = qry.SP_RevDate; obj.SP_CommValue = qry.SP_CommValue; obj.SP_COARequired = qry.SP_COARequired;  obj.SP_TDSRequired = qry.SP_TDSRequired;
            obj.SP_MSDSLanguage = qry.SP_MSDSLanguage; obj.SP_Freezable = qry.SP_Freezable;  obj.SP_LabelLanguage = qry.SP_LabelLanguage;  obj.SP_Expedite = qry.SP_Expedite;
            obj.BOLComment = qry.BOLComment;
            db.Dispose();
            return obj;
        }
    }
}