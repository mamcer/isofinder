namespace IsoFinder.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateRequired : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.IsoFile", "Parent_Id", "dbo.IsoFolder");
            DropForeignKey("dbo.IsoVolume", "RootFolder_Id", "dbo.IsoFolder");
            DropIndex("dbo.IsoFile", new[] { "Parent_Id" });
            DropIndex("dbo.IsoVolume", new[] { "RootFolder_Id" });
            AlterColumn("dbo.IsoFile", "Extension", c => c.String(maxLength: 255));
            AlterColumn("dbo.IsoFile", "Parent_Id", c => c.Int(nullable: false));
            AlterColumn("dbo.IsoVolume", "RootFolder_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.IsoFile", "Parent_Id");
            CreateIndex("dbo.IsoVolume", "RootFolder_Id");
            AddForeignKey("dbo.IsoFile", "Parent_Id", "dbo.IsoFolder", "Id", cascadeDelete: true);
            AddForeignKey("dbo.IsoVolume", "RootFolder_Id", "dbo.IsoFolder", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.IsoVolume", "RootFolder_Id", "dbo.IsoFolder");
            DropForeignKey("dbo.IsoFile", "Parent_Id", "dbo.IsoFolder");
            DropIndex("dbo.IsoVolume", new[] { "RootFolder_Id" });
            DropIndex("dbo.IsoFile", new[] { "Parent_Id" });
            AlterColumn("dbo.IsoVolume", "RootFolder_Id", c => c.Int());
            AlterColumn("dbo.IsoFile", "Parent_Id", c => c.Int());
            AlterColumn("dbo.IsoFile", "Extension", c => c.String(nullable: false, maxLength: 255));
            CreateIndex("dbo.IsoVolume", "RootFolder_Id");
            CreateIndex("dbo.IsoFile", "Parent_Id");
            AddForeignKey("dbo.IsoVolume", "RootFolder_Id", "dbo.IsoFolder", "Id");
            AddForeignKey("dbo.IsoFile", "Parent_Id", "dbo.IsoFolder", "Id");
        }
    }
}
