namespace IsoFinder.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class First : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.IsoFile",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255),
                        Extension = c.String(nullable: false, maxLength: 255),
                        Created = c.DateTime(nullable: false),
                        Modified = c.DateTime(nullable: false),
                        Size = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Parent_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.IsoFolder", t => t.Parent_Id)
                .Index(t => t.Parent_Id);
            
            CreateTable(
                "dbo.IsoFolder",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255),
                        Path = c.String(nullable: false, maxLength: 255),
                        Parent_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.IsoFolder", t => t.Parent_Id)
                .Index(t => t.Parent_Id);
            
            CreateTable(
                "dbo.IsoVolume",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FileName = c.String(nullable: false, maxLength: 255),
                        VolumeLabel = c.String(nullable: false, maxLength: 255),
                        FileCount = c.Int(nullable: false),
                        Size = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Created = c.DateTime(nullable: false),
                        RootFolder_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.IsoFolder", t => t.RootFolder_Id)
                .Index(t => t.RootFolder_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.IsoVolume", "RootFolder_Id", "dbo.IsoFolder");
            DropForeignKey("dbo.IsoFolder", "Parent_Id", "dbo.IsoFolder");
            DropForeignKey("dbo.IsoFile", "Parent_Id", "dbo.IsoFolder");
            DropIndex("dbo.IsoVolume", new[] { "RootFolder_Id" });
            DropIndex("dbo.IsoFolder", new[] { "Parent_Id" });
            DropIndex("dbo.IsoFile", new[] { "Parent_Id" });
            DropTable("dbo.IsoVolume");
            DropTable("dbo.IsoFolder");
            DropTable("dbo.IsoFile");
        }
    }
}
