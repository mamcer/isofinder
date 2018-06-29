namespace IsoFinder.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsoRequestUpdates : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.IsoRequests", "User_Id", "dbo.IsoUsers");
            DropIndex("dbo.IsoRequests", new[] { "User_Id" });
            AddColumn("dbo.IsoRequests", "Status", c => c.Int(nullable: false));
            AlterColumn("dbo.IsoRequests", "User_Id", c => c.Int(nullable: false));
            AlterColumn("dbo.IsoUsers", "Name", c => c.String(nullable: false, maxLength: 255));
            CreateIndex("dbo.IsoRequests", "User_Id");
            AddForeignKey("dbo.IsoRequests", "User_Id", "dbo.IsoUsers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.IsoRequests", "User_Id", "dbo.IsoUsers");
            DropIndex("dbo.IsoRequests", new[] { "User_Id" });
            AlterColumn("dbo.IsoUsers", "Name", c => c.String());
            AlterColumn("dbo.IsoRequests", "User_Id", c => c.Int());
            DropColumn("dbo.IsoRequests", "Status");
            CreateIndex("dbo.IsoRequests", "User_Id");
            AddForeignKey("dbo.IsoRequests", "User_Id", "dbo.IsoUsers", "Id");
        }
    }
}
