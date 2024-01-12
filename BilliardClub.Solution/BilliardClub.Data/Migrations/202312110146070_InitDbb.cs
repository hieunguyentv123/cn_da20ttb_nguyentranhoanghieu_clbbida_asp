namespace BilliardClub.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitDbb : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "Address", c => c.String(nullable: false, maxLength: 500));
            AddColumn("dbo.Orders", "Phone", c => c.String(maxLength: 10));
            AddColumn("dbo.Orders", "Email", c => c.String(nullable: false, maxLength: 256));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "Email");
            DropColumn("dbo.Orders", "Phone");
            DropColumn("dbo.Orders", "Address");
        }
    }
}
