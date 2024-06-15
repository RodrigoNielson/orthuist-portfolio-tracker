using FluentMigrator;

namespace Migrations.Migrations;

[Migration(2024_09_06_000000)]
public class Initial : Migration
{
    public override void Up()
    {
        Create.Table("Portfolio")
            .WithColumn("Id").AsGuid().PrimaryKey().Identity()
            .WithColumn("Name").AsString(50)
            .WithColumn("Created_At").AsDateTime().NotNullable()
            .WithColumn("Updated_At").AsDateTime().Nullable();
    }

    public override void Down()
    {
        Delete.Table("Portfolio");
    }
}