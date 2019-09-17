namespace Doctor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPatient : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Patients",
                c => new
                    {
                        PatientId = c.Int(nullable: false, identity: true),
                        PatientName = c.String(nullable: false),
                        PatientEmail = c.String(nullable: false),
                        PatientPhone = c.String(),
                        BirthDate = c.DateTime(nullable: false),
                        PatientPassword = c.String(nullable: false),
                        EmailIsVerifed = c.Boolean(nullable: false),
                        ActivationCode = c.Guid(nullable: false),
                        ImagePath = c.String(),
                    })
                .PrimaryKey(t => t.PatientId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Patients");
        }
    }
}
