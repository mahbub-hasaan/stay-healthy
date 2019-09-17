namespace Doctor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAgain : DbMigration
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
                        DoctorsId = c.Int(nullable: false),
                        PatientId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Doctors", t => t.DoctorsId, cascadeDelete: true)
                .ForeignKey("dbo.Patients", t => t.PatientId, cascadeDelete: true)
                .Index(t => t.DoctorsId)
                .Index(t => t.PatientId);
            
            CreateTable(
                "dbo.Doctors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
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
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Departments", t => t.DepartmentId, cascadeDelete: true)
                .Index(t => t.DepartmentId);
            
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
                "dbo.Patients",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PatientName = c.String(nullable: false),
                        PatientEmail = c.String(nullable: false),
                        PatientPhone = c.String(),
                        BirthDate = c.DateTime(),
                        PatientPassword = c.String(nullable: false),
                        EmailIsVerifed = c.Boolean(nullable: false),
                        ActivationCode = c.Guid(nullable: false),
                        ImagePath = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Appointments", "PatientId", "dbo.Patients");
            DropForeignKey("dbo.Appointments", "DoctorsId", "dbo.Doctors");
            DropForeignKey("dbo.Doctors", "DepartmentId", "dbo.Departments");
            DropIndex("dbo.Doctors", new[] { "DepartmentId" });
            DropIndex("dbo.Appointments", new[] { "PatientId" });
            DropIndex("dbo.Appointments", new[] { "DoctorsId" });
            DropTable("dbo.Patients");
            DropTable("dbo.Departments");
            DropTable("dbo.Doctors");
            DropTable("dbo.Appointments");
        }
    }
}
