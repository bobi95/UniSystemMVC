namespace UniversitySystemMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GradeValueEnumAdded : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Grades", "GradeValue", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Grades", "GradeValue", c => c.String());
        }
    }
}
