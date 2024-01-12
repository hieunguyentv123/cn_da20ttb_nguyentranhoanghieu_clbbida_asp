namespace BilliardClub.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitDb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ArticleCategories",
                c => new
                    {
                        ArticleCatId = c.Int(nullable: false, identity: true),
                        ArticleCatName = c.String(nullable: false, maxLength: 256),
                        IsDisplay = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ArticleCatId);
            
            CreateTable(
                "dbo.Articles",
                c => new
                    {
                        ArticleId = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 500),
                        Content = c.String(nullable: false, maxLength: 4000),
                        VideoPath = c.String(maxLength: 4000),
                        ImagePath = c.String(nullable: false, maxLength: 500),
                        CreatedAt = c.DateTime(nullable: false),
                        CreateBy = c.Int(nullable: false),
                        CategoryId = c.Int(nullable: false),
                        IsDisplay = c.Boolean(nullable: false),
                        ViewCount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ArticleId)
                .ForeignKey("dbo.ArticleCategories", t => t.CategoryId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.CreateBy, cascadeDelete: true)
                .Index(t => t.CreateBy)
                .Index(t => t.CategoryId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        Username = c.String(nullable: false, maxLength: 256),
                        Password = c.String(nullable: false, maxLength: 256),
                        Fullname = c.String(nullable: false, maxLength: 256),
                        Email = c.String(nullable: false, maxLength: 256),
                        Address = c.String(nullable: false, maxLength: 4000),
                        Phone = c.String(maxLength: 10),
                        DOB = c.DateTime(nullable: false),
                        AvatarPath = c.String(maxLength: 4000),
                        IsAdministrator = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.ContactInformations",
                c => new
                    {
                        ContactInfoId = c.Int(nullable: false, identity: true),
                        Phone = c.String(maxLength: 10),
                        Address = c.String(nullable: false, maxLength: 4000),
                        IsDisplay = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ContactInfoId);
            
            CreateTable(
                "dbo.Contacts",
                c => new
                    {
                        ContactId = c.Int(nullable: false, identity: true),
                        ContactDetail = c.String(nullable: false, maxLength: 256),
                        Fullname = c.String(nullable: false, maxLength: 256),
                        Email = c.String(nullable: false, maxLength: 256),
                        Phone = c.String(maxLength: 10),
                        Message = c.String(nullable: false, maxLength: 4000),
                    })
                .PrimaryKey(t => t.ContactId);
            
            CreateTable(
                "dbo.OrderDetails",
                c => new
                    {
                        OrderId = c.Guid(nullable: false),
                        ProductId = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                        SinglePrice = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.OrderId, t.ProductId })
                .ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.OrderId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        OrderId = c.Guid(nullable: false),
                        Note = c.String(nullable: false),
                        OrderAt = c.DateTime(nullable: false),
                        OrderBy = c.Int(nullable: false),
                        PaymentId = c.Int(nullable: false),
                        IsPaid = c.Boolean(nullable: false),
                        TotalPrice = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.OrderId)
                .ForeignKey("dbo.Payments", t => t.PaymentId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.OrderBy, cascadeDelete: true)
                .Index(t => t.OrderBy)
                .Index(t => t.PaymentId);
            
            CreateTable(
                "dbo.Payments",
                c => new
                    {
                        PaymentId = c.Int(nullable: false, identity: true),
                        Method = c.String(nullable: false, maxLength: 500),
                        IsApproved = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.PaymentId);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        ProductId = c.Int(nullable: false, identity: true),
                        ProductName = c.String(nullable: false, maxLength: 500),
                        ProductDetail = c.String(nullable: false),
                        Price = c.Int(nullable: false),
                        DiscountPercent = c.Int(nullable: false),
                        Stock = c.Int(nullable: false),
                        IsDisplay = c.Boolean(nullable: false),
                        CategoryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProductId)
                .ForeignKey("dbo.ProductCategories", t => t.CategoryId, cascadeDelete: true)
                .Index(t => t.CategoryId);
            
            CreateTable(
                "dbo.ProductCategories",
                c => new
                    {
                        CategoryId = c.Int(nullable: false, identity: true),
                        CategoryName = c.String(nullable: false, maxLength: 256),
                        IsDisplay = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.CategoryId);
            
            CreateTable(
                "dbo.ProductReviews",
                c => new
                    {
                        ReviewId = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 500),
                        Content = c.String(nullable: false, maxLength: 4000),
                        Image = c.String(nullable: false, maxLength: 500),
                        ReviewAt = c.DateTime(nullable: false),
                        ReviewBy = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        Star = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ReviewId)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.ReviewBy, cascadeDelete: true)
                .Index(t => t.ReviewBy)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.Slides",
                c => new
                    {
                        SlideId = c.Int(nullable: false, identity: true),
                        SlidePath = c.String(nullable: false, maxLength: 4000),
                        OrderNum = c.Int(nullable: false),
                        IsDisplay = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.SlideId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderDetails", "ProductId", "dbo.Products");
            DropForeignKey("dbo.ProductReviews", "ReviewBy", "dbo.Users");
            DropForeignKey("dbo.ProductReviews", "ProductId", "dbo.Products");
            DropForeignKey("dbo.Products", "CategoryId", "dbo.ProductCategories");
            DropForeignKey("dbo.OrderDetails", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.Orders", "OrderBy", "dbo.Users");
            DropForeignKey("dbo.Orders", "PaymentId", "dbo.Payments");
            DropForeignKey("dbo.Articles", "CreateBy", "dbo.Users");
            DropForeignKey("dbo.Articles", "CategoryId", "dbo.ArticleCategories");
            DropIndex("dbo.ProductReviews", new[] { "ProductId" });
            DropIndex("dbo.ProductReviews", new[] { "ReviewBy" });
            DropIndex("dbo.Products", new[] { "CategoryId" });
            DropIndex("dbo.Orders", new[] { "PaymentId" });
            DropIndex("dbo.Orders", new[] { "OrderBy" });
            DropIndex("dbo.OrderDetails", new[] { "ProductId" });
            DropIndex("dbo.OrderDetails", new[] { "OrderId" });
            DropIndex("dbo.Articles", new[] { "CategoryId" });
            DropIndex("dbo.Articles", new[] { "CreateBy" });
            DropTable("dbo.Slides");
            DropTable("dbo.ProductReviews");
            DropTable("dbo.ProductCategories");
            DropTable("dbo.Products");
            DropTable("dbo.Payments");
            DropTable("dbo.Orders");
            DropTable("dbo.OrderDetails");
            DropTable("dbo.Contacts");
            DropTable("dbo.ContactInformations");
            DropTable("dbo.Users");
            DropTable("dbo.Articles");
            DropTable("dbo.ArticleCategories");
        }
    }
}
