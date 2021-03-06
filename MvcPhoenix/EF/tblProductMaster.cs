//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MvcPhoenix.EF
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblProductMaster
    {
        public int ProductMasterID { get; set; }
        public Nullable<int> ClientID { get; set; }
        public Nullable<int> SGLegacyID { get; set; }
        public Nullable<int> SDLegacyID { get; set; }
        public string MasterCode { get; set; }
        public string MasterName { get; set; }
        public string SUPPLYID { get; set; }
        public Nullable<bool> Discontinued { get; set; }
        public Nullable<bool> NoReorder { get; set; }
        public Nullable<bool> PackOutOnReceipt { get; set; }
        public Nullable<decimal> RestrictedToAmount { get; set; }
        public string MasterNotes { get; set; }
        public Nullable<bool> MasterNotesAlert { get; set; }
        public Nullable<int> ReOrderAdjustmentDays { get; set; }
        public Nullable<int> CeaseShipDifferential { get; set; }
        public Nullable<bool> CleanRoomGMP { get; set; }
        public Nullable<bool> NitrogenBlanket { get; set; }
        public Nullable<bool> MoistureSensitive { get; set; }
        public Nullable<bool> MixWell { get; set; }
        public string MixingInstructions { get; set; }
        public Nullable<bool> Refrigerate { get; set; }
        public Nullable<bool> DoNotPackAbove60 { get; set; }
        public string HandlingOther { get; set; }
        public Nullable<bool> FlammableGround { get; set; }
        public Nullable<bool> HeatPriorToFilling { get; set; }
        public Nullable<decimal> FlashPoint { get; set; }
        public string HeatingInstructions { get; set; }
        public string OtherHandlingInstr { get; set; }
        public string NormalAppearence { get; set; }
        public string RejectionCriterion { get; set; }
        public Nullable<bool> Hood { get; set; }
        public Nullable<bool> LabHood { get; set; }
        public Nullable<bool> WalkInHood { get; set; }
        public Nullable<bool> SafetyGlasses { get; set; }
        public Nullable<bool> Gloves { get; set; }
        public string GloveType { get; set; }
        public Nullable<bool> Apron { get; set; }
        public Nullable<bool> ArmSleeves { get; set; }
        public Nullable<bool> Respirator { get; set; }
        public Nullable<bool> FaceShield { get; set; }
        public Nullable<bool> FullSuit { get; set; }
        public Nullable<bool> CleanRoomEquipment { get; set; }
        public Nullable<bool> OtherEquipment { get; set; }
        public Nullable<bool> Toxic { get; set; }
        public Nullable<bool> HighlyToxic { get; set; }
        public Nullable<bool> ReproductiveToxin { get; set; }
        public Nullable<bool> CorrosiveHaz { get; set; }
        public Nullable<bool> Sensitizer { get; set; }
        public Nullable<bool> Carcinogen { get; set; }
        public Nullable<bool> Ingestion { get; set; }
        public Nullable<bool> Inhalation { get; set; }
        public Nullable<bool> Skin { get; set; }
        public Nullable<bool> SkinContact { get; set; }
        public Nullable<bool> EyeContact { get; set; }
        public Nullable<bool> Combustible { get; set; }
        public Nullable<bool> Corrosive { get; set; }
        public Nullable<bool> Flammable { get; set; }
        public Nullable<bool> Oxidizer { get; set; }
        public Nullable<bool> Reactive { get; set; }
        public Nullable<bool> Hepatotoxins { get; set; }
        public Nullable<bool> Nephrotoxins { get; set; }
        public Nullable<bool> Neurotoxins { get; set; }
        public Nullable<bool> Hepatopoetics { get; set; }
        public Nullable<bool> PulmonaryDisfunction { get; set; }
        public Nullable<bool> ReporductiveToxin { get; set; }
        public Nullable<bool> CutaneousHazards { get; set; }
        public Nullable<bool> EyeHazards { get; set; }
        public string Health { get; set; }
        public string Flammability { get; set; }
        public string Reactivity { get; set; }
        public string OtherEquipmentDescription { get; set; }
        public Nullable<int> ShelfLife { get; set; }
        public Nullable<bool> Booties { get; set; }
        public string HazardClassGround_SG { get; set; }
        public Nullable<bool> irritant { get; set; }
        public Nullable<bool> RighttoKnow { get; set; }
        public Nullable<bool> SARA { get; set; }
        public Nullable<bool> FlammableStorageRoom { get; set; }
        public Nullable<bool> FireList { get; set; }
        public Nullable<bool> FreezableList { get; set; }
        public Nullable<bool> RefrigeratedList { get; set; }
        public string MSDSOTHERNUMBER { get; set; }
        public Nullable<bool> FREEZERSTORAGE { get; set; }
        public Nullable<bool> CLIENTREQ { get; set; }
        public Nullable<bool> CMCREQ { get; set; }
        public string RETURNLOCATION { get; set; }
        public Nullable<decimal> DENSITY { get; set; }
        public Nullable<bool> SpecialBlend { get; set; }
        public Nullable<bool> SARA302EHS { get; set; }
        public Nullable<bool> SARA313 { get; set; }
        public Nullable<bool> HalfMaskRespirator { get; set; }
        public Nullable<bool> FullFaceRespirator { get; set; }
        public Nullable<bool> Torque { get; set; }
        public string TorqueRequirements { get; set; }
        public string OtherStorage { get; set; }
        public string EECAll { get; set; }
        public string RPhrasesAll { get; set; }
        public string SPhrasesAll { get; set; }
        public string PHAll { get; set; }
        public string English { get; set; }
        public string German { get; set; }
        public string Dutch { get; set; }
        public string EECSymbol1 { get; set; }
        public string EECSymbol2 { get; set; }
        public string EECSymbol3 { get; set; }
        public string Handling { get; set; }
        public string ShippingNotes { get; set; }
        public string OtherLabelNotes { get; set; }
        public string ProductDescription { get; set; }
        public Nullable<bool> PeroxideFormer { get; set; }
        public Nullable<decimal> SpecificGravity { get; set; }
        public Nullable<decimal> phValue { get; set; }
        public Nullable<bool> PhysicalToxic { get; set; }
        public string WasteCode { get; set; }
        public Nullable<System.DateTime> WasteAccumStartDate { get; set; }
        public Nullable<System.DateTime> RCRAReviewDate { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string CreateUser { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string UpdateUser { get; set; }
        public string CountryOfOrigin { get; set; }
        public Nullable<int> LeadTime { get; set; }
        public Nullable<bool> DustFilter { get; set; }
        public Nullable<bool> TemperatureControlledStorage { get; set; }
        public Nullable<bool> PrePacked { get; set; }
        public string AlertNotesReceiving { get; set; }
        public string AlertNotesPackout { get; set; }
        public Nullable<System.DateTime> ProductSetupDate { get; set; }
        public string Company_MDB { get; set; }
        public string Division_MDB { get; set; }
        public string Location_MDB { get; set; }
    }
}
