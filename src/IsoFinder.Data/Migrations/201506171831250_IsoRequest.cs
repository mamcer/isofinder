namespace IsoFinder.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsoRequest : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.IsoRequests",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Created = c.DateTime(nullable: false),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.IsoUsers", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.IsoUsers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Created = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.IsoRequests", "User_Id", "dbo.IsoUsers");
            DropIndex("dbo.IsoRequests", new[] { "User_Id" });
            DropTable("dbo.IsoUsers");
            DropTable("dbo.IsoRequests");
        }
    }
}
