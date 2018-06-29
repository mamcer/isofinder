namespace IsoFinder.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsoVolumeOnEntities : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.IsoRequests", newName: "IsoRequest");
            AddColumn("dbo.IsoFile", "IsoVolumeId", c => c.Int(nullable: false));
            AddColumn("dbo.IsoFolder", "IsoVolumeId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.IsoFolder", "IsoVolumeId");
            DropColumn("dbo.IsoFile", "IsoVolumeId");
            RenameTable(name: "dbo.IsoRequest", newName: "IsoRequests");
        }
    }
}
