using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

// ************** ProductsService.cs ********************
// This class contains Product Profile service methods
// ******************************************************

namespace MvcPhoenix.Models
{
    public class ProductsService
    {

        public static string fnProductCodesDropDown(int id, string divid)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                var qry = (from t in db.tblProductDetail
                           join t2 in db.tblProductMaster on t.ProductMasterID equals t2.ProductMasterID
                           where t2.ClientID == id
                           orderby t.ProductCode
                           select new { t.ProductDetailID, t.ProductCode, t.ProductName, t2.MasterCode });
                string sb = "";
                //sb = sb + string.Format("<select name='{0}' id='{1}' class='{2}'  >", divid, divid, "form-control"); - Iffy
                sb = sb + string.Format("<select name='{0}' id='{1}' class='{2}' onchange='getid(this)' >", divid, divid, "form-control");

                sb = sb + string.Format("<option value='{0}' selected=true >{1}</option>", "0", "Product Code");
                foreach (var item in qry)
                {
                    sb = sb + string.Format("<option value={0}> {1} - {2} - {3} </option>", item.ProductDetailID.ToString(), item.ProductCode, item.MasterCode, item.ProductName);
                }
                sb = sb + "</select>";
                //return HttpUtility.HtmlEncode(sb);
                return sb;
            }
        }

        public static ProductProfile fnFillOtherPMProps(ProductProfile PP)
        {

            using (var db = new EF.CMCSQL03Entities())
            {

                //PackagePartNumbers
                PP.ListOfPackagePartNumbers = (from t in db.tblPackage orderby t.Size select new SelectListItem { Value = t.PackageID.ToString(), Text = t.Size + " " + t.PartNumber + " " + t.Description }).ToList();
                PP.ListOfPackagePartNumbers.Insert(0, new SelectListItem { Value = "0", Text = "" });

                //GloveType - maybe move to a tblGlove??
                List<SelectListItem> gloves = new List<SelectListItem>();
                gloves.Add(new SelectListItem { Value = "", Text = "" });
                gloves.Add(new SelectListItem { Value = "GMP NITRILE", Text = "GMP NITRILE" });
                gloves.Add(new SelectListItem { Value = "NEOPRENE", Text = "NEOPRENE" });
                gloves.Add(new SelectListItem { Value = "NEOPRENE+NITRIL", Text = "NEOPRENE+NITRIL" });
                gloves.Add(new SelectListItem { Value = "NITRILE", Text = "NITRILE" });
                PP.ListOfGloves = gloves;


                PP.ListOfEquivalents = (from t in db.tblProductDetail where t.ProductMasterID == PP.productmasterid && t.ProductCode != PP.productcode select new SelectListItem { Value = t.ProductCode, Text = t.ProductCode }).ToList();

                PP.ListOfEndUsesForCustoms = (from t in db.tblEndUseForCustoms orderby t.EndUse select new SelectListItem { Value = t.EndUse, Text = t.EndUse }).ToList();
                PP.ListOfEndUsesForCustoms.Insert(0, new SelectListItem { Value = "0", Text = "" });

                PP.ListOfHarmonizedCodes = (from t in db.tblHSCode orderby t.HarmonizedCode select new SelectListItem { Value = t.HarmonizedCode, Text = t.HarmonizedCode }).ToList();
                PP.ListOfHarmonizedCodes.Insert(0, new SelectListItem { Value = "0", Text = "" });

                PP.ListOfProductCodesXRefs = (from t in db.tblProductXRef where t.CMCProductCode == PP.mastercode select new SelectListItem { Value = t.ProductXRefID.ToString(), Text = t.CustProductCode }).ToList();

                // Logo filename (needs to be moved to a Client class)
                var q = (from t in db.tblClient where t.ClientID == PP.clientid select new { t.ClientName, t.LogoFileName }).FirstOrDefault();
                PP.clientname = q.ClientName;
                PP.logofilename = "http://www.mysamplecenter.com/Logos/" + q.LogoFileName;

                PP.ListOfDivisions = (from t in db.tblDivision where t.ClientID == PP.clientid select new SelectListItem { Value = t.DivisionID.ToString(), Text = t.Division }).ToList();
                PP.ListOfDivisions.Insert(0, (new SelectListItem { Value = "0", Text = "" }));

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

                // example of a left outer join with concept of Flattening the join (convert to use a SQL View later)
                var qshelf = (from t in db.tblShelfMaster
                              join p in db.tblPackage on t.PackageID equals p.PackageID into temp
                              from m in temp.DefaultIfEmpty()
                              where t.ProductDetailID == PP.productdetailid
                              select new ShelfMaster
                              {
                                  shelfid = t.ShelfID,
                                  warehouse = t.Warehouse,
                                  size = t.Size,
                                  bin = t.Bin,
                                  packageid = t.PackageID,
                                  packagepartnumber = m.PartNumber,
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

        public static ProductProfile FillFromPD(ProductProfile PP)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                var qd = (from t in db.tblProductDetail where t.ProductDetailID == PP.productdetailid select t).FirstOrDefault();
                PP.productdetailid = qd.ProductDetailID;
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
                PP.polymerizationhazard = qd.PolymerizationHazard;
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

                // fields add by Iffy 10/28 (already in Master ???)
                PP.shippingchemicalname = qd.ShippingChemicalName;
                PP.labelnotesepa = qd.LabelNotesEPA;


                // Eight standard ground fields pulled from tblUN
                PP.grnunnumber = qd.GRNUNNUMBER;
                PP.grnpkgrp = qd.GRNPKGRP;
                PP.grnhazsubcl = qd.GRNHAZSUBCL;
                PP.grnlabel = qd.GRNLABEL;
                PP.grnsublabel = qd.GRNSUBLABEL;
                PP.grnhazcl = qd.GRNHAZCL;
                PP.grnshipname = qd.GRNSHIPNAME;
                PP.grnosname = qd.GRNOSNAME;
                // Additional ground fields
                PP.grnshipnamed = qd.GRNSHIPNAMED;
                PP.grntremacard = qd.GRNTREMACARD;

                // Eight standard air fields pulled from tblUN
                PP.airunnumber = qd.AIRUNNUMBER;
                PP.airpkgrp = qd.AIRPKGRP;
                PP.airhazsubcl = qd.AIRHAZSUBCL;
                PP.airlabel = qd.AIRLABEL;
                PP.airsublabel = qd.AIRSUBLABEL;
                PP.airhazcl = qd.AIRHAZCL;
                PP.airshipname = qd.AIRSHIPNAME;
                PP.airnosname = qd.AIRNOSNAME;

                // Eight standard sea fields pulled from tblUN
                PP.seaunnum = qd.SEAUNNUM;
                PP.seapkgrp = qd.SEAPKGRP;
                PP.seahazsubcl = qd.SEAHAZSUBCL;
                PP.sealabel = qd.SEALABEL;
                PP.seasublabel = qd.SEASUBLABEL;
                PP.seahazcl = qd.SEAHAZCL;
                PP.seashipname = qd.SEASHIPNAME;
                PP.seanosname = qd.SEANOSNAME;
                // Additional sea fields
                PP.seashipnamed = qd.SEASHIPNAMED;
                PP.seahazmat = qd.SEAHAZMAT;
                PP.seaemsno = qd.SEAEMSNO;
                PP.seamfagno = qd.SEAMFAGNO;

                PP.detaillastupdate = qd.LastUpDate;
                return PP;
            }
        }

        public static ProductProfile FillFromPM(ProductProfile PP)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                var q = (from t in db.tblProductMaster where t.ProductMasterID == PP.productmasterid select t).FirstOrDefault();
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

                PP.masterlastupdate = q.LastUpDate;
                PP.physicaltoxic = q.PhysicalToxic;
                PP.wastecode = q.WasteCode;
                //PP.company_mdb = q.Company_MDB;
                //PP.division_mdb = q.Division_MDB;
                //PP.location_mdb = q.Location_MDB;

                return PP;
            }
        }

        public static int fnProductMasterID(int id)
        {
            // return MasterID of a Detail
            using (var db = new EF.CMCSQL03Entities())
            {
                var q = (from t in db.tblProductDetail where t.ProductDetailID == id select new { t.ProductMasterID }).FirstOrDefault();
                return Convert.ToInt32(q.ProductMasterID);
            }


        }

        public static bool fnSaveProductProfile(ProductProfile PPVM)
        {
            try
            {
                ProductsService.SaveProductMaster(PPVM);
                ProductsService.SaveProductDetail(PPVM);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void SaveProductDetail(ProductProfile PP)
        {
            using (var db = new EF.CMCSQL03Entities())
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
                q.PolymerizationHazard = PP.polymerizationhazard;
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

                // fields add by Iffy 10/28 (already in Master ???)
                q.ShippingChemicalName = PP.shippingchemicalname;
                q.LabelNotesEPA = PP.labelnotesepa;


                q.GRNUNNUMBER = PP.grnunnumber;
                q.GRNPKGRP = PP.grnpkgrp;
                q.GRNHAZSUBCL = PP.grnhazsubcl;
                q.GRNLABEL = PP.grnlabel;
                q.GRNSUBLABEL = PP.grnsublabel;
                q.GRNHAZCL = PP.grnhazcl;
                q.GRNSHIPNAME = PP.grnshipname;
                q.GRNOSNAME = PP.grnosname;
                q.GRNSHIPNAMED = PP.grnshipnamed;
                q.GRNTREMACARD = PP.grntremacard;

                q.AIRUNNUMBER = PP.airunnumber;
                q.AIRPKGRP = PP.airpkgrp;
                q.AIRHAZSUBCL = PP.airhazsubcl;
                q.AIRLABEL = PP.airlabel;
                q.AIRSUBLABEL = PP.airsublabel;
                q.AIRHAZCL = PP.airhazcl;
                q.AIRSHIPNAME = PP.airshipname;
                q.AIRNOSNAME = PP.airnosname;


                q.SEAUNNUM = PP.seaunnum;
                q.SEAPKGRP = PP.seapkgrp;
                q.SEAHAZSUBCL = PP.seahazsubcl;
                q.SEALABEL = PP.sealabel;
                q.SEASUBLABEL = PP.seasublabel;
                q.SEAHAZCL = PP.seahazcl;
                q.SEASHIPNAME = PP.seashipname;
                q.SEANOSNAME = PP.seanosname;
                q.SEASHIPNAMED = PP.seashipnamed;
                q.SEAHAZMAT = PP.seahazmat;
                q.SEAEMSNO = PP.seaemsno;
                q.SEAMFAGNO = PP.seamfagno;

                q.LastUpDate = DateTime.Now;

                //q.Company_MDB = pd.company_mdb;
                //q.MasterCode_MDB = pd.mastercode_mdb;
                //q.Division_MDB = pd.division_mdb;
                db.SaveChanges();

            }


        }

        public static void SaveProductMaster(ProductProfile pm)
        {
            using (var db = new EF.CMCSQL03Entities())
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

                q.LastUpDate = DateTime.Now;

                q.PhysicalToxic = pm.physicaltoxic;

                q.WasteCode = pm.wastecode;
                //q.Company_MDB = pm.company_mdb;
                //q.Division_MDB = pm.division_mdb;
                //q.Location_MDB = pm.location_mdb;
                db.SaveChanges();
            }

        }

        public static int fnNewProductDetailID()
        {
            using (var db = new EF.CMCSQL03Entities())
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

        private static void fnArchiveProductMaster(int id)
        {
            // Add logic to copy pm record to tblProductMasterArchive
            // If exists tblPmTemp Drop table tblPMTemp;
            // Select * into  tblPMTemp from tblProductMaster where ProductMasterID=id
            // Insert into tblProductMasterArchive Select * from tblPMTemp;
            // If exists tblPmTemp Drop table tblPMTemp;

        }

        public static int fnNewProductMasterID()
        {
            using (var db = new EF.CMCSQL03Entities())
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

        public static void fnDeActivateProductMaster(int id)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                string s = "Update tblProductMaster set Discontinued=1 where ProductMasterID=" + id;
                db.Database.ExecuteSqlCommand(s);
            }
        }

        public static UN fnGetUN(string id)
        {
            // fill a json object for View use
            using (var db = new EF.CMCSQL03Entities())
            {
                UN obj = new UN();
                var q = (from t in db.tblUN orderby t.UNID descending where t.UNNumber == id select t).FirstOrDefault();
                obj.unid = q.UNID;
                obj.unnumber = q.UNNumber;
                obj.hazardclass = q.HazardClass;
                obj.propershippingname = q.ProperShippingName;
                obj.nosname = q.NOSName;
                obj.labelreq = q.LabelReq;
                obj.subclass = q.SubClass;
                obj.subsidlabelreq = q.SubSidLabelReq;
                obj.packinggroup = q.PackingGroup;
                return obj;
            }
        }


        // -----------------------------------------------
        #region ProductNotes Methods
        public static ProductNote fnCreateProductNote(int id)
        {
            ProductNote PN = new ProductNote();
            using (var db = new EF.CMCSQL03Entities())
            {
                PN.productnoteid = -1;
                PN.productdetailid = id;  // important 
                PN.reasoncode = null;
                PN.notedate = DateTime.Now;
                PN.notes = null;
                PN.ListOfReasonCodes = (from t in db.tblReasonCode orderby t.Reason select new SelectListItem { Value = t.Reason, Text = t.Reason }).ToList();
                PN.ListOfReasonCodes.Insert(0, new SelectListItem { Value = "", Text = "Select Reason Code" });
            }
            return PN;
        }

        public static ProductNote fnGetProductNote(int id)
        {
            ProductNote PN = new ProductNote();
            using (var db = new EF.CMCSQL03Entities())
            {
                var q = (from t in db.tblProductNotes where t.ProductNoteID == id select t).FirstOrDefault();
                PN.productnoteid = q.ProductNoteID;
                PN.productdetailid = q.ProductDetailID;
                PN.reasoncode = q.ReasonCode;
                PN.notedate = q.NoteDate;
                PN.notes = q.Notes;
                PN.ListOfReasonCodes = (from t in db.tblReasonCode orderby t.Reason select new SelectListItem { Value = t.Reason, Text = t.Reason }).ToList();
                PN.ListOfReasonCodes.Insert(0, new SelectListItem { Value = "", Text = "Select Reason Code" });
            }
            return PN;
        }

        public static int fnSaveProductNoteToDB(ProductNote PN)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                if (PN.productnoteid == -1)
                {
                    var newrec = new EF.tblProductNotes();
                    db.tblProductNotes.Add(newrec);
                    db.SaveChanges();
                    PN.productnoteid = newrec.ProductNoteID;
                }
                var q = (from t in db.tblProductNotes where t.ProductNoteID == PN.productnoteid select t).FirstOrDefault();
                q.ProductDetailID = PN.productdetailid;
                q.NoteDate = PN.notedate;
                q.Notes = PN.notes;
                q.ReasonCode = PN.reasoncode;
                db.SaveChanges();
                return q.ProductNoteID;
            }
        }

        public static int fnDeleteProductNote(int id)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                db.Database.ExecuteSqlCommand("Delete from tblProductNotes Where ProductNoteID=" + id);
            }
            return id;
        }
        #endregion
        // -----------------------------------------------


        // -----------------------------------------------
        #region CAS Methods
        public static Cas fnCreateCAS(int id)
        {
            Cas CS = new Cas();
            CS.casid = -1;
            CS.productdetailid = id;
            return CS;
        }

        public static Cas fnGetCAS(int id)
        {
            Cas CS = new Cas();
            using (var db = new EF.CMCSQL03Entities())
            {
                var q = (from t in db.tblCAS where t.CASID == id select t).FirstOrDefault();
                CS.casid = q.CASID;
                CS.productdetailid = q.ProductDetailID;
                CS.casnumber = q.CasNumber;
                CS.chemicalname = q.ChemicalName;
                CS.percentage = q.Percentage;
                CS.restrictedqty = q.RestrictedQty;
                CS.restrictedamount = q.RestrictedAmount;
                CS.packonreceipt = q.PackOnReceipt;
                CS.reportableqty = q.ReportableQty;
                CS.reportableamount = q.ReportableAmount;
                CS.lessthan = q.LessThan;
                CS.excludefromlabel = q.ExcludeFromLabel;
                return CS;
            }
        }

        public static int fnSaveCASToDB(Cas CS)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                if (CS.casid == -1)
                {
                    var newrec = new EF.tblCAS();
                    db.tblCAS.Add(newrec);
                    db.SaveChanges();
                    CS.casid = newrec.CASID;
                }
                var q = (from t in db.tblCAS where t.CASID == CS.casid select t).FirstOrDefault();
                q.ProductDetailID = CS.productdetailid;
                q.CasNumber = CS.casnumber;
                q.ChemicalName = CS.chemicalname;
                q.Percentage = CS.percentage;
                q.RestrictedQty = CS.restrictedqty;
                q.RestrictedAmount = CS.restrictedamount;
                q.PackOnReceipt = CS.packonreceipt;
                q.ReportableQty = CS.reportableqty;
                q.ReportableAmount = CS.reportableamount;
                q.LessThan = CS.lessthan;
                q.ExcludeFromLabel = CS.excludefromlabel;
                db.SaveChanges();
                return q.CASID;
            }
        }
                       
        public static int fnDeleteCAS(int id)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                db.Database.ExecuteSqlCommand("Delete from tblCAS Where CASID=" + id);
            }
            return id;
        }

        #endregion
        // -----------------------------------------------

    }
}

