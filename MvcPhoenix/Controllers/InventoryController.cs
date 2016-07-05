using MvcPhoenix.Models;
using PagedList;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MvcPhoenix.Controllers
{
    public class InventoryController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Search(string sortOrder, string currentFilter, string searchString, int? page)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                ViewBag.CurrentSort = sortOrder;
                ViewBag.CodeSortParm = String.IsNullOrEmpty(sortOrder) ? "code_desc" : "";
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

                if (searchString != null)
                {
                    page = 1;
                }
                else { searchString = currentFilter; }

                ViewBag.CurrentFilter = searchString;
                ViewBag.SearchString = searchString;

                var productCodes = from p in db.tblProductDetail select p;

                if (!String.IsNullOrEmpty(searchString))
                {
                    productCodes = productCodes.Where(p => p.ProductCode.Contains(searchString)
                        || p.ProductName.Contains(searchString));
                }

                switch (sortOrder)
                {
                    case "name":
                        productCodes = productCodes.OrderBy(p => p.ProductName);
                        break;

                    case "name_desc":
                        productCodes = productCodes.OrderByDescending(p => p.ProductName);
                        break;

                    case "code_desc":
                        productCodes = productCodes.OrderByDescending(p => p.ProductCode);
                        break;

                    default:
                        productCodes = productCodes.OrderBy(p => p.ProductCode);
                        break;
                }

                int pageSize = 10;
                int pageNumber = (page ?? 1);

                return View(productCodes.ToPagedList(pageNumber, pageSize));
            }
        }

        public ActionResult Edit()
        {
            return View();
        }

        public ActionResult ShelfStock()
        {
            return View();
        }

        public ActionResult BulkStock()
        {
            return View();
        }

        // Action To call Edit Bulk Container View
        // View needs to be moved to Inventory and cleaned up
        public ActionResult EditBulkContainer(int id)
        {
            // Called with a parameter BulkID, build an obj and return edit view
            ViewBag.SearchName = null;
            BulkContainerViewModel obj = new BulkContainerViewModel();
            obj = fnFillBulkContainer(id);
            return View(obj);
        }

        private BulkContainerViewModel fnFillBulkContainer(int id)
        {
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            var qry = (from t in db.tblBulk
                       join t2 in db.tblProductMaster on t.ProductMasterID equals t2.ProductMasterID
                       let cname = (from c in db.tblClient where c.ClientID == t2.ClientID select c.ClientName).FirstOrDefault()
                       where t.BulkID == id
                       select new BulkContainerViewModel
                       {
                           clientid = t2.ClientID,
                           clientname = cname,
                           bulkid = t.BulkID,
                           warehouse = t.Warehouse,
                           receivedate = t.ReceiveDate,
                           carrier = t.Carrier,
                           receivedby = t.ReceivedBy,
                           enteredby = t.EnteredBy,
                           productmasterid = t.ProductMasterID,
                           receiveweight = t.ReceiveWeight,
                           lotnumber = t.LotNumber,
                           mfgdate = t.MfgDate,
                           expirationdate = t.ExpirationDate,
                           ceaseshipdate = t.CeaseShipDate,
                           bulkstatus = t.BulkStatus,
                           um = t.UM,
                           containercolor = t.ContainerColor,
                           bin = t.Bin,
                           containertype = t.ContainerType,
                           coaincluded = t.COAIncluded,
                           msdsincluded = t.MSDSIncluded,
                           containernotes = t.ContainerNotes,
                           currentweight = t.CurrentWeight,
                           qcdate = t.QCDate,
                           returnlocation = t.ReturnLocation,
                           noticedate = t.NoticeDate,
                           bulklabelnote = t.BulkLabelNote,
                           receivedascode = t.ReceivedAsCode,
                           receivedasname = t.ReceivedAsName
                       }).FirstOrDefault();
            // Have to fill list items after object is built

            // Fix these when I get to Inventory
            //qry.ListOfWareHouses =ReceivingService.
            //qry.ListOfProductMasters = fnProductMasterIDs(qry.clientid);
            //qry.ListOfBulkStatusIDs = fnBulkStatusIDs();
            //qry.ListOfContainerTypeIDs = fnContainerTypeIDs();
            db.Dispose();
            return qry;
        }

        #region Search Related Actions

        [HttpPost]
        public ActionResult SearchBulkContainerUserCriteria(FormCollection fc)
        {
            // Reads posted form inputs and builds a custom qry, then returns a partial for grid
            System.Threading.Thread.Sleep(1000);
            ViewBag.SearchName = "User Criteria:";
            int rowcount = Convert.ToInt32(fc["rowcount"]);
            int clientid = Convert.ToInt32(fc["clientid"]);
            string mywarehouse = fc["warehouse"];

            using (var db = new EF.CMCSQL03Entities())
            {
                List<BulkContainerSearchResults> mylist = new List<BulkContainerSearchResults>();
                mylist = (from t in db.tblBulk
                          join t2 in db.tblProductMaster on t.ProductMasterID equals t2.ProductMasterID
                          let cname = (from c in db.tblClient where c.ClientID == t2.ClientID select c.ClientName).FirstOrDefault()
                          orderby t.ReceiveDate descending
                          where t2.ClientID == clientid
                          select new BulkContainerSearchResults
                          {
                              bulkid = t.BulkID,
                              warehouse = t.Warehouse,
                              receivedate = t.ReceiveDate,
                              carrier = t.Carrier,
                              productmasterid = t.ProductMasterID,
                              mastercode = t2.MasterCode,
                              mastername = t2.MasterName,
                              lotnumber = t.LotNumber,
                              bulkstatus = t.BulkStatus,
                              um = t.UM,
                              bin = t.Bin,
                              containertype = t.ContainerType,
                              clientname = cname,
                              expirationdate = t.ExpirationDate,
                              currentweight = t.CurrentWeight
                          }).Take(rowcount).ToList();

                if (mywarehouse != "")
                {
                    mylist = mylist.Where(x => x.warehouse == mywarehouse).ToList();
                }
                return PartialView("~/Views/Receiving/_SearchResults.cshtml", mylist);
            }
        }

        private List<BulkContainerSearchResults> fnDefaultResults()
        {
            // This method is the base qry for the Qucik Search links on the Landing page
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            List<BulkContainerSearchResults> mylist = new List<BulkContainerSearchResults>();
            mylist = (from t in db.tblBulk
                      join t2 in db.tblProductMaster on t.ProductMasterID equals t2.ProductMasterID
                      let cname = (from c in db.tblClient where c.ClientID == t2.ClientID select c.ClientName).FirstOrDefault()
                      orderby t.ReceiveDate descending
                      select new BulkContainerSearchResults
                      {
                          bulkid = t.BulkID,
                          warehouse = t.Warehouse,
                          receivedate = t.ReceiveDate,
                          carrier = t.Carrier,
                          productmasterid = t.ProductMasterID,
                          mastercode = t2.MasterCode,
                          mastername = t2.MasterName,
                          lotnumber = t.LotNumber,
                          bulkstatus = t.BulkStatus,
                          um = t.UM,
                          bin = t.Bin,
                          containertype = t.ContainerType,
                          clientname = cname,
                          expirationdate = t.ExpirationDate,
                          currentweight = t.CurrentWeight
                      }).ToList();
            db.Dispose();

            return mylist;
        }

        [HttpGet]
        public ActionResult SearchInReceiving()
        {
            ViewBag.SearchName = "In Receiving:";
            using (var db = new EF.CMCSQL03Entities())
            {
                List<BulkContainerSearchResults> mylist = fnDefaultResults();
                mylist = mylist.Where(x => x.bulkstatus == "RECD").ToList();
                return PartialView("~/Views/Receiving/_SearchResults.cshtml", mylist);
            }
        }

        [HttpGet]
        public ActionResult SearchNonAvailable()
        {
            ViewBag.SearchName = "Status = Not Available:";
            using (var db = new EF.CMCSQL03Entities())
            {
                List<BulkContainerSearchResults> mylist = fnDefaultResults();
                mylist = mylist.Where(x => x.bulkstatus != "AVAIL").ToList();
                return PartialView("~/Views/Receiving/_SearchResults.cshtml", mylist);
            }
        }

        [HttpGet]
        public ActionResult SearchExpired()
        {
            ViewBag.SearchName = "Expired Containers:";
            using (var db = new EF.CMCSQL03Entities())
            {
                List<BulkContainerSearchResults> mylist = fnDefaultResults();
                mylist = mylist.Where(x => x.expirationdate <= DateTime.Now).ToList();
                return PartialView("~/Views/Receiving/_SearchResults.cshtml", mylist);
            }
        }

        #endregion Search Related Actions
    }
}