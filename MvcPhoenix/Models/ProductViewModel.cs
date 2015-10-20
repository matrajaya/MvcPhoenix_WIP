using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcPhoenix.Models
{
    public class ProductViewModel
    {
        #region

        [Display(Name = "Client Name")]
        public string ClientName { get; set; }

        [Display(Name = "Business Unit")]
        public string ClientBusUnit { get; set; }

        public IEnumerable<SelectListItem> ClientBusUnitList { get; set; }

        [Display(Name = "Division")]
        public string ClientDivision { get; set; }

        public IEnumerable<SelectListItem> ClientDivisionList { get; set; }

        [Display(Name = "Cross Reference List")]
        public string ProductXRefList { get; set; }

        [Display(Name = "Profile Revision Date")]
        public DateTime ProfileRevDate { get; set; }

        [Display(Name = "Global Product")]
        public bool GlobalProductOpt { get; set; }

        [Display(Name = "Accuracy Verified")]
        public bool AccuracyVerifiedOpt { get; set; }

        [Display(Name = "Discontinued")]
        public bool DiscontinuedOpt { get; set; }

        [Display(Name = "Product Code")]
        public string ProductCode { get; set; }

        [Display(Name = "Product Name")]
        public string ProductName { get; set; }

        [Display(Name = "Equivalence")]
        public string ProductEquivalence { get; set; }

        [Display(Name = "Shelf Life")]
        public string ProductShelfLife { get; set; }

        [Display(Name = "Hazard Class")]
        public string ProductHazardClass { get; set; }

        [Display(Name = "S.G.")]
        public string ProductSG { get; set; }

        [Display(Name = "Density lbs/gal")]
        public string ProductDensity { get; set; }

        [Display(Name = "ph Value")]
        public string ProductPHVal { get; set; }

        [Display(Name = "Package Part Number")]
        public string PackagePartNumber { get; set; }

        [Display(Name = "Restricted Quantity (lbs)")]
        public string ProductRestrictedQty { get; set; }

        [Display(Name = "Global Product")]
        public bool SHGlobalProductOpt { get; set; }

        [Display(Name = "Clean Room GMP")]
        public bool SHCleanRoomGMPOpt { get; set; }

        [Display(Name = "Nitrogen Blanket")]
        public bool SHNitrogenBlanketOpt { get; set; }

        [Display(Name = "Moisture Sensitive")]
        public bool SHMoistureSensitiveOpt { get; set; }

        [Display(Name = "Mix Well")]
        public bool SHMixWellOpt { get; set; }

        [Display(Name = "Don't Pack Above 60% RH")]
        public bool SHNoPackOverRhOpt { get; set; }

        [Display(Name = "Flammable Ground")]
        public bool SHFlammableGroundOpt { get; set; }

        [Display(Name = "Torque")]
        public string SHTorqueInp { get; set; }

        [Display(Name = "Torque Required")]
        public bool SHTorqueOpt { get; set; }

        [Display(Name = "Flash Point")]
        public string SHFlashPointInp { get; set; }

        [Display(Name = "Special Blend")]
        public bool SHSpecialBlendOpt { get; set; }

        [Display(Name = "Heat Prior to Filling")]
        public bool SHHeatFillingOpt { get; set; }

        [Display(Name = "Freezer Storage")]
        public bool SHFreezerStorageOpt { get; set; }

        [Display(Name = "Refridgerate Storage")]
        public bool SHRefridgeStorageOpt { get; set; }

        [Display(Name = "Flammable Storage")]
        public bool SHFlammableStorageOpt { get; set; }

        [Display(Name = "Client")]
        public bool SHReqByClientOpt { get; set; }

        [Display(Name = "CMC")]
        public bool SHReqByCMCOpt { get; set; }

        [Display(Name = "Other Storage")]
        public string SHOtherStorage { get; set; }

        [Display(Name = "Heating Instructions")]
        public string SHHeatingInstructions { get; set; }

        [Display(Name = "Mixing Instructions")]
        public string SHMixingInstructions { get; set; }

        [Display(Name = "Other Instructions")]
        public string SHOtherInstructions { get; set; }

        [Display(Name = "Shipping SOP")]
        public string SHShippingSOP { get; set; }

        [Display(Name = "Normal Appearance")]
        public string SHNormalAppearance { get; set; }

        [Display(Name = "Rejection Criterion")]
        public string SHRejectionCriterion { get; set; }

        [Display(Name = "Right to Know")]
        public bool SHRightToKnowOpt { get; set; }

        [Display(Name = "Fire List")]
        public bool SHFireListOpt { get; set; }

        [Display(Name = "Freezable List")]
        public bool SHFreezeListOpt { get; set; }

        [Display(Name = "SARA")]
        public bool SHSARAOpt { get; set; }

        [Display(Name = "SARA 313")]
        public bool SHSARA313Opt { get; set; }

        [Display(Name = "SARA 302")]
        public bool SHSARA302Opt { get; set; }

        [Display(Name = "EPA Biocide")]
        public bool SHEPABiocideOpt { get; set; }

        [Display(Name = "Hood")]
        public bool HSEquipHoodOpt { get; set; }

        [Display(Name = "Safety Glasses")]
        public bool HSEquipSafetyGlassOpt { get; set; }

        [Display(Name = "Gloves")]
        public bool HSEquipGlovesOpt { get; set; }

        [Display(Name = "Glove Type")]
        public string HSEquipGloveType { get; set; }

        [Display(Name = "Booties")]
        public bool HSEquipBootiesOpt { get; set; }

        [Display(Name = "Clean Room Equip")]
        public bool HSEquipCleanRoomOpt { get; set; }

        [Display(Name = "Apron")]
        public bool HSEquipApronOpt { get; set; }

        [Display(Name = "Arm Sleeves")]
        public bool HSEquipArmSleevesOpt { get; set; }

        [Display(Name = "Half Mask Resp")]
        public bool HSEquipHalfMaskOpt { get; set; }

        [Display(Name = "Full Face Mask Resp")]
        public bool HSEquipFullMaskOpt { get; set; }

        [Display(Name = "Full Suit")]
        public bool HSEquipFullSuitOpt { get; set; }

        [Display(Name = "Other Equipment")]
        public bool HSEquipOtherOpt { get; set; }

        [Display(Name = "Type Other Equipment")]
        public string HSEquipOtherInp { get; set; }

        [Display(Name = "Toxic")]
        public bool HSHealthToxicOpt { get; set; }

        [Display(Name = "Highly Toxic")]
        public bool HSHealthHighToxicOpt { get; set; }

        [Display(Name = "Reproductive Toxin")]
        public bool HSHealthReprodToxinOpt { get; set; }

        [Display(Name = "Corrosive Hazard")]
        public bool HSHealthCorrosiveHzdOpt { get; set; }

        [Display(Name = "Sensitizer")]
        public bool HSHealthSensitizerOpt { get; set; }

        [Display(Name = "Carcinogen")]
        public bool HSHealthCarcinogenOpt { get; set; }

        [Display(Name = "Irritant")]
        public bool HSHealthIrritantOpt { get; set; }

        [Display(Name = "Ingestion")]
        public bool HSEntryIngestionOpt { get; set; }

        [Display(Name = "Inhalation")]
        public bool HSEntryInhalationOpt { get; set; }

        [Display(Name = "Skin Contact")]
        public bool HSEntrySkinOpt { get; set; }

        [Display(Name = "Eye Contact")]
        public bool HSEntryEyeOpt { get; set; }

        [Display(Name = "Combustible")]
        public bool HSPhysCombustibleOpt { get; set; }

        [Display(Name = "Corrosive")]
        public bool HSPhysCorrosiveOpt { get; set; }

        [Display(Name = "Flammable")]
        public bool HSPhysFlammableOpt { get; set; }

        [Display(Name = "Oxidizer")]
        public bool HSPhysOxidizerOpt { get; set; }

        [Display(Name = "Reactive")]
        public bool HSPhysReactiveOpt { get; set; }

        [Display(Name = "Toxic")]
        public bool HSPhysToxicOpt { get; set; }

        [Display(Name = "Peroxide Former")]
        public bool HSPhysPeroxideOpt { get; set; }

        [Display(Name = "Polymerization Hazard")]
        public bool HSPhysPolymerHzdOpt { get; set; }

        [Display(Name = "Health")]
        public string HMISHealth { get; set; }

        [Display(Name = "Flammability")]
        public string HMISFlammability { get; set; }

        [Display(Name = "Reactivity")]
        public string HMISReactivity { get; set; }

        [Display(Name = "Nephrotoxins kidney damage - edema, proteinurea")]
        public bool TOENephrotoxinsOpt { get; set; }

        [Display(Name = "Neurotoxins nervous system damage - narcosis, behaviour change, decrease in motor functions")]
        public bool TOENeurotoxinsOpt { get; set; }

        [Display(Name = "Hermatopoietics blood damage - cyanosis, unconsciousness")]
        public bool TOEHermatopoieticsOpt { get; set; }

        [Display(Name = "Reproductive toxin birth defects and sterility")]
        public bool TOEReproductiveOpt { get; set; }

        [Display(Name = "Hepatotoxins: Liver damage - jaundice, liver enlargement")]
        public bool TOEHepatotoxinsOpt { get; set; }

        [Display(Name = "Pulmonary disfunctions: Lung damage, shortness of breath, chest tightness, cough")]
        public bool TOEPulmonaryOpt { get; set; }

        [Display(Name = "Cutaneous hazards - skin damage, rashes, defatting of skin")]
        public bool TOECutaneousOpt { get; set; }

        [Display(Name = "Eye hazards: Impaired vision, conjunctivitis, corneal damage")]
        public bool TOEEyeHazardOpt { get; set; }

        [Display(Name = "")]
        public bool GHSLabelExcludeOpt { get; set; }

        [Display(Name = "GHS Ready")]
        public bool GHSReadyOpt { get; set; }

        [Display(Name = "GHS Signal Word")]
        public string GHSSignalWord { get; set; }

        [Display(Name = "GHS Symbol 1")]
        public string GHSSymbol1 { get; set; }

        [Display(Name = "GHS Symbol 2")]
        public string GHSSymbol2 { get; set; }

        [Display(Name = "GHS Symbol 3")]
        public string GHSSymbol3 { get; set; }

        [Display(Name = "GHS Symbol 4")]
        public string GHSSymbol4 { get; set; }

        [Display(Name = "MSDS Revision Date")]
        public DateTime MSDSRevDate { get; set; }

        [Display(Name = "MSDS Revision Number")]
        public string MSDSRevNum { get; set; }

        [Display(Name = "MSDS Other Number")]
        public string MSDSOtherNum { get; set; }

        [Display(Name = "SDS Contact Name")]
        public string SDSContactName { get; set; }

        [Display(Name = "SDS Contact Phone")]
        public string SDSContactPhone { get; set; }

        [Display(Name = "Label Revision Date")]
        public DateTime LabelRevDate { get; set; }

        [Display(Name = "Label Number")]
        public string LabelNumber { get; set; }

        [Display(Name = "China Test Certification Date")]
        public string ChinaTestCertDate { get; set; }

        [Display(Name = "Label Contact Name")]
        public string LabelContactName { get; set; }

        [Display(Name = "Label Contact Phone")]
        public string LabelContactPhone { get; set; }

        [Display(Name = "Technical Sheet Attached")]
        public bool TechSheetAttachOpt { get; set; }

        [Display(Name = "Technical Sheet Revision Date")]
        public DateTime TechSheetRevDate { get; set; }

        [Display(Name = "Return Location")]
        public string MSDSReturnLoc { get; set; }

        [Display(Name = "Emergency Contact Number")]
        public string ShipEmergContactNum { get; set; }

        [Display(Name = "End Use")]
        public string ShipEndUse { get; set; }

        [Display(Name = "Harmonized Code")]
        public string ShipHarmonizedCode { get; set; }

        [Display(Name = "Chemical Name")]
        public string ShipChemicalName { get; set; }

        [Display(Name = "Shipping Label Notes")]
        public string ShipLabelNotes { get; set; }

        [Display(Name = "Proper Shipping Name")]
        public string GrdShipName { get; set; }

        [Display(Name = "NOS Name")]
        public string GrdNOSName { get; set; }

        [Display(Name = "Hazard Class")]
        public string GrdHzdClass { get; set; }

        [Display(Name = "Packaging Group")]
        public string GrdPackagingGrp { get; set; }

        [Display(Name = "Sub Class")]
        public string GrdSubClass { get; set; }

        [Display(Name = "UN Number")]
        public string GrdUNNumber { get; set; }

        [Display(Name = "Label Required")]
        public string GrdLabelReqd { get; set; }

        [Display(Name = "Subsidiary Label Required")]
        public string GrdSubLabelReqd { get; set; }

        [Display(Name = "Proper Shipping Name")]
        public string AirShipName { get; set; }

        [Display(Name = "NOS Name")]
        public string AirNOSName { get; set; }

        [Display(Name = "Hazard Class")]
        public string AirHzdClass { get; set; }

        [Display(Name = "Packaging Group")]
        public string AirPackagingGrp { get; set; }

        [Display(Name = "Sub Class")]
        public string AirSubClass { get; set; }

        [Display(Name = "UN Number")]
        public string AirUNNumber { get; set; }

        [Display(Name = "Label Required")]
        public string AirLabelReqd { get; set; }

        [Display(Name = "Subsidiary Label Required")]
        public string AirSubLabelReqd { get; set; }

        [Display(Name = "Proper Shipping Name")]
        public string RCRAShipName { get; set; }

        [Display(Name = "NOS Name")]
        public string RCRANOSName { get; set; }

        [Display(Name = "Hazard Class")]
        public string RCRAHzdClass { get; set; }

        [Display(Name = "Packaging Group")]
        public string RCRAPackagingGrp { get; set; }

        [Display(Name = "Sub Class")]
        public string RCRASubClass { get; set; }

        [Display(Name = "UN Number")]
        public string RCRAUNNumber { get; set; }

        [Display(Name = "Label Required")]
        public string RCRALabelReqd { get; set; }

        [Display(Name = "Subsidiary Label Required")]
        public string RCRASubLabelReqd { get; set; }

        [Display(Name = "Date")]
        public DateTime NotesDate { get; set; }

        [Display(Name = "Product Code")]
        public string NotesProdCode { get; set; }

        [Display(Name = "Notes")]
        public string NotesProdNote { get; set; }

        [Display(Name = "Reason Code")]
        public string NotesReasonCode { get; set; }

        [Display(Name = "CAS Number")]
        public string CASNumber { get; set; }

        [Display(Name = "Chemical Name")]
        public string CASChemicalName { get; set; }

        [Display(Name = "CAS Less Than")]
        public bool CASLessThanOpt { get; set; }

        [Display(Name = "Percentage")]
        public string CASPercentage { get; set; }

        [Display(Name = "Restricted Quantity")]
        public bool CASRestQtyOpt { get; set; }

        [Display(Name = "Restricted Amount")]
        public string CASRestAmt { get; set; }

        [Display(Name = "Pack on Receipt")]
        public bool CASPackRecieptOpt { get; set; }

        [Display(Name = "Reportable Qty")]
        public bool CASReportableQtyOpt { get; set; }

        [Display(Name = "Reportable Amount")]
        public string CASReportableAmount { get; set; }

        [Display(Name = "Exclude From Label")]
        public bool CASExLabelOpt { get; set; }

        [Display(Name = "Waste Code")]
        public string WasteCode { get; set; }

        [Display(Name = "Client")]
        public string WasteClient { get; set; }

        [Display(Name = "Business Unit")]
        public string WasteBusinessUnit { get; set; }

        [Display(Name = "Product Code")]
        public string WasteProductCode { get; set; }

        [Display(Name = "EPA Hazardous Waste")]
        public bool WasteEPAOpt { get; set; }

        [Display(Name = "Non-RCRA Waste")]
        public bool WasteNonRCRAOpt { get; set; }

        [Display(Name = "Waste Profile Number")]
        public string WasteProfileNum { get; set; }

        #endregion
    }
    //---------------------------------------------------------------------------------
}