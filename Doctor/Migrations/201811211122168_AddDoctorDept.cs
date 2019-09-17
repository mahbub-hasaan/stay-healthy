namespace Doctor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDoctorDept : DbMigration
    {
        public override void Up()
        {
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
                .PrimaryKey(t => t.doctorId)
                .ForeignKey("dbo.Departments", t => t.DepartmentId, cascadeDelete: true)
                .Index(t => t.DepartmentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Doctors", "DepartmentId", "dbo.Departments");
            DropIndex("dbo.Doctors", new[] { "DepartmentId" });
            DropTable("dbo.Doctors");
            DropTable("dbo.Departments");
        }
    }
}
