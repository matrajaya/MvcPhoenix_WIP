using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace MvcPhoenix.Controllers
{
    public class ReportsController : Controller
    {
        public ActionResult Index()
        {            
            return View();
        }

        /// <summary>
        /// Run Reports using view template. 
        /// Default values are parsed to ReportServer for inital run on page load.
        /// </summary>
        #region Product Coordinators Reports

        public ActionResult Cas(string unnum)
        {
            ViewBag.Title = "Products: CAS Report";

            ReportViewer reportViewer = rptViewerSettings();
            reportViewer.ServerReport.ReportPath = "/SimpleReports/ProductsCAS";
            reportViewer.ServerReport.SetParameters(new ReportParameter("UNNum", unnum));
            ViewBag.ReportViewer = reportViewer;

            return View("~/Views/Reports/View.cshtml");
        }

        public ActionResult ClientHarmonized(string clientid)
        {
            ViewBag.Title = "Products: Client Harmonized Report";

            ReportViewer reportViewer = rptViewerSettings();
            reportViewer.ServerReport.ReportPath = "/SimpleReports/ProductsClientHarmonized";
            reportViewer.ServerReport.SetParameters(new ReportParameter("Client", clientid));
            ViewBag.ReportViewer = reportViewer;

            return View("~/Views/Reports/View.cshtml");
        }

        public ActionResult ClientMSDSDate(string clientid)
        {
            ViewBag.Title = "Products: Client MSDS Date";

            ReportViewer reportViewer = rptViewerSettings();
            reportViewer.ServerReport.ReportPath = "/SimpleReports/ProductsClientMSDSDate";
            reportViewer.ServerReport.SetParameters(new ReportParameter("Client", clientid));
            ViewBag.ReportViewer = reportViewer;

            return View("~/Views/Reports/View.cshtml");
        }

        public ActionResult ClientPackages(string clientid)
        {
            ViewBag.Title = "Products: Client Packages";

            ReportViewer reportViewer = rptViewerSettings();
            reportViewer.ServerReport.ReportPath = "/SimpleReports/ProductsClientPackages";
            reportViewer.ServerReport.SetParameters(new ReportParameter("Client", clientid));
            ViewBag.ReportViewer = reportViewer;

            return View("~/Views/Reports/View.cshtml");
        }

        public ActionResult ClientProductsList(string clientid)
        {
            ViewBag.Title = "Products: Client Products List";

            ReportViewer reportViewer = rptViewerSettings();
            reportViewer.ServerReport.ReportPath = "/SimpleReports/ProductsClientProductsLst";
            reportViewer.ServerReport.SetParameters(new ReportParameter("Client", clientid));
            ViewBag.ReportViewer = reportViewer;

            return View("~/Views/Reports/View.cshtml");
        }

        public ActionResult ClientHazardList(string clientid)
        {
            ViewBag.Title = "Products: Client Hazard List";

            ReportViewer reportViewer = rptViewerSettings();
            reportViewer.ServerReport.ReportPath = "/SimpleReports/ProductsHazardList";
            reportViewer.ServerReport.SetParameters(new ReportParameter("Client", clientid));
            ViewBag.ReportViewer = reportViewer;

            return View("~/Views/Reports/View.cshtml");
        }

        public ActionResult ClientSampleGuideInfo(string clientid)
        {
            ViewBag.Title = "Products: Sample Guide Information";

            ReportViewer reportViewer = rptViewerSettings();
            reportViewer.ServerReport.ReportPath = "/SimpleReports/ProductsSampleGuideInfo";
            reportViewer.ServerReport.SetParameters(new ReportParameter("Client", clientid));
            ViewBag.ReportViewer = reportViewer;

            return View("~/Views/Reports/View.cshtml");
        }

        public ActionResult ClientSGRevDate(string clientid)
        {
            ViewBag.Title = "Products: Sample Guide Revision Date";

            ReportViewer reportViewer = rptViewerSettings();
            reportViewer.ServerReport.ReportPath = "/SimpleReports/ProductsSGRevDate";
            reportViewer.ServerReport.SetParameters(new ReportParameter("Client", clientid));
            ViewBag.ReportViewer = reportViewer;

            return View("~/Views/Reports/View.cshtml");
        }

        #endregion

        #region Inventory Reports

        public ActionResult InvExpiry(string clientid)
        {
            ViewBag.Title = "Inventory: Expiry Report";

            ReportViewer reportViewer = rptViewerSettings();
            reportViewer.ServerReport.ReportPath = "/SimpleReports/InventoryExpiryReport";
            
            ReportParameter[] reportParameter = new ReportParameter[3];
            reportParameter[0] = new ReportParameter("Client", clientid);
            reportParameter[1] = new ReportParameter("StartDate", DateTime.UtcNow.AddDays(-1).ToShortDateString());
            reportParameter[2] = new ReportParameter("EndDate", DateTime.UtcNow.AddDays(-1).ToShortDateString());

            reportViewer.ServerReport.SetParameters(reportParameter);
            ViewBag.ReportViewer = reportViewer;

            return View("~/Views/Reports/View.cshtml");
        }

        public ActionResult InvTotalInStock(string clientid)
        {
            ViewBag.Title = "Inventory: Total In Stock Report";

            ReportViewer reportViewer = rptViewerSettings();
            reportViewer.ServerReport.ReportPath = "/SimpleReports/InventoryTotalInvInStock";
            reportViewer.ServerReport.SetParameters(new ReportParameter("Client", clientid));
            ViewBag.ReportViewer = reportViewer;

            return View("~/Views/Reports/View.cshtml");
        }

        public ActionResult InvOutOfStock(string clientid)
        {
            ViewBag.Title = "Inventory: Out Of Stock Report";

            ReportViewer reportViewer = rptViewerSettings();
            reportViewer.ServerReport.ReportPath = "/SimpleReports/InventoryTotalInvOutOfStock";
            reportViewer.ServerReport.SetParameters(new ReportParameter("Client", clientid));
            ViewBag.ReportViewer = reportViewer;

            return View("~/Views/Reports/View.cshtml");
        }

        public ActionResult InvOpenReplenishment(string clientid)
        {
            ViewBag.Title = "Inventory: Open Replenishment Orders";

            ReportViewer reportViewer = rptViewerSettings();
            reportViewer.ServerReport.ReportPath = "/SimpleReports/InventoryOpenReplenishment";
            reportViewer.ServerReport.SetParameters(new ReportParameter("Client", clientid));
            ViewBag.ReportViewer = reportViewer;

            return View("~/Views/Reports/View.cshtml");
        }

        #endregion

        #region Order Reports

        public ActionResult OrdersOpen(string clientid, string division)
        {
            ViewBag.Title = "Orders: Current Open Report";

            ReportViewer reportViewer = rptViewerSettings();
            reportViewer.ServerReport.ReportPath = "/SimpleReports/OrdersOpen";

            ReportParameter[] reportParameter = new ReportParameter[2];
            reportParameter[0] = new ReportParameter("Client", clientid);
            reportParameter[1] = new ReportParameter("Division", division);

            reportViewer.ServerReport.SetParameters(reportParameter);
            ViewBag.ReportViewer = reportViewer;

            return View("~/Views/Reports/View.cshtml");
        }

        public ActionResult OrdersExport(string clientid, string division)
        {
            ViewBag.Title = "Orders: General Export";

            ReportViewer reportViewer = rptViewerSettings();
            reportViewer.ServerReport.ReportPath = "/SimpleReports/OrdersExport";

            ReportParameter[] reportParameter = new ReportParameter[4];
            reportParameter[0] = new ReportParameter("Client", clientid);
            reportParameter[1] = new ReportParameter("Division", division);
            reportParameter[2] = new ReportParameter("StartDate", DateTime.UtcNow.AddDays(-1).ToShortDateString());
            reportParameter[3] = new ReportParameter("EndDate", DateTime.UtcNow.AddDays(-1).ToShortDateString());

            reportViewer.ServerReport.SetParameters(reportParameter);
            ViewBag.ReportViewer = reportViewer;

            return View("~/Views/Reports/View.cshtml");
        }

        public ActionResult OrdersSummary(string clientid)
        {
            ViewBag.Title = "Orders: Summary Report (Daily Run)";

            ReportViewer reportViewer = rptViewerSettings();
            reportViewer.ServerReport.ReportPath = "/SimpleReports/OrdersSummary";

            ReportParameter[] reportParameter = new ReportParameter[3];
            reportParameter[0] = new ReportParameter("Client", clientid);
            //reportParameter[1] = new ReportParameter("Warehouse", "EU");
            reportParameter[1] = new ReportParameter("StartDate", DateTime.UtcNow.AddDays(-1).ToShortDateString());
            reportParameter[2] = new ReportParameter("EndDate", DateTime.UtcNow.AddDays(-1).ToShortDateString());

            reportViewer.ServerReport.SetParameters(reportParameter);
            ViewBag.ReportViewer = reportViewer;

            return View("~/Views/Reports/View.cshtml");
        }

        public ActionResult FreightSummary(string clientid)
        {
            ViewBag.Title = "Orders: Air & Truck Freight Summary";

            ReportViewer reportViewer = rptViewerSettings();
            reportViewer.ServerReport.ReportPath = "/SimpleReports/OrdersFreightSum";

            ReportParameter[] reportParameter = new ReportParameter[3];
            reportParameter[0] = new ReportParameter("Client", clientid);
            //reportParameter[1] = new ReportParameter("Warehouse", "EU");
            reportParameter[1] = new ReportParameter("StartDate", DateTime.UtcNow.AddDays(-1).ToShortDateString());
            reportParameter[2] = new ReportParameter("EndDate", DateTime.UtcNow.AddDays(-1).ToShortDateString());

            reportViewer.ServerReport.SetParameters(reportParameter);
            ViewBag.ReportViewer = reportViewer;

            return View("~/Views/Reports/View.cshtml");
        }

        public ActionResult OrdersSPSExport(string clientid)
        {
            ViewBag.Title = "Orders: SPS Billing Export";

            ReportViewer reportViewer = rptViewerSettings();
            reportViewer.ServerReport.ReportPath = "/SimpleReports/OrdersSPSExport";

            ReportParameter[] reportParameter = new ReportParameter[3];
            reportParameter[0] = new ReportParameter("Client", clientid);
            reportParameter[1] = new ReportParameter("StartDate", DateTime.UtcNow.AddDays(-1).ToShortDateString());
            reportParameter[2] = new ReportParameter("EndDate", DateTime.UtcNow.AddDays(-1).ToShortDateString());            

            reportViewer.ServerReport.SetParameters(reportParameter);
            ViewBag.ReportViewer = reportViewer;

            return View("~/Views/Reports/View.cshtml");
        }

        #endregion

        #region Product Management Reports

        public ActionResult ProductsSetup(string clientid)
        {
            ViewBag.Title = "Products: Setup Report";

            ReportViewer reportViewer = rptViewerSettings();
            reportViewer.ServerReport.ReportPath = "/SimpleReports/ProductsSetupReport";
            reportViewer.ServerReport.SetParameters(new ReportParameter("Client", clientid));
            ViewBag.ReportViewer = reportViewer;

            return View("~/Views/Reports/View.cshtml");
        }

        public ActionResult ProductsProfileList(string clientid)
        {
            ViewBag.Title = "Products: Profiles List Export";

            ReportViewer reportViewer = rptViewerSettings();
            reportViewer.ServerReport.ReportPath = "/SimpleReports/ProductsProfileListExport";
            reportViewer.ServerReport.SetParameters(new ReportParameter("Client", clientid));
            ViewBag.ReportViewer = reportViewer;

            return View("~/Views/Reports/View.cshtml");
        }

        public ActionResult ProductsSampleGuideExport(string clientid)
        {
            ViewBag.Title = "Products: Sample Guide Export";

            ReportViewer reportViewer = rptViewerSettings();
            reportViewer.ServerReport.ReportPath = "/SimpleReports/ProductsSampleGuideExport";
            reportViewer.ServerReport.SetParameters(new ReportParameter("Client", clientid));
            ViewBag.ReportViewer = reportViewer;

            return View("~/Views/Reports/View.cshtml");
        }

        public ActionResult ProductsSGRevDate(string clientid, string division)
        {
            ViewBag.Title = "Products: Sample Guide Revision Date";

            ReportViewer reportViewer = rptViewerSettings();
            reportViewer.ServerReport.ReportPath = "/SimpleReports/ProductsSGRevDate";
            
            ReportParameter[] reportParameter = new ReportParameter[4];
            reportParameter[0] = new ReportParameter("Client", clientid);
            reportParameter[1] = new ReportParameter("Division", division);
            reportParameter[2] = new ReportParameter("StartDate", DateTime.UtcNow.AddDays(-1).ToShortDateString());
            reportParameter[3] = new ReportParameter("EndDate", DateTime.UtcNow.AddDays(-1).ToShortDateString());

            reportViewer.ServerReport.SetParameters(reportParameter);
            ViewBag.ReportViewer = reportViewer;

            return View("~/Views/Reports/View.cshtml");
        }

        #endregion

        /// <summary>
        /// Report Viewer Settings.
        /// Acts as connector to Report Server and configuration for reports displayed.
        /// </summary>
        private static ReportViewer rptViewerSettings()
        {
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Remote;
            reportViewer.SizeToReportContent = true;
            reportViewer.Width = Unit.Percentage(100);
            reportViewer.Height = Unit.Percentage(100);
            reportViewer.ServerReport.ReportServerUrl = new Uri("http://DEVSERV:80/ReportServer_CMC/");

            return reportViewer;
        }
    }
}