namespace BilliardClub.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitDbbb : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "Fullname", c => c.String(nullable: false, maxLength: 256));
            AlterColumn("dbo.Orders", "Note", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Orders", "Note", c => c.String(nullable: false));
            DropColumn("dbo.Orders", "Fullname");
        }
    }
}
