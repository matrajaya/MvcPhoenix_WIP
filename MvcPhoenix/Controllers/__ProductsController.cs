using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcPhoenix.Models;
using MvcPhoenix.EF;

namespace MvcPhoenix.Controllers
{
    public class ProductsController : Controller
    {
        public ActionResult Index()
        {
            //return View("~/Views/Products/Index.cshtml");
            return View();
        }

        public string ProductCodesDropDown(int id, string divid)
        {
            string s = "";
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            var qry = (from t in db.tblProductDetail
                       join t2 in db.tblProductMaster on t.ProductMasterID equals t2.ProductMasterID
                       where t2.ClientID == id
                       orderby t.ProductCode
                       select new { t.ProductDetailID, t.ProductCode, t.ProductName, t2.MasterCode });

            // IMPORTANT: set the id and the name and onchange js function
            s = "<select name='" + divid + "' id='" + divid + "' class='form-control' onchange='getid(this)' >";
            s = s + "<option value='0' selected=true>Product Code</option>";

            foreach (var item in qry)
            {
                s = s + "<option value=" + item.ProductDetailID.ToString() + ">" + item.ProductCode + " - " + item.MasterCode + " - " + item.ProductName + "</option>";
            }
            
            s = s + "</select>";
            
            db.Dispose();
            
            return s;
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (id == 0)
            {
                // return View("~/Views/Products/Index.cshtml"); // cycle back
                return RedirectToAction("Index", "Products");
            }
            else
            {
                ProductProfile PP = new ProductProfile();
                PP.productdetailid = id;
                PP = FillFromPD(PP);
                PP = FillFromPM(PP);
                PP = fnFillOtherPMProps(PP);
                //return View("~/Views/Products/ProductProfileEdit.cshtml", PP);
                return View(PP);
            }
        }

        [HttpPost]
        // public ActionResult New(int id)
        public ActionResult Create(int clientid2)
        {
            if (clientid2 == 0)
            {
                //return View("~/Views/Products/Index.cshtml"); // cycle back
                return RedirectToAction("Index", "Products");
            }
            else
            {
                ProductProfile PP = new ProductProfile();
                PP.clientid = clientid2;
                PP.productmasterid = -1;
                PP.productdetailid = -1;

                PP = fnFillOtherPMProps(PP);

                return View(PP);
            }
        }

        [HttpPost]
        public ActionResult Equiv(int productdetailid3)
        {
            if (productdetailid3 == 0)
            {
                //return View("~/Views/Products/Index.cshtml"); // cycle back
                return RedirectToAction("Index", "Products");
            }
            else
            {
                using (var db = new CMCSQL03Entities())
                {
                    var q = (from t in db.tblProductDetail where t.ProductDetailID == productdetailid3 select new { t.ProductMasterID }).FirstOrDefault();
                    ProductProfile PP = new ProductProfile();
                    PP.productdetailid = -1;
                    PP.productmasterid = q.ProductMasterID;
                    PP = FillFromPM(PP);
                    PP = fnFillOtherPMProps(PP);
                    return View("~/Views/Products/Create.cshtml", PP);
                }
            }
        }

        #region Older Post Actions
        // Older POST versions - need for pc to test plumbing using landing page 
        [HttpPost]
        public ActionResult SetUpProductProfileEdit(int productdetailid1)
        {
            if (productdetailid1 == 0)
            { return View("~/Views/Products/Index.cshtml"); }
            else
            { return RedirectToAction("Edit", new { id = productdetailid1 }); }
        }

        [HttpPost]
        public ActionResult SetUpProductProfileNew(int clientid2)
        {
            if (clientid2 == 0)
            { return View("~/Views/Products/Index.cshtml"); }
            else
            { return RedirectToAction("Edit", new { id = clientid2 }); }
        }

        [HttpPost]
        public ActionResult SetUpProductProfileEquiv(int productdetailid3)
        {
            if (productdetailid3 == 0)
            { return View("~/Views/Products/Index.cshtml"); }
            else
            {
                using (var db = new CMCSQL03Entities())
                {
                    return RedirectToAction("Edit", new { id = productdetailid3 });
                }

            }
        }
        #endregion


        // ***********************************************************************
        // Controller Support Actions Below
        // ***********************************************************************

        // These can be moved to an external .cs file and referenced in from this controller...

