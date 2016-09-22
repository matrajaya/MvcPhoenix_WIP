using System;

namespace MvcPhoenix.Models
{
    public class GHSViewModel   // full joined viewmodel, all tables
    {
        // detail fields
        public int PHDetailID { get; set; }

        public int? ProductDetailID { get; set; }
        public string PHNumber { get; set; }

        // source fields
        public int PHSourceID { get; set; }

        public string Language { get; set; }
        public string PHStatement { get; set; }

        // PD fields
        public string ProductCode { get; set; }

        public string ProductName { get; set; }
    }

    public class GHSPHDetail    //crud
    {
        public GHSPHDetail()    //constructor
        {
            // set default values here
        }

        // CRUD fields
        public int PHDetailID { get; set; }

        public int? ProductDetailID { get; set; }
        public string PHNumber { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdateUser { get; set; }

        // PD fields
        public string ProductCode { get; set; }

        public string ProductName { get; set; }

        //public List<GHSPHSource> ListOfPHSourceItems { get; set; }
    }

    public class GHSPHSource    //crud
    {
        public GHSPHSource()    //constructor
        {
            // set default values here
        }

        public int PHSourceID { get; set; }
        public string PHNumber { get; set; }
        public string Language { get; set; }
        public string PHStatement { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdateUser { get; set; }

        // lists
    }
}