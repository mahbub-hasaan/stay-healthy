namespace Doctor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class patienbloodgroup : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Patients", "BloodGroup", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Patients", "BloodGroup");
        }
    }
}
