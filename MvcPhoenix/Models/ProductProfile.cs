using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


// pc add
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MvcPhoenix.Models
{

    public class ProductProfileLanding
    {
        // small class used by the landing page, built in the index view load
        public int searchclientid { get; set; }

        [Display(Name = "ClientID")]
        public List<SelectListItem> ListOfClients { get; set; }

        public int searchproductdetailid { get; set; }
    }

    public class ProductNote
    {
        public int productnoteid { get; set; }
        public int? productdetailid { get; set; }
        public DateTime? notedate { get; set; }
        public string notes { get; set; }
        public string reasoncode { get; set; }
    }

    public class UN
    {
        public string unnumber { get; set; }
        public string hazardclass { get; set; }
        public string propershippingname { get; set; }
        public string nosname { get; set; }
        public string labelreq { get; set; }
        public string subclass { get; set; }
        public string subsidlabelreq { get; set; }
        public string packinggroup { get; set; }
    }


    public class Cas
    {
        [Display(Name = "CASID")]
        public int casid { get; set; }

        [Display(Name = "ProductDetailID")]
        public int? productdetailid { get; set; }

        [Display(Name = "CasNumber")]
        public string casnumber { get; set; }

        [Display(Name = "ChemicalName")]
        public string chemicalname { get; set; }

        [Display(Name = "Percentage")]
        public string percentage { get; set; }

        [Display(Name = "RestrictedQty")]
        public bool? restrictedqty { get; set; }

        [Display(Name = "RestrictedAmount")]
        public decimal? restrictedamount { get; set; }

        [Display(Name = "PackOnReceipt")]
        public bool? packonreceipt { get; set; }

        [Display(Name = "ReportableQty")]
        public bool? reportableqty { get; set; }

        [Display(Name = "ReportableAmount")]
        public decimal? reportableamount { get; set; }

        [Display(Name = "LessThan")]
        public bool? lessthan { get; set; }

        [Display(Name = "ExcludeFromLabel")]
        public bool? excludefromlabel { get; set; }

    }

    public class ProductProfile
    {
        public List<SelectListItem> ListOfPackageTypes { get; set; }
        public List<ProductNote> ListOfProductNotes { get; set; }
        public List<Cas> ListOfCasNumbers { get; set; }

        public DateTime masterlastupdate { get; set; }
        public DateTime detaillastupdate { get; set; }

        // **********************************************************************
        // tblProductDetail Fields
        // **********************************************************************

        [Display(Name = "ProductDetailID")]
        public int productdetailid { get; set; }

        // tblShelfMaster records
        public List<ShelfMaster> ListOfShelfItems { get; set; }

        [Display(Name = "DivisionID")]
        public int? divisionid { get; set; }

        [Display(Name = "BusArea")]
        public string busarea { get; set; }

        [Display(Name = "ProductCode")]
        public string productcode { get; set; }

        [Display(Name = "ProductName")]
        public string productname { get; set; }

        [Display(Name = "CustCode")]
        public string custcode { get; set; }

        [Display(Name = "MultiLotReq")]
        public bool? multilotreq { get; set; }

        [Display(Name = "ExtendableExpDt")]
        public bool? extendableexpdt { get; set; }

        [Display(Name = "HarmonizedCode")]
        public string harmonizedcode { get; set; }

        public List<SelectListItem> ListOfHarmonizedCodes { get; set; }

        [Display(Name = "EndUse")]
        public string enduse { get; set; }

        public List<SelectListItem> ListOfEndUsesForCustoms { get; set; }


        [Display(Name = "SGRevisionDate")]
        public DateTime? sgrevisiondate { get; set; }

        [Display(Name = "MSDS Revision Date")]
        public DateTime? msdsrevisiondate { get; set; }

        [Display(Name = "MSDS Revision Number")]
        public string msdsrevisionnumber { get; set; }

        [Display(Name = "Label Revision Date")]
        public DateTime? labelrevisiondate { get; set; }

        [Display(Name = "Label Number")]
        public string labelnumber { get; set; }

        [Display(Name = "Product Checked")]
        // table field in JET is named [checked], changed to productchecked for SQL Server
        public bool? productchecked { get; set; }

        [Display(Name = "Checked By")]
        public string checkedby { get; set; }

        [Display(Name = "Checked When")]
        public DateTime? checkedwhen { get; set; }

        [Display(Name = "EPA Biocide")]
        public bool? epabiocide { get; set; }

        [Display(Name = "Label Info")]
        public string labelinfo { get; set; }

        [Display(Name = "GHS Ready")]
        public bool? ghsready { get; set; }

        [Display(Name = "Customs Value")]
        public decimal? customsvalue { get; set; }

        [Display(Name = "Customs Value Unit")]
        public string customsvalueunit { get; set; }

        [Display(Name = "Global Product")]
        public bool? globalproduct { get; set; }

        //pc 10/23/2015 new fields created by Iffy

        public bool? polmerizationhazard { get; set; }

        [Display(Name = "Accuracy Verified")]
        public bool? accuracyverified { get; set; }

        [Display(Name = "SDS Contact Name")]
        public string sdscontactname { get; set; }

        [Display(Name = "SDS Contact Phone")]
        public string sdscontactphone { get; set; }

        [Display(Name = "China Certification Date")]
        public DateTime? chinacertificationdate { get; set; }

        [Display(Name = "Label Contact Name")]
        public string labelcontactname { get; set; }

        [Display(Name = "Label Contact Phone")]
        public string labelcontactphone { get; set; }

        [Display(Name = "Technical Sheet")]
        public bool? technicalsheet { get; set; }

        [Display(Name = "Tech Sheet Rev Date")]
        public DateTime? technicalsheetrevisondate { get; set; }

        [Display(Name = "Emergency Contact Number")]
        public string emergencycontactnumber { get; set; }

        [Display(Name = "EPA Hazardous Waste")]
        public bool? epahazardouswaste { get; set; }

        [Display(Name = "Non RCRA Waste")]
        public bool? nonrcrawaste { get; set; }

        [Display(Name = "Waste Profile Number")]
        public string wasteprofilenumber { get; set; }

        // **********************************************************************
        // tblProductMaster Fields
        // **********************************************************************

        public string logofilename { get; set; }

        [Display(Name = "Equivalents")]
        public List<SelectListItem> ListOfEquivalents { get; set; }

        public List<SelectListItem> ListOfProductCodesXRefs { get; set; }

        [Display(Name = "ProductMasterID")]
        public int? productmasterid { get; set; }

        //[Display(Name = "ClientID")]
        public int? clientid { get; set; }

        [Display(Name = "ClientID")]
        public List<SelectListItem> ListOfClients { get; set; }

        [Display(Name = "Client")]
        public string clientname { get; set; }

        // Panel Product Identification
        //[Display(Name = "MasterDivisionID")]
        public int? masterdivisionid { get; set; }

        [Display(Name = "Business Unit/Division")]
        //public IEnumerable<SelectListItem> ListOfDivisions { get; set; }
        public List<SelectListItem> ListOfDivisions { get; set; }

        [Display(Name = "Discontinued")]
        public bool? discontinued { get; set; }

        [Display(Name = "MasterCode")]
        public string mastercode { get; set; }

        [Display(Name = "MasterName")]
        public string mastername { get; set; }

        [Display(Name = "ShlfLife")]
        public int? shlflife { get; set; }

        [Display(Name = "DENSITY")]
        public decimal? density { get; set; }

        [Display(Name = "Restrictedamount")]
        public decimal? restrictedamount { get; set; }
        
        // *********************************************************

        //[Display(Name = "SUPPLYID")]
        public string supplyid { get; set; }

        [Display(Name = "SUPPLYID")]
        public List<SelectListItem> ListOfSupplyIDs { get; set; }

        [Display(Name = "NoReorder")]
        public bool? noreorder { get; set; }

        [Display(Name = "PackOutOnReceipt")]
        public bool? packoutonreceipt { get; set; }

        [Display(Name = "RestrictedToAmount")]
        public decimal? restrictedtoamount { get; set; }

        [Display(Name = "CreateDate")]
        public DateTime? createdate { get; set; }

        [Display(Name = "MasterNotes")]
        public string masternotes { get; set; }

        [Display(Name = "MasterNotesAlert")]
        public bool? masternotesalert { get; set; }

        [Display(Name = "ReOrderAdjustmentDays")]
        public int? reorderadjustmentdays { get; set; }

        [Display(Name = "CeaseShipDifferential")]
        public int? ceaseshipdifferential { get; set; }

        [Display(Name = "CleanRoomGMP")]
        public bool? cleanroomgmp { get; set; }

        [Display(Name = "NitrogenBlanket")]
        public bool? nitrogenblanket { get; set; }

        [Display(Name = "MoistureSensitive")]
        public bool? moisturesensitive { get; set; }

        [Display(Name = "MixWell")]
        public bool? mixwell { get; set; }

        [Display(Name = "MixingInstructions")]
        public string mixinginstructions { get; set; }

        [Display(Name = "Refrigerate")]
        public bool? refrigerate { get; set; }

        [Display(Name = "DoNotPackAbove60")]
        public bool? donotpackabove60 { get; set; }

        [Display(Name = "HandlingOther")]
        public string handlingother { get; set; }

        [Display(Name = "FlammableGround")]
        public bool? flammableground { get; set; }

        [Display(Name = "HeatPriorToFilling")]
        public bool? heatpriortofilling { get; set; }

        [Display(Name = "FlashPoint")]
        public decimal? flashpoint { get; set; }

        [Display(Name = "HeatingInstructions")]
        public string heatinginstructions { get; set; }

        // 10/24/2015 per cd rename in migration
        //[Display(Name = "Other")]
        //public string other { get; set; }
        [Display(Name = "Other Handling Instr")]
        public string otherhandlinginstr { get; set; }

        [Display(Name = "NormalAppearence")]
        public string normalappearence { get; set; }

        [Display(Name = "RejectionCriterion")]
        public string rejectioncriterion { get; set; }

        [Display(Name = "Hood")]
        public bool? hood { get; set; }

        [Display(Name = "LabHood")]
        public bool? labhood { get; set; }

        [Display(Name = "WalkInHood")]
        public bool? walkinhood { get; set; }

        [Display(Name = "SafetyGlasses")]
        public bool? safetyglasses { get; set; }

        [Display(Name = "Gloves")]
        public bool? gloves { get; set; }

        public List<SelectListItem> ListOfGloves { get; set; }

        [Display(Name = "GloveType")]
        public string glovetype { get; set; }

        [Display(Name = "Apron")]
        public bool? apron { get; set; }

        [Display(Name = "ArmSleeves")]
        public bool? armsleeves { get; set; }

        [Display(Name = "Respirator")]
        public bool? respirator { get; set; }

        [Display(Name = "FaceShield")]
        public bool? faceshield { get; set; }

        [Display(Name = "FullSuit")]
        public bool? fullsuit { get; set; }

        [Display(Name = "CleanRoomEquipment")]
        public bool? cleanroomequipment { get; set; }

        [Display(Name = "OtherEquipment")]
        public bool? otherequipment { get; set; }

        [Display(Name = "Toxic")]
        public bool? toxic { get; set; }

        [Display(Name = "HighlyToxic")]
        public bool? highlytoxic { get; set; }

        [Display(Name = "ReproductiveToxin")]
        public bool? reproductivetoxin { get; set; }

        [Display(Name = "CorrosiveHaz")]
        public bool? corrosivehaz { get; set; }

        [Display(Name = "Sensitizer")]
        public bool? sensitizer { get; set; }

        [Display(Name = "Carcinogen")]
        public bool? carcinogen { get; set; }

        [Display(Name = "Ingestion")]
        public bool? ingestion { get; set; }

        [Display(Name = "Inhalation")]
        public bool? inhalation { get; set; }

        [Display(Name = "Skin")]
        public bool? skin { get; set; }

        [Display(Name = "SkinContact")]
        public bool? skincontact { get; set; }

        [Display(Name = "EyeContact")]
        public bool? eyecontact { get; set; }

        [Display(Name = "Combustible")]
        public bool? combustible { get; set; }

        [Display(Name = "Corrosive")]
        public bool? corrosive { get; set; }

        [Display(Name = "Flammable")]
        public bool? flammable { get; set; }

        [Display(Name = "Oxidizer")]
        public bool? oxidizer { get; set; }

        [Display(Name = "Reactive")]
        public bool? reactive { get; set; }

        [Display(Name = "Hepatotoxins")]
        public bool? hepatotoxins { get; set; }

        [Display(Name = "Nephrotoxins")]
        public bool? nephrotoxins { get; set; }

        [Display(Name = "Neurotoxins")]
        public bool? neurotoxins { get; set; }

        [Display(Name = "Hepatopoetics")]
        public bool? hepatopoetics { get; set; }

        [Display(Name = "PulmonaryDisfunction")]
        public bool? pulmonarydisfunction { get; set; }

        [Display(Name = "ReporductiveToxin")]
        public bool? reporductivetoxin { get; set; }

        [Display(Name = "CutaneousHazards")]
        public bool? cutaneoushazards { get; set; }

        [Display(Name = "EyeHazards")]
        public bool? eyehazards { get; set; }

        [Display(Name = "Health")]
        public string health { get; set; }

        [Display(Name = "Flammability")]
        public string flammability { get; set; }

        [Display(Name = "Reactivity")]
        public string reactivity { get; set; }

        [Display(Name = "OtherEquipmentDescription")]
        public string otherequipmentdescription { get; set; }

        [Display(Name = "Booties")]
        public bool? booties { get; set; }

        [Display(Name = "HazardClassGround_SG")]
        public string hazardclassground_sg { get; set; }

        [Display(Name = "irritant")]
        public bool? irritant { get; set; }

        [Display(Name = "RighttoKnow")]
        public bool? righttoknow { get; set; }

        [Display(Name = "SARA")]
        public bool? sara { get; set; }

        [Display(Name = "FlammableStorageRoom")]
        public bool? flammablestorageroom { get; set; }

        [Display(Name = "FireList")]
        public bool? firelist { get; set; }

        [Display(Name = "FreezableList")]
        public bool? freezablelist { get; set; }

        [Display(Name = "RefrigeratedList")]
        public bool? refrigeratedlist { get; set; }

        [Display(Name = "MSDSOTHERNUMBER")]
        public string msdsothernumber { get; set; }

        [Display(Name = "FREEZERSTORAGE")]
        public bool? freezerstorage { get; set; }

        [Display(Name = "CLIENTREQ")]
        public bool? clientreq { get; set; }

        [Display(Name = "CMCREQ")]
        public bool? cmcreq { get; set; }

        [Display(Name = "RETURNLOCATION")]
        public string returnlocation { get; set; }

        [Display(Name = "SpecialBlend")]
        public bool? specialblend { get; set; }

        [Display(Name = "SARA302EHS")]
        public bool? sara302ehs { get; set; }

        [Display(Name = "SARA313")]
        public bool? sara313 { get; set; }

        [Display(Name = "HalfMaskRespirator")]
        public bool? halfmaskrespirator { get; set; }

        [Display(Name = "FullFaceRespirator")]
        public bool? fullfacerespirator { get; set; }

        [Display(Name = "Torque")]
        public bool? torque { get; set; }

        [Display(Name = "TorqueRequirements")]
        public string torquerequirements { get; set; }

        [Display(Name = "OtherPkg")]
        public string otherpkg { get; set; }

        [Display(Name = "EECAll")]
        public string eecall { get; set; }

        [Display(Name = "RPhrasesAll")]
        public string rphrasesall { get; set; }

        [Display(Name = "SPhrasesAll")]
        public string sphrasesall { get; set; }

        [Display(Name = "PHAll")]
        public string phall { get; set; }

        [Display(Name = "English")]
        public string english { get; set; }

        [Display(Name = "German")]
        public string german { get; set; }

        [Display(Name = "Dutch")]
        public string dutch { get; set; }

        [Display(Name = "EECSymbol1")]
        public string eecsymbol1 { get; set; }

        [Display(Name = "EECSymbol2")]
        public string eecsymbol2 { get; set; }

        [Display(Name = "EECSymbol3")]
        public string eecsymbol3 { get; set; }

        [Display(Name = "Handling")]
        public string handling { get; set; }

        [Display(Name = "ShippingNotes")]
        public string shippingnotes { get; set; }

        [Display(Name = "OtherLabelNotes")]
        public string otherlabelnotes { get; set; }

        [Display(Name = "ProductDescription")]
        public string productdescription { get; set; }

        [Display(Name = "PeroxideFormer")]
        public bool? peroxideformer { get; set; }

        [Display(Name = "GRNSHIPNAME")]
        public string grnshipname { get; set; }

        [Display(Name = "GRNSHIPNAMED")]
        public string grnshipnamed { get; set; }

        [Display(Name = "GRNOSNAME")]
        public string grnosname { get; set; }

        [Display(Name = "GRNUNNUMBER")]
        public string grnunnumber { get; set; }

        [Display(Name = "GRNHAZCL")]
        public string grnhazcl { get; set; }

        [Display(Name = "GRNPKGRP")]
        public string grnpkgrp { get; set; }

        [Display(Name = "GRNLABEL")]
        public string grnlabel { get; set; }

        [Display(Name = "GRNSUBLABEL")]
        public string grnsublabel { get; set; }

        [Display(Name = "GRNTREMACARD")]
        public string grntremacard { get; set; }

        [Display(Name = "AIRSHIPNAME")]
        public string airshipname { get; set; }

        [Display(Name = "AIRNOSNAME")]
        public string airnosname { get; set; }

        [Display(Name = "AIRUNNUMBER")]
        public string airunnumber { get; set; }

        [Display(Name = "AIRHAZCL")]
        public string airhazcl { get; set; }

        [Display(Name = "AIRPKGRP")]
        public string airpkgrp { get; set; }

        [Display(Name = "AIRLABEL")]
        public string airlabel { get; set; }

        [Display(Name = "AIRSUBLABEL")]
        public string airsublabel { get; set; }

        [Display(Name = "AIRHAZSUBCL")]
        public string airhazsubcl { get; set; }

        [Display(Name = "GRNHAZSUBCL")]
        public string grnhazsubcl { get; set; }

        [Display(Name = "SEASHIPNAME")]
        public string seashipname { get; set; }

        [Display(Name = "SEASHIPNAMED")]
        public string seashipnamed { get; set; }

        [Display(Name = "SEANOSNAME")]
        public string seanosname { get; set; }

        [Display(Name = "SEAUNNUM")]
        public string seaunnum { get; set; }

        [Display(Name = "SEAPKGRP")]
        public string seapkgrp { get; set; }

        [Display(Name = "SEAHAZCL")]
        public string seahazcl { get; set; }

        [Display(Name = "SEALABEL")]
        public string sealabel { get; set; }

        [Display(Name = "SEAHAZSUBCL")]
        public string seahazsubcl { get; set; }

        [Display(Name = "SEASUBLABEL")]
        public string seasublabel { get; set; }

        [Display(Name = "SEAHAZMAT")]
        public string seahazmat { get; set; }

        [Display(Name = "SEAEMSNO")]
        public string seaemsno { get; set; }

        [Display(Name = "SEAMFAGNO")]
        public string seamfagno { get; set; }

        [Display(Name = "S.G.")]
        public decimal? specificgravity { get; set; }

        [Display(Name = "pH")]
        public decimal? phvalue { get; set; }
    }
}