using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

//pc add
using MvcPhoenix.Models;
using MvcPhoenix.EF;

namespace MvcPhoenix.Controllers
{
    public class ProductsController : Controller
    {
        // GET: Products/
        // Build a ProductProfileLanding model and return the view
        public ActionResult Index()
        {
            return View("~/Views/Products/Index.cshtml");
        }

        // GET: Products/Edit
        public ActionResult Edit()
        {
            return View();
        }

        // called from javascript function in Index to return a string as a DD
        public string ProductCodesDropDown(int id, string divid)
        {
            string s = "";
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            var qry = (from t in db.tblProductDetail
                       join t2 in db.tblProductMaster on t.ProductMasterID equals t2.ProductMasterID
                       where t2.ClientID == id
                       orderby t.ProductCode
                       select new { t.ProductDetailID, t.ProductCode, t.ProductName, t2.MasterCode });

            // IMPORTANT: set the id and the name 
            s = "<select name='" + divid + "' id='" + divid + "' class='form-control'>";
            s = s + "<option value='0' selected=true>Product Code</option>";
            foreach (var item in qry)
            {
                s = s + "<option value=" + item.ProductDetailID.ToString() + ">" + item.ProductCode + " - " + item.MasterCode + " - " + item.ProductName + "</option>";
            }
            s = s + "</select>";
            db.Dispose();
            return s;
        }


        [HttpPost]
        public ActionResult SetUpProductProfileEdit(int productdetailid1)
        {
            if (productdetailid1 == 0)
            {
                return View("~/Views/Products/Index.cshtml"); // cycle back
            }
            else
            {
                ProductProfile PP = new ProductProfile();
                PP.productdetailid = productdetailid1;
                PP = FillFromPD(PP);
                PP = FillFromPM(PP);
                PP = fnFillOtherPMProps(PP);
                return View("~/Views/Products/ProductProfileEdit.cshtml", PP);
            }
        }


        [HttpPost]
        public ActionResult SetUpProductProfileNew(int clientid2)
        {
            if (clientid2 == 0)
            {
                return View("~/Views/Products/Index.cshtml"); // cycle back
            }
            else
            {
                ProductProfile PP = new ProductProfile();
                PP.clientid = clientid2;
                PP.productmasterid = -1;
                PP.productdetailid = -1;
                // no reason to go to DB - this is a new object
                PP = fnFillOtherPMProps(PP);
                return View("~/Views/Products/ProductProfileEdit.cshtml", PP);
            }
        }


        [HttpPost]
        public ActionResult SetUpProductProfileEquiv(int productdetailid3)
        {
            if (productdetailid3 == 0)
            {
                return View("~/Views/Products/Index.cshtml"); // cycle back
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
                    // NO  PP = FillPD(PP);
                    PP = fnFillOtherPMProps(PP);
                    return View("~/Views/Products/ProductProfileEdit.cshtml", PP);
                }

            }
        }

        // ***********************************************************************
        // Controller Support Actions Below
        // ***********************************************************************

        private ProductProfile fnFillOtherPMProps(ProductProfile PP)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                // Add equivalents
                var qry = (from t in db.tblProductDetail where t.ProductMasterID == PP.productmasterid select new { t.ProductCode }).Distinct().ToList();
                var mylist1 = new List<string>();
                foreach (var item in qry)
                { mylist1.Add(item.ProductCode); }
                PP.ListOfEquivalents = mylist1;

                var q = (from t in db.tblClient where t.ClientID == PP.clientid select new { t.ClientName, t.LogoFileName }).FirstOrDefault();
                PP.clientname = q.ClientName;
                PP.logofilename = "http://www.mysamplecenter.com/Logos/" + q.LogoFileName;

                List<SelectListItem> mylist2 = new List<SelectListItem>();
                mylist2 = (from t in db.tblDivision where t.ClientID == PP.clientid select new SelectListItem { Value = t.DivisionID.ToString(), Text = t.Division }).ToList();
                mylist2.Insert(0, new SelectListItem { Value = "0", Text = "" });
                PP.ListOfDivisions = mylist2;

                List<SelectListItem> mylist3 = new List<SelectListItem>();
                mylist3 = (from t in db.tblBulkSupplier where t.ClientID == PP.clientid select new SelectListItem { Value = t.SupplyID, Text = t.SupplyID }).ToList();
                mylist3.Insert(0, new SelectListItem { Value = "0", Text = "" });
                PP.ListOfSupplyIDs = mylist3;

