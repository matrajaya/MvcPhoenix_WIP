using MvcPhoenix.EF;
using MvcPhoenix.Extensions;
using MvcPhoenix.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcPhoenix.Models
{
    public class ProductService
    {
        public static ProductProfile GetProductDetail(int productdetailid)
        {
            var productProfile = new ProductProfile();
            productProfile.productdetailid = productdetailid;

            productProfile = ProductService.GetProductDetail(productProfile);

            return productProfile;
        }

        public static ProductProfile GetProductDetail(ProductProfile productProfile)
        {
            int? productDetailId = productProfile.productdetailid;

            using (var db = new CMCSQL03Entities())
            {
                var productDetail = db.tblProductDetail.Find(productDetailId);

                productProfile.productdetailid = productDetail.ProductDetailID;
                productProfile.productmasterid = productDetail.ProductMasterID;
                productProfile.divisionid = productDetail.DivisionID;
                productProfile.productcode = productDetail.ProductCode;
                productProfile.productname = productDetail.ProductName;
                productProfile.custcode = productDetail.CustCode;
                productProfile.multilotreq = productDetail.MultiLotReq;
                productProfile.extendableexpdt = productDetail.ExtendableExpDt;
                productProfile.harmonizedcode = productDetail.HarmonizedCode;
                productProfile.enduse = productDetail.EndUse;
                productProfile.sgrevisiondate = productDetail.SGRevisionDate;
                productProfile.msdsrevisiondate = productDetail.MSDSRevisionDate;
                productProfile.msdsrevisionnumber = productDetail.MSDSRevisionNumber;
                productProfile.labelrevisiondate = productDetail.LabelRevisionDate;
                productProfile.labelnumber = productDetail.LabelNumber;
                productProfile.productchecked = productDetail.ProductChecked;
                productProfile.checkedby = productDetail.CheckedBy;
                productProfile.checkedwhen = productDetail.CheckedWhen;
                productProfile.epabiocide = productDetail.EPABiocide;
                productProfile.labelinfo = productDetail.LabelInfo;
                productProfile.ghsready = productDetail.GHSReady;
                productProfile.customsvalue = productDetail.CustomsValue;
                productProfile.customsvalueunit = productDetail.CustomsValueUnit;
                productProfile.globalproduct = productDetail.GlobalProduct;
                productProfile.polymerizationhazard = productDetail.PolymerizationHazard;
                productProfile.sdscontactname = productDetail.SDSContactName;
                productProfile.sdscontactphone = productDetail.SDSContactPhone;
                productProfile.chinacertificationdate = productDetail.ChinaCertificationDate;
                productProfile.labelcontactname = productDetail.LabelContactName;
                productProfile.labelcontactphone = productDetail.LabelContactPhone;
                productProfile.technicalsheet = productDetail.TechnicalSheet;
                productProfile.technicalsheetrevisondate = productDetail.TechnicalSheetRevisionDate;
                productProfile.emergencycontactnumber = productDetail.EmergencyContactNumber;
                productProfile.epahazardouswaste = productDetail.EPAHazardousWaste;
                productProfile.nonrcrawaste = productDetail.NonRCRAWaste;
                productProfile.wasteprofilenumber = productDetail.WasteProfileNumber;
                productProfile.shippingchemicalname = productDetail.ShippingChemicalName;
                productProfile.labelnotesepa = productDetail.LabelNotesEPA;
                productProfile.grnunnumber = productDetail.GRNUNNUMBER;
                productProfile.grnpkgrp = productDetail.GRNPKGRP;
                productProfile.grnhazsubcl = productDetail.GRNHAZSUBCL;
                productProfile.grnlabel = productDetail.GRNLABEL;
                productProfile.grnsublabel = productDetail.GRNSUBLABEL;
                productProfile.grnhazcl = productDetail.GRNHAZCL;
                productProfile.grnshipname = productDetail.GRNSHIPNAME;
                productProfile.grnosname = productDetail.GRNOSNAME;
                productProfile.grnshipnamed = productDetail.GRNSHIPNAMED;
                productProfile.grntremacard = productDetail.GRNTREMACARD;
                productProfile.airunnumber = productDetail.AIRUNNUMBER;
                productProfile.airpkgrp = productDetail.AIRPKGRP;
                productProfile.airhazsubcl = productDetail.AIRHAZSUBCL;
                productProfile.airlabel = productDetail.AIRLABEL;
                productProfile.airsublabel = productDetail.AIRSUBLABEL;
                productProfile.airhazcl = productDetail.AIRHAZCL;
                productProfile.airshipname = productDetail.AIRSHIPNAME;
                productProfile.airnosname = productDetail.AIRNOSNAME;
                productProfile.seaunnum = productDetail.SEAUNNUM;
                productProfile.seapkgrp = productDetail.SEAPKGRP;
                productProfile.seahazsubcl = productDetail.SEAHAZSUBCL;
                productProfile.sealabel = productDetail.SEALABEL;
                productProfile.seasublabel = productDetail.SEASUBLABEL;
                productProfile.seahazcl = productDetail.SEAHAZCL;
                productProfile.seashipname = productDetail.SEASHIPNAME;
                productProfile.seanosname = productDetail.SEANOSNAME;
                productProfile.seashipnamed = productDetail.SEASHIPNAMED;
                productProfile.seahazmat = productDetail.SEAHAZMAT;
                productProfile.seaemsno = productDetail.SEAEMSNO;
                productProfile.seamfagno = productDetail.SEAMFAGNO;
                productProfile.rcraunnumber = productDetail.RCRAUNNumber;
                productProfile.rcrapkgrp = productDetail.RCRAPKGRP;
                productProfile.rcrahazsubcl = productDetail.RCRAHAZSUBCL;
                productProfile.rcralabel = productDetail.RCRALABEL;
                productProfile.rcrasublabel = productDetail.RCRASUBLABEL;
                productProfile.rcrahazcl = productDetail.RCRAHAZCL;
                productProfile.rcrashipname = productDetail.RCRASHIPNAME;
                productProfile.rcranosname = productDetail.RCRANOSNAME;
                productProfile.alertnotesshipping = productDetail.AlertNotesShipping;
                productProfile.alertnotesorderentry = productDetail.AlertNotesOrderEntry;
                productProfile.accuracyverified = productDetail.AccuracyVerified;
                productProfile.accuracyverifiedby = productDetail.AccuracyVerifiedBy;
                productProfile.active = productDetail.Active;
                productProfile.activedate = productDetail.ActiveDate;
                productProfile.CreateDateDetail = productDetail.CreateDate;
                productProfile.CreateUserDetail = productDetail.CreateUser;
                productProfile.UpdateDateDetail = productDetail.UpdateDate;
                productProfile.UpdateUserDetail = productDetail.UpdateUser;
            }

            return productProfile;
        }

        public static ProductProfile GetProductMaster(int? productmasterid)
        {
            var productProfile = new ProductProfile();
            productProfile.productmasterid = productmasterid;

            productProfile = ProductService.GetProductDetail(productProfile);

            return productProfile;
        }

        public static ProductProfile GetProductMaster(ProductProfile productProfile)
        {
            int? productMasterId = productProfile.productmasterid;

            using (var db = new CMCSQL03Entities())
            {
                var productMaster = db.tblProductMaster.Find(productMasterId);
                var client = db.tblClient.Find(productMaster.ClientID);

                productProfile.productmasterid = productMaster.ProductMasterID;
                productProfile.clientid = Convert.ToInt32(productMaster.ClientID);
                productProfile.clientname = client.ClientName;
                productProfile.mastercode = productMaster.MasterCode;
                productProfile.mastername = productMaster.MasterName;
                productProfile.supplyid = productMaster.SUPPLYID;
                productProfile.discontinued = productMaster.Discontinued;
                productProfile.noreorder = productMaster.NoReorder;
                productProfile.restrictedtoamount = productMaster.RestrictedToAmount;
                productProfile.createdate = productMaster.CreateDate;
                productProfile.masternotes = productMaster.MasterNotes;
                productProfile.masternotesalert = productMaster.MasterNotesAlert;
                productProfile.reorderadjustmentdays = productMaster.ReOrderAdjustmentDays;
                productProfile.ceaseshipdifferential = productMaster.CeaseShipDifferential;
                productProfile.cleanroomgmp = productMaster.CleanRoomGMP;
                productProfile.nitrogenblanket = productMaster.NitrogenBlanket;
                productProfile.moisturesensitive = productMaster.MoistureSensitive;
                productProfile.mixwell = productMaster.MixWell;
                productProfile.mixinginstructions = productMaster.MixingInstructions;
                productProfile.refrigerate = productMaster.Refrigerate;
                productProfile.donotpackabove60 = productMaster.DoNotPackAbove60;
                productProfile.handlingother = productMaster.HandlingOther;
                productProfile.flammableground = productMaster.FlammableGround;
                productProfile.heatpriortofilling = productMaster.HeatPriorToFilling;
                productProfile.flashpoint = productMaster.FlashPoint;
                productProfile.heatinginstructions = productMaster.HeatingInstructions;
                productProfile.otherhandlinginstr = productMaster.OtherHandlingInstr;
                productProfile.normalappearence = productMaster.NormalAppearence;
                productProfile.rejectioncriterion = productMaster.RejectionCriterion;
                productProfile.hood = productMaster.Hood;
                productProfile.labhood = productMaster.LabHood;
                productProfile.walkinhood = productMaster.WalkInHood;
                productProfile.safetyglasses = productMaster.SafetyGlasses;
                productProfile.gloves = productMaster.Gloves;
                productProfile.glovetype = productMaster.GloveType;
                productProfile.apron = productMaster.Apron;
                productProfile.armsleeves = productMaster.ArmSleeves;
                productProfile.respirator = productMaster.Respirator;
                productProfile.faceshield = productMaster.FaceShield;
                productProfile.fullsuit = productMaster.FullSuit;
                productProfile.cleanroomequipment = productMaster.CleanRoomEquipment;
                productProfile.otherequipment = productMaster.OtherEquipment;
                productProfile.toxic = productMaster.Toxic;
                productProfile.highlytoxic = productMaster.HighlyToxic;
                productProfile.reproductivetoxin = productMaster.ReproductiveToxin;
                productProfile.corrosivehaz = productMaster.CorrosiveHaz;
                productProfile.sensitizer = productMaster.Sensitizer;
                productProfile.carcinogen = productMaster.Carcinogen;
                productProfile.ingestion = productMaster.Ingestion;
                productProfile.inhalation = productMaster.Inhalation;
                productProfile.skin = productMaster.Skin;
                productProfile.skincontact = productMaster.SkinContact;
                productProfile.eyecontact = productMaster.EyeContact;
                productProfile.combustible = productMaster.Combustible;
                productProfile.corrosive = productMaster.Corrosive;
                productProfile.flammable = productMaster.Flammable;
                productProfile.oxidizer = productMaster.Oxidizer;
                productProfile.reactive = productMaster.Reactive;
                productProfile.hepatotoxins = productMaster.Hepatotoxins;
                productProfile.nephrotoxins = productMaster.Nephrotoxins;
                productProfile.neurotoxins = productMaster.Neurotoxins;
                productProfile.hepatopoetics = productMaster.Hepatopoetics;
                productProfile.pulmonarydisfunction = productMaster.PulmonaryDisfunction;
                productProfile.reporductivetoxin = productMaster.ReporductiveToxin;
                productProfile.cutaneoushazards = productMaster.CutaneousHazards;
                productProfile.eyehazards = productMaster.EyeHazards;
                productProfile.health = productMaster.Health;
                productProfile.flammability = productMaster.Flammability;
                productProfile.reactivity = productMaster.Reactivity;
                productProfile.otherequipmentdescription = productMaster.OtherEquipmentDescription;
                productProfile.shelflife = productMaster.ShelfLife;
                productProfile.booties = productMaster.Booties;
                productProfile.hazardclassground_sg = productMaster.HazardClassGround_SG;
                productProfile.irritant = productMaster.irritant;
                productProfile.righttoknow = productMaster.RighttoKnow;
                productProfile.sara = productMaster.SARA;
                productProfile.flammablestorageroom = productMaster.FlammableStorageRoom;
                productProfile.firelist = productMaster.FireList;
                productProfile.freezablelist = productMaster.FreezableList;
                productProfile.msdsothernumber = productMaster.MSDSOTHERNUMBER;
                productProfile.freezerstorage = productMaster.FREEZERSTORAGE;
                productProfile.clientreq = productMaster.CLIENTREQ;
                productProfile.cmcreq = productMaster.CMCREQ;
                productProfile.returnlocation = productMaster.RETURNLOCATION;
                productProfile.density = productMaster.DENSITY;
                productProfile.specialblend = productMaster.SpecialBlend;
                productProfile.sara302ehs = productMaster.SARA302EHS;
                productProfile.sara313 = productMaster.SARA313;
                productProfile.halfmaskrespirator = productMaster.HalfMaskRespirator;
                productProfile.fullfacerespirator = productMaster.FullFaceRespirator;
                productProfile.torque = productMaster.Torque;
                productProfile.torquerequirements = productMaster.TorqueRequirements;
                productProfile.otherstorage = productMaster.OtherStorage;
                productProfile.eecall = productMaster.EECAll;
                productProfile.rphrasesall = productMaster.RPhrasesAll;
                productProfile.sphrasesall = productMaster.SPhrasesAll;
                productProfile.phall = productMaster.PHAll;
                productProfile.english = productMaster.English;
                productProfile.german = productMaster.German;
                productProfile.dutch = productMaster.Dutch;
                productProfile.eecsymbol1 = productMaster.EECSymbol1;
                productProfile.eecsymbol2 = productMaster.EECSymbol2;
                productProfile.eecsymbol3 = productMaster.EECSymbol3;
                productProfile.handling = productMaster.Handling;
                productProfile.shippingnotes = productMaster.ShippingNotes;
                productProfile.otherlabelnotes = productMaster.OtherLabelNotes;
                productProfile.productdescription = productMaster.ProductDescription;
                productProfile.peroxideformer = productMaster.PeroxideFormer;
                productProfile.specificgravity = productMaster.SpecificGravity;
                productProfile.phvalue = productMaster.phValue;
                productProfile.physicaltoxic = productMaster.PhysicalToxic;
                productProfile.wastecode = productMaster.WasteCode;
                productProfile.countryoforigin = productMaster.CountryOfOrigin;
                productProfile.leadtime = productMaster.LeadTime;
                productProfile.dustfilter = productMaster.DustFilter;
                productProfile.temperaturecontrolledstorage = productMaster.TemperatureControlledStorage;
                productProfile.prepacked = productMaster.PrePacked;
                productProfile.alertnotesreceiving = productMaster.AlertNotesReceiving;
                productProfile.alertnotespackout = productMaster.AlertNotesPackout;
                productProfile.rcrareviewdate = productMaster.RCRAReviewDate;
                productProfile.wasteaccumstartdate = productMaster.WasteAccumStartDate;
                productProfile.productsetupdate = productMaster.ProductSetupDate;
                productProfile.CreateDateMaster = productMaster.CreateDate;
                productProfile.CreateUserMaster = productMaster.CreateUser;
                productProfile.UpdateDateMaster = productMaster.UpdateDate;
                productProfile.UpdateUserMaster = productMaster.UpdateUser;
            }

            return productProfile;
        }

        public static ProductProfile GetProductExtendedProps(ProductProfile productProfile)
        {
            int? productDetailId = productProfile.productdetailid;

            productProfile.ListOfProductNotes = ProductService.GetProductNotes(productDetailId);

            productProfile.ListOfCasNumbers = ProductService.GetCASItems(productDetailId);

            productProfile.ListOfShelfItems = ProductService.GetShelfItems(productDetailId);

            return productProfile;
        }

        public static List<ShelfMasterViewModel> GetShelfItems(int? productDetailId)
        {
            var shelfItems = new List<ShelfMasterViewModel>();

            using (var db = new CMCSQL03Entities())
            {
                shelfItems = (from t in db.tblShelfMaster
                              join p in db.tblPackage on t.PackageID equals p.PackageID into temp
                              from m in temp.DefaultIfEmpty()
                              where t.ProductDetailID == productDetailId
                              select new ShelfMasterViewModel
                              {
                                  shelfid = t.ShelfID,
                                  size = t.Size,
                                  bin = t.Bin,
                                  packageid = t.PackageID,
                                  notes = t.Notes,
                                  reordermin = t.ReorderMin,
                                  reordermax = t.ReorderMax,
                                  reorderqty = t.ReorderQty
                              }).ToList();
            }

            return shelfItems;
        }

        private static List<Cas> GetCASItems(int? productDetailId)
        {
            var casItems = new List<Cas>();

            using (var db = new CMCSQL03Entities())
            {
                casItems = (from t in db.tblCAS
                            where t.ProductDetailID == productDetailId
                            select new Cas
                            {
                                casid = t.CASID,
                                productdetailid = t.ProductDetailID,
                                casnumber = t.CasNumber,
                                chemicalname = t.ChemicalName,
                                percentage = t.Percentage,
                                restrictedqty = t.RestrictedQty,
                                restrictedamount = t.RestrictedAmount,
                                reportableqty = t.ReportableQty,
                                reportableamount = t.ReportableAmount,
                                lessthan = t.LessThan,
                                excludefromlabel = t.ExcludeFromLabel
                            }).ToList();
            }

            return casItems;
        }

        public static int? GetProductMasterId(int? productDetailId)
        {
            using (var db = new CMCSQL03Entities())
            {
                int? productMasterId = db.tblProductDetail
                                         .Where(x => x.ProductDetailID == productDetailId)
                                         .Select(x => x.ProductMasterID).FirstOrDefault();

                return productMasterId;
            }
        }

        public static int GetProductDetailId(int? productmasterid)
        {
            using (var db = new CMCSQL03Entities())
            {
                int productDetailId = (from pd in db.tblProductDetail
                                       join pm in db.tblProductMaster on pd.ProductMasterID equals pm.ProductMasterID
                                       where pm.ProductMasterID == productmasterid
                                       && pd.ProductCode == pm.MasterCode
                                       select pd.ProductDetailID).FirstOrDefault();

                return productDetailId;
            }
        }

        public static int CreateEquivalent(int productDetailId)
        {
            var productProfile = new ProductProfile();
            productProfile.productdetailid = productDetailId;

            productProfile = ProductService.GetProductDetail(productProfile);
            productProfile = ProductService.GetProductMaster(productProfile);
            productProfile = ProductService.GetProductExtendedProps(productProfile);

            // create new record and clear select values for manual entry
            productProfile.productdetailid = ProductService.NewProductDetailId();
            productProfile.productcode = productProfile.productcode + " Clone";
            productProfile.productname = productProfile.productname + " Clone";
            productProfile.sgrevisiondate = DateTime.UtcNow;
            productProfile.createdate = DateTime.UtcNow;
            productProfile.CreateDateDetail = DateTime.UtcNow;
            productProfile.CreateUserDetail = HttpContext.Current.User.Identity.Name;
            productProfile.UpdateDateDetail = DateTime.UtcNow;
            productProfile.UpdateUserDetail = HttpContext.Current.User.Identity.Name;

            // Save product profile in memory to db with new productdetailid
            int newProductDetailId = ProductService.SaveProductProfile(productProfile);

            // Clone details of original product shelfsize, ghs, and cas for new productdetailid
            ProductService.CloneShelfItems(productDetailId, productProfile.productdetailid);
            ProductService.CloneGHSItems(productDetailId, productProfile.productdetailid);
            ProductService.ClonePHItems(productDetailId, productProfile.productdetailid);
            ProductService.CloneCASItems(productDetailId, productProfile.productdetailid);

            // Create product log note
            var productNote = new ProductNote();
            productNote.ProductDetailId = productProfile.productdetailid;
            productNote.NoteDate = DateTime.UtcNow;
            productNote.Notes = "Equivalent created from product id: " + productDetailId;
            productNote.ReasonCode = "New";

            ProductService.SaveProductNote(productNote);

            return newProductDetailId;
        }

        #region Clone

        private static void CloneCASItems(int originalProductDetailId, int newProductDetailId)
        {
            using (var db = new CMCSQL03Entities())
            {
                var casItems = db.tblCAS
                                 .Where(x => x.ProductDetailID == originalProductDetailId)
                                 .ToList();

                for (int i = 0; i < casItems.Count; i++)
                {
                    var newCas = casItems[i].Clone();
                    newCas.ProductDetailID = newProductDetailId;

                    db.tblCAS.Add(newCas);
                    db.SaveChanges();
                }
            }
        }

        private static void ClonePHItems(int originalProductDetailId, int newProductDetailId)
        {
            using (var db = new CMCSQL03Entities())
            {
                var phItems = db.tblGHSPHDetail
                                .Where(x => x.ProductDetailID == originalProductDetailId)
                                .ToList();

                for (int i = 0; i < phItems.Count; i++)
                {
                    var newPh = phItems[i].Clone();
                    newPh.ProductDetailID = newProductDetailId;

                    db.tblGHSPHDetail.Add(newPh);
                    db.SaveChanges();
                }
            }
        }

        private static void CloneGHSItems(int originalProductDetailId, int newProductDetailId)
        {
            using (var db = new CMCSQL03Entities())
            {
                var ghsItems = db.tblGHS
                                 .Where(x => x.ProductDetailID == originalProductDetailId)
                                .ToList();

                for (int i = 0; i < ghsItems.Count; i++)
                {
                    var newGhs = ghsItems[i].Clone();
                    newGhs.ProductDetailID = newProductDetailId;

                    db.tblGHS.Add(newGhs);
                    db.SaveChanges();
                }
            }
        }

        private static void CloneShelfItems(int originalProductDetailId, int newProductDetailId)
        {
            using (var db = new CMCSQL03Entities())
            {
                var shelfItems = db.tblShelfMaster
                                   .Where(x => x.ProductDetailID == originalProductDetailId)
                                   .ToList();

                for (int i = 0; i < shelfItems.Count; i++)
                {
                    var newShelfItem = shelfItems[i].Clone();
                    newShelfItem.ProductDetailID = newProductDetailId;

                    db.tblShelfMaster.Add(newShelfItem);
                    db.SaveChanges();
                }
            }
        }

        #endregion Clone

        public static int SaveProductProfile(ProductProfile productProfile)
        {
            var productNote = new ProductNote();
            bool isNew = false;

            if (productProfile.productmasterid < 1)
            {
                productProfile.productmasterid = ProductService.NewProductMasterId();
                productNote.Notes = "Master product created";
                isNew = true;
            }

            if (productProfile.productdetailid < 1)
            {
                productProfile.productdetailid = ProductService.NewProductDetailId();
                productNote.Notes = "New product created";
                isNew = true;
            }

            ProductService.SaveProductMaster(productProfile);
            ProductService.SaveProductDetail(productProfile);

            if (isNew)
            {
                productNote.ReasonCode = "New";
                productNote.ProductDetailId = productProfile.productdetailid;
                productNote.NoteDate = DateTime.UtcNow;

                ProductService.SaveProductNote(productNote);
            }

            return productProfile.productdetailid;
        }

        public static void SaveProductDetail(ProductProfile productProfile)
        {
            using (var db = new CMCSQL03Entities())
            {
                var productDetail = db.tblProductDetail.Find(productProfile.productdetailid);

                productDetail.UpdateDate = DateTime.UtcNow;
                productDetail.UpdateUser = HttpContext.Current.User.Identity.Name;
                productDetail.ProductCode = productProfile.productcode;
                productDetail.ProductDetailID = productProfile.productdetailid;
                productDetail.ProductMasterID = productProfile.productmasterid;
                productDetail.DivisionID = productProfile.divisionid;
                productDetail.ProductCode = productProfile.productcode;
                productDetail.ProductName = productProfile.productname;
                productDetail.CustCode = productProfile.custcode;
                productDetail.MultiLotReq = productProfile.multilotreq;
                productDetail.ExtendableExpDt = productProfile.extendableexpdt;
                productDetail.HarmonizedCode = productProfile.harmonizedcode;
                productDetail.EndUse = productProfile.enduse;
                productDetail.SGRevisionDate = productProfile.sgrevisiondate;
                productDetail.MSDSRevisionDate = productProfile.msdsrevisiondate;
                productDetail.MSDSRevisionNumber = productProfile.msdsrevisionnumber;
                productDetail.LabelRevisionDate = productProfile.labelrevisiondate;
                productDetail.LabelNumber = productProfile.labelnumber;
                productDetail.ProductChecked = productProfile.productchecked;
                productDetail.CheckedBy = productProfile.checkedby;
                productDetail.CheckedWhen = productProfile.checkedwhen;
                productDetail.EPABiocide = productProfile.epabiocide;
                productDetail.LabelInfo = productProfile.labelinfo;
                productDetail.GHSReady = productProfile.ghsready;
                productDetail.CustomsValue = productProfile.customsvalue;
                productDetail.CustomsValueUnit = productProfile.customsvalueunit;
                productDetail.GlobalProduct = productProfile.globalproduct;
                productDetail.PolymerizationHazard = productProfile.polymerizationhazard;
                productDetail.SDSContactName = productProfile.sdscontactname;
                productDetail.SDSContactPhone = productProfile.sdscontactphone;
                productDetail.ChinaCertificationDate = productProfile.chinacertificationdate;
                productDetail.LabelContactName = productProfile.labelcontactname;
                productDetail.LabelContactPhone = productProfile.labelcontactphone;
                productDetail.TechnicalSheet = productProfile.technicalsheet;
                productDetail.TechnicalSheetRevisionDate = productProfile.technicalsheetrevisondate;
                productDetail.EmergencyContactNumber = productProfile.emergencycontactnumber;
                productDetail.EPAHazardousWaste = productProfile.epahazardouswaste;
                productDetail.NonRCRAWaste = productProfile.nonrcrawaste;
                productDetail.WasteProfileNumber = productProfile.wasteprofilenumber;
                productDetail.ShippingChemicalName = productProfile.shippingchemicalname;
                productDetail.LabelNotesEPA = productProfile.labelnotesepa;
                productDetail.GRNUNNUMBER = productProfile.grnunnumber;
                productDetail.GRNPKGRP = productProfile.grnpkgrp;
                productDetail.GRNHAZSUBCL = productProfile.grnhazsubcl;
                productDetail.GRNLABEL = productProfile.grnlabel;
                productDetail.GRNSUBLABEL = productProfile.grnsublabel;
                productDetail.GRNHAZCL = productProfile.grnhazcl;
                productDetail.GRNSHIPNAME = productProfile.grnshipname;
                productDetail.GRNOSNAME = productProfile.grnosname;
                productDetail.GRNSHIPNAMED = productProfile.grnshipnamed;
                productDetail.GRNTREMACARD = productProfile.grntremacard;
                productDetail.AIRUNNUMBER = productProfile.airunnumber;
                productDetail.AIRPKGRP = productProfile.airpkgrp;
                productDetail.AIRHAZSUBCL = productProfile.airhazsubcl;
                productDetail.AIRLABEL = productProfile.airlabel;
                productDetail.AIRSUBLABEL = productProfile.airsublabel;
                productDetail.AIRHAZCL = productProfile.airhazcl;
                productDetail.AIRSHIPNAME = productProfile.airshipname;
                productDetail.AIRNOSNAME = productProfile.airnosname;
                productDetail.SEAUNNUM = productProfile.seaunnum;
                productDetail.SEAPKGRP = productProfile.seapkgrp;
                productDetail.SEAHAZSUBCL = productProfile.seahazsubcl;
                productDetail.SEALABEL = productProfile.sealabel;
                productDetail.SEASUBLABEL = productProfile.seasublabel;
                productDetail.SEAHAZCL = productProfile.seahazcl;
                productDetail.SEASHIPNAME = productProfile.seashipname;
                productDetail.SEANOSNAME = productProfile.seanosname;
                productDetail.SEASHIPNAMED = productProfile.seashipnamed;
                productDetail.SEAHAZMAT = productProfile.seahazmat;
                productDetail.SEAEMSNO = productProfile.seaemsno;
                productDetail.SEAMFAGNO = productProfile.seamfagno;
                productDetail.AlertNotesShipping = productProfile.alertnotesshipping;
                productDetail.AlertNotesOrderEntry = productProfile.alertnotesorderentry;
                productDetail.RCRAUNNumber = productProfile.rcraunnumber;
                productDetail.RCRAPKGRP = productProfile.rcrapkgrp;
                productDetail.RCRAHAZSUBCL = productProfile.rcrahazsubcl;
                productDetail.RCRALABEL = productProfile.rcralabel;
                productDetail.RCRASUBLABEL = productProfile.rcrasublabel;
                productDetail.RCRAHAZCL = productProfile.rcrahazcl;
                productDetail.RCRASHIPNAME = productProfile.rcrashipname;
                productDetail.RCRANOSNAME = productProfile.rcranosname;
                productDetail.Active = productProfile.active;
                productDetail.ActiveDate = productProfile.activedate;
                productDetail.AccuracyVerified = productProfile.accuracyverified;

                if (productDetail.AccuracyVerified == true)
                {
                    productDetail.AccuracyVerifiedBy = HttpContext.Current.User.Identity.Name;
                }

                db.SaveChanges();
            }
        }

        public static void SaveProductMaster(ProductProfile productProfile)
        {
            using (var db = new CMCSQL03Entities())
            {
                var productMaster = db.tblProductMaster.Find(productProfile.productmasterid);

                productMaster.UpdateDate = DateTime.UtcNow;
                productMaster.UpdateUser = HttpContext.Current.User.Identity.Name;
                productMaster.ClientID = productProfile.clientid;
                productMaster.MasterCode = productProfile.mastercode;
                productMaster.MasterName = productProfile.mastername;
                productMaster.SUPPLYID = productProfile.supplyid;
                productMaster.Discontinued = productProfile.discontinued;
                productMaster.NoReorder = productProfile.noreorder;
                productMaster.RestrictedToAmount = productProfile.restrictedtoamount;
                productMaster.CreateDate = productProfile.createdate;
                productMaster.MasterNotes = productProfile.masternotes;
                productMaster.MasterNotesAlert = productProfile.masternotesalert;
                productMaster.ReOrderAdjustmentDays = productProfile.reorderadjustmentdays;
                productMaster.CeaseShipDifferential = productProfile.ceaseshipdifferential;
                productMaster.CleanRoomGMP = productProfile.cleanroomgmp;
                productMaster.NitrogenBlanket = productProfile.nitrogenblanket;
                productMaster.MoistureSensitive = productProfile.moisturesensitive;
                productMaster.MixWell = productProfile.mixwell;
                productMaster.MixingInstructions = productProfile.mixinginstructions;
                productMaster.Refrigerate = productProfile.refrigerate;
                productMaster.DoNotPackAbove60 = productProfile.donotpackabove60;
                productMaster.HandlingOther = productProfile.handlingother;
                productMaster.FlammableGround = productProfile.flammableground;
                productMaster.HeatPriorToFilling = productProfile.heatpriortofilling;
                productMaster.FlashPoint = productProfile.flashpoint;
                productMaster.HeatingInstructions = productProfile.heatinginstructions;
                productMaster.OtherHandlingInstr = productProfile.otherhandlinginstr;
                productMaster.NormalAppearence = productProfile.normalappearence;
                productMaster.RejectionCriterion = productProfile.rejectioncriterion;
                productMaster.Hood = productProfile.hood;
                productMaster.LabHood = productProfile.labhood;
                productMaster.WalkInHood = productProfile.walkinhood;
                productMaster.SafetyGlasses = productProfile.safetyglasses;
                productMaster.Gloves = productProfile.gloves;
                productMaster.GloveType = productProfile.glovetype;
                productMaster.Apron = productProfile.apron;
                productMaster.ArmSleeves = productProfile.armsleeves;
                productMaster.Respirator = productProfile.respirator;
                productMaster.FaceShield = productProfile.faceshield;
                productMaster.FullSuit = productProfile.fullsuit;
                productMaster.CleanRoomEquipment = productProfile.cleanroomequipment;
                productMaster.OtherEquipment = productProfile.otherequipment;
                productMaster.Toxic = productProfile.toxic;
                productMaster.HighlyToxic = productProfile.highlytoxic;
                productMaster.ReproductiveToxin = productProfile.reproductivetoxin;
                productMaster.CorrosiveHaz = productProfile.corrosivehaz;
                productMaster.Sensitizer = productProfile.sensitizer;
                productMaster.Carcinogen = productProfile.carcinogen;
                productMaster.Ingestion = productProfile.ingestion;
                productMaster.Inhalation = productProfile.inhalation;
                productMaster.Skin = productProfile.skin;
                productMaster.SkinContact = productProfile.skincontact;
                productMaster.EyeContact = productProfile.eyecontact;
                productMaster.Combustible = productProfile.combustible;
                productMaster.Corrosive = productProfile.corrosive;
                productMaster.Flammable = productProfile.flammable;
                productMaster.Oxidizer = productProfile.oxidizer;
                productMaster.Reactive = productProfile.reactive;
                productMaster.Hepatotoxins = productProfile.hepatotoxins;
                productMaster.Nephrotoxins = productProfile.nephrotoxins;
                productMaster.Neurotoxins = productProfile.neurotoxins;
                productMaster.Hepatopoetics = productProfile.hepatopoetics;
                productMaster.PulmonaryDisfunction = productProfile.pulmonarydisfunction;
                productMaster.ReporductiveToxin = productProfile.reporductivetoxin;
                productMaster.CutaneousHazards = productProfile.cutaneoushazards;
                productMaster.EyeHazards = productProfile.eyehazards;
                productMaster.Health = productProfile.health;
                productMaster.Flammability = productProfile.flammability;
                productMaster.Reactivity = productProfile.reactivity;
                productMaster.OtherEquipmentDescription = productProfile.otherequipmentdescription;
                productMaster.ShelfLife = productProfile.shelflife;
                productMaster.Booties = productProfile.booties;
                productMaster.HazardClassGround_SG = productProfile.hazardclassground_sg;
                productMaster.irritant = productProfile.irritant;
                productMaster.RighttoKnow = productProfile.righttoknow;
                productMaster.SARA = productProfile.sara;
                productMaster.FlammableStorageRoom = productProfile.flammablestorageroom;
                productMaster.FireList = productProfile.firelist;
                productMaster.FreezableList = productProfile.freezablelist;
                productMaster.MSDSOTHERNUMBER = productProfile.msdsothernumber;
                productMaster.FREEZERSTORAGE = productProfile.freezerstorage;
                productMaster.CLIENTREQ = productProfile.clientreq;
                productMaster.CMCREQ = productProfile.cmcreq;
                productMaster.RETURNLOCATION = productProfile.returnlocation;
                productMaster.DENSITY = productProfile.density;
                productMaster.SpecialBlend = productProfile.specialblend;
                productMaster.SARA302EHS = productProfile.sara302ehs;
                productMaster.SARA313 = productProfile.sara313;
                productMaster.HalfMaskRespirator = productProfile.halfmaskrespirator;
                productMaster.FullFaceRespirator = productProfile.fullfacerespirator;
                productMaster.Torque = productProfile.torque;
                productMaster.TorqueRequirements = productProfile.torquerequirements;
                productMaster.OtherStorage = productProfile.otherstorage;
                productMaster.EECAll = productProfile.eecall;
                productMaster.RPhrasesAll = productProfile.rphrasesall;
                productMaster.SPhrasesAll = productProfile.sphrasesall;
                productMaster.PHAll = productProfile.phall;
                productMaster.English = productProfile.english;
                productMaster.German = productProfile.german;
                productMaster.Dutch = productProfile.dutch;
                productMaster.EECSymbol1 = productProfile.eecsymbol1;
                productMaster.EECSymbol2 = productProfile.eecsymbol2;
                productMaster.EECSymbol3 = productProfile.eecsymbol3;
                productMaster.Handling = productProfile.handling;
                productMaster.ShippingNotes = productProfile.shippingnotes;
                productMaster.OtherLabelNotes = productProfile.otherlabelnotes;
                productMaster.ProductDescription = productProfile.productdescription;
                productMaster.PeroxideFormer = productProfile.peroxideformer;
                productMaster.SpecificGravity = productProfile.specificgravity;
                productMaster.phValue = productProfile.phvalue;
                productMaster.PhysicalToxic = productProfile.physicaltoxic;
                productMaster.WasteCode = productProfile.wastecode;
                productMaster.CountryOfOrigin = productProfile.countryoforigin;
                productMaster.LeadTime = productProfile.leadtime;
                productMaster.DustFilter = productProfile.dustfilter;
                productMaster.TemperatureControlledStorage = productProfile.temperaturecontrolledstorage;
                productMaster.PrePacked = productProfile.prepacked;
                productMaster.AlertNotesReceiving = productProfile.alertnotesreceiving;
                productMaster.AlertNotesPackout = productProfile.alertnotespackout;
                productMaster.RCRAReviewDate = productProfile.rcrareviewdate;
                productMaster.WasteAccumStartDate = productProfile.wasteaccumstartdate;
                productMaster.ProductSetupDate = productProfile.productsetupdate;

                db.SaveChanges();
            }
        }

        public static int NewProductDetailId()
        {
            using (var db = new CMCSQL03Entities())
            {
                var productDetail = new tblProductDetail { };
                productDetail.CreateDate = DateTime.UtcNow;
                productDetail.CreateUser = HttpContext.Current.User.Identity.Name;

                db.tblProductDetail.Add(productDetail);
                db.SaveChanges();

                int newProductDetailId = productDetail.ProductDetailID;

                return newProductDetailId;
            }
        }

        public static int NewProductMasterId()
        {
            using (var db = new CMCSQL03Entities())
            {
                var productMaster = new tblProductMaster { };
                productMaster.CreateDate = DateTime.UtcNow;
                productMaster.CreateUser = HttpContext.Current.User.Identity.Name;

                db.tblProductMaster.Add(productMaster);
                db.SaveChanges();

                int newProductMasterId = productMaster.ProductMasterID;

                return newProductMasterId;
            }
        }

        public static tblProductMaster GetProductMasterReference(int productdetailid)
        {
            using (var db = new CMCSQL03Entities())
            {
                var productReference = (from productMaster in db.tblProductMaster
                                        join productDetail in db.tblProductDetail on productMaster.ProductMasterID equals productDetail.ProductMasterID
                                        where productDetail.ProductDetailID == productdetailid
                                        select productMaster).FirstOrDefault();

                return productReference;
            }
        }

        public static void DeActivateProductMaster(int productMasterId)
        {
            using (var db = new CMCSQL03Entities())
            {
                string updateQuery = "UPDATE tblProductMaster SET Discontinued=1 WHERE ProductMasterID=" + productMasterId;
                db.Database.ExecuteSqlCommand(updateQuery);
            }
        }

        public static UN GetUN(string unNumber)
        {
            var un = new UN();

            using (var db = new CMCSQL03Entities())
            {
                var getUN = db.tblUN.OrderByDescending(x => x.UNID)
                                    .Where(x => x.UNNumber == unNumber)
                                    .FirstOrDefault();

                un.unid = getUN.UNID;
                un.unnumber = getUN.UNNumber;
                un.hazardclass = getUN.HazardClass;
                un.propershippingname = getUN.ProperShippingName;
                un.nosname = getUN.NOSName;
                un.labelreq = getUN.LabelReq;
                un.subclass = getUN.SubClass;
                un.subsidlabelreq = getUN.SubSidLabelReq;
                un.packinggroup = getUN.PackingGroup;
            }

            return un;
        }

        #region Product Notes

        public static List<ProductNote> GetProductNotes(int? productDetailId)
        {
            var productNotes = new List<ProductNote>();

            using (var db = new CMCSQL03Entities())
            {
                productNotes = (from t in db.tblPPPDLogNote
                                where t.ProductDetailID == productDetailId
                                orderby t.PPPDLogNoteID descending
                                select new ProductNote
                                {
                                    ProductNoteId = t.PPPDLogNoteID,
                                    ProductDetailId = t.ProductDetailID,
                                    NoteDate = t.NoteDate,
                                    Notes = t.Notes,
                                    ReasonCode = t.ReasonCode,
                                    Charge = t.Charge,
                                    UpdateDate = t.UpdateDate,
                                    UpdateUser = t.UpdateUser,
                                    CreateDate = t.CreateDate,
                                    CreateUser = t.CreateUser
                                }).ToList();
            }

            return productNotes;
        }
        
        public static ProductNote GetProductNote(int productDetailLogNoteId)
        {
            var productNote = new ProductNote();

            using (var db = new CMCSQL03Entities())
            {
                var getProductNote = db.tblPPPDLogNote.Find(productDetailLogNoteId);

                productNote.ProductNoteId = getProductNote.PPPDLogNoteID;
                productNote.ProductDetailId = getProductNote.ProductDetailID;
                productNote.ReasonCode = getProductNote.ReasonCode;
                productNote.NoteDate = getProductNote.NoteDate;
                productNote.Notes = getProductNote.Notes;
                productNote.Charge = getProductNote.Charge;
                productNote.UpdateDate = getProductNote.UpdateDate;
                productNote.UpdateUser = getProductNote.UpdateUser;
                productNote.CreateDate = getProductNote.CreateDate;
                productNote.CreateUser = getProductNote.CreateUser;
            }

            return productNote;
        }

        public static ProductNote CreateProductNote(int productDetailId)
        {
            var productNote = new ProductNote();

            using (var db = new CMCSQL03Entities())
            {
                productNote.ProductNoteId = -1;
                productNote.ProductDetailId = productDetailId;
                productNote.ReasonCode = null;
                productNote.NoteDate = DateTime.UtcNow;
                productNote.Notes = null;
                productNote.Charge = 0;
                productNote.UpdateDate = DateTime.UtcNow;
                productNote.UpdateUser = HttpContext.Current.User.Identity.Name;
                productNote.CreateDate = DateTime.UtcNow;
                productNote.CreateUser = HttpContext.Current.User.Identity.Name;
            }

            return productNote;
        }

        public static void SaveProductNote(ProductNote productNote)
        {
            using (var db = new CMCSQL03Entities())
            {
                if (productNote.ProductNoteId < 1)
                {
                    var newProductDetailLogNote = new tblPPPDLogNote();
                    newProductDetailLogNote.CreateDate = DateTime.UtcNow;
                    newProductDetailLogNote.CreateUser = HttpContext.Current.User.Identity.Name;
                    newProductDetailLogNote.Charge = GetReasonCodeCharge(productNote.ProductDetailId, productNote.ReasonCode);

                    db.tblPPPDLogNote.Add(newProductDetailLogNote);
                    db.SaveChanges();

                    productNote.ProductNoteId = newProductDetailLogNote.PPPDLogNoteID;
                    productNote.Charge = newProductDetailLogNote.Charge;
                }

                var productDetailLogNote = db.tblPPPDLogNote.Find(productNote.ProductNoteId);

                productDetailLogNote.ProductDetailID = productNote.ProductDetailId;
                productDetailLogNote.NoteDate = productNote.NoteDate;
                productDetailLogNote.Notes = productNote.Notes;
                productDetailLogNote.ReasonCode = productNote.ReasonCode;
                productDetailLogNote.Charge = productNote.Charge;
                productDetailLogNote.UpdateDate = DateTime.UtcNow;
                productDetailLogNote.UpdateUser = HttpContext.Current.User.Identity.Name;

                db.SaveChanges();
            }
        }

        private static decimal? GetReasonCodeCharge(int? productDetailId, string reasonCode)
        {
            decimal? charge = 0;
            int clientId;
            int? productMasterId = GetProductMasterId(productDetailId);
            

            using (var db = new CMCSQL03Entities())
            {
                var productMaster = db.tblProductMaster.Find(productMasterId);

                clientId = productMaster.ClientID ?? 0;
                if (clientId > 0)
                {
                    var serviceChargeRates = ClientService.GetServiceChargeRates(clientId);

                    if (reasonCode == "New")
                    {
                        charge = serviceChargeRates.NewProductSetup;
                    }
                }
            }

            return charge;
        }

        public static void DeleteProductNote(int productDetailLogNoteId)
        {
            using (var db = new CMCSQL03Entities())
            {
                string deleteQuery = "DELETE FROM tblPPPDLogNote WHERE PPPDLogNoteID=" + productDetailLogNoteId;
                db.Database.ExecuteSqlCommand(deleteQuery);
            }
        }

        #endregion Product Notes

        #region CAS

        public static List<Cas> GetCasItems(int productDetailId)
        {
            var casItems = new List<Cas>();

            using (var db = new CMCSQL03Entities())
            {
                casItems = (from cas in db.tblCAS
                            where cas.ProductDetailID == productDetailId
                            select new Cas
                            {
                                casid = cas.CASID,
                                productdetailid = cas.ProductDetailID,
                                casnumber = cas.CasNumber,
                                chemicalname = cas.ChemicalName,
                                percentage = cas.Percentage,
                                restrictedqty = cas.RestrictedQty,
                                restrictedamount = cas.RestrictedAmount,
                                reportableqty = cas.ReportableQty,
                                reportableamount = cas.ReportableAmount,
                                lessthan = cas.LessThan,
                                excludefromlabel = cas.ExcludeFromLabel
                            }).ToList();
            }

            return casItems;
        }

        public static Cas GetCAS(int casId)
        {
            var cas = new Cas();

            using (var db = new CMCSQL03Entities())
            {
                var getCAS = db.tblCAS.Find(casId);

                cas.casid = getCAS.CASID;
                cas.productdetailid = getCAS.ProductDetailID;
                cas.casnumber = getCAS.CasNumber;
                cas.chemicalname = getCAS.ChemicalName;
                cas.percentage = getCAS.Percentage;
                cas.restrictedqty = getCAS.RestrictedQty;
                cas.restrictedamount = getCAS.RestrictedAmount;
                cas.reportableqty = getCAS.ReportableQty;
                cas.reportableamount = getCAS.ReportableAmount;
                cas.lessthan = getCAS.LessThan;
                cas.excludefromlabel = getCAS.ExcludeFromLabel;
            }

            return cas;
        }

        public static Cas CreateCAS(int productDetailId)
        {
            Cas cas = new Cas();
            cas.casid = -1;
            cas.productdetailid = productDetailId;

            return cas;
        }

        public static void SaveCAS(Cas cas)
        {
            using (var db = new CMCSQL03Entities())
            {
                if (cas.casid == -1)
                {
                    var newrec = new tblCAS();
                    db.tblCAS.Add(newrec);

                    db.SaveChanges();
                    cas.casid = newrec.CASID;
                }

                var CAS = db.tblCAS.Find(cas.casid);

                CAS.ProductDetailID = cas.productdetailid;
                CAS.CasNumber = cas.casnumber;
                CAS.ChemicalName = cas.chemicalname;
                CAS.Percentage = cas.percentage;
                CAS.RestrictedQty = cas.restrictedqty;
                CAS.RestrictedAmount = cas.restrictedamount;
                CAS.ReportableQty = cas.reportableqty;
                CAS.ReportableAmount = cas.reportableamount;
                CAS.LessThan = cas.lessthan;
                CAS.ExcludeFromLabel = cas.excludefromlabel;

                db.SaveChanges();
            }
        }

        public static void DeleteCAS(int casId)
        {
            using (var db = new CMCSQL03Entities())
            {
                string deleteQuery = "DELETE FROM tblCAS WHERE CASID=" + casId;
                db.Database.ExecuteSqlCommand(deleteQuery);
            }
        }

        #endregion CAS

        #region Client Product Cross Reference

        public static ClientProductXRef GetClientProductXRef(int productXRefId)
        {
            var clientXRef = new ClientProductXRef();

            using (var db = new CMCSQL03Entities())
            {
                var productXRef = db.tblProductXRef.Find(productXRefId);

                clientXRef.ProductXRefID = productXRef.ProductXRefID;
                clientXRef.ClientID = productXRef.ClientID;
                clientXRef.CMCProductCode = productXRef.CMCProductCode;
                clientXRef.CMCSize = productXRef.CMCSize;
                clientXRef.ClientProductCode = productXRef.CustProductCode;
                clientXRef.ClientProductName = productXRef.CustProductName;
                clientXRef.ClientSize = productXRef.CustSize;
            }

            return clientXRef;
        }

        public static void SaveClientProductXRef(ClientProductXRef clientProductXRef)
        {
            using (var db = new CMCSQL03Entities())
            {
                if (clientProductXRef.ProductXRefID == -1)
                {
                    var newProductXRef = new tblProductXRef();
                    db.tblProductXRef.Add(newProductXRef);
                    db.SaveChanges();

                    clientProductXRef.ProductXRefID = newProductXRef.ProductXRefID;
                }

                var productXRef = db.tblProductXRef.Find(clientProductXRef.ProductXRefID);

                productXRef.ClientID = clientProductXRef.ClientID;
                productXRef.CMCProductCode = clientProductXRef.CMCProductCode;
                productXRef.CMCSize = clientProductXRef.CMCSize;
                productXRef.CustProductCode = clientProductXRef.ClientProductCode;
                productXRef.CustSize = clientProductXRef.ClientSize;
                productXRef.CustProductName = clientProductXRef.ClientProductName;

                db.SaveChanges();
            }
        }

        public static void DeleteProductXRef(int productXRefId)
        {
            using (var db = new CMCSQL03Entities())
            {
                string deleteQuery = "DELETE FROM tblProductXRef WHERE ProductXRefID=" + productXRefId;
                db.Database.ExecuteSqlCommand(deleteQuery);
            }
        }

        #endregion Client Product Cross Reference
    }
}