        private ProductProfile fnFillOtherPMProps(ProductProfile PP)
        {

            using (var db = new EF.CMCSQL03Entities())
            {

                //WasteCodes
                PP.ListOfWasteCodes = (from t in db.tblWasteCode
                                       where t.ProductDetailID == PP.productdetailid
                                       select
                                           new WasteCode { wastecodeid = t.WasteCodeID, wastecode = t.WasteCode, profilenumber = t.ProfileNumber }).ToList();

                //GloveType
                List<SelectListItem> gloves = new List<SelectListItem>();
                gloves.Add(new SelectListItem { Value = "", Text = "" });
                gloves.Add(new SelectListItem { Value = "GMP NITRILE", Text = "GMP NITRILE" });
                gloves.Add(new SelectListItem { Value = "NEOPRENE", Text = "NEOPRENE" });
                gloves.Add(new SelectListItem { Value = "NEOPRENE+NITRIL", Text = "NEOPRENE+NITRIL" });
                gloves.Add(new SelectListItem { Value = "NITRILE", Text = "NITRILE" });
                PP.ListOfGloves = gloves;

                //PackageTypes
                PP.ListOfPackageTypes = (from t in db.tblPackageType orderby t.PartNumber select new SelectListItem { Value = t.PartNumber, Text = t.Description }).ToList();

                // Equivs
                PP.ListOfEquivalents = (from t in db.tblProductDetail where t.ProductMasterID == PP.productmasterid && t.ProductCode != PP.productcode select new SelectListItem { Value = t.ProductCode, Text = t.ProductCode }).ToList();
                //if(PP.ListOfEquivalents.Count()==0)
                //{ PP.ListOfEquivalents.Add(new SelectListItem { Value = "0", Text = "No Equivalents" }); }

                PP.ListOfEndUsesForCustoms = (from t in db.tblEndUseForCustoms orderby t.EndUse select new SelectListItem { Value = t.EndUse, Text = t.EndUse }).ToList();

                PP.ListOfHarmonizedCodes = (from t in db.tblHSCode orderby t.HarmonizedCode select new SelectListItem { Value = t.HarmonizedCode, Text = t.HarmonizedCode }).ToList();

                // Productcode xRefs
                PP.ListOfProductCodesXRefs = (from t in db.tblProductXRef where t.CMCProductCode == PP.mastercode select new SelectListItem { Value = t.ProductXRefID.ToString(), Text = t.CustProductCode }).ToList();

                // Logo filename (needs to be moved to a Client class)
                var q = (from t in db.tblClient where t.ClientID == PP.clientid select new { t.ClientName, t.LogoFileName }).FirstOrDefault();
                PP.clientname = q.ClientName;
                PP.logofilename = "http://www.mysamplecenter.com/Logos/" + q.LogoFileName;

                // Divisions
                PP.ListOfDivisions = (from t in db.tblDivision where t.ClientID == PP.clientid select new SelectListItem { Value = t.DivisionID.ToString(), Text = t.Division }).ToList();
                PP.ListOfDivisions.Insert(0, (new SelectListItem { Value = "0", Text = "" }));

                // SupplyIDs
                PP.ListOfSupplyIDs = (from t in db.tblBulkSupplier where t.ClientID == PP.clientid select new SelectListItem { Value = t.SupplyID, Text = t.SupplyID }).ToList();
                PP.ListOfSupplyIDs.Insert(0, new SelectListItem { Value = "0", Text = "" });

                PP.ListOfProductNotes = (from t in db.tblProductNotes
                                         where t.ProductDetailID == PP.productdetailid
                                         select new ProductNote
                                         {
                                             productnoteid = t.ProductNoteID,
                                             productdetailid = t.ProductDetailID,
                                             notedate = t.NoteDate,
                                             notes = t.Notes,
                                             reasoncode = t.ReasonCode
                                         }).ToList();

                PP.ListOfCasNumbers = (from t in db.tblCAS
                                       where t.ProductDetailID == PP.productdetailid
                                       select new Cas
                                       {
                                           casid = t.CASID,
                                           productdetailid = t.ProductDetailID,
                                           casnumber = t.CasNumber,
                                           chemicalname = t.ChemicalName,
                                           percentage = t.Percentage,
                                           restrictedqty = t.RestrictedQty,
                                           restrictedamount = t.RestrictedAmount,
                                           packonreceipt = t.PackOnReceipt,
                                           reportableqty = t.ReportableQty,
                                           reportableamount = t.ReportableAmount,
                                           lessthan = t.LessThan,
                                           excludefromlabel = t.ExcludeFromLabel
                                       }).ToList();


                // ShelfItems
                var qshelf = (from t in db.tblShelfMaster
                              where t.ProductDetailID == PP.productdetailid
                              select new ShelfMaster
                              {
                                  shelfid = t.ShelfID,
                                  warehouse = t.Warehouse,
                                  size = t.Size,
                                  bin = t.Bin,
                                  package = t.Package,
                                  groundhazard = t.GroundHazard,
                                  airhazard = t.AirHazard,
                                  notes = t.Notes,
                                  busarea = t.BusArea,
                                  mnemonic = t.Mnemonic,
                                  reordermin = t.ReorderMin,
                                  reordermax = t.ReorderMax,
                                  reorderqty = t.ReorderQty
                              }).ToList();
                PP.ListOfShelfItems = qshelf;

            }
            return PP;
        }

