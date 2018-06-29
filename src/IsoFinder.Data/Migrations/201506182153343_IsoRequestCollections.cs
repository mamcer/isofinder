namespace IsoFinder.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsoRequestCollections : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.IsoFile", "IsoRequest_Id", c => c.Int());
            AddColumn("dbo.IsoFolder", "IsoRequest_Id", c => c.Int());
            CreateIndex("dbo.IsoFile", "IsoRequest_Id");
            CreateIndex("dbo.IsoFolder", "IsoRequest_Id");
            AddForeignKey("dbo.IsoFile", "IsoRequest_Id", "dbo.IsoRequests", "Id");
            AddForeignKey("dbo.IsoFolder", "IsoRequest_Id", "dbo.IsoRequests", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.IsoFolder", "IsoRequest_Id", "dbo.IsoRequests");
            DropForeignKey("dbo.IsoFile", "IsoRequest_Id", "dbo.IsoRequests");
            DropIndex("dbo.IsoFolder", new[] { "IsoRequest_Id" });
            DropIndex("dbo.IsoFile", new[] { "IsoRequest_Id" });
            DropColumn("dbo.IsoFolder", "IsoRequest_Id");
            DropColumn("dbo.IsoFile", "IsoRequest_Id");
        }
    }
}
