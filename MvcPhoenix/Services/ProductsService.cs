using MvcPhoenix.EF;
using System;
using System.Linq;
using System.Web;

namespace MvcPhoenix.Models
{
    public class ProductsService
    {
        public static ProductProfile FillFromPM(ProductProfile productProfile)
        {
            using (var db = new CMCSQL03Entities())
            {
                var productMaster = (from t in db.tblProductMaster
                                     where t.ProductMasterID == productProfile.productmasterid
                                     select t).FirstOrDefault();

                var client = (from t in db.tblClient
                              where t.ClientID == productMaster.ClientID
                              select t).FirstOrDefault();

                productProfile.productmasterid = productMaster.ProductMasterID;
                productProfile.clientid = productMaster.ClientID;
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

        public static ProductProfile FillFromPD(ProductProfile productProfile)
        {
            using (var db = new CMCSQL03Entities())
            {
                var productDetail = (from t in db.tblProductDetail
                                     where t.ProductDetailID == productProfile.productdetailid
                                     select t).FirstOrDefault();

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

        public static ProductProfile FillOtherPMProps(ProductProfile productProfile)
        {
            using (var db = new CMCSQL03Entities())
            {
                productProfile.ListOfProductNotes = (from t in db.tblPPPDLogNote
                                                     where t.ProductDetailID == productProfile.productdetailid
                                                     select new ProductNote
                                                     {
                                                         productnoteid = t.PPPDLogNoteID,
                                                         productdetailid = t.ProductDetailID,
                                                         notedate = t.NoteDate,
                                                         notes = t.Notes,
                                                         reasoncode = t.ReasonCode,
                                                         UpdateDate = t.UpdateDate,
                                                         UpdateUser = t.UpdateUser,
                                                         CreateDate = t.CreateDate,
                                                         CreateUser = t.CreateUser
                                                     }).ToList();

                productProfile.ListOfCasNumbers = (from t in db.tblCAS
                                                   where t.ProductDetailID == productProfile.productdetailid
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

                // example of a left outer join with concept of Flattening the join (convert to use a SQL View later)
                var shelfItems = (from t in db.tblShelfMaster
                                  join p in db.tblPackage on t.PackageID equals p.PackageID into temp
                                  from m in temp.DefaultIfEmpty()
                                  where t.ProductDetailID == productProfile.productdetailid
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

                productProfile.ListOfShelfItems = shelfItems;
            }

            return productProfile;
        }

        public static int? GetProductMasterId(int productDetailId)
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

        public static int SaveProductProfile(ProductProfile productProfile)
        {
            int pkProductMaster;
            int pkProductDetail;

            if (productProfile.productmasterid == -1)
            {
                productProfile.productmasterid = NewProductMasterID();

                if (productProfile.productdetailid == -1)
                {
                    productProfile.productdetailid = NewProductDetailID();

                    using (var db = new CMCSQL03Entities())
                    {
                        var productDetailLogNote = new tblPPPDLogNote();

                        productDetailLogNote.ProductDetailID = productProfile.productdetailid;
                        productDetailLogNote.NoteDate = DateTime.UtcNow;
                        productDetailLogNote.Notes = "Master product created";
                        productDetailLogNote.ReasonCode = "New";
                        productDetailLogNote.CreateDate = DateTime.UtcNow;
                        productDetailLogNote.CreateUser = HttpContext.Current.User.Identity.Name;
                        productDetailLogNote.UpdateDate = DateTime.UtcNow;
                        productDetailLogNote.UpdateUser = HttpContext.Current.User.Identity.Name;

                        db.tblPPPDLogNote.Add(productDetailLogNote);
                        db.SaveChanges();
                    }
                }
            }

            SaveProductMaster(productProfile);

            if (productProfile.productdetailid == -1)
            {
                productProfile.productdetailid = NewProductDetailID();
            }

            SaveProductDetail(productProfile);

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

        public static int NewProductDetailID()
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

        public static int NewProductMasterID()
        {
            using (var db = new CMCSQL03Entities())
            {
                var productMaster = new tblProductMaster { };
                productMaster.CreateDate = DateTime.UtcNow;
                productMaster.CreateUser = HttpContext.Current.User.Identity.Name;

                db.tblProductMaster.Add(productMaster);
                db.SaveChanges();

                int newProductMaster = productMaster.ProductMasterID;

                return newProductMaster;
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
            // fill a json object for View use
            using (var db = new CMCSQL03Entities())
            {
                UN un = new UN();

                var getUN = (from t in db.tblUN
                             orderby t.UNID descending
                             where t.UNNumber == unNumber
                             select t).FirstOrDefault();

                un.unid = getUN.UNID;
                un.unnumber = getUN.UNNumber;
                un.hazardclass = getUN.HazardClass;
                un.propershippingname = getUN.ProperShippingName;
                un.nosname = getUN.NOSName;
                un.labelreq = getUN.LabelReq;
                un.subclass = getUN.SubClass;
                un.subsidlabelreq = getUN.SubSidLabelReq;
                un.packinggroup = getUN.PackingGroup;

                return un;
            }
        }

        #region ProductNotes Methods

        public static ProductNote CreateProductNote(int productDetailId)
        {
            ProductNote productNote = new ProductNote();

            using (var db = new CMCSQL03Entities())
            {
                productNote.productnoteid = -1;
                productNote.productdetailid = productDetailId;  // important
                productNote.reasoncode = null;
                productNote.notedate = DateTime.UtcNow;
                productNote.notes = null;
                productNote.UpdateDate = DateTime.UtcNow;
                productNote.UpdateUser = HttpContext.Current.User.Identity.Name;
                productNote.CreateDate = DateTime.UtcNow;
                productNote.CreateUser = HttpContext.Current.User.Identity.Name;
            }

            return productNote;
        }

        public static ProductNote GetProductNote(int productDetailLogNoteId)
        {
            ProductNote productNote = new ProductNote();

            using (var db = new CMCSQL03Entities())
            {
                var getProductNote = (from t in db.tblPPPDLogNote
                                      where t.PPPDLogNoteID == productDetailLogNoteId
                                      select t).FirstOrDefault();

                productNote.productnoteid = getProductNote.PPPDLogNoteID;
                productNote.productdetailid = getProductNote.ProductDetailID;
                productNote.reasoncode = getProductNote.ReasonCode;
                productNote.notedate = getProductNote.NoteDate;
                productNote.notes = getProductNote.Notes;
                productNote.UpdateDate = getProductNote.UpdateDate;
                productNote.UpdateUser = getProductNote.UpdateUser;
                productNote.CreateDate = getProductNote.CreateDate;
                productNote.CreateUser = getProductNote.CreateUser;
            }

            return productNote;
        }

        public static int SaveProductNote(ProductNote productNote)
        {
            using (var db = new CMCSQL03Entities())
            {
                if (productNote.productnoteid == -1)
                {
                    var newProductDetailLogNote = new tblPPPDLogNote();
                    newProductDetailLogNote.CreateDate = DateTime.UtcNow;
                    newProductDetailLogNote.CreateUser = HttpContext.Current.User.Identity.Name;

                    db.tblPPPDLogNote.Add(newProductDetailLogNote);
                    db.SaveChanges();

                    productNote.productnoteid = newProductDetailLogNote.PPPDLogNoteID;
                }

                var productDetailLogNote = (from t in db.tblPPPDLogNote
                                            where t.PPPDLogNoteID == productNote.productnoteid
                                            select t).FirstOrDefault();

                productDetailLogNote.ProductDetailID = productNote.productdetailid;
                productDetailLogNote.NoteDate = productNote.notedate;
                productDetailLogNote.Notes = productNote.notes;
                productDetailLogNote.ReasonCode = productNote.reasoncode;
                productDetailLogNote.UpdateDate = DateTime.UtcNow;
                productDetailLogNote.UpdateUser = HttpContext.Current.User.Identity.Name;

                db.SaveChanges();

                return productDetailLogNote.PPPDLogNoteID;
            }
        }

        public static int DeleteProductNote(int productDetailLogNoteId)
        {
            using (var db = new CMCSQL03Entities())
            {
                db.Database.ExecuteSqlCommand("DELETE FROM tblPPPDLogNote WHERE PPPDLogNoteID=" + productDetailLogNoteId);
            }

            return productDetailLogNoteId;
        }

        #endregion ProductNotes Methods

        #region CAS Methods

        public static Cas CreateCAS(int productDetailId)
        {
            Cas cas = new Cas();
            cas.casid = -1;
            cas.productdetailid = productDetailId;

            return cas;
        }

        public static Cas GetCAS(int casId)
        {
            Cas cas = new Cas();

            using (var db = new CMCSQL03Entities())
            {
                var getCAS = (from t in db.tblCAS
                              where t.CASID == casId
                              select t).FirstOrDefault();

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

                return cas;
            }
        }

        public static int SaveCAS(Cas cas)
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

                var CAS = (from t in db.tblCAS
                           where t.CASID == cas.casid
                           select t).FirstOrDefault();

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

                return CAS.CASID;
            }
        }

        public static int DeleteCAS(int casId)
        {
            using (var db = new CMCSQL03Entities())
            {
                db.Database.ExecuteSqlCommand("DELETE FROM tblCAS WHERE CASID=" + casId);
            }

            return casId;
        }

        #endregion CAS Methods

        #region Client Product Cross Reference Methods

        public static ClientProductXRef GetClientProductCrossReference(int productXReferenceId)
        {
            ClientProductXRef clientXReference = new ClientProductXRef();

            using (var db = new CMCSQL03Entities())
            {
                var productXReference = (from t in db.tblProductXRef
                                         where t.ProductXRefID == productXReferenceId
                                         select t).FirstOrDefault();

                clientXReference.ProductXRefID = productXReference.ProductXRefID;
                clientXReference.ClientID = productXReference.ClientID;
                clientXReference.CMCProductCode = productXReference.CMCProductCode;
                clientXReference.CMCSize = productXReference.CMCSize;
                clientXReference.ClientProductCode = productXReference.CustProductCode;
                clientXReference.ClientProductName = productXReference.CustProductName;
                clientXReference.ClientSize = productXReference.CustSize;

                return clientXReference;
            }
        }

        public static int SaveClientProductCrossReference(ClientProductXRef clientProductXRef)
        {
            using (var db = new CMCSQL03Entities())
            {
                if (clientProductXRef.ProductXRefID == -1)
                {
                    var newProductXReference = new tblProductXRef();
                    db.tblProductXRef.Add(newProductXReference);
                    db.SaveChanges();

                    clientProductXRef.ProductXRefID = newProductXReference.ProductXRefID;
                }

                var productXReference = (from t in db.tblProductXRef
                                         where t.ProductXRefID == clientProductXRef.ProductXRefID
                                         select t).FirstOrDefault();

                productXReference.ClientID = clientProductXRef.ClientID;
                productXReference.CMCProductCode = clientProductXRef.CMCProductCode;
                productXReference.CMCSize = clientProductXRef.CMCSize;
                productXReference.CustProductCode = clientProductXRef.ClientProductCode;
                productXReference.CustSize = clientProductXRef.ClientSize;
                productXReference.CustProductName = clientProductXRef.ClientProductName;

                db.SaveChanges();

                return productXReference.ProductXRefID;
            }
        }

        public static int DeleteProductCrossReference(int productXReferenceId)
        {
            using (var db = new CMCSQL03Entities())
            {
                db.Database.ExecuteSqlCommand("DELETE FROM tblProductXRef WHERE ProductXRefID=" + productXReferenceId);
            }

            return productXReferenceId;
        }

        #endregion Client Product Cross Reference Methods
    }
}