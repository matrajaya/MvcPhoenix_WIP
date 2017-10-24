using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MvcPhoenix.Models
{
    public class ProductProfileLanding
    {
        public int searchclientid { get; set; }
        public int searchproductdetailid { get; set; }
    }

    public class UN
    {
        public int unid { get; set; }
        public string unnumber { get; set; }
        public string hazardclass { get; set; }
        public string propershippingname { get; set; }
        public string nosname { get; set; }
        public string labelreq { get; set; }
        public string subclass { get; set; }
        public string subsidlabelreq { get; set; }
        public string packinggroup { get; set; }
    }

    public class ProductNote
    {
        public int ProductNoteId { get; set; }
        public int? ProductDetailId { get; set; }
        public DateTime? NoteDate { get; set; }
        public string Notes { get; set; }
        public string ReasonCode { get; set; }
        public decimal? Charge { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdateUser { get; set; }
    }

    public class Cas
    {
        public int casid { get; set; }
        public int? productdetailid { get; set; }
        public string casnumber { get; set; }
        public string chemicalname { get; set; }
        public string percentage { get; set; }
        public bool? restrictedqty { get; set; }
        public decimal? restrictedamount { get; set; }
        public bool? reportableqty { get; set; }
        public decimal? reportableamount { get; set; }
        public bool? lessthan { get; set; }
        public bool? excludefromlabel { get; set; }
    }

    public class ClientProductXRef
    {
        public int ProductXRefID { get; set; }
        public int? ClientID { get; set; }
        public string ClientName { get; set; }
        public int? ProductID { get; set; }
        public string ProductName { get; set; }
        public string CMCProductCode { get; set; }
        public string CMCSize { get; set; }
        public string ClientProductCode { get; set; }
        public string ClientProductName { get; set; }
        public string ClientSize { get; set; }
    }

    public class ProductProfile
    {
        public ProductProfile()
        {
            active = true;                                      // pc: move the list items here and cleanup controller code
        }

        public List<ProductNote> ListOfProductNotes { get; set; }
        public List<Cas> ListOfCasNumbers { get; set; }
        public List<ShelfMasterViewModel> ListOfShelfItems { get; set; }

        public string logofilename { get; set; }
        public string alertnotesshipping { get; set; }
        public string alertnotesreceiving { get; set; }
        public string alertnotespackout { get; set; }
        public string alertnotesorderentry { get; set; }
        public string countryoforigin { get; set; }
        public string accuracyverifiedby { get; set; }
        public int? leadtime { get; set; }
        public bool? dustfilter { get; set; }
        public string rcraunnumber { get; set; }
        public string rcrapkgrp { get; set; }
        public string rcrahazsubcl { get; set; }
        public string rcralabel { get; set; }
        public string rcrasublabel { get; set; }
        public string rcrahazcl { get; set; }
        public string rcrashipname { get; set; }
        public string rcranosname { get; set; }
        [Required]
        public int productdetailid { get; set; }
        [Required]
        public int? divisionid { get; set; }
        [Required]
        public string productcode { get; set; }
        [Required]
        public string productname { get; set; }
        public string custcode { get; set; }
        public bool? multilotreq { get; set; }
        public bool? extendableexpdt { get; set; }
        public string harmonizedcode { get; set; }
        public string enduse { get; set; }
        public DateTime? sgrevisiondate { get; set; }
        public DateTime? msdsrevisiondate { get; set; }
        public string msdsrevisionnumber { get; set; }
        public DateTime? labelrevisiondate { get; set; }
        public string labelnumber { get; set; }
        public bool? productchecked { get; set; }
        public string checkedby { get; set; }
        public DateTime? checkedwhen { get; set; }
        public bool? epabiocide { get; set; }
        public string labelinfo { get; set; }
        public bool? ghsready { get; set; }
        public decimal? customsvalue { get; set; }
        public string customsvalueunit { get; set; }
        public bool? globalproduct { get; set; }
        public bool? polymerizationhazard { get; set; }
        public bool? accuracyverified { get; set; }
        public string sdscontactname { get; set; }
        public string sdscontactphone { get; set; }
        public DateTime? chinacertificationdate { get; set; }
        public string labelcontactname { get; set; }
        public string labelcontactphone { get; set; }
        public bool? technicalsheet { get; set; }
        public DateTime? technicalsheetrevisondate { get; set; }
        public string emergencycontactnumber { get; set; }
        public bool? epahazardouswaste { get; set; }
        public bool? nonrcrawaste { get; set; }
        public string wasteprofilenumber { get; set; }
        public string shippingchemicalname { get; set; }
        public string labelnotesepa { get; set; }
        public bool? active { get; set; }
        public DateTime? activedate { get; set; }
        [Required]
        public int? productmasterid { get; set; }
        [Required]
        public int clientid { get; set; }
        public string clientname { get; set; }
        public bool? discontinued { get; set; }
        [Required]
        public string mastercode { get; set; }
        [Required]
        public string mastername { get; set; }
        public int? shelflife { get; set; }
        public decimal? density { get; set; }
        public string supplyid { get; set; }
        public bool? noreorder { get; set; }
        public decimal? restrictedtoamount { get; set; }
        public DateTime? createdate { get; set; }
        public string masternotes { get; set; }
        public bool? masternotesalert { get; set; }
        public int? reorderadjustmentdays { get; set; }
        public int? ceaseshipdifferential { get; set; }
        public bool? cleanroomgmp { get; set; }
        public bool? nitrogenblanket { get; set; }
        public bool? moisturesensitive { get; set; }
        public bool? mixwell { get; set; }
        public string mixinginstructions { get; set; }
        public bool? refrigerate { get; set; }
        public bool? donotpackabove60 { get; set; }
        public string handlingother { get; set; }
        public bool? flammableground { get; set; }
        public bool? heatpriortofilling { get; set; }
        public decimal? flashpoint { get; set; }
        public string heatinginstructions { get; set; }
        public string otherhandlinginstr { get; set; }
        public string normalappearence { get; set; }
        public string rejectioncriterion { get; set; }
        public bool? hood { get; set; }
        public bool? labhood { get; set; }
        public bool? walkinhood { get; set; }
        public bool? safetyglasses { get; set; }
        public bool? gloves { get; set; }
        public string glovetype { get; set; }
        public bool? apron { get; set; }
        public bool? armsleeves { get; set; }
        public bool? respirator { get; set; }
        public bool? faceshield { get; set; }
        public bool? fullsuit { get; set; }
        public bool? cleanroomequipment { get; set; }
        public bool? otherequipment { get; set; }
        public bool? toxic { get; set; }
        public bool? highlytoxic { get; set; }
        public bool? reproductivetoxin { get; set; }
        public bool? corrosivehaz { get; set; }
        public bool? sensitizer { get; set; }
        public bool? carcinogen { get; set; }
        public bool? ingestion { get; set; }
        public bool? inhalation { get; set; }
        public bool? skin { get; set; }
        public bool? skincontact { get; set; }
        public bool? eyecontact { get; set; }
        public bool? combustible { get; set; }
        public bool? corrosive { get; set; }
        public bool? flammable { get; set; }
        public bool? oxidizer { get; set; }
        public bool? reactive { get; set; }
        public bool? hepatotoxins { get; set; }
        public bool? nephrotoxins { get; set; }
        public bool? neurotoxins { get; set; }
        public bool? hepatopoetics { get; set; }
        public bool? pulmonarydisfunction { get; set; }
        public bool? reporductivetoxin { get; set; }
        public bool? cutaneoushazards { get; set; }
        public bool? eyehazards { get; set; }
        public string health { get; set; }
        public string flammability { get; set; }
        public string reactivity { get; set; }
        public string otherequipmentdescription { get; set; }
        public bool? booties { get; set; }
        public string hazardclassground_sg { get; set; }
        public bool? irritant { get; set; }
        public bool? righttoknow { get; set; }
        public bool? sara { get; set; }
        public bool? flammablestorageroom { get; set; }
        public bool? firelist { get; set; }
        public bool? freezablelist { get; set; }
        public string msdsothernumber { get; set; }
        public bool? freezerstorage { get; set; }
        public bool? clientreq { get; set; }
        public bool? cmcreq { get; set; }
        public string returnlocation { get; set; }
        public bool? specialblend { get; set; }
        public bool? sara302ehs { get; set; }
        public bool? sara313 { get; set; }
        public bool? halfmaskrespirator { get; set; }
        public bool? fullfacerespirator { get; set; }
        public bool? torque { get; set; }
        public string torquerequirements { get; set; }
        public string otherstorage { get; set; }
        public string eecall { get; set; }
        public string rphrasesall { get; set; }
        public string sphrasesall { get; set; }
        public string phall { get; set; }
        public string english { get; set; }
        public string german { get; set; }
        public string dutch { get; set; }
        public string eecsymbol1 { get; set; }
        public string eecsymbol2 { get; set; }
        public string eecsymbol3 { get; set; }
        public string handling { get; set; }
        public string shippingnotes { get; set; }
        public string otherlabelnotes { get; set; }
        public string productdescription { get; set; }
        public bool? peroxideformer { get; set; }
        public string grnunnumber { get; set; }
        public string grnpkgrp { get; set; }
        public string grnhazsubcl { get; set; }
        public string grnlabel { get; set; }
        public string grnsublabel { get; set; }
        public string grnhazcl { get; set; }
        public string grnshipname { get; set; }
        public string grnosname { get; set; }
        public string grnshipnamed { get; set; }
        public string grntremacard { get; set; }
        public string airunnumber { get; set; }
        public string airpkgrp { get; set; }
        public string airhazsubcl { get; set; }
        public string airlabel { get; set; }
        public string airsublabel { get; set; }
        public string airhazcl { get; set; }
        public string airshipname { get; set; }
        public string airnosname { get; set; }
        public string seaunnum { get; set; }
        public string seapkgrp { get; set; }
        public string seahazsubcl { get; set; }
        public string sealabel { get; set; }
        public string seasublabel { get; set; }
        public string seahazcl { get; set; }
        public string seashipname { get; set; }
        public string seanosname { get; set; }
        public string seashipnamed { get; set; }
        public string seahazmat { get; set; }
        public string seaemsno { get; set; }
        public string seamfagno { get; set; }
        public decimal? specificgravity { get; set; }
        public decimal? phvalue { get; set; }
        public bool? physicaltoxic { get; set; }
        public string wastecode { get; set; }
        public bool? temperaturecontrolledstorage { get; set; }
        public bool? prepacked { get; set; }
        public DateTime? rcrareviewdate { get; set; }
        public DateTime? wasteaccumstartdate { get; set; }
        public DateTime? productsetupdate { get; set; }
        public DateTime? CreateDateMaster { get; set; }
        public string CreateUserMaster { get; set; }
        public DateTime? UpdateDateMaster { get; set; }
        public string UpdateUserMaster { get; set; }
        public DateTime? CreateDateDetail { get; set; }
        public string CreateUserDetail { get; set; }
        public DateTime? UpdateDateDetail { get; set; }
        public string UpdateUserDetail { get; set; }
    }
}