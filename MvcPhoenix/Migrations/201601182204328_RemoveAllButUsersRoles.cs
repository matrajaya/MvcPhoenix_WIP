namespace MvcPhoenix.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveAllButUsersRoles : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.InventoryItems", "CategoryId", "dbo.Categories");
            DropForeignKey("dbo.WorkOrders", "CurrentWorkerId", "dbo.AspNetUsers");
            DropForeignKey("dbo.WorkOrders", "CustomerId", "dbo.Customers");
            DropForeignKey("dbo.Labors", "WorkOrderID", "dbo.WorkOrders");
            DropForeignKey("dbo.Parts", "WorkOrderId", "dbo.WorkOrders");
            DropIndex("dbo.Categories", "AK_Category_CategoryName");
            DropIndex("dbo.InventoryItems", "AK_InventoryItem_InventoryItemCode");
            DropIndex("dbo.InventoryItems", "AK_InventoryItem_InventoryItemName");
            DropIndex("dbo.InventoryItems", new[] { "CategoryId" });
            DropIndex("dbo.Customers", "AK_Customer_AccountNumber");
            DropIndex("dbo.Customers", "AK_Customer_CompanyName");
            DropIndex("dbo.WorkOrders", new[] { "CustomerId" });
            DropIndex("dbo.WorkOrders", new[] { "CurrentWorkerId" });
            DropIndex("dbo.Labors", "AK_Labor");
            DropIndex("dbo.Parts", "AK_Part");
            DropIndex("dbo.ServiceItems", "AK_ServiceItem_ServiceItemCode");
            DropIndex("dbo.ServiceItems", "AK_ServiceItem_ServiceItemName");
            DropTable("dbo.Categories");
            DropTable("dbo.InventoryItems");
            DropTable("dbo.Customers");
            DropTable("dbo.WorkOrders");
            DropTable("dbo.Labors");
            DropTable("dbo.Parts");
            DropTable("dbo.ServiceItems");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ServiceItems",
                c => new
                    {
                        ServiceItemId = c.Int(nullable: false, identity: true),
                        ServiceItemCode = c.String(nullable: false, maxLength: 15),
                        ServiceItemName = c.String(nullable: false, maxLength: 80),
                        Rate = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ServiceItemId);
            
            CreateTable(
                "dbo.Parts",
                c => new
                    {
                        PartId = c.Int(nullable: false, identity: true),
                        WorkOrderId = c.Int(nullable: false),
                        InventoryItemCode = c.String(nullable: false, maxLength: 15),
                        InventoryItemName = c.String(nullable: false, maxLength: 80),
                        Quantity = c.Int(nullable: false),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ExtendedPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Notes = c.String(maxLength: 140),
                        IsInstalled = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.PartId);
            
            CreateTable(
                "dbo.Labors",
                c => new
                    {
                        LaborId = c.Int(nullable: false, identity: true),
                        WorkOrderID = c.Int(nullable: false),
                        ServiceItemCode = c.String(nullable: false, maxLength: 15),
                        ServiceItemName = c.String(nullable: false, maxLength: 80),
                        LaborHours = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Rate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ExtendedPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Notes = c.String(maxLength: 140),
                    })
                .PrimaryKey(t => t.LaborId);
            
            CreateTable(
                "dbo.WorkOrders",
                c => new
                    {
                        WorkOrderId = c.Int(nullable: false, identity: true),
                        CustomerId = c.Int(nullable: false),
                        OrderDateTime = c.DateTime(nullable: false),
                        TargetDateTime = c.DateTime(),
                        DropDeadDateTime = c.DateTime(),
                        Description = c.String(maxLength: 256),
                        WorkOrderStatus = c.Int(nullable: false),
                        Total = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CertificationRequirements = c.String(maxLength: 120),
                        CurrentWorkerId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.WorkOrderId);
            
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        CustomerId = c.Int(nullable: false, identity: true),
                        AccountNumber = c.String(nullable: false, maxLength: 8),
                        CompanyName = c.String(nullable: false, maxLength: 50),
                        Address = c.String(nullable: false, maxLength: 50),
                        City = c.String(nullable: false, maxLength: 50),
                        State = c.String(nullable: false, maxLength: 50),
                        PostalCode = c.String(nullable: false, maxLength: 10),
                        Country = c.String(nullable: false, maxLength: 50),
                        Phone = c.String(maxLength: 50),
                        Fax = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.CustomerId);
            
            CreateTable(
                "dbo.InventoryItems",
                c => new
                    {
                        InventoryItemId = c.Int(nullable: false, identity: true),
                        InventoryItemCode = c.String(nullable: false, maxLength: 15),
                        InventoryItemName = c.String(nullable: false, maxLength: 80),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CategoryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.InventoryItemId);
            
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        CategoryId = c.Int(nullable: false, identity: true),
                        CategoryName = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.CategoryId);
            
            CreateIndex("dbo.ServiceItems", "ServiceItemName", unique: true, name: "AK_ServiceItem_ServiceItemName");
            CreateIndex("dbo.ServiceItems", "ServiceItemCode", unique: true, name: "AK_ServiceItem_ServiceItemCode");
            CreateIndex("dbo.Parts", new[] { "WorkOrderId", "InventoryItemCode" }, unique: true, name: "AK_Part");
            CreateIndex("dbo.Labors", new[] { "WorkOrderID", "ServiceItemCode" }, unique: true, name: "AK_Labor");
            CreateIndex("dbo.WorkOrders", "CurrentWorkerId");
            CreateIndex("dbo.WorkOrders", "CustomerId");
            CreateIndex("dbo.Customers", "CompanyName", unique: true, name: "AK_Customer_CompanyName");
            CreateIndex("dbo.Customers", "AccountNumber", unique: true, name: "AK_Customer_AccountNumber");
            CreateIndex("dbo.InventoryItems", "CategoryId");
            CreateIndex("dbo.InventoryItems", "InventoryItemName", unique: true, name: "AK_InventoryItem_InventoryItemName");
            CreateIndex("dbo.InventoryItems", "InventoryItemCode", unique: true, name: "AK_InventoryItem_InventoryItemCode");
            CreateIndex("dbo.Categories", "CategoryName", unique: true, name: "AK_Category_CategoryName");
            AddForeignKey("dbo.Parts", "WorkOrderId", "dbo.WorkOrders", "WorkOrderId", cascadeDelete: true);
            AddForeignKey("dbo.Labors", "WorkOrderID", "dbo.WorkOrders", "WorkOrderId", cascadeDelete: true);
            AddForeignKey("dbo.WorkOrders", "CustomerId", "dbo.Customers", "CustomerId");
            AddForeignKey("dbo.WorkOrders", "CurrentWorkerId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.InventoryItems", "CategoryId", "dbo.Categories", "CategoryId");
        }
    }
}
