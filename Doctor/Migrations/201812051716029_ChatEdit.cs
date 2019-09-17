namespace Doctor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChatEdit : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Chats", "DoctorsId", "dbo.Doctors");
            DropForeignKey("dbo.Chats", "PatientId", "dbo.Patients");
            DropIndex("dbo.Chats", new[] { "DoctorsId" });
            DropIndex("dbo.Chats", new[] { "PatientId" });
            AddColumn("dbo.Chats", "Sender", c => c.Int(nullable: false));
            AddColumn("dbo.Chats", "Reciver", c => c.Int(nullable: false));
            DropColumn("dbo.Chats", "DoctorsId");
            DropColumn("dbo.Chats", "PatientId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Chats", "PatientId", c => c.Int(nullable: false));
            AddColumn("dbo.Chats", "DoctorsId", c => c.Int(nullable: false));
            DropColumn("dbo.Chats", "Reciver");
            DropColumn("dbo.Chats", "Sender");
            CreateIndex("dbo.Chats", "PatientId");
            CreateIndex("dbo.Chats", "DoctorsId");
            AddForeignKey("dbo.Chats", "PatientId", "dbo.Patients", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Chats", "DoctorsId", "dbo.Doctors", "Id", cascadeDelete: true);
        }
    }
}
