namespace MvcPhoenix.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserProfile : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "FaxNumber", c => c.String(maxLength: 50));
            AddColumn("dbo.AspNetUsers", "PostalCode", c => c.String(maxLength: 10));
            AddColumn("dbo.AspNetUsers", "Country", c => c.String(maxLength: 50));
            AlterColumn("dbo.AspNetUsers", "FirstName", c => c.String(maxLength: 50));
            AlterColumn("dbo.AspNetUsers", "LastName", c => c.String(maxLength: 50));
            AlterColumn("dbo.AspNetUsers", "Address", c => c.String(maxLength: 50));
            AlterColumn("dbo.AspNetUsers", "City", c => c.String(maxLength: 50));
            AlterColumn("dbo.AspNetUsers", "State", c => c.String(maxLength: 50));
            DropColumn("dbo.AspNetUsers", "ZipCode");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "ZipCode", c => c.String(maxLength: 10));
            AlterColumn("dbo.AspNetUsers", "State", c => c.String(maxLength: 2));
            AlterColumn("dbo.AspNetUsers", "City", c => c.String(maxLength: 20));
            AlterColumn("dbo.AspNetUsers", "Address", c => c.String(maxLength: 30));
            AlterColumn("dbo.AspNetUsers", "LastName", c => c.String(maxLength: 15));
            AlterColumn("dbo.AspNetUsers", "FirstName", c => c.String(maxLength: 15));
            DropColumn("dbo.AspNetUsers", "Country");
            DropColumn("dbo.AspNetUsers", "PostalCode");
            DropColumn("dbo.AspNetUsers", "FaxNumber");
        }
    }
}
