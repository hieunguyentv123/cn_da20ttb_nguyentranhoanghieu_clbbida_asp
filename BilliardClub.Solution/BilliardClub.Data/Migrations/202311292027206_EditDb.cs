namespace BilliardClub.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EditDb : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Articles", "ImagePath", c => c.String(maxLength: 500));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Articles", "ImagePath", c => c.String(nullable: false, maxLength: 500));
        }
    }
}