        // Pass a PP, return original plus tblProductDetail properties
        private ProductProfile FillFromPD(ProductProfile PP)
        {
            //db
            using (var db = new EF.CMCSQL03Entities())
            {
                var qd = (from t in db.tblProductDetail where t.ProductDetailID == PP.productdetailid select t).FirstOrDefault();
                PP.productdetailid = qd.ProductDetailID;
                //PP.fkproductmasterid = qd.ProductMasterID;
                PP.productmasterid = qd.ProductMasterID;
                //PP.sglegacyid = qd.SGLegacyID;
                PP.divisionid = qd.DivisionID;
                PP.busarea = qd.BusArea;
                PP.productcode = qd.ProductCode;
                PP.productname = qd.ProductName;
                PP.custcode = qd.CustCode;
                PP.multilotreq = qd.MultiLotReq;
                PP.extendableexpdt = qd.ExtendableExpDt;
                PP.harmonizedcode = qd.HarmonizedCode;
                PP.enduse = qd.EndUse;
                PP.sgrevisiondate = qd.SGRevisionDate;
                PP.msdsrevisiondate = qd.MSDSRevisionDate;
                PP.msdsrevisionnumber = qd.MSDSRevisionNumber;
                PP.labelrevisiondate = qd.LabelRevisionDate;
                PP.labelnumber = qd.LabelNumber;
                PP.productchecked = qd.ProductChecked;
                PP.checkedby = qd.CheckedBy;
                PP.checkedwhen = qd.CheckedWhen;
                PP.epabiocide = qd.EPABiocide;
                PP.labelinfo = qd.LabelInfo;
                PP.ghsready = qd.GHSReady;
                PP.customsvalue = qd.CustomsValue;
                PP.customsvalueunit = qd.CustomsValueUnit;
                PP.globalproduct = qd.GlobalProduct;
                // added by II 10/23/2015
                PP.accuracyverified = qd.AccuracyVerified;

                PP.sdscontactname = qd.SDSContactName;
                PP.sdscontactphone = qd.SDSContactPhone;
                PP.chinacertificationdate = qd.ChinaCertificationDate;
                PP.labelcontactname = qd.LabelContactName;
                PP.labelcontactphone = qd.LabelContactPhone;
                PP.technicalsheet = qd.TechnicalSheet;
                PP.technicalsheetrevisondate = qd.TechnicalSheetRevisionDate;

                PP.emergencycontactnumber = qd.EmergencyContactNumber;

                PP.epahazardouswaste = qd.EPAHazardousWaste;
                PP.nonrcrawaste = qd.NonRCRAWaste;
                PP.wasteprofilenumber = qd.WasteProfileNumber;

                PP.polymerizationhazard = qd.PolymerizationHazard;

                // fields add by Iffy 10/28 (already in Master ???)
                PP.shippingchemicalname = qd.ShippingChemicalName;
                PP.labelnotesepa = qd.LabelNotesEPA;

                PP.grnshipname = qd.GRNSHIPNAME;
                PP.grnshipnamed = qd.GRNSHIPNAMED;
                PP.grnosname = qd.GRNOSNAME;
                PP.grnunnumber = qd.GRNUNNUMBER;
                PP.grnhazcl = qd.GRNHAZCL;
                PP.grnpkgrp = qd.GRNPKGRP;
                PP.grnlabel = qd.GRNLABEL;
                PP.grnsublabel = qd.GRNSUBLABEL;
                PP.grntremacard = qd.GRNTREMACARD;
                PP.airshipname = qd.AIRSHIPNAME;
                PP.airnosname = qd.AIRNOSNAME;
                PP.airunnumber = qd.AIRUNNUMBER;
                PP.airhazcl = qd.AIRHAZCL;
                PP.airpkgrp = qd.AIRPKGRP;
                PP.airlabel = qd.AIRLABEL;
                PP.airsublabel = qd.AIRSUBLABEL;
                PP.airhazsubcl = qd.AIRHAZSUBCL;
                PP.grnhazsubcl = qd.GRNHAZSUBCL;
                PP.seashipname = qd.SEASHIPNAME;
                PP.seashipnamed = qd.SEASHIPNAMED;
                PP.seanosname = qd.SEANOSNAME;
                PP.seaunnum = qd.SEAUNNUM;
                PP.seapkgrp = qd.SEAPKGRP;
                PP.seahazcl = qd.SEAHAZCL;
                PP.sealabel = qd.SEALABEL;
                PP.seahazsubcl = qd.SEAHAZSUBCL;
                PP.seasublabel = qd.SEASUBLABEL;
                PP.seahazmat = qd.SEAHAZMAT;
                PP.seaemsno = qd.SEAEMSNO;
                PP.seamfagno = qd.SEAMFAGNO;


                return PP;
            }
        }

