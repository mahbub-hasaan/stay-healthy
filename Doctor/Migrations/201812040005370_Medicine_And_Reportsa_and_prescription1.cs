namespace Doctor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Medicine_And_Reportsa_and_prescription1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Medicines", "Amount", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Medicines", "Amount", c => c.String(nullable: false));
        }
    }
}
