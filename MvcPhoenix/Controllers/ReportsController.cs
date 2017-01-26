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

        public ActionResult Cas()
        {
            ViewBag.Title = "Products: CAS Report";

            ReportViewer reportViewer = rptViewerSettings();
            reportViewer.ServerReport.ReportPath = "/SimpleReports/ProductsCAS";
            reportViewer.ServerReport.SetParameters(new ReportParameter("UNNum", "UN3082"));
            ViewBag.ReportViewer = reportViewer;

            return View("~/Views/Reports/View.cshtml");
        }

        public ActionResult ClientHarmonized()
        {
            ViewBag.Title = "Products: Client Harmonized Report";

            ReportViewer reportViewer = rptViewerSettings();
            reportViewer.ServerReport.ReportPath = "/SimpleReports/ProductsClientHarmonized";
            reportViewer.ServerReport.SetParameters(new ReportParameter("Client", "1"));
            ViewBag.ReportViewer = reportViewer;

            return View("~/Views/Reports/View.cshtml");
        }

        public ActionResult ClientMSDSDate()
        {
            ViewBag.Title = "Products: Client MSDS Date";

            ReportViewer reportViewer = rptViewerSettings();
            reportViewer.ServerReport.ReportPath = "/SimpleReports/ProductsClientMSDSDate";
            reportViewer.ServerReport.SetParameters(new ReportParameter("Client", "1"));
            ViewBag.ReportViewer = reportViewer;

            return View("~/Views/Reports/View.cshtml");
        }

        public ActionResult ClientPackages()
        {
            ViewBag.Title = "Products: Client Packages";

            ReportViewer reportViewer = rptViewerSettings();
            reportViewer.ServerReport.ReportPath = "/SimpleReports/ProductsClientPackages";
            reportViewer.ServerReport.SetParameters(new ReportParameter("Client", "1"));
            ViewBag.ReportViewer = reportViewer;

            return View("~/Views/Reports/View.cshtml");
        }

        public ActionResult ClientProductsList()
        {
            ViewBag.Title = "Products: Client Products List";

            ReportViewer reportViewer = rptViewerSettings();
            reportViewer.ServerReport.ReportPath = "/SimpleReports/ProductsClientProductsLst";
            reportViewer.ServerReport.SetParameters(new ReportParameter("Client", "1"));
            ViewBag.ReportViewer = reportViewer;

            return View("~/Views/Reports/View.cshtml");
        }

        public ActionResult ClientHazardList()
        {
            ViewBag.Title = "Products: Client Hazard List";

            ReportViewer reportViewer = rptViewerSettings();
            reportViewer.ServerReport.ReportPath = "/SimpleReports/ProductsHazardList";
            reportViewer.ServerReport.SetParameters(new ReportParameter("Client", "1"));
            ViewBag.ReportViewer = reportViewer;

            return View("~/Views/Reports/View.cshtml");
        }

        public ActionResult ClientSampleGuideInfo()
        {
            ViewBag.Title = "Products: Sample Guide Information";

            ReportViewer reportViewer = rptViewerSettings();
            reportViewer.ServerReport.ReportPath = "/SimpleReports/ProductsSampleGuideInfo";
            reportViewer.ServerReport.SetParameters(new ReportParameter("Client", "1"));
            ViewBag.ReportViewer = reportViewer;

            return View("~/Views/Reports/View.cshtml");
        }

        public ActionResult ClientSGRevDate()
        {
            ViewBag.Title = "Products: Sample Guide Revision Date";

            ReportViewer reportViewer = rptViewerSettings();
            reportViewer.ServerReport.ReportPath = "/SimpleReports/ProductsSGRevDate";
            reportViewer.ServerReport.SetParameters(new ReportParameter("Client", "1"));
            ViewBag.ReportViewer = reportViewer;

            return View("~/Views/Reports/View.cshtml");
        }

        #endregion

        #region Inventory Reports

        public ActionResult InvExpiry()
        {
            ViewBag.Title = "Inventory: Expiry Report";

            ReportViewer reportViewer = rptViewerSettings();
            reportViewer.ServerReport.ReportPath = "/SimpleReports/InventoryExpiryReport";
            
            ReportParameter[] reportParameter = new ReportParameter[3];
            reportParameter[0] = new ReportParameter("Client", "1");
            reportParameter[1] = new ReportParameter("StartDate", DateTime.UtcNow.AddDays(-365).ToShortDateString());
            reportParameter[2] = new ReportParameter("EndDate", DateTime.UtcNow.ToShortDateString());

            reportViewer.ServerReport.SetParameters(reportParameter);
            ViewBag.ReportViewer = reportViewer;

            return View("~/Views/Reports/View.cshtml");
        }

        public ActionResult InvTotalInStock()
        {
            ViewBag.Title = "Inventory: Total In Stock Report";

            ReportViewer reportViewer = rptViewerSettings();
            reportViewer.ServerReport.ReportPath = "/SimpleReports/InventoryTotalInvInStock";
            reportViewer.ServerReport.SetParameters(new ReportParameter("Client", "1"));
            ViewBag.ReportViewer = reportViewer;

            return View("~/Views/Reports/View.cshtml");
        }
        
        public ActionResult InvOutOfStock()
        {
            ViewBag.Title = "Inventory: Out Of Stock Report";

            ReportViewer reportViewer = rptViewerSettings();
            reportViewer.ServerReport.ReportPath = "/SimpleReports/InventoryTotalInvOutOfStock";
            reportViewer.ServerReport.SetParameters(new ReportParameter("Client", "1"));
            ViewBag.ReportViewer = reportViewer;

            return View("~/Views/Reports/View.cshtml");
        }
        
        public ActionResult InvOpenReplenishment()
        {
            ViewBag.Title = "Inventory: Open Replenishment Orders";

            ReportViewer reportViewer = rptViewerSettings();
            reportViewer.ServerReport.ReportPath = "/SimpleReports/InventoryOpenReplenishment";
            reportViewer.ServerReport.SetParameters(new ReportParameter("Client", "1"));
            ViewBag.ReportViewer = reportViewer;

            return View("~/Views/Reports/View.cshtml");
        }

        #endregion

        #region Order Reports

        public ActionResult OrdersOpen()
        {
            ViewBag.Title = "Orders: Current Open Report";

            ReportViewer reportViewer = rptViewerSettings();
            reportViewer.ServerReport.ReportPath = "/SimpleReports/OrdersOpen";
            reportViewer.ServerReport.SetParameters(new ReportParameter("Client", "1"));
            ViewBag.ReportViewer = reportViewer;

            return View("~/Views/Reports/View.cshtml");
        }

        public ActionResult OrdersExport()
        {
            ViewBag.Title = "Orders: General Export Report (for partial deliveries pivot?)";

            ReportViewer reportViewer = rptViewerSettings();
            reportViewer.ServerReport.ReportPath = "/SimpleReports/OrdersExport";

            ReportParameter[] reportParameter = new ReportParameter[3];
            reportParameter[0] = new ReportParameter("Client", "1");
            reportParameter[1] = new ReportParameter("StartDate", DateTime.UtcNow.AddDays(-365).ToShortDateString());
            reportParameter[2] = new ReportParameter("EndDate", DateTime.UtcNow.ToShortDateString());

            reportViewer.ServerReport.SetParameters(reportParameter);
            ViewBag.ReportViewer = reportViewer;

            return View("~/Views/Reports/View.cshtml");
        }

        public ActionResult OrdersSummary(string client = "1")
        {
            ViewBag.Title = "Orders: Summary Report (Daily Run)";

            ReportViewer reportViewer = rptViewerSettings();
            reportViewer.ServerReport.ReportPath = "/SimpleReports/OrdersSummary";

            ReportParameter[] reportParameter = new ReportParameter[3];
            reportParameter[0] = new ReportParameter("Client", client);
            //reportParameter[1] = new ReportParameter("Warehouse", "EU");
            reportParameter[1] = new ReportParameter("StartDate", DateTime.UtcNow.AddDays(-1).ToShortDateString());
            reportParameter[2] = new ReportParameter("EndDate", DateTime.UtcNow.AddDays(-1).ToShortDateString());

            reportViewer.ServerReport.SetParameters(reportParameter);
            ViewBag.ReportViewer = reportViewer;

            return View("~/Views/Reports/View.cshtml");
        }

        public ActionResult FreightSummary(string client = "1")
        {
            ViewBag.Title = "Orders: Air & Truck Freight Summary";

            ReportViewer reportViewer = rptViewerSettings();
            reportViewer.ServerReport.ReportPath = "/SimpleReports/OrdersFreightSum";

            ReportParameter[] reportParameter = new ReportParameter[3];
            reportParameter[0] = new ReportParameter("Client", client);
            //reportParameter[1] = new ReportParameter("Warehouse", "EU");
            reportParameter[1] = new ReportParameter("StartDate", DateTime.UtcNow.AddDays(-1).ToShortDateString());
            reportParameter[2] = new ReportParameter("EndDate", DateTime.UtcNow.AddDays(-1).ToShortDateString());

            reportViewer.ServerReport.SetParameters(reportParameter);
            ViewBag.ReportViewer = reportViewer;

            return View("~/Views/Reports/View.cshtml");
        }

        #endregion

        #region Product Management Reports
        
        public ActionResult ProductsSetup(string client = "1")
        {
            ViewBag.Title = "Products: Setup Report";

            ReportViewer reportViewer = rptViewerSettings();
            reportViewer.ServerReport.ReportPath = "/SimpleReports/ProductsSetupReport";
            reportViewer.ServerReport.SetParameters(new ReportParameter("Client", client));
            ViewBag.ReportViewer = reportViewer;

            return View("~/Views/Reports/View.cshtml");
        }

        public ActionResult ProductsProfileList(string client = "1")
        {
            ViewBag.Title = "Products: Profiles List Export";

            ReportViewer reportViewer = rptViewerSettings();
            reportViewer.ServerReport.ReportPath = "/SimpleReports/ProductsProfileListExport";
            reportViewer.ServerReport.SetParameters(new ReportParameter("Client", client));
            ViewBag.ReportViewer = reportViewer;

            return View("~/Views/Reports/View.cshtml");
        }

        public ActionResult ProductsSampleGuideExport(string client = "1")
        {
            ViewBag.Title = "Products: Sample Guide Export";

            ReportViewer reportViewer = rptViewerSettings();
            reportViewer.ServerReport.ReportPath = "/SimpleReports/ProductsSampleGuideExport";
            reportViewer.ServerReport.SetParameters(new ReportParameter("Client", client));
            ViewBag.ReportViewer = reportViewer;

            return View("~/Views/Reports/View.cshtml");
        }

        public ActionResult ProductsSGRevDate()
        {
            ViewBag.Title = "Products: Sample Guide Revision Date";

            ReportViewer reportViewer = rptViewerSettings();
            reportViewer.ServerReport.ReportPath = "/SimpleReports/ProductsSGRevDate";
            reportViewer.ServerReport.SetParameters(new ReportParameter("Client", "1"));
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