                // build ListOfShelfItems
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
                //PP.checked = qd.Checked;
                PP.checkedby = qd.CheckedBy;
                PP.checkedwhen = qd.CheckedWhen;
                PP.epabiocide = qd.EPABiocide;
                PP.labelinfo = qd.LabelInfo;
                PP.ghsready = qd.GHSReady;
                PP.customsvalue = qd.CustomsValue;
                PP.customsvalueunit = qd.CustomsValueUnit;
                PP.globalproduct = qd.GlobalProduct;

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
                PP.productmasterid = q.ProductMasterID;
                PP.clientid = q.ClientID;
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
                PP.other = q.Other;
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
                PP.grnshipname = q.GRNSHIPNAME;
                PP.grnshipnamed = q.GRNSHIPNAMED;
                PP.grnosname = q.GRNOSNAME;
                PP.grnunnumber = q.GRNUNNUMBER;
                PP.grnhazcl = q.GRNHAZCL;
                PP.grnpkgrp = q.GRNPKGRP;
                PP.grnlabel = q.GRNLABEL;
                PP.grnsublabel = q.GRNSUBLABEL;
                PP.grntremacard = q.GRNTREMACARD;
                PP.airshipname = q.AIRSHIPNAME;
                PP.airnosname = q.AIRNOSNAME;
                PP.airunnumber = q.AIRUNNUMBER;
                PP.airhazcl = q.AIRHAZCL;
                PP.airpkgrp = q.AIRPKGRP;
                PP.airlabel = q.AIRLABEL;
                PP.airsublabel = q.AIRSUBLABEL;
                PP.airhazsubcl = q.AIRHAZSUBCL;
                PP.grnhazsubcl = q.GRNHAZSUBCL;
                PP.seashipname = q.SEASHIPNAME;
                PP.seashipnamed = q.SEASHIPNAMED;
                PP.seanosname = q.SEANOSNAME;
                PP.seaunnum = q.SEAUNNUM;
                PP.seapkgrp = q.SEAPKGRP;
                PP.seahazcl = q.SEAHAZCL;
                PP.sealabel = q.SEALABEL;
                PP.seahazsubcl = q.SEAHAZSUBCL;
                PP.seasublabel = q.SEASUBLABEL;
                PP.seahazmat = q.SEAHAZMAT;
                PP.seaemsno = q.SEAEMSNO;
                PP.seamfagno = q.SEAMFAGNO;

                return PP;
            }
        }


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
                return Content("Profiled Update At " + DateTime.Now.ToString());
            }
            catch
            {
                return Content("DB Error - Profiled NOT UPDATED At " + DateTime.Now.ToString());
            }

        }


        // ************** Save to DB **********************************************

        public void SaveProductDetail(ProductProfile PP)
        {
            using (var db = new CMCSQL03Entities())
            {
                //fnArchiveProductMaster(pm.productmasterid);
                var q = (from t in db.tblProductDetail where t.ProductDetailID == PP.productdetailid select t).FirstOrDefault();
                q.ProductCode = PP.productcode;
                q.ProductDetailID = PP.productdetailid;
                q.ProductMasterID = PP.productmasterid;
                
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
                
                db.SaveChanges();
            }
        }

        public void SaveProductMaster(ProductProfile pm)
        {
            using (var db = new CMCSQL03Entities())
            {
                //fnArchiveProductMaster(pm.productmasterid);
                var q = (from t in db.tblProductMaster where t.ProductMasterID == pm.productmasterid select t).FirstOrDefault();

                q.ClientID = pm.clientid;
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
                q.Other = pm.other;
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
                q.GRNSHIPNAME = pm.grnshipname;
                q.GRNSHIPNAMED = pm.grnshipnamed;
                q.GRNOSNAME = pm.grnosname;
                q.GRNUNNUMBER = pm.grnunnumber;
                q.GRNHAZCL = pm.grnhazcl;
                q.GRNPKGRP = pm.grnpkgrp;
                q.GRNLABEL = pm.grnlabel;
                q.GRNSUBLABEL = pm.grnsublabel;
                q.GRNTREMACARD = pm.grntremacard;
                q.AIRSHIPNAME = pm.airshipname;
                q.AIRNOSNAME = pm.airnosname;
                q.AIRUNNUMBER = pm.airunnumber;
                q.AIRHAZCL = pm.airhazcl;
                q.AIRPKGRP = pm.airpkgrp;
                q.AIRLABEL = pm.airlabel;
                q.AIRSUBLABEL = pm.airsublabel;
                q.AIRHAZSUBCL = pm.airhazsubcl;
                q.GRNHAZSUBCL = pm.grnhazsubcl;
                q.SEASHIPNAME = pm.seashipname;
                q.SEASHIPNAMED = pm.seashipnamed;
                q.SEANOSNAME = pm.seanosname;
                q.SEAUNNUM = pm.seaunnum;
                q.SEAPKGRP = pm.seapkgrp;
                q.SEAHAZCL = pm.seahazcl;
                q.SEALABEL = pm.sealabel;
                q.SEAHAZSUBCL = pm.seahazsubcl;
                q.SEASUBLABEL = pm.seasublabel;
                q.SEAHAZMAT = pm.seahazmat;
                q.SEAEMSNO = pm.seaemsno;
                q.SEAMFAGNO = pm.seamfagno;

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

    }
}
