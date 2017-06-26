using MvcPhoenix.EF;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
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
        #region Choice Enum Lists

        public enum CMCLocationChoice
        {
            CT,
            CO,
            EU,
            AP
        }

        public enum ContactTypeChoice
        {
            Distributor,
            Requestor,
            SalesRep
        }

        public enum ContainerTypeChoice
        {
            Fiber,
            Other,
            Plastic,
            Steel,
        }

        public enum CurrencyChoice
        {
            USD,
            EUR,
            CNY
        }

        public enum GHSSignalWordChoice
        {
            None,
            Caution,
            Warning,
            Danger
        }

        public enum GHSSymbolChoice
        {
            None,
            Flammable,
            Oxidizing,
            Compressed_Gas,
            Corrosive,
            Toxic,
            Irritant,
            Health_Hazard,
            Environmental_Hazard
        }

        public enum OrderSourceChoice
        {
            Manual,
            Web,
            Download,
            Transfer
        }

        public enum OrderTypeChoice
        {
            [Display(Name = "Sample")]
            S,

            [Display(Name = "Dormant")]
            D,

            [Display(Name = "Return")]
            R,

            [Display(Name = "Commercial")]
            C,

            [Display(Name = "R&D/Transfer")]
            X
        }

        public enum StockStatusChoice
        {
            AVAIL,
            HOLD,
            QC,
            RETURN,
            TEST
        }

        public enum UMChoice
        {
            [Display(Name = "Kilogram")]
            KG,

            [Display(Name = "Pound")]
            LB,
        }

        public enum ValueUnitChoice
        {
            [Display(Name = "Gallon")]
            GAL,

            [Display(Name = "Kilogram")]
            KG,

            [Display(Name = "Liter")]
            L,

            [Display(Name = "Pound")]
            LB,
        }

        #endregion Choice Enum Lists

        #region DropDown Lists Construction

        public static string ddlBuildBillingGroup(int clientid)
        {
            using (var db = new CMCSQL03Entities())
            {
                var divisionid = (from t in db.tblDivision
                                    where t.ClientID == clientid
                                    orderby t.DivisionName
                                    select t);

                var enduses = (from t in db.tblEndUse
                               where t.ClientID == clientid
                               orderby t.EndUse
                               select t);

                string s = "<option value='ALL' selected=true>ALL</option>";

                if (divisionid.Count() > 0)
                {
                    s = s + "<optgroup label='Billing Groups'>";
                    foreach (var item in divisionid)
                    {
                        s = s + "<option value=" + item.DivisionID + ">" + item.DivisionName + " / " + item.BusinessUnit + "</option>";
                    }
                    s = s + "</optgroup>";
                }

                if (enduses.Count() > 0)
                {
                    s = s + "<optgroup label='End Uses'>";
                    foreach (var item in enduses)
                    {
                        s = s + "<option value='" + item.EndUse + "'>" + item.EndUse + "</option>";
                    }
                    s = s + "</optgroup>";
                }

                s = s + "</select>";

                return s;
            }
        }

        public static string ddlBuildDivisionDropDown(int id)
        {
            using (var db = new CMCSQL03Entities())
            {
                var divisions = (from t in db.tblDivision
                                 where t.ClientID == id
                                 orderby t.DivisionName, t.BusinessUnit
                                 select t);

                string s = "<option value='0'></option>";

                if (divisions.Count() > 0)
                {
                    foreach (var item in divisions)
                    {
                        s = s + "<option value=" + item.DivisionID.ToString() + ">" + item.DivisionName + " - " + item.BusinessUnit + "</option>";
                    }
                }
                else
                {
                    s = s + "<option value=0>No Divisions Found</option>";
                }

                s = s + "</select>";

                return s;
            }
        }

        public static string ddlBuildProductCodeDropDown(int clientid)
        {
            using (var db = new CMCSQL03Entities())
            {
                var products = (from t in db.tblProductDetail
                                join pm in db.tblProductMaster on t.ProductMasterID equals pm.ProductMasterID
                                where pm.ClientID == clientid
                                orderby t.ProductCode
                                select t);

                string s = "<option value='' selected=true></option>";

                if (products.Count() > 0)
                {
                    foreach (var item in products)
                    { s = s + "<option value=" + item.ProductDetailID.ToString() + ">" + item.ProductCode + " - " + item.ProductName + "</option>"; }
                }
                else
                {
                    s = s + "<option value=0>No Products Found</option>";
                }

                s = s + "</select>";

                return s;
            }
        }

        public static string ddlBuildProductEquivalentDropdown(int? clientid)
        {
            using (var db = new CMCSQL03Entities())
            {
                var products = (from t in db.tblProductDetail
                                join pm in db.tblProductMaster on t.ProductMasterID equals pm.ProductMasterID
                                where pm.ClientID == clientid
                                && t.ProductCode != pm.MasterCode
                                orderby t.ProductCode
                                select new { t.ProductDetailID, t.ProductCode, t.ProductName }).ToList();

                string s = "<option value='0'></option>";
                if (products.Count() > 0)
                {
                    foreach (var item in products)
                    {
                        s = s + "<option value=" + item.ProductDetailID.ToString() + ">" + item.ProductCode + " - " + item.ProductName + "</option>";
                    }
                }
                else
                {
                    s = s + "<option value=0>No Products Found</option>";
                }
                s = s + "</select>";

                return s;
            }
        }

        public static string ddlBuildProductMasterDropDown(int clientid)
        {
            using (var db = new CMCSQL03Entities())
            {
                var products = (from t in db.tblProductMaster
                                where t.ClientID == clientid
                                orderby t.MasterCode, t.MasterName
                                select t);

                string s = "<option value='' selected=true></option>";

                if (products.Count() > 0)
                {
                    foreach (var item in products)
                    { s = s + "<option value=" + item.ProductMasterID.ToString() + ">" + item.MasterCode + " - " + item.MasterName + "</option>"; }
                }
                else
                {
                    s = s + "<option value=0>No Products Found</option>";
                }

                s = s + "</select>";

                return s;
            }
        }

        public static string ddlBuildShelfMasterPackagesDropDown()
        {
            // This returns ONLY the <option> portion of the <select> tag
            using (var db = new CMCSQL03Entities())
            {
                var packages = (from t in db.tblPackage
                                orderby t.Size
                                select t);

                string s = "<option value='0' selected=true>Select Package</option>";
                if (packages.Count() > 0)
                {
                    foreach (var item in packages)
                    { s = s + "<option value=" + item.PackageID.ToString() + ">" + item.PartNumber + " - " + item.Description + "</option>"; }
                }
                else
                { s = s + "<option value=0>No Packages Found</option>"; }
                s = s + "</select>";
                return s;
            }
        }

        public static string ddlBuildSizeDropDown(int productdetailid)
        {
            using (var db = new CMCSQL03Entities())
            {
                bool isSRcheck = false;                                             // flag to avoid duplicate listing of 1SR
                var shelfsizes = (from t in db.tblShelfMaster
                                  where t.ProductDetailID == productdetailid
                                  && t.InactiveSize != true
                                  orderby t.Size
                                  select t);

                string s = "<option value='' selected=true></option>";

                if (shelfsizes.Count() > 0)
                {
                    foreach (var item in shelfsizes)
                    {
                        if (item.Size == "1SR")
                        {
                            isSRcheck = true;
                        }
                        s = s + "<option value=" + item.ShelfID.ToString() + ">" + item.Size + " - " + item.UnitWeight + "</option>";
                    }
                }

                if (!isSRcheck)
                {
                    s = s + "<option value='0'>1SR</option>";                       // TBD: change value to '1SR' and update checks in other services - iffy
                }

                return s;
            }
        }

        #endregion DropDown Lists Construction

        #region SelectListItem Objects

        public static List<SelectListItem> ddlBulkIDs(int? productdetailid)
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> result = new List<SelectListItem>();

                result = (from t in db.tblBulk
                          join pm in db.tblProductMaster on t.ProductMasterID equals pm.ProductMasterID
                          join pd in db.tblProductDetail on pm.ProductMasterID equals pd.ProductMasterID
                          join sm in db.tblShelfMaster on pd.ProductDetailID equals sm.ProductDetailID
                          where sm.ShelfID == productdetailid
                          select new SelectListItem
                          {
                              Value = t.BulkID.ToString(),
                              Text = t.LotNumber
                          }).ToList();
                result.Insert(0, new SelectListItem { Value = "", Text = "" });

                return result;
            }
        }

        public static List<SelectListItem> ddlBulkSuppliers(int clientid)
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> result = new List<SelectListItem>();

                result = (from t in db.tblBulkSupplier
                          where t.ClientID == clientid
                          orderby t.SupplyID
                          select new SelectListItem
                          {
                              Value = t.BulkSupplierID.ToString(),
                              Text = t.SupplyID
                          }).ToList();
                result.Insert(0, new SelectListItem { Value = "", Text = "" });

                return result;
            }
        }

        public static List<SelectListItem> ddlCarriers()
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> result = new List<SelectListItem>();

                result = (from t in db.tblCarrier
                          orderby t.CarrierName
                          select new SelectListItem
                          {
                              Value = t.CarrierName,
                              Text = t.CarrierName
                          }).ToList();
                result.Insert(0, new SelectListItem { Value = "", Text = "" });

                return result;
            }
        }

        public static List<SelectListItem> ddlClientIDs()
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> result = new List<SelectListItem>();

                result = (from t in db.tblClient
                          orderby t.ClientName
                          select new SelectListItem
                          {
                              Value = t.ClientID.ToString(),
                              Text = t.ClientName
                          }).ToList();
                result.Insert(0, new SelectListItem { Value = "", Text = "" });

                return result;
            }
        }

        public static List<SelectListItem> ddlClientsContacts(int clientid, string sContactType)
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> result = new List<SelectListItem>();

                result = (from t in db.tblClientContact
                          where t.ClientID == clientid
                          && t.ContactType == sContactType
                          orderby t.FullName
                          select new SelectListItem
                          {
                              Value = t.ClientContactID.ToString(),
                              Text = t.FullName
                          }).ToList();
                result.Insert(0, new SelectListItem { Value = "", Text = "" });

                return result;
            }
        }

        public static List<SelectListItem> ddlCountries()
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> result = new List<SelectListItem>();

                result = (from t in db.tblCountry
                          orderby t.Country
                          select new SelectListItem
                          {
                              Value = t.Country,
                              Text = t.Country
                          }).ToList();
                result.Insert(0, new SelectListItem { Value = "0", Text = "" });

                return result;
            }
        }

        public static List<SelectListItem> ddlDelayReasons()
        {
            List<SelectListItem> result = new List<SelectListItem>();

            result.Insert(0, new SelectListItem { Value = "", Text = "" });
            result.Insert(1, new SelectListItem { Value = "Backorder", Text = "Backorder" });
            result.Insert(2, new SelectListItem { Value = "Oven Product", Text = "Oven Product" });
            result.Insert(3, new SelectListItem { Value = "Humidity", Text = "Humidity" });
            result.Insert(4, new SelectListItem { Value = "Very Large Order", Text = "Very Large Order" });
            result.Insert(5, new SelectListItem { Value = "Questions/Info Needed", Text = "Questions/Info Needed" });
            result.Insert(6, new SelectListItem { Value = "Special Doc Required", Text = "Special Doc Required" });
            result.Insert(7, new SelectListItem { Value = "Special Request Size", Text = "Special Request Size" });
            result.Insert(8, new SelectListItem { Value = "Return Order", Text = "Return Order" });
            result.Insert(9, new SelectListItem { Value = "Waste", Text = "Waste" });
            result.Insert(10, new SelectListItem { Value = "No partial delivery", Text = "No partial delivery" });
            result.Insert(11, new SelectListItem { Value = "Special delivery date", Text = "Special delivery date" });
            result.Insert(12, new SelectListItem { Value = "Public holiday", Text = "Public holiday" });
            result.Insert(13, new SelectListItem { Value = "Freezable Procedure", Text = "Freezable Procedure" });
            result.Insert(14, new SelectListItem { Value = "CMC delay, customer informed", Text = "CMC delay, customer informed" });
            result.Insert(15, new SelectListItem { Value = "R&D flow", Text = "R&D flow" });
            result.Insert(16, new SelectListItem { Value = "Special procedure", Text = "Special procedure" });
            result.Insert(17, new SelectListItem { Value = "Misc. Charges", Text = "Misc. Charges" });
            result.Insert(18, new SelectListItem { Value = "Transfer order", Text = "Transfer order" });
            result.Insert(19, new SelectListItem { Value = "Consolidated Order", Text = "Consolidated Order" });
            result.Insert(20, new SelectListItem { Value = "Approval Required", Text = "Approval Required" });
            result.Insert(21, new SelectListItem { Value = "Conditioned packaging", Text = "Conditioned packaging" });
            result.Insert(22, new SelectListItem { Value = "Product Not Setup", Text = "Product Not Setup" });
            result.Insert(23, new SelectListItem { Value = "Frt Fwd-Pending Arrangement", Text = "Frt Fwd-Pending Arrangement" });
            result.Insert(24, new SelectListItem { Value = "Labels, SDS", Text = "Labels, SDS" });

            return result;
        }

        public static List<SelectListItem> ddlDivisionIDs(int? clientid)
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> result = new List<SelectListItem>();

                result = (from t in db.tblDivision
                          where t.ClientID == clientid
                          orderby t.DivisionName
                          select new SelectListItem
                          {
                              Value = t.DivisionID.ToString(),
                              Text = t.DivisionName + " / " + t.BusinessUnit
                          }).ToList();
                result.Insert(0, new SelectListItem { Value = "", Text = "" });

                return result;
            }
        }

        public static List<SelectListItem> ddlEndUses(int? clientid)
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> result = new List<SelectListItem>();

                result = (from t in db.tblEndUse
                          where t.ClientID == clientid
                          orderby t.EndUse
                          select new SelectListItem
                          {
                              Value = t.EndUse,
                              Text = t.EndUse
                          }).ToList();
                result.Insert(0, new SelectListItem { Value = "", Text = "" });

                return result;
            }
        }

        public static List<SelectListItem> ddlEndUsesForCustoms()
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> result = new List<SelectListItem>();

                result = (from t in db.tblEndUseForCustoms
                          orderby t.EndUse
                          select new SelectListItem
                          {
                              Value = t.EndUse,
                              Text = t.EndUse
                          }).ToList();
                result.Insert(0, new SelectListItem { Value = "", Text = "" });

                return result;
            }
        }

        public static List<SelectListItem> ddlHarmonizedCodes()
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> result = new List<SelectListItem>();

                result = (from t in db.tblHSCode
                          orderby t.HarmonizedCode
                          select new SelectListItem
                          {
                              Value = t.HarmonizedCode,
                              Text = t.HarmonizedCode
                          }).ToList();
                result.Insert(0, new SelectListItem { Value = "", Text = "" });

                return result;
            }
        }

        public static List<SelectListItem> ddlHSCodes()
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> result = new List<SelectListItem>();

                result = (from t in db.tblHSCode
                          orderby t.HarmonizedCode
                          select new SelectListItem
                          {
                              Value = t.HarmonizedCode,
                              Text = t.HarmonizedCode
                          }).ToList();
                result.Insert(0, new SelectListItem { Value = "", Text = "" });

                return result;
            }
        }

        public static List<SelectListItem> ddlOrderIDs()
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> result = new List<SelectListItem>();

                result = (from t in db.tblOrderMaster
                          orderby t.OrderID descending
                          select new SelectListItem
                          {
                              Value = t.OrderID.ToString(),
                              Text = t.OrderID.ToString()
                          }).ToList();
                result.Insert(0, new SelectListItem { Value = "", Text = "" });

                return result;
            }
        }

        public static List<SelectListItem> ddlOrderItemIDs(int? orderid)
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> result = new List<SelectListItem>();

                result = (from t in db.tblOrderItem
                          where t.OrderID == orderid
                          orderby t.ProductCode
                          select new SelectListItem
                          {
                              Value = t.ItemID.ToString(),
                              Text = t.ProductCode
                          }).ToList();
                result.Insert(0, new SelectListItem { Value = "", Text = "" });

                return result;
            }
        }

        public static List<SelectListItem> ddlOrderItemStatusIDs()
        {
            List<SelectListItem> result = new List<SelectListItem>();

            result.Insert(0, new SelectListItem { Value = "", Text = "" });
            result.Insert(1, new SelectListItem { Value = "CL", Text = "Closed" });
            result.Insert(2, new SelectListItem { Value = "OP", Text = "Open" });
            result.Insert(3, new SelectListItem { Value = "CN", Text = "Cancelled" });

            return result;
        }

        public static List<SelectListItem> ddlOrderTransactionTypes()
        {
            var FixedCharges = new SelectListGroup() { Name = "Fixed Charges" };
            var VariableCharges = new SelectListGroup() { Name = "Variable Charges" };

            List<SelectListItem> transactiontypes = new List<SelectListItem>();

            transactiontypes.Add(new SelectListItem { Value = "MEMO", Text = "Memo", Selected = true });

            // Fixed Charges
            transactiontypes.Add(new SelectListItem { Value = "Air Hazard Only", Text = "Air Hazard Only", Group = FixedCharges });
            transactiontypes.Add(new SelectListItem { Value = "Certificate Of Origin", Text = "Certificate Of Origin", Group = FixedCharges });
            transactiontypes.Add(new SelectListItem { Value = "CMC Pack", Text = "CMC Pack", Group = FixedCharges });
            transactiontypes.Add(new SelectListItem { Value = "Cool Pack", Text = "Cool Pack", Group = FixedCharges });
            transactiontypes.Add(new SelectListItem { Value = "Credit Card Fee", Text = "Credit Card Fee", Group = FixedCharges });
            transactiontypes.Add(new SelectListItem { Value = "Credit Card Order", Text = "Credit Card Order", Group = FixedCharges });
            transactiontypes.Add(new SelectListItem { Value = "Document Handling", Text = "Document Handling", Group = FixedCharges });
            transactiontypes.Add(new SelectListItem { Value = "Empty Packaging", Text = "Empty Packaging", Group = FixedCharges });
            transactiontypes.Add(new SelectListItem { Value = "External System", Text = "External System", Group = FixedCharges });
            transactiontypes.Add(new SelectListItem { Value = "Follow Up Order", Text = "Follow Up Order", Group = FixedCharges });
            transactiontypes.Add(new SelectListItem { Value = "Freezer Pack", Text = "Freezer Pack", Group = FixedCharges });
            transactiontypes.Add(new SelectListItem { Value = "GHS Labels", Text = "GHS Labels", Group = FixedCharges });
            transactiontypes.Add(new SelectListItem { Value = "Inactive Products", Text = "Inactive Products", Group = FixedCharges });
            transactiontypes.Add(new SelectListItem { Value = "Isolation", Text = "Isolation", Group = FixedCharges });
            transactiontypes.Add(new SelectListItem { Value = "Isolation Box", Text = "Isolation Box", Group = FixedCharges });
            transactiontypes.Add(new SelectListItem { Value = "IT Fee", Text = "IT Fee", Group = FixedCharges });
            transactiontypes.Add(new SelectListItem { Value = "Label Maintainance", Text = "Label Maintainance", Group = FixedCharges });
            transactiontypes.Add(new SelectListItem { Value = "Label Stock", Text = "Label Stock", Group = FixedCharges });
            transactiontypes.Add(new SelectListItem { Value = "Labels Printed", Text = "Labels Printed", Group = FixedCharges });
            transactiontypes.Add(new SelectListItem { Value = "Labor Relabel", Text = "Labor Relabel", Group = FixedCharges });
            transactiontypes.Add(new SelectListItem { Value = "Literature Fee", Text = "Literature Fee", Group = FixedCharges });
            transactiontypes.Add(new SelectListItem { Value = "Limited Quantity", Text = "Limited Quantity", Group = FixedCharges });
            transactiontypes.Add(new SelectListItem { Value = "Manual Handling", Text = "Manual Handling", Group = FixedCharges });
            transactiontypes.Add(new SelectListItem { Value = "MSDS Prints", Text = "MSDS Prints", Group = FixedCharges });
            transactiontypes.Add(new SelectListItem { Value = "New Label Setup", Text = "New Label Setup", Group = FixedCharges });
            transactiontypes.Add(new SelectListItem { Value = "New Product Setup", Text = "New Product Setup", Group = FixedCharges });
            transactiontypes.Add(new SelectListItem { Value = "Oberk Pack", Text = "Oberk Pack", Group = FixedCharges });
            transactiontypes.Add(new SelectListItem { Value = "Order Entry", Text = "Order Entry", Group = FixedCharges });
            transactiontypes.Add(new SelectListItem { Value = "Over Pack", Text = "Over Pack", Group = FixedCharges });
            transactiontypes.Add(new SelectListItem { Value = "Pallet Return", Text = "Pallet Return", Group = FixedCharges });
            transactiontypes.Add(new SelectListItem { Value = "Poison Pack", Text = "Poison Pack", Group = FixedCharges });
            transactiontypes.Add(new SelectListItem { Value = "Product Setup Changes", Text = "Product Setup Changes", Group = FixedCharges });
            transactiontypes.Add(new SelectListItem { Value = "QC Storage", Text = "QC Storage", Group = FixedCharges });
            transactiontypes.Add(new SelectListItem { Value = "RD Handling ADR", Text = "R&D Handling ADR", Group = FixedCharges });
            transactiontypes.Add(new SelectListItem { Value = "RD Handling IATA", Text = "R&D Handling IATA", Group = FixedCharges });
            transactiontypes.Add(new SelectListItem { Value = "RD Handling LQ", Text = "R&D Handling LQ", Group = FixedCharges });
            transactiontypes.Add(new SelectListItem { Value = "RD Handling Non Hazard", Text = "R&D Handling Non Hazard", Group = FixedCharges });
            transactiontypes.Add(new SelectListItem { Value = "Refrigerator Storage", Text = "Refrigerator Storage", Group = FixedCharges });
            transactiontypes.Add(new SelectListItem { Value = "Relabels", Text = "Relabels", Group = FixedCharges });
            transactiontypes.Add(new SelectListItem { Value = "Rush Shipment", Text = "Rush Shipment", Group = FixedCharges });
            transactiontypes.Add(new SelectListItem { Value = "SPA 197 Applied", Text = "SPA 197 Applied", Group = FixedCharges });
            transactiontypes.Add(new SelectListItem { Value = "SPS Paid Order", Text = "SPS Paid Order", Group = FixedCharges });
            transactiontypes.Add(new SelectListItem { Value = "UN Box", Text = "UN Box", Group = FixedCharges });
            transactiontypes.Add(new SelectListItem { Value = "Warehouse Storage", Text = "Warehouse Storage", Group = FixedCharges });
            transactiontypes.Add(new SelectListItem { Value = "WHMIS Labels", Text = "WHMIS Labels", Group = FixedCharges });

            // Variable Charges
            transactiontypes.Add(new SelectListItem { Value = "Administrative Waste Fee", Text = "Administrative Waste Fee", Group = VariableCharges });
            transactiontypes.Add(new SelectListItem { Value = "Credit", Text = "Credit", Group = VariableCharges });
            transactiontypes.Add(new SelectListItem { Value = "Customs Documents", Text = "Customs Documents", Group = VariableCharges });
            transactiontypes.Add(new SelectListItem { Value = "Delivery Duties Taxes", Text = "Delivery Duties Taxes", Group = VariableCharges });
            transactiontypes.Add(new SelectListItem { Value = "Documents", Text = "Documents", Group = VariableCharges });
            transactiontypes.Add(new SelectListItem { Value = "Handling", Text = "Handling", Group = VariableCharges });
            transactiontypes.Add(new SelectListItem { Value = "MautFuel", Text = "Maut Fuel", Group = VariableCharges });
            transactiontypes.Add(new SelectListItem { Value = "Miscellaneous Labor", Text = "Miscellaneous Labor", Group = VariableCharges });
            transactiontypes.Add(new SelectListItem { Value = "Other", Text = "Other", Group = VariableCharges });
            transactiontypes.Add(new SelectListItem { Value = "Waste Processing", Text = "Waste Processing", Group = VariableCharges });

            return transactiontypes;
        }

        public static List<SelectListItem> ddlOtherProductStorage()
        {
            List<SelectListItem> otherstoragelist = new List<SelectListItem>();

            otherstoragelist.Add(new SelectListItem { Value = "", Text = "" });
            otherstoragelist.Add(new SelectListItem { Value = "Toxic Storage", Text = "Toxic Storage" });
            otherstoragelist.Add(new SelectListItem { Value = "Spontaneously Combustible", Text = "Spontaneously Combustible" });
            otherstoragelist.Add(new SelectListItem { Value = "Oxidizer", Text = "Oxidizer" });
            otherstoragelist.Add(new SelectListItem { Value = "Base", Text = "Base" });
            otherstoragelist.Add(new SelectListItem { Value = "Acid", Text = "Acid" });
            otherstoragelist.Add(new SelectListItem { Value = "Clean Storage", Text = "Clean Storage" });
            otherstoragelist.Add(new SelectListItem { Value = "Flammable/Corrosive", Text = "Flammable/Corrosive" });
            otherstoragelist.Add(new SelectListItem { Value = "Toxic/Corrosive", Text = "Toxic/Corrosive" });
            otherstoragelist.Add(new SelectListItem { Value = "Biocide", Text = "Biocide" });
            otherstoragelist.Add(new SelectListItem { Value = "Biocide/Toxic", Text = "Biocide/Toxic" });
            otherstoragelist.Add(new SelectListItem { Value = "Biocide/Toxic/Corrosive", Text = "Biocide/Toxic/Corrosive" });
            otherstoragelist.Add(new SelectListItem { Value = "Biocide/Corrosive", Text = "Biocide/Corrosive" });
            otherstoragelist.Add(new SelectListItem { Value = "Biocide/Oxidizer", Text = "Biocide/Oxidizer" });
            otherstoragelist.Add(new SelectListItem { Value = "Flammable/Pueblo", Text = "Flammable/Pueblo" });
            otherstoragelist.Add(new SelectListItem { Value = "Corrosive/Flammable", Text = "Corrosive/Flammable" });
            otherstoragelist.Add(new SelectListItem { Value = "Biocide/Corrosive/Toxic", Text = "Biocide/Corrosive/Toxic" });
            otherstoragelist.Add(new SelectListItem { Value = "Corrosive/Toxic", Text = "Corrosive/Toxic" });
            otherstoragelist.Add(new SelectListItem { Value = "Biocide/Flammable/Corrosive", Text = "Biocide/Flammable/Corrosive" });
            otherstoragelist.Add(new SelectListItem { Value = "Flammable/Toxic/Corrosive", Text = "Flammable/Toxic/Corrosive" });
            otherstoragelist.Add(new SelectListItem { Value = "Biocide/Corrosive/Flammable", Text = "Biocide/Corrosive/Flammable" });
            otherstoragelist.Add(new SelectListItem { Value = "Biocide/Flammable/Toxic/Corrosive", Text = "Biocide/Flammable/Toxic/Corrosive" });
            otherstoragelist.Add(new SelectListItem { Value = "Biocide/Flammable/Corrosive/Toxic", Text = "Biocide/Flammable/Corrosive/Toxic" });

            return otherstoragelist;
        }

        public static List<SelectListItem> ddlPackageIDs(string size)
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> result = new List<SelectListItem>();

                result = (from t in db.tblPackage
                          orderby t.Size
                          select new SelectListItem
                          {
                              Value = t.PackageID.ToString(),
                              Text = t.PartNumber + "-" + t.Description
                          }).ToList();
                result.Insert(0, new SelectListItem { Value = "", Text = "" });

                return result;
            }
        }

        public static List<SelectListItem> ddlProductCodes(int? clientid)
        {
            List<SelectListItem> result = new List<SelectListItem>();

            using (var db = new CMCSQL03Entities())
            {
                result = (from t in db.tblProductDetail
                          join m in db.tblProductMaster on t.ProductMasterID equals m.ProductMasterID
                          where m.ClientID == clientid
                          orderby t.ProductCode
                          select new SelectListItem
                          {
                              Value = t.ProductDetailID.ToString(),
                              Text = t.ProductCode + " " + t.ProductName
                          }).ToList();
                result.Insert(0, new SelectListItem { Value = "", Text = "" });

                return result;
            }
        }

        public static List<SelectListItem> ddlProductCodeSizes(int productdetailid)
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> result = new List<SelectListItem>();

                result = (from t in db.tblShelfMaster
                          where t.ProductDetailID == productdetailid
                          && t.InactiveSize != true
                          orderby t.Size
                          select new SelectListItem
                          {
                              Value = t.ShelfID.ToString(),
                              Text = t.Size + " - " + t.UnitWeight
                          }).ToList();

                return result;
            }
        }

        public static List<SelectListItem> ddlProductCodesXRefs(string productcode)
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> result = new List<SelectListItem>();

                result = (from t in db.tblProductXRef
                          where t.CMCProductCode == productcode
                          select new SelectListItem
                          {
                              Value = t.CustProductCode + " - " + t.CustProductName,
                              Text = t.CustProductCode + " - " + t.CustProductName
                          }).ToList();

                return result;
            }
        }

        public static List<SelectListItem> ddlProductCodeXref(int? clientid)
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> result = new List<SelectListItem>();

                result = (from t in db.tblProductXRef
                          where t.ClientID == clientid
                          orderby t.CustProductCode
                          select new SelectListItem
                          {
                              Value = t.CustProductCode,
                              Text = t.CustProductCode + " : " + t.CMCProductCode
                          }).ToList();
                result.Insert(0, new SelectListItem { Value = "", Text = "" });

                return result;
            }
        }

        public static List<SelectListItem> ddlProductEquivalents(int? productmasterid, string productcode)
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> result = new List<SelectListItem>();

                result = (from t in db.tblProductDetail
                          where t.ProductMasterID == productmasterid
                          && t.ProductCode != productcode
                          select new SelectListItem
                          {
                              Value = t.ProductCode + " / " + t.ProductName,
                              Text = t.ProductCode + " / " + t.ProductName
                          }).ToList();

                return result;
            }
        }

        public static List<SelectListItem> ddlProductEquivalents(int? clientid)
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> result = new List<SelectListItem>();

                result = (from t in db.tblProductDetail
                          join pm in db.tblProductMaster on t.ProductMasterID equals pm.ProductMasterID
                          where pm.ClientID == clientid
                          && t.ProductCode != pm.MasterCode
                          orderby t.ProductCode
                          select new SelectListItem
                          {
                              Value = t.ProductDetailID.ToString(),
                              Text = t.ProductCode + " - " + t.ProductName
                          }).ToList();

                return result;
            }
        }

        public static List<SelectListItem> ddlProductHandlingGloves()
        {
            List<SelectListItem> gloves = new List<SelectListItem>();

            gloves.Add(new SelectListItem { Value = "", Text = "" });
            gloves.Add(new SelectListItem { Value = "GMP NITRILE", Text = "GMP NITRILE" });
            gloves.Add(new SelectListItem { Value = "NEOPRENE", Text = "NEOPRENE" });
            gloves.Add(new SelectListItem { Value = "NEOPRENE+NITRIL", Text = "NEOPRENE+NITRIL" });
            gloves.Add(new SelectListItem { Value = "NITRILE", Text = "NITRILE" });

            return gloves;
        }

        public static List<SelectListItem> ddlProductMasterIDs(int? clientid)
        {
            using (var db = new CMCSQL03Entities())                                                             // 06/13/2016 This now is a list of PD-PN records
            {
                List<SelectListItem> result = new List<SelectListItem>();

                result = (from t in db.tblProductMaster
                          where t.ClientID == clientid
                          orderby t.MasterCode
                          select new SelectListItem
                          {
                              Value = t.ProductMasterID.ToString(),
                              Text = t.MasterCode + " - " + t.MasterName.Substring(0, 25)
                          }).ToList();
                result.Insert(0, new SelectListItem { Value = "", Text = "" });

                return result;
            }
        }

        public static List<SelectListItem> ddlProductMasterIDs(int? clientid, int? PmID = null)
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> result = new List<SelectListItem>();

                if (PmID == null)
                {
                    result = (from t in db.tblProductMaster
                              where t.ClientID == clientid
                              orderby t.MasterCode, t.MasterName
                              select new SelectListItem
                              {
                                  Value = t.ProductMasterID.ToString(),
                                  Text = t.MasterCode + " - " + t.MasterName.Substring(0, 25)
                              }).ToList();
                    result.Insert(0, new SelectListItem { Value = "", Text = "" });
                }
                else
                {
                    result = (from t in db.tblProductMaster
                              where t.ClientID == clientid
                              && t.ProductMasterID == PmID
                              orderby t.MasterCode, t.MasterName
                              select new SelectListItem
                              {
                                  Value = t.ProductMasterID.ToString(),
                                  Text = t.MasterCode + " - " + t.MasterName.Substring(0, 25)
                              }).ToList();
                }

                return result;
            }
        }

        public static List<SelectListItem> ddlProductPackagePartNumbers()
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> result = new List<SelectListItem>();

                result = (from t in db.tblPackage
                          orderby t.Size
                          select new SelectListItem
                          {
                              Value = t.PackageID.ToString(),
                              Text = t.Size + " " + t.PartNumber + " " + t.Description
                          }).ToList();
                result.Insert(0, new SelectListItem { Value = "", Text = "" });

                return result;
            }
        }

        public static List<SelectListItem> ddlReasonCodes()
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> result = new List<SelectListItem>();

                result = (from t in db.tblReasonCode
                          orderby t.Reason
                          select new SelectListItem
                          {
                              Value = t.Reason,
                              Text = t.Reason
                          }).ToList();
                result.Insert(0, new SelectListItem { Value = "", Text = "" });

                return result;
            }
        }

        public static List<SelectListItem> ddlReportCriterias()
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> result = new List<SelectListItem>();

                result = (from t in db.tblReportCriteria
                          orderby t.Display
                          select new SelectListItem
                          {
                              Value = t.Display,
                              Text = t.ReportName
                          }).ToList();
                result.Insert(0, new SelectListItem { Value = "", Text = "" });

                return result;
            }
        }

        public static List<SelectListItem> ddlRequestors(int? clientid)
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> result = new List<SelectListItem>();

                result = (from t in db.tblClientContact
                          where t.ClientID == clientid
                          where t.ContactType == "Requestor"
                          orderby t.FullName
                          select new SelectListItem
                          {
                              Value = t.ClientContactID.ToString(),
                              Text = t.FullName
                          }).ToList();
                result.Insert(0, new SelectListItem { Value = "", Text = "" });

                return result;
            }
        }

        public static List<SelectListItem> ddlSalesReps(int? clientid)
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> result = new List<SelectListItem>();

                result = (from t in db.tblClientContact
                          where t.ClientID == clientid
                          where t.ContactType == "SalesRep"
                          orderby t.FullName
                          select new SelectListItem
                          {
                              Value = t.ClientContactID.ToString(),
                              Text = t.FullName
                          }).ToList();
                result.Insert(0, new SelectListItem { Value = "", Text = "" });

                return result;
            }
        }

        public static List<SelectListItem> ddlShelfMasterIDs(int? productdetailid)
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> result = new List<SelectListItem>();

                result = (from t in db.tblShelfMaster
                          join pd in db.tblProductDetail on t.ProductDetailID equals pd.ProductDetailID
                          where t.ProductDetailID == productdetailid
                          select new SelectListItem
                          {
                              Value = t.ShelfID.ToString(),
                              Text = t.Size
                          }).ToList();
                result.Insert(0, new SelectListItem { Value = "", Text = "" });

                return result;
            }
        }

        public static List<SelectListItem> ddlShipVias()
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> result = new List<SelectListItem>();

                result = (from t in db.tblCarrier
                          orderby t.CarrierName
                          select new SelectListItem
                          {
                              Value = t.CarrierName,
                              Text = t.CarrierName
                          }).ToList();
                result.Insert(0, new SelectListItem { Value = "", Text = "" });

                return result;
            }
        }

        public static List<SelectListItem> ddlShipViasItemLevel()
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> result = new List<SelectListItem>();

                result = (from t in db.tblCarrier
                          orderby t.CarrierName
                          select new SelectListItem
                          {
                              Value = t.CarrierName,
                              Text = t.CarrierName
                          }).ToList();
                result.Insert(0, new SelectListItem { Value = "", Text = "" });

                return result;
            }
        }

        public static List<SelectListItem> ddlStates()
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> result = new List<SelectListItem>();

                result = (from t in db.tblState
                          orderby t.StateName
                          select new SelectListItem
                          {
                              Value = t.StateAbbr,
                              Text = t.StateName
                          }).ToList();
                result.Insert(0, new SelectListItem { Value = "", Text = "" });

                return result;
            }
        }

        public static List<SelectListItem> ddlStatusNotes()
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> result = new List<SelectListItem>();

                result = (from t in db.tblStatusNotes
                          orderby t.Note
                          select new SelectListItem
                          {
                              Value = t.StatusNotesID.ToString(),
                              Text = t.Note
                          }).ToList();
                result.Insert(0, new SelectListItem { Value = "", Text = "" });

                return result;
            }
        }

        public static List<SelectListItem> ddlStatusNotesIDs()
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> result = new List<SelectListItem>();

                result = (from t in db.tblStatusNotes
                          orderby t.Note
                          select new SelectListItem
                          {
                              Value = t.Note,
                              Text = t.Note
                          }).ToList();
                result.Insert(0, new SelectListItem { Value = "", Text = "" });

                return result;
            }
        }

        public static List<SelectListItem> ddlSupplyIDs(int? clientid)
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> result = new List<SelectListItem>();

                result = (from t in db.tblBulkSupplier
                          where t.ClientID == clientid
                          orderby t.SupplyID
                          select new SelectListItem
                          {
                              Value = t.SupplyID,
                              Text = t.SupplyID
                          }).ToList();
                result.Insert(0, new SelectListItem { Value = "", Text = "" });

                return result;
            }
        }

        public static List<SelectListItem> ddlTierSizes(int? clientid)
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> result = new List<SelectListItem>();

                result = (from t in db.tblTier
                          where t.ClientID == clientid
                          orderby t.Size
                          select new SelectListItem
                          {
                              Value = t.Size,
                              Text = t.Size
                          }).Distinct().ToList();
                result.Insert(0, new SelectListItem { Value = "", Text = "" });

                return result;
            }
        }

        public static List<SelectListItem> ddlUnitMeasure(int? clientid)
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> result = new List<SelectListItem>();

                result = (from t in db.tblTier
                          orderby t.ClientID
                          where t.ClientID == clientid
                          select new SelectListItem
                          {
                              Value = t.Size,
                              Text = t.Size
                          }).Distinct().ToList();
                result.Insert(0, new SelectListItem { Value = "", Text = "" });

                return result;
            }
        }
        
        #endregion SelectListItem Objects

        public static void EmailSmtpSend(string from, string to, string subject, string body)
        {
            string hostName = ConfigurationManager.AppSettings["rackspace.hostname"];
            string userName = ConfigurationManager.AppSettings["rackspace.username"];
            string password = ConfigurationManager.AppSettings["rackspace.password"];
            string port = ConfigurationManager.AppSettings["rackspace.port"];

            var msg = new System.Net.Mail.MailMessage();

            msg.From = new MailAddress(from);
            msg.To.Add(new MailAddress(to));
            msg.Bcc.Add(new MailAddress(from));                                                             //send email copy to self
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
                smtp.Port = Convert.ToInt32(port);
                smtp.EnableSsl = true;

                smtp.Send(msg);
            }
        }
    }
}