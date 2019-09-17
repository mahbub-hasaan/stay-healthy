namespace Doctor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class rEMOVEALL : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Doctors", "DepartmentId", "dbo.Departments");
            DropForeignKey("dbo.Appointments", "Doctors_doctorId1", "dbo.Doctors");
            DropForeignKey("dbo.Appointments", "Patient_PatientId1", "dbo.Patients");
            DropIndex("dbo.Appointments", new[] { "Doctors_doctorId1" });
            DropIndex("dbo.Appointments", new[] { "Patient_PatientId1" });
            DropIndex("dbo.Doctors", new[] { "DepartmentId" });
            DropTable("dbo.Appointments");
            DropTable("dbo.Doctors");
            DropTable("dbo.Departments");
            DropTable("dbo.Patients");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Patients",
                c => new
                    {
                        PatientId = c.Int(nullable: false, identity: true),
                        PatientName = c.String(nullable: false),
                        PatientEmail = c.String(nullable: false),
                        PatientPhone = c.String(),
                        BirthDate = c.DateTime(),
                        PatientPassword = c.String(nullable: false),
                        EmailIsVerifed = c.Boolean(nullable: false),
                        ActivationCode = c.Guid(nullable: false),
                        ImagePath = c.String(),
                    })
                .PrimaryKey(t => t.PatientId);
            
            CreateTable(
                "dbo.Departments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        deptName = c.String(nullable: false),
                        deptDetail = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Doctors",
                c => new
                    {
                        doctorId = c.Int(nullable: false, identity: true),
                        doctorName = c.String(nullable: false),
                        doctorBirthDate = c.DateTime(nullable: false),
                        doctorEmail = c.String(nullable: false),
                        doctorImagePath = c.String(),
                        doctorDetails = c.String(nullable: false, maxLength: 200),
                        doctorDegree = c.String(nullable: false, maxLength: 200),
                        doctorPassword = c.String(nullable: false, maxLength: 200),
                        regNo = c.String(nullable: false),
                        IsEmailVarifide = c.Boolean(nullable: false),
                        ActivationCode = c.Guid(nullable: false),
                        DepartmentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.doctorId);
            
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
                        Doctors_doctorId = c.Int(nullable: false),
                        Patient_PatientId = c.Int(nullable: false),
                        Doctors_doctorId1 = c.Int(),
                        Patient_PatientId1 = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.Doctors", "DepartmentId");
            CreateIndex("dbo.Appointments", "Patient_PatientId1");
            CreateIndex("dbo.Appointments", "Doctors_doctorId1");
            AddForeignKey("dbo.Appointments", "Patient_PatientId1", "dbo.Patients", "PatientId");
            AddForeignKey("dbo.Appointments", "Doctors_doctorId1", "dbo.Doctors", "doctorId");
            AddForeignKey("dbo.Doctors", "DepartmentId", "dbo.Departments", "Id", cascadeDelete: true);
        }
    }
}
