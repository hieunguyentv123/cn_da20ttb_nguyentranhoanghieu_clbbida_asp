namespace BilliardClub.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteSlideOrderNum : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Slides", "OrderNum");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Slides", "OrderNum", c => c.Int(nullable: false));
        }
    }
}
