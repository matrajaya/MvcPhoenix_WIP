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

        #region Product Coordinators Reports

        public ActionResult Cas()
        {
            ReportViewer reportViewer = rptViewerSettings();
            reportViewer.ServerReport.ReportPath = "/SimpleReports/ProductsCAS";
            reportViewer.ServerReport.SetParameters(new ReportParameter("UNNum", "UN3082"));
            ViewBag.ReportViewer = reportViewer;

            return View();
        }

        public ActionResult ClientHarmonized()
        {
            ReportViewer reportViewer = rptViewerSettings();
            reportViewer.ServerReport.ReportPath = "/SimpleReports/ProductsClientHarmonized";
            reportViewer.ServerReport.SetParameters(new ReportParameter("Client", "1"));
            ViewBag.ReportViewer = reportViewer;

            return View();
        }

        public ActionResult ClientMSDSDate()
        {
            ReportViewer reportViewer = rptViewerSettings();
            reportViewer.ServerReport.ReportPath = "/SimpleReports/ProductsClientMSDSDate";
            reportViewer.ServerReport.SetParameters(new ReportParameter("Client", "1"));
            ViewBag.ReportViewer = reportViewer;

            return View();
        }

        public ActionResult ClientPackages()
        {
            ReportViewer reportViewer = rptViewerSettings();
            reportViewer.ServerReport.ReportPath = "/SimpleReports/ProductsClientPackages";
            reportViewer.ServerReport.SetParameters(new ReportParameter("Client", "1"));
            ViewBag.ReportViewer = reportViewer;

            return View();
        }

        public ActionResult ClientProductsList()
        {
            ReportViewer reportViewer = rptViewerSettings();
            reportViewer.ServerReport.ReportPath = "/SimpleReports/ProductsClientProductsLst";
            reportViewer.ServerReport.SetParameters(new ReportParameter("Client", "1"));
            ViewBag.ReportViewer = reportViewer;

            return View();
        }

        public ActionResult HazardList()
        {
            ReportViewer reportViewer = rptViewerSettings();
            reportViewer.ServerReport.ReportPath = "/SimpleReports/ProductsHazardList";
            reportViewer.ServerReport.SetParameters(new ReportParameter("Client", "1"));
            ViewBag.ReportViewer = reportViewer;

            return View();
        }

        public ActionResult SampleGuideInfo()
        {
            ReportViewer reportViewer = rptViewerSettings();
            reportViewer.ServerReport.ReportPath = "/SimpleReports/ProductsSampleGuideInfo";
            reportViewer.ServerReport.SetParameters(new ReportParameter("Client", "1"));
            ViewBag.ReportViewer = reportViewer;

            return View();
        }

        public ActionResult SGRevDate()
        {
            ReportViewer reportViewer = rptViewerSettings();
            reportViewer.ServerReport.ReportPath = "/SimpleReports/ProductsSGRevDate";
            reportViewer.ServerReport.SetParameters(new ReportParameter("Client", "1"));
            ViewBag.ReportViewer = reportViewer;

            return View();
        }

        #endregion

        #region Inventory Reports

        public ActionResult InvExpiryReport()
        {
            ReportViewer reportViewer = rptViewerSettings();
            reportViewer.ServerReport.ReportPath = "/SimpleReports/InventoryExpiryReport";
            
            ReportParameter[] reportParameter = new ReportParameter[4];
            reportParameter[0] = new ReportParameter("Client", "1");
            reportParameter[1] = new ReportParameter("Warehouse", "EU");
            reportParameter[2] = new ReportParameter("StartDate", DateTime.UtcNow.AddDays(-365).ToShortDateString());
            reportParameter[3] = new ReportParameter("EndDate", DateTime.UtcNow.ToShortDateString());

            reportViewer.ServerReport.SetParameters(reportParameter);
            ViewBag.ReportViewer = reportViewer;

            return View();
        }

        public ActionResult InvTotalInStock()
        {
            ReportViewer reportViewer = rptViewerSettings();
            reportViewer.ServerReport.ReportPath = "/SimpleReports/InventoryTotalInvInStock";
            reportViewer.ServerReport.SetParameters(new ReportParameter("Client", "1"));
            ViewBag.ReportViewer = reportViewer;

            return View();
        }
        
        public ActionResult InvOutOfStock()
        {
            ReportViewer reportViewer = rptViewerSettings();
            reportViewer.ServerReport.ReportPath = "/SimpleReports/InventoryTotalInvOutOfStock";
            reportViewer.ServerReport.SetParameters(new ReportParameter("Client", "1"));
            ViewBag.ReportViewer = reportViewer;

            return View();
        }
        
        public ActionResult InvOpenReplenishment()
        {
            ReportViewer reportViewer = rptViewerSettings();
            reportViewer.ServerReport.ReportPath = "/SimpleReports/InventoryOpenReplenishment";
            reportViewer.ServerReport.SetParameters(new ReportParameter("Client", "1"));
            ViewBag.ReportViewer = reportViewer;

            return View();
        }

        #endregion

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