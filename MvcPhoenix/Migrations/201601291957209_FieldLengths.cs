namespace MvcPhoenix.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FieldLengths : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AspNetUsers", "FirstName", c => c.String(maxLength: 256));
            AlterColumn("dbo.AspNetUsers", "LastName", c => c.String(maxLength: 256));
            AlterColumn("dbo.AspNetUsers", "FaxNumber", c => c.String(maxLength: 256));
            AlterColumn("dbo.AspNetUsers", "Address", c => c.String(maxLength: 256));
            AlterColumn("dbo.AspNetUsers", "City", c => c.String(maxLength: 256));
            AlterColumn("dbo.AspNetUsers", "State", c => c.String(maxLength: 256));
            AlterColumn("dbo.AspNetUsers", "PostalCode", c => c.String(maxLength: 50));
            AlterColumn("dbo.AspNetUsers", "Country", c => c.String(maxLength: 256));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "Country", c => c.String(maxLength: 50));
            AlterColumn("dbo.AspNetUsers", "PostalCode", c => c.String(maxLength: 10));
            AlterColumn("dbo.AspNetUsers", "State", c => c.String(maxLength: 50));
            AlterColumn("dbo.AspNetUsers", "City", c => c.String(maxLength: 50));
            AlterColumn("dbo.AspNetUsers", "Address", c => c.String(maxLength: 50));
            AlterColumn("dbo.AspNetUsers", "FaxNumber", c => c.String(maxLength: 50));
            AlterColumn("dbo.AspNetUsers", "LastName", c => c.String(maxLength: 50));
            AlterColumn("dbo.AspNetUsers", "FirstName", c => c.String(maxLength: 50));
        }
    }
}