        // Pass a PP, return original plus tblProductMaster properties
        private ProductProfile FillFromPM(ProductProfile PP)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                var q = (from t in db.tblProductMaster where t.ProductMasterID == PP.productmasterid select t).FirstOrDefault();
                // we known the PD.id
                //PP.productmasterid = qm.ProductMasterID;
                PP.productmasterid = q.ProductMasterID;
                PP.clientid = q.ClientID;
                //PP.sglegacyid = q.SGLegacyID;
                //PP.sdlegacyid = q.SDLegacyID;
                PP.mastercode = q.MasterCode;
                PP.mastername = q.MasterName;
                PP.masterdivisionid = q.MasterDivisionID;
                PP.supplyid = q.SUPPLYID;
                PP.discontinued = q.Discontinued;
                PP.noreorder = q.NoReorder;
                PP.packoutonreceipt = q.PackOutOnReceipt;
                PP.restrictedtoamount = q.RestrictedToAmount;
                PP.createdate = q.CreateDate;
                PP.masternotes = q.MasterNotes;
                PP.masternotesalert = q.MasterNotesAlert;
                PP.reorderadjustmentdays = q.ReOrderAdjustmentDays;
                PP.ceaseshipdifferential = q.CeaseShipDifferential;
                PP.cleanroomgmp = q.CleanRoomGMP;
                PP.nitrogenblanket = q.NitrogenBlanket;
                PP.moisturesensitive = q.MoistureSensitive;
                PP.mixwell = q.MixWell;
                PP.mixinginstructions = q.MixingInstructions;
                PP.refrigerate = q.Refrigerate;
                PP.donotpackabove60 = q.DoNotPackAbove60;
                PP.handlingother = q.HandlingOther;
                PP.flammableground = q.FlammableGround;
                PP.heatpriortofilling = q.HeatPriorToFilling;
                PP.flashpoint = q.FlashPoint;
                PP.heatinginstructions = q.HeatingInstructions;

                //cd says rename to OtherHandlingInstr
                //PP.other = q.Other; 
                PP.otherhandlinginstr = q.OtherHandlingInstr;


