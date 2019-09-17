namespace Doctor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPatient_2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Patients", "BirthDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Patients", "BirthDate", c => c.DateTime(nullable: false));
        }
    }
}
