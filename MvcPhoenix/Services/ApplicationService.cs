using MvcPhoenix.DataLayer;
using MvcPhoenix.EF;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

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
            GL,

            [Display(Name = "Kilogram")]
            KG,

            [Display(Name = "Liter")]
            LT,

            [Display(Name = "Pound")]
            LB,
        }

        #endregion Choice Enum Lists

        #region DropDown Lists Construction

        public static string ddlBuildBillingGroup(int clientId)
        {
            using (var db = new CMCSQL03Entities())
            {
                var divisions = db.tblDivision
                                   .Where(t => t.ClientID == clientId)
                                   .OrderBy(t => t.DivisionName);

                var endUses = db.tblEndUse
                                .Where(t => t.ClientID == clientId)
                                .OrderBy(t => t.EndUse);

                string htmlString = "<option value='ALL' selected=true>ALL</option>";

                if (divisions.Count() > 0)
                {
                    htmlString = htmlString + "<optgroup label='Billing Groups'>";
                    foreach (var item in divisions)
                    {
                        htmlString = htmlString + "<option value=" + item.DivisionID + ">" + item.DivisionName + " / " + item.BusinessUnit + "</option>";
                    }
                    htmlString = htmlString + "</optgroup>";
                }

                if (endUses.Count() > 0)
                {
                    htmlString = htmlString + "<optgroup label='End Uses'>";
                    foreach (var item in endUses)
                    {
                        htmlString = htmlString + "<option value='" + item.EndUse + "'>" + item.EndUse + "</option>";
                    }
                    htmlString = htmlString + "</optgroup>";
                }

                htmlString = htmlString + "</select>";

                return htmlString;
            }
        }

        public static string ddlBuildDivision(int clientId)
        {
            using (var db = new CMCSQL03Entities())
            {
                var divisions = db.tblDivision
                                  .Where(t => t.ClientID == clientId)
                                  .OrderBy(t => t.DivisionName)
                                  .OrderBy(t => t.BusinessUnit);

                string htmlString = "<option value='0'></option>";

                if (divisions.Count() > 0)
                {
                    foreach (var item in divisions)
                    {
                        htmlString = htmlString + "<option value=" + item.DivisionID.ToString() + ">" + item.DivisionName + " - " + item.BusinessUnit + "</option>";
                    }
                }
                else
                {
                    htmlString = htmlString + "<option value=0>No Divisions Found</option>";
                }

                htmlString = htmlString + "</select>";

                return htmlString;
            }
        }

        public static string ddlBuildProductCode(int clientId)
        {
            using (var db = new CMCSQL03Entities())
            {
                var products = (from productdetail in db.tblProductDetail
                                join productmaster in db.tblProductMaster on productdetail.ProductMasterID equals productmaster.ProductMasterID
                                where productmaster.ClientID == clientId
                                orderby productdetail.ProductCode
                                select new { productdetail, productmaster });

                string htmlString = "<option value='' selected=true></option>";

                if (products.Count() > 0)
                {
                    foreach (var item in products)
                    {
                        htmlString = htmlString + "<option value=" + item.productdetail.ProductDetailID.ToString() + ">" + item.productdetail.ProductCode + " - (" + item.productmaster.MasterCode + ") - " + item.productdetail.ProductName + "</option>";
                    }
                }
                else
                {
                    htmlString = htmlString + "<option value=0>No Products Found</option>";
                }

                htmlString = htmlString + "</select>";

                return htmlString;
            }
        }

        public static string ddlBuildProductEquivalent(int clientId)
        {
            using (var db = new CMCSQL03Entities())
            {
                var products = (from productdetail in db.tblProductDetail
                                join productmaster in db.tblProductMaster on productdetail.ProductMasterID equals productmaster.ProductMasterID
                                where productmaster.ClientID == clientId
                                && productdetail.ProductCode != productmaster.MasterCode
                                orderby productdetail.ProductCode
                                select new
                                {
                                    productdetail.ProductDetailID,
                                    productdetail.ProductCode,
                                    productdetail.ProductName
                                }).ToList();

                string htmlString = "<option value='0'></option>";

                if (products.Count() > 0)
                {
                    foreach (var item in products)
                    {
                        htmlString = htmlString + "<option value='" + item.ProductDetailID + "'>" + item.ProductCode + " - " + item.ProductName + "</option>";
                    }
                }
                else
                {
                    htmlString = htmlString + "<option value=0>No Products Found</option>";
                }

                htmlString = htmlString + "</select>";

                return htmlString;
            }
        }

        public static string ddlBuildProductMaster(int clientId)
        {
            using (var db = new CMCSQL03Entities())
            {
                var products = db.tblProductMaster
                                 .Where(t => t.ClientID == clientId)
                                 .OrderBy(t => t.MasterCode)
                                 .OrderBy(t => t.MasterName);

                string htmlString = "<option value='' selected=true></option>";

                if (products.Count() > 0)
                {
                    foreach (var item in products)
                    {
                        htmlString = htmlString + "<option value='" + item.ProductMasterID + "'>" + item.MasterCode + " - " + item.MasterName + "</option>";
                    }
                }
                else
                {
                    htmlString = htmlString + "<option value=0>No Products Found</option>";
                }

                htmlString = htmlString + "</select>";

                return htmlString;
            }
        }

        public static string ddlBuildShelfMasterPackage()
        {
            using (var db = new CMCSQL03Entities())
            {
                var packages = db.tblPackage.OrderBy(t => t.Size);

                string htmlString = "<option value='0' selected=true>Select Package</option>";

                if (packages.Count() > 0)
                {
                    foreach (var item in packages)
                    {
                        htmlString = htmlString + "<option value='" + item.PackageID + "'>" + item.PartNumber + " - " + item.Description + "</option>";
                    }
                }
                else
                {
                    htmlString = htmlString + "<option value=0>No Packages Found</option>";
                }

                htmlString = htmlString + "</select>";

                return htmlString;
            }
        }

        public static string ddlBuildSize(int productdetailid)
        {
            bool isSpecialRequest = false;

            using (var db = new CMCSQL03Entities())
            {
                var shelfSizes = db.tblShelfMaster
                                   .Where(t => t.ProductDetailID == productdetailid
                                            && t.InactiveSize != true)
                                   .OrderBy(t => t.Size);

                string htmlString = "<option value='' selected=true></option>";

                if (shelfSizes.Count() > 0)
                {
                    foreach (var item in shelfSizes)
                    {
                        if (item.Size == "1SR")
                        {
                            isSpecialRequest = true;
                        }

                        htmlString = htmlString + "<option value=" + item.ShelfID.ToString() + ">" + item.Size + " - " + item.UnitWeight + "</option>";
                    }
                }

                if (!isSpecialRequest)
                {
                    htmlString = htmlString + "<option value='0'>1SR</option>";
                }

                return htmlString;
            }
        }

        #endregion DropDown Lists Construction

        #region SelectListItem Objects

        public static List<SelectListItem> ddlCarriers()
        {
            var carriers = new List<SelectListItem>();

            using (var db = new CMCSQL03Entities())
            {
                carriers = (from carrier in db.tblCarrier
                            orderby carrier.CarrierName
                            select new SelectListItem
                            {
                                Value = carrier.CarrierName,
                                Text = carrier.CarrierName
                            }).ToList();
                carriers.Insert(0, new SelectListItem { Value = "", Text = "" });
            }

            return carriers;
        }

        public static List<SelectListItem> ddlClientIDs()
        {
            var clients = new List<SelectListItem>();

            using (var db = new CMCSQL03Entities())
            {
                clients = (from client in db.tblClient
                           orderby client.ClientName
                           select new SelectListItem
                           {
                               Value = client.ClientID.ToString(),
                               Text = client.ClientName
                           }).ToList();
                clients.Insert(0, new SelectListItem { Value = "", Text = "" });
            }

            return clients;
        }

        public static List<SelectListItem> ddlCountries()
        {
            var countries = new List<SelectListItem>();

            using (var db = new CMCSQL03Entities())
            {
                countries = (from country in db.tblCountry
                             orderby country.Country
                             select new SelectListItem
                             {
                                 Value = country.Country,
                                 Text = country.Country
                             }).ToList();
                countries.Insert(0, new SelectListItem { Value = " ", Text = "" });
            }

            return countries;
        }

        public static List<SelectListItem> ddlDelayReasons()
        {
            var delayReasons = new List<SelectListItem>();

            delayReasons.Add(new SelectListItem { Value = "", Text = "" });
            delayReasons.Add(new SelectListItem { Value = "Backorder", Text = "Backorder" });
            delayReasons.Add(new SelectListItem { Value = "Oven Product", Text = "Oven Product" });
            delayReasons.Add(new SelectListItem { Value = "Humidity", Text = "Humidity" });
            delayReasons.Add(new SelectListItem { Value = "Very Large Order", Text = "Very Large Order" });
            delayReasons.Add(new SelectListItem { Value = "Questions/Info Needed", Text = "Questions/Info Needed" });
            delayReasons.Add(new SelectListItem { Value = "Special Doc Required", Text = "Special Doc Required" });
            delayReasons.Add(new SelectListItem { Value = "Special Request Size", Text = "Special Request Size" });
            delayReasons.Add(new SelectListItem { Value = "Return Order", Text = "Return Order" });
            delayReasons.Add(new SelectListItem { Value = "Waste", Text = "Waste" });
            delayReasons.Add(new SelectListItem { Value = "No partial delivery", Text = "No partial delivery" });
            delayReasons.Add(new SelectListItem { Value = "Special delivery date", Text = "Special delivery date" });
            delayReasons.Add(new SelectListItem { Value = "Public holiday", Text = "Public holiday" });
            delayReasons.Add(new SelectListItem { Value = "Freezable Procedure", Text = "Freezable Procedure" });
            delayReasons.Add(new SelectListItem { Value = "CMC delay, customer informed", Text = "CMC delay, customer informed" });
            delayReasons.Add(new SelectListItem { Value = "R&D flow", Text = "R&D flow" });
            delayReasons.Add(new SelectListItem { Value = "Special procedure", Text = "Special procedure" });
            delayReasons.Add(new SelectListItem { Value = "Misc. Charges", Text = "Misc. Charges" });
            delayReasons.Add(new SelectListItem { Value = "Transfer order", Text = "Transfer order" });
            delayReasons.Add(new SelectListItem { Value = "Consolidated Order", Text = "Consolidated Order" });
            delayReasons.Add(new SelectListItem { Value = "Approval Required", Text = "Approval Required" });
            delayReasons.Add(new SelectListItem { Value = "Conditioned packaging", Text = "Conditioned packaging" });
            delayReasons.Add(new SelectListItem { Value = "Product Not Setup", Text = "Product Not Setup" });
            delayReasons.Add(new SelectListItem { Value = "Frt Fwd-Pending Arrangement", Text = "Frt Fwd-Pending Arrangement" });
            delayReasons.Add(new SelectListItem { Value = "Labels, SDS", Text = "Labels, SDS" });

            return delayReasons;
        }

        public static List<SelectListItem> ddlDivisionIDs(int? clientid)
        {
            var divisions = new List<SelectListItem>();

            using (var db = new CMCSQL03Entities())
            {
                divisions = (from division in db.tblDivision
                             where division.ClientID == clientid
                             orderby division.DivisionName
                             select new SelectListItem
                             {
                                 Value = division.DivisionID.ToString(),
                                 Text = division.DivisionName + " / " + division.BusinessUnit
                             }).ToList();
            }

            return divisions;
        }

        public static List<SelectListItem> ddlEndUses(int? clientid)
        {
            var endUses = new List<SelectListItem>();

            using (var db = new CMCSQL03Entities())
            {
                endUses = (from enduse in db.tblEndUse
                           where enduse.ClientID == clientid
                           orderby enduse.EndUse
                           select new SelectListItem
                           {
                               Value = enduse.EndUse,
                               Text = enduse.EndUse
                           }).ToList();
                endUses.Insert(0, new SelectListItem { Value = "", Text = "" });
            }

            return endUses;
        }

        public static List<SelectListItem> ddlOrderItemIDs(int? orderid)
        {
            var orderItems = new List<SelectListItem>();

            using (var db = new CMCSQL03Entities())
            {
                orderItems = (from orderitem in db.tblOrderItem
                              where orderitem.OrderID == orderid
                              orderby orderitem.ProductCode
                              select new SelectListItem
                              {
                                  Value = orderitem.ItemID.ToString(),
                                  Text = orderitem.ProductCode
                              }).ToList();
                orderItems.Insert(0, new SelectListItem { Value = "", Text = "" });
            }

            return orderItems;
        }

        public static List<SelectListItem> ddlOrderItemStatusIDs()
        {
            var orderItemStatus = new List<SelectListItem>();

            orderItemStatus.Add(new SelectListItem { Value = "", Text = "" });
            orderItemStatus.Add(new SelectListItem { Value = "CL", Text = "Closed" });
            orderItemStatus.Add(new SelectListItem { Value = "OP", Text = "Open" });
            orderItemStatus.Add(new SelectListItem { Value = "CN", Text = "Cancelled" });

            return orderItemStatus;
        }

        public static List<SelectListItem> ddlOrderTransactionTypes()
        {
            var fixedCharges = new SelectListGroup() { Name = "Fixed Charges" };
            var variableCharges = new SelectListGroup() { Name = "Variable Charges" };

            var transactionTypes = new List<SelectListItem>();

            transactionTypes.Add(new SelectListItem { Value = "MEMO", Text = "Memo", Selected = true });

            // Fixed Charges
            transactionTypes.Add(new SelectListItem { Value = "Air Hazard Only", Text = "Air Hazard Only", Group = fixedCharges });
            transactionTypes.Add(new SelectListItem { Value = "Certificate Of Origin", Text = "Certificate Of Origin", Group = fixedCharges });
            transactionTypes.Add(new SelectListItem { Value = "CMC Pack", Text = "CMC Pack", Group = fixedCharges });
            transactionTypes.Add(new SelectListItem { Value = "Cool Pack", Text = "Cool Pack", Group = fixedCharges });
            transactionTypes.Add(new SelectListItem { Value = "Credit Card Fee", Text = "Credit Card Fee", Group = fixedCharges });
            transactionTypes.Add(new SelectListItem { Value = "Credit Card Order", Text = "Credit Card Order", Group = fixedCharges });
            transactionTypes.Add(new SelectListItem { Value = "Document Handling", Text = "Document Handling", Group = fixedCharges });
            transactionTypes.Add(new SelectListItem { Value = "Empty Packaging", Text = "Empty Packaging", Group = fixedCharges });
            transactionTypes.Add(new SelectListItem { Value = "External System", Text = "External System", Group = fixedCharges });
            transactionTypes.Add(new SelectListItem { Value = "Follow Up Order", Text = "Follow Up Order", Group = fixedCharges });
            transactionTypes.Add(new SelectListItem { Value = "Freezer Pack", Text = "Freezer Pack", Group = fixedCharges });
            transactionTypes.Add(new SelectListItem { Value = "GHS Labels", Text = "GHS Labels", Group = fixedCharges });
            transactionTypes.Add(new SelectListItem { Value = "Inactive Products", Text = "Inactive Products", Group = fixedCharges });
            transactionTypes.Add(new SelectListItem { Value = "Isolation", Text = "Isolation", Group = fixedCharges });
            transactionTypes.Add(new SelectListItem { Value = "Isolation Box", Text = "Isolation Box", Group = fixedCharges });
            transactionTypes.Add(new SelectListItem { Value = "IT Fee", Text = "IT Fee", Group = fixedCharges });
            transactionTypes.Add(new SelectListItem { Value = "Label Maintainance", Text = "Label Maintainance", Group = fixedCharges });
            transactionTypes.Add(new SelectListItem { Value = "Label Stock", Text = "Label Stock", Group = fixedCharges });
            transactionTypes.Add(new SelectListItem { Value = "Labels Printed", Text = "Labels Printed", Group = fixedCharges });
            transactionTypes.Add(new SelectListItem { Value = "Labor Relabel", Text = "Labor Relabel", Group = fixedCharges });
            transactionTypes.Add(new SelectListItem { Value = "Literature Fee", Text = "Literature Fee", Group = fixedCharges });
            transactionTypes.Add(new SelectListItem { Value = "Limited Quantity", Text = "Limited Quantity", Group = fixedCharges });
            transactionTypes.Add(new SelectListItem { Value = "Manual Handling", Text = "Manual Handling", Group = fixedCharges });
            transactionTypes.Add(new SelectListItem { Value = "MSDS Prints", Text = "MSDS Prints", Group = fixedCharges });
            transactionTypes.Add(new SelectListItem { Value = "New Label Setup", Text = "New Label Setup", Group = fixedCharges });
            transactionTypes.Add(new SelectListItem { Value = "New Product Setup", Text = "New Product Setup", Group = fixedCharges });
            transactionTypes.Add(new SelectListItem { Value = "Oberk Pack", Text = "Oberk Pack", Group = fixedCharges });
            transactionTypes.Add(new SelectListItem { Value = "Order Entry", Text = "Order Entry", Group = fixedCharges });
            transactionTypes.Add(new SelectListItem { Value = "Over Pack", Text = "Over Pack", Group = fixedCharges });
            transactionTypes.Add(new SelectListItem { Value = "Pallet Return", Text = "Pallet Return", Group = fixedCharges });
            transactionTypes.Add(new SelectListItem { Value = "Poison Pack", Text = "Poison Pack", Group = fixedCharges });
            transactionTypes.Add(new SelectListItem { Value = "Product Setup Changes", Text = "Product Setup Changes", Group = fixedCharges });
            transactionTypes.Add(new SelectListItem { Value = "QC Storage", Text = "QC Storage", Group = fixedCharges });
            transactionTypes.Add(new SelectListItem { Value = "RD Handling ADR", Text = "R&D Handling ADR", Group = fixedCharges });
            transactionTypes.Add(new SelectListItem { Value = "RD Handling IATA", Text = "R&D Handling IATA", Group = fixedCharges });
            transactionTypes.Add(new SelectListItem { Value = "RD Handling LQ", Text = "R&D Handling LQ", Group = fixedCharges });
            transactionTypes.Add(new SelectListItem { Value = "RD Handling Non Hazard", Text = "R&D Handling Non Hazard", Group = fixedCharges });
            transactionTypes.Add(new SelectListItem { Value = "Refrigerator Storage", Text = "Refrigerator Storage", Group = fixedCharges });
            transactionTypes.Add(new SelectListItem { Value = "Relabels", Text = "Relabels", Group = fixedCharges });
            transactionTypes.Add(new SelectListItem { Value = "Rush Shipment", Text = "Rush Shipment", Group = fixedCharges });
            transactionTypes.Add(new SelectListItem { Value = "SPA 197 Applied", Text = "SPA 197 Applied", Group = fixedCharges });
            transactionTypes.Add(new SelectListItem { Value = "SPS Paid Order", Text = "SPS Paid Order", Group = fixedCharges });
            transactionTypes.Add(new SelectListItem { Value = "UN Box", Text = "UN Box", Group = fixedCharges });
            transactionTypes.Add(new SelectListItem { Value = "Warehouse Storage", Text = "Warehouse Storage", Group = fixedCharges });
            transactionTypes.Add(new SelectListItem { Value = "WHMIS Labels", Text = "WHMIS Labels", Group = fixedCharges });

            // Variable Charges
            transactionTypes.Add(new SelectListItem { Value = "Administrative Waste Fee", Text = "Administrative Waste Fee", Group = variableCharges });
            transactionTypes.Add(new SelectListItem { Value = "Credit", Text = "Credit", Group = variableCharges });
            transactionTypes.Add(new SelectListItem { Value = "Customs Documents", Text = "Customs Documents", Group = variableCharges });
            transactionTypes.Add(new SelectListItem { Value = "Delivery Duties Taxes", Text = "Delivery Duties Taxes", Group = variableCharges });
            transactionTypes.Add(new SelectListItem { Value = "Documents", Text = "Documents", Group = variableCharges });
            transactionTypes.Add(new SelectListItem { Value = "Handling", Text = "Handling", Group = variableCharges });
            transactionTypes.Add(new SelectListItem { Value = "MautFuel", Text = "Maut Fuel", Group = variableCharges });
            transactionTypes.Add(new SelectListItem { Value = "Miscellaneous Labor", Text = "Miscellaneous Labor", Group = variableCharges });
            transactionTypes.Add(new SelectListItem { Value = "Other", Text = "Other", Group = variableCharges });
            transactionTypes.Add(new SelectListItem { Value = "Waste Processing", Text = "Waste Processing", Group = variableCharges });

            return transactionTypes;
        }

        public static List<SelectListItem> ddlOtherProductStorage()
        {
            var otherStorages = new List<SelectListItem>();

            otherStorages.Add(new SelectListItem { Value = "", Text = "" });
            otherStorages.Add(new SelectListItem { Value = "Toxic Storage", Text = "Toxic Storage" });
            otherStorages.Add(new SelectListItem { Value = "Spontaneously Combustible", Text = "Spontaneously Combustible" });
            otherStorages.Add(new SelectListItem { Value = "Oxidizer", Text = "Oxidizer" });
            otherStorages.Add(new SelectListItem { Value = "Base", Text = "Base" });
            otherStorages.Add(new SelectListItem { Value = "Acid", Text = "Acid" });
            otherStorages.Add(new SelectListItem { Value = "Clean Storage", Text = "Clean Storage" });
            otherStorages.Add(new SelectListItem { Value = "Flammable/Corrosive", Text = "Flammable/Corrosive" });
            otherStorages.Add(new SelectListItem { Value = "Toxic/Corrosive", Text = "Toxic/Corrosive" });
            otherStorages.Add(new SelectListItem { Value = "Biocide", Text = "Biocide" });
            otherStorages.Add(new SelectListItem { Value = "Biocide/Toxic", Text = "Biocide/Toxic" });
            otherStorages.Add(new SelectListItem { Value = "Biocide/Toxic/Corrosive", Text = "Biocide/Toxic/Corrosive" });
            otherStorages.Add(new SelectListItem { Value = "Biocide/Corrosive", Text = "Biocide/Corrosive" });
            otherStorages.Add(new SelectListItem { Value = "Biocide/Oxidizer", Text = "Biocide/Oxidizer" });
            otherStorages.Add(new SelectListItem { Value = "Flammable/Pueblo", Text = "Flammable/Pueblo" });
            otherStorages.Add(new SelectListItem { Value = "Corrosive/Flammable", Text = "Corrosive/Flammable" });
            otherStorages.Add(new SelectListItem { Value = "Biocide/Corrosive/Toxic", Text = "Biocide/Corrosive/Toxic" });
            otherStorages.Add(new SelectListItem { Value = "Corrosive/Toxic", Text = "Corrosive/Toxic" });
            otherStorages.Add(new SelectListItem { Value = "Biocide/Flammable/Corrosive", Text = "Biocide/Flammable/Corrosive" });
            otherStorages.Add(new SelectListItem { Value = "Flammable/Toxic/Corrosive", Text = "Flammable/Toxic/Corrosive" });
            otherStorages.Add(new SelectListItem { Value = "Biocide/Corrosive/Flammable", Text = "Biocide/Corrosive/Flammable" });
            otherStorages.Add(new SelectListItem { Value = "Biocide/Flammable/Toxic/Corrosive", Text = "Biocide/Flammable/Toxic/Corrosive" });
            otherStorages.Add(new SelectListItem { Value = "Biocide/Flammable/Corrosive/Toxic", Text = "Biocide/Flammable/Corrosive/Toxic" });

            return otherStorages;
        }

        public static List<SelectListItem> ddlPackageIDs(string size)
        {
            var packages = new List<SelectListItem>();

            using (var db = new CMCSQL03Entities())
            {
                packages = (from package in db.tblPackage
                            orderby package.Size
                            select new SelectListItem
                            {
                                Value = package.PackageID.ToString(),
                                Text = package.PartNumber + "-" + package.Description
                            }).ToList();
                packages.Insert(0, new SelectListItem { Value = "", Text = "" });
            }

            return packages;
        }

        public static List<SelectListItem> ddlProductCodes(int? clientid)
        {
            var products = new List<SelectListItem>();

            using (var db = new CMCSQL03Entities())
            {
                products = (from productdetail in db.tblProductDetail
                            join productmaster in db.tblProductMaster on productdetail.ProductMasterID equals productmaster.ProductMasterID
                            where productmaster.ClientID == clientid
                            orderby productdetail.ProductCode
                            select new SelectListItem
                            {
                                Value = productdetail.ProductDetailID.ToString(),
                                Text = productdetail.ProductCode + " " + productdetail.ProductName
                            }).ToList();
                products.Insert(0, new SelectListItem { Value = "", Text = "" });
            }

            return products;
        }

        public static List<SelectListItem> ddlProductCodeSizes(int? productdetailid)
        {
            var productSizes = new List<SelectListItem>();

            using (var db = new CMCSQL03Entities())
            {
                productSizes = (from shelf in db.tblShelfMaster
                                where shelf.ProductDetailID == productdetailid
                                && shelf.InactiveSize != true
                                orderby shelf.Size
                                select new SelectListItem
                                {
                                    Value = shelf.ShelfID.ToString(),
                                    Text = shelf.Size + " - " + shelf.UnitWeight
                                }).ToList();
            }

            return productSizes;
        }

        public static List<SelectListItem> ddlProductCodeXRefs(string productcode)
        {
            var productXRefs = new List<SelectListItem>();

            using (var db = new CMCSQL03Entities())
            {
                productXRefs = (from productxref in db.tblProductXRef
                                where productxref.CMCProductCode == productcode
                                select new SelectListItem
                                {
                                    Value = productxref.CustProductCode + " - " + productxref.CustProductName,
                                    Text = productxref.CustProductCode + " - " + productxref.CustProductName
                                }).ToList();
            }

            return productXRefs;
        }

        public static List<SelectListItem> ddlProductEquivalents(int? productmasterid, string productcode)
        {
            var products = new List<SelectListItem>();

            using (var db = new CMCSQL03Entities())
            {
                products = (from productdetail in db.tblProductDetail
                            where productdetail.ProductMasterID == productmasterid
                            && productdetail.ProductCode != productcode
                            select new SelectListItem
                            {
                                Value = productdetail.ProductCode + " / " + productdetail.ProductName,
                                Text = productdetail.ProductCode + " / " + productdetail.ProductName
                            }).ToList();
            }

            return products;
        }

        public static List<SelectListItem> ddlProductHandlingGloves()
        {
            var gloves = new List<SelectListItem>();

            gloves.Add(new SelectListItem { Value = "", Text = "" });
            gloves.Add(new SelectListItem { Value = "GMP NITRILE", Text = "GMP NITRILE" });
            gloves.Add(new SelectListItem { Value = "NEOPRENE", Text = "NEOPRENE" });
            gloves.Add(new SelectListItem { Value = "NEOPRENE+NITRIL", Text = "NEOPRENE+NITRIL" });
            gloves.Add(new SelectListItem { Value = "NITRILE", Text = "NITRILE" });

            return gloves;
        }

        public static List<SelectListItem> ddlProductMasterIDs(int? clientid)
        {
            var products = new List<SelectListItem>();

            using (var db = new CMCSQL03Entities())
            {
                products = (from productmaster in db.tblProductMaster
                            where productmaster.ClientID == clientid
                            orderby productmaster.MasterCode
                            select new SelectListItem
                            {
                                Value = productmaster.ProductMasterID.ToString(),
                                Text = productmaster.MasterCode + " - " + productmaster.MasterName.Substring(0, 25)
                            }).ToList();
                products.Insert(0, new SelectListItem { Value = "", Text = "" });
            }

            return products;
        }

        public static List<SelectListItem> ddlReasonCodes()
        {
            var reasonCodes = new List<SelectListItem>();

            using (var db = new CMCSQL03Entities())
            {
                reasonCodes = (from reasoncode in db.tblReasonCode
                               orderby reasoncode.Reason
                               select new SelectListItem
                               {
                                   Value = reasoncode.Reason,
                                   Text = reasoncode.Reason
                               }).ToList();
                reasonCodes.Insert(0, new SelectListItem { Value = "", Text = "" });
            }

            return reasonCodes;
        }

        public static List<SelectListItem> ddlRequestors(int? clientid)
        {
            var requestors = new List<SelectListItem>();

            using (var db = new CMCSQL03Entities())
            {
                requestors = (from clientcontact in db.tblClientContact
                              where clientcontact.ClientID == clientid
                              where clientcontact.ContactType == "Requestor"
                              orderby clientcontact.FullName
                              select new SelectListItem
                              {
                                  Value = clientcontact.ClientContactID.ToString(),
                                  Text = clientcontact.FullName
                              }).ToList();
                requestors.Insert(0, new SelectListItem { Value = "", Text = "" });
            }

            return requestors;
        }

        public static List<SelectListItem> ddlSalesReps(int? clientid)
        {
            var salesReps = new List<SelectListItem>();

            using (var db = new CMCSQL03Entities())
            {
                salesReps = (from clientcontact in db.tblClientContact
                             where clientcontact.ClientID == clientid
                             where clientcontact.ContactType == "SalesRep"
                             orderby clientcontact.FullName
                             select new SelectListItem
                             {
                                 Value = clientcontact.ClientContactID.ToString(),
                                 Text = clientcontact.FullName
                             }).ToList();
                salesReps.Insert(0, new SelectListItem { Value = "", Text = "" });
            }

            return salesReps;
        }

        public static List<SelectListItem> ddlShelfMasterIDs(int? productdetailid)
        {
            var shelfMasters = new List<SelectListItem>();

            using (var db = new CMCSQL03Entities())
            {
                shelfMasters = (from shelf in db.tblShelfMaster
                                join productdetail in db.tblProductDetail on shelf.ProductDetailID equals productdetail.ProductDetailID
                                where shelf.ProductDetailID == productdetailid
                                select new SelectListItem
                                {
                                    Value = shelf.ShelfID.ToString(),
                                    Text = shelf.Size
                                }).ToList();
                shelfMasters.Insert(0, new SelectListItem { Value = "", Text = "" });
            }

            return shelfMasters;
        }

        public static List<SelectListItem> ddlShipVias()
        {
            var carriers = new List<SelectListItem>();

            using (var db = new CMCSQL03Entities())
            {
                carriers = (from carrier in db.tblCarrier
                            orderby carrier.CarrierName
                            select new SelectListItem
                            {
                                Value = carrier.CarrierName,
                                Text = carrier.CarrierName
                            }).ToList();
                carriers.Insert(0, new SelectListItem { Value = "", Text = "" });
            }

            return carriers;
        }

        public static List<SelectListItem> ddlStatusNotes()
        {
            var statusNotes = new List<SelectListItem>();

            using (var db = new CMCSQL03Entities())
            {
                statusNotes = (from statusnote in db.tblStatusNotes
                               orderby statusnote.Note
                               select new SelectListItem
                               {
                                   Value = statusnote.Note,
                                   Text = statusnote.Note
                               }).ToList();
                statusNotes.Insert(0, new SelectListItem { Value = "", Text = "" });
            }

            return statusNotes;
        }

        public static List<SelectListItem> ddlSupplyIDs(int? clientid)
        {
            var suppliers = new List<SelectListItem>();

            using (var db = new CMCSQL03Entities())
            {
                suppliers = (from supplier in db.tblBulkSupplier
                             where supplier.ClientID == clientid
                             orderby supplier.SupplyID
                             select new SelectListItem
                             {
                                 Value = supplier.SupplyID,
                                 Text = supplier.SupplyID
                             }).ToList();
                suppliers.Insert(0, new SelectListItem { Value = "", Text = "" });
            }

            return suppliers;
        }

        public static List<SelectListItem> ddlTierSizes(int? clientid)
        {
            var tierSizes = new List<SelectListItem>();

            using (var db = new CMCSQL03Entities())
            {
                tierSizes = (from tier in db.tblTier
                             where tier.ClientID == clientid
                             orderby tier.Size
                             select new SelectListItem
                             {
                                 Value = tier.Size,
                                 Text = tier.Size
                             }).Distinct().ToList();
                tierSizes.Insert(0, new SelectListItem { Value = "", Text = "" });
            }

            return tierSizes;
        }

        public static List<SelectListItem> ddlUsers()
        {
            var users = new List<SelectListItem>();

            using (var db = new ApplicationDbContext())
            {
                users = (from user in db.Users
                         orderby user.FirstName
                         select new SelectListItem
                         {
                             Value = user.Email,
                             Text = user.FirstName + " " + user.LastName
                         }).ToList();
                users.Insert(0, new SelectListItem { Value = "", Text = "" });
            }

            return users;
        }

        #endregion SelectListItem Objects
    }
}