                PP.normalappearence = q.NormalAppearence;
                PP.rejectioncriterion = q.RejectionCriterion;
                PP.hood = q.Hood;
                PP.labhood = q.LabHood;
                PP.walkinhood = q.WalkInHood;
                PP.safetyglasses = q.SafetyGlasses;
                PP.gloves = q.Gloves;
                PP.glovetype = q.GloveType;
                PP.apron = q.Apron;
                PP.armsleeves = q.ArmSleeves;
                PP.respirator = q.Respirator;
                PP.faceshield = q.FaceShield;
                PP.fullsuit = q.FullSuit;
                PP.cleanroomequipment = q.CleanRoomEquipment;
                PP.otherequipment = q.OtherEquipment;
                PP.toxic = q.Toxic;
                PP.highlytoxic = q.HighlyToxic;
                PP.reproductivetoxin = q.ReproductiveToxin;
                PP.corrosivehaz = q.CorrosiveHaz;
                PP.sensitizer = q.Sensitizer;
                PP.carcinogen = q.Carcinogen;
                PP.ingestion = q.Ingestion;
                PP.inhalation = q.Inhalation;
                PP.skin = q.Skin;
                PP.skincontact = q.SkinContact;
                PP.eyecontact = q.EyeContact;
                PP.combustible = q.Combustible;
                PP.corrosive = q.Corrosive;
                PP.flammable = q.Flammable;
                PP.oxidizer = q.Oxidizer;
                PP.reactive = q.Reactive;
                PP.hepatotoxins = q.Hepatotoxins;
                PP.nephrotoxins = q.Nephrotoxins;
                PP.neurotoxins = q.Neurotoxins;
                PP.hepatopoetics = q.Hepatopoetics;
                PP.pulmonarydisfunction = q.PulmonaryDisfunction;
                PP.reporductivetoxin = q.ReporductiveToxin;
                PP.cutaneoushazards = q.CutaneousHazards;
                PP.eyehazards = q.EyeHazards;
                PP.health = q.Health;
                PP.flammability = q.Flammability;
                PP.reactivity = q.Reactivity;
                PP.otherequipmentdescription = q.OtherEquipmentDescription;
                PP.shlflife = q.ShlfLife;
                PP.booties = q.Booties;
                PP.hazardclassground_sg = q.HazardClassGround_SG;
                PP.irritant = q.irritant;
                PP.righttoknow = q.RighttoKnow;
                PP.sara = q.SARA;
                PP.flammablestorageroom = q.FlammableStorageRoom;
                PP.firelist = q.FireList;
                PP.freezablelist = q.FreezableList;
                PP.refrigeratedlist = q.RefrigeratedList;
                PP.msdsothernumber = q.MSDSOTHERNUMBER;
                PP.freezerstorage = q.FREEZERSTORAGE;
                PP.clientreq = q.CLIENTREQ;
                PP.cmcreq = q.CMCREQ;
                PP.returnlocation = q.RETURNLOCATION;
                PP.density = q.DENSITY;
                PP.specialblend = q.SpecialBlend;
                PP.sara302ehs = q.SARA302EHS;
                PP.sara313 = q.SARA313;
                PP.halfmaskrespirator = q.HalfMaskRespirator;
                PP.fullfacerespirator = q.FullFaceRespirator;
                PP.restrictedamount = q.Restrictedamount;
                PP.torque = q.Torque;
                PP.torquerequirements = q.TorqueRequirements;
                PP.otherpkg = q.OtherPkg;
                PP.eecall = q.EECAll;
                PP.rphrasesall = q.RPhrasesAll;
                PP.sphrasesall = q.SPhrasesAll;
                PP.phall = q.PHAll;
                PP.english = q.English;
                PP.german = q.German;
                PP.dutch = q.Dutch;
                PP.eecsymbol1 = q.EECSymbol1;
                PP.eecsymbol2 = q.EECSymbol2;
                PP.eecsymbol3 = q.EECSymbol3;
                PP.handling = q.Handling;
                PP.shippingnotes = q.ShippingNotes;
                PP.otherlabelnotes = q.OtherLabelNotes;
                PP.productdescription = q.ProductDescription;
                PP.peroxideformer = q.PeroxideFormer;


                PP.specificgravity = q.SpecificGravity;
                PP.phvalue = q.phValue;

                //new field added by II
                PP.physicaltoxic = q.PhysicalToxic;

                //PP.company_mdb = q.Company_MDB;
                //PP.division_mdb = q.Division_MDB;
                //PP.location_mdb = q.Location_MDB;

