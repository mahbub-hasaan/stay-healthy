namespace Doctor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateAppointment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Appointments", "Weight", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Appointments", "Weight");
        }
    }
}
