using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms;
using System.Web.UI.WebControls;

namespace MvcPhoenix.Controllers
{
    public class ReportsController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

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

        private static ReportViewer rptViewerSettings()
        {
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Remote;
            reportViewer.SizeToReportContent = true;
            reportViewer.Width = Unit.Percentage(100);
            reportViewer.Height = Unit.Percentage(100);
            reportViewer.ServerReport.ReportServerUrl = new Uri("http://DEV_SERV:80/ReportServer_CMC/");

            return reportViewer;
        }
    }
}