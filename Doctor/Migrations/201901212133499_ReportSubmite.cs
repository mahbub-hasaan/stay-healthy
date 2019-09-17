namespace Doctor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReportSubmite : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reports", "Status", c => c.Boolean(nullable: false));
            AddColumn("dbo.Reports", "ImagePath", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Reports", "ImagePath");
            DropColumn("dbo.Reports", "Status");
        }
    }
}
