namespace Doctor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Pe : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Prescriptions", "Advice", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Prescriptions", "Advice", c => c.String());
        }
    }
}
