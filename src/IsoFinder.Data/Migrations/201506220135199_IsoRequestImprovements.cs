namespace IsoFinder.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsoRequestImprovements : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.IsoRequest", "Completed", c => c.DateTime(nullable: false));
            AddColumn("dbo.IsoRequest", "FileName", c => c.String(maxLength: 255));
            AddColumn("dbo.IsoRequest", "Query", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.IsoRequest", "Query");
            DropColumn("dbo.IsoRequest", "FileName");
            DropColumn("dbo.IsoRequest", "Completed");
        }
    }
}
