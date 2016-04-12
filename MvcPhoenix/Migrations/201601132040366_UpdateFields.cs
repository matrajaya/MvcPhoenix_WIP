namespace MvcPhoenix.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateFields : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Customers", "AK_Customer_CompanyName");
            AddColumn("dbo.Customers", "PostalCode", c => c.String(nullable: false, maxLength: 10));
            AddColumn("dbo.Customers", "Country", c => c.String(nullable: false, maxLength: 50));
            AddColumn("dbo.Customers", "Fax", c => c.String(maxLength: 50));
            AlterColumn("dbo.Customers", "CompanyName", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Customers", "Address", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Customers", "City", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Customers", "State", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Customers", "Phone", c => c.String(maxLength: 50));
            CreateIndex("dbo.Customers", "CompanyName", unique: true, name: "AK_Customer_CompanyName");
            DropColumn("dbo.Customers", "ZipCode");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Customers", "ZipCode", c => c.String(nullable: false, maxLength: 10));
            DropIndex("dbo.Customers", "AK_Customer_CompanyName");
            AlterColumn("dbo.Customers", "Phone", c => c.String(maxLength: 15));
            AlterColumn("dbo.Customers", "State", c => c.String(nullable: false, maxLength: 2));
            AlterColumn("dbo.Customers", "City", c => c.String(nullable: false, maxLength: 15));
            AlterColumn("dbo.Customers", "Address", c => c.String(nullable: false, maxLength: 30));
            AlterColumn("dbo.Customers", "CompanyName", c => c.String(nullable: false, maxLength: 30));
            DropColumn("dbo.Customers", "Fax");
            DropColumn("dbo.Customers", "Country");
            DropColumn("dbo.Customers", "PostalCode");
            CreateIndex("dbo.Customers", "CompanyName", unique: true, name: "AK_Customer_CompanyName");
        }
    }
}
