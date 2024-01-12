namespace BilliardClub.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdjustTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "CreatedAt", c => c.DateTime(nullable: false));
            DropColumn("dbo.ProductReviews", "Title");
            DropColumn("dbo.ProductReviews", "Image");
            DropColumn("dbo.ProductReviews", "Star");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ProductReviews", "Star", c => c.Int(nullable: false));
            AddColumn("dbo.ProductReviews", "Image", c => c.String(nullable: false, maxLength: 500));
            AddColumn("dbo.ProductReviews", "Title", c => c.String(nullable: false, maxLength: 500));
            DropColumn("dbo.Products", "CreatedAt");
        }
    }
}
