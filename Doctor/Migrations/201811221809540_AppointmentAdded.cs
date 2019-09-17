namespace Doctor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AppointmentAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Appointments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(),
                        runningMedicine = c.Boolean(nullable: false),
                        bp = c.String(),
                        Problem = c.String(nullable: false),
                        Status = c.Boolean(nullable: false),
                        Doctors_doctorId = c.Int(),
                        Patient_PatientId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Doctors", t => t.Doctors_doctorId)
                .ForeignKey("dbo.Patients", t => t.Patient_PatientId)
                .Index(t => t.Doctors_doctorId)
                .Index(t => t.Patient_PatientId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Appointments", "Patient_PatientId", "dbo.Patients");
            DropForeignKey("dbo.Appointments", "Doctors_doctorId", "dbo.Doctors");
            DropIndex("dbo.Appointments", new[] { "Patient_PatientId" });
            DropIndex("dbo.Appointments", new[] { "Doctors_doctorId" });
            DropTable("dbo.Appointments");
        }
    }
}
