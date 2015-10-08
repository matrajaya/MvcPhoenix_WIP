using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcPhoenix.Models
{
    public class CMCUser
    {
        private static MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
        
        public int UserID { get; set; }
        public string FullName { get; set; }
        public string LocationDefault { get; set; }
        public string Location { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public Nullable<int> UserLevel { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public Nullable<int> ClientID { get; set; }

        //constructor
        public CMCUser()
        {
            //userlevel = 0;
            //phone = "860-354-3997";
        }

        public static CMCUser fnFillUser(int UserID)
        {
            CMCUser obj = new CMCUser();
            var qry = (from t in db.tblUser
                       where t.UserID == UserID
                       select t).FirstOrDefault();
            obj.UserID=qry.UserID;
            obj.ClientID = qry.ClientID;
            obj.Location=qry.Location;
            obj.UserName=qry.UserName;
            obj.Password=qry.Password;
            obj.UserLevel = qry.UserLevel;
            obj.Email=qry.Email;
            obj.Phone=qry.Phone;
            return obj;
        }
    }
}