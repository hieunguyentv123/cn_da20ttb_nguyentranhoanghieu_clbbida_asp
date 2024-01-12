namespace BilliardClub.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdjustContact : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Contacts", "SendAt", c => c.DateTime(nullable: false));
            AddColumn("dbo.Contacts", "IsReplied", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Contacts", "IsReplied");
            DropColumn("dbo.Contacts", "SendAt");
        }
    }
}