                return PP;
            }
        }


        // ************** Save to DB **********************************************
        [HttpPost]
        public ActionResult SaveProductProfile(ProductProfile PP)
        {
            //save master first, if new, push the new Productmasterid into the detail property
            System.Threading.Thread.Sleep(1000);    // help AJAX
            if (!ModelState.IsValid)
            {
                return Content("Model is invalid At " + DateTime.Now.ToString());
            }

            try
            {
                if (PP.productmasterid == -1)
                {
                    PP.productmasterid = fnNewProductMasterID();
                }
                SaveProductMaster(PP);
                if (PP.productdetailid == -1)
                {
                    PP.productdetailid = fnNewProductDetailID();
                }
                SaveProductDetail(PP);
                return Content("Profile UPDATED at " + DateTime.Now.ToString());
            }
            catch
            {
                return Content("DB Error - Profile NOT UPDATED at " + DateTime.Now.ToString());
            }
        }

        public void SaveProductDetail(ProductProfile PP)
        {
            using (var db = new CMCSQL03Entities())
            {
                //fnArchiveProductMaster(pm.productmasterid);
                var q = (from t in db.tblProductDetail where t.ProductDetailID == PP.productdetailid select t).FirstOrDefault();
                q.ProductCode = PP.productcode;
                q.ProductDetailID = PP.productdetailid;
                q.ProductMasterID = PP.productmasterid;
                //q.SGLegacyID = pd.sglegacyid;
                q.DivisionID = PP.divisionid;
                q.BusArea = PP.busarea;
                q.ProductCode = PP.productcode;
                q.ProductName = PP.productname;
                q.CustCode = PP.custcode;
                q.MultiLotReq = PP.multilotreq;
                q.ExtendableExpDt = PP.extendableexpdt;
                q.HarmonizedCode = PP.harmonizedcode;
                q.EndUse = PP.enduse;
                q.SGRevisionDate = PP.sgrevisiondate;
                q.MSDSRevisionDate = PP.msdsrevisiondate;
                q.MSDSRevisionNumber = PP.msdsrevisionnumber;
                q.LabelRevisionDate = PP.labelrevisiondate;
                q.LabelNumber = PP.labelnumber;
                q.ProductChecked = PP.productchecked;
                q.CheckedBy = PP.checkedby;
                q.CheckedWhen = PP.checkedwhen;
                q.EPABiocide = PP.epabiocide;
                q.LabelInfo = PP.labelinfo;
                q.GHSReady = PP.ghsready;
                q.CustomsValue = PP.customsvalue;
                q.CustomsValueUnit = PP.customsvalueunit;
                q.GlobalProduct = PP.globalproduct;

                q.AccuracyVerified = PP.accuracyverified;

                q.SDSContactName = PP.sdscontactname;
                q.SDSContactPhone = PP.sdscontactphone;
                q.ChinaCertificationDate = PP.chinacertificationdate;
                q.LabelContactName = PP.labelcontactname;
                q.LabelContactPhone = PP.labelcontactphone;
                q.TechnicalSheet = PP.technicalsheet;
                q.TechnicalSheetRevisionDate = PP.technicalsheetrevisondate;

                q.EmergencyContactNumber = PP.emergencycontactnumber;

                q.EPAHazardousWaste = PP.epahazardouswaste;
                q.NonRCRAWaste = PP.nonrcrawaste;
                q.WasteProfileNumber = PP.wasteprofilenumber;

                q.PolymerizationHazard = PP.polymerizationhazard;

                // fields add by Iffy 10/28 (already in Master ???)
                q.ShippingChemicalName = PP.shippingchemicalname;
                q.LabelNotesEPA = PP.labelnotesepa;

                q.GRNSHIPNAME = PP.grnshipname;
                q.GRNSHIPNAMED = PP.grnshipnamed;
                q.GRNOSNAME = PP.grnosname;
                q.GRNUNNUMBER = PP.grnunnumber;
                q.GRNHAZCL = PP.grnhazcl;
                q.GRNPKGRP = PP.grnpkgrp;
                q.GRNLABEL = PP.grnlabel;
                q.GRNSUBLABEL = PP.grnsublabel;
                q.GRNTREMACARD = PP.grntremacard;
                q.AIRSHIPNAME = PP.airshipname;
                q.AIRNOSNAME = PP.airnosname;
                q.AIRUNNUMBER = PP.airunnumber;
                q.AIRHAZCL = PP.airhazcl;
                q.AIRPKGRP = PP.airpkgrp;
                q.AIRLABEL = PP.airlabel;
                q.AIRSUBLABEL = PP.airsublabel;
                q.AIRHAZSUBCL = PP.airhazsubcl;
                q.GRNHAZSUBCL = PP.grnhazsubcl;
                q.SEASHIPNAME = PP.seashipname;
                q.SEASHIPNAMED = PP.seashipnamed;
                q.SEANOSNAME = PP.seanosname;
                q.SEAUNNUM = PP.seaunnum;
                q.SEAPKGRP = PP.seapkgrp;
                q.SEAHAZCL = PP.seahazcl;
                q.SEALABEL = PP.sealabel;
                q.SEAHAZSUBCL = PP.seahazsubcl;
                q.SEASUBLABEL = PP.seasublabel;
                q.SEAHAZMAT = PP.seahazmat;
                q.SEAEMSNO = PP.seaemsno;
                q.SEAMFAGNO = PP.seamfagno;

                //q.Company_MDB = pd.company_mdb;
                //q.MasterCode_MDB = pd.mastercode_mdb;
                //q.Division_MDB = pd.division_mdb;
                db.SaveChanges();
            }
        }

        public void SaveProductMaster(ProductProfile pm)
        {
            using (var db = new CMCSQL03Entities())
            {
                //fnArchiveProductMaster(pm.productmasterid);
                var q = (from t in db.tblProductMaster where t.ProductMasterID == pm.productmasterid select t).FirstOrDefault();

                //q.ProductMasterID = pm.productmasterid;
                q.ClientID = pm.clientid;
                //q.SGLegacyID = pm.sglegacyid;
                //q.SDLegacyID = pm.sdlegacyid;
                q.MasterCode = pm.mastercode;
                q.MasterName = pm.mastername;
                q.MasterDivisionID = pm.masterdivisionid;
                q.SUPPLYID = pm.supplyid;
                q.Discontinued = pm.discontinued;
                q.NoReorder = pm.noreorder;
                q.PackOutOnReceipt = pm.packoutonreceipt;
                q.RestrictedToAmount = pm.restrictedtoamount;
                q.CreateDate = pm.createdate;
                q.MasterNotes = pm.masternotes;
                q.MasterNotesAlert = pm.masternotesalert;
                q.ReOrderAdjustmentDays = pm.reorderadjustmentdays;
                q.CeaseShipDifferential = pm.ceaseshipdifferential;
                q.CleanRoomGMP = pm.cleanroomgmp;
                q.NitrogenBlanket = pm.nitrogenblanket;
                q.MoistureSensitive = pm.moisturesensitive;
                q.MixWell = pm.mixwell;
                q.MixingInstructions = pm.mixinginstructions;
                q.Refrigerate = pm.refrigerate;
                q.DoNotPackAbove60 = pm.donotpackabove60;
                q.HandlingOther = pm.handlingother;
                q.FlammableGround = pm.flammableground;
                q.HeatPriorToFilling = pm.heatpriortofilling;
                q.FlashPoint = pm.flashpoint;
                q.HeatingInstructions = pm.heatinginstructions;
                // name change per cd q.Other = pm.other;
                q.OtherHandlingInstr = pm.otherhandlinginstr;
                q.NormalAppearence = pm.normalappearence;
                q.RejectionCriterion = pm.rejectioncriterion;
                q.Hood = pm.hood;
                q.LabHood = pm.labhood;
                q.WalkInHood = pm.walkinhood;
                q.SafetyGlasses = pm.safetyglasses;
                q.Gloves = pm.gloves;
                q.GloveType = pm.glovetype;
                q.Apron = pm.apron;
                q.ArmSleeves = pm.armsleeves;
                q.Respirator = pm.respirator;
                q.FaceShield = pm.faceshield;
                q.FullSuit = pm.fullsuit;
                q.CleanRoomEquipment = pm.cleanroomequipment;
                q.OtherEquipment = pm.otherequipment;
                q.Toxic = pm.toxic;
                q.HighlyToxic = pm.highlytoxic;
                q.ReproductiveToxin = pm.reproductivetoxin;
                q.CorrosiveHaz = pm.corrosivehaz;
                q.Sensitizer = pm.sensitizer;
                q.Carcinogen = pm.carcinogen;
                q.Ingestion = pm.ingestion;
                q.Inhalation = pm.inhalation;
                q.Skin = pm.skin;
                q.SkinContact = pm.skincontact;
                q.EyeContact = pm.eyecontact;
                q.Combustible = pm.combustible;
                q.Corrosive = pm.corrosive;
                q.Flammable = pm.flammable;
                q.Oxidizer = pm.oxidizer;
                q.Reactive = pm.reactive;
                q.Hepatotoxins = pm.hepatotoxins;
                q.Nephrotoxins = pm.nephrotoxins;
                q.Neurotoxins = pm.neurotoxins;
                q.Hepatopoetics = pm.hepatopoetics;
                q.PulmonaryDisfunction = pm.pulmonarydisfunction;
                q.ReporductiveToxin = pm.reporductivetoxin;
                q.CutaneousHazards = pm.cutaneoushazards;
                q.EyeHazards = pm.eyehazards;
                q.Health = pm.health;
                q.Flammability = pm.flammability;
                q.Reactivity = pm.reactivity;
                q.OtherEquipmentDescription = pm.otherequipmentdescription;
                q.ShlfLife = pm.shlflife;
                q.Booties = pm.booties;
                q.HazardClassGround_SG = pm.hazardclassground_sg;
                q.irritant = pm.irritant;
                q.RighttoKnow = pm.righttoknow;
                q.SARA = pm.sara;
                q.FlammableStorageRoom = pm.flammablestorageroom;
                q.FireList = pm.firelist;
                q.FreezableList = pm.freezablelist;
                q.RefrigeratedList = pm.refrigeratedlist;
                q.MSDSOTHERNUMBER = pm.msdsothernumber;
                q.FREEZERSTORAGE = pm.freezerstorage;
                q.CLIENTREQ = pm.clientreq;
                q.CMCREQ = pm.cmcreq;
                q.RETURNLOCATION = pm.returnlocation;
                q.DENSITY = pm.density;
                q.SpecialBlend = pm.specialblend;
                q.SARA302EHS = pm.sara302ehs;
                q.SARA313 = pm.sara313;
                q.HalfMaskRespirator = pm.halfmaskrespirator;
                q.FullFaceRespirator = pm.fullfacerespirator;
                q.Restrictedamount = pm.restrictedamount;
                q.Torque = pm.torque;
                q.TorqueRequirements = pm.torquerequirements;
                q.OtherPkg = pm.otherpkg;
                q.EECAll = pm.eecall;
                q.RPhrasesAll = pm.rphrasesall;
                q.SPhrasesAll = pm.sphrasesall;
                q.PHAll = pm.phall;
                q.English = pm.english;
                q.German = pm.german;
                q.Dutch = pm.dutch;
                q.EECSymbol1 = pm.eecsymbol1;
                q.EECSymbol2 = pm.eecsymbol2;
                q.EECSymbol3 = pm.eecsymbol3;
                q.Handling = pm.handling;
                q.ShippingNotes = pm.shippingnotes;
                q.OtherLabelNotes = pm.otherlabelnotes;
                q.ProductDescription = pm.productdescription;
                q.PeroxideFormer = pm.peroxideformer;

                q.SpecificGravity = pm.specificgravity;
                q.phValue = pm.phvalue;

                q.PhysicalToxic = pm.physicaltoxic;

                //q.Company_MDB = pm.company_mdb;
                //q.Division_MDB = pm.division_mdb;
                //q.Location_MDB = pm.location_mdb;

                db.SaveChanges();
            }

        }
        
        private int fnNewProductDetailID()
        {
            using (var db = new CMCSQL03Entities())
            {
                var newrecord = new EF.tblProductDetail
                {
                    // dont need to insert any fields, just need the new PK
                };
                db.tblProductDetail.Add(newrecord); db.SaveChanges();
                int newpk = newrecord.ProductDetailID;
                return newpk;
            }
        }


        private int fnNewProductMasterID()
        {
            using (var db = new CMCSQL03Entities())
            {
                var newrecord = new EF.tblProductMaster
                {
                    // dont need to insert any fields, just need the new PK
                };
                db.tblProductMaster.Add(newrecord);
                db.SaveChanges();
                int newpk = newrecord.ProductMasterID;
                return newpk;
            }
        }

        [HttpGet]
        public ActionResult DeActivateProductMaster(int id)
        {
            // just update the field and return 
            using (var db = new EF.CMCSQL03Entities())
            {
                string s = "Update tblProductMaster set Discontinued=1 where ProductMasterID=" + id;
                db.Database.ExecuteSqlCommand(s);
                return Content("Product De-Activated");
            }
        }


        private static void fnArchiveProductMaster(int id)
        {
            // Add logic to copy pm record to tblProductMasterArchive
            // If exists tblPmTemp Drop table tblPMTemp;
            // Select * into  tblPMTemp from tblProductMaster where ProductMasterID=id
            // Insert into tblProductMasterArchive Select * from tblPMTemp;
            // If exists tblPmTemp Drop table tblPMTemp;

        }

        public class dto_01
        {
            // small dto object to schuttle data thru json
            // more complex task should use the full ViewModel object..
            public string key { get; set; }
            public string keyvalue { get; set; }
        }

        public ActionResult FillJsonTest(string testkey)
        {
            var q = new dto_01();
            q.key = 99.ToString();
            q.keyvalue = "phil";
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LookupUNGround(string UN)
        {
            //string test="Group1";
            var q = new dto_01();
            q.key = 99.ToString();
            q.keyvalue = "phil";
            return Json(q, JsonRequestBehavior.AllowGet);
        }
    }
}
