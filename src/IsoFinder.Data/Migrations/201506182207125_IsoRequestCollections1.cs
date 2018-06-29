namespace IsoFinder.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsoRequestCollections1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.IsoFile", "IsoRequest_Id", "dbo.IsoRequests");
            DropForeignKey("dbo.IsoFolder", "IsoRequest_Id", "dbo.IsoRequests");
            DropIndex("dbo.IsoFile", new[] { "IsoRequest_Id" });
            DropIndex("dbo.IsoFolder", new[] { "IsoRequest_Id" });
            CreateTable(
                "dbo.IsoFileRequest",
                c => new
                    {
                        IsoRequestId = c.Int(nullable: false),
                        IsoFileId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.IsoRequestId, t.IsoFileId })
                .ForeignKey("dbo.IsoRequests", t => t.IsoRequestId, cascadeDelete: true)
                .ForeignKey("dbo.IsoFile", t => t.IsoFileId, cascadeDelete: true)
                .Index(t => t.IsoRequestId)
                .Index(t => t.IsoFileId);
            
            CreateTable(
                "dbo.IsoFolderRequest",
                c => new
                    {
                        IsoRequestId = c.Int(nullable: false),
                        IsoFolderId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.IsoRequestId, t.IsoFolderId })
                .ForeignKey("dbo.IsoRequests", t => t.IsoRequestId, cascadeDelete: true)
                .ForeignKey("dbo.IsoFolder", t => t.IsoFolderId, cascadeDelete: true)
                .Index(t => t.IsoRequestId)
                .Index(t => t.IsoFolderId);
            
            DropColumn("dbo.IsoFile", "IsoRequest_Id");
            DropColumn("dbo.IsoFolder", "IsoRequest_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.IsoFolder", "IsoRequest_Id", c => c.Int());
            AddColumn("dbo.IsoFile", "IsoRequest_Id", c => c.Int());
            DropForeignKey("dbo.IsoFolderRequest", "IsoFolderId", "dbo.IsoFolder");
            DropForeignKey("dbo.IsoFolderRequest", "IsoRequestId", "dbo.IsoRequests");
            DropForeignKey("dbo.IsoFileRequest", "IsoFileId", "dbo.IsoFile");
            DropForeignKey("dbo.IsoFileRequest", "IsoRequestId", "dbo.IsoRequests");
            DropIndex("dbo.IsoFolderRequest", new[] { "IsoFolderId" });
            DropIndex("dbo.IsoFolderRequest", new[] { "IsoRequestId" });
            DropIndex("dbo.IsoFileRequest", new[] { "IsoFileId" });
            DropIndex("dbo.IsoFileRequest", new[] { "IsoRequestId" });
            DropTable("dbo.IsoFolderRequest");
            DropTable("dbo.IsoFileRequest");
            CreateIndex("dbo.IsoFolder", "IsoRequest_Id");
            CreateIndex("dbo.IsoFile", "IsoRequest_Id");
            AddForeignKey("dbo.IsoFolder", "IsoRequest_Id", "dbo.IsoRequests", "Id");
            AddForeignKey("dbo.IsoFile", "IsoRequest_Id", "dbo.IsoRequests", "Id");
        }
    }
}
