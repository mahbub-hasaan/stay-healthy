namespace Doctor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Medicine_And_Reportsa_and_prescription : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Prescriptions", "DoctorsId", "dbo.Doctors");
            DropForeignKey("dbo.Prescriptions", "PatientId", "dbo.Patients");
            DropIndex("dbo.Prescriptions", new[] { "DoctorsId" });
            DropIndex("dbo.Prescriptions", new[] { "PatientId" });
            CreateTable(
                "dbo.Medicines",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Amount = c.String(nullable: false),
                        Dose = c.Int(nullable: false),
                        Day = c.Int(nullable: false),
                        Comment = c.String(nullable: false),
                        AppointmentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Appointments", t => t.AppointmentId, cascadeDelete: true)
                .Index(t => t.AppointmentId);
            
            CreateTable(
                "dbo.Reports",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        AppointmentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Appointments", t => t.AppointmentId, cascadeDelete: true)
                .Index(t => t.AppointmentId);
            
            AddColumn("dbo.Prescriptions", "AppointmentId", c => c.Int(nullable: false));
            CreateIndex("dbo.Prescriptions", "AppointmentId");
            AddForeignKey("dbo.Prescriptions", "AppointmentId", "dbo.Appointments", "Id", cascadeDelete: true);
            DropColumn("dbo.Prescriptions", "DoctorsId");
            DropColumn("dbo.Prescriptions", "PatientId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Prescriptions", "PatientId", c => c.Int(nullable: false));
            AddColumn("dbo.Prescriptions", "DoctorsId", c => c.Int(nullable: false));
            DropForeignKey("dbo.Reports", "AppointmentId", "dbo.Appointments");
            DropForeignKey("dbo.Prescriptions", "AppointmentId", "dbo.Appointments");
            DropForeignKey("dbo.Medicines", "AppointmentId", "dbo.Appointments");
            DropIndex("dbo.Reports", new[] { "AppointmentId" });
            DropIndex("dbo.Prescriptions", new[] { "AppointmentId" });
            DropIndex("dbo.Medicines", new[] { "AppointmentId" });
            DropColumn("dbo.Prescriptions", "AppointmentId");
            DropTable("dbo.Reports");
            DropTable("dbo.Medicines");
            CreateIndex("dbo.Prescriptions", "PatientId");
            CreateIndex("dbo.Prescriptions", "DoctorsId");
            AddForeignKey("dbo.Prescriptions", "PatientId", "dbo.Patients", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Prescriptions", "DoctorsId", "dbo.Doctors", "Id", cascadeDelete: true);
        }
    }
}
