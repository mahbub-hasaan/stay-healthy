namespace Doctor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChatUpdate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Chats", "Sender", c => c.String(nullable: false));
            AlterColumn("dbo.Chats", "Reciver", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Chats", "Reciver", c => c.Int(nullable: false));
            AlterColumn("dbo.Chats", "Sender", c => c.Int(nullable: false));
        }
    }
}
