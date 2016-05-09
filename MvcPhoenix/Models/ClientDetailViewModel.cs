using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MvcPhoenix.Models
{
    public class ClientDetailViewModel
    {
        public int ClientID { get; set; }
        public string ClientName { get; set; }
        public string LogoFileName { get; set; }
        public string WarehouseLocation { get; set; }
        public string ClientContactInfo { get; set; }
        public string CMCContactInfo { get; set; }
        public string ClientWebsite { get; set; }
        public string ClientCurrency { get; set; }
        public string ProductSetupDetails { get; set; }
        public string SDSLanguage { get; set; }
        public string LabelLanguage { get; set; }
        public bool DSCoa { get; set; }
        public bool DSTds { get; set; }
        public bool DSStandardLetter { get; set; }
        public string InventoryReports { get; set; }

        public string CeaseShipmentsPriorExpiry { get; set; }
        public string ReplenishmentLeadtime { get; set; }
        public bool ExpiryDateOnSampleLabel { get; set; }
        public string InterpretationExpiryDate { get; set; }
        public string WasteProcedure { get; set; }
        public string InventoryAdditionalInstructions { get; set; } //external object
        public string InventoryChecklist { get; set; } //external object
        public string SpecialRequestsAllowed { get; set; }
        public string ValueOncommercialInvoice { get; set; }
        public string FreezableProcedure { get; set; }
        public string BusinessRulesCS { get; set; } //external object
        public string EmailConfirmations { get; set; }
        public bool WebTool { get; set; }
        public string InvoicingBasis { get; set; }
        
        //Document & Handling Charges
        public string DHC_InternationalTruckLTLNonHaz { get; set; }
        public string DHC_InternationalTruckLTLHaz { get; set; }
        public string DHC_DomesticAirHaz { get; set; }
        public string DHC_DomesticAirNonHaz { get; set; }
        public string DHC_GuaranteedSameDayRushOrders { get; set; }

        //Freight Surcharges
        public string FS_DHLInternationalHaz { get; set; }
        public string FS_UPSDomesticGroundHaz { get; set; }
        public string FS_UPSDomesticAirHaz { get; set; }
        public string FS_UPSInternationalAirHaz { get; set; }
        public string FS_UPSInternationalGroundHaz { get; set; }
        public string FS_FedexDomesticGroundHaz { get; set; }
        public string FS_FedexDomesticAirHaz { get; set; }
        public string FS_FedexInternationalHaz { get; set; }

        //Miscellaneous Charges
        public string MscChg_PoisonPak { get; set; }


        /////////////////////////////////////////////////////////////////////
        
        public int LegacyID { get; set; }
        public int GlobalClientID { get; set; }
        public string ClientCode { get; set; }
        
        public string CMCLocation { get; set; }
        public string CustomerReference { get; set; }
        public string CMCLongCustomer { get; set; }
        
        public string ClientUM { get; set; }
        public bool MSDS { get; set; }
        public bool TDS { get; set; }
        public bool COA { get; set; }
        
        public string MSDS_Folder { get; set; }
        public string TDS_Folder { get; set; }
        public string COA_Folder { get; set; }
        public string BOLComment { get; set; }
        
        public string InvoiceAddress { get; set; }
        public string InvoiceEmailTo { get; set; }
        public string KeyContactList { get; set; }
        public string MainContactClient { get; set; }
        public string MainContactsCMC { get; set; }
        public string ClientURL { get; set; }
        
        
        public string SDSUpdateMethod { get; set; }
        public bool SDSRequired { get; set; }
        public bool COARequired { get; set; }
        public bool TDSRequired { get; set; }
        public string CoverLetterRequired { get; set; }
        public string InvReports { get; set; }
        public string CeaseShipMonths { get; set; }
        public string ReplenLeadDays { get; set; }
        public bool ExpDateOnLabel { get; set; }
        public string ExpDateRules { get; set; }
        
        public string InvBusinessRules { get; set; }
        public string InvChecklist { get; set; }
        public string SpecReqAllowed { get; set; }
        public string CommValue { get; set; }
        
        public bool SurveyUsed { get; set; }
        public string CSBusinessRules { get; set; }
        public string ShipConfEmail { get; set; }
        public bool InvoiceSegregation { get; set; }
        public string ChargesSummary { get; set; }
        public string PartialDelivAllowed { get; set; }
        public string ClientStatus { get; set; }
        public DateTime ActiveDate { get; set; }
    }
}