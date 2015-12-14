TODOs: Missing Features

Orders
 - Allocation
 - Packout Orders
 - Queue system		- Prerequisite User Management
 - Extract service functions from _TransEditModal.cshtml

Order Import
 - SSIS packages 	- activate listener on web upload folder
			- Re-certify data written to orders table

Client Management
 - Entire module
 - Req - Model design, data assimilation from intranet site
 - 

Invoice
 - Entire module
 - Req Prerequisites - Pricing tier tables, surchage, flp, etc, db table design/model

Products
 - Product Identification
	- Add Shelf Size Information
 - GHS Information
	- TBD
	- Redesign and streamline current tables
 - MSDS Information
	- SDS PDF View/Upload (Basic Document Management)
 - Shipping Description
	- UN Number search and populate needs exception handled
 - Miscellaneous
	- Notes
	- CAS

Inventory
 - Entire module. Basic Functions View and Edit Stock levels

Replenishments
 - Send Confirmation Email feature
 - Order Item Edit does not save Item Notes on initial add

Receiving
 - Prepacked Stock Create (sub feature)
 - Create, Edit (missing fields)
	- Product Information	- Carrier DDL
	- Batch/Lot Information - Shelf Life(readonly)
				- Remove Notice Date
	- Container Information - Location
	- Other Information	- Readonyl(Flammable, Freezer, refrigerated, other storage, restricted quantity/amount, )
				- Delete (received as name/code, return location, notice date)
 - CreateUnknown - leave in Recevied As Name/Code, return location

User Management
 - TBD
 - Implement Identity 2.0

Validation (data and system)
 - TBD
 - Pre-req - User mangement