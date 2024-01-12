namespace BilliardClub.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Reinit : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "Address", c => c.String(nullable: false, maxLength: 4000));
            AddColumn("dbo.ContactInformations", "Address", c => c.String(nullable: false, maxLength: 4000));
            DropColumn("dbo.Users", "Adress");
            DropColumn("dbo.ContactInformations", "Adress");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ContactInformations", "Adress", c => c.String(nullable: false, maxLength: 4000));
            AddColumn("dbo.Users", "Adress", c => c.String(nullable: false, maxLength: 4000));
            DropColumn("dbo.ContactInformations", "Address");
            DropColumn("dbo.Users", "Address");
        }
    }
}
