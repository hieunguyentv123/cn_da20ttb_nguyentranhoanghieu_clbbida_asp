namespace BilliardClub.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDataFilter : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductCategories", "DataFilter", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductCategories", "DataFilter");
        }
    }
}
