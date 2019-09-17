namespace Doctor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChatAdd : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Chats",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MessageText = c.String(nullable: false),
                        DoctorsId = c.Int(nullable: false),
                        PatientId = c.Int(nullable: false),
                        time = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Doctors", t => t.DoctorsId, cascadeDelete: true)
                .ForeignKey("dbo.Patients", t => t.PatientId, cascadeDelete: true)
                .Index(t => t.DoctorsId)
                .Index(t => t.PatientId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Chats", "PatientId", "dbo.Patients");
            DropForeignKey("dbo.Chats", "DoctorsId", "dbo.Doctors");
            DropIndex("dbo.Chats", new[] { "PatientId" });
            DropIndex("dbo.Chats", new[] { "DoctorsId" });
            DropTable("dbo.Chats");
        }
    }
}
