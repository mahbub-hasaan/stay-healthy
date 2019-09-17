namespace Doctor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Unknown : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Appointments", "Doctors_doctorId", "dbo.Doctors");
            DropForeignKey("dbo.Appointments", "Patient_PatientId", "dbo.Patients");
            DropIndex("dbo.Appointments", new[] { "Doctors_doctorId" });
            DropIndex("dbo.Appointments", new[] { "Patient_PatientId" });
            AddColumn("dbo.Appointments", "Doctors_doctorId1", c => c.Int());
            AddColumn("dbo.Appointments", "Patient_PatientId1", c => c.Int());
            AlterColumn("dbo.Appointments", "Patient_PatientId", c => c.Int(nullable: false));
            CreateIndex("dbo.Appointments", "Doctors_doctorId1");
            CreateIndex("dbo.Appointments", "Patient_PatientId1");
            AddForeignKey("dbo.Appointments", "Doctors_doctorId1", "dbo.Doctors", "doctorId");
            AddForeignKey("dbo.Appointments", "Patient_PatientId1", "dbo.Patients", "PatientId");
            DropColumn("dbo.Appointments", "Doctor_doctorId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Appointments", "Doctor_doctorId", c => c.Int(nullable: false));
            DropForeignKey("dbo.Appointments", "Patient_PatientId1", "dbo.Patients");
            DropForeignKey("dbo.Appointments", "Doctors_doctorId1", "dbo.Doctors");
            DropIndex("dbo.Appointments", new[] { "Patient_PatientId1" });
            DropIndex("dbo.Appointments", new[] { "Doctors_doctorId1" });
            AlterColumn("dbo.Appointments", "Patient_PatientId", c => c.Int());
            DropColumn("dbo.Appointments", "Patient_PatientId1");
            DropColumn("dbo.Appointments", "Doctors_doctorId1");
            CreateIndex("dbo.Appointments", "Patient_PatientId");
            CreateIndex("dbo.Appointments", "Doctors_doctorId");
            AddForeignKey("dbo.Appointments", "Patient_PatientId", "dbo.Patients", "PatientId");
            AddForeignKey("dbo.Appointments", "Doctors_doctorId", "dbo.Doctors", "doctorId");
        }
    }
}
